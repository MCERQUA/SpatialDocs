# INetworkingService

Networking Service Interface

Service for managing networking functionality in Spatial spaces. This service provides comprehensive control over networking features including connectivity, server management, matchmaking, and remote events.

## Properties

### Connection State
| Property | Description |
| --- | --- |
| connectionStatus | Server connection state |
| isConnected | Connection status |
| networkTime | Synced server time |

### Server Info
| Property | Description |
| --- | --- |
| serverCount | Total active servers |
| serverParticipantCount | Current server players |
| serverParticipantCountUnique | Unique server players |
| spaceParticipantCount | Total space players |
| serverMaxParticipantCount | Max server capacity |

### Server State
| Property | Description |
| --- | --- |
| isServerOpen | Server join status |
| isServerVisible | Server visibility |
| isServerInstancingEnabled | Multiple servers allowed |
| isMasterClient | Master client status |
| masterClientActorNumber | Master client ID |

### Remote Events
| Property | Description |
| --- | --- |
| remoteEvents | Network messaging interface |

## Methods

### Server Management
| Method | Description |
| --- | --- |
| SetServerOpen(bool) | Toggle server joining |
| SetServerVisible(bool) | Toggle server visibility |
| SetServerMaxParticipantCount(int) | Set player capacity |
| GetServerProperties() | Get server properties |
| SetServerProperties(...) | Set server properties |

### Server Teleportation
| Method | Description |
| --- | --- |
| TeleportToNewServer(...) | Create new server |
| TeleportToBestMatchServer(...) | Join matching server |
| TeleportActorsToNewServer(...) | Move players to server |

## Events

### Network Events
| Event | Description |
| --- | --- |
| onConnectionStatusChanged | Connection updates |
| onMasterClientChanged | Master client changes |
| onServerParticipantCountChanged | Player count updates |
| onSpaceParticipantCountChanged | Space count updates |

## Usage Examples

```csharp
// Example: Network Manager
public class NetworkManager : MonoBehaviour
{
    private INetworkingService networkingService;
    private Dictionary<string, NetworkState> networkStates;
    private bool isInitialized;

    private class NetworkState
    {
        public bool isActive;
        public float lastUpdateTime;
        public Dictionary<string, object> settings;
        public Dictionary<string, object> metrics;
    }

    void Start()
    {
        networkingService = SpatialBridge.networkingService;
        networkStates = new Dictionary<string, NetworkState>();
        InitializeManager();
        SubscribeToEvents();
    }

    private void InitializeManager()
    {
        InitializeNetworkState("server", new Dictionary<string, object>
        {
            { "maxPlayers", 100 },
            { "isOpen", true },
            { "isVisible", true }
        });

        InitializeNetworkState("matchmaking", new Dictionary<string, object>
        {
            { "region", "us-east" },
            { "gameMode", "default" },
            { "minPlayers", 2 }
        });
    }

    private void SubscribeToEvents()
    {
        networkingService.onConnectionStatusChanged += HandleConnectionStatusChanged;
        networkingService.onMasterClientChanged += HandleMasterClientChanged;
        networkingService.onServerParticipantCountChanged += HandleParticipantCountChanged;
    }

    public void ConfigureServer(
        int maxPlayers,
        bool isOpen = true,
        bool isVisible = true
    )
    {
        try
        {
            if (!networkingService.isConnected)
            {
                Debug.LogWarning("Cannot configure server: Not connected");
                return;
            }

            networkingService.SetServerMaxParticipantCount(maxPlayers);
            networkingService.SetServerOpen(isOpen);
            networkingService.SetServerVisible(isVisible);

            var state = networkStates["server"];
            state.settings["maxPlayers"] = maxPlayers;
            state.settings["isOpen"] = isOpen;
            state.settings["isVisible"] = isVisible;

            UpdateServerProperties();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error configuring server: {e.Message}");
        }
    }

    public void JoinBestMatchServer(
        Dictionary<string, object> properties,
        IReadOnlyCollection<string> propertiesToMatch
    )
    {
        try
        {
            networkingService.TeleportToBestMatchServer(
                100, // maxPlayers
                properties,
                propertiesToMatch
            );

            var state = networkStates["matchmaking"];
            foreach (var kvp in properties)
            {
                state.settings[kvp.Key] = kvp.Value;
            }

            UpdateNetworkMetrics("matchmaking", new Dictionary<string, object>
            {
                { "lastMatchAttempt", DateTime.UtcNow },
                { "matchProperties", properties }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error joining match: {e.Message}");
        }
    }

    public void CreateNewServer(
        Dictionary<string, object> properties,
        bool isOpen = true,
        bool isVisible = true
    )
    {
        try
        {
            networkingService.TeleportToNewServer(
                100, // maxPlayers
                isOpen,
                isVisible,
                properties,
                null // propertiesToMatch
            );

            var state = networkStates["server"];
            foreach (var kvp in properties)
            {
                state.settings[kvp.Key] = kvp.Value;
            }

            UpdateNetworkMetrics("server", new Dictionary<string, object>
            {
                { "creationTime", DateTime.UtcNow },
                { "initialProperties", properties }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error creating server: {e.Message}");
        }
    }

    private void InitializeNetworkState(
        string stateId,
        Dictionary<string, object> settings
    )
    {
        var state = new NetworkState
        {
            isActive = true,
            lastUpdateTime = Time.time,
            settings = settings,
            metrics = new Dictionary<string, object>()
        };

        networkStates[stateId] = state;
    }

    private void UpdateServerProperties()
    {
        if (!networkingService.isConnected)
            return;

        var state = networkStates["server"];
        networkingService.SetServerProperties(
            state.settings.Select(kvp => 
                new KeyValuePair<string, object>(kvp.Key, kvp.Value)
            ).ToList()
        );
    }

    private void HandleConnectionStatusChanged(ServerConnectionStatus status)
    {
        UpdateNetworkMetrics("server", new Dictionary<string, object>
        {
            { "lastConnectionStatus", status },
            { "statusChangeTime", DateTime.UtcNow }
        });
    }

    private void HandleMasterClientChanged(int actorNumber)
    {
        UpdateNetworkMetrics("server", new Dictionary<string, object>
        {
            { "masterClientActor", actorNumber },
            { "masterChangeTime", DateTime.UtcNow }
        });
    }

    private void HandleParticipantCountChanged(int count)
    {
        UpdateNetworkMetrics("server", new Dictionary<string, object>
        {
            { "participantCount", count },
            { "countChangeTime", DateTime.UtcNow }
        });
    }

    private void UpdateNetworkMetrics(
        string stateId,
        Dictionary<string, object> metrics
    )
    {
        if (!networkStates.TryGetValue(stateId, out var state))
            return;

        foreach (var kvp in metrics)
        {
            state.metrics[kvp.Key] = kvp.Value;
        }

        state.lastUpdateTime = Time.time;
    }

    void OnDestroy()
    {
        if (networkingService != null)
        {
            networkingService.onConnectionStatusChanged -= HandleConnectionStatusChanged;
            networkingService.onMasterClientChanged -= HandleMasterClientChanged;
            networkingService.onServerParticipantCountChanged -= HandleParticipantCountChanged;
        }
    }
}

// Example: Network Monitor
public class NetworkMonitor : MonoBehaviour
{
    private NetworkManager networkManager;
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
        networkManager = GetComponent<NetworkManager>();
        monitorStates = new Dictionary<string, MonitorState>();
        InitializeMonitors();
    }

    private void InitializeMonitors()
    {
        InitializeMonitor("connection", new Dictionary<string, object>
        {
            { "updateInterval", 1.0f },
            { "timeoutThreshold", 5.0f }
        });

        InitializeMonitor("performance", new Dictionary<string, object>
        {
            { "updateInterval", 5.0f },
            { "latencyThreshold", 100.0f }
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

1. Network Management
   - Handle connections
   - Track states
   - Monitor metrics
   - Cache data

2. Server Control
   - Manage capacity
   - Handle visibility
   - Track properties
   - Monitor players

3. Performance
   - Monitor latency
   - Cache states
   - Handle timing
   - Optimize updates

4. Error Handling
   - Validate states
   - Handle failures
   - Recover connections
   - Log issues

## Common Use Cases

1. Network Types
   - Server hosting
   - Matchmaking
   - Player migration
   - State sync

2. Network Features
   - Connection management
   - Server configuration
   - Player tracking
   - Property sync

3. Network Systems
   - Server instancing
   - Load balancing
   - Player matching
   - State replication

4. Network Processing
   - Connection handling
   - State management
   - Event processing
   - Metric tracking
