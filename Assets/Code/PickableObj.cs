using System.Collections;
using Code;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PickableObj : NetworkBehaviour, INetworkInteractable, IDestructable
{
    [Networked] public bool IsKinematic { get; set; }

    private bool isHeld;

    private ChangeDetector _changeDetector;
    private NetworkObject _networkObject;


    public void Interact(NetworkBehaviour networkBehaviour)
    {
        PickUpObject(networkBehaviour.Object.InputAuthority);
        if (networkBehaviour.TryGetComponent(out PlayerInteractionHandler playerInteractionHandler))
        {
            playerInteractionHandler.DetectedNetworkObject = _networkObject;
        }
    }

    public void UnInteract(NetworkBehaviour networkBehaviour)
    {
        DropObject();
    }

    public void DestroyObject(NetworkBehaviour networkBehaviour)
    {
        Destroy(gameObject);
    }

    public override void Spawned()
    {
        _networkObject = GetComponent<NetworkObject>();
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(IsKinematic):
                    GetComponent<Rigidbody>().isKinematic = IsKinematic;
                    break;
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput<NetworkInputData>(out var input))
        {
            transform.position = input.holdPosition;
            transform.rotation = input.holdRotation;
        }
    }
    

    private void PickUpObject(PlayerRef player)
    {
        if (isHeld)
        {
            return;
        }

        isHeld = true;

        if (!_networkObject.HasStateAuthority)
            return;

        if (_networkObject.InputAuthority != player)
        {
            IsKinematic = true;
            _networkObject.AssignInputAuthority(player);
        }
    }

    private void DropObject()
    {
        isHeld = false;
        IsKinematic = false;
        _networkObject.RemoveInputAuthority();
    }
}