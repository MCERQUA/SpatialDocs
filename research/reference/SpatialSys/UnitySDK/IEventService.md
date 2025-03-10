# IEventService

Event Service Interface

Service for managing event handling functionality in Spatial spaces. This service provides alternative methods for subscribing to events when direct event subscription is prohibited by the Creator Toolkit.

## Methods

### Event Handling
| Method | Description |
| --- | --- |
| AddEventHandler(object, string, Delegate) | Add instance handler |
| AddStaticEventHandler(Type, string, Delegate) | Add static handler |
| AddVisualScriptEventHandler<TArgs>(EventHook, Action<TArgs>) | Add visual script handler |
| RemoveEventHandler(object, string, Delegate) | Remove instance handler |
| RemoveStaticEventHandler(Type, string, Delegate) | Remove static handler |
| RemoveVisualScriptEventHandler(EventHook, Delegate) | Remove visual script handler |

## Usage Examples

```csharp
// Example: Event Manager
public class EventManager : MonoBehaviour
{
    private IEventService eventService;
    private Dictionary<string, EventState> eventStates;
    private bool isInitialized;

    private class EventState
    {
        public bool isActive;
        public float lastTriggerTime;
        public Dictionary<string, object> settings;
        public Dictionary<string, List<Delegate>> handlers;
    }

    void Start()
    {
        eventService = SpatialBridge.eventService;
        eventStates = new Dictionary<string, EventState>();
        InitializeManager();
    }

    private void InitializeManager()
    {
        InitializeEventState("gameplay", new Dictionary<string, object>
        {
            { "category", "gameplay" },
            { "priority", "high" },
            { "throttle", 0.1f }
        });

        InitializeEventState("ui", new Dictionary<string, object>
        {
            { "category", "ui" },
            { "priority", "medium" },
            { "throttle", 0.05f }
        });
    }

    public void AddHandler<T>(
        string eventType,
        object target,
        string eventName,
        Action<T> handler
    )
    {
        try
        {
            if (!eventStates.TryGetValue(eventType, out var state))
            {
                Debug.LogError($"Unknown event type: {eventType}");
                return;
            }

            eventService.AddEventHandler(target, eventName, handler);

            if (!state.handlers.ContainsKey(eventName))
            {
                state.handlers[eventName] = new List<Delegate>();
            }

            state.handlers[eventName].Add(handler);

            HandleHandlerAdded(eventType, new Dictionary<string, object>
            {
                { "target", target },
                { "eventName", eventName },
                { "handlerType", typeof(T).Name }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error adding event handler: {e.Message}");
        }
    }

    public void AddStaticHandler<T>(
        string eventType,
        Type targetType,
        string eventName,
        Action<T> handler
    )
    {
        try
        {
            if (!eventStates.TryGetValue(eventType, out var state))
            {
                Debug.LogError($"Unknown event type: {eventType}");
                return;
            }

            eventService.AddStaticEventHandler(targetType, eventName, handler);

            if (!state.handlers.ContainsKey(eventName))
            {
                state.handlers[eventName] = new List<Delegate>();
            }

            state.handlers[eventName].Add(handler);

            HandleHandlerAdded(eventType, new Dictionary<string, object>
            {
                { "targetType", targetType.Name },
                { "eventName", eventName },
                { "handlerType", typeof(T).Name }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error adding static event handler: {e.Message}");
        }
    }

    public void AddVisualScriptHandler<T>(
        string eventType,
        EventHook hook,
        Action<T> handler
    )
    {
        try
        {
            if (!eventStates.TryGetValue(eventType, out var state))
            {
                Debug.LogError($"Unknown event type: {eventType}");
                return;
            }

            eventService.AddVisualScriptEventHandler(hook, handler);

            if (!state.handlers.ContainsKey(hook.ToString()))
            {
                state.handlers[hook.ToString()] = new List<Delegate>();
            }

            state.handlers[hook.ToString()].Add(handler);

            HandleHandlerAdded(eventType, new Dictionary<string, object>
            {
                { "hook", hook },
                { "handlerType", typeof(T).Name }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error adding visual script handler: {e.Message}");
        }
    }

    public void RemoveHandler<T>(
        string eventType,
        object target,
        string eventName,
        Action<T> handler
    )
    {
        try
        {
            if (!eventStates.TryGetValue(eventType, out var state))
            {
                Debug.LogError($"Unknown event type: {eventType}");
                return;
            }

            eventService.RemoveEventHandler(target, eventName, handler);

            if (state.handlers.TryGetValue(eventName, out var handlers))
            {
                handlers.Remove(handler);
            }

            HandleHandlerRemoved(eventType, new Dictionary<string, object>
            {
                { "target", target },
                { "eventName", eventName },
                { "handlerType", typeof(T).Name }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error removing event handler: {e.Message}");
        }
    }

    private void InitializeEventState(
        string eventType,
        Dictionary<string, object> settings
    )
    {
        var state = new EventState
        {
            isActive = true,
            lastTriggerTime = -float.MaxValue,
            settings = settings,
            handlers = new Dictionary<string, List<Delegate>>()
        };

        eventStates[eventType] = state;
    }

    private void HandleHandlerAdded(
        string eventType,
        Dictionary<string, object> handlerData
    )
    {
        if (!eventStates.TryGetValue(eventType, out var state))
            return;

        state.lastTriggerTime = Time.time;

        UpdateEventMetrics(eventType, new Dictionary<string, object>
        {
            { "lastHandlerAdded", DateTime.UtcNow },
            { "lastHandlerData", handlerData }
        });
    }

    private void HandleHandlerRemoved(
        string eventType,
        Dictionary<string, object> handlerData
    )
    {
        if (!eventStates.TryGetValue(eventType, out var state))
            return;

        state.lastTriggerTime = Time.time;

        UpdateEventMetrics(eventType, new Dictionary<string, object>
        {
            { "lastHandlerRemoved", DateTime.UtcNow },
            { "lastHandlerData", handlerData }
        });
    }

    private void UpdateEventMetrics(
        string eventType,
        Dictionary<string, object> metrics
    )
    {
        if (!eventStates.TryGetValue(eventType, out var state))
            return;

        foreach (var kvp in metrics)
        {
            state.settings[kvp.Key] = kvp.Value;
        }
    }

    void OnDestroy()
    {
        foreach (var state in eventStates.Values)
        {
            foreach (var handlers in state.handlers.Values)
            {
                handlers.Clear();
            }
        }
    }
}

// Example: Event Monitor
public class EventMonitor : MonoBehaviour
{
    private EventManager eventManager;
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
        eventManager = GetComponent<EventManager>();
        monitorStates = new Dictionary<string, MonitorState>();
        InitializeMonitors();
    }

    private void InitializeMonitors()
    {
        InitializeMonitor("performance", new Dictionary<string, object>
        {
            { "updateInterval", 1.0f },
            { "maxHandlers", 100 }
        });

        InitializeMonitor("usage", new Dictionary<string, object>
        {
            { "updateInterval", 5.0f },
            { "checkThreshold", 0.8f }
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

1. Event Management
   - Handle registration
   - Track handlers
   - Control timing
   - Cache data

2. Handler Control
   - Manage lifecycle
   - Handle cleanup
   - Track usage
   - Monitor performance

3. Performance
   - Monitor metrics
   - Cache states
   - Handle timing
   - Optimize dispatch

4. Error Handling
   - Validate handlers
   - Handle failures
   - Recover states
   - Log issues

## Common Use Cases

1. Event Types
   - Instance events
   - Static events
   - Visual scripts
   - Custom events

2. Event Features
   - Handler tracking
   - State management
   - Performance monitoring
   - Cleanup handling

3. Event Systems
   - Event routing
   - Handler pooling
   - Priority queuing
   - State tracking

4. Event Processing
   - Handler dispatch
   - State management
   - Event filtering
   - Metric tracking
