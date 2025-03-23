# IActor

Actor Interface

Interface representing an actor (user) in a Spatial space. This interface provides access to actor properties, state management, and avatar functionality.

## Properties

### Identity
| Property | Description |
| --- | --- |
| actorNumber | Unique server ID |
| userID | User identifier |
| username | Public username |
| displayName | Display name |

### State
| Property | Description |
| --- | --- |
| isDisposed | Actor left status |
| isRegistered | Account status |
| customProperties | Synced properties |
| platform | Client platform |

### Roles
| Property | Description |
| --- | --- |
| isSpaceOwner | Space ownership |
| isSpaceAdministrator | Admin privileges |

### Avatar
| Property | Description |
| --- | --- |
| avatar | Actor avatar |
| hasAvatar | Avatar status |

### Profile
| Property | Description |
| --- | --- |
| profileColor | Profile color |
| isTalking | Voice chat status |

## Methods

### Profile Management
| Method | Description |
| --- | --- |
| GetProfilePicture() | Get profile image |

## Events

### State Changes
| Event | Description |
| --- | --- |
| onAvatarExistsChanged | Avatar updates |
| onCustomPropertiesChanged | Property updates |

## Usage Examples

```csharp
// Example: Actor Handler
public class ActorHandler : MonoBehaviour
{
    private IActor actor;
    private Dictionary<string, ActorPropertyState> propertyStates;
    private bool isInitialized;

    private class ActorPropertyState
    {
        public bool isActive;
        public float lastUpdateTime;
        public Dictionary<string, object> settings;
        public Dictionary<string, object> metrics;
    }

    void Start()
    {
        actor = SpatialBridge.actorService.localActor;
        propertyStates = new Dictionary<string, ActorPropertyState>();
        InitializeHandler();
        SubscribeToEvents();
    }

    private void InitializeHandler()
    {
        InitializePropertyState("profile", new Dictionary<string, object>
        {
            { "type", "identity" },
            { "priority", "high" },
            { "updateInterval", 0.1f }
        });

        InitializePropertyState("avatar", new Dictionary<string, object>
        {
            { "type", "appearance" },
            { "priority", "medium" },
            { "updateInterval", 0.2f }
        });
    }

    private void SubscribeToEvents()
    {
        actor.onCustomPropertiesChanged += HandlePropertiesChanged;
        actor.onAvatarExistsChanged += HandleAvatarChanged;
    }

    public void UpdateCustomProperties(
        Dictionary<string, object> properties
    )
    {
        try
        {
            if (actor.isDisposed)
            {
                Debug.LogWarning("Cannot update properties: Actor disposed");
                return;
            }

            foreach (var kvp in properties)
            {
                actor.customProperties[kvp.Key] = kvp.Value;
            }

            UpdatePropertyMetrics("profile", new Dictionary<string, object>
            {
                { "lastUpdate", DateTime.UtcNow },
                { "properties", properties }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating properties: {e.Message}");
        }
    }

    public void LoadProfilePicture(
        Action<Texture2D> onLoaded = null,
        Action onFailed = null
    )
    {
        try
        {
            var texture = actor.GetProfilePicture();
            if (texture != null)
            {
                UpdatePropertyMetrics("profile", new Dictionary<string, object>
                {
                    { "lastPictureLoad", DateTime.UtcNow },
                    { "pictureSize", texture.width * texture.height }
                });

                onLoaded?.Invoke(texture);
            }
            else
            {
                onFailed?.Invoke();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading profile picture: {e.Message}");
            onFailed?.Invoke();
        }
    }

    private void HandlePropertiesChanged()
    {
        try
        {
            UpdatePropertyMetrics("profile", new Dictionary<string, object>
            {
                { "lastChange", DateTime.UtcNow },
                { "propertyCount", actor.customProperties.Count }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error handling property change: {e.Message}");
        }
    }

    private void HandleAvatarChanged(bool exists)
    {
        try
        {
            UpdatePropertyMetrics("avatar", new Dictionary<string, object>
            {
                { "lastChange", DateTime.UtcNow },
                { "hasAvatar", exists }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error handling avatar change: {e.Message}");
        }
    }

    private void InitializePropertyState(
        string stateId,
        Dictionary<string, object> settings
    )
    {
        var state = new ActorPropertyState
        {
            isActive = true,
            lastUpdateTime = Time.time,
            settings = settings,
            metrics = new Dictionary<string, object>()
        };

        propertyStates[stateId] = state;
    }

    private void UpdatePropertyMetrics(
        string stateId,
        Dictionary<string, object> metrics
    )
    {
        if (!propertyStates.TryGetValue(stateId, out var state))
            return;

        foreach (var kvp in metrics)
        {
            state.metrics[kvp.Key] = kvp.Value;
        }

        state.lastUpdateTime = Time.time;
    }

    void OnDestroy()
    {
        if (actor != null)
        {
            actor.onCustomPropertiesChanged -= HandlePropertiesChanged;
            actor.onAvatarExistsChanged -= HandleAvatarChanged;
        }
    }
}

// Example: Actor Monitor
public class ActorMonitor : MonoBehaviour
{
    private ActorHandler actorHandler;
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
        actorHandler = GetComponent<ActorHandler>();
        monitorStates = new Dictionary<string, MonitorState>();
        InitializeMonitors();
    }

    private void InitializeMonitors()
    {
        InitializeMonitor("properties", new Dictionary<string, object>
        {
            { "updateInterval", 1.0f },
            { "maxProperties", 100 }
        });

        InitializeMonitor("avatar", new Dictionary<string, object>
        {
            { "updateInterval", 2.0f },
            { "checkInterval", 0.5f }
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

1. Actor Properties
   - Handle updates
   - Track changes
   - Validate state
   - Cache data

2. Property Control
   - Manage sync
   - Handle cleanup
   - Track updates
   - Monitor changes

3. Performance
   - Batch updates
   - Cache states
   - Handle timing
   - Optimize checks

4. Error Handling
   - Validate state
   - Handle failures
   - Recover data
   - Log issues

## Common Use Cases

1. Actor States
   - Identity info
   - Custom data
   - Avatar state
   - Voice status

2. Actor Features
   - Property sync
   - Avatar control
   - Profile management
   - State tracking

3. Actor Systems
   - Property updates
   - Avatar handling
   - Event processing
   - State management

4. Actor Processing
   - State validation
   - Update handling
   - Event processing
   - Metric tracking
