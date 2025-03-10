# ActorCustomPropertiesChangedEventArgs

Category: Actor Service

Struct: Event Arguments

Arguments for the `onCustomPropertiesChanged` event in the IActor interface. This struct provides information about which properties were changed, added, or removed when an actor's custom properties are updated.

## Properties

| Property | Description |
| --- | --- |
| changedProperties | Dictionary containing properties that were newly added or changed. This dictionary reference is re-pooled and re-used between events, so you should not cache it. |
| removedProperties | List of properties that were removed. This list reference is re-pooled and re-used between events, so you should not cache it. |

## Usage Examples

```csharp
// Example: Tracking player inventory changes
public class PlayerInventoryTracker : MonoBehaviour
{
    private void OnEnable()
    {
        // Subscribe to actor joined event
        SpatialBridge.actorService.onActorJoined += HandleActorJoined;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        SpatialBridge.actorService.onActorJoined -= HandleActorJoined;
    }

    private void HandleActorJoined(ActorJoinedEventArgs args)
    {
        IActor actor = SpatialBridge.actorService.actors[args.actorNumber];
        
        // Subscribe to property changes for this actor
        actor.onCustomPropertiesChanged += HandleCustomPropertiesChanged;
    }
    
    private void HandleCustomPropertiesChanged(ActorCustomPropertiesChangedEventArgs args)
    {
        // Check for inventory changes
        if (args.changedProperties.ContainsKey("inventory"))
        {
            var inventory = args.changedProperties["inventory"];
            Debug.Log($"Actor inventory updated: {inventory}");
        }
        
        // Check for specific items being removed
        foreach (var key in args.removedProperties)
        {
            if (key.StartsWith("item_"))
            {
                Debug.Log($"Actor lost item: {key}");
            }
        }
    }
}

// Example: Tracking player stats and notifying other players
public class PlayerStatsNotifier : MonoBehaviour
{
    private int playerScore = 0;
    
    public void UpdateScore(int additionalPoints)
    {
        playerScore += additionalPoints;
        
        // Update the custom property to notify other players
        SpatialBridge.actorService.localActor.SetCustomProperty("score", playerScore);
    }
    
    private void OnEnable()
    {
        // Subscribe to all actors joining
        SpatialBridge.actorService.onActorJoined += HandleActorJoined;
    }
    
    private void OnDisable()
    {
        SpatialBridge.actorService.onActorJoined -= HandleActorJoined;
    }
    
    private void HandleActorJoined(ActorJoinedEventArgs args)
    {
        IActor actor = SpatialBridge.actorService.actors[args.actorNumber];
        
        // Subscribe to property changes to show notifications
        actor.onCustomPropertiesChanged += (ActorCustomPropertiesChangedEventArgs propArgs) => {
            if (propArgs.changedProperties.ContainsKey("score"))
            {
                int newScore = (int)propArgs.changedProperties["score"];
                SpatialBridge.coreGUIService.DisplayToastMessage(
                    $"{actor.displayName} now has {newScore} points!");
            }
        };
    }
}
```

## Best Practices

1. **Don't Cache References**: Both the `changedProperties` dictionary and `removedProperties` list are re-pooled and re-used between events. Make copies of any data you need to preserve.

2. **Check for Keys**: Always use `ContainsKey()` before accessing `changedProperties` to avoid errors.

3. **Clean Subscriptions**: Always unsubscribe from the `onCustomPropertiesChanged` event when your components are disabled or destroyed to prevent memory leaks.

4. **Process Immediately**: Handle the event data immediately within the event handler, as the data may not be valid after the handler returns.

5. **Type Safety**: Cast values from `changedProperties` to the expected type, as they are stored as `object`.

## Common Use Cases

1. **Player Inventory Systems**: Track changes to a player's inventory or equipment.

2. **Multiplayer Game States**: Monitor player status changes like health, score, or resources.

3. **Progress Tracking**: Observe quest or achievement progress across multiplayer sessions.

4. **UI Updates**: Update UI elements when specific properties change for a player.

5. **Synchronization**: Keep client-side representations in sync with network state.

6. **Event Triggering**: Trigger gameplay events when specific property thresholds are reached.

## Related Components

- [IActor](IActor.md): The interface that exposes the `onCustomPropertiesChanged` event.
- [ActorJoinedEventArgs](ActorJoinedEventArgs.md): Event args for when actors join the space.
- [IActorService](IActorService.md): Service for accessing and managing actors.
