# IMarketplaceService

Marketplace Service Interface

Service for managing marketplace functionality in Spatial spaces. This service provides functionality for handling item purchases and marketplace-related events.

## Methods

### Purchase Management
| Method | Description |
| --- | --- |
| PurchaseItem(string, ulong, bool) | Process item purchase |

## Events

### Purchase Events
| Event | Description |
| --- | --- |
| onItemPurchased | Purchase completion |

## Usage Examples

```csharp
// Example: Marketplace Manager
public class MarketplaceManager : MonoBehaviour
{
    private IMarketplaceService marketplaceService;
    private Dictionary<string, MarketplaceState> marketplaceStates;
    private bool isInitialized;

    private class MarketplaceState
    {
        public bool isActive;
        public float lastUpdateTime;
        public Dictionary<string, object> settings;
        public Dictionary<string, object> transactions;
    }

    void Start()
    {
        marketplaceService = SpatialBridge.marketplaceService;
        marketplaceStates = new Dictionary<string, MarketplaceState>();
        InitializeManager();
        SubscribeToEvents();
    }

    private void InitializeManager()
    {
        InitializeMarketplace();
    }

    private void SubscribeToEvents()
    {
        marketplaceService.onItemPurchased += HandleItemPurchased;
    }

    private void InitializeMarketplace()
    {
        InitializeMarketplaceState("shop", new Dictionary<string, object>
        {
            { "name", "Main Shop" },
            { "currency", "coins" },
            { "confirmPurchases", true }
        });

        InitializeMarketplaceState("premium", new Dictionary<string, object>
        {
            { "name", "Premium Shop" },
            { "currency", "gems" },
            { "confirmPurchases", true }
        });
    }

    public void PurchaseItem(
        string itemId,
        ulong price,
        bool useWorldCurrency = false,
        Dictionary<string, object> context = null
    )
    {
        try
        {
            marketplaceService.PurchaseItem(itemId, price, useWorldCurrency);

            var transaction = new Dictionary<string, object>
            {
                { "itemId", itemId },
                { "price", price },
                { "currency", useWorldCurrency ? "world" : "coins" },
                { "timestamp", DateTime.UtcNow }
            };

            if (context != null)
            {
                foreach (var kvp in context)
                {
                    transaction[kvp.Key] = kvp.Value;
                }
            }

            RecordTransaction(itemId, transaction);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error purchasing item {itemId}: {e.Message}");
        }
    }

    private void InitializeMarketplaceState(
        string shopId,
        Dictionary<string, object> settings
    )
    {
        var state = new MarketplaceState
        {
            isActive = true,
            lastUpdateTime = Time.time,
            settings = settings,
            transactions = new Dictionary<string, object>()
        };

        marketplaceStates[shopId] = state;
    }

    private void RecordTransaction(
        string itemId,
        Dictionary<string, object> transaction
    )
    {
        foreach (var state in marketplaceStates.Values)
        {
            var transactionId = System.Guid.NewGuid().ToString();
            state.transactions[transactionId] = transaction;
            state.lastUpdateTime = Time.time;
        }
    }

    private void HandleItemPurchased(
        string itemId,
        ulong price,
        bool usedWorldCurrency
    )
    {
        try
        {
            var context = new Dictionary<string, object>
            {
                { "purchaseTime", DateTime.UtcNow },
                { "usedWorldCurrency", usedWorldCurrency }
            };

            OnPurchaseComplete(itemId, price, context);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error handling purchase completion: {e.Message}");
        }
    }

    private void OnPurchaseComplete(
        string itemId,
        ulong price,
        Dictionary<string, object> context
    )
    {
        // Handle post-purchase logic
        UpdateInventory(itemId);
        CheckAchievements(itemId, price);
        NotifyPurchaseComplete(itemId, context);
    }

    private void UpdateInventory(string itemId)
    {
        // Implementation for inventory updates
    }

    private void CheckAchievements(
        string itemId,
        ulong price
    )
    {
        // Implementation for achievement checks
    }

    private void NotifyPurchaseComplete(
        string itemId,
        Dictionary<string, object> context
    )
    {
        // Implementation for purchase notifications
    }

    void OnDestroy()
    {
        if (marketplaceService != null)
        {
            marketplaceService.onItemPurchased -= HandleItemPurchased;
        }
    }
}

// Example: Purchase Handler
public class PurchaseHandler : MonoBehaviour
{
    private MarketplaceManager marketplaceManager;
    private Dictionary<string, PurchaseState> purchaseStates;
    private bool isInitialized;

    private class PurchaseState
    {
        public bool isProcessing;
        public float startTime;
        public Dictionary<string, object> settings;
    }

    void Start()
    {
        marketplaceManager = GetComponent<MarketplaceManager>();
        purchaseStates = new Dictionary<string, PurchaseState>();
        InitializeHandler();
    }

    private void InitializeHandler()
    {
        InitializePurchaseState("standard", new Dictionary<string, object>
        {
            { "timeout", 30.0f },
            { "retryCount", 3 },
            { "confirmThreshold", 1000 }
        });

        InitializePurchaseState("premium", new Dictionary<string, object>
        {
            { "timeout", 60.0f },
            { "retryCount", 5 },
            { "confirmThreshold", 5000 }
        });
    }

    private void InitializePurchaseState(
        string purchaseType,
        Dictionary<string, object> settings
    )
    {
        var state = new PurchaseState
        {
            isProcessing = false,
            startTime = 0f,
            settings = settings
        };

        purchaseStates[purchaseType] = state;
    }

    public void ProcessPurchase(
        string itemId,
        ulong price,
        string purchaseType = "standard"
    )
    {
        if (!purchaseStates.TryGetValue(purchaseType, out var state))
        {
            Debug.LogError($"Invalid purchase type: {purchaseType}");
            return;
        }

        if (state.isProcessing)
        {
            Debug.LogWarning("Purchase already in progress");
            return;
        }

        var confirmThreshold = (ulong)state.settings["confirmThreshold"];
        var needsConfirmation = price >= confirmThreshold;

        state.isProcessing = true;
        state.startTime = Time.time;

        marketplaceManager.PurchaseItem(
            itemId,
            price,
            false,
            new Dictionary<string, object>
            {
                { "type", purchaseType },
                { "confirmed", needsConfirmation }
            }
        );
    }

    private void UpdatePurchaseStates()
    {
        var time = Time.time;
        foreach (var state in purchaseStates.Values)
        {
            if (!state.isProcessing)
                continue;

            var timeout = (float)state.settings["timeout"];
            if (time - state.startTime >= timeout)
            {
                HandlePurchaseTimeout(state);
            }
        }
    }

    private void HandlePurchaseTimeout(PurchaseState state)
    {
        state.isProcessing = false;
        // Implementation for timeout handling
    }

    void Update()
    {
        UpdatePurchaseStates();
    }
}
```

## Best Practices

1. Purchase Management
   - Validate items
   - Handle currency
   - Track transactions
   - Cache states

2. Transaction Processing
   - Confirm purchases
   - Handle timeouts
   - Track progress
   - Verify completion

3. Performance
   - Batch updates
   - Cache states
   - Handle timing
   - Optimize checks

4. Error Handling
   - Validate purchases
   - Handle failures
   - Recover states
   - Log issues

## Common Use Cases

1. Purchase Types
   - Item purchases
   - Currency exchange
   - Premium content
   - Subscriptions

2. Marketplace Features
   - Transaction tracking
   - Purchase validation
   - Currency management
   - Inventory updates

3. Shop Systems
   - Item catalogs
   - Price management
   - Currency conversion
   - Purchase history

4. Purchase Processing
   - Transaction validation
   - State management
   - Event handling
   - Reward distribution
