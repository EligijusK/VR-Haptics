using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using UnityEngine.Playables;
using WeArt.Core;
using Texture = WeArt.Core.Texture;

using static WeArt.Components.WeArtTouchableObject;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace WeArt.Components
{
    /// <summary>
    /// This component is able to animate a virtual hand using closure data coming from
    /// a set of <see cref="WeArtThimbleTrackingObject"/> components.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class WeArtHandController : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// Defines the _openedHandState.
        /// </summary>
        [SerializeField]
        internal AnimationClip _openedHandState;

        /// <summary>
        /// Defines the _closedHandState.
        /// </summary>
        [SerializeField]
        internal AnimationClip _closedHandState;

        /// <summary>
        /// Defines the _abductionHandState.
        /// </summary>
        [SerializeField]
        internal AnimationClip _abductionHandState;

        /// <summary>
        /// Defines the _thumbMask, _indexMask, _middleMask, _ringMask, _pinkyMask.
        /// </summary>
        [SerializeField]
        internal AvatarMask _thumbMask, _indexMask, _middleMask, _ringMask, _pinkyMask;

        /// <summary>
        /// Defines the _thumbThimble, _indexThimble, _middleThimble.
        /// </summary>
        [SerializeField]
        internal WeArtThimbleTrackingObject _thumbThimbleTracking, _indexThimbleTracking, _middleThimbleTracking;

        [SerializeField]
        internal WeArtHapticObject _thumbThimbleHaptic, _indexThimbleHaptic, _middleThimbleHaptic, _palmThimbleHaptic;

        [SerializeField]
        internal WeArtGraspCheck _thumbGraspCheckHaptic, _indexGraspCheckHaptic, _middleGraspCheckHaptic;

        [SerializeField]
        internal Transform _realThumbPosition, _realIndexPosition, _realMiddlePosition;

        [SerializeField]
        internal Transform _thumbExplorationOrigin, _indexExplorationOrigin, _middleExplorationOrigin, _palmExplorationOrigin;

        [SerializeField]
        internal SkinnedMeshRenderer _handSkinnedMeshRenderer;

        [SerializeField]
        internal GameObject _logoHandPanel;

        [SerializeField]
        internal Material _ghostHandInvisibleMaterial;

        [SerializeField]
        internal Material _ghostHandTransparentMaterial;

        [SerializeField]
        internal GameObject _grasper;

        /// <summary>
        /// If you use custom poses, you have to add WeArtGraspPose.cs
        /// </summary>
        [SerializeField]
        internal bool _useCustomPoses;

        /// <summary>
        /// Defines the _animator.
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// Defines the _fingers.
        /// </summary>
        private AvatarMask[] _fingers;

        /// <summary>
        /// Defines the _thimbles.
        /// </summary>
        private WeArtThimbleTrackingObject[] _thimbles;

        private PlayableGraph _graph;

        private AnimationLayerMixerPlayable[] _fingersMixers;

        private WeArtTouchableObject _touchableObject;

        private float _slowFingerAnimationTime;

        private float _slowFingerAnimationTimeMax = 1f;

        private GraspingState _graspingState = GraspingState.Released;

        private readonly WeArtTouchEffect _thumbGraspingEffect = new WeArtTouchEffect();

        private readonly WeArtTouchEffect _indexGraspingEffect = new WeArtTouchEffect();

        private readonly WeArtTouchEffect _middleGraspingEffect = new WeArtTouchEffect();

        /// <summary>
        /// Define the hand side
        /// </summary>
        [SerializeField]
        internal HandSide _handSide;

        public delegate void GraspingDelegate(HandSide handSide, GameObject gameObject);
        public GraspingDelegate OnGraspingEvent;
        public GraspingDelegate OnReleaseEvent;


        // Finger colliders containers
        private List<WeArtTouchableObject> _thumbContactTouchables = new List<WeArtTouchableObject>();
        private List<WeArtTouchableObject> _indexContactTouchables = new List<WeArtTouchableObject>();
        private List<WeArtTouchableObject> _middleContactTouchables = new List<WeArtTouchableObject>();
        private List<WeArtTouchableObject> _palmContactTouchables = new List<WeArtTouchableObject>();

        private List<WeArtTouchableObject> _thumbGraspCheckTouchables = new List<WeArtTouchableObject>();
        private List<WeArtTouchableObject> _indexGraspCheckTouchables = new List<WeArtTouchableObject>();
        private List<WeArtTouchableObject> _middleGraspCheckTouchables = new List<WeArtTouchableObject>();

        private List<WeArtTouchableObject> _thumbBlockingTouchables = new List<WeArtTouchableObject>();
        private List<WeArtTouchableObject> _indexBlockingTouchables = new List<WeArtTouchableObject>();
        private List<WeArtTouchableObject> _middleBlockingTouchables = new List<WeArtTouchableObject>();

        private float _fingersAnimationSpeed = 3f;
        private float _fingersSlideSpeed = 1.5f;
        private float _extraFingerSpeed = 10f;
        private float _safeUnlockFrames = 0.2f;
        private float _intentionalGraspThreshold = 0.1f;
        private float _tryingToGraspThreshhold = 0.1f;
        private const float _dynamicForceSensitivity = 0.05f;

        private bool _isThumbTryingToGrasp = false;
        private bool _isIndexTryingToGrasp = false;
        private bool _isMiddleTryingToGrasp = false;

        private bool _isPalmGrasping = false;

        private Vector3 _thumbHapticSize;
        private Vector3 _indexHapticSize;
        private Vector3 _middleHapticSize;

        private Vector3 _thumbHapticPosition;
        private Vector3 _indexHapticPosition;
        private Vector3 _middleHapticPosition;

        private Vector3 _thumbHapticExplorationPosition;
        private Vector3 _indexHapticExplorationPosition;
        private Vector3 _middleHapticExplorationPosition;

        [SerializeField]
        internal Transform _explorationThumbHapticTransform;
        [SerializeField]
        internal Transform _explorationIndexHapticTransform;
        [SerializeField]
        internal Transform _explorationMiddleHapticTransform;

        private WeArtHandController _otherHand;
        private float _notAllowedToGrabSeconds = 0f;
        private float _notAllowedToGrabSecondsMax = 1f;
        #endregion

        #region Methods

        /// <summary>
        /// Initial set up
        /// </summary>
        private void Awake()
        {

            // Setup animation components
            _animator = GetComponent<Animator>();
            _fingers = new AvatarMask[] { _thumbMask, _indexMask, _middleMask, _ringMask, _pinkyMask };
            _thimbles = new WeArtThimbleTrackingObject[] {
                _thumbThimbleTracking,
                _indexThimbleTracking,
                _middleThimbleTracking, _middleThimbleTracking, _middleThimbleTracking
            };

            _thumbThimbleHaptic.SetIsUsedByController(true);
            _thumbThimbleHaptic.HandController = this;
            _indexThimbleHaptic.SetIsUsedByController(true);
            _indexThimbleHaptic.HandController = this;
            _middleThimbleHaptic.SetIsUsedByController(true);
            _middleThimbleHaptic.HandController = this;

            _thumbHapticPosition = _thumbThimbleHaptic.transform.localPosition;
            _indexHapticPosition = _indexThimbleHaptic.transform.localPosition;
            _middleHapticPosition = _middleThimbleHaptic.transform.localPosition;

            _thumbHapticExplorationPosition = _explorationThumbHapticTransform.localPosition;
            _indexHapticExplorationPosition = _explorationIndexHapticTransform.localPosition;
            _middleHapticExplorationPosition = _explorationMiddleHapticTransform.localPosition;
            
            WeArtHandController[] components = GameObject.FindObjectsOfType<WeArtHandController>();
            foreach (var component in components)
            {
                if (component._handSide != _handSide)
                {
                    _otherHand = component;
                }
            }
        }

        /// <summary>
        /// The OnEnable.
        /// </summary>
        private void OnEnable()
        {
            // Fingers collider info assignments ****

            _thumbThimbleHaptic.TriggerEnter -= ThumbTriggerEnterHandle;
            _thumbThimbleHaptic.TriggerEnter -= ThumbTriggerStayHandle;
            _thumbThimbleHaptic.TriggerExit -= ThumbTriggerExitHandle;
            _thumbThimbleHaptic.TriggerEnter += ThumbTriggerEnterHandle;
            _thumbThimbleHaptic.TriggerEnter += ThumbTriggerStayHandle;
            _thumbThimbleHaptic.TriggerExit += ThumbTriggerExitHandle;

            _indexThimbleHaptic.TriggerEnter -= IndexTriggerEnterHandle;
            _indexThimbleHaptic.TriggerStay -= IndexTriggerStayHandle;
            _indexThimbleHaptic.TriggerExit -= IndexTriggerExitHandle;
            _indexThimbleHaptic.TriggerEnter += IndexTriggerEnterHandle;
            _indexThimbleHaptic.TriggerStay += IndexTriggerStayHandle;
            _indexThimbleHaptic.TriggerExit += IndexTriggerExitHandle;

            _middleThimbleHaptic.TriggerEnter -= MiddleTriggerEnterHandle;
            _middleThimbleHaptic.TriggerEnter -= MiddleTriggerStayHandle;
            _middleThimbleHaptic.TriggerExit -= MiddleTriggerExitHandle;
            _middleThimbleHaptic.TriggerEnter += MiddleTriggerEnterHandle;
            _middleThimbleHaptic.TriggerEnter += MiddleTriggerStayHandle;
            _middleThimbleHaptic.TriggerExit += MiddleTriggerExitHandle;

            _palmThimbleHaptic.TriggerEnter -= PalmTriggerEnterHandle;
            _palmThimbleHaptic.TriggerExit -= PalmTriggerExitHandle;
            _palmThimbleHaptic.TriggerEnter += PalmTriggerEnterHandle;
            _palmThimbleHaptic.TriggerExit += PalmTriggerExitHandle;

            // **************************

            // Fingers grasp checkers info assignments ****

            _thumbGraspCheckHaptic.TriggerEnter -= ThumbGraspCheckEnterHandle;
            _thumbGraspCheckHaptic.TriggerStay -= ThumbGraspCheckStayHandle;
            _thumbGraspCheckHaptic.TriggerExit -= ThumbGraspCheckExitHandle;
            _thumbGraspCheckHaptic.TriggerEnter += ThumbGraspCheckEnterHandle;
            _thumbGraspCheckHaptic.TriggerStay += ThumbGraspCheckStayHandle;
            _thumbGraspCheckHaptic.TriggerExit += ThumbGraspCheckExitHandle;

            _indexGraspCheckHaptic.TriggerEnter -= IndexGraspCheckEnterHandle;
            _indexGraspCheckHaptic.TriggerStay -= IndexGraspCheckStayHandle;
            _indexGraspCheckHaptic.TriggerExit -= IndexGraspCheckExitHandle;
            _indexGraspCheckHaptic.TriggerEnter += IndexGraspCheckEnterHandle;
            _indexGraspCheckHaptic.TriggerStay += IndexGraspCheckStayHandle;
            _indexGraspCheckHaptic.TriggerExit += IndexGraspCheckExitHandle;

            _middleGraspCheckHaptic.TriggerEnter -= MiddleGraspCheckEnterHandle;
            _middleGraspCheckHaptic.TriggerStay -= MiddleGraspCheckStayHandle;
            _middleGraspCheckHaptic.TriggerExit -= MiddleGraspCheckExitHandle;
            _middleGraspCheckHaptic.TriggerEnter += MiddleGraspCheckEnterHandle;
            _middleGraspCheckHaptic.TriggerStay += MiddleGraspCheckStayHandle;
            _middleGraspCheckHaptic.TriggerExit += MiddleGraspCheckExitHandle;

            // **************************

            _graph = PlayableGraph.Create(nameof(WeArtHandController));

            var fingersLayerMixer = AnimationLayerMixerPlayable.Create(_graph, _fingers.Length);
            _fingersMixers = new AnimationLayerMixerPlayable[_fingers.Length];

            for (uint i = 0; i < _fingers.Length; i++)
            {
                var fingerMixer = AnimationLayerMixerPlayable.Create(_graph, 3);
                _graph.Connect(AnimationClipPlayable.Create(_graph, _openedHandState), 0, fingerMixer, 0);
                _graph.Connect(AnimationClipPlayable.Create(_graph, _closedHandState), 0, fingerMixer, 1);
                _graph.Connect(AnimationClipPlayable.Create(_graph, _abductionHandState), 0, fingerMixer, 2);

                fingerMixer.SetLayerAdditive(0, false);
                fingerMixer.SetLayerMaskFromAvatarMask(0, _fingers[i]);
                fingerMixer.SetInputWeight(0, 1);
                fingerMixer.SetInputWeight(1, 0);
                _fingersMixers[i] = fingerMixer;

                fingersLayerMixer.SetLayerAdditive(i, false);
                fingersLayerMixer.SetLayerMaskFromAvatarMask(i, _fingers[i]);
                _graph.Connect(fingerMixer, 0, fingersLayerMixer, (int)i);
                fingersLayerMixer.SetInputWeight((int)i, 1);
            }

            var handMixer = AnimationMixerPlayable.Create(_graph, 2);
            _graph.Connect(fingersLayerMixer, 0, handMixer, 0);
            handMixer.SetInputWeight(0, 1);
            var playableOutput = AnimationPlayableOutput.Create(_graph, nameof(WeArtHandController), _animator);
            playableOutput.SetSourcePlayable(handMixer);
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            _graph.Play();

            // Subscribe custom finger closure behaviour during the grasp
            OnGraspingEvent += UpdateFingerClosure;
        }

        /// <summary>
        /// Get the grasping state of the hand controller.
        /// </summary>
        public GraspingState GraspingState
        {
            get => _graspingState;
            set => _graspingState = value;
        }

        /// <summary>
        /// Getr the invisible material
        /// </summary>
        /// <returns></returns>
        public Material GetGhostHandInvisibleMaterial()
        {
            return _ghostHandInvisibleMaterial;
        }

        /// <summary>
        /// Get the transparent material
        /// </summary>
        /// <returns></returns>
        public Material GetGhostHandTransparentMaterial()
        {
            return _ghostHandTransparentMaterial;
        }

        // Return the object that is currently grabbed 
        public WeArtTouchableObject GetGraspedObject()
        {
            return _touchableObject;
        }

        /// <summary>
        /// Debug method
        /// </summary>
        private void OnDrawGizmos()
        {
        }

        private void Start()
        {
            EnableThimbleHaptic(false);
        }
        
        /// <summary>
        /// The Update.
        /// </summary>
        private void Update()
        {
            _graph.Evaluate();

            if (_useCustomPoses && _graspingState == GraspingState.Grabbed)
            {
                // In this case the fingers not follow the tracking but are driven by WeArtGraspPose
                // The behaviour is called in this script in -> UpdateFingerClousure
            }
            else // Otherwise fingers behaviour works as always
            {
                for (int i = 0; i < _fingers.Length; i++)
                {
                    if (!_thimbles[i].IsBlocked)
                    {
                        bool isGettingCloseToGrasp = false;

                        if (i == 0 && _thumbGraspCheckTouchables.Count > 0 && _thimbles[i].Closure.Value > _fingersMixers[i].GetInputWeight(1))
                            isGettingCloseToGrasp = true; // Finger is getting close to an object that it can grab and slows the speed in order to ensure a perfect position on the touchable object

                        if (i == 1 && _indexGraspCheckTouchables.Count > 0 && _thimbles[i].Closure.Value > _fingersMixers[i].GetInputWeight(1))
                            isGettingCloseToGrasp = true;

                        if (i == 2 && _middleGraspCheckTouchables.Count > 0 && _thimbles[i].Closure.Value > _fingersMixers[i].GetInputWeight(1))
                            isGettingCloseToGrasp = true;

                        float weight;
                        if (!isGettingCloseToGrasp) 
                        {
                            weight = _thimbles[i].Closure.Value;
                        }
                        else // If in proximity, move slower in order to avoid clipping through colliders at high movement speed per frame
                        {
                            weight = Mathf.Lerp(_fingersMixers[i].GetInputWeight(1), _thimbles[i].Closure.Value,
                               Time.deltaTime * (_thimbles[i].SafeUnblockSeconds > 0 ? _fingersSlideSpeed : _fingersAnimationSpeed));
                        }

                        if (_slowFingerAnimationTime > 0)
                        {
                            weight = Mathf.Lerp(_fingersMixers[i].GetInputWeight(1), _thimbles[i].Closure.Value,
                               Time.deltaTime * _fingersSlideSpeed * _extraFingerSpeed);
                        }

                        if (i <= 2)
                        {
                            _fingersMixers[i].SetInputWeight(0, 1 - weight);
                            _fingersMixers[i].SetInputWeight(1, weight);
                        }
                        else
                        {
                            _fingersMixers[i].SetInputWeight(0, 1 - _fingersMixers[2].GetInputWeight(1));
                            _fingersMixers[i].SetInputWeight(1, _fingersMixers[2].GetInputWeight(1));
                        }

                        // Thumb has an extra field called abduction that allows the finger to move up and down (non closing motion)
                        if (_thimbles[i].ActuationPoint == ActuationPoint.Thumb)
                        {
                            float abduction;
                            if (!isGettingCloseToGrasp)
                            {
                                abduction = _thimbles[i].Abduction.Value;
                            }
                            else
                            {
                                abduction = Mathf.Lerp(_fingersMixers[i].GetInputWeight(2), _thimbles[i].Abduction.Value,
                                Time.deltaTime * (_thimbles[i].SafeUnblockSeconds > 0 ? _fingersSlideSpeed : _fingersAnimationSpeed));
                            }

                            if (_slowFingerAnimationTime > 0)
                            {
                                abduction = Mathf.Lerp(_fingersMixers[i].GetInputWeight(2), _thimbles[i].Abduction.Value,
                                Time.deltaTime * (_thimbles[i].SafeUnblockSeconds > 0 ? _fingersSlideSpeed * _extraFingerSpeed : _fingersAnimationSpeed));
                            }

                            _fingersMixers[i].SetInputWeight(2, abduction);
                        }

                        if (_thimbles[i].SafeUnblockSeconds > 0)
                            _thimbles[i].SafeUnblockSeconds -= Time.deltaTime;
                    }
                }
            }

            SurfaceExploringCheck();

            CheckFingerBlocking();

            CheckGraspingConditions();

            HandleFingerEffects();
        }

        private void OnDisable()
        {
            // Disable animation
            _graph.Destroy();

            OnGraspingEvent -= UpdateFingerClosure;
        }
        
        /// <summary>
        /// If two of the three rayscasts that start from the fingertips of Thumb, Index or Middle hit a touchable object with the Surface Exploration flag true, displace the haptic objects for a better exploration experience
        /// </summary>
        private void SurfaceExploringCheck()
        {
            if (_graspingState == GraspingState.Grabbed)
                return;

            float raycastDistance = 0.03f;
            RaycastHit[] hits;
            bool isThumbOnSurface = false;
            bool isIndexOnSurface = false;
            bool isMiddleOnSurface = false;

            hits = Physics.RaycastAll(_thumbExplorationOrigin.position, transform.up * -1, raycastDistance);
            Debug.DrawRay(_thumbExplorationOrigin.position, transform.up * -1 * raycastDistance, Color.green);
            foreach (var item in hits)
            {
                if (TryGetTouchableObjectFromCollider(item.collider, out var touchable))
                {
                    if (touchable._surfaceExploration)
                    {
                        isThumbOnSurface = true;
                    }
                }
            }

            hits = Physics.RaycastAll(_indexExplorationOrigin.position, transform.up * -1, raycastDistance);
            Debug.DrawRay(_indexExplorationOrigin.position, transform.up * -1 * raycastDistance, Color.green);
            foreach (var item in hits)
            {
                if (TryGetTouchableObjectFromCollider(item.collider, out var touchable))
                {
                    if (touchable._surfaceExploration)
                    {
                        isIndexOnSurface = true;
                    }
                }
            }

            hits = Physics.RaycastAll(_middleExplorationOrigin.position, transform.up * -1, raycastDistance);
            Debug.DrawRay(_middleExplorationOrigin.position, transform.up * -1 * raycastDistance, Color.green);

            foreach (var item in hits)
            {
                if (TryGetTouchableObjectFromCollider(item.collider, out var touchable))
                {
                    if (touchable._surfaceExploration)
                    {
                        isMiddleOnSurface = true;
                    }
                }
            }

            if ((isThumbOnSurface && isIndexOnSurface) || (isThumbOnSurface && isMiddleOnSurface) || (isIndexOnSurface && isMiddleOnSurface))
            {
                _thumbThimbleTracking.SafeUnblockSeconds = _safeUnlockFrames;
                _indexThimbleTracking.SafeUnblockSeconds = _safeUnlockFrames;
                _middleThimbleTracking.SafeUnblockSeconds = _safeUnlockFrames;

                _slowFingerAnimationTime = _safeUnlockFrames;

                _thumbThimbleHaptic.transform.localPosition = _thumbHapticExplorationPosition;
                _indexThimbleHaptic.transform.localPosition = _indexHapticExplorationPosition;
                _middleThimbleHaptic.transform.localPosition = _middleHapticExplorationPosition;
            }
            else
            {
                _thumbThimbleHaptic.transform.localPosition = _thumbHapticPosition;
                _indexThimbleHaptic.transform.localPosition = _indexHapticPosition;
                _middleThimbleHaptic.transform.localPosition = _middleHapticPosition;
            }

        }

        /// <summary>
        /// Take care of the temperature, force and texture effects of the fingers
        /// </summary>
        private void HandleFingerEffects()
        {
            float distance = Vector3.Distance(_thumbThimbleHaptic.transform.position, _realThumbPosition.position) / _dynamicForceSensitivity;
            _thumbThimbleHaptic.SetPhysicalForce(distance);
            distance = Vector3.Distance(_indexThimbleHaptic.transform.position, _realIndexPosition.position) / _dynamicForceSensitivity;
            _indexThimbleHaptic.SetPhysicalForce(distance);
            distance = Vector3.Distance(_middleThimbleHaptic.transform.position, _realMiddlePosition.position) / _dynamicForceSensitivity;
            _middleThimbleHaptic.SetPhysicalForce(distance);

            if (_graspingState == GraspingState.Grabbed)
            {
                if (_thumbThimbleTracking.IsGrasping != _thumbThimbleHaptic.IsGrasping)
                    _thumbThimbleHaptic.IsGrasping = _thumbThimbleTracking.IsGrasping;

                if (_indexThimbleTracking.IsGrasping != _indexThimbleHaptic.IsGrasping)
                    _indexThimbleHaptic.IsGrasping = _indexThimbleTracking.IsGrasping;

                if (_middleThimbleTracking.IsGrasping != _middleThimbleHaptic.IsGrasping)
                    _middleThimbleHaptic.IsGrasping = _middleThimbleTracking.IsGrasping;
            }
            else
            {
                if (_thumbThimbleHaptic.IsGrasping)
                    _thumbThimbleHaptic.IsGrasping = false;

                if (_indexThimbleHaptic.IsGrasping)
                    _indexThimbleHaptic.IsGrasping = false;

                if (_middleThimbleHaptic.IsGrasping)
                    _middleThimbleHaptic.IsGrasping = false;
            }

            _thumbThimbleHaptic.CheckForNewEffect();
            _indexThimbleHaptic.CheckForNewEffect();
            _middleThimbleHaptic.CheckForNewEffect();

            CheckAllGraspingEffects();
        }

        /// <summary>
        /// Check if the grasping effects are applied, especially for mesh colliders
        /// </summary>
        private void CheckAllGraspingEffects()
        {
            if (_graspingState == GraspingState.Released || _touchableObject == null)
                return;

            if (_thumbThimbleTracking.IsBlocked && _thumbThimbleTracking.IsGrasping && _thumbThimbleHaptic.GetEffect() == null)
            {
                _touchableObject.ForceOnColliderEnter(_thumbThimbleHaptic.GetComponent<Collider>());
            }

            if (_indexThimbleTracking.IsBlocked && _indexThimbleTracking.IsGrasping && _indexThimbleHaptic.GetEffect() == null)
            {
                _touchableObject.ForceOnColliderEnter(_indexThimbleHaptic.GetComponent<Collider>());
            }

            if (_middleThimbleTracking.IsBlocked && _middleThimbleTracking.IsGrasping && _middleThimbleHaptic.GetEffect() == null)
            {
                _touchableObject.ForceOnColliderEnter(_middleThimbleHaptic.GetComponent<Collider>());
            }

            // Temperature check
            if (_thumbThimbleTracking.IsBlocked && _thumbThimbleTracking.IsGrasping && _thumbThimbleHaptic.Temperature.Value != _touchableObject.Temperature.Value)
            {
                WeArtTouchEffect touchEffect = new WeArtTouchEffect();
                touchEffect.Set(_touchableObject.Temperature, _touchableObject.Stiffness, _touchableObject.Texture, new WeArtTouchEffect.WeArtImpactInfo());
                _thumbThimbleHaptic.AddEffect(touchEffect);
            }

            if (_indexThimbleTracking.IsBlocked && _indexThimbleTracking.IsGrasping && _indexThimbleHaptic.Temperature.Value != _touchableObject.Temperature.Value)
            {
                WeArtTouchEffect touchEffect = new WeArtTouchEffect();
                touchEffect.Set(_touchableObject.Temperature, _touchableObject.Stiffness, _touchableObject.Texture, new WeArtTouchEffect.WeArtImpactInfo());
                _indexThimbleHaptic.AddEffect(touchEffect);
            }

            if (_middleThimbleTracking.IsBlocked && _middleThimbleTracking.IsGrasping && _middleThimbleHaptic.Temperature.Value != _touchableObject.Temperature.Value)
            {
                WeArtTouchEffect touchEffect = new WeArtTouchEffect();
                touchEffect.Set(_touchableObject.Temperature, _touchableObject.Stiffness, _touchableObject.Texture, new WeArtTouchEffect.WeArtImpactInfo());
                _middleThimbleHaptic.AddEffect(touchEffect);
            }
        }

        /// <summary>
        /// Check if the finger is stoped by a surface
        /// </summary>
        private void CheckFingerBlocking()
        {
            if (_slowFingerAnimationTime > 0)
                _slowFingerAnimationTime -= Time.deltaTime;

            CheckIndividualFingerBlocking(_thumbThimbleTracking, _thumbContactTouchables, _thumbGraspCheckTouchables, _thumbBlockingTouchables, 0);
            CheckIndividualFingerBlocking(_indexThimbleTracking, _indexContactTouchables, _indexGraspCheckTouchables, _indexBlockingTouchables, 1);
            CheckIndividualFingerBlocking(_middleThimbleTracking, _middleContactTouchables, _middleGraspCheckTouchables, _middleBlockingTouchables, 2);
        }

        /// <summary>
        /// Check if a finger should be blocked when touching an object
        /// </summary>
        /// <param name="thimbleTracking"></param>
        /// <param name="contactTouchables"></param>
        /// <param name="graspCheckTouchables"></param>
        /// <param name="blockingTouchables"></param>
        /// <param name="fingerNumber"></param>
        private void CheckIndividualFingerBlocking(WeArtThimbleTrackingObject thimbleTracking, List<WeArtTouchableObject> contactTouchables, List<WeArtTouchableObject> graspCheckTouchables, List<WeArtTouchableObject> blockingTouchables, int fingerNumber)
        {
            bool hasToUnblock = true;

            if (thimbleTracking.IsBlocked)
            {
                if (thimbleTracking.IsGrasping == false)
                {
                    //Colapse the next "if" for better overview
                    if (_graspingState == GraspingState.Released)
                    {
                        foreach (WeArtTouchableObject contactItem in contactTouchables)
                        {
                            bool isContactItemBlocking = false;
                            foreach (WeArtTouchableObject graspCheckItem in graspCheckTouchables)
                            {
                                if (contactItem == graspCheckItem)
                                {
                                    hasToUnblock = false;
                                    isContactItemBlocking = true;
                                }
                            }

                            if (!isContactItemBlocking)
                            {
                                if (blockingTouchables.Contains(contactItem))
                                {
                                    blockingTouchables.Remove(contactItem);
                                }
                            }
                        }

                        if (hasToUnblock)
                        {
                            thimbleTracking.IsBlocked = false;
                            thimbleTracking.SafeUnblockSeconds = _safeUnlockFrames;

                        }
                        else
                        {
                            if (thimbleTracking.Closure.Value < thimbleTracking.BlockedClosureValue)
                            {
                                thimbleTracking.IsBlocked = false;
                                thimbleTracking.IsGrasping = false;
                            }
                        }
                    }
                    else
                    {
                        if (thimbleTracking.Closure.Value > thimbleTracking.BlockedClosureValue)
                        {
                            thimbleTracking.IsBlocked = false;
                        }
                    }
                }
                else
                {
                    if (thimbleTracking.Closure.Value < thimbleTracking.BlockedClosureValue)
                    {
                        thimbleTracking.IsBlocked = false;
                        thimbleTracking.IsGrasping = false;
                    }
                }
            }
            else
            {
                foreach (WeArtTouchableObject contactItem in contactTouchables)
                {
                    foreach (WeArtTouchableObject graspCheckItem in graspCheckTouchables)
                    {
                        if (contactItem == graspCheckItem)
                        {
                            thimbleTracking.IsBlocked = true;

                            if (_graspingState == GraspingState.Grabbed)
                                thimbleTracking.IsGrasping = true;

                            thimbleTracking.BlockedClosureValue = _fingersMixers[fingerNumber].GetInputWeight(1);
                            if (!blockingTouchables.Contains(contactItem))
                                blockingTouchables.Add(contactItem);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check the three grasping conditions
        /// </summary>
        private void CheckGraspingConditions()
        {
            if (_notAllowedToGrabSeconds > 0)
            {
                _notAllowedToGrabSeconds -= Time.deltaTime;
                return;
            }

            bool condA = false;
            bool condB = false;
            bool condC = false;

            if (_graspingState != GraspingState.Grabbed)
            {
                condA = ConditionA();

                if (condA)
                    condB = ConditionB();

                if (condB)
                    condC = ConditionC();
            }
            else
            {
                condA = true;
                condB = true;
                condC = true;
                CheckReleaseConditions();
            }

        }

        /// <summary>
        /// Condition is true if the Thumb or Palm are in contact with a touchable object and Index or Middle are also in contact with the same object
        /// </summary>
        /// <returns></returns>
        private bool ConditionA()
        {
            if (_thumbContactTouchables.Count > 0)
            {
                if (_indexContactTouchables.Count > 0)
                {
                    return true;
                }

                if (_middleContactTouchables.Count > 0)
                {
                    return true;
                }

                if (_palmContactTouchables.Count > 0)
                {
                    return true;
                }
            }

            if (_palmContactTouchables.Count > 0)
            {
                if (_indexContactTouchables.Count > 0)
                {
                    return true;
                }

                if (_middleContactTouchables.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Condition is true if the user applies force on the object, making sure that the user truly wants to grab the object 
        /// </summary>
        /// <returns></returns>
        private bool ConditionB()
        {
            _isThumbTryingToGrasp = false;
            _isIndexTryingToGrasp = false;
            _isMiddleTryingToGrasp = false;

            bool anyFingerTryingToGrasp = false;

            if (_thumbThimbleTracking.IsBlocked)
            {
                if (_thumbThimbleTracking.Closure.Value > _fingersMixers[0].GetInputWeight(1) + _intentionalGraspThreshold
                    && _fingersMixers[0].GetInputWeight(1) > _tryingToGraspThreshhold)
                {
                    _isThumbTryingToGrasp = true;
                    anyFingerTryingToGrasp = true;
                }
            }

            if (_indexThimbleTracking.IsBlocked)
            {
                if (_indexThimbleTracking.Closure.Value > _fingersMixers[1].GetInputWeight(1) + _intentionalGraspThreshold
                    && _fingersMixers[1].GetInputWeight(1) > _tryingToGraspThreshhold)
                {
                    _isIndexTryingToGrasp = true;
                    anyFingerTryingToGrasp = true;
                }
            }

            if (_middleThimbleTracking.IsBlocked)
            {
                if (_middleThimbleTracking.Closure.Value > _fingersMixers[2].GetInputWeight(1) + _intentionalGraspThreshold
                    && _fingersMixers[2].GetInputWeight(1) > _tryingToGraspThreshhold)
                {
                    _isMiddleTryingToGrasp = true;
                    anyFingerTryingToGrasp = true;
                }
            }

            return anyFingerTryingToGrasp;
        }

        /// <summary>
        ///  Condition is true if the Thumb or Palm are in contact with a touchable object and Index or Middle are also in contact with the same object and fingers are also blocked on the object and the user applies pressure on the object
        /// </summary>
        /// <returns></returns>
        private bool ConditionC()
        {
            if (_isThumbTryingToGrasp)
            {
                if (_indexThimbleTracking.IsBlocked)
                {
                    foreach (WeArtTouchableObject item in _thumbBlockingTouchables)
                    {
                        if (!item.IsGraspable)
                            continue;

                        foreach (WeArtTouchableObject otherItem in _indexBlockingTouchables)
                        {
                            if (item == otherItem)
                            {
                                _thumbThimbleTracking.TouchableObject = item;
                                _thumbThimbleTracking.IsGrasping = true;
                                _thumbThimbleHaptic.IsGrasping = true;
                                _indexThimbleTracking.TouchableObject = item;
                                _indexThimbleTracking.IsGrasping = true;
                                _indexThimbleHaptic.IsGrasping = true;
                                GraspTouchableObject(item);
                                return true;
                            }
                        }
                    }
                }

                if (_middleThimbleTracking.IsBlocked)
                {
                    foreach (WeArtTouchableObject item in _thumbBlockingTouchables)
                    {
                        if (!item.IsGraspable)
                            continue;

                        foreach (WeArtTouchableObject otherItem in _middleBlockingTouchables)
                        {
                            if (item == otherItem)
                            {
                                _thumbThimbleTracking.TouchableObject = item;
                                _thumbThimbleTracking.IsGrasping = true;
                                _thumbThimbleHaptic.IsGrasping = true;
                                _middleThimbleTracking.TouchableObject = item;
                                _middleThimbleTracking.IsGrasping = true;
                                _middleThimbleHaptic.IsGrasping = true;
                                GraspTouchableObject(item);
                                return true;
                            }
                        }
                    }
                }

                if (_fingersMixers[0].GetInputWeight(1) >= WeArtConstants.palmGraspClosureThreshold)
                {
                    foreach (WeArtTouchableObject item in _thumbBlockingTouchables)
                    {
                        if (!item.IsGraspable)
                            continue;

                        foreach (WeArtTouchableObject otherItem in _palmContactTouchables)
                        {
                            if (item == otherItem)
                            {
                                _isPalmGrasping = true;
                                _thumbThimbleTracking.TouchableObject = item;
                                _thumbThimbleTracking.IsGrasping = true;
                                _thumbThimbleHaptic.IsGrasping = true;
                                GraspTouchableObject(item);
                                return true;
                            }
                        }
                    }
                }
            }

            if (_isIndexTryingToGrasp)
            {
                if (_thumbThimbleTracking.IsBlocked)
                {
                    foreach (WeArtTouchableObject item in _thumbBlockingTouchables)
                    {
                        if (!item.IsGraspable)
                            continue;

                        foreach (WeArtTouchableObject otherItem in _indexBlockingTouchables)
                        {
                            if (item == otherItem)
                            {
                                _thumbThimbleTracking.TouchableObject = item;
                                _thumbThimbleTracking.IsGrasping = true;
                                _thumbThimbleHaptic.IsGrasping = true;
                                _indexThimbleTracking.TouchableObject = item;
                                _indexThimbleTracking.IsGrasping = true;
                                _indexThimbleHaptic.IsGrasping = true;
                                GraspTouchableObject(item);
                                return true;
                            }
                        }
                    }
                }

                if (_fingersMixers[1].GetInputWeight(1) >= WeArtConstants.palmGraspClosureThreshold)
                {
                    foreach (WeArtTouchableObject item in _indexBlockingTouchables)
                    {
                        if (!item.IsGraspable)
                            continue;

                        foreach (WeArtTouchableObject otherItem in _palmContactTouchables)
                        {
                            if (item == otherItem)
                            {
                                _isPalmGrasping = true;
                                _indexThimbleTracking.TouchableObject = item;
                                _indexThimbleTracking.IsGrasping = true;
                                _indexThimbleHaptic.IsGrasping = true;
                                GraspTouchableObject(item);
                                return true;
                            }
                        }
                    }
                }
            }

            if (_isMiddleTryingToGrasp)
            {
                if (_thumbThimbleTracking.IsBlocked)
                {
                    foreach (WeArtTouchableObject item in _thumbBlockingTouchables)
                    {
                        if (!item.IsGraspable)
                            continue;

                        foreach (WeArtTouchableObject otherItem in _middleBlockingTouchables)
                        {
                            if (item == otherItem)
                            {
                                _thumbThimbleTracking.TouchableObject = item;
                                _thumbThimbleTracking.IsGrasping = true;
                                _thumbThimbleHaptic.IsGrasping = true;
                                _middleThimbleTracking.TouchableObject = item;
                                _middleThimbleTracking.IsGrasping = true;
                                _middleThimbleHaptic.IsGrasping = true;
                                GraspTouchableObject(item);
                                return true;
                            }
                        }
                    }
                }

                if (_fingersMixers[2].GetInputWeight(1) >= WeArtConstants.palmGraspClosureThreshold)
                {
                    foreach (WeArtTouchableObject item in _middleBlockingTouchables)
                    {
                        if (!item.IsGraspable)
                            continue;

                        foreach (WeArtTouchableObject otherItem in _palmContactTouchables)
                        {
                            if (item == otherItem)
                            {
                                _isPalmGrasping = true;
                                _middleThimbleTracking.TouchableObject = item;
                                _middleThimbleTracking.IsGrasping = true;
                                _middleThimbleHaptic.IsGrasping = true;
                                GraspTouchableObject(item);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Check grasping release conditions, if not enough fingers are holding the object, release it
        /// </summary>
        private void CheckReleaseConditions()
        {
            bool isReleaseCheckGrasping = false;

            ConditionB();

            if (_isThumbTryingToGrasp && _thumbThimbleTracking.IsGrasping)
            {
                if (_indexThimbleTracking.IsGrasping)
                {
                    isReleaseCheckGrasping = true;
                }

                if (_middleThimbleTracking.IsGrasping)
                {
                    isReleaseCheckGrasping = true;
                }

                if (_isPalmGrasping && _fingersMixers[0].GetInputWeight(1) > WeArtConstants.palmGraspClosureThreshold)
                    isReleaseCheckGrasping = true;

            }

            if (_isIndexTryingToGrasp && _indexThimbleTracking.IsGrasping)
            {
                if (_thumbThimbleTracking.IsGrasping)
                {
                    isReleaseCheckGrasping = true;
                }

                if (_isPalmGrasping && _fingersMixers[1].GetInputWeight(1) > WeArtConstants.palmGraspClosureThreshold)

                    isReleaseCheckGrasping = true;
            }

            if (_isMiddleTryingToGrasp && _middleThimbleTracking.IsGrasping)
            {
                if (_thumbThimbleTracking.IsGrasping)
                {
                    isReleaseCheckGrasping = true;
                }

                if (_isPalmGrasping && _fingersMixers[2].GetInputWeight(1) > WeArtConstants.palmGraspClosureThreshold)
                    isReleaseCheckGrasping = true;
            }

            if (!isReleaseCheckGrasping)
                ReleaseTouchableObject();
        }

        /// <summary>
        /// Grab a touchable object
        /// </summary>
        /// <param name="touchable"></param>
        private void GraspTouchableObject(WeArtTouchableObject touchable)
        {
            if (_otherHand != null)
            {
                if (_otherHand.GraspingState == GraspingState.Grabbed)
                {
                    if (_otherHand.GetGraspedObject() == touchable)
                    {
                        _otherHand.MakeTheHandUnableToGrab();
                        _otherHand.ReleaseTouchableObject();

                        if (_thumbThimbleTracking.IsBlocked)
                        {

                            WeArtTouchEffect effect = new WeArtTouchEffect();
                            effect.Set(touchable.Temperature, touchable.Stiffness, touchable.Texture, null);
                            _thumbThimbleHaptic.AddEffect(effect);
                            _thumbThimbleHaptic.IsGrasping = true;
                        }

                        if (_indexThimbleTracking.IsBlocked)
                        {
                            WeArtTouchEffect effect = new WeArtTouchEffect();
                            effect.Set(touchable.Temperature, touchable.Stiffness, touchable.Texture, null);
                            _indexThimbleHaptic.AddEffect(effect);
                            _indexThimbleHaptic.IsGrasping = true;
                        }

                        if (_middleThimbleTracking.IsBlocked)
                        {
                            WeArtTouchEffect effect = new WeArtTouchEffect();
                            effect.Set(touchable.Temperature, touchable.Stiffness, touchable.Texture, null);
                            _middleThimbleHaptic.AddEffect(effect);
                            _middleThimbleHaptic.IsGrasping = true;
                        }
                    }
                }
            }

            _graspingState = GraspingState.Grabbed;
            _touchableObject = touchable;
            OnGraspingEvent?.Invoke(_handSide, _touchableObject.gameObject);

            _touchableObject.Grab(_grasper);
            _touchableObject.GraspingHandController = this;

            _thumbThimbleHaptic.transform.localPosition = _thumbHapticPosition;
            _indexThimbleHaptic.transform.localPosition = _indexHapticPosition;
            _middleThimbleHaptic.transform.localPosition = _middleHapticPosition;
        }

        /// <summary>
        /// Release the grabbed object
        /// </summary>
        public void ReleaseTouchableObject()
        {
            _graspingState = GraspingState.Released;
            _isPalmGrasping = false;

            _touchableObject.Release();
            _touchableObject.GraspingHandController = null;

            _thumbThimbleTracking.IsBlocked = false;
            _indexThimbleTracking.IsBlocked = false;
            _middleThimbleTracking.IsBlocked = false;

            _thumbThimbleTracking.IsGrasping = false;
            _indexThimbleTracking.IsGrasping = false;
            _middleThimbleTracking.IsGrasping = false;

            _thumbThimbleHaptic.IsGrasping = false;
            _indexThimbleHaptic.IsGrasping = false;
            _middleThimbleHaptic.IsGrasping = false;

            _thumbThimbleTracking.TouchableObject = null;
            _indexThimbleTracking.TouchableObject = null;
            _middleThimbleTracking.TouchableObject = null;

            _thumbBlockingTouchables.Clear();
            _indexBlockingTouchables.Clear();
            _middleBlockingTouchables.Clear();

            _thumbContactTouchables.Clear();
            _indexContactTouchables.Clear();
            _middleContactTouchables.Clear();

            _thumbGraspCheckTouchables.Clear();
            _indexGraspCheckTouchables.Clear();
            _middleGraspCheckTouchables.Clear();

            _thumbThimbleHaptic.ClearTouchedObjects();
            _indexThimbleHaptic.ClearTouchedObjects();
            _middleThimbleHaptic.ClearTouchedObjects();

            _thumbThimbleHaptic.Release(_thumbThimbleHaptic.GetEffect());
            _indexThimbleHaptic.Release(_indexThimbleHaptic.GetEffect());
            _middleThimbleHaptic.Release(_middleThimbleHaptic.GetEffect());

            _touchableObject = null;
            OnReleaseEvent?.Invoke(_handSide, _touchableObject.gameObject);
        }

        /// <summary>
        /// Make the hand unable to grab objects for a set time
        /// </summary>
        public void MakeTheHandUnableToGrab()
        {
            _notAllowedToGrabSeconds = _notAllowedToGrabSecondsMax;
            _slowFingerAnimationTime = _slowFingerAnimationTimeMax;
        }

        /// <summary>
        /// When a touchable object is disabled, unblock the fingers that were holding it
        /// </summary>
        /// <param name="hapticObject"></param>
        /// <param name="touchable"></param>
        public void UnblockFingerOnDisable(WeArtHapticObject hapticObject, WeArtTouchableObject touchable)
        {
            if (_graspingState == GraspingState.Grabbed)
            {
                if (touchable == _touchableObject)
                {
                    ReleaseTouchableObject();
                    MakeTheHandUnableToGrab();
                }
                return;
            }

            if (_notAllowedToGrabSeconds > 0)
            {
                return;
            }

            if (hapticObject == _thumbThimbleHaptic)
            {
                ThumbGraspCheckExitHandle(touchable.GetComponent<Collider>());
                ThumbTriggerExitHandle(touchable.GetComponent<Collider>());
            }

            if (hapticObject == _indexThimbleHaptic)
            {
                IndexGraspCheckExitHandle(touchable.GetComponent<Collider>());
                IndexTriggerExitHandle(touchable.GetComponent<Collider>());
            }

            if (hapticObject == _middleThimbleHaptic)
            {
                MiddleGraspCheckExitHandle(touchable.GetComponent<Collider>());
                MiddleTriggerExitHandle(touchable.GetComponent<Collider>());
            }
        }

        /// <summary>
        /// Handle the behaviour of all fingers during the grasp
        /// </summary>
        private void UpdateFingerClosure(HandSide hand, GameObject gameObject)
        {
            if (_useCustomPoses)
            {
                // In this case you have to use WeArtGraspPose on your touchable object to handle the fingers poses
                if (TryGetCustomPosesFromTouchable(gameObject, out var customPoses))
                {
                    StopAllCoroutines();

                    for (int i = 0; i < customPoses.fingersClosure.Length; i++)
                    {
                        var weight = customPoses.fingersClosure[i];

                        StartCoroutine(LerpPoses(_fingersMixers[i], 0, 1 - weight, customPoses.lerpTime));
                        StartCoroutine(LerpPoses(_fingersMixers[i], 1, weight, customPoses.lerpTime));
                    }
                }
            }
        }

        /// <summary>
        /// Deprecated method for calculation the grasping force 
        /// </summary>
        /// <param name="thumbClosureValue"></param>
        /// <param name="middelClosureValue"></param>
        /// <param name="indexClosureValue"></param>
        private void ComputeDynamicGraspForce(float thumbClosureValue, float middelClosureValue, float indexClosureValue)
        {
            float deltaThumb = thumbClosureValue - WeArtConstants.thresholdThumbClosure;
            float deltaMiddle = middelClosureValue - WeArtConstants.thresholdMiddleClosure;
            float deltaIndex = indexClosureValue - WeArtConstants.thresholdIndexClosure;

            float stiffnessObject = _touchableObject.Stiffness.Value;

            float dinamicForceThumb = (deltaThumb * stiffnessObject) * WeArtConstants.dinamicForceSensibility;
            float dinamicForceMiddle = (deltaMiddle * stiffnessObject) * WeArtConstants.dinamicForceSensibility;
            float dinamicForceIndex = (deltaIndex * stiffnessObject) * WeArtConstants.dinamicForceSensibility;

            dinamicForceThumb = WeArtUtility.NormalizedGraspForceValue(dinamicForceThumb);
            dinamicForceMiddle = WeArtUtility.NormalizedGraspForceValue(dinamicForceMiddle);
            dinamicForceIndex = WeArtUtility.NormalizedGraspForceValue(dinamicForceIndex);

            UpdateGraspingEffect(dinamicForceThumb, dinamicForceIndex, dinamicForceMiddle);
        }

        /// <summary>
        /// Update the grasping effect of the fingers
        /// </summary>
        /// <param name="thumbForce"></param>
        /// <param name="indexForce"></param>
        /// <param name="middleForce"></param>
        private void UpdateGraspingEffect(float thumbForce, float indexForce, float middleForce)
        {
            Temperature temperature = Temperature.Default;
            Texture texture = Texture.Default;
            if (_touchableObject != null)
            {
                temperature = _touchableObject.Temperature;

                if (_touchableObject._forcedVelocity)
                {
                    texture = _touchableObject._texture;
                    texture._forcedVelocity = true;
                }
                else
                {
                    texture._forcedVelocity = false;
                }

                if (!_touchableObject._stiffness.Active)
                {
                    thumbForce = 0;
                    indexForce = 0;
                    middleForce = 0;
                }
            }


            Force force = Force.Default;
            force.Active = true;

            force.Value = thumbForce;
            _thumbGraspingEffect.Set(temperature, force, texture, null);

            force.Value = indexForce;
            _indexGraspingEffect.Set(temperature, force, texture, null);

            force.Value = middleForce;
            _middleGraspingEffect.Set(temperature, force, texture, null);
        }

        /// <summary>
        /// Calibration manager signaling its success
        /// </summary>
        public void CalibrationSuccessful()
        {
            _thumbThimbleHaptic.Release(null);
            _indexThimbleHaptic.Release(null);
            _middleThimbleHaptic.Release(null);
        }

        /// <summary>
        /// Enables/Disables thimble haptic objects at the hand.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableThimbleHaptic(bool enable)
        {
            _thumbThimbleHaptic.EnablingActuating(enable);
            _indexThimbleHaptic.EnablingActuating(enable);
            _middleThimbleHaptic.EnablingActuating(enable);
        }
        
        /// <summary>
        /// Try to find a touchable object component on the collider
        /// </summary>
        /// <param name="collider">The collider<see cref="Collider"/>.</param>
        /// <param name="touchableObject">The touchableObject<see cref="WeArtTouchableObject"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private static bool TryGetTouchableObjectFromCollider(Collider collider, out WeArtTouchableObject touchableObject)
        {
            touchableObject = collider.gameObject.GetComponent<WeArtTouchableObject>();
            return touchableObject != null;
        }

        /// <summary>
        /// Try to get WeArtGraspPose script from current touchable object
        /// </summary>
        /// <param name="gameObject">Current grasped object</param>
        /// <param name="customPoses">Output of this result</param>
        /// <returns></returns>
        private static bool TryGetCustomPosesFromTouchable(GameObject gameObject, out WeArtGraspPose customPoses)
        {
            customPoses = gameObject.GetComponent<WeArtGraspPose>();
            return customPoses != null;
        }


        #endregion

        #region Coroutines

        /// <summary>
        /// Interpolate the finger pose during grasp when using WeArtGraspPose.cs
        /// </summary>
        /// <param name="finger"></param>
        /// <param name="inputIndex"></param>
        /// <param name="closure"></param>
        /// <param name="lerpTime"></param>
        /// <returns></returns>
        private IEnumerator LerpPoses(AnimationLayerMixerPlayable finger, int inputIndex, float closure, float lerpTime)
        {
            float t = 0f;
            float to = finger.GetInputWeight(inputIndex);

            while (t < lerpTime)
            {
                float lerp;
                lerp = Mathf.Lerp(to, closure, t / lerpTime);
                t += Time.deltaTime;

                finger.SetInputWeight(inputIndex, lerp);
                yield return null;
            }
        }
        #endregion

        #region Finger Info Assigments

        /// <summary>
        /// Thumb haptic entering a touchable object collider
        /// </summary>
        /// <param name="other"></param>
        void ThumbTriggerEnterHandle(Collider other)
        {
            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                _thumbThimbleHaptic.AddTouchedObject(touchable);

                if (!_thumbContactTouchables.Contains(touchable))
                {
                    _thumbContactTouchables.Add(touchable);

                    if (_graspingState == GraspingState.Released && touchable.GetGraspingState() == GraspingState.Grabbed)
                    {
                        touchable.ForceOnColliderEnter(_thumbThimbleHaptic.GetComponent<Collider>());
                    }
                }

                if (!_thumbThimbleTracking.IsBlocked && _graspingState == GraspingState.Grabbed && _touchableObject == touchable)
                {
                    _thumbThimbleTracking.IsBlocked = true;
                    _thumbThimbleTracking.BlockedClosureValue = _fingersMixers[0].GetInputWeight(1);
                    _thumbThimbleTracking.TouchableObject = touchable;
                    _thumbThimbleHaptic.IsGrasping = true;
                    foreach (var item in _thumbGraspCheckTouchables)
                    {
                        if (item == touchable)
                            _thumbThimbleTracking.IsGrasping = true;
                    }

                    WeArtTouchEffect effect = new WeArtTouchEffect();
                    effect.Set(touchable.Temperature, touchable.Stiffness, touchable.Texture, null);
                    _thumbThimbleHaptic.AddEffect(effect);
                }

            }
        }

        /// <summary>
        /// Thumb haptic staying in a touchable object collider
        /// </summary>
        /// <param name="other"></param>
        void ThumbTriggerStayHandle(Collider other)
        {
            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                _thumbThimbleHaptic.AddTouchedObject(touchable);

                if (_graspingState == GraspingState.Released && touchable.GetGraspingState() == GraspingState.Grabbed)
                {
                    touchable.ForceOnColliderStay(_thumbThimbleHaptic.GetComponent<Collider>());
                }
            }
        }

        /// <summary>
        /// Thumb haptic exiting a touchable object collider
        /// </summary>
        /// <param name="other"></param>
        void ThumbTriggerExitHandle(Collider other)
        {
            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                _thumbThimbleHaptic.RemoveTouchedObject(touchable);

                if (_thumbContactTouchables.Contains(touchable))
                {
                    _thumbContactTouchables.Remove(touchable);

                    if (_graspingState == GraspingState.Released && touchable.GetGraspingState() == GraspingState.Grabbed)
                    {
                        touchable.ForceOnColliderExit(_thumbThimbleHaptic.GetComponent<Collider>());
                    }
                }

                if (_thumbBlockingTouchables.Contains(touchable))
                {
                    _thumbBlockingTouchables.Remove(touchable);
                }

                if (_graspingState == GraspingState.Grabbed && touchable == _touchableObject)
                {
                    _thumbThimbleHaptic.RemoveEffect(_thumbThimbleHaptic.GetEffect());
                }
            }
        }

        /// <summary>
        /// Index haptic entering a touchable object collider
        /// </summary>
        /// <param name="other"></param>
        void IndexTriggerEnterHandle(Collider other)
        {
            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                _indexThimbleHaptic.AddTouchedObject(touchable);

                if (!_indexContactTouchables.Contains(touchable))
                {
                    _indexContactTouchables.Add(touchable);

                    if (_graspingState == GraspingState.Released && touchable.GetGraspingState() == GraspingState.Grabbed)
                    {
                        touchable.ForceOnColliderEnter(_indexThimbleHaptic.GetComponent<Collider>());
                    }
                }

                if (!_indexThimbleTracking.IsBlocked && _graspingState == GraspingState.Grabbed && _touchableObject == touchable)
                {
                    _indexThimbleTracking.IsBlocked = true;
                    _indexThimbleTracking.BlockedClosureValue = _fingersMixers[1].GetInputWeight(1);
                    _indexThimbleTracking.TouchableObject = touchable;
                    _indexThimbleHaptic.IsGrasping = true;
                    foreach (var item in _indexGraspCheckTouchables)
                    {
                        if (item == touchable)
                            _indexThimbleTracking.IsGrasping = true;
                    }

                    WeArtTouchEffect effect = new WeArtTouchEffect();
                    effect.Set(touchable.Temperature, touchable.Stiffness, touchable.Texture, null);
                    _indexThimbleHaptic.AddEffect(effect);
                }
            }
        }

        /// <summary>
        /// Index haptic staying in a touchable object collider
        /// </summary>
        /// <param name="other"></param>
        void IndexTriggerStayHandle(Collider other)
        {
            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                _indexThimbleHaptic.AddTouchedObject(touchable);

                if (_graspingState == GraspingState.Released && touchable.GetGraspingState() == GraspingState.Grabbed)
                {
                    touchable.ForceOnColliderStay(_indexThimbleHaptic.GetComponent<Collider>());
                }
            }
        }

        /// <summary>
        /// Index haptic exiting a touchable object collider
        /// </summary>
        /// <param name="other"></param>
        void IndexTriggerExitHandle(Collider other)
        {
            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                _indexThimbleHaptic.RemoveTouchedObject(touchable);
                if (_indexContactTouchables.Contains(touchable))
                {
                    _indexContactTouchables.Remove(touchable);

                    if (_graspingState == GraspingState.Released && touchable.GetGraspingState() == GraspingState.Grabbed)
                    {
                        touchable.ForceOnColliderExit(_indexThimbleHaptic.GetComponent<Collider>());
                    }
                }

                if (_indexBlockingTouchables.Contains(touchable))
                {
                    _indexBlockingTouchables.Remove(touchable);
                }

                if (_graspingState == GraspingState.Grabbed && touchable == _touchableObject)
                {
                    _indexThimbleHaptic.RemoveEffect(_indexThimbleHaptic.GetEffect());
                }
            }
        }

        /// <summary>
        /// Middle haptic entering a touchable object collider
        /// </summary>
        /// <param name="other"></param>
        void MiddleTriggerEnterHandle(Collider other)
        {
            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                _middleThimbleHaptic.AddTouchedObject(touchable);

                if (!_middleContactTouchables.Contains(touchable))
                {
                    _middleContactTouchables.Add(touchable);

                    if (_graspingState == GraspingState.Released && touchable.GetGraspingState() == GraspingState.Grabbed)
                    {
                        touchable.ForceOnColliderEnter(_middleThimbleHaptic.GetComponent<Collider>());
                    }
                }

                if (!_middleThimbleTracking.IsBlocked && _graspingState == GraspingState.Grabbed && _touchableObject == touchable)
                {
                    _middleThimbleTracking.IsBlocked = true;
                    _middleThimbleTracking.BlockedClosureValue = _fingersMixers[2].GetInputWeight(1);
                    _middleThimbleTracking.TouchableObject = touchable;
                    _middleThimbleHaptic.IsGrasping = true;
                    foreach (var item in _middleGraspCheckTouchables)
                    {
                        if (item == touchable)
                            _middleThimbleTracking.IsGrasping = true;
                    }

                    WeArtTouchEffect effect = new WeArtTouchEffect();
                    effect.Set(touchable.Temperature, touchable.Stiffness, touchable.Texture, null);
                    _middleThimbleHaptic.AddEffect(effect);
                }
            }
        }

        /// <summary>
        /// Middle haptic staying in a touchable object collider
        /// </summary>
        /// <param name="other"></param>
        void MiddleTriggerStayHandle(Collider other)
        {
            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                _middleThimbleHaptic.AddTouchedObject(touchable);

                if (_graspingState == GraspingState.Released && touchable.GetGraspingState() == GraspingState.Grabbed)
                {
                    touchable.ForceOnColliderStay(_middleThimbleHaptic.GetComponent<Collider>());
                }
            }
        }

        /// <summary>
        /// Middle haptic exiting a touchable object collider
        /// </summary>
        /// <param name="other"></param>
        void MiddleTriggerExitHandle(Collider other)
        {
            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                _middleThimbleHaptic.RemoveTouchedObject(touchable);

                if (_middleContactTouchables.Contains(touchable))
                {
                    _middleContactTouchables.Remove(touchable);

                    if (_graspingState == GraspingState.Released && touchable.GetGraspingState() == GraspingState.Grabbed)
                    {
                        touchable.ForceOnColliderExit(_middleThimbleHaptic.GetComponent<Collider>());
                    }
                }

                if (_middleBlockingTouchables.Contains(touchable))
                {
                    _middleBlockingTouchables.Remove(touchable);
                }

                if (_graspingState == GraspingState.Grabbed && touchable == _touchableObject)
                {
                    _middleThimbleHaptic.RemoveEffect(_middleThimbleHaptic.GetEffect());

                }
            }
        }

        /// <summary>
        /// Palm haptic entering a touchable object collider
        /// </summary>
        /// <param name="other"></param>
        void PalmTriggerEnterHandle(Collider other)
        {
            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                if (!_palmContactTouchables.Contains(touchable))
                {
                    _palmContactTouchables.Add(touchable);
                }
            }
        }

        /// <summary>
        /// Palm haptic exiting a touchable object collider
        /// </summary>
        /// <param name="other"></param>
        void PalmTriggerExitHandle(Collider other)
        {
            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                if (_palmContactTouchables.Contains(touchable))
                {
                    _palmContactTouchables.Remove(touchable);
                }
            }
        }

        #endregion

        #region Grasp Checkers 

        /// <summary>
        /// Thumb proximity check collides with a touchable object
        /// </summary>
        /// <param name="other"></param>
        void ThumbGraspCheckEnterHandle(Collider other)
        {
            if (other.isTrigger)
                return;

            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                if (!_thumbGraspCheckTouchables.Contains(touchable))
                {
                    _thumbGraspCheckTouchables.Add(touchable);
                }

            }
        }

        /// <summary>
        /// Thumb proximity check stays in contact with a touchable object
        /// </summary>
        /// <param name="other"></param>
        void ThumbGraspCheckStayHandle(Collider other)
        {
            if (other.isTrigger)
                return;

            if (_thumbThimbleTracking.IsBlocked)
                return;

            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                if (!_thumbGraspCheckTouchables.Contains(touchable))
                {
                    _thumbGraspCheckTouchables.Add(touchable);
                }

            }
        }

        /// <summary>
        /// Thumb proximity check exits a touchable object
        /// </summary>
        /// <param name="other"></param>
        void ThumbGraspCheckExitHandle(Collider other)
        {
            if (other.isTrigger)
                return;

            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                if (_thumbGraspCheckTouchables.Contains(touchable))
                {
                    _thumbGraspCheckTouchables.Remove(touchable);
                }
            }
        }

        /// <summary>
        /// Index proximity check collides with a touchable object
        /// </summary>
        /// <param name="other"></param>
        void IndexGraspCheckEnterHandle(Collider other)
        {
            if (other.isTrigger)
                return;

            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                if (!_indexGraspCheckTouchables.Contains(touchable))
                {
                    _indexGraspCheckTouchables.Add(touchable);
                }
            }
        }

        /// <summary>
        /// Index proximity check stays in contact with a touchable object
        /// </summary>
        /// <param name="other"></param>
        void IndexGraspCheckStayHandle(Collider other)
        {
            if (other.isTrigger)
                return;

            if (_indexThimbleTracking.IsBlocked)
                return;

            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                if (!_indexGraspCheckTouchables.Contains(touchable))
                {
                    _indexGraspCheckTouchables.Add(touchable);
                }
            }
        }

        /// <summary>
        /// Index proximity check exits a touchable object
        /// </summary>
        /// <param name="other"></param>
        void IndexGraspCheckExitHandle(Collider other)
        {
            if (other.isTrigger)
                return;

            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                if (_indexGraspCheckTouchables.Contains(touchable))
                {
                    _indexGraspCheckTouchables.Remove(touchable);
                }
            }
        }

        /// <summary>
        /// Middle proximity check collides with a touchable object
        /// </summary>
        /// <param name="other"></param>
        void MiddleGraspCheckEnterHandle(Collider other)
        {
            if (other.isTrigger)
                return;

            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                if (!_middleGraspCheckTouchables.Contains(touchable))
                {
                    _middleGraspCheckTouchables.Add(touchable);
                }
            }
        }

        /// <summary>
        /// Middle proximity check stays in contact with a touchable object
        /// </summary>
        /// <param name="other"></param>
        void MiddleGraspCheckStayHandle(Collider other)
        {
            if (other.isTrigger)
                return;

            if (_middleThimbleTracking.IsBlocked)
                return;

            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                if (!_middleGraspCheckTouchables.Contains(touchable))
                {
                    _middleGraspCheckTouchables.Add(touchable);
                }
            }
        }

        /// <summary>
        /// Middle proximity check exits a touchable object
        /// </summary>
        /// <param name="other"></param>
        void MiddleGraspCheckExitHandle(Collider other)
        {
            if (other.isTrigger)
                return;

            if (TryGetTouchableObjectFromCollider(other, out var touchable))
            {
                if (_middleGraspCheckTouchables.Contains(touchable))
                {
                    _middleGraspCheckTouchables.Remove(touchable);
                }
            }
        }

        #endregion

    }
}