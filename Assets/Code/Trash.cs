using Fusion;
using UnityEngine;

public class Trash : PickableObj, IDestructable
{
    public void DestroyObject(NetworkBehaviour networkBehaviour)
    {
        Destroy(gameObject);
    }
}
