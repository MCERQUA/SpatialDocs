# SpawnPrefabObjectRequest

Category: Space Content Service

Class

Represents the result of a request to spawn a prefab object in the space. This class inherits from SpawnSpaceObjectRequest and provides access to the spawned prefab object.

## Properties

| Property | Description |
| --- | --- |
| prefabObject | The prefab object that was spawned as a result of the request. Will be null if the request failed. |

## Inherited Members

| Member | Description |
| --- | --- |
| spaceObject | The space object that was created. Will be null if the request failed. |
| succeeded | Whether the spawn operation was successful. |
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

public class PrefabObjectSpawner : MonoBehaviour
{
    // Example: Spawn a prefab object at a specific position
    public void SpawnPrefabAtPosition(Vector3 position, Quaternion rotation)
    {
        Debug.Log($"Spawning prefab object at position {position}");
        
        // Specify the asset type and ID when spawning
        SpatialBridge.spaceContentService.SpawnPrefabObject(
            AssetType.SpatialPrefab,  // Asset type 
            "prefab_asset_id",        // Asset ID from your Spatial project
            position,                 // Position
            rotation,                 // Rotation
            Vector3.one               // Scale (1,1,1)
        ).SetCompletedEvent(HandlePrefabSpawnResult);
    }
    
    // Handler for prefab spawn completion
    private void HandlePrefabSpawnResult(SpawnPrefabObjectRequest request)
    {
        if (request.succeeded)
        {
            Debug.Log($"Prefab object successfully spawned with space object ID: {request.spaceObject.objectID}");
            
            // Access the spawned prefab object
            IPrefabObject prefabObj = request.prefabObject;
            
            Debug.Log($"Prefab asset type: {prefabObj.assetType}");
            Debug.Log($"Prefab asset ID: {prefabObj.assetID}");
            
            // Example: Store data in space object variables
            request.spaceObject.SetVariable(0, "SpawnTime");
            request.spaceObject.SetVariable(1, System.DateTime.Now.ToString());
            
            // Example: Set up ownership change listeners
            request.spaceObject.onOwnerChanged += (args) => {
                Debug.Log($"Prefab ownership changed from actor #{args.oldOwnerActorNumber} to actor #{args.newOwnerActorNumber}");
            };
        }
        else
        {
            Debug.LogWarning("Failed to spawn prefab object");
        }
    }
    
    // Example: Spawn multiple prefabs with different asset IDs
    public void SpawnMultiplePrefabs(string[] assetIds, float spacing = 2.0f)
    {
        Vector3 basePosition = transform.position;
        
        for (int i = 0; i < assetIds.Length; i++)
        {
            Vector3 position = basePosition + new Vector3(i * spacing, 0, 0);
            string assetId = assetIds[i];
            
            SpatialBridge.spaceContentService.SpawnPrefabObject(
                AssetType.SpatialPrefab,
                assetId,
                position,
                Quaternion.identity,
                Vector3.one
            ).SetCompletedEvent((request) => {
                if (request.succeeded)
                {
                    Debug.Log($"Spawned prefab with asset ID: {request.prefabObject.assetID}");
                    
                    // Store the index in the space object variables
                    request.spaceObject.SetVariable(0, "Index");
                    request.spaceObject.SetVariable(1, i);
                }
            });
        }
    }
    
    // Example: Spawn prefabs with different asset types
    public void SpawnPrefabsOfDifferentTypes()
    {
        // Spawn a Spatial Prefab 
        SpatialBridge.spaceContentService.SpawnPrefabObject(
            AssetType.SpatialPrefab,
            "prefab_asset_id", 
            transform.position,
            Quaternion.identity,
            Vector3.one
        ).SetCompletedEvent((request) => {
            if (request.succeeded)
            {
                Debug.Log($"Spawned SpatialPrefab at {request.spaceObject.position}");
            }
        });
        
        // Spawn a Spatial Avatar (at a different position)
        SpatialBridge.spaceContentService.SpawnPrefabObject(
            AssetType.SpatialAvatar,
            "avatar_asset_id", 
            transform.position + new Vector3(3, 0, 0),
            Quaternion.identity,
            Vector3.one
        ).SetCompletedEvent((request) => {
            if (request.succeeded)
            {
                Debug.Log($"Spawned SpatialAvatar at {request.spaceObject.position}");
            }
        });
        
        // Spawn a Spatial Vehicle (at a different position)
        SpatialBridge.spaceContentService.SpawnPrefabObject(
            AssetType.SpatialVehicle,
            "vehicle_asset_id", 
            transform.position + new Vector3(0, 0, 3),
            Quaternion.identity,
            Vector3.one
        ).SetCompletedEvent((request) => {
            if (request.succeeded)
            {
                Debug.Log($"Spawned SpatialVehicle at {request.spaceObject.position}");
            }
        });
    }
    
    // Example: Using coroutines with prefab spawning
    public IEnumerator SpawnAndModifyPrefab(Vector3 position, string assetId)
    {
        Debug.Log("Starting prefab spawn process...");
        
        // Request to spawn the prefab
        var request = SpatialBridge.spaceContentService.SpawnPrefabObject(
            AssetType.SpatialPrefab,
            assetId,
            position,
            Quaternion.identity,
            Vector3.one
        );
        
        // Wait for the operation to complete
        yield return request;
        
        if (request.succeeded)
        {
            Debug.Log("Prefab spawn successful!");
            
            // Get reference to the prefab object and space object
            IPrefabObject prefabObj = request.prefabObject;
            ISpaceObject spaceObj = request.spaceObject;
            
            // Example: Wait for a condition before continuing
            yield return new WaitForSeconds(2.0f);
            
            // Example: Change the asset ID after a delay
            if (spaceObj.isMine) // Make sure we still own it
            {
                Debug.Log($"Changing prefab asset from {prefabObj.assetID} to a different asset");
                
                // Change the prefab's asset
                prefabObj.SetAsset(AssetType.SpatialPrefab, "different_asset_id");
                
                Debug.Log("Asset changed!");
            }
            else
            {
                Debug.Log("Cannot change asset - no longer own the prefab");
                
                // Try to take ownership
                var takeOwnershipRequest = SpatialBridge.spaceContentService.TakeOwnership(spaceObj);
                yield return takeOwnershipRequest;
                
                if (takeOwnershipRequest.succeeded)
                {
                    Debug.Log("Took ownership, now changing asset");
                    prefabObj.SetAsset(AssetType.SpatialPrefab, "different_asset_id");
                }
            }
        }
        else
        {
            Debug.LogWarning("Prefab spawn failed!");
        }
    }
    
    // Example: Create a circle of prefabs that change over time
    public IEnumerator CreatePrefabCircle(int count, float radius, string[] assetIds)
    {
        List<IPrefabObject> prefabObjects = new List<IPrefabObject>();
        
        // Spawn the prefabs in a circle
        for (int i = 0; i < count; i++)
        {
            float angle = i * (360f / count);
            float radian = angle * Mathf.Deg2Rad;
            Vector3 position = transform.position + new Vector3(
                Mathf.Cos(radian) * radius,
                0,
                Mathf.Sin(radian) * radius
            );
            
            // Get initial asset ID (cycling through the provided array)
            string assetId = assetIds[i % assetIds.Length];
            
            var request = SpatialBridge.spaceContentService.SpawnPrefabObject(
                AssetType.SpatialPrefab,
                assetId,
                position,
                Quaternion.LookAt(position, transform.position),
                Vector3.one
            );
            
            yield return request;
            
            if (request.succeeded)
            {
                prefabObjects.Add(request.prefabObject);
            }
        }
        
        Debug.Log($"Created circle of {prefabObjects.Count} prefab objects");
        
        // Animate the prefabs by cycling through different assets
        for (int cycle = 0; cycle < 5; cycle++)
        {
            yield return new WaitForSeconds(2.0f);
            
            for (int i = 0; i < prefabObjects.Count; i++)
            {
                // Cycle to the next asset ID for each prefab
                IPrefabObject prefab = prefabObjects[i];
                
                if (!prefab.isDisposed && prefab.spaceObject.isMine)
                {
                    int nextAssetIndex = (cycle + i) % assetIds.Length;
                    string nextAssetId = assetIds[nextAssetIndex];
                    
                    prefab.SetAsset(AssetType.SpatialPrefab, nextAssetId);
                }
            }
            
            Debug.Log($"Changed assets - cycle {cycle + 1}/5");
        }
    }
}
```

## Best Practices

1. Always check the `succeeded` property before accessing the `prefabObject` or `spaceObject` properties.
2. Use `SetCompletedEvent` for clean callback handling of prefab spawn operations.
3. Verify that you have ownership (`spaceObject.isMine`) before attempting to modify a prefab object (e.g., calling `SetAsset`).
4. Use appropriate `AssetType` values that match the assets you're spawning (e.g., `AssetType.SpatialPrefab`, `AssetType.SpatialAvatar`).
5. Consider using space object variables to store metadata about your prefab objects.
6. Be careful when storing references to prefab objects - always check `isDisposed` before accessing them.
7. Remember that changing a prefab's asset with `SetAsset` causes the visual representation to be re-instantiated for all clients.

## Common Use Cases

1. Creating dynamic environments with assets that can be changed at runtime.
2. Spawning interactive objects in multiplayer spaces.
3. Building inventory or collection systems where items can be placed in the world.
4. Implementing puzzle or game mechanics that involve different object variations.
5. Creating customizable spaces where users can place and modify objects.
6. Building showcase systems that display different asset types.
7. Implementing progression systems where objects evolve or transform as players complete objectives.

## Completed: March 9, 2025