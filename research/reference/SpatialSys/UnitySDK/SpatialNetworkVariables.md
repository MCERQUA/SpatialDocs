# SpatialNetworkVariables

Category: Core Components

Interface/Class/Enum: Class

The SpatialNetworkVariables component enables developers to define and synchronize custom variables across the network in a Spatial environment. This component allows for the creation, management, and synchronization of network variables that can be used in visual scripting or directly in code, providing a way to share state information between clients.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| variableSettings | List<Data> | Collection of network variable settings defining the variables to be synchronized. |
| version | int | Version number of the network variables configuration, used for compatibility checks. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |
| documentationURL | string | URL to documentation for this component. |

## Methods

| Method | Description |
| --- | --- |
| GenerateUniqueVariableID() | Generates a unique identifier for a new network variable. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour
{
    // Reference to the SpatialNetworkVariables component
    private SpatialNetworkVariables networkVariables;
    
    // Reference to the SpatialNetworkObject for ownership checks
    private SpatialNetworkObject networkObject;
    
    // Cached variable IDs for efficient access
    private string scoreVariableID = "score";
    private string gameStateVariableID = "gameState";
    private string timerVariableID = "timer";
    
    // Network variable access through SpatialBridge (for code access)
    private NetworkVariable<int> scoreVariable;
    private NetworkVariable<string> gameStateVariable;
    private NetworkVariable<float> timerVariable;
    
    private void Awake()
    {
        // Get references
        networkVariables = GetComponent<SpatialNetworkVariables>();
        networkObject = GetComponent<SpatialNetworkObject>();
        
        if (networkVariables == null)
        {
            Debug.LogError("SpatialNetworkVariables component is missing!");
            return;
        }
        
        if (networkObject == null)
        {
            Debug.LogError("SpatialNetworkObject component is missing!");
            return;
        }
    }
    
    private void Start()
    {
        InitializeNetworkVariables();
    }
    
    private void Update()
    {
        // Example: Update game timer if we own the object and game is in 'Playing' state
        if (networkObject.isMine && gameStateVariable.Value == "Playing")
        {
            UpdateGameTimer();
        }
        
        // Display current values
        DisplayNetworkVariables();
    }
    
    private void InitializeNetworkVariables()
    {
        // Retrieve the network variables through code
        // These variables must already be defined in the Unity Editor on the SpatialNetworkVariables component
        
        // Get reference to the score variable (int type)
        if (SpatialBridge.networkingService.TryGetNetworkVariable(gameObject, scoreVariableID, out NetworkVariable<int> score))
        {
            scoreVariable = score;
            
            // Subscribe to value change events
            scoreVariable.OnValueChanged += OnScoreChanged;
            
            Debug.Log($"Score variable initialized. Current value: {scoreVariable.Value}");
        }
        else
        {
            Debug.LogError($"Failed to get network variable: {scoreVariableID}");
        }
        
        // Get reference to the game state variable (string type)
        if (SpatialBridge.networkingService.TryGetNetworkVariable(gameObject, gameStateVariableID, out NetworkVariable<string> state))
        {
            gameStateVariable = state;
            
            // Subscribe to value change events
            gameStateVariable.OnValueChanged += OnGameStateChanged;
            
            Debug.Log($"Game state variable initialized. Current value: {gameStateVariable.Value}");
        }
        else
        {
            Debug.LogError($"Failed to get network variable: {gameStateVariableID}");
        }
        
        // Get reference to the timer variable (float type)
        if (SpatialBridge.networkingService.TryGetNetworkVariable(gameObject, timerVariableID, out NetworkVariable<float> timer))
        {
            timerVariable = timer;
            
            // Subscribe to value change events
            timerVariable.OnValueChanged += OnTimerChanged;
            
            Debug.Log($"Timer variable initialized. Current value: {timerVariable.Value}");
        }
        else
        {
            Debug.LogError($"Failed to get network variable: {timerVariableID}");
        }
    }
    
    // Methods to modify network variables (only the owner should call these)
    
    public void StartGame()
    {
        if (!networkObject.isMine)
        {
            Debug.LogWarning("Cannot start game - we don't own this object.");
            return;
        }
        
        // Reset game state
        scoreVariable.Value = 0;
        gameStateVariable.Value = "Playing";
        timerVariable.Value = 60.0f; // 60 second game timer
        
        Debug.Log("Game started!");
    }
    
    public void EndGame()
    {
        if (!networkObject.isMine)
        {
            Debug.LogWarning("Cannot end game - we don't own this object.");
            return;
        }
        
        gameStateVariable.Value = "GameOver";
        
        Debug.Log("Game ended! Final score: " + scoreVariable.Value);
    }
    
    public void AddScore(int points)
    {
        if (!networkObject.isMine)
        {
            Debug.LogWarning("Cannot add score - we don't own this object.");
            return;
        }
        
        if (gameStateVariable.Value != "Playing")
        {
            Debug.LogWarning("Cannot add score - game is not in Playing state.");
            return;
        }
        
        // Update score
        scoreVariable.Value += points;
        
        Debug.Log($"Added {points} points. New score: {scoreVariable.Value}");
    }
    
    private void UpdateGameTimer()
    {
        // Decrease timer
        float newTime = timerVariable.Value - Time.deltaTime;
        
        // Update timer value
        timerVariable.Value = Mathf.Max(0, newTime);
        
        // Check if time ran out
        if (timerVariable.Value <= 0 && gameStateVariable.Value == "Playing")
        {
            EndGame();
        }
    }
    
    // Event handlers for network variable changes
    
    private void OnScoreChanged(int oldValue, int newValue)
    {
        Debug.Log($"Score changed from {oldValue} to {newValue}");
        
        // Update UI or trigger other game logic here
    }
    
    private void OnGameStateChanged(string oldValue, string newValue)
    {
        Debug.Log($"Game state changed from {oldValue} to {newValue}");
        
        // Handle game state transitions
        switch (newValue)
        {
            case "Lobby":
                // Setup lobby UI
                break;
                
            case "Playing":
                // Setup gameplay UI
                break;
                
            case "GameOver":
                // Show game over screen
                break;
        }
    }
    
    private void OnTimerChanged(float oldValue, float newValue)
    {
        // Only log significant changes to avoid spam
        if (Mathf.FloorToInt(oldValue) != Mathf.FloorToInt(newValue))
        {
            Debug.Log($"Timer changed to {Mathf.FloorToInt(newValue)}");
        }
        
        // Update timer UI
    }
    
    private void DisplayNetworkVariables()
    {
        // For debugging purposes
        if (scoreVariable != null && gameStateVariable != null && timerVariable != null)
        {
            string debug = $"Game State: {gameStateVariable.Value} | " +
                          $"Score: {scoreVariable.Value} | " +
                          $"Time: {Mathf.FloorToInt(timerVariable.Value)}";
            
            // This could update a UI element or just log occasionally
            // Debug.Log(debug);
        }
    }
    
    // Example of how to create network variables programmatically
    // (Usually this would be done in the Unity Editor)
    public void SetupNetworkVariables()
    {
        if (networkVariables == null)
            return;
            
        // Clear existing variables
        networkVariables.variableSettings.Clear();
        
        // Add score variable
        SpatialNetworkVariables.Data scoreData = new SpatialNetworkVariables.Data
        {
            id = networkVariables.GenerateUniqueVariableID(),
            name = "score",
            declaration = "int",
            syncRate = 0.1f, // 10 times per second
            saveWithSpace = false
        };
        networkVariables.variableSettings.Add(scoreData);
        
        // Add game state variable
        SpatialNetworkVariables.Data gameStateData = new SpatialNetworkVariables.Data
        {
            id = networkVariables.GenerateUniqueVariableID(),
            name = "gameState",
            declaration = "string",
            syncRate = 0.2f, // 5 times per second
            saveWithSpace = true
        };
        networkVariables.variableSettings.Add(gameStateData);
        
        // Add timer variable
        SpatialNetworkVariables.Data timerData = new SpatialNetworkVariables.Data
        {
            id = networkVariables.GenerateUniqueVariableID(),
            name = "timer",
            declaration = "float",
            syncRate = 0.25f, // 4 times per second
            saveWithSpace = false
        };
        networkVariables.variableSettings.Add(timerData);
        
        Debug.Log("Network variables have been set up programmatically.");
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (scoreVariable != null)
        {
            scoreVariable.OnValueChanged -= OnScoreChanged;
        }
        
        if (gameStateVariable != null)
        {
            gameStateVariable.OnValueChanged -= OnGameStateChanged;
        }
        
        if (timerVariable != null)
        {
            timerVariable.OnValueChanged -= OnTimerChanged;
        }
    }
}
```

## Best Practices

1. Define network variables in the Unity Editor whenever possible for better visibility and easier management.
2. Use appropriate data types for your variables to minimize network bandwidth.
3. Set appropriate sync rates for each variable based on how frequently it needs to be updated - higher rates use more bandwidth.
4. Only modify network variables from code running on the owner of the network object to avoid synchronization conflicts.
5. Subscribe to OnValueChanged events to respond to variable changes rather than polling each frame.
6. Unsubscribe from OnValueChanged events when objects are destroyed to prevent memory leaks.
7. Use the saveWithSpace flag for variables that need to persist when the space is saved.
8. Cache network variable references during initialization rather than looking them up repeatedly.
9. Use descriptive names for network variables to make your code more maintainable.
10. Be aware of network latency when designing systems that depend on synchronized variables - values may not update instantly across all clients.

## Common Use Cases

1. Synchronizing game state across multiple players (lobby, playing, game over, etc.).
2. Tracking scores, health, or other player statistics in multiplayer games.
3. Implementing timers that are synchronized across all clients.
4. Sharing puzzle or game progress with all participants.
5. Keeping track of which items have been collected or objectives completed.
6. Implementing voting systems where players can influence shared outcomes.
7. Creating synchronized animations or effects that all players can see.
8. Managing ownership or state of interactive objects in the environment.
9. Implementing turn-based mechanics in multiplayer games.
10. Storing persistent data that should be saved with the space.

## Completed: March 10, 2025