# ActorJoinedEventArgs

Category: Actor Service

Struct: Event Arguments

`ActorJoinedEventArgs` is a struct that contains information about an actor joining a server instance. This struct is passed as an argument to the `onActorJoined` event of the `IActorService` when an actor joins the space.

## Properties/Fields

| Property | Description |
| --- | --- |
| actorNumber | The unique number of the actor joining the server. This number is unique within the current server instance. |
| wasAlreadyJoined | A boolean flag indicating if the actor was already in the server when the local actor joined. Since the `onActorJoined` event is triggered for all actors, even those that joined before the local actor, this flag can be used to determine if the actor was already joined or not. |

## Methods

This struct does not define any custom methods beyond the standard methods inherited from ValueType.

## Inherited Members

| Member | Description |
| --- | --- |
| Equals(object) | Determines whether the specified object is equal to the current object. |
| GetHashCode() | Serves as the default hash function. |
| ToString() | Returns a string that represents the current object. |

## Usage Examples

### Basic Actor Join Handler

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class ActorJoinHandler : MonoBehaviour
{
    private void OnEnable()
    {
        // Subscribe to actor joined event
        SpatialBridge.actorService.onActorJoined += HandleActorJoined;
    }

    private void OnDisable()
    {
        // Unsubscribe when disabled
        SpatialBridge.actorService.onActorJoined -= HandleActorJoined;
    }

    private void HandleActorJoined(ActorJoinedEventArgs args)
    {
        // Get actor reference from the actors collection
        IActor actor = SpatialBridge.actorService.actors[args.actorNumber];
        
        if (args.wasAlreadyJoined)
        {
            // Actor was already in the space when the local actor joined
            Debug.Log($"{actor.displayName} was already in the space");
        }
        else
        {
            // Actor just joined the space
            Debug.Log($"{actor.displayName} just joined the space");
            SpatialBridge.coreGUIService.DisplayToastMessage($"Welcome, {actor.displayName}!");
        }
    }
}
```

### Player Tracking System

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class PlayerTrackingSystem : MonoBehaviour
{
    // Dictionary to track player join times
    private Dictionary<int, System.DateTime> playerJoinTimes = new Dictionary<int, System.DateTime>();
    
    // Track total player count
    private int totalPlayerCount = 0;
    
    private void OnEnable()
    {
        // Subscribe to player join/leave events
        SpatialBridge.actorService.onActorJoined += HandleActorJoined;
        SpatialBridge.actorService.onActorLeft += HandleActorLeft;
        
        // Initialize with current player count
        totalPlayerCount = SpatialBridge.actorService.actorCount;
    }
    
    private void OnDisable()
    {
        // Unsubscribe when disabled
        SpatialBridge.actorService.onActorJoined -= HandleActorJoined;
        SpatialBridge.actorService.onActorLeft -= HandleActorLeft;
    }
    
    private void HandleActorJoined(ActorJoinedEventArgs args)
    {
        IActor actor = SpatialBridge.actorService.actors[args.actorNumber];
        
        // Record join time
        playerJoinTimes[args.actorNumber] = System.DateTime.Now;
        
        // Only count as a new player if they actually just joined
        if (!args.wasAlreadyJoined)
        {
            totalPlayerCount++;
            UpdatePlayerCountDisplay();
            
            // Notify other systems about new player
            GameEvents.NotifyPlayerJoined(actor);
            
            // If player count reaches threshold, trigger special event
            if (totalPlayerCount >= 10)
            {
                TriggerSpecialEvent();
            }
        }
        
        Debug.Log($"Player joined: {actor.displayName} (Actor #{args.actorNumber})");
        Debug.Log($"Total players: {totalPlayerCount}");
    }
    
    private void HandleActorLeft(ActorLeftEventArgs args)
    {
        IActor actor = SpatialBridge.actorService.actors[args.actorNumber];
        
        // Calculate session duration if we have join time
        if (playerJoinTimes.TryGetValue(args.actorNumber, out System.DateTime joinTime))
        {
            TimeSpan sessionDuration = System.DateTime.Now - joinTime;
            Debug.Log($"{actor.displayName} session duration: {sessionDuration.TotalMinutes:F1} minutes");
            
            // Remove from tracking dictionary
            playerJoinTimes.Remove(args.actorNumber);
        }
        
        totalPlayerCount--;
        UpdatePlayerCountDisplay();
        
        Debug.Log($"Player left: {actor.displayName} (Actor #{args.actorNumber})");
        Debug.Log($"Total players: {totalPlayerCount}");
    }
    
    private void UpdatePlayerCountDisplay()
    {
        // Update UI showing player count
        // Implementation depends on your UI system
        Debug.Log($"Updated player count display: {totalPlayerCount}");
    }
    
    private void TriggerSpecialEvent()
    {
        // Trigger a special event when player count reaches threshold
        Debug.Log("Special event triggered: 10+ players in the space!");
        SpatialBridge.coreGUIService.DisplayToastMessage("The space is getting crowded! Special event activated!");
    }
    
    // Utility method to get a player's session duration
    public string GetPlayerSessionDuration(int actorNumber)
    {
        if (playerJoinTimes.TryGetValue(actorNumber, out System.DateTime joinTime))
        {
            TimeSpan duration = System.DateTime.Now - joinTime;
            return $"{duration.Hours}h {duration.Minutes}m {duration.Seconds}s";
        }
        
        return "Unknown";
    }
}
```

## Best Practices

1. **Always handle both wasAlreadyJoined states**: Distinguish between actors who were already in the space and those who just joined to provide the appropriate experience for each scenario.

2. **Unsubscribe from events**: Always remember to unsubscribe from the `onActorJoined` event when your component is disabled or destroyed to prevent memory leaks.

3. **Validate actor references**: Before accessing actor properties, ensure the actor reference is valid by checking if it exists in the `actors` collection.

4. **Avoid heavy processing in the event handler**: The event handler should process joins quickly to avoid blocking the main thread. For intensive operations, consider queueing the joins for processing in an update loop.

5. **Combine with onActorLeft**: Usually, `onActorJoined` is paired with `onActorLeft` to create a complete lifecycle management system for players in your space.

## Common Use Cases

1. **Player welcome messages**: Display personalized welcome messages when new players join the space.

2. **Player count tracking**: Keep track of how many players are in the space at any given time.

3. **Session analytics**: Record when players join and how long they stay in the space.

4. **Dynamic gameplay adjustments**: Adjust game difficulty or spawn rates based on the number of players.

5. **Team assignment**: Automatically assign new players to teams or roles upon joining.

6. **Tutorial triggers**: Start tutorials or guidance for new players joining the space.

7. **Dynamic world events**: Trigger special events when player count reaches certain thresholds.

## Related Components

- [IActorService](./IActorService.md): Service that manages actors in the Spatial environment and provides the onActorJoined event.
- [IActor](./IActor.md): Interface that represents an actor in the Spatial environment.
- [ActorLeftEventArgs](./ActorLeftEventArgs.md): Event arguments for when an actor leaves the space.
- [ActorCustomPropertiesChangedEventArgs](./ActorCustomPropertiesChangedEventArgs.md): Event arguments for when an actor's custom properties change.