# ICoreGUIShopService

Core GUI Shop Service Interface

Service for managing shop GUI functionality in Spatial spaces. This service provides functionality for controlling the world shop interface, including item visibility, selection, and availability.

## Properties

### GUI State
| Property | Description |
| --- | --- |
| isGUIEnabled | Shop GUI availability |
| isGUIOpen | Shop GUI open status |

## Methods

### Item Management
| Method | Description |
| --- | --- |
| SelectItem(string) | Select shop item |
| SetItemEnabled(string, bool, string) | Toggle item availability |
| SetItemVisibility(string, bool) | Toggle item visibility |

## Usage Examples

```csharp
// Example: Shop GUI Manager
public class ShopGUIManager : MonoBehaviour
{
    private ICoreGUIShopService shopGUIService;
    private Dictionary<string, ShopGUIState> shopStates;
    private bool isInitialized;

    private class ShopGUIState
    {
        public bool isActive;
        public float lastUpdateTime;
        public Dictionary<string, object> settings;
        public Dictionary<string, ItemState> items;
    }

    private class ItemState
    {
        public bool isEnabled;
        public bool isVisible;
        public string disableReason;
        public Dictionary<string, object> metadata;
    }

    void Start()
    {
        shopGUIService = SpatialBridge.coreGUIShopService;
        shopStates = new Dictionary<string, ShopGUIState>();
        InitializeManager();
    }

    private void InitializeManager()
    {
        InitializeShopState("main", new Dictionary<string, object>
        {
            { "name", "Main Shop" },
            { "levelRequirement", 0 },
            { "updateInterval", 5.0f }
        });

        InitializeShopState("premium", new Dictionary<string, object>
        {
            { "name", "Premium Shop" },
            { "levelRequirement", 10 },
            { "updateInterval", 10.0f }
        });
    }

    public void SelectShopItem(
        string itemId,
        Dictionary<string, object> context = null
    )
    {
        try
        {
            if (!shopGUIService.isGUIOpen)
            {
                Debug.LogWarning("Cannot select item: Shop GUI is not open");
                return;
            }

            shopGUIService.SelectItem(itemId);

            foreach (var state in shopStates.Values)
            {
                if (state.items.TryGetValue(itemId, out var itemState))
                {
                    itemState.metadata["lastSelected"] = DateTime.UtcNow;
                    
                    if (context != null)
                    {
                        foreach (var kvp in context)
                        {
                            itemState.metadata[kvp.Key] = kvp.Value;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error selecting shop item {itemId}: {e.Message}");
        }
    }

    public void SetItemAvailability(
        string itemId,
        bool enabled,
        string reason = null
    )
    {
        try
        {
            shopGUIService.SetItemEnabled(itemId, enabled, reason);

            foreach (var state in shopStates.Values)
            {
                if (state.items.TryGetValue(itemId, out var itemState))
                {
                    itemState.isEnabled = enabled;
                    itemState.disableReason = reason;
                    itemState.metadata["lastUpdate"] = DateTime.UtcNow;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error setting item availability {itemId}: {e.Message}");
        }
    }

    public void SetItemVisibility(
        string itemId,
        bool visible
    )
    {
        try
        {
            shopGUIService.SetItemVisibility(itemId, visible);

            foreach (var state in shopStates.Values)
            {
                if (state.items.TryGetValue(itemId, out var itemState))
                {
                    itemState.isVisible = visible;
                    itemState.metadata["lastVisibilityUpdate"] = DateTime.UtcNow;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error setting item visibility {itemId}: {e.Message}");
        }
    }

    private void InitializeShopState(
        string shopId,
        Dictionary<string, object> settings
    )
    {
        var state = new ShopGUIState
        {
            isActive = true,
            lastUpdateTime = Time.time,
            settings = settings,
            items = new Dictionary<string, ItemState>()
        };

        // Initialize example items
        InitializeShopItem(state, "weapon_sword", new Dictionary<string, object>
        {
            { "name", "Sword" },
            { "category", "weapons" },
            { "levelRequirement", 1 }
        });

        InitializeShopItem(state, "armor_shield", new Dictionary<string, object>
        {
            { "name", "Shield" },
            { "category", "armor" },
            { "levelRequirement", 5 }
        });

        shopStates[shopId] = state;
    }

    private void InitializeShopItem(
        ShopGUIState shopState,
        string itemId,
        Dictionary<string, object> metadata
    )
    {
        var itemState = new ItemState
        {
            isEnabled = true,
            isVisible = true,
            disableReason = null,
            metadata = metadata
        };

        shopState.items[itemId] = itemState;
    }

    private void UpdateShopStates()
    {
        var time = Time.time;
        foreach (var state in shopStates.Values)
        {
            if (!state.isActive)
                continue;

            var interval = (float)state.settings["updateInterval"];
            if (time - state.lastUpdateTime >= interval)
            {
                UpdateShopItems(state);
                state.lastUpdateTime = time;
            }
        }
    }

    private void UpdateShopItems(ShopGUIState state)
    {
        foreach (var item in state.items.Values)
        {
            var levelReq = (int)item.metadata["levelRequirement"];
            var playerLevel = GetPlayerLevel(); // Implementation needed

            if (levelReq > playerLevel)
            {
                if (item.isEnabled)
                {
                    SetItemAvailability(
                        (string)item.metadata["name"],
                        false,
                        $"Requires Level {levelReq}"
                    );
                }
            }
            else if (!item.isEnabled)
            {
                SetItemAvailability(
                    (string)item.metadata["name"],
                    true
                );
            }
        }
    }

    private int GetPlayerLevel()
    {
        // Implementation for getting player level
        return 1;
    }

    void Update()
    {
        UpdateShopStates();
    }
}

// Example: Shop Item Monitor
public class ShopItemMonitor : MonoBehaviour
{
    private ShopGUIManager shopGUIManager;
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
        shopGUIManager = GetComponent<ShopGUIManager>();
        monitorStates = new Dictionary<string, MonitorState>();
        InitializeMonitors();
    }

    private void InitializeMonitors()
    {
        InitializeMonitor("availability", new Dictionary<string, object>
        {
            { "updateInterval", 1.0f },
            { "checkDelay", 0.5f }
        });

        InitializeMonitor("visibility", new Dictionary<string, object>
        {
            { "updateInterval", 5.0f },
            { "checkDelay", 1.0f }
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

1. GUI Management
   - Handle states
   - Track changes
   - Validate actions
   - Cache data

2. Item Control
   - Manage visibility
   - Handle availability
   - Track selection
   - Update states

3. Performance
   - Batch updates
   - Cache states
   - Handle timing
   - Optimize checks

4. Error Handling
   - Validate items
   - Handle failures
   - Recover states
   - Log issues

## Common Use Cases

1. Shop Types
   - Main shop
   - Premium shop
   - Special shop
   - Event shop

2. Shop Features
   - Item selection
   - Availability control
   - Visibility management
   - State tracking

3. Shop Systems
   - Level requirements
   - Currency checks
   - Time restrictions
   - Special conditions

4. Shop Processing
   - State validation
   - Update handling
   - Event management
   - Metric tracking
