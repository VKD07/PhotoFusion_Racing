using System;
using Fusion;
using UnityEngine;

namespace Code
{
    public class FirstPersonController : NetworkBehaviour
    {
        public static FirstPersonController Instance;

        [Header("Movement Speeds")] [SerializeField]
        private float _walkSpeed = 3f;

        [SerializeField] private float _sprintMultiplier = 2f;

        [Header("Movement Speeds")] [SerializeField]
        private float _jumpForce = 5f;

        [SerializeField] private float _gravityMultiplier = 1f;

        [Header("Look Parameters")] [SerializeField]
        private float _mouseSensitivity = 2f;

        [SerializeField] private float _upDownLookRange = 80f;

        [Header("References")] [SerializeField]
        // private CharacterController _characterController;
        private NetworkCharacterController _characterController;

        [SerializeField] private Camera _mainCamera;

        private PlayerInputHandler _playerInputHandler;
        private Vector3 _currentMovement;
        private float _verticalRotation;
        private float CurrentSpeed => _walkSpeed * (_playerInputHandler.SprintTriggered ? _sprintMultiplier : 1f);


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (!Object.HasInputAuthority)
            {
                _mainCamera.enabled = false;
                _mainCamera.GetComponent<AudioListener>().enabled = false;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                HandleMovement(data);
                HandleRotation(data);
            }
        }

        private Vector3 CalculateWorldDirection(NetworkInputData data)
        {
            Vector3 inputDirection = data.direction;
            Vector3 worldDirection = transform.TransformDirection(inputDirection);
            return worldDirection.normalized;
        }

        private void HandleMovement(NetworkInputData data)
        {
            Vector3 worldDirection = CalculateWorldDirection(data);
            _currentMovement.x = worldDirection.x;
            _currentMovement.z = worldDirection.z;

            _characterController.Move(_currentMovement * Runner.DeltaTime);
        }

        private void ApplyHorizontalRotation(float rotationAmount)
        {
            transform.Rotate(0, rotationAmount, 0);
        }

        private void ApplyVerticalRotation(float rotationAmount, float pitchRotation)
        {
            _mainCamera.transform.localRotation = Quaternion.Euler(pitchRotation, 0, 0);
        }

        private void HandleRotation(NetworkInputData data)
        {
            ApplyHorizontalRotation(data.mouseXRotation);
            ApplyVerticalRotation(data.mouseYRotation, data.pupilVerticalPitch);
        }
    }
}