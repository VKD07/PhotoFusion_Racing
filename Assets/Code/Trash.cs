using System;
using Fusion;
using UnityEngine;

public class Trash : PickableObj, IDestructable, ITrackable
{
    public Action OnItemDestroyed { get; set; }
    public Action OnProgressCompleted { get; set; }

    public void DestroyObject(NetworkBehaviour networkBehaviour)
    {
        OnItemDestroyed?.Invoke();
        Destroy(gameObject);
    }
}
