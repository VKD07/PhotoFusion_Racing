using Fusion;
using UnityEngine;

namespace Code
{
    public class TrashCan : NetworkBehaviour, INetworkInteractable
    {
        public void Interact(NetworkBehaviour networkBehaviour)
        {
            if (networkBehaviour.TryGetComponent(out PlayerInteractionHandler playerInteractionHandler))
            {
                if (playerInteractionHandler.DetectedNetworkObjectOnHand == null)
                {
                    return;
                }
                if (playerInteractionHandler.DetectedNetworkObjectOnHand.TryGetComponent(out IDestructable destructable))
                {
                    destructable.DestroyObject(networkBehaviour);
                }
            }
        }

        public void UnInteract(NetworkBehaviour networkBehaviour)
        {
        }
    }
}