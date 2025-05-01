using Code;
using Fusion;

public class SmallTrash : FloorTrash, IDestructable
{
    public void DestroyObject(NetworkBehaviour networkBehaviour)
    {
        Destroy(gameObject);
    }
}
