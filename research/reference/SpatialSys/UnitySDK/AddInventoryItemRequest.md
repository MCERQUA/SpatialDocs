## Overview
The AddInventoryItemRequest class represents the result of a request to add an item to the user's inventory. It inherits from SpatialAsyncOperation and provides information about whether the item was added successfully, as well as details about the item added.

## Properties
- **amount**: The amount of the item to add
- **itemID**: The ID of the item to add
- **succeeded**: True if the item was added successfully

## Inherited Members
- **InvokeCompletionEvent()**: Invokes the completion event
- **completed**: Event that is invoked when the operation is completed
- **isDone**: Returns true if the operation is done
- **keepWaiting**: Returns true if the operation is not done

## Usage Example
```csharp
public void AddItemToInventory(string itemId, int amount)
{
    // Add an item to the user's inventory
    AddInventoryItemRequest request = SpatialBridge.inventoryService.AddItem(itemId, (ulong)amount);
    
    // Option 1: Use SetCompletedEvent extension method for a callback
    request.SetCompletedEvent((completedRequest) => {
        if (completedRequest.succeeded)
        {
            Debug.Log($"Successfully added {completedRequest.amount} of item {completedRequest.itemID} to inventory");
            // Update UI or game state to reflect the new item
            UpdateInventoryUI();
        }
        else
        {
            Debug.LogWarning($"Failed to add item {completedRequest.itemID} to inventory");
            // Handle failure - perhaps show an error message to the user
            ShowErrorMessage("Failed to add item to inventory");
        }
    });
    
    // Option 2: Use it in a coroutine
    StartCoroutine(AddItemCoroutine(itemId, amount));
}

private IEnumerator AddItemCoroutine(string itemId, int amount)
{
    AddInventoryItemRequest request = SpatialBridge.inventoryService.AddItem(itemId, (ulong)amount);
    
    // Wait until the operation is complete
    yield return request;
    
    if (request.succeeded)
    {
        Debug.Log($"Successfully added {request.amount} of item {request.itemID} to inventory");
        // Update UI or game state to reflect the new item
        UpdateInventoryUI();
    }
    else
    {
        Debug.LogWarning($"Failed to add item {request.itemID} to inventory");
        // Handle failure
        ShowErrorMessage("Failed to add item to inventory");
    }
}

private void UpdateInventoryUI()
{
    // Example method to refresh inventory UI
}

private void ShowErrorMessage(string message)
{
    // Example method to display error message to user
    SpatialBridge.coreGUIService.DisplayToastMessage(message);
}
```

## Best Practices
- Always check the `succeeded` property before assuming the item was added successfully
- Use the `SetCompletedEvent` extension method for clean, callback-based code
- For better user experience, provide visual or audio feedback when items are added to inventory
- Consider showing a progress indicator while waiting for the request to complete
- Handle failures gracefully with informative error messages to the user
- When adding multiple items, consider batching requests or showing a summary of results

## Common Use Cases
- Rewarding players for completing quests or challenges
- Adding collected items from the environment to the player's inventory
- Providing starter items to new players
- Implementing pickup mechanics for collectible items
- Giving bonus items during special events or promotions
- Restoring previously owned items after certain game events

## Completed: March 9, 2025