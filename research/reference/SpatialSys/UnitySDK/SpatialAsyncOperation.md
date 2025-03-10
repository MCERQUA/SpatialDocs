## Overview
SpatialAsyncOperation is the base class for all Spatial's asynchronous operations. It serves as the foundation for handling operations that may not complete immediately, such as network requests, data loading, or other time-consuming tasks. This class is yieldable and can be used in coroutines to await completion of operations.

## Properties

| Property | Description |
|----------|-------------|
| isDone | Returns true if the operation is done. |
| keepWaiting | Returns true if the operation is not done. Used for coroutine compatibility. |

## Methods

| Method | Description |
|--------|-------------|
| InvokeCompletionEvent() | Invokes the completion event. This is typically called internally when an operation completes. |

## Events

| Event | Description |
|-------|-------------|
| completed | Event that is invoked when the operation is completed. Subscribe to this event to be notified when the operation finishes. |

## Usage Example

### Using with Callbacks

```csharp
public void SavePlayerHighScore(int score)
{
    // Start the async operation
    SpatialBridge.userWorldDataStoreService.SetVariable("playerHighScore", score).completed += (op) =>
    {
        // This code executes when the operation completes
        Debug.Log("High score saved successfully!");
    };
}
```

### Using with SetCompletedEvent Extension Method

```csharp
public void SavePlayerHighScore(int score)
{
    // Start the async operation and set up completion callback in one line
    SpatialBridge.userWorldDataStoreService.SetVariable("playerHighScore", score).SetCompletedEvent((op) =>
    {
        // This code executes when the operation completes
        Debug.Log("High score saved successfully!");
    });
}
```

### Using with Coroutines

```csharp
public IEnumerator SavePlayerDataCoroutine(int score, string playerName)
{
    // Start first async operation
    var highScoreRequest = SpatialBridge.userWorldDataStoreService.SetVariable("playerHighScore", score);
    
    // Wait for it to complete
    yield return highScoreRequest;
    
    // Start another async operation
    var nameRequest = SpatialBridge.userWorldDataStoreService.SetVariable("playerName", playerName);
    
    // Wait for it to complete
    yield return nameRequest;
    
    // Both operations have completed
    Debug.Log("All player data saved successfully!");
}

// Call the coroutine from somewhere
private void SaveAllPlayerData()
{
    StartCoroutine(SavePlayerDataCoroutine(1000, "Champion"));
}
```

## Best Practices

- Always check if operations have completed before accessing their results to avoid null reference exceptions.
- When using callbacks, remember they might execute on a different frame than when the operation was started.
- Unsubscribe from the completed event if you store the SpatialAsyncOperation in a longer-lived object to avoid memory leaks.
- Use the `SetCompletedEvent` extension method for cleaner, more readable code when setting up callbacks.
- For related operations that depend on one another, consider using coroutines for a more sequential flow.
- Handle operation failures appropriately in your callbacks or coroutine code.

## Common Use Cases

- Loading profile pictures for actors
- Setting or retrieving data from the data store
- Purchasing or using inventory items
- Spawning network objects in the space
- Transferring ownership of space objects
- Making API requests to external services
- Loading resources or assets at runtime

## Derived Types

SpatialAsyncOperation serves as the base class for many specific async operation types in Spatial:

- ActorProfilePictureRequest - For retrieving actor profile pictures
- AdRequest - For requesting ads
- AddInventoryItemRequest - For adding items to a user's inventory
- AwardWorldCurrencyRequest - For awarding currency to users
- DataStoreOperationRequest - For data store operations
- DeleteInventoryItemRequest - For deleting inventory items
- DestroySpaceObjectRequest - For destroying space objects
- EquipAttachmentRequest - For equipping avatar attachments
- GetInventoryItemRequest - For retrieving inventory items
- PurchaseItemRequest - For handling item purchases
- SpaceObjectOwnershipTransferRequest - For transferring object ownership
- SpawnSpaceObjectRequest - For spawning space objects
- UseInventoryItemRequest - For using inventory items

## Completed: March 10, 2025
