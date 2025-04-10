using Code;
using Fusion;

public class PickableObj : NetworkBehaviour, IInteractable
{
    [Networked] public bool IsHeld { get; set; }

    public void Interact()
    {
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput<NetworkInputData>(out var input))
        {
            transform.position = input.holdPosition;
        }
    }
    public void AssignAuthority(PlayerRef player)
    {
        var netObj = GetComponent<NetworkObject>();

        if (!netObj.HasStateAuthority)
            return;

        if (netObj.InputAuthority != player)
        {
            if (netObj.InputAuthority != PlayerRef.None)
                netObj.RemoveInputAuthority();

            netObj.AssignInputAuthority(player);
        }
    }


    public void UnInteract()
    {
    }
}