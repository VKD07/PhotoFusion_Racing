using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code
{
    public class PlayerInteractionHandler : NetworkBehaviour
    {
        [SerializeField] private RaycastHandler _raycastHandler;
        private NetworkInputData _networkInputData;
        [FormerlySerializedAs("_detectedNetworkObject")] public NetworkObject DetectedNetworkObject;
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
                        if (networkObject.GetComponent<IDestructable>() != null)
                        {
                            DetectedNetworkObject = networkObject;
                            RPC_Interact(DetectedNetworkObject);
                            return;
                        }
                        RPC_Interact(networkObject);
                    }
                    else
                    {
                        Debug.LogWarning("No network object");
                    }
                }

                if (_networkInputData.buttons.IsSet(NetworkInputData.DROPBUTTON))
                {
                    RPC_UnInteract(DetectedNetworkObject);
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