# ActorCustomPropertiesChangedEventArgs

Category: Actor Service

Type: Struct

## Overview
The ActorCustomPropertiesChangedEventArgs struct provides information about changes to an actor's custom properties. It's used as the event argument for the `onCustomPropertiesChanged` event in the IActor interface, allowing developers to react to property changes with information about which properties were changed, added, or removed.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| changedProperties | Dictionary<string, object> | Contains properties that were newly added or modified. The dictionary reference is re-pooled and re-used between events, so you should not cache it. Instead, copy any values you need for later use. Keys represent property names, and values contain the new property values. |
| removedProperties | List<string> | Contains a list of property names that were removed. The list reference is re-pooled and re-used between events, so you should not cache it. Make a copy if you need to retain this information. |

## Inherited Methods
The struct inherits standard methods from System.ValueType:

| Method | Return Type | Description |
| --- | --- | --- |
| Equals(object) | bool | Determines whether the specified object is equal to the current struct |
| GetHashCode() | int | Returns the hash code for this instance |
| ToString() | string | Returns a string representation of the current object |

## Usage Examples

```csharp
// Example: Tracking player cookies in a multiplayer game
using SpatialSys.UnitySDK;
using UnityEngine;

public class CookieTracker : MonoBehaviour
{
    private int cookieCount = 0;

    private void OnEnable()
    {
        SpatialBridge.actorService.onActorJoined += HandleActorJoined;
        SpatialBridge.actorService.onActorLeft += HandleActorLeft;
    }

    private void OnDisable()
    {
        SpatialBridge.actorService.onActorJoined -= HandleActorJoined;
        SpatialBridge.actorService.onActorLeft -= HandleActorLeft;
    }

    private void HandleActorJoined(ActorJoinedEventArgs args)
    {
        IActor actor = SpatialBridge.actorService.actors[args.actorNumber];
        SpatialBridge.coreGUIService.DisplayToastMessage(actor.displayName + " joined the space");

        // Subscribe to property changes
        actor.onCustomPropertiesChanged += HandlePropertyChanges;
    }

    private void HandlePropertyChanges(ActorCustomPropertiesChangedEventArgs args)
    {
        // Check if the "cookies" property changed
        if (args.changedProperties.ContainsKey("cookies"))
        {
            IActor actor = args.actor; // Note: args.actor is not actually in the struct, this is pseudo-code
            int cookies = (int)args.changedProperties["cookies"];
            
            // Display a notification
            SpatialBridge.coreGUIService.DisplayToastMessage(
                $"{actor.displayName} has collected {cookies} cookies"
            );
            
            // Important: If you need to store this value, make a copy
            // Do not store args.changedProperties directly
            int currentCookies = cookies; // Store in a local variable
        }
        
        // Check if any properties were removed
        foreach (string removedProp in args.removedProperties)
        {
            Debug.Log($"Property {removedProp} was removed");
        }
    }

    private void HandleActorLeft(ActorLeftEventArgs args)
    {
        IActor actor = SpatialBridge.actorService.actors[args.actorNumber];
        SpatialBridge.coreGUIService.DisplayToastMessage(actor.displayName + " left the space");
        
        // Don't forget to unsubscribe to avoid memory leaks
        actor.onCustomPropertiesChanged -= HandlePropertyChanges;
    }

    // Method to update the local player's cookie count
    public void CollectCookies(int amount)
    {
        cookieCount += amount;
        SpatialBridge.actorService.localActor.SetCustomProperty("cookies", cookieCount);
    }
}
```

## Best Practices

1. **Don't cache references**: Both `changedProperties` and `removedProperties` are re-pooled and reused between events. Make copies of any values you need to keep.

2. **Use type checking**: When retrieving values from `changedProperties`, verify the type or use appropriate casting as values are stored as `object`.

3. **Handle removals**: Always check both `changedProperties` and `removedProperties` to properly track all changes.

4. **Unsubscribe properly**: When an actor leaves the space, be sure to unsubscribe from their `onCustomPropertiesChanged` event to prevent memory leaks.

5. **Property naming conventions**: Use consistent, descriptive names for custom properties to make your code more maintainable.

## Common Use Cases

1. **Player state synchronization**: Track player states such as health, score, or status across the network.
   ```csharp
   // Update player health
   actor.SetCustomProperty("health", currentHealth);
   ```

2. **Game progress tracking**: Monitor player progress, achievements, or collectibles.
   ```csharp
   // Update collected items
   actor.SetCustomProperty("collectedItems", collectedItemsList);
   ```

3. **Team or group assignment**: Manage team assignments or group affiliations.
   ```csharp
   // Assign player to team
   actor.SetCustomProperty("team", "blue");
   ```

4. **Inventory management**: Keep track of items or resources a player has.
   ```csharp
   // Update inventory
   actor.SetCustomProperty("inventory", new Dictionary<string, int> {
       {"wood", 5},
       {"stone", 3}
   });
   ```

## Related Components

- **IActor** - The interface that exposes the `onCustomPropertiesChanged` event and `customProperties` dictionary.
- **ActorJoinedEventArgs** - Event args for when a new actor joins, often where you'd set up the property change subscription.
- **ActorLeftEventArgs** - Event args for when an actor leaves, where you'd typically unsubscribe from the property change event.
- **IActorService** - The service that manages actors and provides access to actors through `actors` dictionary.

## Notes
- The OnCustomPropertiesChanged event doesn't specify which actor the properties belong to, so ensure you have the correct context when handling multiple actors.
- Changes to custom properties are networked to all clients, so avoid frequent updates to minimize network traffic.
- Custom properties are not persisted across sessions unless you manually save and restore them using a service like IUserWorldDataStoreService.
