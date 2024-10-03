using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace WeArt.Components
{
    /// <summary>
    /// This component can be used to follow a specified spatially tracked transform.
    /// Add it to the root object of your avatar virtual hand. Make sure to specify
    /// the right spatial offset between the tracked object and the WeArt device.
    /// </summary>
    public class WeArtDeviceTrackingObject : MonoBehaviour
    {
        /// <summary>
        /// Possible tracking follow behaviours
        /// </summary>
        public enum TrackingUpdateMethod
        {
            TransformUpdate,
            TransformLateUpdate,
            RigidbodyUpdate,
        }

        [SerializeField]
        internal WeArtHandController _controller;

        [SerializeField]
        internal WeArtGhostHandController _ghostHandDeviceTracking;

        [SerializeField]
        internal TrackingUpdateMethod _updateMethod;

        [SerializeField]
        internal Transform _trackingSource;

        [SerializeField]
        internal Vector3 _positionOffset;

        [SerializeField]
        internal Vector3 _rotationOffset;

        [NonSerialized]
        internal Rigidbody _rigidBody;

        [SerializeField] 
        internal Renderer _ghostHandRenderer;

        [SerializeField]
        internal bool _showGhostHand = true;

        [SerializeField]
        internal float _ghostHandShowDistance = 0.05f;

        private WeArtHandController _handController;

        private Material _invisibleMaterial;

        private Material _visibleMaterial;

        private float _handFollowSpeed = 0.3f;

        private float _handFollowPowerDuringGrab = 3;

        /// <summary>
        /// The method to use in order to update the position and the rotation of this device
        /// </summary>
        public TrackingUpdateMethod UpdateMethod
        {
            get => _updateMethod;
            set => _updateMethod = value;
        }

        /// <summary>
        /// The transform attached to the tracked device object
        /// </summary>
        public Transform TrackingSource
        {
            get => _trackingSource;
            set => _trackingSource = value;
        }

        /// <summary>
        /// The position offset between this device and the tracked one
        /// </summary>
        public Vector3 PositionOffset
        {
            get => _positionOffset;
            set => _positionOffset = value;
        }

        /// <summary>
        /// The rotation offset between this device and the tracked one
        /// </summary>
        public Vector3 RotationOffset
        {
            get => _rotationOffset;
            set => _rotationOffset = value;
        }

        private void Awake()
        {
                // Find ghost hand
                int childrenCount = transform.childCount;
                for (int i = 0; i < childrenCount; ++i)
                {
                    if (transform.GetChild(i).GetComponent<WeArtGhostHandController>() != null)
                    {
                        _ghostHandDeviceTracking = transform.GetChild(i).GetComponent<WeArtGhostHandController>();
                        break;
                    }
                }

                // Find ghost hand renderer
                childrenCount = _ghostHandDeviceTracking.transform.childCount;
                for (int i = 0; i < _ghostHandDeviceTracking.transform.childCount; ++i)
                {
                    if (_ghostHandDeviceTracking.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>() != null)
                    {
                        _ghostHandRenderer = _ghostHandDeviceTracking.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
                        break;
                    }
                }
                _rigidBody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _handController = GetComponent<WeArtHandController>();
        }

        private void Update()
        {
            if (UpdateMethod == TrackingUpdateMethod.TransformUpdate)
                UpdateHands();
        }

        private void LateUpdate()
        {
            if (UpdateMethod == TrackingUpdateMethod.TransformLateUpdate)
                UpdateHands();
        }

        private void FixedUpdate()
        {
            if (UpdateMethod == TrackingUpdateMethod.RigidbodyUpdate)
                UpdateHands();

            UpdateRigidbody();
        }

        /// <summary>
        /// Updated the ghost hands real position and rotation in the real world
        /// </summary>
        private void UpdateHands()
        {
            _ghostHandDeviceTracking.transform.position = TrackingSource.TransformPoint(_positionOffset);
            _ghostHandDeviceTracking.transform.rotation = TrackingSource.rotation * Quaternion.Euler(_rotationOffset);

            if (_showGhostHand)
            {
                float distance = Vector3.Distance(_ghostHandDeviceTracking.transform.position, transform.position);

                if (distance > _ghostHandShowDistance)
                {
                    _ghostHandRenderer.sharedMaterial = _handController.GetGhostHandTransparentMaterial();
                }
                else
                {
                    _ghostHandRenderer.sharedMaterial = _handController.GetGhostHandInvisibleMaterial();
                }
            }
        }

        /// <summary>
        /// Applies velocity to the visible hand to track the position and rotation of the ghost hand
        /// </summary>
        private void UpdateRigidbody()
        {
            if (TrackingSource == null)
                return;

            // Get Rigid Body
            if (_rigidBody == null)
            {
                TryGetComponent<Rigidbody>(out Rigidbody rb);
                _rigidBody = rb;
                if (_rigidBody == null)
                {
                    Debug.LogWarning($"Cannot use {nameof(TrackingUpdateMethod.RigidbodyUpdate)} method without a {nameof(Rigidbody)}");
                    return;
                }
            }

            // Velocity and Rotation
            _rigidBody.velocity = (TrackingSource.TransformPoint(_positionOffset) - transform.position) / Time.fixedDeltaTime *_handFollowSpeed;
            if (_handController.GraspingState == Core.GraspingState.Grabbed)
                _rigidBody.velocity = _handFollowPowerDuringGrab * _rigidBody.velocity;

            // Absolute rotation 
            transform.rotation = TrackingSource.rotation * Quaternion.Euler(_rotationOffset);
        }
    }
}