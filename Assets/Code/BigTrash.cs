using System;
using Code;
using UnityEngine;

public class BigTrash : FloorTrash, IAttachable
{
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void AttachTo(Transform target)
    {
        transform.position = target.position;
        transform.SetParent(target, true);
        _rb.isKinematic = true;
    }

    public void DeAttach()
    {
        transform.parent = null;
        _rb.isKinematic = false;
    }
}
