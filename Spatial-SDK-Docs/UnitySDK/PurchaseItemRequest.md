# PurchaseItemRequest

Category: Marketplace Service Related

Structure: Class (Inherits from [SpatialAsyncOperation](./SpatialAsyncOperation.md))

Represents the result of a request to purchase an item from the marketplace. This class inherits from SpatialAsyncOperation and provides details about the purchase operation.

## Properties

| Property | Description |
| --- | --- |
| amount | The quantity of the item that was requested to be purchased. |
| itemID | The unique identifier of the item that was requested to be purchased. |
| succeeded | Indicates whether the purchase operation was successful. |

## Inherited Properties/Methods

| Member | Description |
| --- | --- |
| InvokeCompletionEvent() | Invokes the completion event. |
| completed | Event that is invoked when the operation is completed. |
| isDone | Returns true if the operation is done. |
| keepWaiting | Returns true if the operation is not done. |

## Extension Methods

| Method | Description |
| --- | --- |
| SetCompletedEvent<T>(T, Action<T>) | Sets the completion event, same as setting the event using the completed property, but returns the operation itself. This allows for fluent method chaining. |

## Usage Examples

```csharp
public void PurchaseItem(string itemID, ulong amount = 1)
{
    // Start the purchase process and get the request
    PurchaseItemRequest request = SpatialBridge.marketplaceService.PurchaseItem(itemID, amount);
    
    // Attach a callback to the request completion
    request.completed += OnPurchaseCompleted;
}

// Alternative using the extension method for a more fluent API
public void PurchaseItemFluent(string itemID, ulong amount = 1)
{
    // Start the purchase process and set the completion callback in one line
    SpatialBridge.marketplaceService.PurchaseItem(itemID, amount)
        .SetCompletedEvent(OnPurchaseCompleted);
}

private void OnPurchaseCompleted(SpatialAsyncOperation operation)
{
    // Cast to the specific request type
    PurchaseItemRequest request = (PurchaseItemRequest)operation;
    
    if (request.succeeded)
    {
        Debug.Log($"Successfully purchased {request.amount}x {request.itemID}");
        
        // Handle successful purchase (e.g., update UI, grant rewards)
        UpdateInventoryUI();
        ShowPurchaseSuccessMessage(request.itemID);
    }
    else
    {
        Debug.LogWarning($"Failed to purchase {request.itemID}");
        
        // Handle failed purchase (e.g., show error message)
        ShowPurchaseFailureMessage();
    }
}

// Using with a coroutine
private IEnumerator PurchaseItemCoroutine(string itemID, ulong amount = 1)
{
    // Start the purchase process
    PurchaseItemRequest request = SpatialBridge.marketplaceService.PurchaseItem(itemID, amount);
    
    // Wait for the operation to complete
    yield return request;
    
    // Handle the result
    if (request.succeeded)
    {
        Debug.Log($"Successfully purchased {request.amount}x {request.itemID}");
        UpdateInventoryUI();
    }
    else
    {
        Debug.LogWarning($"Failed to purchase {request.itemID}");
        ShowPurchaseFailureMessage();
    }
}
```

## Best Practices

1. Always check the `succeeded` property before performing actions that assume a successful purchase
2. Provide clear feedback to users about the status of their purchase attempts
3. Use the `SetCompletedEvent` extension method for a cleaner, more fluent coding style
4. Consider implementing a timeout mechanism for purchase operations that take too long
5. Don't rely solely on client-side purchase confirmation; validate purchases on the server when possible
6. Handle purchase failures gracefully with appropriate user feedback
7. For high-value or critical items, consider implementing a confirmation dialog before initiating purchase

## Common Use Cases

1. In-game stores where players can purchase virtual items, abilities, or cosmetics
2. Premium content unlocking in freemium or tiered access experiences
3. Consumable item purchases like power-ups, boosts, or time-savers
4. Season pass or battle pass tier purchases
5. Virtual currency conversions (e.g., purchasing world currency with Spatial Coins)
6. Virtual merchandise shops for avatar customization items
7. Event ticket or special access purchases

## Completed: March 9, 2025
