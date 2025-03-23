## Overview
The DeleteInventoryItemRequest class represents the result of a request to delete an item from the user's inventory. It inherits from SpatialAsyncOperation and provides information about whether the item deletion operation succeeded.

## Properties
- **itemID**: The ID of the item to delete
- **succeeded**: True if the item was deleted successfully

## Inherited Members
- **InvokeCompletionEvent()**: Invokes the completion event
- **completed**: Event that is invoked when the operation is completed
- **isDone**: Returns true if the operation is done
- **keepWaiting**: Returns true if the operation is not done

## Usage Example
```csharp
public void RemoveItemFromInventory(string itemId)
{
    // Check if the item exists in the inventory before attempting to delete
    if (SpatialBridge.inventoryService.items.ContainsKey(itemId))
    {
        // Show confirmation dialog before deletion (optional)
        if (ConfirmItemDeletion())
        {
            // Delete the item from the user's inventory
            DeleteInventoryItemRequest request = SpatialBridge.inventoryService.DeleteItem(itemId);
            
            // Option 1: Use SetCompletedEvent extension method for a callback
            request.SetCompletedEvent((completedRequest) => {
                if (completedRequest.succeeded)
                {
                    Debug.Log($"Successfully deleted item {completedRequest.itemID} from inventory");
                    // Update UI to reflect the item removal
                    UpdateInventoryUI();
                    // Notify the user
                    SpatialBridge.coreGUIService.DisplayToastMessage("Item removed from inventory");
                }
                else
                {
                    Debug.LogWarning($"Failed to delete item {completedRequest.itemID} from inventory");
                    // Handle failure case
                    SpatialBridge.coreGUIService.DisplayToastMessage("Failed to remove item");
                }
            });
        }
    }
    else
    {
        Debug.LogWarning($"Cannot delete item {itemId} - not found in inventory");
        SpatialBridge.coreGUIService.DisplayToastMessage("Item not found in inventory");
    }
}

// Alternative implementation using a coroutine
private IEnumerator DeleteItemCoroutine(string itemId)
{
    DeleteInventoryItemRequest request = SpatialBridge.inventoryService.DeleteItem(itemId);
    
    // Wait until the operation is complete
    yield return request;
    
    if (request.succeeded)
    {
        Debug.Log($"Successfully deleted item {request.itemID} from inventory");
        UpdateInventoryUI();
        SpatialBridge.coreGUIService.DisplayToastMessage("Item removed from inventory");
    }
    else
    {
        Debug.LogWarning($"Failed to delete item {request.itemID} from inventory");
        SpatialBridge.coreGUIService.DisplayToastMessage("Failed to remove item");
    }
}

private bool ConfirmItemDeletion()
{
    // Example method that would show a confirmation dialog to the user
    // In a real implementation, this might open a UI and wait for user input
    return true; // For this example, always return true
}

private void UpdateInventoryUI()
{
    // Example method to refresh the inventory UI after changes
    Debug.Log("Updating inventory UI after item deletion");
}
```

## Best Practices
- Always check if the item exists in the inventory before attempting to delete it
- Consider implementing a confirmation step before deleting valuable or rare items
- Check the `succeeded` property to verify the deletion was successful
- Provide clear feedback to the user about the result of the delete operation
- Update any UI elements that display inventory contents after deletion
- Remember that this operation will only work for items that are in the same world as the current space
- For important items, consider implementing a "soft delete" mechanism (like marking as inactive) before permanent deletion

## Common Use Cases
- Removing consumable items after they've been used up
- Clearing inventory space when reaching capacity limits
- Implementing item trading or gifting systems
- Removing temporary or time-limited items after expiration
- Managing quest items that are no longer needed
- Implementing "sell" or "discard" functionality in inventory management
- Removing deprecated or replaced items during game updates

## Completed: March 9, 2025