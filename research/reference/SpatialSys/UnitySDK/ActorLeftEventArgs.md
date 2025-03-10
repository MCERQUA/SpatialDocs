# ActorLeftEventArgs

Category: Actor Service

Struct: Event Arguments

`ActorLeftEventArgs` is a struct that contains information about an actor leaving a server instance. This struct is passed as an argument to the `onActorLeft` event of the `IActorService` when an actor leaves the space.

## Properties/Fields

| Property | Description |
| --- | --- |
| actorNumber | The unique number of the actor that is leaving the server. This number is unique within the current server instance. |

## Methods

This struct does not define any custom methods beyond the standard methods inherited from ValueType.

## Inherited Members

| Member | Description |
| --- | --- |
| Equals(object) | Determines whether the specified object is equal to the current object. |
| GetHashCode() | Serves as the default hash function. |
| ToString() | Returns a string that represents the current object. |

## Usage Examples

### Basic Actor Leave Handler

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class ActorLeaveHandler : MonoBehaviour
{
    private void OnEnable()
    {
        // Subscribe to actor left event
        SpatialBridge.actorService.onActorLeft += HandleActorLeft;
    }

    private void OnDisable()
    {
        // Unsubscribe when disabled
        SpatialBridge.actorService.onActorLeft -= HandleActorLeft;
    }

    private void HandleActorLeft(ActorLeftEventArgs args)
    {
        // The actor will still be available in the actors collection during this event
        IActor actor = SpatialBridge.actorService.actors[args.actorNumber];
        
        Debug.Log($"{actor.displayName} left the space");
        SpatialBridge.coreGUIService.DisplayToastMessage($"Goodbye, {actor.displayName}!");
        
        // After this event handler completes, the actor will be removed from the collection
    }
}
```

### Resource Cleanup System

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class PlayerResourceManager : MonoBehaviour
{
    // Dictionary to track player-specific game objects
    private Dictionary<int, List<GameObject>> playerResources = new Dictionary<int, List<GameObject>>();
    
    private void OnEnable()
    {
        // Subscribe to actor events
        SpatialBridge.actorService.onActorJoined += HandleActorJoined;
        SpatialBridge.actorService.onActorLeft += HandleActorLeft;
    }

    private void OnDisable()
    {
        // Unsubscribe when disabled
        SpatialBridge.actorService.onActorJoined -= HandleActorJoined;
        SpatialBridge.actorService.onActorLeft -= HandleActorLeft;
        
        // Clean up any remaining resources
        CleanupAllResources();
    }

    private void HandleActorJoined(ActorJoinedEventArgs args)
    {
        // Initialize resource tracking for this actor
        if (!playerResources.ContainsKey(args.actorNumber))
        {
            playerResources[args.actorNumber] = new List<GameObject>();
        }
    }

    private void HandleActorLeft(ActorLeftEventArgs args)
    {
        IActor actor = SpatialBridge.actorService.actors[args.actorNumber];
        Debug.Log($"Cleaning up resources for {actor.displayName} (Actor #{args.actorNumber})");
        
        // Cleanup player resources
        CleanupPlayerResources(args.actorNumber);
        
        // Update game state if needed
        UpdateGameStateForPlayerLeaving(actor);
        
        // Notify other systems
        GameEvents.NotifyPlayerLeft(actor);
    }
    
    // Add a resource that should be cleaned up when a player leaves
    public void AddPlayerResource(int actorNumber, GameObject resource)
    {
        if (!playerResources.ContainsKey(actorNumber))
        {
            playerResources[actorNumber] = new List<GameObject>();
        }
        
        playerResources[actorNumber].Add(resource);
        Debug.Log($"Added resource for Actor #{actorNumber}");
    }
    
    // Clean up resources for a specific player
    private void CleanupPlayerResources(int actorNumber)
    {
        if (playerResources.TryGetValue(actorNumber, out List<GameObject> resources))
        {
            foreach (GameObject obj in resources)
            {
                if (obj != null)
                {
                    Debug.Log($"Destroying resource {obj.name} for Actor #{actorNumber}");
                    Destroy(obj);
                }
            }
            
            // Clear and remove the list
            resources.Clear();
            playerResources.Remove(actorNumber);
        }
    }
    
    // Clean up all resources (used when component is disabled)
    private void CleanupAllResources()
    {
        foreach (var actorResources in playerResources.Values)
        {
            foreach (GameObject obj in actorResources)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }
        
        playerResources.Clear();
    }
    
    // Update game state when a player leaves
    private void UpdateGameStateForPlayerLeaving(IActor actor)
    {
        // Example: If this player was the host of a mini-game, reassign the host
        if (GameState.currentHostActor == actor.actorNumber)
        {
            AssignNewHost();
        }
        
        // Example: If this player was carrying a flag in CTF, drop the flag
        if (GameState.flagCarrier == actor.actorNumber)
        {
            DropFlag(actor.position);
        }
    }
    
    // Example game logic methods
    private void AssignNewHost()
    {
        // Find a new host from remaining players
        Debug.Log("Assigning new game host");
    }
    
    private void DropFlag(Vector3 position)
    {
        // Drop the flag at the player's last position
        Debug.Log("Flag carrier left - dropping flag");
    }
}
```

## Best Practices

1. **Remember actor accessibility timing**: The actor is still available in the `actors` collection during the `onActorLeft` event, but will be removed afterwards. This is your last chance to access the actor's properties.

2. **Clean up player-specific resources**: Use this event to destroy any GameObjects, components, or other resources that were created specifically for the departing player.

3. **Unsubscribe from events**: Always remember to unsubscribe from the `onActorLeft` event when your component is disabled or destroyed to prevent memory leaks.

4. **Maintain game state**: Update any game state that might be affected by a player leaving, such as reassigning roles or responsibilities.

5. **Avoid heavy processing in the event handler**: The event handler should process departures quickly to avoid blocking the main thread. For intensive operations, consider queueing the tasks for processing in an update loop.

6. **Combine with onActorJoined**: Usually, `onActorLeft` is paired with `onActorJoined` to create a complete lifecycle management system for players in your space.

## Common Use Cases

1. **Farewell messages**: Display goodbye messages when players leave the space.

2. **Resource cleanup**: Destroy any GameObjects or components that were created for the departing player.

3. **Player count tracking**: Keep track of how many players are in the space at any given time.

4. **Session analytics**: Record how long players stayed in the space and when they left.

5. **Game state management**: Update game state when players leave, such as reassigning roles or ending games if player count drops too low.

6. **Drop items**: When a player leaves while holding or carrying items in a game, drop those items at their last location.

7. **Team rebalancing**: Rebalance teams when players leave to maintain fair gameplay.

## Related Components

- [IActorService](./IActorService.md): Service that manages actors in the Spatial environment and provides the onActorLeft event.
- [IActor](./IActor.md): Interface that represents an actor in the Spatial environment.
- [ActorJoinedEventArgs](./ActorJoinedEventArgs.md): Event arguments for when an actor joins the space.
- [ActorCustomPropertiesChangedEventArgs](./ActorCustomPropertiesChangedEventArgs.md): Event arguments for when an actor's custom properties change.