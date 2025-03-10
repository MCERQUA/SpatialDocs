# SpawnSpaceObjectRequest

Category: Space Content Service

Class

Represents the result of a request to spawn a space object. This is a base class that is inherited by more specific spawn request types like SpawnAvatarRequest, SpawnNetworkObjectRequest, and SpawnPrefabObjectRequest.

## Properties

| Property | Description |
| --- | --- |
| spaceObject | The space object that was created. Will be null if the request failed. |
| succeeded | Whether the spawn operation was successful. |

## Inherited Members

| Member | Description |
| --- | --- |
| InvokeCompletionEvent() | Invokes the completion event. |
| completed | Event that is invoked when the operation is completed. |
| isDone | Returns true if the operation is done. |
| keepWaiting | Returns true if the operation is not done. |

## Extension Methods

| Method | Description |
| --- | --- |
| SetCompletedEvent<T>(T, Action<T>) | Sets the completion event, same as setting the event using the completed property, but returns the operation itself for method chaining. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceObjectManager : MonoBehaviour
{
    // Reference to a prefab with a SpatialNetworkObject component
    public GameObject networkObjectPrefab;
    
    // Dictionary to track space objects by type
    private Dictionary<SpaceObjectType, List<ISpaceObject>> spaceObjectsByType = 
        new Dictionary<SpaceObjectType, List<ISpaceObject>>();
    
    private void Awake()
    {
        // Initialize our tracking dictionary
        foreach (SpaceObjectType type in System.Enum.GetValues(typeof(SpaceObjectType)))
        {
            spaceObjectsByType[type] = new List<ISpaceObject>();
        }
    }
    
    private void OnEnable()
    {
        // Subscribe to space object events
        SpatialBridge.spaceContentService.onSpaceObjectCreated += HandleSpaceObjectCreated;
        SpatialBridge.spaceContentService.onSpaceObjectDestroyed += HandleSpaceObjectDestroyed;
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        SpatialBridge.spaceContentService.onSpaceObjectCreated -= HandleSpaceObjectCreated;
        SpatialBridge.spaceContentService.onSpaceObjectDestroyed -= HandleSpaceObjectDestroyed;
    }
    
    private void HandleSpaceObjectCreated(IReadOnlySpaceObject spaceObject)
    {
        if (!spaceObject.isDisposed)
        {
            // Add to our tracking
            spaceObjectsByType[spaceObject.objectType].Add(spaceObject as ISpaceObject);
        }
    }
    
    private void HandleSpaceObjectDestroyed(IReadOnlySpaceObject spaceObject)
    {
        // Remove from our tracking
        spaceObjectsByType[spaceObject.objectType].RemoveAll(obj => obj.objectID == spaceObject.objectID);
    }
    
    // Example: Spawn different types of space objects
    public void SpawnVariousObjects()
    {
        // Spawn a prefab object
        SpawnPrefabObject(transform.position, AssetType.SpatialPrefab, "prefab_asset_id");
        
        // Spawn a network object
        SpawnNetworkObject(transform.position + new Vector3(3, 0, 0));
        
        // Spawn an avatar
        SpawnAvatar(transform.position + new Vector3(0, 0, 3));
    }
    
    // Example: Spawn a prefab object
    private void SpawnPrefabObject(Vector3 position, AssetType assetType, string assetId)
    {
        SpatialBridge.spaceContentService.SpawnPrefabObject(
            assetType,
            assetId,
            position,
            Quaternion.identity,
            Vector3.one
        ).SetCompletedEvent(request => HandleSpawnResult(request, "prefab"));
    }
    
    // Example: Spawn a network object
    private void SpawnNetworkObject(Vector3 position)
    {
        SpatialBridge.spaceContentService.SpawnNetworkObject(
            networkObjectPrefab,
            position,
            Quaternion.identity
        ).SetCompletedEvent(request => HandleSpawnResult(request, "network"));
    }
    
    // Example: Spawn an avatar
    private void SpawnAvatar(Vector3 position)
    {
        SpatialBridge.spaceContentService.SpawnAvatar(
            position,
            Quaternion.identity
        ).SetCompletedEvent(request => HandleSpawnResult(request, "avatar"));
    }
    
    // Generic handler for any spawn request
    private void HandleSpawnResult(SpawnSpaceObjectRequest request, string type)
    {
        if (request.succeeded)
        {
            Debug.Log($"Successfully spawned {type} object with ID: {request.spaceObject.objectID}");
            
            // We can add common handling for space objects here
            // All spawn requests return a spaceObject property
            ISpaceObject spaceObject = request.spaceObject;
            
            // Example: Set up a variable on all spawned objects
            spaceObject.SetVariable(0, "SpawnedBy");
            spaceObject.SetVariable(1, SpatialBridge.actorService.localActor.displayName);
            spaceObject.SetVariable(2, System.DateTime.Now.ToString());
            
            // Example: Add ownership change tracking
            spaceObject.onOwnerChanged += (args) => {
                Debug.Log($"Space object {spaceObject.objectID} ownership changed:");
                Debug.Log($"  From: Actor #{args.oldOwnerActorNumber}");
                Debug.Log($"  To: Actor #{args.newOwnerActorNumber}");
            };
        }
        else
        {
            Debug.LogWarning($"Failed to spawn {type} object");
        }
    }
    
    // Example: Use coroutines to spawn and manage multiple objects
    public IEnumerator SpawnObjectsSequentially(int count, float spawnInterval)
    {
        Debug.Log($"Starting sequential spawn of {count} objects...");
        
        for (int i = 0; i < count; i++)
        {
            // Alternate between different object types
            switch (i % 3)
            {
                case 0:
                    SpawnPrefabObject(
                        transform.position + new Vector3(i * 2, 0, 0),
                        AssetType.SpatialPrefab,
                        "prefab_asset_id"
                    );
                    break;
                    
                case 1:
                    SpawnNetworkObject(
                        transform.position + new Vector3(i * 2, 0, 0)
                    );
                    break;
                    
                case 2:
                    SpawnAvatar(
                        transform.position + new Vector3(i * 2, 0, 0)
                    );
                    break;
            }
            
            // Wait between spawns
            yield return new WaitForSeconds(spawnInterval);
        }
        
        Debug.Log("Spawning sequence complete!");
        DisplayObjectCounts();
    }
    
    // Example: Count objects of each type
    public void DisplayObjectCounts()
    {
        Debug.Log("Space Object Counts:");
        
        foreach (var kvp in spaceObjectsByType)
        {
            // Only count non-disposed objects
            int activeCount = kvp.Value.Count(obj => !obj.isDisposed);
            Debug.Log($"  {kvp.Key}: {activeCount}");
        }
    }
    
    // Example: Destroy all spawned objects
    public void DestroyAllSpawnedObjects()
    {
        int count = 0;
        
        foreach (var objectList in spaceObjectsByType.Values)
        {
            // Create a copy to avoid modification during iteration
            List<ISpaceObject> objectsToDestroy = new List<ISpaceObject>(objectList);
            
            foreach (var spaceObject in objectsToDestroy)
            {
                if (!spaceObject.isDisposed)
                {
                    DestroySpaceObject(spaceObject);
                    count++;
                }
            }
        }
        
        Debug.Log($"Requested destruction of {count} space objects");
    }
    
    // Helper to destroy a space object
    private void DestroySpaceObject(ISpaceObject spaceObject)
    {
        // Check if we can destroy it (we own it or can take ownership)
        if (spaceObject.isMine)
        {
            // We own it, so destroy it directly
            SpatialBridge.spaceContentService.DestroySpaceObject(spaceObject)
                .SetCompletedEvent((request) => {
                    if (request.succeeded)
                    {
                        Debug.Log($"Destroyed space object #{spaceObject.objectID}");
                    }
                });
        }
        else if (spaceObject.canTakeOwnership)
        {
            // Take ownership first, then destroy
            SpatialBridge.spaceContentService.TakeOwnership(spaceObject)
                .SetCompletedEvent((request) => {
                    if (request.succeeded)
                    {
                        SpatialBridge.spaceContentService.DestroySpaceObject(spaceObject);
                    }
                });
        }
    }
}
```

## Best Practices

1. Always check the `succeeded` property before accessing the `spaceObject` property.
2. Use the specific spawn request classes (SpawnAvatarRequest, SpawnNetworkObjectRequest, SpawnPrefabObjectRequest) to access the type-specific properties.
3. Use `SetCompletedEvent` for clean callback handling of spawn operations.
4. Consider subscribing to the space object's events (like `onOwnerChanged`) immediately after successful spawning.
5. Remember that the local client owns all newly spawned space objects initially.
6. Use space object variables to store metadata that needs to be accessible by all clients.
7. Subscribe to `onSpaceObjectCreated` and `onSpaceObjectDestroyed` events to track space objects if you need a global view.

## Common Use Cases

1. Creating a unified system for managing different types of space objects.
2. Building a centralized object spawning framework in your Spatial application.
3. Implementing pooling or recycling systems for space objects.
4. Creating spawning patterns that mix different object types (like avatars and prefabs).
5. Developing management systems that track and categorize objects by type.
6. Building debugging or monitoring tools that log space object activity.
7. Creating cleanup systems that handle destroying all created objects when appropriate.

## Completed: March 9, 2025