# SpatialNetworkVariables.Data

Category: Core Components

Interface/Class/Enum: Class

This class represents the configuration data for a single network variable within the SpatialNetworkVariables component. Each Data instance defines the properties of a network variable, including its identifier, name, type, synchronization rate, and persistence settings.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| id | string | Unique identifier for the network variable. |
| name | string | Name of the network variable used for reference in code and visual scripting. |
| declaration | string | Type declaration for the variable (e.g., "int", "float", "string", "bool", "Vector3"). |
| syncRate | float | Rate at which the variable is synchronized across the network, in seconds. Lower values mean more frequent updates but higher bandwidth usage. |
| saveWithSpace | bool | Whether the variable's value should be saved with the space, allowing it to persist when the space is reloaded. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections.Generic;

public class NetworkVariableConfigurator : MonoBehaviour
{
    // Reference to the SpatialNetworkVariables component
    private SpatialNetworkVariables networkVariables;
    
    // Types of variables we'll support
    private readonly string[] supportedTypes = new string[]
    {
        "int",
        "float",
        "bool",
        "string",
        "Vector2",
        "Vector3",
        "Color"
    };
    
    private void Awake()
    {
        // Get reference to the SpatialNetworkVariables component
        networkVariables = GetComponent<SpatialNetworkVariables>();
        
        if (networkVariables == null)
        {
            Debug.LogError("SpatialNetworkVariables component is missing!");
            return;
        }
    }
    
    private void Start()
    {
        // Display current variable configuration
        DisplayNetworkVariables();
    }
    
    // Method to create a new network variable
    public void CreateNetworkVariable(string name, string type, float syncRate = 0.1f, bool saveWithSpace = false)
    {
        if (networkVariables == null)
            return;
            
        // Validate the type
        if (!IsValidType(type))
        {
            Debug.LogError($"Invalid network variable type: {type}");
            return;
        }
        
        // Check if a variable with this name already exists
        if (HasVariableWithName(name))
        {
            Debug.LogWarning($"A network variable with name '{name}' already exists.");
            return;
        }
        
        // Create a new Data instance
        SpatialNetworkVariables.Data variableData = new SpatialNetworkVariables.Data
        {
            id = networkVariables.GenerateUniqueVariableID(),
            name = name,
            declaration = type,
            syncRate = syncRate,
            saveWithSpace = saveWithSpace
        };
        
        // Add the new variable configuration
        networkVariables.variableSettings.Add(variableData);
        
        Debug.Log($"Created new network variable: {name} ({type}) with sync rate {syncRate}s");
        
        // Refresh to show the updated list
        DisplayNetworkVariables();
    }
    
    // Method to remove a network variable by name
    public void RemoveNetworkVariable(string name)
    {
        if (networkVariables == null)
            return;
            
        // Find the variable with the specified name
        SpatialNetworkVariables.Data variableToRemove = null;
        
        foreach (var variable in networkVariables.variableSettings)
        {
            if (variable.name == name)
            {
                variableToRemove = variable;
                break;
            }
        }
        
        // Remove if found
        if (variableToRemove != null)
        {
            networkVariables.variableSettings.Remove(variableToRemove);
            Debug.Log($"Removed network variable: {name}");
            
            // Refresh to show the updated list
            DisplayNetworkVariables();
        }
        else
        {
            Debug.LogWarning($"No network variable found with name: {name}");
        }
    }
    
    // Method to update a network variable's sync rate
    public void UpdateSyncRate(string name, float newSyncRate)
    {
        if (networkVariables == null)
            return;
            
        // Find the variable with the specified name
        foreach (var variable in networkVariables.variableSettings)
        {
            if (variable.name == name)
            {
                float oldSyncRate = variable.syncRate;
                variable.syncRate = newSyncRate;
                
                Debug.Log($"Updated sync rate for '{name}' from {oldSyncRate}s to {newSyncRate}s");
                
                // Refresh to show the updated list
                DisplayNetworkVariables();
                return;
            }
        }
        
        Debug.LogWarning($"No network variable found with name: {name}");
    }
    
    // Method to toggle the saveWithSpace setting
    public void ToggleSaveWithSpace(string name)
    {
        if (networkVariables == null)
            return;
            
        // Find the variable with the specified name
        foreach (var variable in networkVariables.variableSettings)
        {
            if (variable.name == name)
            {
                variable.saveWithSpace = !variable.saveWithSpace;
                
                Debug.Log($"Toggled saveWithSpace for '{name}' to {variable.saveWithSpace}");
                
                // Refresh to show the updated list
                DisplayNetworkVariables();
                return;
            }
        }
        
        Debug.LogWarning($"No network variable found with name: {name}");
    }
    
    // Helper method to check if a variable with the given name already exists
    private bool HasVariableWithName(string name)
    {
        foreach (var variable in networkVariables.variableSettings)
        {
            if (variable.name == name)
            {
                return true;
            }
        }
        
        return false;
    }
    
    // Helper method to validate the variable type
    private bool IsValidType(string type)
    {
        foreach (var supportedType in supportedTypes)
        {
            if (supportedType == type)
            {
                return true;
            }
        }
        
        return false;
    }
    
    // Display all configured network variables
    private void DisplayNetworkVariables()
    {
        if (networkVariables == null || networkVariables.variableSettings == null)
            return;
            
        Debug.Log($"--- Network Variables ({networkVariables.variableSettings.Count}) ---");
        
        foreach (var variable in networkVariables.variableSettings)
        {
            Debug.Log($"ID: {variable.id}, " +
                    $"Name: {variable.name}, " +
                    $"Type: {variable.declaration}, " +
                    $"Sync Rate: {variable.syncRate}s, " +
                    $"Save With Space: {variable.saveWithSpace}");
        }
        
        Debug.Log("--- End of Network Variables ---");
    }
    
    // Method to configure a set of common game variables
    public void SetupGameVariables()
    {
        if (networkVariables == null)
            return;
            
        // Clear existing variables first
        networkVariables.variableSettings.Clear();
        
        // Add game state variable (saved with space)
        CreateNetworkVariable("gameState", "string", 0.2f, true);
        
        // Add score variables
        CreateNetworkVariable("playerScore", "int", 0.1f, false);
        CreateNetworkVariable("teamScore", "int", 0.1f, true);
        
        // Add timer variable
        CreateNetworkVariable("gameTimer", "float", 0.5f, false);
        
        // Add position variable for a moving object
        CreateNetworkVariable("objectPosition", "Vector3", 0.05f, false);
        
        // Add game flags
        CreateNetworkVariable("isRoundActive", "bool", 0.1f, false);
        CreateNetworkVariable("isPaused", "bool", 0.1f, false);
        
        Debug.Log("Game variables setup complete!");
    }
    
    // Method to export variable configurations for documentation
    public string ExportVariableConfigForDocumentation()
    {
        if (networkVariables == null || networkVariables.variableSettings == null)
            return "No variables configured.";
            
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        
        sb.AppendLine("# Network Variable Configuration");
        sb.AppendLine();
        sb.AppendLine("| Name | Type | Sync Rate | Save With Space | ID |");
        sb.AppendLine("| --- | --- | --- | --- | --- |");
        
        foreach (var variable in networkVariables.variableSettings)
        {
            sb.AppendLine($"| {variable.name} | {variable.declaration} | {variable.syncRate}s | {variable.saveWithSpace} | {variable.id} |");
        }
        
        return sb.ToString();
    }
}
```

## Best Practices

1. Use a consistent naming convention for network variables to improve code readability.
2. Set appropriate sync rates based on how frequently the variable changes:
   - High-frequency updates (positions, rotations): 0.05-0.1s
   - Medium-frequency updates (scores, timers): 0.1-0.3s
   - Low-frequency updates (game states, flags): 0.3-1.0s
3. Only enable saveWithSpace for variables that truly need to persist, as this increases save data size.
4. Use the most appropriate data type for each variable to minimize network bandwidth.
5. Generate unique IDs using the SpatialNetworkVariables.GenerateUniqueVariableID() method rather than creating them manually.
6. Document the purpose and expected value range for each network variable to aid in debugging and maintenance.
7. Group related variables together and consider their synchronization needs as a set.
8. When working with teams, establish clear ownership rules for who can modify which variables.
9. Avoid creating too many high-frequency variables, as this can impact network performance.
10. Consider using the NetworkVariable<T> class in code to access and modify these variables rather than using SpatialNetworkVariables.Data directly at runtime.

## Common Use Cases

1. Defining player statistics in multiplayer games (health, ammo, score, etc.).
2. Creating synchronized game states that all clients can reference.
3. Tracking object positions and rotations that need to be network-synchronized.
4. Setting up timers and counters for game events.
5. Storing and synchronizing environmental properties like weather conditions or time of day.
6. Creating flags for game mechanics (round started, checkpoint reached, etc.).
7. Storing team-based information in competitive games.
8. Tracking collectible item states in shared environments.
9. Maintaining persistent game progress across sessions when enabled with saveWithSpace.
10. Creating synchronized animation parameters or visual effects settings.

## Completed: March 10, 2025