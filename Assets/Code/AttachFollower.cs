using Fusion;
using UnityEngine;

namespace Code
{
    public class AttachFollower : NetworkBehaviour
    {
        private Transform _target;
        [SerializeField] private float offset = 0.1f;

        public void SetFollowTarget(Transform target)
        {
            _target = target;
        }

        public void ClearTarget()
        {
            _target = null;
        }

        public override void FixedUpdateNetwork()
        {
            if (_target == null)
            {
                return;
            }

            Vector3 forward = _target.forward.normalized;
            float separation = (transform.localScale.z + _target.localScale.z) * 0.5f + offset;
            transform.position = _target.position + forward * separation;
            transform.forward = forward;
        }
    }
}