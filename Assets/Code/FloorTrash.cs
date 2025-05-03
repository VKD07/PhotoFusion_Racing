using System;
using Fusion;
using UnityEngine;

namespace Code
{
    public abstract class FloorTrash : NetworkBehaviour, IPullable
    {
        [SerializeField] private float pullForce = 5f;
        protected Rigidbody _rb;
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void PullTowards(Vector3 pullPoint)
        {
            Vector3 forceDir = (pullPoint - transform.position).normalized;
            _rb.AddForce(forceDir * pullForce);
        }
    }
}