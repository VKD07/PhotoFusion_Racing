using Fusion;
using NUnit.Framework;
using UnityEngine;

namespace Code
{
    public abstract class EnemyBase : NetworkBehaviour, IDamageable, IKillable
    {
        [SerializeField] protected float _maxHealth = 100f;
        protected float _currentHealth;
        protected IMovementBehavior _movementBehavior;

        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                _currentHealth = _maxHealth;
                _movementBehavior = GetComponent<IMovementBehavior>();
            }
        }

        public virtual void SetDestination(Vector3 destination)
        {
            if (HasStateAuthority)
            {
                _movementBehavior?.MoveTo(destination);
            }
        }

        public virtual void StopMovement()
        {
            if (HasStateAuthority)
            {
                _movementBehavior?.Stop();
            }
        }

        public void TakeDamage(float damage)
        {
            _currentHealth -= damage;

            RPC_SendDamage(damage);
            if (_currentHealth <= 0f)
            {
                Die();
            }
        }
        
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_SendDamage(float damage)
        {
            Debug.Log("Damage Taken " + damage);
        }

        public virtual void Die()
        {
            Debug.Log($"{gameObject.name} died.");
            // TODO: Handle death logic (despawn, play VFX, notify systems, etc.)
        }
    }
}