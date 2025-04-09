using Code;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class PickUp : NetworkBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private Transform _holdPosition;
    [SerializeField] private float _throwForce = 500f; //force at which the object is thrown at
    [SerializeField] private float _pickUpRange = 5f; //how far the player can pickup the object from
    private float _rotationSensitivity = 1f; //how fast/slow the object is rotated in relation to mouse movement
    private GameObject _heldObj; //object which we pick up
    private Rigidbody _heldObjRb; //rigidbody of object we pick up
    private bool _canDrop = true; //this is needed so we don't throw/drop object when rotating the object
    private int _LayerNumber; //layer index
    private const string PICKABLEOBJECT = "PickableObj";

    //Reference to script which includes mouse movement of player (looking around)
    //we want to disable the player looking around when rotating the object
    //example below 
    //MouseLookScript mouseLookScript;
    void Awake()
    {
        _LayerNumber =
            LayerMask.NameToLayer(PICKABLEOBJECT); //if your holdLayer is named differently make sure to change this ""

        //mouseLookScript = player.GetComponent<MouseLookScript>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            if (data.buttons.IsSet(NetworkInputData.INTERACTBUTTON))
            {
                Debug.Log("Object detected");
                if (_heldObj == null) //if currently not holding anything
                {
                    //perform raycast to check if player is looking at object within pickuprange
                    RaycastHit hit;
                    if (Physics.Raycast(_playerCamera.position, _playerCamera.TransformDirection(Vector3.forward), out hit,
                            _pickUpRange))
                    {
                        if (hit.transform.GetComponent<PickableObj>() != null)
                        {
                            Debug.Log("Object detected");
                            PickUpObject(hit.transform.gameObject);
                        }
                    }
                }
                else
                {
                    if (_canDrop == true)
                    {
                        StopClipping(); //prevents object from clipping through walls
                        DropObject();
                    }
                }
            }
        }

        if (_heldObj != null) //if player is holding object
        {
            MoveObject(); //keep object position at holdPos
            RotateObject();
            if (Input.GetKeyDown(KeyCode.Mouse0) &&
                _canDrop == true) //Mous0 (leftclick) is used to throw, change this if you want another button to be used)
            {
                StopClipping();
                ThrowObject();
            }
        }
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.E)) //change E to whichever key you want to press to pick up
    //     {
    //         if (_heldObj == null) //if currently not holding anything
    //         {
    //             //perform raycast to check if player is looking at object within pickuprange
    //             RaycastHit hit;
    //             if (Physics.Raycast(_playerCamera.position, _playerCamera.TransformDirection(Vector3.forward), out hit,
    //                     _pickUpRange))
    //             {
    //                 if (hit.transform.GetComponent<PickableObj>() != null)
    //                 {
    //                     PickUpObject(hit.transform.gameObject);
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             if (_canDrop == true)
    //             {
    //                 StopClipping(); //prevents object from clipping through walls
    //                 DropObject();
    //             }
    //         }
    //     }
    //
    //     if (_heldObj != null) //if player is holding object
    //     {
    //         MoveObject(); //keep object position at holdPos
    //         RotateObject();
    //         if (Input.GetKeyDown(KeyCode.Mouse0) &&
    //             _canDrop == true) //Mous0 (leftclick) is used to throw, change this if you want another button to be used)
    //         {
    //             StopClipping();
    //             ThrowObject();
    //         }
    //     }
    // }

    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>()) //make sure the object has a RigidBody
        {
            _heldObj = pickUpObj; //assign heldObj to the object that was hit by the raycast (no longer == null)
            _heldObjRb = pickUpObj.GetComponent<Rigidbody>(); //assign Rigidbody
            _heldObjRb.isKinematic = true;
            _heldObjRb.transform.parent = _holdPosition.transform; //parent object to holdposition
            _heldObj.layer = _LayerNumber; //change the object layer to the holdLayer
            //make sure object doesnt collide with player, it can cause weird bugs
            Physics.IgnoreCollision(_heldObj.GetComponent<Collider>(), _player.GetComponent<Collider>(), true);
        }
    }

    void DropObject()
    {
        //re-enable collision with player
        Physics.IgnoreCollision(_heldObj.GetComponent<Collider>(), _player.GetComponent<Collider>(), false);
        _heldObj.layer = 0; //object assigned back to default layer
        _heldObjRb.isKinematic = false;
        _heldObj.transform.parent = null; //unparent object
        _heldObj = null; //undefine game object
    }

    void MoveObject()
    {
        //keep object position the same as the holdPosition position
        if (_heldObj != null && Object.HasInputAuthority)
        {
            _heldObj.transform.position = _holdPosition.position;
        }
    }

    void RotateObject()
    {
        if (Input.GetKey(KeyCode.R)) //hold R key to rotate, change this to whatever key you want
        {
            _canDrop = false; //make sure throwing can't occur during rotating

            //disable player being able to look around
            //mouseLookScript.verticalSensitivity = 0f;
            //mouseLookScript.lateralSensitivity = 0f;

            float XaxisRotation = Input.GetAxis("Mouse X") * _rotationSensitivity;
            float YaxisRotation = Input.GetAxis("Mouse Y") * _rotationSensitivity;
            //rotate the object depending on mouse X-Y Axis
            _heldObj.transform.Rotate(Vector3.down, XaxisRotation);
            _heldObj.transform.Rotate(Vector3.right, YaxisRotation);
        }
        else
        {
            //re-enable player being able to look around
            //mouseLookScript.verticalSensitivity = originalvalue;
            //mouseLookScript.lateralSensitivity = originalvalue;
            _canDrop = true;
        }
    }

    void ThrowObject()
    {
        //same as drop function, but add force to object before undefining it
        Physics.IgnoreCollision(_heldObj.GetComponent<Collider>(), _player.GetComponent<Collider>(), false);
        _heldObj.layer = 0;
        _heldObjRb.isKinematic = false;
        _heldObj.transform.parent = null;
        _heldObjRb.AddForce(transform.forward * _throwForce);
        _heldObj = null;
    }

    void StopClipping() //function only called when dropping/throwing
    {
        var clipRange =
            Vector3.Distance(_heldObj.transform.position, transform.position); //distance from holdPos to the camera
        //have to use RaycastAll as object blocks raycast in center screen
        //RaycastAll returns array of all colliders hit within the cliprange
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
        if (hits.Length > 1)
        {
            //change object position to camera position 
            _heldObj.transform.position =
                transform.position +
                new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
            //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
        }
    }
}