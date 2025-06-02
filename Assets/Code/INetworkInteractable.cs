using Fusion;

public interface INetworkInteractable
{
    public void Interact(NetworkBehaviour networkBehaviour);
    public void UnInteract(NetworkBehaviour networkBehaviour);
}