using System;
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
    private NetworkObject _suckingPointNetworkObject;

    public NetworkObject CurrentAttachedObj;

    public override void Spawned()
    {
        _suckingPointNetworkObject = _suckingPointOrigin.GetComponent<NetworkObject>();
    }

    private void Update()
    {
        if (!HasInputAuthority)
        {
            return;
        }

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
        if (Input.GetMouseButton(0) && CurrentAttachedObj == null)
        {
            for (int i = 0; i < _detectedTrash.Length; i++)
            {
                if (_detectedTrash[i].TryGetComponent(out NetworkObject networkObject))
                {
                    if (networkObject.TryGetComponent(out IPullable pullable))
                    {
                        float distance =
                            Vector3.Distance(_suckingPointOrigin.position, networkObject.transform.position);
                        if (distance <= 1f)
                        {
                            if (networkObject.TryGetComponent(out IAttachable attachable))
                            {
                                RPC_RequestAttached(networkObject);
                                CurrentAttachedObj = networkObject;
                                return;
                            }

                            if (networkObject.TryGetComponent(out IDestructable destructable))
                            {
                                RPC_RequestDestroy(networkObject);
                                return;
                            }
                        }

                        RPC_RequestPullForce(networkObject);
                    }
                }
            }
        }
        else if (Input.GetMouseButtonUp(0) && CurrentAttachedObj != null)
        {
            RPC_RequestDeattach();
            CurrentAttachedObj = null;
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestAttached(NetworkObject networkObject)
    {
        if (networkObject.TryGetComponent(out IAttachable attachable))
        {
            attachable.AttachTo(_suckingPointNetworkObject, this);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestDestroy(NetworkObject networkObject)
    {
        if (networkObject == null)
        {
            return;
        }

        if (networkObject.TryGetComponent(out IDestructable destructable))
        {
            destructable.DestroyObject(this);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestPullForce(NetworkObject networkObject)
    {
        if (networkObject.TryGetComponent(out IPullable pullable))
        {
            pullable.PullTowards(_suckingPointOrigin.position);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestDeattach()
    {
        if (CurrentAttachedObj.TryGetComponent(out IAttachable attachable))
        {
            attachable.DeAttach();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_suckingPointOrigin.position + (_suckingPointOrigin.forward * _suckingPointDistance),
            _vacuumRadius);
    }
}