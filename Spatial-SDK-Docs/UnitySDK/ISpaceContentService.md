# ISpaceContentService

Space Content Service Interface

Service for managing synchronized content in Spatial spaces. This service provides functionality for spawning, managing, and interacting with space objects, including avatars, prefabs, and network objects.

## Properties

### Object Collections
| Property | Description |
| --- | --- |
| allObjects | All space objects |
| networkObjects | Network objects |
| prefabs | Prefab objects |
| avatars | Avatar objects |

### State
| Property | Description |
| --- | --- |
| isSceneInitialized | Scene ready status |

## Methods

### Object Creation
| Method | Description |
| --- | --- |
| SpawnSpaceObject() | Create empty object |
| SpawnPrefabObject(...) | Create from prefab |
| SpawnNetworkObject(...) | Create network object |
| SpawnAvatar(...) | Create NPC avatar |

### Object Management
| Method | Description |
| --- | --- |
| DestroySpaceObject(int) | Remove object |
| TryGetSpaceObjectID(...) | Get object ID |
| TryFindNetworkObject(...) | Find network object |

### Ownership Control
| Method | Description |
| --- | --- |
| GetOwner(...) | Get object owner |
| TakeOwnership(int) | Claim ownership |
| ReleaseOwnership(int) | Release ownership |
| TransferOwnership(int, int) | Transfer ownership |

## Events

### Scene Events
| Event | Description |
| --- | --- |
| onSceneInitialized | Scene ready |
| onObjectSpawned | Object created |
| onObjectDestroyed | Object removed |

## Usage Examples

```csharp
// Example: Content Manager
public class ContentManager : MonoBehaviour
{
    private ISpaceContentService contentService;
    private Dictionary<string, ContentState> contentStates;
    private bool isInitialized;

    private class ContentState
    {
        public bool isActive;
        public float lastUpdateTime;
        public Dictionary<string, object> settings;
        public Dictionary<string, object> metrics;
    }

    void Start()
    {
        contentService = SpatialBridge.spaceContentService;
        contentStates = new Dictionary<string, ContentState>();
        InitializeManager();
        SubscribeToEvents();
    }

    private void InitializeManager()
    {
        InitializeContentState("objects", new Dictionary<string, object>
        {
            { "type", "general" },
            { "maxObjects", 100 },
            { "cleanupInterval", 60.0f }
        });

        InitializeContentState("avatars", new Dictionary<string, object>
        {
            { "type", "npc" },
            { "maxAvatars", 10 },
            { "updateInterval", 0.1f }
        });
    }

    private void SubscribeToEvents()
    {
        contentService.onObjectSpawned += HandleObjectSpawned;
        contentService.onObjectDestroyed += HandleObjectDestroyed;
        contentService.onSceneInitialized += HandleSceneInitialized;
    }

    public void SpawnGameObject(
        string prefabId,
        Vector3 position,
        Quaternion rotation
    )
    {
        try
        {
            if (!CanSpawnObject())
                return;

            contentService.SpawnPrefabObject(
                AssetType.Prefab,
                prefabId,
                position,
                rotation
            ).SetCompletedEvent((response) =>
            {
                if (response.succeeded)
                {
                    HandleObjectCreated(response.spaceObject);
                }
                else
                {
                    Debug.LogError($"Failed to spawn object: {response.error}");
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error spawning object: {e.Message}");
        }
    }

    public void SpawnNPCAvatar(
        string avatarId,
        Vector3 position,
        Quaternion rotation,
        string displayName = null
    )
    {
        try
        {
            if (!CanSpawnAvatar())
                return;

            contentService.SpawnAvatar(
                AssetType.Avatar,
                avatarId,
                position,
                rotation,
                displayName
            ).SetCompletedEvent((response) =>
            {
                if (response.succeeded)
                {
                    HandleAvatarCreated(response.spaceObject);
                }
                else
                {
                    Debug.LogError($"Failed to spawn avatar: {response.error}");
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error spawning avatar: {e.Message}");
        }
    }

    public void DestroyObject(int objectId)
    {
        try
        {
            if (!CanDestroyObject(objectId))
                return;

            contentService.DestroySpaceObject(objectId);

            UpdateContentMetrics("objects", new Dictionary<string, object>
            {
                { "lastDestroy", DateTime.UtcNow },
                { "destroyedId", objectId }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error destroying object: {e.Message}");
        }
    }

    private void InitializeContentState(
        string stateId,
        Dictionary<string, object> settings
    )
    {
        var state = new ContentState
        {
            isActive = true,
            lastUpdateTime = Time.time,
            settings = settings,
            metrics = new Dictionary<string, object>()
        };

        contentStates[stateId] = state;
    }

    private bool CanSpawnObject()
    {
        if (!contentService.isSceneInitialized)
            return false;

        var state = contentStates["objects"];
        var maxObjects = (int)state.settings["maxObjects"];
        return contentService.allObjects.Count < maxObjects;
    }

    private bool CanSpawnAvatar()
    {
        if (!contentService.isSceneInitialized)
            return false;

        var state = contentStates["avatars"];
        var maxAvatars = (int)state.settings["maxAvatars"];
        return contentService.avatars.Count < maxAvatars;
    }

    private bool CanDestroyObject(int objectId)
    {
        if (!contentService.allObjects.ContainsKey(objectId))
            return false;

        var obj = contentService.allObjects[objectId];
        return obj.isMine || obj.hasControl;
    }

    private void HandleObjectSpawned(IReadOnlySpaceObject obj)
    {
        UpdateContentMetrics("objects", new Dictionary<string, object>
        {
            { "lastSpawn", DateTime.UtcNow },
            { "spawnedId", obj.objectID },
            { "objectType", obj.objectType }
        });
    }

    private void HandleObjectDestroyed(IReadOnlySpaceObject obj)
    {
        UpdateContentMetrics("objects", new Dictionary<string, object>
        {
            { "lastDestroy", DateTime.UtcNow },
            { "destroyedId", obj.objectID },
            { "objectType", obj.objectType }
        });
    }

    private void HandleSceneInitialized()
    {
        UpdateContentMetrics("scene", new Dictionary<string, object>
        {
            { "initTime", DateTime.UtcNow },
            { "objectCount", contentService.allObjects.Count }
        });
    }

    private void HandleObjectCreated(ISpaceObject obj)
    {
        UpdateContentMetrics("objects", new Dictionary<string, object>
        {
            { "lastCreate", DateTime.UtcNow },
            { "createdId", obj.objectID },
            { "objectType", obj.objectType }
        });
    }

    private void HandleAvatarCreated(ISpaceObject obj)
    {
        UpdateContentMetrics("avatars", new Dictionary<string, object>
        {
            { "lastCreate", DateTime.UtcNow },
            { "createdId", obj.objectID },
            { "avatarType", obj.GetComponent<IAvatar>()?.GetType().Name }
        });
    }

    private void UpdateContentMetrics(
        string stateId,
        Dictionary<string, object> metrics
    )
    {
        if (!contentStates.TryGetValue(stateId, out var state))
            return;

        foreach (var kvp in metrics)
        {
            state.metrics[kvp.Key] = kvp.Value;
        }

        state.lastUpdateTime = Time.time;
    }

    void OnDestroy()
    {
        if (contentService != null)
        {
            contentService.onObjectSpawned -= HandleObjectSpawned;
            contentService.onObjectDestroyed -= HandleObjectDestroyed;
            contentService.onSceneInitialized -= HandleSceneInitialized;
        }
    }
}

// Example: Content Monitor
public class ContentMonitor : MonoBehaviour
{
    private ContentManager contentManager;
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
        contentManager = GetComponent<ContentManager>();
        monitorStates = new Dictionary<string, MonitorState>();
        InitializeMonitors();
    }

    private void InitializeMonitors()
    {
        InitializeMonitor("objects", new Dictionary<string, object>
        {
            { "updateInterval", 1.0f },
            { "objectThreshold", 50 }
        });

        InitializeMonitor("performance", new Dictionary<string, object>
        {
            { "updateInterval", 5.0f },
            { "memoryThreshold", 1000000 }
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

1. Content Management
   - Handle spawning
   - Track objects
   - Manage ownership
   - Cache data

2. Object Control
   - Validate creation
   - Handle cleanup
   - Track metrics
   - Monitor states

3. Performance
   - Batch updates
   - Cache states
   - Handle timing
   - Optimize spawning

4. Error Handling
   - Validate objects
   - Handle failures
   - Recover states
   - Log issues

## Common Use Cases

1. Content Types
   - Space objects
   - Network objects
   - Prefab objects
   - Avatar objects

2. Content Features
   - Object spawning
   - State management
   - Ownership control
   - Object tracking

3. Content Systems
   - Object pooling
   - State replication
   - Ownership handling
   - Event processing

4. Content Processing
   - State validation
   - Update handling
   - Event processing
   - Metric tracking
