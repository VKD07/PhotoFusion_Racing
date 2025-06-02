using Fusion;
using UnityEngine;

namespace Code
{
    public class VacuumCleaner : NetworkBehaviour, IAttachmentProvider
    {
        [SerializeField] private Transform _suckingPointOrigin;
        [SerializeField] private float _vacuumRadius = 1f;
        [SerializeField] private float _suckingPointDistance = 2f;
        [SerializeField] private Collider[] _detectedTrash;
        private Vector3 _suckingPointPosition;
        private NetworkObject _suckingPointNetworkObject;

        public NetworkObject CurrentAttachedObj { get; set; }

        private bool _isThrowing;

        public override void Spawned()
        {
            _suckingPointNetworkObject = _suckingPointOrigin.GetComponent<NetworkObject>();
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasInputAuthority)
            {
                return;
            }
            
            DetectTrash();

            if (GetInput(out NetworkInputData data))
            {
                VacuumObjects(data);
            }
        }

        private void DetectTrash()
        {
            _detectedTrash =
                Physics.OverlapSphere(
                    _suckingPointOrigin.position + (_suckingPointOrigin.forward * _suckingPointDistance),
                    _vacuumRadius);
        }

        private void VacuumObjects(NetworkInputData data)
        {
            if (data.buttons.IsSet(NetworkInputData.GUNBUTTON) && CurrentAttachedObj == null && !_isThrowing)
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
            else if (!data.buttons.IsSet(NetworkInputData.GUNBUTTON) && CurrentAttachedObj != null)
            {
                RPC_RequestDeattach();
                CurrentAttachedObj = null;
            }

            if (data.buttons.IsSet(NetworkInputData.THROWBUTTON) && CurrentAttachedObj != null)
            {
                RPC_RequestThrow();
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

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_RequestThrow()
        {
            if (CurrentAttachedObj.TryGetComponent(out IThrowable throwable))
            {
                throwable.Throw();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_suckingPointOrigin.position + (_suckingPointOrigin.forward * _suckingPointDistance),
                _vacuumRadius);
        }
    }
}