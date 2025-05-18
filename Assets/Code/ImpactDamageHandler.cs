using UnityEngine;

namespace Code
{
    [RequireComponent(typeof(Rigidbody))]
    public class ImpactDamageHandler : MonoBehaviour
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

            if (collision.transform.TryGetComponent<IDamageable>(out var damageable))
            {
                float speed = _rb.linearVelocity.magnitude;
                float damage = speed * damageMultiplier;
                damageable.TakeDamage(damage);
            }

            _active = false;
        }
    }
}