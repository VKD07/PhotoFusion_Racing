using Fusion;
using UnityEngine;

namespace Code
{
    [RequireComponent(typeof(Rigidbody))]
    public class ImpactDamageHandler : NetworkBehaviour
    {
        [SerializeField] private float damageMultiplier = 1f;
        private Rigidbody _rb;
        private bool _active;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void Activate() => _active = true;
        public void Deactivate() => _active = false;

        private void OnCollisionEnter(Collision collision)
        {
            if (!_active) return;

            if (collision.transform.TryGetComponent(out NetworkObject networkObject))
            {
                if (networkObject.GetComponent<IDamageable>() != null)
                {
                    RPC_SendDamage(networkObject);
                }
            }

            _active = false;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_SendDamage(NetworkObject networkObject)
        {
            if (networkObject.TryGetComponent(out IDamageable damageable))
            {
                float speed = _rb.linearVelocity.magnitude;
                float damage = speed * damageMultiplier;
                damageable.TakeDamage(damage);
            }
        }
    }
}