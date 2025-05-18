using Fusion;
using UnityEngine;

namespace Code
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class BigTrash : FloorTrash, IAttachable, IThrowable
    {
        [Networked] private bool IsKinematic { get; set; }

        private IAttachmentProvider _attachmentProvider;
        private Transform _target;

        private AttachFollower _follower;
        private KinematicHandler _kinematic;
        private ImpactDamageHandler _damageHandler;

        private ChangeDetector _changeDetector;

        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                _follower = GetComponent<AttachFollower>();
                _kinematic = GetComponent<KinematicHandler>();
                _damageHandler = GetComponent<ImpactDamageHandler>();
                _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            }
        }

        public override void Render()
        {
            foreach (var change in _changeDetector.DetectChanges(this))
            {
                if (change == nameof(IsKinematic))
                {
                    _kinematic.SetKinematic(IsKinematic);
                }
            }
        }

        public void AttachTo(NetworkObject attachPoint, NetworkBehaviour playerRef)
        {
            _target = attachPoint.transform;
            _follower.SetFollowTarget(_target);

            if (playerRef.TryGetComponent(out IAttachmentProvider provider) && provider.CurrentAttachedObj == null)
            {
                _attachmentProvider = provider;
                provider.CurrentAttachedObj = Object;
            }

            _kinematic.SetKinematic(true);
            IsKinematic = true;
        }

        public void DeAttach()
        {
            _follower.ClearTarget();

            if (_attachmentProvider != null)
            {
                _attachmentProvider.CurrentAttachedObj = null;
                _attachmentProvider = null;
            }

            _kinematic.SetKinematic(false);
            _kinematic.ResetAngularVelocity();
            IsKinematic = false;
            _target = null;
        }

        public void Throw()
        {
            DeAttach();
            _damageHandler.Activate();
            GetComponent<Rigidbody>().AddForce(transform.forward * 30f, ForceMode.Impulse); // Optional: use a ThrowHandler
        }
    }
}
