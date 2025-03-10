# IInventoryService

Inventory Service Interface

Service for managing player inventories and currency in Spatial spaces. This service provides functionality for handling items, currency, and inventory state management.

## Properties

### Inventory State
| Property | Description |
| --- | --- |
| items | Player's inventory items |
| worldCurrencyBalance | Player's currency amount |

## Methods

### Item Management
| Method | Description |
| --- | --- |
| AddItem(string, ulong, bool) | Add inventory item |
| DeleteItem(string) | Remove inventory item |
| SetItemTypeEnabled(ItemType, bool, string) | Toggle item type state |

### Currency Management
| Method | Description |
| --- | --- |
| AwardWorldCurrency(ulong) | Grant currency amount |

## Events

### Item Events
| Event | Description |
| --- | --- |
| onItemOwnedChanged | Item acquisition updates |
| onItemUsed | Item usage notification |
| onItemConsumed | Consumable item usage |

### Currency Events
| Event | Description |
| --- | --- |
| onWorldCurrencyBalanceChanged | Currency balance updates |

## Usage Examples

```csharp
// Example: Inventory Manager
public class InventoryManager : MonoBehaviour
{
    private IInventoryService inventoryService;
    private Dictionary<string, ItemState> itemStates;
    private bool isInitialized;

    private class ItemState
    {
        public bool isOwned;
        public float acquireTime;
        public ulong quantity;
        public Dictionary<string, object> metadata;
    }

    void Start()
    {
        inventoryService = SpatialBridge.inventoryService;
        itemStates = new Dictionary<string, ItemState>();
        InitializeManager();
        SubscribeToEvents();
    }

    private void InitializeManager()
    {
        foreach (var item in inventoryService.items)
        {
            InitializeItemState(item.Key, item.Value.quantity);
        }
    }

    private void SubscribeToEvents()
    {
        inventoryService.onItemOwnedChanged += HandleItemOwnershipChanged;
        inventoryService.onItemUsed += HandleItemUsed;
        inventoryService.onItemConsumed += HandleItemConsumed;
        inventoryService.onWorldCurrencyBalanceChanged += HandleCurrencyChanged;
    }

    public void AddInventoryItem(
        string itemId,
        ulong quantity,
        bool allowDuplicates = false
    )
    {
        try
        {
            inventoryService.AddItem(itemId, quantity, allowDuplicates);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error adding item {itemId}: {e.Message}");
        }
    }

    public void RemoveInventoryItem(string itemId)
    {
        try
        {
            inventoryService.DeleteItem(itemId);
            
            if (itemStates.TryGetValue(itemId, out var state))
            {
                state.isOwned = false;
                state.metadata["removeTime"] = Time.time;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error removing item {itemId}: {e.Message}");
        }
    }

    public void AwardCurrency(ulong amount)
    {
        try
        {
            inventoryService.AwardWorldCurrency(amount);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error awarding currency: {e.Message}");
        }
    }

    private void InitializeItemState(
        string itemId,
        ulong quantity
    )
    {
        var state = new ItemState
        {
            isOwned = true,
            acquireTime = Time.time,
            quantity = quantity,
            metadata = new Dictionary<string, object>
            {
                { "useCount", 0 },
                { "lastUseTime", 0f }
            }
        };

        itemStates[itemId] = state;
    }

    private void HandleItemOwnershipChanged(
        string itemId,
        bool isOwned,
        ulong quantity
    )
    {
        if (!itemStates.TryGetValue(itemId, out var state))
        {
            if (isOwned)
            {
                InitializeItemState(itemId, quantity);
            }
            return;
        }

        state.isOwned = isOwned;
        state.quantity = quantity;
        
        if (isOwned)
        {
            state.acquireTime = Time.time;
            state.metadata["acquireCount"] = 
                (int)state.metadata.GetValueOrDefault("acquireCount", 0) + 1;
        }
        
        OnItemOwnershipChanged(itemId, isOwned, quantity);
    }

    private void HandleItemUsed(string itemId)
    {
        if (!itemStates.TryGetValue(itemId, out var state))
            return;

        state.metadata["useCount"] = 
            (int)state.metadata["useCount"] + 1;
        state.metadata["lastUseTime"] = Time.time;
        
        OnItemUsed(itemId);
    }

    private void HandleItemConsumed(string itemId)
    {
        if (!itemStates.TryGetValue(itemId, out var state))
            return;

        state.metadata["consumeCount"] = 
            (int)state.metadata.GetValueOrDefault("consumeCount", 0) + 1;
        state.metadata["lastConsumeTime"] = Time.time;
        
        OnItemConsumed(itemId);
    }

    private void HandleCurrencyChanged(ulong newBalance)
    {
        OnCurrencyChanged(newBalance);
    }

    private void OnItemOwnershipChanged(
        string itemId,
        bool isOwned,
        ulong quantity
    )
    {
        // Implementation for ownership change handling
    }

    private void OnItemUsed(string itemId)
    {
        // Implementation for item use handling
    }

    private void OnItemConsumed(string itemId)
    {
        // Implementation for item consumption handling
    }

    private void OnCurrencyChanged(ulong newBalance)
    {
        // Implementation for currency change handling
    }

    void OnDestroy()
    {
        if (inventoryService != null)
        {
            inventoryService.onItemOwnedChanged -= HandleItemOwnershipChanged;
            inventoryService.onItemUsed -= HandleItemUsed;
            inventoryService.onItemConsumed -= HandleItemConsumed;
            inventoryService.onWorldCurrencyBalanceChanged -= HandleCurrencyChanged;
        }
    }
}

// Example: Item System Manager
public class ItemSystemManager : MonoBehaviour
{
    private IInventoryService inventoryService;
    private Dictionary<ItemType, SystemState> systemStates;
    private bool isInitialized;

    private class SystemState
    {
        public bool isEnabled;
        public float lastUpdateTime;
        public Dictionary<string, object> settings;
    }

    void Start()
    {
        inventoryService = SpatialBridge.inventoryService;
        systemStates = new Dictionary<ItemType, SystemState>();
        InitializeSystems();
    }

    private void InitializeSystems()
    {
        InitializeItemType(ItemType.Equipment, new Dictionary<string, object>
        {
            { "maxEquipped", 5 },
            { "allowDuplicates", false },
            { "autoEquip", true }
        });

        InitializeItemType(ItemType.Consumable, new Dictionary<string, object>
        {
            { "stackLimit", 99 },
            { "autoStack", true },
            { "useDelay", 1.0f }
        });
    }

    private void InitializeItemType(
        ItemType type,
        Dictionary<string, object> settings
    )
    {
        var state = new SystemState
        {
            isEnabled = true,
            lastUpdateTime = Time.time,
            settings = settings
        };

        systemStates[type] = state;
        UpdateItemTypeState(type);
    }

    public void SetSystemEnabled(
        ItemType type,
        bool enabled,
        string reason = null
    )
    {
        if (!systemStates.TryGetValue(type, out var state))
            return;

        state.isEnabled = enabled;
        state.lastUpdateTime = Time.time;
        
        try
        {
            inventoryService.SetItemTypeEnabled(type, enabled, reason);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error setting {type} system state: {e.Message}");
        }
    }

    private void UpdateItemTypeState(ItemType type)
    {
        if (!systemStates.TryGetValue(type, out var state))
            return;

        // Implementation for type state updates
    }
}
```

## Best Practices

1. Item Management
   - Track ownership
   - Handle quantities
   - Validate changes
   - Cache states

2. Currency Control
   - Validate amounts
   - Track changes
   - Handle errors
   - Log transactions

3. Performance
   - Batch updates
   - Cache data
   - Monitor usage
   - Handle timing

4. Error Handling
   - Validate items
   - Handle failures
   - Recover states
   - Log issues

## Common Use Cases

1. Item Types
   - Equipment items
   - Consumable items
   - Currency items
   - Special items

2. Item Features
   - Ownership tracking
   - Usage monitoring
   - State management
   - Type control

3. Currency Systems
   - Balance tracking
   - Transaction handling
   - Award management
   - History logging

4. Inventory Systems
   - Item management
   - State tracking
   - Event handling
   - Data persistence
