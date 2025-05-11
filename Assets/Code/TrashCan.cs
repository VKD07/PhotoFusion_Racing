using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

namespace Code
{
    public class TrashCan : NetworkBehaviour, INetworkInteractable
    {
        //TODO: Create a box cast colldier above the trash and when this is interacted it destroyes all trash add a time to destroy
        private List<FloorTrash> _trashes = new List<FloorTrash>();

        public void Interact(NetworkBehaviour networkBehaviour)
        {
           Debug.Log("Interacted");
            if (_trashes.Count <= 0)
            {
                return;
            }
            
            for (int i = 0; i < _trashes.Count; i++)
            {
                _trashes[i].RPC_DestroyObject();
            }
        }
        
        public void UnInteract(NetworkBehaviour networkBehaviour)
        {
        }
 
        

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out FloorTrash floorTrash))
            {
                if (_trashes.Contains(floorTrash))
                {
                    return;
                }
                _trashes.Add(floorTrash);
                Debug.Log("Added: " + floorTrash.gameObject.name);
            }
        }
    }
}