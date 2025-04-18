using Code;
using Fusion;
using UnityEngine;

public class PickUp : NetworkBehaviour
{
    public static PickUp Instance;

    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private Transform _holdPosition;
    [SerializeField] private float _throwForce = 500f;
    [SerializeField] private float _pickUpRange = 5f;
    private float _rotationSensitivity = 1f;
    private GameObject _heldObj;
    private Rigidbody _heldObjRb;
    private bool _canDrop;
    private int _LayerNumber;
    private const string PICKABLEOBJECT = "PickableObj";
    private RaycastHit _hit;
    private Transform _objectOnHand;
    private NetworkInputData _networkInputData;
    private PickableObj _pickable;

    public Vector3 HoldPosition()
    {
        return _holdPosition.position;
    }

    public Quaternion HoldRotation()
    {
        return _holdPosition.rotation;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _pickable = FindAnyObjectByType<PickableObj>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }

        if (GetInput(out _networkInputData))
        {
            if (_networkInputData.buttons.IsSet(NetworkInputData.INTERACTBUTTON))
            {
                if (Physics.Raycast(_playerCamera.position, _playerCamera.TransformDirection(Vector3.forward), out _hit,
                        _pickUpRange))
                {
                    if (_hit.transform.GetComponent<PickableObj>() != null)
                    {
                        NetworkObject netObj = _hit.transform.GetComponent<NetworkObject>();
                        Debug.Log(Object.Name);
                       RPC_RequestPickup(netObj, Object.InputAuthority);
                    }
                }
            }

            if (_networkInputData.buttons.IsSet(NetworkInputData.DROPBUTTON))
            {
                RPC_RequestDropObject();
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestPickup(NetworkObject targetObj, PlayerRef requestingPlayer)
    {
        PickableObj pickable = targetObj.GetComponent<PickableObj>();
        if (pickable != null)
        {
            pickable.AssignAuthority(requestingPlayer);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestDropObject()
    {
        if (_pickable != null)
        {
            Debug.Log("Object dropped");
            _pickable.DropObject();
            // if (requestingPlayer == Object.InputAuthority)
            // {
            //     PickUpObject(targetObj.gameObject);
            // }
        }
    }

    void ThrowObject()
    {
        Physics.IgnoreCollision(_heldObj.GetComponent<Collider>(), _player.GetComponent<Collider>(), false);
        _heldObj.layer = 0;
        _heldObjRb.isKinematic = false;
        _heldObj.transform.parent = null;
        _heldObjRb.AddForce(transform.forward * _throwForce);
        _heldObj = null;
    }

    void StopClipping()
    {
        var clipRange = Vector3.Distance(_heldObj.transform.position, transform.position);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        if (hits.Length > 1)
        {
            _heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f);
        }
    }
}