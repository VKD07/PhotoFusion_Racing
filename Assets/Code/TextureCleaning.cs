using System;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TextureCleaning : NetworkBehaviour, ITrackable
{
    [SerializeField] private Texture2D _dirtMaskTextureBase;
    [SerializeField] private Texture2D _dirtBrush;
    [SerializeField] private Material _material;
    [SerializeField] private TextMeshProUGUI _uiText;
    [SerializeField] private float _brushScale = 1.5f;
    [SerializeField] private int _brushRadius = 30;
    [SerializeField] private float _dirtAmountPercentageNeeded = 2;
    private Texture2D _dirtMaskTexture;
    private bool _isFlipped;
    private float _dirtAmountTotal;
    private float _dirtAmount;
    private Vector2Int _lastPaintPixelPosition;
    private bool _isCleaned;

    public override void Spawned()
    {
        base.Spawned();

        _dirtMaskTexture = new Texture2D(_dirtMaskTextureBase.width, _dirtMaskTextureBase.height);
        _dirtMaskTexture.SetPixels(_dirtMaskTextureBase.GetPixels());
        _dirtMaskTexture.Apply();

        _material.SetTexture("_DirtMask", _dirtMaskTexture);

        _dirtAmountTotal = 0f;
        for (int x = 0; x < _dirtMaskTextureBase.width; x++)
        {
            for (int y = 0; y < _dirtMaskTextureBase.height; y++)
            {
                _dirtAmountTotal += _dirtMaskTextureBase.GetPixel(x, y).g;
            }
        }

        _dirtAmount = _dirtAmountTotal;
    }

    //Circle

    // private void ApplyBrush(Vector2 textureCoord)
    // {
    //     int centerX = (int)(textureCoord.x * dirtMaskTexture.width);
    //     int centerY = (int)(textureCoord.y * dirtMaskTexture.height);
    //
    //     Vector2Int paintPixelPosition = new Vector2Int(centerX, centerY);
    //     int paintPixelDistance = Mathf.Abs(paintPixelPosition.x - lastPaintPixelPosition.x) + Mathf.Abs(paintPixelPosition.y - lastPaintPixelPosition.y);
    //     int maxPaintDistance = 7;
    //     if (paintPixelDistance < maxPaintDistance) return;
    //
    //     lastPaintPixelPosition = paintPixelPosition;
    //
    //  
    //     for (int x = -brushRadius; x <= brushRadius; x++)
    //     {
    //         for (int y = -brushRadius; y <= brushRadius; y++)
    //         {
    //             if (x * x + y * y > brushRadius * brushRadius)
    //                 continue; // skip if outside circle
    //
    //             int px = centerX + x;
    //             int py = centerY + y;
    //
    //             if (px < 0 || px >= dirtMaskTexture.width || py < 0 || py >= dirtMaskTexture.height)
    //                 continue;
    //
    //             Color pixel = dirtMaskTexture.GetPixel(px, py);
    //             dirtAmount -= pixel.g;
    //
    //             dirtMaskTexture.SetPixel(px, py, new Color(0, 0, 0));
    //         }
    //     }
    //
    //     dirtMaskTexture.Apply();
    // }


    //Square
    private void ApplyBrush(Vector2 textureCoord)
    {
        if (GetDirtAmountPercentage() <= _dirtAmountPercentageNeeded && !_isCleaned)
        {
            _isCleaned = true;
            OnProgressCompleted?.Invoke();
            return;
        }

        int pixelX = (int)(textureCoord.x * _dirtMaskTexture.width);
        int pixelY = (int)(textureCoord.y * _dirtMaskTexture.height);

        Vector2Int paintPixelPosition = new Vector2Int(pixelX, pixelY);
        int paintPixelDistance = Mathf.Abs(paintPixelPosition.x - _lastPaintPixelPosition.x) +
                                 Mathf.Abs(paintPixelPosition.y - _lastPaintPixelPosition.y);
        int maxPaintDistance = 7;
        if (paintPixelDistance < maxPaintDistance) return;

        _lastPaintPixelPosition = paintPixelPosition;

        int scaledWidth = Mathf.RoundToInt(_dirtBrush.width * _brushScale);
        int scaledHeight = Mathf.RoundToInt(_dirtBrush.height * _brushScale);

        int pixelXOffset = pixelX - (scaledWidth / 2);
        int pixelYOffset = pixelY - (scaledHeight / 2);

        for (int x = 0; x < scaledWidth; x++)
        {
            for (int y = 0; y < scaledHeight; y++)
            {
                float u = (float)x / scaledWidth;
                float v = (float)y / scaledHeight;

                Color pixelDirt = _dirtBrush.GetPixelBilinear(u, v);

                int texX = pixelXOffset + x;
                int texY = pixelYOffset + y;

                if (texX < 0 || texX >= _dirtMaskTexture.width || texY < 0 || texY >= _dirtMaskTexture.height)
                    continue;

                Color pixelDirtMask = _dirtMaskTexture.GetPixel(texX, texY);

                float removedAmount = pixelDirtMask.g - (pixelDirtMask.g * pixelDirt.g);
                _dirtAmount -= removedAmount;

                _dirtMaskTexture.SetPixel(
                    texX,
                    texY,
                    new Color(0, pixelDirtMask.g * pixelDirt.g, 0)
                );
            }
        }
        _dirtMaskTexture.Apply();
    }

    public float GetDirtAmountPercentage()
    {
        float dirtAmount = this._dirtAmount / _dirtAmountTotal;
        return dirtAmount * 100;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_CleanAt(Vector2 textureCoord)
    {
        ApplyBrush(textureCoord);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ApplyBrushVisual(Vector2 textureCoord)
    {
        ApplyBrush(textureCoord); // run on all clients
    }

    public void RPC_CleanFromServer(Vector2 textureCoord)
    {
        ApplyBrush(textureCoord); // Server updates the texture
        RPC_ApplyBrushVisual(textureCoord); // Sync to all clients
    }

    public Action OnItemDestroyed { get; set; }
    public Action OnProgressCompleted { get; set; }
}