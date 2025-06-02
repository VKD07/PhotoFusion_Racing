using System.Collections;
using Code;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public abstract class PickableObj : NetworkBehaviour, INetworkInteractable, IHoldable
{
    [Networked] private bool IsKinematic { get; set; }

    private bool isHeld;

    private ChangeDetector _changeDetector;
    private NetworkObject _networkObject;


    public void Interact(NetworkBehaviour networkBehaviour)
    {
        if (PlayerHasNoItemOnHand(networkBehaviour))
        {
            PickUpObject(networkBehaviour.Object.InputAuthority);
        }
    }

    private bool PlayerHasNoItemOnHand(NetworkBehaviour networkBehaviour)
    {
        if (_networkObject == null)
        {
            return false;
        }

        if (networkBehaviour.TryGetComponent(out PlayerInteractionHandler playerInteractionHandler))
        {
            if (playerInteractionHandler.DetectedNetworkObjectOnHand == null)
            {
                RPC_SendNetworkGameObjectToClient(_networkObject, networkBehaviour);
                return true;
            }
        }

        return false;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_SendNetworkGameObjectToClient(NetworkObject networkObject, NetworkBehaviour networkBehaviour)
    {
        if (networkBehaviour.TryGetComponent(out PlayerInteractionHandler playerInteractionHandler))
        {
            playerInteractionHandler.DetectedNetworkObjectOnHand = networkObject;
        }
    }

    public void UnInteract(NetworkBehaviour networkBehaviour)
    {
        DropObject(networkBehaviour);
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

    private void DropObject(NetworkBehaviour networkBehaviour)
    {
        isHeld = false;
        IsKinematic = false;

        if (networkBehaviour.TryGetComponent(out PlayerInteractionHandler playerInteractionHandler))
        {
            if (playerInteractionHandler.DetectedNetworkObjectOnHand != null)
            {
                playerInteractionHandler.DetectedNetworkObjectOnHand = null;
            }
        }
        _networkObject.RemoveInputAuthority();
    }
}