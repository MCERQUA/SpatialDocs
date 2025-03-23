# Obby (Obstacle Course) Game Template

## Overview
The Obby (Obstacle Course) Game template provides a complete, ready-to-use implementation of a classic obstacle course game. It includes all necessary scripts, prefabs, and systems for creating platforming challenges where players navigate through obstacles to reach a goal. The template features checkpoints, various obstacle types, moving platforms, and automatic respawn mechanics.

## Features
- **Modular Obstacle System**: Easy-to-use prefabs for building obstacle courses
- **Checkpoint System**: Automatic saving of progress when players reach checkpoints
- **Respawn Mechanics**: Handles player death and respawning at the latest checkpoint
- **Effect System**: Various trigger effects including kill zones, bounce pads, and push areas
- **Moving Platforms**: System for creating and configuring moving platforms
- **Extensible Framework**: Designed to be easily extended with custom obstacles and effects
- **Performance Optimized**: Built with web performance in mind

## Included Components

### 1. Checkpoint System
Saves player progress throughout the course:
- Automatic checkpoint activation when players touch the checkpoint
- Persistent progress saving between sessions (optional)
- Visual feedback when checkpoints are activated
- Teleport functionality to return to checkpoints

### 2. Obstacle Types
Various obstacle prefabs included in the template:
- **Kill Zones**: Areas that cause the player to respawn when touched
- **Bounce Pads**: Surfaces that propel the player upward or in a direction
- **Push Areas**: Zones that apply force to the player
- **Slippery Surfaces**: Areas with reduced friction
- **Speed Boost Pads**: Zones that temporarily increase player movement speed

### 3. Moving Platform System
Complete system for creating dynamic platforms:
- Linear movement between waypoints
- Circular/rotational movement
- Oscillating movement patterns
- Customizable speed and timing
- Trigger-activated movement

### 4. Level Management
Tools for organizing and structuring multiple levels:
- Level completion detection
- Level transition system
- Optional progress tracking
- Difficulty progression

## Integration with SDK Components
The template integrates with these key SDK components:

| SDK Component | Usage in Template |
|---------------|-------------------|
| IActorService | Player detection and respawn handling |
| IAvatar | Controlling avatar state and properties |
| SpatialTriggerEvent | For obstacle and checkpoint detection |
| IUserWorldDataStoreService | For saving checkpoint progress (optional) |
| SpatialPointOfInterest | For marking level start/end points |
| SpatialNetworkObject | For synchronized moving platforms |

## When to Use
Use this template when:
- Creating platforming challenges or obstacle courses
- Building games that require checkpoint progress
- Implementing games with physics-based obstacles
- Designing levels with dynamic moving elements
- Creating competitive time trial or completion-based experiences

## Implementation Details

### Obstacle Base System
All obstacles inherit from a common base class:

```csharp
public abstract class ObbyObstacleBase : MonoBehaviour
{
    [SerializeField] protected bool applyToLocalAvatarOnly = true;
    
    protected virtual void OnTriggerEnter(Collider other)
    {
        // Check if we should apply effect to this avatar
        if (applyToLocalAvatarOnly)
        {
            if (!IsLocalAvatar(other.gameObject))
                return;
        }
        
        // Apply the obstacle effect
        ApplyEffect(other.gameObject);
    }
    
    protected abstract void ApplyEffect(GameObject avatarObject);
    
    protected bool IsLocalAvatar(GameObject obj)
    {
        if (obj.TryGetComponent<SpatialAvatar>(out var avatar))
        {
            return avatar.actorId == SpatialBridge.actorService.localActor.actorNumber;
        }
        return false;
    }
}
```

### Kill Zone Implementation
Example of a kill zone obstacle implementation:

```csharp
public class ObbyKillZone : ObbyObstacleBase
{
    [SerializeField] private ObbyCheckpointManager checkpointManager;
    
    private void Start()
    {
        if (checkpointManager == null)
        {
            // Try to find checkpoint manager in the scene
            checkpointManager = FindObjectOfType<ObbyCheckpointManager>();
        }
    }
    
    protected override void ApplyEffect(GameObject avatarObject)
    {
        // Respawn the player at the last checkpoint
        if (checkpointManager != null)
        {
            checkpointManager.RespawnAtLastCheckpoint();
        }
        else
        {
            Debug.LogWarning("Kill zone has no reference to checkpoint manager. Player can't respawn.");
        }
    }
}
```

### Checkpoint System
The core checkpoint system keeps track of the player's progress:

```csharp
public class ObbyCheckpointManager : MonoBehaviour
{
    [SerializeField] private Transform defaultSpawnPoint;
    [SerializeField] private bool persistBetweenSessions = false;
    
    private Transform currentCheckpoint;
    private const string CHECKPOINT_KEY = "obby_last_checkpoint";
    
    private void Start()
    {
        currentCheckpoint = defaultSpawnPoint;
        
        if (persistBetweenSessions)
        {
            LoadCheckpointProgress();
        }
    }
    
    public void RegisterCheckpoint(Transform checkpoint)
    {
        currentCheckpoint = checkpoint;
        
        if (persistBetweenSessions)
        {
            SaveCheckpointProgress();
        }
    }
    
    public void RespawnAtLastCheckpoint()
    {
        if (currentCheckpoint != null)
        {
            // Teleport the avatar to the checkpoint position
            var localAvatar = SpatialBridge.actorService.localActor.avatar;
            localAvatar.TeleportTo(currentCheckpoint.position, currentCheckpoint.rotation);
        }
    }
    
    private void SaveCheckpointProgress()
    {
        // In a real implementation, this would save the checkpoint data
        // using IUserWorldDataStoreService
    }
    
    private void LoadCheckpointProgress()
    {
        // In a real implementation, this would load the checkpoint data
        // using IUserWorldDataStoreService
    }
}
```

### Moving Platform System
Example of the moving platform implementation:

```csharp
public class ObbyMovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waypointPauseTime = 0f;
    [SerializeField] private bool loopMovement = true;
    
    [SpatialNetworkVariable]
    private NetworkVariable<int> currentWaypointIndex = new NetworkVariable<int>(0);
    
    [SpatialNetworkVariable]
    private NetworkVariable<float> pauseTimer = new NetworkVariable<float>(0);
    
    private Vector3 startPosition;
    
    private void Awake()
    {
        startPosition = transform.position;
    }
    
    private void Update()
    {
        if (!SpatialBridge.networkingService.IsConnectedToServer)
            return;
            
        if (waypoints.Length == 0)
            return;
            
        // Only the owner updates the position
        if (SpatialBridge.spaceContentService.IsOwner(gameObject))
        {
            UpdatePlatformPosition();
        }
    }
    
    private void UpdatePlatformPosition()
    {
        // Check if we're paused at a waypoint
        if (pauseTimer.Value > 0)
        {
            pauseTimer.Value -= Time.deltaTime;
            return;
        }
        
        // Calculate target position based on waypoints
        Vector3 targetPosition = GetWaypointPosition(currentWaypointIndex.Value);
        
        // Move towards target
        transform.position = Vector3.MoveTowards(
            transform.position, 
            targetPosition, 
            moveSpeed * Time.deltaTime
        );
        
        // Check if we've reached the waypoint
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            // Start pause timer
            if (waypointPauseTime > 0)
            {
                pauseTimer.Value = waypointPauseTime;
            }
            
            // Move to next waypoint
            currentWaypointIndex.Value = GetNextWaypointIndex();
        }
    }
    
    private int GetNextWaypointIndex()
    {
        int nextIndex = currentWaypointIndex.Value + 1;
        
        if (nextIndex >= waypoints.Length)
        {
            if (loopMovement)
                return 0;
            else
                return waypoints.Length - 1;
        }
        
        return nextIndex;
    }
    
    private Vector3 GetWaypointPosition(int index)
    {
        if (index < 0 || index >= waypoints.Length)
            return startPosition;
            
        return waypoints[index].position;
    }
}
```

## Best Practices
- **Start Simple**: Begin with a basic level layout before adding complex elements
- **Playtest Frequently**: Regularly test your obstacle course for difficulty and fun
- **Checkpoint Placement**: Place checkpoints at reasonable intervals to avoid frustration
- **Difficulty Progression**: Gradually increase the difficulty throughout the course
- **Visual Clarity**: Ensure obstacles are visually distinctive to players
- **Performance**: Limit the number of simultaneously active moving platforms
- **Testing**: Test with multiple players to ensure synchronization works properly

## Related Templates
- [Camera Modes](../Camera/CameraModes.md) - For unique camera perspectives in your obby
- [Daily/Weekly Rewards](../UX/DailyRewards.md) - For adding reward systems to your obby
- [Top Down Shooter](./TopDownShooter.md) - For adding combat elements to your obstacle course

## Additional Resources
- [GitHub Repository](https://github.com/spatialsys/spatial-obby-template)
- [Spatial Trigger Events Documentation](../../SpatialSys/UnitySDK/SpatialTriggerEvent.md)
- [Network Variables Documentation](../../SpatialSys/UnitySDK/NetworkVariable.md)
