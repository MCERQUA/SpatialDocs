# ISpaceService

Space Service Interface

Service for managing space-related functionality in Spatial. This service provides functionality for interacting with the current space, handling space transitions, and managing space-specific features.

## Properties

### Space State
| Property | Description |
| --- | --- |
| isSandbox | Sandbox mode status |
| hasLikedSpace | Space like status |
| spacePackageVersion | Package version |

## Methods

### Space Control
| Method | Description |
| --- | --- |
| TeleportToSpace(...) | Space transition |
| OpenURL(...) | Open external URL |
| EnableAvatarToAvatarCollisions(...) | Toggle avatar collisions |

## Events

### Space Updates
| Event | Description |
| --- | --- |
| onSpaceLiked | Space like notification |

## Usage Examples

```csharp
// Example: Space Manager
public class SpaceManager : MonoBehaviour
{
    private ISpaceService spaceService;
    private Dictionary<string, SpaceState> spaceStates;
    private bool isInitialized;

    private class SpaceState
    {
        public bool isActive;
        public float enterTime;
        public Dictionary<string, object> settings;
        public Dictionary<string, object> metadata;
    }

    void Start()
    {
        spaceService = SpatialBridge.spaceService;
        spaceStates = new Dictionary<string, SpaceState>();
        InitializeManager();
        SubscribeToEvents();
    }

    private void InitializeManager()
    {
        InitializeSpaceState(new Dictionary<string, object>
        {
            { "avatarCollisions", true },
            { "sandboxLogging", true },
            { "transitionDelay", 1.0f }
        });
    }

    private void SubscribeToEvents()
    {
        spaceService.onSpaceLiked += HandleSpaceLiked;
    }

    public void TransitionToSpace(
        string spaceId,
        bool useLoadingScreen = true
    )
    {
        try
        {
            spaceService.TeleportToSpace(spaceId, useLoadingScreen);
            
            var state = GetCurrentSpaceState();
            if (state != null)
            {
                state.metadata["lastTransition"] = new Dictionary<string, object>
                {
                    { "targetSpace", spaceId },
                    { "timestamp", DateTime.UtcNow },
                    { "useLoadingScreen", useLoadingScreen }
                };
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error transitioning to space {spaceId}: {e.Message}");
        }
    }

    public void OpenExternalURL(string url)
    {
        try
        {
            spaceService.OpenURL(url);
            
            var state = GetCurrentSpaceState();
            if (state != null)
            {
                state.metadata["lastExternalURL"] = new Dictionary<string, object>
                {
                    { "url", url },
                    { "timestamp", DateTime.UtcNow }
                };
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error opening URL {url}: {e.Message}");
        }
    }

    public void SetAvatarCollisions(bool enabled)
    {
        try
        {
            spaceService.EnableAvatarToAvatarCollisions(enabled);
            
            var state = GetCurrentSpaceState();
            if (state != null)
            {
                state.settings["avatarCollisions"] = enabled;
                state.metadata["lastCollisionUpdate"] = DateTime.UtcNow;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error setting avatar collisions: {e.Message}");
        }
    }

    public void LogInSandbox(
        string message,
        LogType type = LogType.Log
    )
    {
        if (!spaceService.isSandbox)
            return;

        var state = GetCurrentSpaceState();
        if (state == null || !(bool)state.settings["sandboxLogging"])
            return;

        switch (type)
        {
            case LogType.Log:
                Debug.Log($"[Sandbox] {message}");
                break;
            case LogType.Warning:
                Debug.LogWarning($"[Sandbox] {message}");
                break;
            case LogType.Error:
                Debug.LogError($"[Sandbox] {message}");
                break;
        }

        if (state.metadata.ContainsKey("logs"))
        {
            var logs = state.metadata["logs"] as List<Dictionary<string, object>>;
            logs.Add(new Dictionary<string, object>
            {
                { "message", message },
                { "type", type },
                { "timestamp", DateTime.UtcNow }
            });
        }
    }

    private void InitializeSpaceState(
        Dictionary<string, object> settings
    )
    {
        var state = new SpaceState
        {
            isActive = true,
            enterTime = Time.time,
            settings = settings,
            metadata = new Dictionary<string, object>
            {
                { "version", spaceService.spacePackageVersion },
                { "isSandbox", spaceService.isSandbox },
                { "hasLiked", spaceService.hasLikedSpace },
                { "logs", new List<Dictionary<string, object>>() }
            }
        };

        spaceStates["current"] = state;
    }

    private SpaceState GetCurrentSpaceState()
    {
        return spaceStates.GetValueOrDefault("current");
    }

    private void HandleSpaceLiked()
    {
        var state = GetCurrentSpaceState();
        if (state != null)
        {
            state.metadata["hasLiked"] = true;
            state.metadata["likeTime"] = DateTime.UtcNow;
        }
    }

    void OnDestroy()
    {
        if (spaceService != null)
        {
            spaceService.onSpaceLiked -= HandleSpaceLiked;
        }
    }
}

// Example: Space Monitor
public class SpaceMonitor : MonoBehaviour
{
    private SpaceManager spaceManager;
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
        spaceManager = GetComponent<SpaceManager>();
        monitorStates = new Dictionary<string, MonitorState>();
        InitializeMonitors();
    }

    private void InitializeMonitors()
    {
        InitializeMonitor("performance", new Dictionary<string, object>
        {
            { "updateInterval", 5.0f },
            { "warningThreshold", 30.0f },
            { "errorThreshold", 15.0f }
        });

        InitializeMonitor("network", new Dictionary<string, object>
        {
            { "updateInterval", 1.0f },
            { "timeoutThreshold", 5.0f },
            { "retryCount", 3 }
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

1. Space Management
   - Handle transitions
   - Track states
   - Validate actions
   - Cache data

2. Space Control
   - Manage collisions
   - Handle URLs
   - Track versions
   - Monitor likes

3. Performance
   - Batch updates
   - Cache states
   - Handle timing
   - Monitor metrics

4. Error Handling
   - Validate input
   - Handle failures
   - Recover states
   - Log issues

## Common Use Cases

1. Space Types
   - Game spaces
   - Social spaces
   - Sandbox spaces
   - Custom spaces

2. Space Features
   - State tracking
   - URL handling
   - Version control
   - Like system

3. Space Systems
   - Transition handling
   - Collision management
   - Performance monitoring
   - State tracking

4. Space Processing
   - State validation
   - Metric tracking
   - Event handling
   - Log management
