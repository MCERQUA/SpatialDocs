# INetworkingRemoteEventsService

Networking Remote Events Service Interface

Service for managing remote event functionality in Spatial spaces. This service provides functionality for sending and receiving custom network messages between connected actors.

## Methods

### Event Broadcasting
| Method | Description |
| --- | --- |
| RaiseEvent(IReadOnlyCollection<int>, byte, params object[]) | Send to specific actors |
| RaiseEventAll(byte, params object[]) | Send to all actors |
| RaiseEventOthers(byte, params object[]) | Send to other actors |

## Events

### Network Events
| Event | Description |
| --- | --- |
| onEvent | Remote event received |

## Supported Data Types
- Basic Types: byte, bool, short, int, long, float, double, string
- Unity Types: Vector2, Vector3, Vector4, Quaternion, Color, Color32
- Other Types: DateTime

## Usage Examples

```csharp
// Example: Remote Event Manager
public class RemoteEventManager : MonoBehaviour
{
    private INetworkingRemoteEventsService remoteEvents;
    private Dictionary<byte, EventState> eventStates;
    private bool isInitialized;

    private class EventState
    {
        public bool isActive;
        public float lastTriggerTime;
        public Dictionary<string, object> settings;
        public Dictionary<string, object> metrics;
    }

    // Event IDs
    private const byte EVENT_PLAYER_ACTION = 1;
    private const byte EVENT_GAME_STATE = 2;
    private const byte EVENT_CHAT_MESSAGE = 3;

    void Start()
    {
        remoteEvents = SpatialBridge.networkingService.remoteEvents;
        eventStates = new Dictionary<byte, EventState>();
        InitializeManager();
        SubscribeToEvents();
    }

    private void InitializeManager()
    {
        InitializeEventState(EVENT_PLAYER_ACTION, new Dictionary<string, object>
        {
            { "type", "player" },
            { "priority", "high" },
            { "throttle", 0.1f }
        });

        InitializeEventState(EVENT_GAME_STATE, new Dictionary<string, object>
        {
            { "type", "game" },
            { "priority", "medium" },
            { "throttle", 0.5f }
        });

        InitializeEventState(EVENT_CHAT_MESSAGE, new Dictionary<string, object>
        {
            { "type", "chat" },
            { "priority", "low" },
            { "throttle", 0.0f }
        });
    }

    private void SubscribeToEvents()
    {
        remoteEvents.onEvent += HandleRemoteEvent;
    }

    public void SendPlayerAction(
        string action,
        Vector3 position,
        IReadOnlyCollection<int> targetActors = null
    )
    {
        try
        {
            if (!CanSendEvent(EVENT_PLAYER_ACTION))
                return;

            var eventData = new object[]
            {
                action,
                position,
                DateTime.UtcNow
            };

            if (targetActors != null)
            {
                remoteEvents.RaiseEvent(targetActors, EVENT_PLAYER_ACTION, eventData);
            }
            else
            {
                remoteEvents.RaiseEventOthers(EVENT_PLAYER_ACTION, eventData);
            }

            HandleEventSent(EVENT_PLAYER_ACTION, new Dictionary<string, object>
            {
                { "action", action },
                { "position", position },
                { "targets", targetActors }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error sending player action: {e.Message}");
        }
    }

    public void BroadcastGameState(
        string state,
        Dictionary<string, object> stateData
    )
    {
        try
        {
            if (!CanSendEvent(EVENT_GAME_STATE))
                return;

            var eventData = new object[]
            {
                state,
                stateData,
                DateTime.UtcNow
            };

            remoteEvents.RaiseEventAll(EVENT_GAME_STATE, eventData);

            HandleEventSent(EVENT_GAME_STATE, new Dictionary<string, object>
            {
                { "state", state },
                { "stateData", stateData }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error broadcasting game state: {e.Message}");
        }
    }

    public void SendChatMessage(
        string message,
        IReadOnlyCollection<int> recipients = null
    )
    {
        try
        {
            if (!CanSendEvent(EVENT_CHAT_MESSAGE))
                return;

            var eventData = new object[]
            {
                message,
                DateTime.UtcNow
            };

            if (recipients != null)
            {
                remoteEvents.RaiseEvent(recipients, EVENT_CHAT_MESSAGE, eventData);
            }
            else
            {
                remoteEvents.RaiseEventAll(EVENT_CHAT_MESSAGE, eventData);
            }

            HandleEventSent(EVENT_CHAT_MESSAGE, new Dictionary<string, object>
            {
                { "message", message },
                { "recipients", recipients }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error sending chat message: {e.Message}");
        }
    }

    private void HandleRemoteEvent(NetworkingRemoteEventArgs args)
    {
        try
        {
            if (!eventStates.ContainsKey(args.eventID))
                return;

            switch (args.eventID)
            {
                case EVENT_PLAYER_ACTION:
                    HandlePlayerAction(
                        (string)args.eventArgs[0],
                        (Vector3)args.eventArgs[1],
                        (DateTime)args.eventArgs[2]
                    );
                    break;

                case EVENT_GAME_STATE:
                    HandleGameState(
                        (string)args.eventArgs[0],
                        (Dictionary<string, object>)args.eventArgs[1],
                        (DateTime)args.eventArgs[2]
                    );
                    break;

                case EVENT_CHAT_MESSAGE:
                    HandleChatMessage(
                        (string)args.eventArgs[0],
                        (DateTime)args.eventArgs[1]
                    );
                    break;
            }

            HandleEventReceived(args.eventID, new Dictionary<string, object>
            {
                { "sender", args.senderActorNumber },
                { "args", args.eventArgs }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error handling remote event: {e.Message}");
        }
    }

    private void InitializeEventState(
        byte eventId,
        Dictionary<string, object> settings
    )
    {
        var state = new EventState
        {
            isActive = true,
            lastTriggerTime = -float.MaxValue,
            settings = settings,
            metrics = new Dictionary<string, object>()
        };

        eventStates[eventId] = state;
    }

    private bool CanSendEvent(byte eventId)
    {
        if (!eventStates.TryGetValue(eventId, out var state))
            return false;

        if (!state.isActive)
            return false;

        var throttle = (float)state.settings["throttle"];
        if (throttle <= 0)
            return true;

        var timeSinceLastTrigger = Time.time - state.lastTriggerTime;
        return timeSinceLastTrigger >= throttle;
    }

    private void HandleEventSent(
        byte eventId,
        Dictionary<string, object> eventData
    )
    {
        if (!eventStates.TryGetValue(eventId, out var state))
            return;

        state.lastTriggerTime = Time.time;

        UpdateEventMetrics(eventId, new Dictionary<string, object>
        {
            { "lastSent", DateTime.UtcNow },
            { "lastSentData", eventData }
        });
    }

    private void HandleEventReceived(
        byte eventId,
        Dictionary<string, object> eventData
    )
    {
        if (!eventStates.TryGetValue(eventId, out var state))
            return;

        UpdateEventMetrics(eventId, new Dictionary<string, object>
        {
            { "lastReceived", DateTime.UtcNow },
            { "lastReceivedData", eventData }
        });
    }

    private void HandlePlayerAction(
        string action,
        Vector3 position,
        DateTime timestamp
    )
    {
        // Implementation for handling player actions
    }

    private void HandleGameState(
        string state,
        Dictionary<string, object> stateData,
        DateTime timestamp
    )
    {
        // Implementation for handling game state
    }

    private void HandleChatMessage(
        string message,
        DateTime timestamp
    )
    {
        // Implementation for handling chat messages
    }

    private void UpdateEventMetrics(
        byte eventId,
        Dictionary<string, object> metrics
    )
    {
        if (!eventStates.TryGetValue(eventId, out var state))
            return;

        foreach (var kvp in metrics)
        {
            state.metrics[kvp.Key] = kvp.Value;
        }
    }

    void OnDestroy()
    {
        if (remoteEvents != null)
        {
            remoteEvents.onEvent -= HandleRemoteEvent;
        }
    }
}

// Example: Event Monitor
public class EventMonitor : MonoBehaviour
{
    private RemoteEventManager eventManager;
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
        eventManager = GetComponent<RemoteEventManager>();
        monitorStates = new Dictionary<string, MonitorState>();
        InitializeMonitors();
    }

    private void InitializeMonitors()
    {
        InitializeMonitor("performance", new Dictionary<string, object>
        {
            { "updateInterval", 1.0f },
            { "eventThreshold", 100 }
        });

        InitializeMonitor("reliability", new Dictionary<string, object>
        {
            { "updateInterval", 5.0f },
            { "lossThreshold", 0.01f }
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
   - Track metrics
   - Control timing
   - Cache data

2. Message Control
   - Validate data
   - Handle throttling
   - Track delivery
   - Monitor performance

3. Performance
   - Batch messages
   - Cache states
   - Handle timing
   - Optimize payload

4. Error Handling
   - Validate events
   - Handle failures
   - Recover states
   - Log issues

## Common Use Cases

1. Event Types
   - Player actions
   - Game states
   - Chat messages
   - System events

2. Event Features
   - Message targeting
   - State replication
   - Performance monitoring
   - Reliability tracking

3. Event Systems
   - Message routing
   - Event filtering
   - Priority queuing
   - State tracking

4. Event Processing
   - Message handling
   - State management
   - Event filtering
   - Metric tracking
