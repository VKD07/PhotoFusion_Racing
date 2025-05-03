using Code;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BigTrash : FloorTrash, IAttachable
{
    [Networked] private bool IsKinematic { get; set; }

    private ChangeDetector _changeDetector;
    private Collider _collider;
    private Transform _target;
    private VacuumCleaner _vacuumCleaner;

    public override void Spawned()
    {
        _collider = GetComponent<Collider>();
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void FixedUpdateNetwork()
    {
        if (_target == null)
        {
            return;
        }

        if (!IsKinematic)
        {
            SetKinematicState(true);
        }

        Vector3 forward = _target.forward.normalized;
        float separation = (transform.localScale.z + _target.localScale.z) * 0.5f + 0.1f;
        transform.position = _target.position + forward * separation;
        transform.forward = _target.forward;
    }

    public void AttachTo(NetworkObject attachPoint, NetworkBehaviour playerRef)
    {
        _target = attachPoint.transform;

        if (playerRef.TryGetComponent(out VacuumCleaner vacuumCleaner) &&
            vacuumCleaner.CurrentAttachedObj == null)
        {
            _vacuumCleaner = vacuumCleaner;
            vacuumCleaner.CurrentAttachedObj = Object;
        }
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            if (change == nameof(IsKinematic))
            {
                SetKinematicState(IsKinematic);
            }
        }
    }

    public void DeAttach()
    {
        if (_vacuumCleaner != null)
        {
            _vacuumCleaner.CurrentAttachedObj = null;
            _vacuumCleaner = null;
        }

        _target = null;
        SetKinematicState(false);
        _rb.angularVelocity = Vector3.zero;
    }

    private void SetKinematicState(bool state)
    {
        _rb.isKinematic = state;
        _collider.enabled = !state;
        IsKinematic = state;
    }
}