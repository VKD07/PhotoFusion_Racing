using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Code
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Input Action Asset")] [SerializeField]
        private InputActionAsset _playerControls;

        [Header("Action Map Name Reference")] [SerializeField]
        private string _actionMapName = "Player";

        [FormerlySerializedAs("movement")] [Header("Action Name Reference")] [SerializeField]
        private string _movement = "Movement";

        [SerializeField] private string _rotation = "Rotation";
        [SerializeField] private string _interact = "Interact";
        [SerializeField] private string _jump = "Jump";
        [SerializeField] private string _throw = "Throw";
        [SerializeField] private string _gun = "Gun";
        [SerializeField] private string _sprint = "Sprint";


        public float AccumulatedPitch;
        private InputAction _movementAction;
        private InputAction _rotationAction;
        private InputAction _interactAction;
        private InputAction _jumpAction;
        private InputAction _sprintAction;
        private InputAction _throwAction;
        private InputAction _gunAction;

        public Vector2 MovementInput { get; private set; }
        public Vector2 RotationInput { get; private set; }
        
        public bool JumpTriggered { get; private set; }
        public bool InteractTriggered { get; private set; }
        public bool SprintTriggered { get; private set; }
        public bool ThrowTriggered { get; private set; }

        public bool GunTriggered { get; private set; }


        private void Awake()
        {
            InputActionMap mapReference = _playerControls.FindActionMap(_actionMapName);

            _movementAction = mapReference.FindAction(_movement);
            _rotationAction = mapReference.FindAction(_rotation);
            _interactAction = mapReference.FindAction(_interact);
            _throwAction = mapReference.FindAction(_throw);
            _gunAction = mapReference.FindAction(_gun);
            // jumpAction = mapReference.FindAction(jump);
            //sprintAction = mapReference.FindAction(sprint);

            SubscribeActionValuesToInputEvents();
        }

        private void SubscribeActionValuesToInputEvents()
        {
            _movementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
            _movementAction.canceled += inputInfo => MovementInput = Vector2.zero;

            _rotationAction.performed += inputInfo => RotationInput = inputInfo.ReadValue<Vector2>();
            _rotationAction.canceled += inputInfo => RotationInput = Vector2.zero;
            
            _interactAction.performed += inputInfo => InteractTriggered = true;
            _interactAction.canceled += inputInfo => InteractTriggered = false;

            _throwAction.performed += inputInfo => ThrowTriggered = true;
            _throwAction.canceled += inputInfo => ThrowTriggered = false;

            _gunAction.performed += inputInfo => GunTriggered = true;
            
            if (_gunAction.WasReleasedThisFrame())
            {
                GunTriggered = false;
            }
            

            // _jumpAction.performed += inputInfo => JumpTriggered = true;
            //jumpAction.canceled += inputInfo => JumpTriggered = false;

            //sprintAction.performed += inputInfo => SprintTriggered = true;
            //sprintAction.canceled += inputInfo => SprintTriggered = false;
        }

        private void OnEnable()
        {
            _playerControls.FindActionMap(_actionMapName).Enable();
        }

        private void OnDisable()
        {
            _playerControls.FindActionMap(_actionMapName).Disable();
        }
    }
}