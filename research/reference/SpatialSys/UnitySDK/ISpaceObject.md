# ISpaceObject

Space Object Interface

Interface representing a synchronized object in a Spatial space. This interface provides read and write access to object properties, variables, and ownership controls.

## Properties

### Transform
| Property | Description |
| --- | --- |
| position | World position |
| rotation | World rotation |
| scale | World scale |

### Identity
| Property | Description |
| --- | --- |
| objectID | Unique identifier |
| objectType | Object category |
| flags | Behavior flags |

### Ownership
| Property | Description |
| --- | --- |
| ownerActorNumber | Current owner |
| creatorActorNumber | Original creator |
| isMine | Local ownership |
| hasControl | Control status |
| canTakeOwnership | Ownership available |

### State
| Property | Description |
| --- | --- |
| isDisposed | Destroyed status |
| hasVariables | Has custom data |
| variables | Synced variables |

## Methods

### Variable Management
| Method | Description |
| --- | --- |
| SetVariable<T>(byte, T) | Set variable |
| SetVariable(byte, object) | Set untyped |
| RemoveVariable(byte) | Remove variable |
| TryGetVariable(byte, out object) | Get variable |

## Events

### State Changes
| Event | Description |
| --- | --- |
| onOwnerChanged | Owner updates |
| onVariablesChanged | Variable updates |

## Usage Examples

```csharp
// Example: Space Object Handler
public class SpaceObjectHandler : MonoBehaviour
{
    private ISpaceObject spaceObject;
    private Dictionary<string, ObjectState> objectStates;
    private bool isInitialized;

    private class ObjectState
    {
        public bool isActive;
        public float lastUpdateTime;
        public Dictionary<string, object> settings;
        public Dictionary<string, object> metrics;
    }

    void Start()
    {
        spaceObject = GetComponent<ISpaceObject>();
        objectStates = new Dictionary<string, ObjectState>();
        InitializeHandler();
        SubscribeToEvents();
    }

    private void InitializeHandler()
    {
        InitializeObjectState("transform", new Dictionary<string, object>
        {
            { "syncInterval", 0.1f },
            { "interpolation", true },
            { "threshold", 0.01f }
        });

        InitializeObjectState("variables", new Dictionary<string, object>
        {
            { "syncInterval", 0.2f },
            { "compression", true },
            { "maxSize", 1024 }
        });
    }

    private void SubscribeToEvents()
    {
        spaceObject.onOwnerChanged += HandleOwnerChanged;
        spaceObject.onVariablesChanged += HandleVariablesChanged;
    }

    public void UpdateTransform(
        Vector3? position = null,
        Quaternion? rotation = null,
        Vector3? scale = null
    )
    {
        try
        {
            if (!CanModifyTransform())
                return;

            if (position.HasValue)
                spaceObject.position = position.Value;
            
            if (rotation.HasValue)
                spaceObject.rotation = rotation.Value;
            
            if (scale.HasValue)
                spaceObject.scale = scale.Value;

            UpdateObjectMetrics("transform", new Dictionary<string, object>
            {
                { "lastUpdate", DateTime.UtcNow },
                { "position", spaceObject.position },
                { "rotation", spaceObject.rotation },
                { "scale", spaceObject.scale }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating transform: {e.Message}");
        }
    }

    public void SetObjectVariable<T>(
        byte key,
        T value,
        bool overwrite = true
    )
    {
        try
        {
            if (!CanModifyVariables())
                return;

            if (!overwrite && spaceObject.variables.ContainsKey(key))
                return;

            spaceObject.SetVariable(key, value);

            UpdateObjectMetrics("variables", new Dictionary<string, object>
            {
                { "lastSet", DateTime.UtcNow },
                { "key", key },
                { "type", typeof(T).Name }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error setting variable: {e.Message}");
        }
    }

    public void RemoveObjectVariable(byte key)
    {
        try
        {
            if (!CanModifyVariables())
                return;

            if (!spaceObject.variables.ContainsKey(key))
                return;

            spaceObject.RemoveVariable(key);

            UpdateObjectMetrics("variables", new Dictionary<string, object>
            {
                { "lastRemove", DateTime.UtcNow },
                { "key", key }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error removing variable: {e.Message}");
        }
    }

    private void InitializeObjectState(
        string stateId,
        Dictionary<string, object> settings
    )
    {
        var state = new ObjectState
        {
            isActive = true,
            lastUpdateTime = Time.time,
            settings = settings,
            metrics = new Dictionary<string, object>()
        };

        objectStates[stateId] = state;
    }

    private bool CanModifyTransform()
    {
        return spaceObject != null &&
               !spaceObject.isDisposed &&
               spaceObject.isMine &&
               objectStates["transform"].isActive;
    }

    private bool CanModifyVariables()
    {
        return spaceObject != null &&
               !spaceObject.isDisposed &&
               spaceObject.isMine &&
               objectStates["variables"].isActive;
    }

    private void HandleOwnerChanged(int previousOwner, int newOwner)
    {
        UpdateObjectMetrics("ownership", new Dictionary<string, object>
        {
            { "changeTime", DateTime.UtcNow },
            { "previousOwner", previousOwner },
            { "newOwner", newOwner }
        });
    }

    private void HandleVariablesChanged(
        IReadOnlyDictionary<byte, object> changes
    )
    {
        UpdateObjectMetrics("variables", new Dictionary<string, object>
        {
            { "changeTime", DateTime.UtcNow },
            { "changeCount", changes.Count },
            { "totalVariables", spaceObject.variables.Count }
        });
    }

    private void UpdateObjectMetrics(
        string stateId,
        Dictionary<string, object> metrics
    )
    {
        if (!objectStates.TryGetValue(stateId, out var state))
            return;

        foreach (var kvp in metrics)
        {
            state.metrics[kvp.Key] = kvp.Value;
        }

        state.lastUpdateTime = Time.time;
    }

    void OnDestroy()
    {
        if (spaceObject != null)
        {
            spaceObject.onOwnerChanged -= HandleOwnerChanged;
            spaceObject.onVariablesChanged -= HandleVariablesChanged;
        }
    }
}

// Example: Object Monitor
public class ObjectMonitor : MonoBehaviour
{
    private SpaceObjectHandler objectHandler;
    private Dictionary<string, MonitorState> monitorStates;
    private bool isInitialized;

    private class MonitorState
    {
        public bool isActive;
        public float lastUpdateTime;
        public Dictionary<string, object> settings;
    }

    void Start()
    {
        objectHandler = GetComponent<SpaceObjectHandler>();
        monitorStates = new Dictionary<string, MonitorState>();
        InitializeMonitors();
    }

    private void InitializeMonitors()
    {
        InitializeMonitor("transform", new Dictionary<string, object>
        {
            { "updateInterval", 0.1f },
            { "positionThreshold", 0.01f }
        });

        InitializeMonitor("variables", new Dictionary<string, object>
        {
            { "updateInterval", 0.2f },
            { "sizeThreshold", 1024 }
        });
    }

    private void InitializeMonitor(
        string monitorId,
        Dictionary<string, object> settings
    )
    {
        var state = new MonitorState
        {
            isActive = true,
            lastUpdateTime = Time.time,
            settings = settings
        };

        monitorStates[monitorId] = state;
    }

    private void UpdateMonitors()
    {
        foreach (var state in monitorStates.Values)
        {
            if (!state.isActive)
                continue;

            var interval = (float)state.settings["updateInterval"];
            if (Time.time - state.lastUpdateTime >= interval)
            {
                UpdateMonitorMetrics(state);
                state.lastUpdateTime = Time.time;
            }
        }
    }

    private void UpdateMonitorMetrics(MonitorState state)
    {
        // Implementation for monitor metric updates
    }

    void Update()
    {
        UpdateMonitors();
    }
}
```

## Best Practices

1. Object Management
   - Handle ownership
   - Track states
   - Manage variables
   - Cache data

2. Transform Control
   - Validate changes
   - Handle sync
   - Track updates
   - Monitor changes

3. Performance
   - Batch updates
   - Cache states
   - Handle timing
   - Optimize sync

4. Error Handling
   - Validate state
   - Handle failures
   - Recover data
   - Log issues

## Common Use Cases

1. Object States
   - Transform sync
   - Variable storage
   - Ownership control
   - State replication

2. Object Features
   - Position updates
   - Variable sync
   - Owner management
   - State tracking

3. Object Systems
   - Transform handling
   - Variable management
   - Ownership control
   - Event processing

4. Object Processing
   - State validation
   - Update handling
   - Event processing
   - Metric tracking
