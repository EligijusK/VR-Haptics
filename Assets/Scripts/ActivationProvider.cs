using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using WeArt.Components;
using WeArt.Core;
using WeArt.GestureInteractions.Gestures;
using WeArt.GestureInteractions.Utils;
using WeArt.Utils;

namespace WeArt.GestureInteractions
{
    public class ActivationProvider : MonoBehaviour
    {
        [Header("Activation Settings")]
        [SerializeField] internal HandSide activationHandSide;
        [SerializeField] internal GestureName prepareGesture;
        [SerializeField] internal GestureName launchGesture;
        [SerializeField] internal float recognitionDelay = 1f;

        [Header("Laser Settings")]
        [SerializeField] internal Transform laserOriginLeft;
        [SerializeField] internal Transform laserOriginRight;

        [Header("Grace & Sticky Settings")]
        [SerializeField] private float graceDelay = 0.1f;
        [SerializeField] private float stickyDuration = 0.3f;
        [SerializeField] private float laserSmoothing = 5f;

        [Header("HandCleaning Drag Settings")]
        [SerializeField] private float dragMultiplier = 2f; // 2x distance multiplier

        private WeArtHandController _handController;
        private LineRenderer _laser;
        private const int LaserSteps = 50;
        private const float LaserSegmentDistance = 0.25f;
        private const float DropPerSegment = 0.025f;
        private float _recognizeTimer;
        private Transform _laserOrigin;
        private Vector3 _targetPos;
        private XRSimpleInteractable _targetInteractable;
        private XRSimpleInteractable _currentHoveredInteractable;
        private XRSimpleInteractable _lastValidInteractable;
        private float _lastValidHoveredTime = -1f;

        private bool _isActivating;
        private bool _hasExclusiveActivation;
        private WaitForSeconds _reloadWaiter;
        private Coroutine _gracePeriodCoroutine;
        private float _stickyTimer = 0f;
        private Vector3 _stickyEndpoint;

        // HandCleaning tracking
        private bool _isTrackingHandCleaning = false;
        private OrderElement _currentOrderElement;
        private bool _wasLaunchGestureHeld = false;
        private Vector3 _dragOffset;
        private Transform _dragTargetTransform;

        private void Awake() => InitVariables();

        private void Start()
        {
            _handController = WeArtController.Instance.GetHandController(activationHandSide);
            if (_handController == null)
            {
                Debug.LogError("ActivationProvider: Hand controller not found.");
                gameObject.SetActive(false);
                return;
            }
        }

        private void Update()
        {
            CheckActivationGesture();

            if (_stickyTimer > 0 && _laser.enabled)
            {
                Vector3 currentEndpoint = _laser.GetPosition(_laser.positionCount - 1);
                Vector3 smoothed = Vector3.Lerp(currentEndpoint, _stickyEndpoint, laserSmoothing * Time.deltaTime);
                _laser.SetPosition(_laser.positionCount - 1, smoothed);
            }

            bool launchHeld = GestureRecognizer.CheckMatchGesture(launchGesture, _handController);

            if (launchHeld && !_isActivating)
            {
                if (_lastValidInteractable != null &&
                    (Time.time - _lastValidHoveredTime <= 2f) &&
                    _lastValidInteractable.CompareTag("HandCleaning"))
                {
                    Debug.Log("HandCleaning: Holding launch gesture on valid object.");

                    if (!_isTrackingHandCleaning)
                    {
                        _currentOrderElement = _lastValidInteractable.GetComponent<OrderElement>();
                        if (_currentOrderElement != null)
                        {
                            _isTrackingHandCleaning = true;
                            _laser.enabled = true;
                            _stickyTimer = stickyDuration;
                            _currentOrderElement.StartTracking();
                            _dragTargetTransform = _lastValidInteractable.transform;
                            _dragOffset = _dragTargetTransform.position - _handController.transform.position;
                            Debug.Log("HandCleaning: StartTracking() called and drag initialized.");
                        }
                        else
                        {
                            Debug.LogWarning("HandCleaning: Object is missing OrderElement component.");
                        }
                    }

                    // Apply amplified horizontal dragging
                    if (_isTrackingHandCleaning && _dragTargetTransform != null)
                    {
                        Vector3 handPos = _handController.transform.position;
                        float handX = handPos.x + (_dragOffset.x * dragMultiplier);
                        Vector3 newPosition = new Vector3(handX, _dragTargetTransform.position.y, _dragTargetTransform.position.z);
                        _dragTargetTransform.position = newPosition;
                        Debug.Log("HandCleaning: Dragging to X=" + newPosition.x);
                    }
                }
                else if (_lastValidInteractable != null)
                {
                    _targetInteractable = _lastValidInteractable;
                    ActivateTarget();
                    _lastValidInteractable = null;
                }
            }

            if (!launchHeld && _wasLaunchGestureHeld && _isTrackingHandCleaning)
            {
                if (_currentOrderElement != null)
                {
                    Debug.Log("HandCleaning: Releasing launch gesture, calling CheckIfCorrect().");
                    _currentOrderElement.CheckIfCorrect();
                }

                _isTrackingHandCleaning = false;
                _currentOrderElement = null;
                _dragTargetTransform = null;
                _dragOffset = Vector3.zero;
                DisableActivationTools();
            }

            _wasLaunchGestureHeld = launchHeld;
        }

        private void CheckActivationGesture()
        {
            if (GestureRecognizer.CheckMatchGesture(prepareGesture, _handController))
            {
                if (_gracePeriodCoroutine != null)
                {
                    StopCoroutine(_gracePeriodCoroutine);
                    _gracePeriodCoroutine = null;
                }

                if (!_hasExclusiveActivation && !BeginActivation())
                    return;

                _recognizeTimer += Time.deltaTime;
                if (_recognizeTimer < recognitionDelay)
                    return;

                if (!_laser.enabled)
                    _laser.enabled = true;

                CheckActivationConditions();
            }
            else
            {
                if (_gracePeriodCoroutine == null)
                    _gracePeriodCoroutine = StartCoroutine(GracePeriodCoroutine());
            }
        }

        private bool BeginActivation()
        {
            if (!CheckActivationRequirements())
                return false;

            _hasExclusiveActivation = true;
            return true;
        }

        private void EndActivation() => _hasExclusiveActivation = false;

        private IEnumerator GracePeriodCoroutine()
        {
            yield return new WaitForSeconds(graceDelay);

            if (!GestureRecognizer.CheckMatchGesture(prepareGesture, _handController))
            {
                _recognizeTimer = 0f;
                DisableActivationTools();
                EndActivation();
            }

            _gracePeriodCoroutine = null;
        }

        private bool CheckActivationRequirements()
        {
            if (!WeArtController.Instance._allowGestures)
            {
                DisableActivationTools();
                return false;
            }

            if (_handController.GetGraspingSystem().GraspingState == GraspingState.Grabbed)
            {
                DisableActivationTools();
                return false;
            }

            return !_isActivating;
        }

        private void CheckActivationConditions()
        {
            if (!CheckActivationRequirements())
                return;

            Vector3 origin = _laserOrigin.position;
            _laser.SetPosition(0, origin);
            bool validTargetFound = false;
            _targetInteractable = null;

            for (int i = 0; i < LaserSteps - 1; i++)
            {
                Vector3 offset = (_laserOrigin.forward + (Vector3.down * (DropPerSegment * i))).normalized * LaserSegmentDistance;
                if (Physics.Raycast(origin, offset, out RaycastHit hit, LaserSegmentDistance))
                {
                    for (int j = i + 1; j < _laser.positionCount; j++)
                        _laser.SetPosition(j, hit.point);

                    XRSimpleInteractable interactable = hit.transform.GetComponent<XRSimpleInteractable>();
                    if (interactable != null)
                    {
                        SetLaserColor(Color.green);
                        _targetPos = hit.point;
                        _targetInteractable = interactable;
                        validTargetFound = true;
                        _stickyEndpoint = hit.point;
                        _stickyTimer = stickyDuration;
                        _lastValidHoveredTime = Time.time;
                        _lastValidInteractable = _targetInteractable;

                        if (_targetInteractable != _currentHoveredInteractable)
                        {
                            _currentHoveredInteractable?.hoverExited.Invoke(new HoverExitEventArgs { interactorObject = null });
                            _currentHoveredInteractable = _targetInteractable;
                            _currentHoveredInteractable.hoverEntered.Invoke(new HoverEnterEventArgs { interactorObject = null });
                        }

                        if (_targetInteractable.CompareTag("HandCleaning"))
                        {
                            Debug.Log("HandCleaning: Hovered over valid HandCleaning object.");
                        }
                    }
                    else
                    {
                        ShowNoActivation();
                        return;
                    }

                    break;
                }
                else
                {
                    Vector3 nextPoint = origin + offset;
                    _laser.SetPosition(i + 1, nextPoint);
                    origin = nextPoint;
                }
            }

            if (!validTargetFound)
            {
                _currentHoveredInteractable?.hoverExited.Invoke(new HoverExitEventArgs { interactorObject = null });
                _currentHoveredInteractable = null;
                ShowNoActivation();
            }
        }

        private void ActivateTarget()
        {
            if (_targetInteractable != null)
            {
                _isActivating = true;
                var target = _targetInteractable;

                ResetLaser();

                if (target.activated == null)
                {
                    target.activated = new ActivateEvent();
                    Debug.LogWarning("Activated event was null; it has been initialized.");
                }

                target.activated.Invoke(new ActivateEventArgs { interactorObject = null });

                DisableActivationTools();
                StartCoroutine(ActivationReloadCor());
                EndActivation();

                _lastValidInteractable = null;
            }
            else
            {
                Debug.LogError("ActivateTarget: No target interactable available.");
            }
        }

        private IEnumerator ActivationReloadCor()
        {
            if (_reloadWaiter == null)
                _reloadWaiter = new WaitForSeconds(0.5f);
            yield return _reloadWaiter;
            _isActivating = false;
        }

        private void SetLaserColor(Color color)
        {
            _laser.startColor = color;
            _laser.endColor = color;
        }

        private void ShowNoActivation()
        {
            SetLaserColor(Color.red);
            _targetInteractable = null;
            _currentHoveredInteractable?.hoverExited.Invoke(new HoverExitEventArgs { interactorObject = null });
            _currentHoveredInteractable = null;
        }

        private void ResetLaser()
        {
            for (int i = 0; i < _laser.positionCount; i++)
                _laser.SetPosition(i, Vector3.zero);
        }

        private void DisableActivationTools()
        {
            if (_laser.enabled)
                _laser.enabled = false;

            _targetInteractable = null;
            _currentHoveredInteractable?.hoverExited.Invoke(new HoverExitEventArgs { interactorObject = null });
            _currentHoveredInteractable = null;
        }

        private void InitVariables()
        {
            _laserOrigin = (activationHandSide == HandSide.Right) ? laserOriginRight : laserOriginLeft;
            _laser = GetComponent<LineRenderer>();
            if (_laser != null)
                _laser.positionCount = LaserSteps;
        }
    }
}
