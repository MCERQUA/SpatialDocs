# DestroySpaceObjectRequest

Category: Space Content Service Related

Interface/Class/Enum: Class

A class representing the result of a request to destroy a space object. This inherits from SpatialAsyncOperation and provides information about whether the destroy operation succeeded.

## Properties/Fields

| Property | Description |
| --- | --- |
| spaceObjectID | The ID of the space object to be destroyed. |
| succeeded | Whether the request to destroy the space object was successful. |

## Inherited Members

| Member | Description |
| --- | --- |
| InvokeCompletionEvent() | Invokes the completion event. |
| completed | Event that is invoked when the operation is completed. |
| isDone | Returns true if the operation is done. |
| keepWaiting | Returns true if the operation is not done. |

## Usage Examples

```csharp
// Example: Object Cleanup Manager
public class ObjectCleanupManager : MonoBehaviour
{
    private ISpaceContentService contentService;
    private List<int> destroyRequestIds = new List<int>();
    private Dictionary<int, DateTime> destroyAttemptTimes = new Dictionary<int, DateTime>();
    private float retryInterval = 5.0f; // Retry failed destroys after 5 seconds
    
    void Start()
    {
        contentService = SpatialBridge.spaceContentService;
        StartCoroutine(ProcessRetries());
    }
    
    public void CleanupObject(int objectId)
    {
        if (!CanDestroyObject(objectId))
        {
            Debug.LogWarning($"Cannot destroy object {objectId}: Invalid object, no permission, or already being destroyed");
            return;
        }
        
        DestroyObjectWithTracking(objectId);
    }
    
    public void CleanupObjectsOfType(SpaceObjectType objectType)
    {
        var objectsToDestroy = contentService.allObjects.Values
            .Where(obj => obj.objectType == objectType && CanDestroyObject(obj.objectID))
            .Select(obj => obj.objectID)
            .ToList();
            
        Debug.Log($"Attempting to clean up {objectsToDestroy.Count} objects of type {objectType}");
        
        foreach (var objectId in objectsToDestroy)
        {
            DestroyObjectWithTracking(objectId);
        }
    }
    
    private bool CanDestroyObject(int objectId)
    {
        // Check if object ID is valid
        if (!contentService.allObjects.ContainsKey(objectId))
            return false;
            
        // Check if we're already trying to destroy this object
        if (destroyRequestIds.Contains(objectId))
            return false;
            
        var obj = contentService.allObjects[objectId];
        
        // Check if we have permission to destroy it
        return obj.isMine || obj.hasControl;
    }
    
    private void DestroyObjectWithTracking(int objectId)
    {
        // Add to tracking list
        destroyRequestIds.Add(objectId);
        destroyAttemptTimes[objectId] = DateTime.UtcNow;
        
        // Attempt to destroy the object
        var request = contentService.DestroySpaceObject(objectId);
        
        // Register for completion callback
        request.completed += (asyncOp) => {
            var destroyRequest = (DestroySpaceObjectRequest)asyncOp;
            HandleDestroyCompleted(destroyRequest);
        };
    }
    
    private void HandleDestroyCompleted(DestroySpaceObjectRequest request)
    {
        int objectId = request.spaceObjectID;
        
        if (request.succeeded)
        {
            // Successful destroy
            Debug.Log($"Successfully destroyed object {objectId}");
            destroyRequestIds.Remove(objectId);
            destroyAttemptTimes.Remove(objectId);
        }
        else
        {
            // Failed destroy - keep in list for retry
            Debug.LogWarning($"Failed to destroy object {objectId}. Will retry later.");
            // The object stays in the list for retry
        }
    }
    
    // Process retry attempts for failed destroy operations
    private IEnumerator ProcessRetries()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            
            var now = DateTime.UtcNow;
            var objectsToRetry = new List<int>();
            
            // Find objects that need retry
            foreach (var kvp in destroyAttemptTimes)
            {
                int objectId = kvp.Key;
                DateTime lastAttempt = kvp.Value;
                
                if ((now - lastAttempt).TotalSeconds >= retryInterval)
                {
                    objectsToRetry.Add(objectId);
                }
            }
            
            // Process retries
            foreach (var objectId in objectsToRetry)
            {
                if (CanDestroyObject(objectId))
                {
                    Debug.Log($"Retrying destroy for object {objectId}");
                    
                    // Update attempt time
                    destroyAttemptTimes[objectId] = now;
                    
                    // Retry destroy
                    var request = contentService.DestroySpaceObject(objectId);
                    request.completed += (asyncOp) => {
                        var destroyRequest = (DestroySpaceObjectRequest)asyncOp;
                        HandleDestroyCompleted(destroyRequest);
                    };
                }
                else
                {
                    // Object no longer exists or we lost permission, remove from tracking
                    destroyRequestIds.Remove(objectId);
                    destroyAttemptTimes.Remove(objectId);
                }
            }
        }
    }
    
    // Extension method approach using the SetCompletedEvent extension
    public void CleanupObjectWithExtension(int objectId)
    {
        if (!CanDestroyObject(objectId))
            return;
            
        // Add to tracking list
        destroyRequestIds.Add(objectId);
        destroyAttemptTimes[objectId] = DateTime.UtcNow;
        
        // Using the SetCompletedEvent extension method
        contentService.DestroySpaceObject(objectId).SetCompletedEvent((request) => {
            if (request.succeeded)
            {
                Debug.Log($"Successfully destroyed object {request.spaceObjectID}");
                destroyRequestIds.Remove(request.spaceObjectID);
                destroyAttemptTimes.Remove(request.spaceObjectID);
            }
            else
            {
                Debug.LogWarning($"Failed to destroy object {request.spaceObjectID}. Will retry later.");
                // The object stays in the list for retry
            }
        });
    }
}
```

## Best Practices

1. Always check the `succeeded` property to verify if the destroy operation was successful
2. Implement retry mechanisms for failed destroy operations
3. Use the `SetCompletedEvent` extension method for more readable code when handling completion callbacks
4. Only attempt to destroy objects that you own or have control over
5. Handle failures gracefully, as destroy operations may fail due to network issues or permission changes
6. Keep track of destroy requests to avoid duplicate attempts
7. Consider implementing a cleanup system that handles destroy operations in batches

## Common Use Cases

1. Cleaning up objects when they are no longer needed
2. Removing temporary objects after a time limit
3. Implementing despawn mechanics for game objects
4. Managing object lifetimes in multiplayer scenarios
5. Creating systems that limit the number of objects by destroying oldest ones first
6. Implementing "delete" functionality in user interfaces
7. Building cleanup utilities that remove specific types of objects from the space

## Completed: March 9, 2025
