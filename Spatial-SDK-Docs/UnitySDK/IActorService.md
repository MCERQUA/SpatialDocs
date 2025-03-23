# IActorService

Actor Service Interface

Service for managing actor functionality in Spatial spaces. This service provides functionality for handling actors, including player representation, state management, and actor interactions.

## Properties

### Actor State
| Property | Description |
| --- | --- |
| localActor | Current local actor |
| actors | Connected actors list |
| actorCount | Total actor count |

## Methods

### Actor Management
| Method | Description |
| --- | --- |
| GetActor(int) | Get actor by ID |
| GetActorByUserId(string) | Get actor by user ID |
| GetActorsByUserIds(IReadOnlyCollection<string>) | Get actors by user IDs |

## Usage Examples

```csharp
// Example: Actor Manager
public class ActorManager : MonoBehaviour
{
    private IActorService actorService;
    private Dictionary<string, ActorState> actorStates;
    private bool isInitialized;

    private class ActorState
    {
        public bool isActive;
        public float lastUpdateTime;
        public Dictionary<string, object> settings;
        public Dictionary<string, object> metrics;
    }

    void Start()
    {
        actorService = SpatialBridge.actorService;
        actorStates = new Dictionary<string, ActorState>();
        InitializeManager();
    }

    private void InitializeManager()
    {
        InitializeActorState("local", new Dictionary<string, object>
        {
            { "type", "player" },
            { "priority", "high" },
            { "updateInterval", 0.1f }
        });

        InitializeActorState("remote", new Dictionary<string, object>
        {
            { "type", "other" },
            { "priority", "medium" },
            { "updateInterval", 0.2f }
        });
    }

    public IActor GetActorById(int actorId)
    {
        try
        {
            var actor = actorService.GetActor(actorId);
            if (actor != null)
            {
                UpdateActorMetrics("remote", new Dictionary<string, object>
                {
                    { "lastAccess", DateTime.UtcNow },
                    { "actorId", actorId }
                });
            }
            return actor;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error getting actor by ID: {e.Message}");
            return null;
        }
    }

    public IActor GetActorByUser(string userId)
    {
        try
        {
            var actor = actorService.GetActorByUserId(userId);
            if (actor != null)
            {
                UpdateActorMetrics("remote", new Dictionary<string, object>
                {
                    { "lastAccess", DateTime.UtcNow },
                    { "userId", userId }
                });
            }
            return actor;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error getting actor by user ID: {e.Message}");
            return null;
        }
    }

    public IReadOnlyList<IActor> GetActorsByUsers(
        IReadOnlyCollection<string> userIds
    )
    {
        try
        {
            var actors = actorService.GetActorsByUserIds(userIds);
            if (actors != null && actors.Count > 0)
            {
                UpdateActorMetrics("remote", new Dictionary<string, object>
                {
                    { "lastAccess", DateTime.UtcNow },
                    { "userIds", userIds },
                    { "actorCount", actors.Count }
                });
            }
            return actors;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error getting actors by user IDs: {e.Message}");
            return null;
        }
    }

    public void UpdateLocalActor(Dictionary<string, object> properties)
    {
        try
        {
            var localActor = actorService.localActor;
            if (localActor == null)
            {
                Debug.LogWarning("Local actor not available");
                return;
            }

            foreach (var kvp in properties)
            {
                // Implementation for updating actor properties
            }

            UpdateActorMetrics("local", new Dictionary<string, object>
            {
                { "lastUpdate", DateTime.UtcNow },
                { "properties", properties }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating local actor: {e.Message}");
        }
    }

    private void InitializeActorState(
        string stateId,
        Dictionary<string, object> settings
    )
    {
        var state = new ActorState
        {
            isActive = true,
            lastUpdateTime = Time.time,
            settings = settings,
            metrics = new Dictionary<string, object>()
        };

        actorStates[stateId] = state;
    }

    private void UpdateActorMetrics(
        string stateId,
        Dictionary<string, object> metrics
    )
    {
        if (!actorStates.TryGetValue(stateId, out var state))
            return;

        foreach (var kvp in metrics)
        {
            state.metrics[kvp.Key] = kvp.Value;
        }

        state.lastUpdateTime = Time.time;
    }
}

// Example: Actor Monitor
public class ActorMonitor : MonoBehaviour
{
    private ActorManager actorManager;
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
        actorManager = GetComponent<ActorManager>();
        monitorStates = new Dictionary<string, MonitorState>();
        InitializeMonitors();
    }

    private void InitializeMonitors()
    {
        InitializeMonitor("presence", new Dictionary<string, object>
        {
            { "updateInterval", 1.0f },
            { "checkThreshold", 5.0f }
        });

        InitializeMonitor("activity", new Dictionary<string, object>
        {
            { "updateInterval", 2.0f },
            { "idleThreshold", 30.0f }
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

1. Actor Management
   - Handle states
   - Track metrics
   - Control updates
   - Cache data

2. Actor Control
   - Manage properties
   - Handle presence
   - Track activity
   - Monitor state

3. Performance
   - Batch updates
   - Cache states
   - Handle timing
   - Optimize checks

4. Error Handling
   - Validate actors
   - Handle failures
   - Recover states
   - Log issues

## Common Use Cases

1. Actor Types
   - Local player
   - Remote players
   - NPCs
   - System actors

2. Actor Features
   - State management
   - Property sync
   - Presence tracking
   - Activity monitoring

3. Actor Systems
   - Player management
   - State replication
   - Property updates
   - Event handling

4. Actor Processing
   - State validation
   - Update handling
   - Event processing
   - Metric tracking
