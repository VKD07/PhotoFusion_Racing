using Fusion;
using UnityEngine;

namespace Code
{
    public class SmallTrash : FloorTrash, IDestructable
    {
        private Collider _collider;

        public void Awake ()
        {
            _collider = GetComponent<Collider>();
        }

        public void DestroyObject(NetworkBehaviour networkBehaviour)
        {
            _collider.enabled = false;
            Destroy(gameObject);
        }
    }
}