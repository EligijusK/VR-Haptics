using System;
using System.Collections.Generic;
using UnityEngine;
using WeArt.Core;
using Texture = WeArt.Core.Texture;

namespace WeArt.Components
{
    /// <summary>
    /// Use this component to add haptic properties to an object. These properties will create
    /// an haptic effect on <see cref="WeArtHapticObject"/>s on collision (both objects
    /// need to have physical components such as a <see cref="Rigidbody"/> and at least one
    /// <see cref="Collider"/>.
    /// </summary>
    public class WeArtTouchableObject : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// Defines the _temperature.
        /// </summary>
        [SerializeField]
        internal Temperature _temperature = Temperature.Default;

        /// <summary>
        /// Defines the _stiffness.
        /// </summary>
        [SerializeField]
        internal Force _stiffness = Force.Default;

        /// <summary>
        /// Defines the _texture.
        /// </summary>
        [SerializeField]
        internal Texture _texture = Texture.Default;

        /// <summary>
        /// Defines the _volumeTexture.
        /// </summary>
        [SerializeField]
        internal float _volumeTexture = WeArtConstants.defaultVolumeTexture;

        /// <summary>
        /// Defines the _forcedVelocity.
        /// </summary>
        [SerializeField]
        internal bool _forcedVelocity = false;

        /// <summary>
        /// Defines the _graspable.
        /// </summary>
        [SerializeField]
        internal bool _graspable = false;

        [SerializeField]
        internal bool _surfaceExploration = false;

        /// <summary>
        /// Defines the _touchedHapticsEffects.
        /// </summary>
        [NonSerialized]
        internal Dictionary<WeArtHapticObject, WeArtTouchEffect> _touchedHapticsEffects =
             new Dictionary<WeArtHapticObject, WeArtTouchEffect>();

        /// <summary>
        /// Defines the _parentGameObject.
        /// </summary>
        private Transform _parentGameObject;

        /// <summary>
        /// Defines the _originalUseGravity.
        /// </summary>
        private bool _originalUseGravity;

        /// <summary>
        /// Defines the _originalIsKinematic.
        /// </summary>
        private bool _originalIsKinematic;

        /// <summary>
        /// Defines the _rigibody.
        /// </summary>
        private Rigidbody _rigibody;

        /// <summary>
        /// Defines the _graspingState.
        /// </summary>
        private GraspingState _graspingState = GraspingState.Released;

        /// <summary>
        /// The hand controller that is curently grasping the object
        /// </summary>
        private WeArtHandController _graspingHandController;

        #endregion

        #region Events

        /// <summary>
        /// Called when the set of affected haptic objects changed after collision events.
        /// </summary>
        public event Action OnAffectedHapticObjectsUpdate;

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the hand controller that is grasping the object
        /// </summary>
        public WeArtHandController GraspingHandController
        {
            get { return _graspingHandController; }
            set { _graspingHandController = value; }
        }

        /// <summary>
        /// Gets or sets the Temperature
        /// The temperature of this object.
        /// </summary>
        public Temperature Temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                UpdateTouchedHaptics();
            }
        }

        /// <summary>
        /// Gets the grasping state of the object
        /// </summary>
        public GraspingState GetGraspingState()
        {
            return _graspingState;
        }

        /// <summary>
        /// Gets or sets the Stiffness
        /// The stiffness of this object, that will translate as a haptic force.
        /// </summary>
        public Force Stiffness
        {
            get => _stiffness;
            set
            {
                _stiffness = value;
                UpdateTouchedHaptics();
            }
        }

        /// <summary>
        /// Gets or sets the Texture
        /// The haptic texture of this object.
        /// </summary>
        public Texture Texture
        {
            get => _texture;
            set
            {
                if (value.Equals(Texture.Default))
                {
                    Debug.LogError("The texture selected has no custom fields");
                }

                _texture.Active = value.Active;
                _texture.TextureType = value.TextureType;
                _texture.Volume = value.Volume;
                _texture.Velocity = value.Velocity;
                _texture.ForcedVelocity = value.ForcedVelocity;
                ForcedVelocity = value.ForcedVelocity;
                VolumeTexture = value.Volume;

                UpdateTouchedHaptics();
            }
        }


        /// <summary>
        /// Gets and sets the graspable state of the touchable object.
        /// </summary>
        public bool Graspable
        {
            get => _graspable;
            set => _graspable = value;
        }

        /// <summary>
        /// Gets or sets the CollisionMultiplier
        /// A multiplier between 0 and 1 that controls how much the collision speed affects
        /// the perceived stiffness and the texture velocity.
        /// </summary>
        private float CollisionMultiplier
        {
            get => WeArtConstants.defaultCollisionMultiplier;
        }

        /// <summary>
        /// Gets or sets the VolumeTexture
        /// A multiplier between 0 and 100 that controls how much the intensity of texture effect
        /// </summary>
        public float VolumeTexture
        {
            get => WeArtConstants.defaultCollisionMultiplier;
            set
            {
                _volumeTexture = Mathf.Clamp((float)value, WeArtConstants.minVolumeTexture, WeArtConstants.maxVolumeTexture);
                _texture.Volume = _volumeTexture;
                UpdateTouchedHaptics();
            }
        }

        /// <summary>
        /// Set if the velocity should be forced.
        /// </summary>
        public bool ForcedVelocity
        {
            get => _forcedVelocity;
            set
            {
                _forcedVelocity = value;
                _texture.ForcedVelocity = value;
                UpdateTouchedHaptics();
            }
        }

        /// <summary>
        /// Gets the AffectedHapticObjects
        /// The collection of haptic objects currently touching this object.
        /// </summary>
        public IReadOnlyCollection<WeArtHapticObject> AffectedHapticObjects => _touchedHapticsEffects.Keys;

        /// <summary>
        /// Gets a value indicating whether IsGraspable.
        /// </summary>
        public bool IsGraspable { get => _graspable; }


        /// <summary>
        /// The original rigidbody constrains
        /// </summary>
        private RigidbodyConstraints _originalRigidbodyConstraints;

        #endregion

        #region Methods

        /// <summary>
        /// The OnCollisionEnter.
        /// </summary>
        /// <param name="collision">The collision<see cref="Collision"/>.</param>
        private void OnCollisionEnter(Collision collision) => OnColliderEnter(collision.collider);

        /// <summary>
        /// The OnCollisionStay.
        /// </summary>
        /// <param name="collision">The collision<see cref="Collision"/>.</param>
        private void OnCollisionStay(Collision collision) => OnColliderStay(collision.collider);

        /// <summary>
        /// The OnCollisionExit.
        /// </summary>
        /// <param name="collision">The collision<see cref="Collision"/>.</param>
        private void OnCollisionExit(Collision collision) => OnColliderExit(collision.collider);

        /// <summary>
        /// The OnTriggerEnter.
        /// </summary>
        /// <param name="trigger">The trigger<see cref="Collider"/>.</param>
        private void OnTriggerEnter(Collider trigger) => OnColliderEnter(trigger);

        /// <summary>
        /// The OnTriggerStay.
        /// </summary>
        /// <param name="trigger">The trigger<see cref="Collider"/>.</param>
        private void OnTriggerStay(Collider trigger) => OnColliderStay(trigger);

        /// <summary>
        /// The OnTriggerExit.
        /// </summary>
        /// <param name="trigger">The trigger<see cref="Collider"/>.</param>
        private void OnTriggerExit(Collider trigger) => OnColliderExit(trigger);

        /// <summary>
        /// The OnColliderEnter.
        /// </summary>
        /// <param name="collider">The collider<see cref="Collider"/>.</param>
        private void OnColliderEnter(Collider collider)
        {
            if (_graspingState == GraspingState.Grabbed)
                return;

            if (TryGetHapticObjectFromCollider(collider, out var hapticObject))
            {
                if (hapticObject.IsGrasping)
                    return;

                var effect = new WeArtTouchEffect();
                effect.Set(Temperature, Stiffness, Texture, new WeArtTouchEffect.WeArtImpactInfo()
                {
                    Position = collider.transform.position,
                    Time = Time.time,
                    Multiplier = CollisionMultiplier
                });
                hapticObject.AddEffect(effect);
                _touchedHapticsEffects[hapticObject] = effect;
                OnAffectedHapticObjectsUpdate?.Invoke();

                hapticObject.AddTouchedObject(this);
            }
        }

        /// <summary>
        /// WeArtHandController force OnColliderEnter
        /// </summary>
        /// <param name="collider"></param>
        public void ForceOnColliderEnter(Collider collider)
        {
            if (TryGetHapticObjectFromCollider(collider, out var hapticObject))
            {
                var effect = new WeArtTouchEffect();
                effect.Set(Temperature, Stiffness, Texture, new WeArtTouchEffect.WeArtImpactInfo()
                {
                    Position = collider.transform.position,
                    Time = Time.time,
                    Multiplier = CollisionMultiplier
                });
                hapticObject.AddEffect(effect);
                _touchedHapticsEffects[hapticObject] = effect;
                OnAffectedHapticObjectsUpdate?.Invoke();

                hapticObject.AddTouchedObject(this);
            }
        }

        /// <summary>
        /// The OnColliderStay.
        /// </summary>
        /// <param name="collider">The collider<see cref="Collider"/>.</param>
        private void OnColliderStay(Collider collider)
        {
            if (_graspingState == GraspingState.Grabbed)
                return;

            if (TryGetHapticObjectFromCollider(collider, out var hapticObject))
            {
                if (hapticObject.IsGrasping)
                    return;

                if (_touchedHapticsEffects.TryGetValue(hapticObject, out var effect))
                {
                    effect.Set(Temperature, Stiffness, Texture, new WeArtTouchEffect.WeArtImpactInfo()
                    {
                        Position = collider.transform.position,
                        Time = Time.time,
                        Multiplier = CollisionMultiplier
                    });
                }
            }
        }

        /// <summary>
        /// WeArtHandController force OnColliderStay
        /// </summary>
        /// <param name="collider"></param>
        public void ForceOnColliderStay(Collider collider)
        {
            if (TryGetHapticObjectFromCollider(collider, out var hapticObject))
            {
                if (_touchedHapticsEffects.TryGetValue(hapticObject, out var effect))
                {
                    effect.Set(Temperature, Stiffness, Texture, new WeArtTouchEffect.WeArtImpactInfo()
                    {
                        Position = collider.transform.position,
                        Time = Time.time,
                        Multiplier = CollisionMultiplier
                    });
                }
            }
        }

        /// <summary>
        /// The OnColliderExit.
        /// </summary>
        /// <param name="collider">The collider<see cref="Collider"/>.</param>
        private void OnColliderExit(Collider collider)
        {
            if (_graspingState == GraspingState.Grabbed)
                return;

            if (TryGetHapticObjectFromCollider(collider, out var hapticObject))
            {
                hapticObject.RemoveTouchedObject(this);
                _touchedHapticsEffects.Remove(hapticObject);
                hapticObject.RemoveEffect(hapticObject.ActiveEffect);
                OnAffectedHapticObjectsUpdate?.Invoke();
            }
        }

        /// <summary>
        /// WeArtHandController force OnColliderExit
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="isOnDisable"></param>
        public void ForceOnColliderExit(Collider collider, bool isOnDisable = false)
        {
            if (TryGetHapticObjectFromCollider(collider, out var hapticObject))
            {
                if(!isOnDisable)
                _touchedHapticsEffects.Remove(hapticObject);

                hapticObject.RemoveTouchedObject(this);
                hapticObject.RemoveEffect(hapticObject.ActiveEffect);
                OnAffectedHapticObjectsUpdate?.Invoke();
            }
        }

        /// <summary>
        /// The UpdateTouchedHaptics.
        /// </summary>
        private void UpdateTouchedHaptics()
        {
            foreach (var pair in _touchedHapticsEffects)
            {
                pair.Value.Set(Temperature, Stiffness, Texture, null);
            }
        }


        /// <summary>
        /// Set initial values on Awake.
        /// </summary>
        private void Awake()
        {
            _texture.ForcedVelocity = ForcedVelocity;
            _rigibody = gameObject.GetComponent<Rigidbody>();
            _originalUseGravity = _rigibody.useGravity;
            _originalIsKinematic = _rigibody.isKinematic;
            _parentGameObject = gameObject.transform.parent;
            _originalRigidbodyConstraints = _rigibody.constraints;
        }

        /// <summary>
        /// The OnDisable.
        /// </summary>
        private void OnDisable()
        {
            foreach(var haptic in _touchedHapticsEffects)
            {
                if (haptic.Key != null)
                {
                    ForceOnColliderExit(haptic.Key.GetComponent<Collider>(),true);
                    haptic.Key.UnblockFingerOnDisable(this);
                }
            }
            _touchedHapticsEffects.Clear();
            OnAffectedHapticObjectsUpdate?.Invoke();
        }


        /// <summary>
        /// The TryGetHapticObjectFromCollider.
        /// </summary>
        /// <param name="collider">The collider<see cref="Collider"/>.</param>
        /// <param name="hapticObject">The hapticObject<see cref="WeArtHapticObject"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private static bool TryGetHapticObjectFromCollider(Collider collider, out WeArtHapticObject hapticObject)
        {
            hapticObject = collider.gameObject.GetComponent<WeArtHapticObject>();
            return hapticObject != null;
        }

        /// <summary>
        /// The Grab.
        /// </summary>
        /// <param name="grasper">The grasper<see cref="GameObject"/>.</param>
        public void Grab(GameObject grasper)
        {
            _graspingState = GraspingState.Grabbed;

            _rigibody.useGravity = false;
            _rigibody.isKinematic = false;
            _rigibody.constraints = RigidbodyConstraints.FreezeAll;

            transform.parent = grasper.transform;

            FixedJoint joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = grasper.transform.parent.GetComponent<Rigidbody>();
            
        }

        /// <summary>
        /// The Release.
        /// </summary>
        public void Release()
        {
            if (GetComponent<FixedJoint>()!=null)
            {
                GetComponent<FixedJoint>().connectedBody= null;
                Destroy(GetComponent<FixedJoint>());
            }

            transform.parent = _parentGameObject;

            _rigibody.useGravity = _originalUseGravity;
            _rigibody.isKinematic = _originalIsKinematic;
            _rigibody.constraints = _originalRigidbodyConstraints;
            _graspingState = GraspingState.Released;
        }

        /// <summary>
        /// The CompareInstanceID.
        /// </summary>
        /// <param name="instanceID">The instanceID<see cref="int"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool CompareInstanceID(int instanceID)
        {
            return gameObject.GetInstanceID().Equals(instanceID);
        }

        #endregion
       
    }
}
