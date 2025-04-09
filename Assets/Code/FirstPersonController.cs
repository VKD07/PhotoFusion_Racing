using System;
using Fusion;
using UnityEngine;

namespace Code
{
    public class FirstPersonController : NetworkBehaviour
    {
        [Header("Movement Speeds")]
        [SerializeField] private float _walkSpeed = 3f;
        [SerializeField] private float _sprintMultiplier = 2f;
        
        [Header("Movement Speeds")]
        [SerializeField] private float _jumpForce = 5f;
        [SerializeField] private float _gravityMultiplier = 1f;
                
        [Header("Look Parameters")]
        [SerializeField] private float _mouseSensitivity = 2f;
        [SerializeField] private float _upDownLookRange = 80f;
        
        [Header("References")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Camera _mainCamera;
        
        private PlayerInputHandler _playerInputHandler;
        private Vector3 _currentMovement;
        private float _verticalRotation;
        private float CurrentSpeed => _walkSpeed * (_playerInputHandler.SprintTriggered ? _sprintMultiplier : 1f);


        public void Init(PlayerInputHandler playerInputHandler)
        {
           // _playerInputHandler = playerInputHandler;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if (!Object.HasInputAuthority)
            {
                _mainCamera.gameObject.SetActive(false);
            }
        }
        
        public override void FixedUpdateNetwork() 
        {
            if (GetInput(out NetworkInputData data))
            {
                HandleMovement(data);
                HandleRotation(data);
                // _cc.Move(5*data.direction*Runner.DeltaTime);
            }
        }

        // private void Update()
        // {
        //     HandleMovement();
        //     HandleRotation();
        // }
        
        private Vector3 CalculateWorldDirection(NetworkInputData data)
        {
            Vector3 inputDirection = data.direction;
            Vector3 worldDirection = transform.TransformDirection(inputDirection);
            return worldDirection.normalized;
        }

        private void HandleMovement(NetworkInputData data)
        {
            Vector3 worldDirection = CalculateWorldDirection(data);
            _currentMovement.x = worldDirection.x * data.currentSpeed;
            _currentMovement.z = worldDirection.z * data.currentSpeed;

            _characterController.Move(_currentMovement * Runner.DeltaTime);
        }

        private void ApplyHorizontalRotation(float rotationAmount)
        {
            transform.Rotate(0, rotationAmount, 0);
        }

        private void ApplyVerticalRotation(float rotationAmount)
        {
            _verticalRotation = Mathf.Clamp(_verticalRotation - rotationAmount, -_upDownLookRange, _upDownLookRange);
            _mainCamera.transform.localRotation = Quaternion.Euler(_verticalRotation,0,0);
        }

        private void HandleRotation(NetworkInputData data)
        {
            // float mouseXRotation = _playerInputHandler.RotationInput.x * _mouseSensitivity;
            // float mouseYRotation = _playerInputHandler.RotationInput.y * _mouseSensitivity;
            
            // ApplyHorizontalRotation(mouseXRotation);
            // ApplyVerticalRotation(mouseYRotation);
            ApplyHorizontalRotation(data.mouseXRotation);
            ApplyVerticalRotation(data.mouseYRotation);
        }
    }
}