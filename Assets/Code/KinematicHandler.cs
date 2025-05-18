using UnityEngine;

namespace Code
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class KinematicHandler : MonoBehaviour
    {
        private Rigidbody _rb;
        private Collider _collider;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        public void SetKinematic(bool isKinematic)
        {
            _rb.isKinematic = isKinematic;
            _collider.enabled = !isKinematic;
        }

        public void ResetAngularVelocity()
        {
            _rb.angularVelocity = Vector3.zero;
        }
    }
}