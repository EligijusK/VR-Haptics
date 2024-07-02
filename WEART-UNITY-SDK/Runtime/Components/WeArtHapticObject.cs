using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeArt.Core;
using WeArt.Messages;
using Texture = WeArt.Core.Texture;

namespace WeArt.Components
{
    /// <summary>
    /// This component controls the haptic actuators of one or more hardware thimbles.
    /// The haptic control can be issued:
    /// 1) Manually from the Unity inspector
    /// 2) When a <see cref="WeArtTouchableObject"/> collides with this object
    /// 3) On custom haptic effects added or removed
    /// 4) On direct value set, through the public properties
    /// </summary>
    public class WeArtHapticObject : MonoBehaviour
    {
        [SerializeField]
        internal HandSideFlags _handSides = HandSideFlags.None;

        [SerializeField]
        internal ActuationPointFlags _actuationPoints = ActuationPointFlags.None;

        [SerializeField]
        internal Temperature _temperature = Temperature.Default;

        [SerializeField]
        internal Force _force = Force.Default;

        [SerializeField]
        internal Texture _texture = Texture.Default;

        [NonSerialized]
        internal IWeArtEffect _activeEffect;

        // The hand controller that owns this haptic object
        private WeArtHandController _handController;

        private List<WeArtTouchableObject> _touchedObjects = new List<WeArtTouchableObject>();

        //Delegates for Trigger events
        public Action<Collider> TriggerEnter = delegate { };

        public Action<Collider> TriggerStay = delegate { };

        public Action<Collider> TriggerExit = delegate { };

        // The dynamic force applied on the touch divers fingers based on physical peressure
        private float _physicalForce;

        // The touchable objeect that hand controller is grasping if this finger is touching it
        private WeArtTouchableObject _touchableObject;

        // If the parent hand controller is grasping something and this haptic object is touchingit
        private bool _isGrasping = false;

        // Is inside a trigger touchable object
        private bool _isAffectedByTrigger = false;

        private int _nonTriggerTouchedObjects = 0;

        // Used for texture velocity calculations
        private Vector3 _lastPosition;
        private float _lastTime;
        private bool _isUsedByHandController = false;

        // Used for dynamic force calculations
        private const float _touchedObjectForcePower = 0.5f;
        private const float _finalForcePower = 0.5f;
        private const float _initialForceOffset = 0.5f;

        private bool isActuating = false;
        
        /// <summary>
        /// Returns physical force
        /// </summary>
        public float PhysicalForce
        {
            get { return _physicalForce;  }
        }

        /// <summary>
        /// In case of the hand controller is grasping something, if the  finger touches it, set the touched object
        /// </summary>
        /// <param name="tObject"></param>
        public void SetTouchableObject(WeArtTouchableObject tObject)
        {
            _touchableObject = tObject;
        }


        /// <summary>
        /// Set if the setup is based on a hand controller
        /// </summary>
        /// <param name="value"></param>
        public void SetIsUsedByController(bool value)
        {
            _isUsedByHandController = value;
        }

        /// <summary>
        /// Get and set the hand controller
        /// </summary>
        public WeArtHandController HandController
        {
            get { return _handController; }
            set { _handController = value; }
        }

        /// <summary>
        /// Get touched object, grasped by the hand controller 
        /// </summary>
        /// <returns></returns>
        public WeArtTouchableObject GetTouchableObject()
        {
            return _touchableObject;
        }

        /// <summary>
        /// Set pshysical force
        /// </summary>
        /// <param name="pForce"></param>
        public void SetPhysicalForce(float pForce)
        {
            _physicalForce = Mathf.Clamp( (_initialForceOffset + (_touchedObjectForcePower * _force.Value)) * pForce * _finalForcePower, WeArtConstants.minForce, WeArtConstants.maxForce);

            if(_isAffectedByTrigger)
                _physicalForce = Mathf.Clamp(_force.Value, WeArtConstants.minForce, WeArtConstants.maxForce);

            if (_force.Active)
                SendSetForce();
        }

        /// <summary>
        /// Get if the haptic object is inside a trigger touchable object
        /// </summary>
        /// <returns></returns>
        public bool GetIsAffectedByTrigger()
        {
            return _isAffectedByTrigger;
        }

        /// <summary>
        /// Called when the resultant haptic effect changes because of the influence
        /// caused by the currently active effects
        /// </summary>
        public event Action OnActiveEffectsUpdate;

        private void Start()
        {
            Vector3 _lastPosition = transform.position;
            float _lastTime = Time.time;
        }

        private void Update()
        {
            if (_isGrasping)
            {
                _isAffectedByTrigger = false;
            }
            else
            {
                if (_isAffectedByTrigger)
                {
                    // Calculate texture velocity
                    float dx = Vector3.Distance(transform.position, _lastPosition);
                    float dt = Mathf.Max(Mathf.Epsilon, Time.time - _lastTime );
                    float slidingSpeed = WeArtConstants.defaultCollisionMultiplier * (dx / dt) * WeArtConstants.textureVelocitySensitivity;
                    _texture.Velocity = slidingSpeed;
                    Texture = _texture;

                    _lastPosition = transform.position;
                    _lastTime = Time.time;
                }
                else
                {
                    if(TouchedObjects.Count == 0 && _activeEffect!= null)
                    {
                        _activeEffect = null;
                        UpdateEffects();
                    }
                }
            }
        }

        /// <summary>
        /// The hand sides to control with this component
        /// </summary>
        public HandSideFlags HandSides
        {
            get => _handSides;
            set
            {
                if (value != _handSides)
                {
                    var sidesToStop = _handSides ^ value & _handSides;
                    _handSides = sidesToStop;
                    StopControl();

                    _handSides = value;
                    StartControl();
                }
            }
        }

        /// <summary>
        /// The thimbles to control with this component
        /// </summary>
        public ActuationPointFlags ActuationPoints
        {
            get => _actuationPoints;
            set
            {
                if (value != _actuationPoints)
                {
                    var pointsToStop = _actuationPoints ^ value & _actuationPoints;
                    _actuationPoints = pointsToStop;
                    StopControl();

                    _actuationPoints = value;
                    StartControl();
                }
            }
        }

        /// <summary>
        /// Sets and Gets if the hand haptic objeect if it is currently grasping
        /// </summary>
        public bool IsGrasping
        {
            get => _isGrasping;
            set 
            { 
                _isGrasping = value;

                if (_texture.Active)
                    SendSetTexture();
                else
                    SendStopTexture();
            }
        }

        /// <summary>
        /// The current temperature of the specified thimbles
        /// </summary>
        public Temperature Temperature
        {
            get => _temperature;
            set
            {
                if (!_temperature.Equals(value))
                {
                    _temperature = value;

                    if (value.Active)
                        SendSetTemperature();
                    else
                        SendStopTemperature();
                }
            }
        }

        /// <summary>
        /// The current pressing force of the specified thimbles
        /// </summary>
        public Force Force
        {
            get => _force;
            set
            {
                if (!_force.Equals(value))
                {
                    _force = value;

                    if(!_isUsedByHandController)
                    _physicalForce = _force.Value;

                    if (value.Active)
                        SendSetForce();
                    else
                        SendStopForce();
                }
            }
        }

        /// <summary>
        /// The current texture feeling applied on the specified thimbles
        /// </summary>
        public Texture Texture
        {
            get => _texture;
            set
            {
                if (!_texture.Equals(value))
                {
                    _texture = value;
                    if (value.Active)
                        SendSetTexture();
                    else
                        SendStopTexture();
                }
            }
        }

        /// <summary>
        /// The currently active effects on this object
        /// </summary>
        public IWeArtEffect ActiveEffect => _activeEffect;

        /// <summary>
        /// The currently active effects on this object
        /// </summary>
        public IReadOnlyList<WeArtTouchableObject> TouchedObjects => _touchedObjects;

        /// <summary>
        /// Add to the touched objects list only if the object does not exist already in the list
        /// </summary>
        /// <param name="obj"></param>
        public void AddTouchedObject(WeArtTouchableObject obj)
        {
            if(!_touchedObjects.Contains(obj))
            {
                _touchedObjects.Add(obj);
            }
        }

        /// <summary>
        /// Removes form the touched objects list only if the object exists already in the list
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveTouchedObject(WeArtTouchableObject obj)
        {
            if (_touchedObjects.Contains(obj))
            {
                _touchedObjects.Remove(obj);
            }
        }

        /// <summary>
        /// Clears all touched objects
        /// </summary>
        public void ClearTouchedObjects()
        {
            _touchedObjects.Clear();
        }

        /// <summary>
        /// When the touchable object gets disabled, signal to the hand controller and request a release
        /// </summary>
        /// <param name="touchable"></param>
        public void UnblockFingerOnDisable(WeArtTouchableObject touchable)
        {
            if(_handController != null)
            {
                _handController.UnblockFingerOnDisable(this, touchable);
            }

        }

        /// <summary>
        /// Adds a haptic effect to this object. This effect will have an influence
        /// as long as it is not removed or the haptic properties are programmatically
        /// forced to have a specified value.
        /// </summary>
        /// <param name="effect">The haptic effect to add to this object</param>
        public void AddEffect(IWeArtEffect effect)
        {
            _activeEffect = effect;
            UpdateEffects();
            effect.OnUpdate += UpdateEffects;
        }

        /// <summary>
        /// Removes a haptic effect from the set of influencing effects
        /// </summary>
        /// <param name="effect">The haptic effect to remove</param>
        public void RemoveEffect(IWeArtEffect effect)
        {
            if (_touchedObjects.Count > 0)
            {
                if (effect != null)
                    effect.OnUpdate -= UpdateEffects;

                if (!_isGrasping)
                {
                    WeArtTouchEffect touchEffect = new WeArtTouchEffect();
                    touchEffect.Set(_touchedObjects[0].Temperature, _touchedObjects[0].Stiffness, _touchedObjects[0].Texture, new WeArtTouchEffect.WeArtImpactInfo());
                    AddEffect(touchEffect);
                }
            }
            else
                _activeEffect = null;

            UpdateEffects();
            if (effect != null)
                effect.OnUpdate -= UpdateEffects;

        }

        /// <summary>
        /// On realease removes all touchable objects and returns to the starting state
        /// </summary>
        /// <param name="effect"></param>
        public void Release(IWeArtEffect effect)
        {
            if (effect != null)
                effect.OnUpdate -= UpdateEffects;

            if (_touchedObjects.Count > 0)
            {
                if (!_isGrasping)
                {
                    WeArtTouchEffect touchEffect = new WeArtTouchEffect();
                    touchEffect.Set(_touchedObjects[0].Temperature, _touchedObjects[0].Stiffness, _touchedObjects[0].Texture, new WeArtTouchEffect.WeArtImpactInfo());
                    AddEffect(touchEffect);
                }
            }
            else
            {
                _activeEffect = null;
                UpdateEffects();
            }
        }

        /// <summary>
        /// Get the active effect
        /// </summary>
        /// <returns></returns>
        public IWeArtEffect GetEffect()
        {
            return _activeEffect;
        }

        /// <summary>
        /// Internally updates the resultant haptic effect caused by the set of active effects.
        /// </summary>
        public void UpdateEffects()
        {
            var lastTemperature = Temperature.Default;
            if (_activeEffect != null)
            {
                lastTemperature = _activeEffect.Temperature;
            }

            Temperature = lastTemperature;

            var lastForce = Force.Default;
            if (_activeEffect != null)
            {
                lastForce = _activeEffect.Force;
            }
            Force = lastForce;

            var lastTexture = Texture.Default;
            if (_activeEffect != null)
            {
                lastTexture = _activeEffect.Texture;
            }
            Texture = lastTexture;

            OnActiveEffectsUpdate?.Invoke();
        }

        private void Init()
        {
            var client = WeArtController.Instance.Client;
            client.OnConnectionStatusChanged -= OnConnectionChanged;
            client.OnConnectionStatusChanged += OnConnectionChanged;
        }

        private void OnEnable()
        {
            Init();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!other.isTrigger && other.GetComponent<WeArtTouchableObject>() != null)
            _nonTriggerTouchedObjects += 1;

            TriggerEnter?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            TriggerStay?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.isTrigger && other.GetComponent<WeArtTouchableObject>() != null)
                _nonTriggerTouchedObjects -= 1;

            TriggerExit?.Invoke(other);
        }

        public void EnablingActuating(bool enable)
        {
            isActuating = enable;
        }
        
        /// <summary>
        /// Checks if there are touchable objects inside a trigger touchable objects and applies them instead of the trigger
        /// </summary>
        public void CheckForNewEffect()
        {
            if (_isGrasping)
                return;

            if (_touchedObjects.Count > 0)
            {
                if (_nonTriggerTouchedObjects > 0)
                {
                    bool foundNonTriggerTouchable = false;
                    foreach (var obj in _touchedObjects)
                    {
                        if (!obj.GetComponent<Collider>().isTrigger)
                        {
                            _isAffectedByTrigger = false;
                            foundNonTriggerTouchable = true;
                            break;
                        }
                    }

                    if (!foundNonTriggerTouchable)
                    {
                        if (_touchedObjects.Count > _nonTriggerTouchedObjects)
                        {
                            foreach (var obj in _touchedObjects)
                            {
                                if (obj.GetComponent<Collider>().isTrigger)
                                {
                                    _isAffectedByTrigger = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    _isAffectedByTrigger = true;
                }
            }
            else
            { 
                _isAffectedByTrigger = false; 
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gameObject.scene.IsValid())
                Init();
        }
#endif

        internal void OnConnectionChanged(bool connected)
        {
            if (connected)
                StartControl();
        }

        internal void StartControl()
        {
            if (Temperature.Active)
                SendSetTemperature();

            if (Force.Active)
                SendSetForce();

            if (Texture.Active)
                SendSetTexture();
        }

        internal void StopControl()
        {
            if (Temperature.Active)
                SendStopTemperature();

            if (Force.Active)
                SendStopForce();

            if (Texture.Active)
                SendStopTexture();
        }

        // Messages
        private void SendSetTemperature() => SendMessage((handSide, actuationPoint) => new SetTemperatureMessage()
        {
            Temperature = _temperature.Value,
            HandSide = handSide,
            ActuationPoint = actuationPoint
        });

        private void SendStopTemperature() => SendMessage((handSide, actuationPoint) => new StopTemperatureMessage()
        {
            HandSide = handSide,
            ActuationPoint = actuationPoint
        });

        private void SendSetForce() => SendMessage((handSide, actuationPoint) => new SetForceMessage()
        {
            Force =new float[] { _physicalForce, _physicalForce, _physicalForce },
            HandSide = handSide,
            ActuationPoint = actuationPoint
        });

        private void SendStopForce() => SendMessage((handSide, actuationPoint) => new StopForceMessage()
        {
            HandSide = handSide,
            ActuationPoint = actuationPoint
        });

        private void SendSetTexture() => SendMessage((handSide, actuationPoint) => new SetTextureMessage()
        {
            TextureIndex = (int)_texture.TextureType,
            TextureVelocity = new float[] { WeArtConstants.defaultTextureVelocity_X, WeArtConstants.defaultTextureVelocity_Y,
               _texture.ForcedVelocity?WeArtConstants.defaultTextureVelocity_X: (_isGrasping? 0: _texture.Velocity) },
            TextureVolume = _texture.Volume,
            HandSide = handSide,
            ActuationPoint = actuationPoint
        });

        private void SendStopTexture() => SendMessage((handSide, actuationPoint) => new StopTextureMessage()
        {
            HandSide = handSide,
            ActuationPoint = actuationPoint
        });

        private void SendMessage(Func<HandSide, ActuationPoint, IWeArtMessage> createMessage)
        {
            if (!isActuating) return;
            
            var controller = WeArtController.Instance;
            if (controller == null)
                return;

            foreach (var handSide in WeArtConstants.HandSides)
                if (HandSides.HasFlag((HandSideFlags)(1 << (int)handSide)))
                    foreach (var actuationPoint in WeArtConstants.ActuationPoints)
                        if (ActuationPoints.HasFlag((ActuationPointFlags)(1 << (int)actuationPoint)))
                            controller.Client.SendMessage(createMessage(handSide, actuationPoint));
        }
    }
}