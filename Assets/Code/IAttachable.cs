using Fusion;
using UnityEngine;

public interface IAttachable
{
    public void AttachTo(NetworkObject attachPoint, NetworkBehaviour playerRef);
    public void DeAttach();
}