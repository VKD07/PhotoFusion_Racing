using UnityEngine;

public interface IAttachable
{
    public void AttachTo(Transform target);
    public void DeAttach();
}