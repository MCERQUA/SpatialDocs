## Overview
The GetInventoryItemRequest class represents the result of a request to retrieve an item from the user's inventory by its ID. It inherits from SpatialAsyncOperation and provides information about whether the item was found in the inventory.

## Properties
- **itemID**: The ID of the item to get
- **succeeded**: True if the item was found

## Inherited Members
- **InvokeCompletionEvent()**: Invokes the completion event
- **completed**: Event that is invoked when the operation is completed
- **isDone**: Returns true if the operation is done
- **keepWaiting**: Returns true if the operation is not done

## Usage Example
```csharp
public void CheckForItemInInventory(string itemId)
{
    // First, check if the item is already in the cached inventory
    if (SpatialBridge.inventoryService.items.TryGetValue(itemId, out IInventoryItem cachedItem))
    {
        Debug.Log($"Item {itemId} found in cached inventory with amount: {cachedItem.amount}");
        ProcessItem(cachedItem);
        return;
    }
    
    // If not in cache, make a request to check if the user owns the item
    Debug.Log($"Item {itemId} not found in cache, checking server...");
    GetInventoryItemRequest request = SpatialBridge.inventoryService.GetItem(itemId);
    
    // Option 1: Use SetCompletedEvent extension method for a callback
    request.SetCompletedEvent((completedRequest) => {
        if (completedRequest.succeeded)
        {
            Debug.Log($"Successfully found item {completedRequest.itemID} in inventory");
            
            // Now we can access the item through the items dictionary
            if (SpatialBridge.inventoryService.items.TryGetValue(itemId, out IInventoryItem item))
            {
                ProcessItem(item);
            }
        }
        else
        {
            Debug.Log($"Item {completedRequest.itemID} not found in inventory");
            // Handle case where user doesn't have the item
            OfferItemPurchase(itemId);
        }
    });
}

// Alternative implementation using a coroutine
private IEnumerator CheckItemCoroutine(string itemId)
{
    GetInventoryItemRequest request = SpatialBridge.inventoryService.GetItem(itemId);
    
    // Wait until the operation is complete
    yield return request;
    
    if (request.succeeded)
    {
        Debug.Log($"Successfully found item {request.itemID} in inventory");
        
        // Now we can access the item through the items dictionary
        if (SpatialBridge.inventoryService.items.TryGetValue(itemId, out IInventoryItem item))
        {
            ProcessItem(item);
        }
    }
    else
    {
        Debug.Log($"Item {request.itemID} not found in inventory");
        // Handle case where user doesn't have the item
        OfferItemPurchase(itemId);
    }
}

private void ProcessItem(IInventoryItem item)
{
    // Example method to process an inventory item once found
    Debug.Log($"Processing item: {item.itemID} (Amount: {item.amount})");
    
    // Example: Check if it's a consumable
    if (item.isConsumable)
    {
        Debug.Log("This is a consumable item");
        
        // Check if it's on cooldown
        if (item.isOnCooldown)
        {
            Debug.Log($"Item is on cooldown. Remaining: {item.consumableCooldownRemaining} seconds");
        }
        else
        {
            Debug.Log("Item is ready to use");
        }
    }
}

private void OfferItemPurchase(string itemId)
{
    // Example method to handle case where user doesn't have the required item
    Debug.Log($"Offering to acquire item: {itemId}");
    // Could open a shop UI or provide information about how to get the item
}
```

## Best Practices
- First check if the item is already in the `SpatialBridge.inventoryService.items` dictionary before making a request
- Always check the `succeeded` property to verify if the item was found
- Be prepared to handle the case where an item is not found in the inventory
- Use this request to refresh inventory data when necessary, especially after extended play sessions
- For frequently accessed items, consider caching the result to reduce unnecessary requests
- When checking for multiple items, consider batching logic to avoid excessive requests

## Common Use Cases
- Verifying if a user has a required item before allowing access to certain features
- Checking for quest items or collectibles as part of progression tracking
- Validating ownership of special items before enabling their use
- Implementing "equipment check" mechanics for game features
- Refreshing inventory displays after potential background changes
- Building dynamic UIs that adapt based on items the user owns
- Verifying item ownership before attempting to use or modify an item

## Completed: March 9, 2025