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
    /// <summary>
    /// ActivationProvider uses gesture recognition to display a laser pointer from the selected hand.
    /// When the launch gesture is performed—instantly, regardless of prepare state—if a valid target is being hovered
    /// or was hovered within the last 1 second, the interactable is activated.
    /// </summary>
    public class ActivationProvider : MonoBehaviour
    {
        [Header("Activation Settings")]
        [Tooltip("Hand (left/right) used to control activation gestures.")]
        [SerializeField] internal HandSide activationHandSide;
        [Tooltip("Gesture that prepares the activation laser.")]
        [SerializeField] internal GestureName prepareGesture;
        [Tooltip("Gesture that activates the target interactable.")]
        [SerializeField] internal GestureName launchGesture;
        [Tooltip("Time (in seconds) to wait while the prepare gesture is held before showing the laser.")]
        [SerializeField] internal float recognitionDelay = 1f;

        [Header("Laser Settings")]
        [Tooltip("Laser origin transform for the left hand.")]
        [SerializeField] internal Transform laserOriginLeft;
        [Tooltip("Laser origin transform for the right hand.")]
        [SerializeField] internal Transform laserOriginRight;

        [Header("Grace & Sticky Settings")]
        [Tooltip("Delay after the prepare gesture is released before disabling the laser.")]
        [SerializeField] private float graceDelay = 0.1f;
        [Tooltip("Time (in seconds) for which the laser remains stuck to the last valid interactable.")]
        [SerializeField] private float stickyDuration = 0.3f;
        [Tooltip("Smoothing factor for laser endpoint movement.")]
        [SerializeField] private float laserSmoothing = 5f;

        // Private variables
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
        private bool _isActivating;
        private bool _hasExclusiveActivation;
        private WaitForSeconds _reloadWaiter;
        private Coroutine _gracePeriodCoroutine;
        private float _stickyTimer = 0f;
        private Vector3 _stickyEndpoint;
        // Records time of the last valid hover.
        private float _lastValidHoveredTime = -1f;

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

            // Smoothly adjust the laser endpoint toward the sticky endpoint when active.
            if (_stickyTimer > 0 && _laser.enabled)
            {
                Vector3 currentEndpoint = _laser.GetPosition(_laser.positionCount - 1);
                Vector3 smoothed = Vector3.Lerp(currentEndpoint, _stickyEndpoint, laserSmoothing * Time.deltaTime);
                _laser.SetPosition(_laser.positionCount - 1, smoothed);
            }

            // Check for launch gesture instantly regardless of prepare state.
            if (!_isActivating && GestureRecognizer.CheckMatchGesture(launchGesture, _handController))
            {
                // Use the current hovered target if it was valid within the last second.
                if (_currentHoveredInteractable != null && (Time.time - _lastValidHoveredTime <= 1f))
                {
                    _targetInteractable = _currentHoveredInteractable;
                    ActivateTarget();
                }
            }
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
                if (!_hasExclusiveActivation)
                {
                    if (!BeginActivation())
                        return;
                }
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
            if (_isActivating)
                return false;
            return true;
        }

        private void CheckActivationConditions()
        {
            if (!CheckActivationRequirements())
                return;
            Vector3 origin = _laserOrigin.position;
            _laser.SetPosition(0, origin);
            bool validTargetFound = false;
            int steps = LaserSteps - 1;
            _targetInteractable = null;
            // Perform raycast segments.
            for (int i = 0; i < steps; i++)
            {
                Vector3 offset = (_laserOrigin.forward + (Vector3.down * (DropPerSegment * i))).normalized * LaserSegmentDistance;
                if (Physics.Raycast(origin, offset, out RaycastHit hit, LaserSegmentDistance))
                {
                    for (int j = i + 1; j < _laser.positionCount; j++)
                        _laser.SetPosition(j, hit.point);
                    XRSimpleInteractable interactable = hit.transform.GetComponent<XRSimpleInteractable>();
                    if (interactable != null)
                    {
                        if (hit.normal.y < 1)
                        {
                            ShowNoActivation();
                            return;
                        }
                        SetLaserColor(Color.green);
                        _targetPos = hit.point;
                        _targetInteractable = interactable;
                        validTargetFound = true;
                        _stickyEndpoint = hit.point;
                        _stickyTimer = stickyDuration;
                        _lastValidHoveredTime = Time.time;
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
            // If no valid hit this frame, use sticky mode if available.
            if (!validTargetFound && _currentHoveredInteractable != null && _stickyTimer > 0)
            {
                validTargetFound = true;
                _targetInteractable = _currentHoveredInteractable;
                _targetPos = _stickyEndpoint;
                _stickyTimer -= Time.deltaTime;
            }
            // Process hover events.
            if (validTargetFound)
            {
                if (_targetInteractable != _currentHoveredInteractable)
                {
                    if (_currentHoveredInteractable != null)
                        _currentHoveredInteractable.hoverExited.Invoke(new HoverExitEventArgs { interactorObject = null });
                    _currentHoveredInteractable = _targetInteractable;
                    _currentHoveredInteractable.hoverEntered.Invoke(new HoverEnterEventArgs { interactorObject = null });
                }
            }
            else
            {
                if (_currentHoveredInteractable != null)
                {
                    _currentHoveredInteractable.hoverExited.Invoke(new HoverExitEventArgs { interactorObject = null });
                    _currentHoveredInteractable = null;
                }
                ShowNoActivation();
            }
        }

        private void ActivateTarget()
        {
            if (_targetInteractable != null)
            {
                _isActivating = true;
                // Store the current target in a local variable.
                XRSimpleInteractable target = _targetInteractable;
        
                // Optionally, call ResetLaser() before clearing the global variable.
                ResetLaser();

                ActivateEventArgs args = new ActivateEventArgs { interactorObject = null };

                // Ensure the activated event is initialized.
                if (target.activated == null)
                {
                    target.activated = new ActivateEvent();
                    Debug.LogWarning("Activated event was null; it has been initialized. Make sure it is set up properly in the Inspector or component.");
                }
        
                // Invoke the activation on the local copy.
                target.activated.Invoke(args);

                // Now disable the activation tools; this clears the globals without affecting our local reference.
                DisableActivationTools();

                StartCoroutine(ActivationReloadCor());
                EndActivation();
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
            if (_currentHoveredInteractable != null)
            {
                _currentHoveredInteractable.hoverExited.Invoke(new HoverExitEventArgs { interactorObject = null });
                _currentHoveredInteractable = null;
            }
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
            if (_currentHoveredInteractable != null)
            {
                _currentHoveredInteractable.hoverExited.Invoke(new HoverExitEventArgs { interactorObject = null });
                _currentHoveredInteractable = null;
            }
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
