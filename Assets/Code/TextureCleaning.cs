using Fusion;
using TMPro;
using UnityEngine;

public class TextureCleaning : NetworkBehaviour
{
    [SerializeField] private Texture2D dirtMaskTextureBase;
    [SerializeField] private Texture2D dirtBrush;
    [SerializeField] private Material material;
    [SerializeField] private TextMeshProUGUI uiText;
    [SerializeField] private float brushScale = 1.5f; 
    [SerializeField] private  int brushRadius = 30;
    private Texture2D dirtMaskTexture;
    private bool isFlipped;
    private float dirtAmountTotal;
    private float dirtAmount;
    private Vector2Int lastPaintPixelPosition;

    public override void Spawned()
    {
        base.Spawned();

        dirtMaskTexture = new Texture2D(dirtMaskTextureBase.width, dirtMaskTextureBase.height);
        dirtMaskTexture.SetPixels(dirtMaskTextureBase.GetPixels());
        dirtMaskTexture.Apply();

        material.SetTexture("_DirtMask", dirtMaskTexture);

        dirtAmountTotal = 0f;
        for (int x = 0; x < dirtMaskTextureBase.width; x++)
        {
            for (int y = 0; y < dirtMaskTextureBase.height; y++)
            {
                dirtAmountTotal += dirtMaskTextureBase.GetPixel(x, y).g;
            }
        }
        dirtAmount = dirtAmountTotal;
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
        int pixelX = (int)(textureCoord.x * dirtMaskTexture.width);
        int pixelY = (int)(textureCoord.y * dirtMaskTexture.height);
    
        Vector2Int paintPixelPosition = new Vector2Int(pixelX, pixelY);
        int paintPixelDistance = Mathf.Abs(paintPixelPosition.x - lastPaintPixelPosition.x) + Mathf.Abs(paintPixelPosition.y - lastPaintPixelPosition.y);
        int maxPaintDistance = 7;
        if (paintPixelDistance < maxPaintDistance) return;
    
        lastPaintPixelPosition = paintPixelPosition;
    
        int scaledWidth = Mathf.RoundToInt(dirtBrush.width * brushScale);
        int scaledHeight = Mathf.RoundToInt(dirtBrush.height * brushScale);
    
        int pixelXOffset = pixelX - (scaledWidth / 2);
        int pixelYOffset = pixelY - (scaledHeight / 2);
    
        for (int x = 0; x < scaledWidth; x++)
        {
            for (int y = 0; y < scaledHeight; y++)
            {
                float u = (float)x / scaledWidth;
                float v = (float)y / scaledHeight;
    
                Color pixelDirt = dirtBrush.GetPixelBilinear(u, v);
    
                int texX = pixelXOffset + x;
                int texY = pixelYOffset + y;
    
                if (texX < 0 || texX >= dirtMaskTexture.width || texY < 0 || texY >= dirtMaskTexture.height)
                    continue;
    
                Color pixelDirtMask = dirtMaskTexture.GetPixel(texX, texY);
    
                float removedAmount = pixelDirtMask.g - (pixelDirtMask.g * pixelDirt.g);
                dirtAmount -= removedAmount;
    
                dirtMaskTexture.SetPixel(
                    texX,
                    texY,
                    new Color(0, pixelDirtMask.g * pixelDirt.g, 0)
                );
            }
        }
    
        dirtMaskTexture.Apply();
    }



    private float GetDirtAmount() {
        return this.dirtAmount / dirtAmountTotal;
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_CleanAt(Vector2 textureCoord)
    {
        ApplyBrush(textureCoord);
        // RPC_ApplyBrushVisual(textureCoord); // broadcast to clients
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ApplyBrushVisual(Vector2 textureCoord)
    {
        ApplyBrush(textureCoord); // run on all clients
    }
    
    public void RPC_CleanFromServer(Vector2 textureCoord)
    {
        ApplyBrush(textureCoord);                     // Server updates the texture
        RPC_ApplyBrushVisual(textureCoord);          // Sync to all clients
    }
}
