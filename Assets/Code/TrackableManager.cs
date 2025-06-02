using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public abstract class TrackableManager<T> : NetworkBehaviour where T : NetworkBehaviour, ITrackable
{
    protected List<T> trackedItems = new List<T>();

    public abstract bool AllItemsCleared();
    
    protected virtual void Awake()
    {
        T[] items = FindObjectsOfType<T>();
        foreach (T item in items)
        {
            Register(item);
        }
    }

    protected void Register(T item)
    {
        if (!trackedItems.Contains(item))
        {
            trackedItems.Add(item);
        }
    }
}