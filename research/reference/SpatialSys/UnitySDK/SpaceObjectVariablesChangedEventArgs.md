# SpaceObjectVariablesChangedEventArgs

Category: Space Content Service Related

Interface/Class/Enum: Struct

Arguments for the ISpaceObject.onVariablesChanged event. This struct provides information about variable changes for space objects, including variables that were added, changed, or removed.

## Properties/Fields

| Property | Description |
| --- | --- |
| changedVariables | Variables that were newly added or changed. This dictionary reference is re-pooled and re-used between events, so you should not cache it. |
| removedVariables | Variables that were removed. This list reference is re-pooled and re-used between events, so you should not cache it. |

## Usage Examples

```csharp
// Example: Variable Change Monitor
public class VariableChangeMonitor : MonoBehaviour
{
    private ISpaceObject spaceObject;
    private Dictionary<byte, object> cachedVariables = new Dictionary<byte, object>();
    private List<VariableChange> variableHistory = new List<VariableChange>();
    private bool isInitialized;

    private class VariableChange
    {
        public byte key;
        public object oldValue;
        public object newValue;
        public ChangeType changeType;
        public DateTime changeTime;

        public enum ChangeType
        {
            Added,
            Modified,
            Removed
        }
    }

    void Start()
    {
        spaceObject = GetComponent<ISpaceObject>();
        if (spaceObject != null)
        {
            SubscribeToEvents();
            CacheCurrentVariables();
            isInitialized = true;
        }
        else
        {
            Debug.LogError("Failed to find an ISpaceObject component");
        }
    }

    private void SubscribeToEvents()
    {
        spaceObject.onVariablesChanged += HandleVariablesChanged;
    }

    private void CacheCurrentVariables()
    {
        // Make a deep copy of the current variables
        foreach (var kvp in spaceObject.variables)
        {
            cachedVariables[kvp.Key] = DeepCopy(kvp.Value);
        }
    }

    private object DeepCopy(object original)
    {
        // For simple values, direct assignment is sufficient
        // For complex types, you might need a more robust deep copy method
        return original;
    }

    private void HandleVariablesChanged(SpaceObjectVariablesChangedEventArgs args)
    {
        // Process added or changed variables
        foreach (var kvp in args.changedVariables)
        {
            byte key = kvp.Key;
            object newValue = kvp.Value;
            
            if (cachedVariables.TryGetValue(key, out var oldValue))
            {
                // Variable modified
                RecordVariableChange(key, oldValue, newValue, VariableChange.ChangeType.Modified);
            }
            else
            {
                // Variable added
                RecordVariableChange(key, null, newValue, VariableChange.ChangeType.Added);
            }
            
            // Update our cached version
            cachedVariables[key] = DeepCopy(newValue);
        }
        
        // Process removed variables
        foreach (byte key in args.removedVariables)
        {
            if (cachedVariables.TryGetValue(key, out var oldValue))
            {
                // Variable removed
                RecordVariableChange(key, oldValue, null, VariableChange.ChangeType.Removed);
                cachedVariables.Remove(key);
            }
        }
    }

    private void RecordVariableChange(byte key, object oldValue, object newValue, VariableChange.ChangeType changeType)
    {
        var change = new VariableChange
        {
            key = key,
            oldValue = oldValue,
            newValue = newValue,
            changeType = changeType,
            changeTime = DateTime.UtcNow
        };

        variableHistory.Add(change);
        
        // Log the change
        string changeTypeStr = Enum.GetName(typeof(VariableChange.ChangeType), changeType);
        string oldValueStr = oldValue?.ToString() ?? "null";
        string newValueStr = newValue?.ToString() ?? "null";
        
        Debug.Log($"Variable {key} {changeTypeStr}: {oldValueStr} -> {newValueStr}");
        
        // If this is our object, no need to respond to our own changes
        if (spaceObject.isMine)
            return;
            
        // Otherwise, respond to the change
        RespondToVariableChange(change);
    }

    private void RespondToVariableChange(VariableChange change)
    {
        // Implement custom logic based on the specific variables that changed
        switch (change.key)
        {
            case 1: // Example: Health variable
                UpdateHealthUI(change.newValue);
                break;
                
            case 2: // Example: State variable
                UpdateObjectState(change.newValue);
                break;
                
            case 3: // Example: Visual customization
                UpdateVisualAppearance(change.newValue);
                break;
                
            // Add more cases as needed
        }
    }
    
    private void UpdateHealthUI(object healthValue)
    {
        if (healthValue is float health)
        {
            Debug.Log($"Health updated to: {health}");
            // Update health UI elements
        }
    }
    
    private void UpdateObjectState(object stateValue)
    {
        if (stateValue is string state)
        {
            Debug.Log($"State changed to: {state}");
            // Update object state and behaviors
        }
    }
    
    private void UpdateVisualAppearance(object appearanceValue)
    {
        Debug.Log($"Appearance updated: {appearanceValue}");
        // Update visual elements based on the new appearance value
    }

    // Get the history of changes for a specific variable key
    public List<VariableChange> GetVariableHistory(byte key)
    {
        return variableHistory.Where(change => change.key == key).ToList();
    }

    // Print the history of all variable changes
    public void PrintVariableHistory()
    {
        Debug.Log("== Variable Change History ==");
        
        foreach (var change in variableHistory)
        {
            string changeTypeStr = Enum.GetName(typeof(VariableChange.ChangeType), change.changeType);
            string oldValueStr = change.oldValue?.ToString() ?? "null";
            string newValueStr = change.newValue?.ToString() ?? "null";
            
            Debug.Log($"{change.changeTime.ToLocalTime()}: Variable {change.key} {changeTypeStr}: {oldValueStr} -> {newValueStr}");
        }
        
        Debug.Log("==========================");
    }

    void OnDestroy()
    {
        if (spaceObject != null)
        {
            spaceObject.onVariablesChanged -= HandleVariablesChanged;
        }
    }
}
```

## Best Practices

1. Never cache the `changedVariables` dictionary or `removedVariables` list provided by the event args, as they are reused between events
2. Make a copy of any variable values you need to preserve for comparison or history
3. Handle variable changes based on the specific keys that are important to your application
4. Consider maintaining your own cache of variables to track changes over time
5. Use a structured approach to handle different variable types and meanings
6. Remember that variable changes may be triggered by other clients or the server
7. Implement validation on received variables to ensure they meet expected formats or ranges

## Common Use Cases

1. Synchronizing game state across multiple clients
2. Implementing data-driven behavior changes based on networked variables
3. Creating visual representations of object state (health bars, status indicators, etc.)
4. Building debug and monitoring tools to track object state changes
5. Implementing event-based systems that respond to specific variable changes
6. Tracking the history of state changes for analytics or replays
7. Creating multiplayer experiences with synchronized object properties

## Completed: March 9, 2025
