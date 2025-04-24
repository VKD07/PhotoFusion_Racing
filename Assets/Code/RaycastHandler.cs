using Code;
using Fusion;
using UnityEngine;

public class RaycastHandler : NetworkBehaviour
{
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private float _pickUpRange = 5f;
    private GameObject _objDetected;
    private int _LayerNumber;
    private RaycastHit _hit;
    private NetworkInputData _networkInputData;


    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }
        
        if (Physics.Raycast(_playerCamera.position, _playerCamera.TransformDirection(Vector3.forward), out _hit,
                _pickUpRange))
        {
            _objDetected = _hit.transform.gameObject;
        }
    }

    public GameObject GetDetectedObject()
    {
        return _objDetected;
    }
}