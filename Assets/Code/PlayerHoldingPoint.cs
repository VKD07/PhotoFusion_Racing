using System;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerHoldingPoint : NetworkBehaviour
{
    public static PlayerHoldingPoint Instance;

    [SerializeField] private Transform _holdTransform;
    
    public Transform HoldingPointTransform => _holdTransform;

    public override void Spawned()
    {
        if (Object.HasInputAuthority && Instance == null)
        {
            Instance = this;
        }
    }
}