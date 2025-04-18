using System.Collections;
using Code;
using Fusion;
using UnityEngine;

public class PickableObj : NetworkBehaviour, IInteractable
{
    [Networked]
    public bool IsKinematic { get; set; }

    private ChangeDetector _changeDetector;

    public override void Spawned()
    {
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
    
    public void Interact()
    {
    }
    
    

    public override void FixedUpdateNetwork()
    {
        
        if (GetInput<NetworkInputData>(out var input))
        {
            transform.position = input.holdPosition;
            transform.rotation = input.holdRotation;
        }
    }

    public void AssignAuthority(PlayerRef player)
    {
        var netObj = GetComponent<NetworkObject>();

        if (!netObj.HasStateAuthority)
            return;

        if (netObj.InputAuthority != player)
        {
            IsKinematic = true;
            netObj.AssignInputAuthority(player);
        }
    }

    public void DropObject()
    {
        NetworkObject netObj = GetComponent<NetworkObject>();
        IsKinematic = false;
        netObj.RemoveInputAuthority();
    }
    

    public void UnInteract()
    {
    }
}