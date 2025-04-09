using System;
using Code;
using Fusion;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : NetworkBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private CinemachineCamera _cinemachineCamera;

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;

    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheeTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    public NetworkObject PlayerNetworkObject => Object;

    private void Awake()
    {
        _cinemachineCamera = FindAnyObjectByType<CinemachineCamera>();
    }

    public override void Spawned()
    {
        if (_cinemachineCamera.Follow == null)
        {
            _cinemachineCamera.Follow = transform;
        }
    }

    public override void FixedUpdateNetwork()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }


    private void GetInput()
    {
        // horizontalInput = Input.GetAxis(HORIZONTAL);
        // verticalInput = Input.GetAxis(VERTICAL);
        // isBreaking = Input.GetKey(KeyCode.Space);

        if (GetInput(out NetworkInputData data))
        {
            horizontalInput = data.horizontal;
            verticalInput = data.vertical;

            isBreaking = data.buttons.IsSet(NetworkInputData.SPACEBUTTON);
        }
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheeTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    [SerializeField] private TextMeshProUGUI _messages;

    private void Update()
    {
        if (Object.HasInputAuthority && Input.GetKeyDown(KeyCode.R))
        {
            RPC_SendMessage("Hey Mate!");
        }
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SendMessage(string message, RpcInfo info = default)
    {
        RPC_RelayMessage(message, info.Source);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RelayMessage(string message, PlayerRef messageSource)
    {
        if (_messages == null)
            _messages = FindObjectOfType<TextMeshProUGUI>();

        if (messageSource == Runner.LocalPlayer)
        {
            message = $"You said: {message}\n";
        }
        else
        {
            message = $"Some other player said: {message}\n";
        }

        _messages.text += message;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Lap>() != null && Object.HasStateAuthority)
        {
            RPC_SendMessage("Hey Mate!");
        }
    }
}