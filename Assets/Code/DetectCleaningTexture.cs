using System;
using Fusion;
using UnityEngine;

public class DetectCleaningTexture : NetworkBehaviour
{
    private Camera _mainCamera;

    public void Awake()
    {
        _mainCamera = Camera.main;
    }

    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority && Input.GetMouseButton(0))
        {
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out TextureCleaning textureCleaning))
                {
                    Vector2 textureCoord = hit.textureCoord;
                    RPC_RequestClean(textureCleaning.Object, textureCoord);
                }
            }
        }
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestClean(NetworkObject targetObject, Vector2 uv)
    {
        if (targetObject.TryGetComponent(out TextureCleaning textureCleaning))
        {
            textureCleaning.RPC_CleanFromServer(uv); // ✅ this call happens on the server
        }
    }
    
}