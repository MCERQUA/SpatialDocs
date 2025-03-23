# ActorCustomPropertiesChangedEventArgs

Category: Actor Service

Struct: Event Arguments

`ActorCustomPropertiesChangedEventArgs` is a struct that contains data about changes to an actor's custom properties. This struct is passed as an argument to the `onCustomPropertiesChanged` event of an `IActor` object when its custom properties are modified. It provides information about which properties were changed, added, or removed.

## Properties/Fields

| Property | Description |
| --- | --- |
| changedProperties | Dictionary containing properties that were newly added or changed. The key is the property name and the value is the property value. **Important**: This dictionary reference is re-pooled and re-used between events, so you should not cache it. |
| removedProperties | List of property names that were removed. **Important**: This list reference is re-pooled and re-used between events, so you should not cache it. |

## Methods

This struct does not define any custom methods beyond the standard methods inherited from ValueType.

## Inherited Members

| Member | Description |
| --- | --- |
| Equals(object) | Determines whether the specified object is equal to the current object. |
| GetHashCode() | Serves as the default hash function. |
| ToString() | Returns a string that represents the current object. |

## Usage Examples

### Basic Property Change Listener

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class PropertyChangeListener : MonoBehaviour
{
    private void OnEnable()
    {
        // Subscribe to property changes for the local actor
        SpatialBridge.actorService.localActor.onCustomPropertiesChanged += HandleLocalPropertyChanges;
    }

    private void OnDisable()
    {
        // Unsubscribe when disabled
        SpatialBridge.actorService.localActor.onCustomPropertiesChanged -= HandleLocalPropertyChanges;
    }

    private void HandleLocalPropertyChanges(ActorCustomPropertiesChangedEventArgs args)
    {
        // Check for changed or added properties
        foreach (var kvp in args.changedProperties)
        {
            Debug.Log($"Property changed: {kvp.Key} = {kvp.Value}");
        }

        // Check for removed properties
        foreach (var propName in args.removedProperties)
        {
            Debug.Log($"Property removed: {propName}");
        }
    }
}
```

### Monitoring All Actors' Property Changes

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class ActorPropertyMonitor : MonoBehaviour
{
    // Dictionary to store our actor property event handlers so we can unsubscribe later
    private Dictionary<int, System.Action<ActorCustomPropertiesChangedEventArgs>> _propertyHandlers = 
        new Dictionary<int, System.Action<ActorCustomPropertiesChangedEventArgs>>();

    private void OnEnable()
    {
        // Subscribe to actor join/leave events
        SpatialBridge.actorService.onActorJoined += HandleActorJoined;
        SpatialBridge.actorService.onActorLeft += HandleActorLeft;
        
        // Register existing actors
        foreach (var actor in SpatialBridge.actorService.actors.Values)
        {
            RegisterActorPropertyListener(actor);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from all events
        SpatialBridge.actorService.onActorJoined -= HandleActorJoined;
        SpatialBridge.actorService.onActorLeft -= HandleActorLeft;
        
        // Unregister all actors
        foreach (var actor in SpatialBridge.actorService.actors.Values)
        {
            UnregisterActorPropertyListener(actor);
        }
        
        _propertyHandlers.Clear();
    }

    private void HandleActorJoined(ActorJoinedEventArgs args)
    {
        IActor actor = SpatialBridge.actorService.actors[args.actorNumber];
        RegisterActorPropertyListener(actor);
    }

    private void HandleActorLeft(ActorLeftEventArgs args)
    {
        IActor actor = SpatialBridge.actorService.actors[args.actorNumber];
        UnregisterActorPropertyListener(actor);
    }

    private void RegisterActorPropertyListener(IActor actor)
    {
        if (_propertyHandlers.ContainsKey(actor.actorNumber))
            return;
            
        // Create handler for this actor
        System.Action<ActorCustomPropertiesChangedEventArgs> handler = (args) => 
        {
            HandleActorPropertyChanged(actor, args);
        };
        
        // Store handler reference
        _propertyHandlers[actor.actorNumber] = handler;
        
        // Subscribe to property changes
        actor.onCustomPropertiesChanged += handler;
        
        Debug.Log($"Now monitoring property changes for {actor.displayName}");
    }

    private void UnregisterActorPropertyListener(IActor actor)
    {
        if (!_propertyHandlers.TryGetValue(actor.actorNumber, out var handler))
            return;
            
        // Unsubscribe from property changes
        actor.onCustomPropertiesChanged -= handler;
        
        // Remove from dictionary
        _propertyHandlers.Remove(actor.actorNumber);
        
        Debug.Log($"Stopped monitoring property changes for {actor.displayName}");
    }

    private void HandleActorPropertyChanged(IActor actor, ActorCustomPropertiesChangedEventArgs args)
    {
        // Process changed properties
        foreach (var kvp in args.changedProperties)
        {
            Debug.Log($"{actor.displayName} property changed: {kvp.Key} = {kvp.Value}");
            
            // Handle specific property changes
            if (kvp.Key == "score")
            {
                UpdateScoreUI(actor.displayName, (int)kvp.Value);
            }
            else if (kvp.Key == "status")
            {
                UpdatePlayerStatus(actor.displayName, kvp.Value.ToString());
            }
        }
        
        // Process removed properties
        foreach (var propName in args.removedProperties)
        {
            Debug.Log($"{actor.displayName} property removed: {propName}");
        }
    }
    
    private void UpdateScoreUI(string playerName, int score)
    {
        // Implementation for updating score UI
        Debug.Log($"Updated score for {playerName}: {score}");
    }
    
    private void UpdatePlayerStatus(string playerName, string status)
    {
        // Implementation for updating player status
        Debug.Log($"Updated status for {playerName}: {status}");
    }
}
```

## Best Practices

1. **Don't cache the dictionaries**: As noted in the property descriptions, both `changedProperties` and `removedProperties` are re-pooled and re-used between events. If you need to store this data, make a copy of the values you need.

2. **Unsubscribe from events**: Always remember to unsubscribe from the `onCustomPropertiesChanged` event when your component is disabled or destroyed to prevent memory leaks.

3. **Check for specific properties**: When handling property changes, use `ContainsKey()` to check for specific properties you're interested in rather than iterating through all changes.

4. **Process changes synchronously**: The event handler should process changes quickly to avoid blocking the main thread. For intensive operations, consider queueing the changes for processing in an update loop.

5. **Handle different property types**: Custom properties can be any serializable type. Use proper type checking or casting when processing property values.

## Common Use Cases

1. **Player statistics tracking**: Monitor and update UI elements when player stats like score, health, or experience change.

2. **Game state synchronization**: Keep track of game state changes across all players in multiplayer games.

3. **Achievement tracking**: Detect when players reach certain milestones or complete objectives.

4. **Player customization**: Update visual elements when players change their appearance or equipment.

5. **Team management**: Track team assignments and roles by monitoring team-related properties.

## Related Components

- [IActor](./IActor.md): Interface that represents an actor in the Spatial environment, which can have custom properties.
- [IActorService](./IActorService.md): Service that manages actors in the Spatial environment.
- [ActorJoinedEventArgs](./ActorJoinedEventArgs.md): Event arguments for when an actor joins the space.
- [ActorLeftEventArgs](./ActorLeftEventArgs.md): Event arguments for when an actor leaves the space.