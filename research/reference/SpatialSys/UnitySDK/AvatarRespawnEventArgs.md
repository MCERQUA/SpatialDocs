# AvatarRespawnEventArgs

Category: Actor Service

Struct: Event Arguments

`AvatarRespawnEventArgs` is a struct that contains information about an avatar being respawned in the space. This struct is passed as an argument to the `onRespawned` event of an `IReadOnlyAvatar` when an avatar respawns.

## Properties/Fields

| Property | Description |
| --- | --- |
| isFirstSpawn | A boolean indicating whether this is the first time the avatar has spawned in the space. |

## Methods

This struct does not define any custom methods beyond the standard methods inherited from ValueType.

## Inherited Members

| Member | Description |
| --- | --- |
| Equals(object) | Determines whether the specified object is equal to the current object. |
| GetHashCode() | Serves as the default hash function. |
| ToString() | Returns a string that represents the current object. |

## Usage Examples

### Basic Avatar Respawn Handler

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class AvatarRespawnHandler : MonoBehaviour
{
    private void OnEnable()
    {
        // Check if local actor has an avatar
        if (SpatialBridge.actorService.localActor.hasAvatar)
        {
            RegisterAvatarEvents(SpatialBridge.actorService.localActor.avatar);
        }
        
        // Listen for avatar changes
        SpatialBridge.actorService.localActor.onAvatarExistsChanged += OnAvatarExistsChanged;
    }
    
    private void OnDisable()
    {
        // Unregister from avatar if it exists
        if (SpatialBridge.actorService.localActor.hasAvatar)
        {
            UnregisterAvatarEvents(SpatialBridge.actorService.localActor.avatar);
        }
        
        // Unsubscribe from avatar change events
        SpatialBridge.actorService.localActor.onAvatarExistsChanged -= OnAvatarExistsChanged;
    }
    
    private void OnAvatarExistsChanged(bool exists)
    {
        if (exists)
        {
            RegisterAvatarEvents(SpatialBridge.actorService.localActor.avatar);
        }
        else
        {
            UnregisterAvatarEvents(SpatialBridge.actorService.localActor.avatar);
        }
    }
    
    private void RegisterAvatarEvents(IReadOnlyAvatar avatar)
    {
        avatar.onRespawned += HandleAvatarRespawned;
    }
    
    private void UnregisterAvatarEvents(IReadOnlyAvatar avatar)
    {
        avatar.onRespawned -= HandleAvatarRespawned;
    }
    
    private void HandleAvatarRespawned(AvatarRespawnEventArgs args)
    {
        if (args.isFirstSpawn)
        {
            // First time this avatar has spawned in the space
            Debug.Log("Welcome! This is your first spawn in this space.");
            SpatialBridge.coreGUIService.DisplayToastMessage("Welcome to the space!");
            
            // Initialize player state
            InitializePlayerState();
            
            // Show tutorial or intro sequence
            ShowTutorial();
        }
        else
        {
            // This is a respawn after already being in the space
            Debug.Log("You have respawned.");
            SpatialBridge.coreGUIService.DisplayToastMessage("Respawned!");
            
            // Reset player state as needed
            ResetTemporaryPlayerState();
        }
    }
    
    private void InitializePlayerState()
    {
        // Initialize player-specific state for new players
        Debug.Log("Initializing new player state");
    }
    
    private void ResetTemporaryPlayerState()
    {
        // Reset temporary state that shouldn't persist after respawn
        Debug.Log("Resetting temporary player state");
    }
    
    private void ShowTutorial()
    {
        // Show tutorial for first-time players
        Debug.Log("Showing tutorial for new player");
    }
}
```

### Comprehensive Respawn Manager

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private GameObject respawnEffectPrefab;
    [SerializeField] private GameObject firstSpawnEffectPrefab;
    
    // Track player spawn counts
    private Dictionary<int, int> playerSpawnCounts = new Dictionary<int, int>();
    
    private void OnEnable()
    {
        // Subscribe to actor events
        SpatialBridge.actorService.onActorJoined += HandleActorJoined;
        SpatialBridge.actorService.onActorLeft += HandleActorLeft;
        
        // Initialize with existing actors
        foreach (var actor in SpatialBridge.actorService.actors.Values)
        {
            TrackActor(actor);
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe from actor events
        SpatialBridge.actorService.onActorJoined -= HandleActorJoined;
        SpatialBridge.actorService.onActorLeft -= HandleActorLeft;
        
        // Untrack all actors
        foreach (var actor in SpatialBridge.actorService.actors.Values)
        {
            UntrackActor(actor);
        }
        
        playerSpawnCounts.Clear();
    }
    
    private void HandleActorJoined(ActorJoinedEventArgs args)
    {
        IActor actor = SpatialBridge.actorService.actors[args.actorNumber];
        TrackActor(actor);
    }
    
    private void HandleActorLeft(ActorLeftEventArgs args)
    {
        IActor actor = SpatialBridge.actorService.actors[args.actorNumber];
        UntrackActor(actor);
        
        // Remove from spawn count tracking
        playerSpawnCounts.Remove(args.actorNumber);
    }
    
    private void TrackActor(IActor actor)
    {
        // Initialize spawn count for this actor
        if (!playerSpawnCounts.ContainsKey(actor.actorNumber))
        {
            playerSpawnCounts[actor.actorNumber] = 0;
        }
        
        // Register for avatar changes
        actor.onAvatarExistsChanged += (exists) => OnActorAvatarChanged(actor, exists);
        
        // Track current avatar if it exists
        if (actor.hasAvatar)
        {
            RegisterAvatarEvents(actor.actorNumber, actor.avatar);
        }
    }
    
    private void UntrackActor(IActor actor)
    {
        // Unregister from avatar if it exists
        if (actor.hasAvatar)
        {
            UnregisterAvatarEvents(actor.avatar);
        }
    }
    
    private void OnActorAvatarChanged(IActor actor, bool exists)
    {
        if (exists)
        {
            RegisterAvatarEvents(actor.actorNumber, actor.avatar);
        }
        else
        {
            UnregisterAvatarEvents(actor.avatar);
        }
    }
    
    private void RegisterAvatarEvents(int actorNumber, IReadOnlyAvatar avatar)
    {
        avatar.onRespawned += (args) => HandleAvatarRespawned(actorNumber, avatar, args);
    }
    
    private void UnregisterAvatarEvents(IReadOnlyAvatar avatar)
    {
        // We can't unregister the specific delegate, but the avatar will be disposed anyway
    }
    
    private void HandleAvatarRespawned(int actorNumber, IReadOnlyAvatar avatar, AvatarRespawnEventArgs args)
    {
        // Increment spawn count
        playerSpawnCounts[actorNumber] = playerSpawnCounts.GetValueOrDefault(actorNumber, 0) + 1;
        int spawnCount = playerSpawnCounts[actorNumber];
        
        // Log spawn information
        IActor actor = SpatialBridge.actorService.actors[actorNumber];
        Debug.Log($"Avatar respawned - {actor.displayName} ({actorNumber}) - isFirstSpawn: {args.isFirstSpawn}, total spawns: {spawnCount}");
        
        // Create appropriate visual effect
        if (args.isFirstSpawn)
        {
            CreateFirstSpawnEffect(avatar.position);
            
            // Handle first spawn logic
            if (actorNumber == SpatialBridge.actorService.localActorNumber)
            {
                // This is the local player's first spawn
                ShowFirstSpawnUI();
            }
        }
        else
        {
            CreateRespawnEffect(avatar.position);
            
            // Check if this is a significant respawn milestone
            if (spawnCount % 10 == 0)
            {
                Debug.Log($"Milestone: {actor.displayName} has respawned {spawnCount} times!");
            }
        }
    }
    
    private void CreateFirstSpawnEffect(Vector3 position)
    {
        if (firstSpawnEffectPrefab != null)
        {
            Instantiate(firstSpawnEffectPrefab, position, Quaternion.identity);
        }
    }
    
    private void CreateRespawnEffect(Vector3 position)
    {
        if (respawnEffectPrefab != null)
        {
            Instantiate(respawnEffectPrefab, position, Quaternion.identity);
        }
    }
    
    private void ShowFirstSpawnUI()
    {
        // Implementation depends on your UI system
        Debug.Log("Showing first spawn UI for local player");
    }
}
```

## Best Practices

1. **Distinguish between first spawn and respawns**: Use the `isFirstSpawn` property to provide different experiences for players entering the space for the first time versus players who are respawning.

2. **Register for avatar events properly**: Always check if an avatar exists before subscribing to its events, and handle avatar existence changes through the actor's `onAvatarExistsChanged` event.

3. **Unsubscribe from events**: Always remember to unsubscribe from events when your component is disabled or destroyed to prevent memory leaks.

4. **Initialize player state on first spawn**: Use the first spawn event to initialize player-specific state, show tutorials, or welcome messages.

5. **Reset temporary state on respawn**: When players respawn (not their first spawn), reset any temporary state that shouldn't persist across respawns, like temporary buffs or status effects.

6. **Create visual feedback**: Consider creating visual effects or feedback to make respawn events clear to the player and others in the space.

## Common Use Cases

1. **Tutorial initiation**: Start tutorials or introduction sequences for first-time players.

2. **Welcome messages**: Display personalized welcome messages for players entering the space for the first time.

3. **Respawn effects**: Create visual or sound effects when players respawn.

4. **State management**: Reset appropriate player state when respawning, while preserving persistent state.

5. **Spawn point selection**: Determine appropriate spawn points based on whether it's a first spawn or a respawn.

6. **Analytics tracking**: Track first spawns versus respawns for player retention and behavior analytics.

7. **Equipment reset**: Reset or adjust player equipment or loadouts when respawning.

## Related Components

- [IReadOnlyAvatar](./IReadOnlyAvatar.md): Interface that provides read-only access to an avatar, including the onRespawned event.
- [IAvatar](./IAvatar.md): Interface that extends IReadOnlyAvatar with additional methods for controlling the avatar.
- [IActor](./IActor.md): Interface that represents an actor in the Spatial environment, which owns an avatar.