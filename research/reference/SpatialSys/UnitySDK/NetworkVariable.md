# NetworkVariable<T>

Category: Networking Components

Interface/Class/Enum: Class

A generic container for variables that automatically synchronize their values across the network to all clients in a Spatial experience. NetworkVariable provides a way to share state between clients with automatic replication and change detection.

## Properties

| Property | Description |
| --- | --- |
| Value | Gets or sets the current value of the network variable. Setting this property will automatically mark the variable as dirty for network synchronization when ownership permissions allow. |
| previousValue | The previous value of the variable, useful for comparing changes. |
| OwnerHasChanged | Returns true if the owner of the network object containing this variable has changed. |
| HasChanged | Returns true if the value has changed since the last synchronization. |

## Events

| Event | Description |
| --- | --- |
| OnValueChanged | Event triggered when the value of the network variable changes, providing both the previous and new values. |

## Usage Examples

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class ScoreManager : SpatialNetworkBehaviour
{
    // Define a network variable to track the team score
    public NetworkVariable<int> teamScore = new NetworkVariable<int>();
    
    // Define a network variable with a default value
    public NetworkVariable<bool> gameActive = new NetworkVariable<bool>(true);
    
    // More complex data types are also supported
    public NetworkVariable<Vector3> treasurePosition = new NetworkVariable<Vector3>();
    
    private void Start()
    {
        // Subscribe to value change events
        teamScore.OnValueChanged += OnTeamScoreChanged;
        gameActive.OnValueChanged += OnGameActiveChanged;
    }
    
    private void OnTeamScoreChanged(int previousScore, int newScore)
    {
        Debug.Log($"Team score changed from {previousScore} to {newScore}");
        UpdateScoreDisplay(newScore);
    }
    
    private void OnGameActiveChanged(bool previousState, bool newState)
    {
        Debug.Log($"Game active state changed from {previousState} to {newState}");
        
        if (newState)
        {
            StartGame();
        }
        else
        {
            EndGame();
        }
    }
    
    // This method can be called by UI buttons or game events
    public void AddPoints(int points)
    {
        // Only the owner of the network object can modify network variables
        if (IsOwner)
        {
            teamScore.Value += points; // This will be automatically synchronized
        }
        else
        {
            Debug.LogWarning("Cannot modify score: not the owner of this object");
        }
    }
    
    public void SetTreasurePosition(Vector3 position)
    {
        if (IsOwner)
        {
            treasurePosition.Value = position;
        }
    }
    
    public void ToggleGameState()
    {
        if (IsOwner)
        {
            gameActive.Value = !gameActive.Value;
        }
    }
    
    private void UpdateScoreDisplay(int score)
    {
        // Update UI or other game elements
    }
    
    private void StartGame()
    {
        // Implement game start logic
    }
    
    private void EndGame()
    {
        // Implement game end logic
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        // Unsubscribe from events to prevent memory leaks
        teamScore.OnValueChanged -= OnTeamScoreChanged;
        gameActive.OnValueChanged -= OnGameActiveChanged;
    }
}
```

## Best Practices

1. Only modify NetworkVariable values when you have ownership of the object (check IsOwner).
2. Use the OnValueChanged event to react to changes rather than polling in Update().
3. Unsubscribe from OnValueChanged events in OnDestroy() to prevent memory leaks.
4. Group related NetworkVariables in the same network object for better organization.
5. Use appropriate data types for efficient network serialization (primitive types use less bandwidth).
6. Initialize NetworkVariables with default values when possible.
7. Consider using NetworkVariable for important game state rather than frequent or high-precision updates.

## Common Use Cases

1. Synchronizing game state (active, paused, completed) across all clients
2. Tracking scores, resource counts, or other numeric values
3. Sharing object positions or rotations that don't require frame-by-frame precision
4. Maintaining inventory or collectible status
5. Broadcasting settings or configuration values to all players
6. Communicating round or phase changes in multiplayer games
7. Implementing voting systems or collaborative puzzles

## Supported Data Types

NetworkVariable supports various data types including:
- Basic types: int, float, bool, string
- Unity types: Vector2, Vector3, Quaternion, Color
- Enums (serialized as integers)
- More complex types may require custom serialization

## Limitations

1. NetworkVariables should be used within SpatialNetworkBehaviour components
2. Only the owner of a network object can modify its NetworkVariables
3. For high-frequency updates (like character movement), consider using more optimized approaches
4. There is a limit to how many NetworkVariables should be used per object for performance reasons
5. Complex custom types require additional serialization support

## Completed: March 10, 2025