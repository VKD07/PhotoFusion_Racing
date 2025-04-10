using Code;
using Fusion;
using UnityEngine;

public class PickUp : NetworkBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private Transform _holdPosition;
    [SerializeField] private float _throwForce = 500f;
    [SerializeField] private float _pickUpRange = 5f;
    private float _rotationSensitivity = 1f;
    private GameObject _heldObj;
    private Rigidbody _heldObjRb;
    private bool _canDrop = true;
    private int _LayerNumber;
    private const string PICKABLEOBJECT = "PickableObj";
    private  RaycastHit _hit;
    private Transform _objectOnHand;
    private NetworkInputData _networkInputData;
    void Awake()
    {
        _LayerNumber = LayerMask.NameToLayer(PICKABLEOBJECT);
    }
    
    

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out _networkInputData))
        {
            if (_networkInputData.buttons.IsSet(NetworkInputData.INTERACTBUTTON))
            {
                if (_heldObj == null)
                {
                    if (Physics.Raycast(_playerCamera.position, _playerCamera.TransformDirection(Vector3.forward), out _hit, _pickUpRange))
                    {
                        if (_hit.transform.GetComponent<PickableObj>() != null && Object.HasInputAuthority)
                        {
                            var netObj = _hit.transform.GetComponent<NetworkObject>();
                            RPC_RequestPickup(netObj, Object.InputAuthority);
                            PickUpObject(_hit.transform.gameObject);
                        }
                    }
                }
                else
                {
                    if (_canDrop)
                    {
                        StopClipping();
                        DropObject();
                    }
                }
            }
        }
        
        

        if (_heldObj != null)
        {
            MoveObject();
            RotateObject();
            if (Input.GetKeyDown(KeyCode.Mouse0) && _canDrop)
            {
                StopClipping();
                ThrowObject();
            }
        }
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestPickup(NetworkObject targetObj, PlayerRef requestingPlayer)
    {
        var pickable = targetObj.GetComponent<PickableObj>();
        if (pickable != null)
        {
            pickable.AssignAuthority(requestingPlayer);

            if (requestingPlayer == Object.InputAuthority)
            {
                PickUpObject(targetObj.gameObject);
            }
        }
    }

    void PickUpObject(GameObject pickUpObj)
    {
        Debug.Log("Has picked up");
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            _heldObj = pickUpObj;
            _heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            _heldObjRb.isKinematic = true;
            // _heldObjRb.transform.parent = _holdPosition.transform;
            _heldObj.layer = _LayerNumber;
            Physics.IgnoreCollision(_heldObj.GetComponent<Collider>(), _player.GetComponent<Collider>(), true);
        }
    }

    void DropObject()
    {
        Physics.IgnoreCollision(_heldObj.GetComponent<Collider>(), _player.GetComponent<Collider>(), false);
        _heldObj.layer = 0;
        _heldObjRb.isKinematic = false;
        _heldObj.transform.parent = null;
        _heldObj = null;
    }

    void MoveObject()
    {
        if (_heldObj != null && Object.HasInputAuthority)
        {
           // _networkInputData.holdPosition = transform.position;
        }
    }

    void RotateObject()
    {
        if (Input.GetKey(KeyCode.R))
        {
            _canDrop = false;
            float XaxisRotation = Input.GetAxis("Mouse X") * _rotationSensitivity;
            float YaxisRotation = Input.GetAxis("Mouse Y") * _rotationSensitivity;
            _heldObj.transform.Rotate(Vector3.down, XaxisRotation);
            _heldObj.transform.Rotate(Vector3.right, YaxisRotation);
        }
        else
        {
            _canDrop = true;
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
