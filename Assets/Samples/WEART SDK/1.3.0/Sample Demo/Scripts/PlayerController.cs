using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace WEART
{
    public class PlayerController : MonoBehaviour
    {

        [SerializeField]
        private Transform _cameraTransform;

        [SerializeField]
        private float _speed = 0.5f;

        [SerializeField]
        private float _stepPosY = 0.05f;


        private void Awake()
        {
            if (!_cameraTransform) _cameraTransform = GetComponentInChildren<Camera>()?.transform;
        }

        // Update is called once per frame
        private void Update()
        {

            if (Input.GetKey(KeyCode.A))
            {
                transform.position -= _cameraTransform.right * (Time.deltaTime * _speed);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                //position.x += 0.05f;
                transform.position += _cameraTransform.right * (Time.deltaTime * _speed);
            }
            else if (Input.GetKey(KeyCode.W))
            {
                transform.position += _cameraTransform.forward * (Time.deltaTime * _speed);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                transform.position -= _cameraTransform.forward * (Time.deltaTime * _speed);
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                Vector3 position = transform.position;
                position.y -= _stepPosY;
                transform.position = position;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                Vector3 position = transform.position;
                position.y += _stepPosY;
                transform.position = position;
            }
            else if (Input.GetKey(KeyCode.Z))
            {
                transform.Rotate(0, -1, 0);
            }
            else if (Input.GetKey(KeyCode.X))
            {
                transform.Rotate(0, 1, 0);
            }
            else if (Input.GetKey(KeyCode.R))
            {
                List<InputDevice> devices = new List<InputDevice>();
                InputDevices.GetDevices(devices);
                if (devices.Count > 0)
                {
                    devices[0].subsystem.TryRecenter();
                }
            }
            else if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }

}