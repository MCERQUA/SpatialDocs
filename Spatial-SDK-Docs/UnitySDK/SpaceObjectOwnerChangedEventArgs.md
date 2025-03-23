# SpaceObjectOwnerChangedEventArgs

Category: Space Content Service Related

Interface/Class/Enum: Struct

Arguments for the ISpaceObject.onOwnerChanged event. This struct provides information about ownership changes for space objects, including the previous and new owner actor numbers.

## Properties/Fields

| Property | Description |
| --- | --- |
| previousOwnerActorNumber | The actor number of the previous owner of the space object. |
| newOwnerActorNumber | The actor number of the new owner of the space object. |

## Usage Examples

```csharp
// Example: Ownership Change Monitor
public class OwnershipChangeMonitor : MonoBehaviour
{
    private ISpaceObject spaceObject;
    private List<OwnershipChange> ownershipHistory = new List<OwnershipChange>();
    private bool isInitialized;

    private class OwnershipChange
    {
        public int previousOwner;
        public int newOwner;
        public DateTime changeTime;
    }

    void Start()
    {
        spaceObject = GetComponent<ISpaceObject>();
        if (spaceObject != null)
        {
            SubscribeToEvents();
            RecordInitialOwnership();
            isInitialized = true;
        }
        else
        {
            Debug.LogError("Failed to find an ISpaceObject component");
        }
    }

    private void SubscribeToEvents()
    {
        spaceObject.onOwnerChanged += HandleOwnerChanged;
    }

    private void RecordInitialOwnership()
    {
        var initialOwnership = new OwnershipChange
        {
            previousOwner = -1, // -1 indicates initial state
            newOwner = spaceObject.ownerActorNumber,
            changeTime = DateTime.UtcNow
        };

        ownershipHistory.Add(initialOwnership);
        Debug.Log($"Initial ownership recorded: Actor {initialOwnership.newOwner}");
    }

    private void HandleOwnerChanged(SpaceObjectOwnerChangedEventArgs args)
    {
        RecordOwnershipChange(args.previousOwnerActorNumber, args.newOwnerActorNumber);
        
        // Check if we now own the object
        if (args.newOwnerActorNumber == SpatialBridge.actorService.localActor.actorNumber)
        {
            OnObjectOwnershipAcquired();
        }
        // Check if we lost ownership of the object
        else if (args.previousOwnerActorNumber == SpatialBridge.actorService.localActor.actorNumber)
        {
            OnObjectOwnershipLost();
        }
    }

    private void RecordOwnershipChange(int previousOwner, int newOwner)
    {
        var change = new OwnershipChange
        {
            previousOwner = previousOwner,
            newOwner = newOwner,
            changeTime = DateTime.UtcNow
        };

        ownershipHistory.Add(change);
        Debug.Log($"Ownership changed: From Actor {previousOwner} to Actor {newOwner}");
    }

    private void OnObjectOwnershipAcquired()
    {
        Debug.Log("Local actor acquired ownership of this object");
        // Implement ownership-specific logic here
    }

    private void OnObjectOwnershipLost()
    {
        Debug.Log("Local actor lost ownership of this object");
        // Clean up or handle ownership loss
    }

    // Get the ownership duration for a specific actor
    public TimeSpan GetOwnershipDuration(int actorNumber)
    {
        TimeSpan totalDuration = TimeSpan.Zero;
        DateTime? ownershipStart = null;

        foreach (var change in ownershipHistory)
        {
            // If actor became the owner
            if (change.newOwner == actorNumber)
            {
                ownershipStart = change.changeTime;
            }
            // If actor was the previous owner
            else if (change.previousOwner == actorNumber && ownershipStart.HasValue)
            {
                totalDuration += change.changeTime - ownershipStart.Value;
                ownershipStart = null;
            }
        }

        // If actor is still the current owner
        if (ownershipStart.HasValue && spaceObject.ownerActorNumber == actorNumber)
        {
            totalDuration += DateTime.UtcNow - ownershipStart.Value;
        }

        return totalDuration;
    }

    public void PrintOwnershipHistory()
    {
        Debug.Log("== Ownership History ==");
        
        foreach (var change in ownershipHistory)
        {
            string previous = change.previousOwner == -1 ? "Initial" : $"Actor {change.previousOwner}";
            Debug.Log($"{change.changeTime.ToLocalTime()}: {previous} -> Actor {change.newOwner}");
        }
        
        Debug.Log("======================");
    }

    void OnDestroy()
    {
        if (spaceObject != null)
        {
            spaceObject.onOwnerChanged -= HandleOwnerChanged;
        }
    }
}
```

## Best Practices

1. Always check if the local actor is now the owner when handling ownership changes
2. Consider saving the previous owner information for potential fallback or recovery
3. Implement different behaviors for when ownership is acquired versus lost
4. Handle ownership changes gracefully to ensure smooth transitions between owners
5. Remember that ownership changes may occur from server actions, not just client requests
6. Use the ownership change event to update UI or game state appropriately

## Common Use Cases

1. Tracking the history of object ownership for debugging or analytics
2. Implementing ownership-dependent features that activate when a player takes control
3. Creating ownership visualizations (like highlighting objects owned by specific players)
4. Building systems that respond to ownership transfers (like collaborative editing tools)
5. Implementing recovery mechanisms when ownership changes unexpectedly
6. Managing distributed responsibility in multiplayer experiences

## Completed: March 9, 2025
