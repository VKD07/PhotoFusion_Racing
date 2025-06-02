using Fusion;
using UnityEngine;

public class PickupableObject : NetworkBehaviour
{
    // A networked reference to the holder; if not held, it will be invalid.
    [Networked] public NetworkObject Holder { get; set; }

    /// <summary>
    /// Determines whether the object is free to be picked up.
    /// </summary>
    public bool IsAvailable()
    {
        return !Holder.IsValid;
    }

    /// <summary>
    /// Called by the player when attempting to pick up this object.
    /// </summary>
    /// <param name="playerNetworkObject">The NetworkObject of the player picking this item up.</param>
    public void PickupObject(NetworkObject playerNetworkObject)
    {
        // Only allow pickup if the object is not already held.
        if (!IsAvailable())
            return;

        // Set the networked holder to the player’s NetworkObject.
        Holder = playerNetworkObject;

        // Disable physics so that movement is controlled via parenting.
        SetPhysics(holding: true);
    }

    /// <summary>
    /// Called to drop the object.
    /// </summary>
    public void DropObject()
    {
        // Only drop if the object is currently held.
        if (IsAvailable())
            return;

        // Reset the holder.
        Holder = default;

        // Re-enable physics on the object.
        SetPhysics(holding: false);
    }

    /// <summary>
    /// Sets the Rigidbody's isKinematic property depending on whether the object is held.
    /// </summary>
    /// <param name="holding">True if the object is held (thus physics should be off).</param>
    private void SetPhysics(bool holding)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = holding;
        }
    }
}