using Fusion;
using UnityEngine;

namespace Code
{
    public class PlayerInteractionHandler : NetworkBehaviour
    {
        [SerializeField] private RaycastHandler _raycastHandler;
        public NetworkObject DetectedNetworkObjectOnHand;
        private NetworkInputData _networkInputData;

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
                    if (_raycastHandler.GetDetectedObject().TryGetComponent(out NetworkObject networkObject))
                    {
                        RPC_Interact(networkObject);
                    }
                }

                if (_networkInputData.buttons.IsSet(NetworkInputData.DROPBUTTON))
                {
                    if (DetectedNetworkObjectOnHand != null)
                    {
                        RPC_UnInteract(DetectedNetworkObjectOnHand);
                        DetectedNetworkObjectOnHand = null;
                    }
                }
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_Interact(NetworkObject networkObject)
        {
            if (networkObject.TryGetComponent(out INetworkInteractable networkInteractable))
            {
                networkInteractable.Interact(this);
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_UnInteract(NetworkObject networkObject)
        {
            if (networkObject.TryGetComponent(out INetworkInteractable networkInteractable))
            {
                networkInteractable.UnInteract(this);
            }
        }
    }
}