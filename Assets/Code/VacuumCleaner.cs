using Code;
using Fusion;
using UnityEngine;

public class VacuumCleaner : NetworkBehaviour
{
    [SerializeField] private Transform _suckingPointOrigin;
    [SerializeField] private float _vacuumRadius = 1f;
    [SerializeField] private float _suckingPointDistance = 2f;
    [SerializeField] private Collider[] _detectedTrash;
    private Vector3 _suckingPointPosition;
    private IAttachable _currentAttachedObj;

    private void Update()
    {
        DetectTrash();
        VacuumObjects();
    }

    private void DetectTrash()
    {
        _detectedTrash =
            Physics.OverlapSphere(_suckingPointOrigin.position + (_suckingPointOrigin.forward * _suckingPointDistance),
                _vacuumRadius);
    }

    private void VacuumObjects()
    {
        if (Input.GetMouseButton(0) && _currentAttachedObj == null)
        {
            for (int i = 0; i < _detectedTrash.Length; i++)
            {
                if (_detectedTrash[i].TryGetComponent(out IPullable pullable))
                {
                    float distance =
                        Vector3.Distance(_suckingPointOrigin.position, _detectedTrash[i].transform.position);
                    if (distance <= 1f)
                    {
                        if (_detectedTrash[i].TryGetComponent(out IAttachable attachable))
                        {
                            attachable.AttachTo(_suckingPointOrigin);
                            _currentAttachedObj = attachable;
                            return;
                        }
                        if (_detectedTrash[i].TryGetComponent(out IDestructable destructable))
                        {
                            destructable.DestroyObject(this);
                        }
                    }
                    pullable.PullTowards(_suckingPointOrigin.position);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0) && _currentAttachedObj != null)
        {
            _currentAttachedObj.DeAttach();
            _currentAttachedObj = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_suckingPointOrigin.position + (_suckingPointOrigin.forward * _suckingPointDistance),
            _vacuumRadius);
    }
}