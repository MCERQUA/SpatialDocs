# IOwnershipChanged

Category: Interfaces

Interface

An interface to handle ownership changes for network objects. To be used in conjunction with `SpatialNetworkBehaviour`. This interface is called right after the object is spawned and whenever the ownership of the network object changes.

## Methods

| Method | Description |
| --- | --- |
| OnOwnershipChanged(NetworkObjectOwnershipChangedEventArgs args) | Called when the ownership of a network object changes from one actor to another. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;

// Example: Ownership-aware network object behavior
public class OwnershipAwareObject : SpatialNetworkBehaviour, IOwnershipChanged
{
    private Color myColor;
    private MeshRenderer meshRenderer;
    
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    
    public override void Spawned()
    {
        base.Spawned();
        
        // Initialize with a default color
        myColor = new Color(
            Random.value, 
            Random.value, 
            Random.value
        );
        
        UpdateVisuals();
    }
    
    // Implement the IOwnershipChanged interface method
    public void OnOwnershipChanged(NetworkObjectOwnershipChangedEventArgs args)
    {
        Debug.Log($"Ownership changed from actor {args.previousOwnerActorNumber} to {args.newOwnerActorNumber}");
        
        // Only the owner can change the color
        if (IsOwner)
        {
            // Set a new random color when ownership changes
            myColor = new Color(
                Random.value, 
                Random.value, 
                Random.value
            );
            
            // Synchronize the color across the network
            SyncColor();
        }
        
        UpdateVisuals();
    }
    
    // Example of updating visuals based on ownership
    private void UpdateVisuals()
    {
        if (meshRenderer != null)
        {
            // Apply the current color
            meshRenderer.material.color = myColor;
            
            // Add a glow effect if this client is the owner
            if (IsOwner)
            {
                meshRenderer.material.EnableKeyword("_EMISSION");
                meshRenderer.material.SetColor("_EmissionColor", myColor * 0.5f);
            }
            else
            {
                meshRenderer.material.DisableKeyword("_EMISSION");
            }
        }
    }
    
    // Example method to request ownership of this object
    public void RequestOwnership()
    {
        if (!IsOwner)
        {
            // Request ownership transfer to the local actor
            SpatialBridge.spaceContentService.RequestSpaceObjectOwnership(spaceObjectId)
                .SetCompletedEvent(request =>
                {
                    if (request.succeeded)
                    {
                        Debug.Log("Successfully took ownership of object");
                    }
                    else
                    {
                        Debug.LogWarning("Failed to take ownership of object");
                    }
                });
        }
    }
    
    // Example of a networked variable change
    private void SyncColor()
    {
        if (IsOwner)
        {
            // Set network variables to synchronize the color
            SetNetworkVariable("colorR", myColor.r);
            SetNetworkVariable("colorG", myColor.g);
            SetNetworkVariable("colorB", myColor.b);
        }
    }
}

// Example: Interactive object that changes ownership on interaction
public class InteractiveNetworkObject : SpatialNetworkBehaviour, IOwnershipChanged
{
    [SerializeField] private GameObject ownerIndicator;
    
    public override void Spawned()
    {
        base.Spawned();
        UpdateOwnerIndicator();
    }
    
    public void OnOwnershipChanged(NetworkObjectOwnershipChangedEventArgs args)
    {
        Debug.Log($"Object ownership transferred from {args.previousOwnerActorNumber} to {args.newOwnerActorNumber}");
        
        // Update visual indicator of ownership
        UpdateOwnerIndicator();
        
        // If the local player just became the owner
        if (IsOwner && args.previousOwnerActorNumber != args.newOwnerActorNumber)
        {
            OnBecameOwner();
        }
    }
    
    private void UpdateOwnerIndicator()
    {
        if (ownerIndicator != null)
        {
            ownerIndicator.SetActive(IsOwner);
        }
    }
    
    private void OnBecameOwner()
    {
        // Perform actions when this client becomes the owner
        Debug.Log("Local player is now the owner of this object");
        
        // Example: spawn an effect to celebrate ownership
        if (IsOwner)
        {
            SpatialBridge.vfxService.CreateFloatingText(
                "OWNED!", 
                FloatingTextAnimStyle.Bouncy,
                transform.position + Vector3.up,
                Vector3.up, 
                Color.green, 
                true
            );
        }
    }
    
    // Example of an interaction that transfers ownership
    public void OnInteract()
    {
        if (!IsOwner)
        {
            // When a player interacts with this object, they become the owner
            SpatialBridge.spaceContentService.RequestSpaceObjectOwnership(spaceObjectId);
        }
    }
}
```

## Best Practices

1. **Ownership Validation**
   - Always check `IsOwner` before making changes that should only be executed by the owner
   - Be cautious about requesting ownership too frequently to avoid network congestion
   - Consider adding permission checks before allowing ownership changes

2. **Ownership Handling Pattern**
   - Implement clear ownership visual feedback so users understand who controls an object
   - Handle ownership transfer failures gracefully with user feedback
   - Use ownership to gate authority-specific actions like physics interactions

3. **Performance Considerations**
   - Avoid expensive operations in the `OnOwnershipChanged` callback
   - Be mindful of potential race conditions when multiple clients request ownership
   - Consider using a request queue if ownership changes are frequent

4. **Visual Feedback**
   - Provide clear visual feedback when ownership changes
   - Use different visuals for owned vs. non-owned objects
   - Consider sound effects or other feedback for ownership state changes

## Common Use Cases

1. **Interactive Objects**
   - Pickable items that transfer to the player who grabs them
   - Doors or switches that can only be operated by their current owner
   - Vehicles that transfer control to the driver
   - Building blocks or positioning systems with exclusive manipulation

2. **Multiplayer Gameplay**
   - Turn-based game pieces that transfer between players
   - Shared resources with transfer of control
   - Territory control mechanics
   - Competitive object possession systems

3. **Collaborative Tools**
   - Shared whiteboards with presenter control
   - Document editing with ownership handoff
   - Media players with DJ/host controls
   - Multi-user positioning tools with exclusive edit rights

4. **Security and Permissions**
   - Access control systems
   - Admin-only configuration objects
   - Permission-based interactive elements
   - Space owner privileged systems

## Completed: March 9, 2025
