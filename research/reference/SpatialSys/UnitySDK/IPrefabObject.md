# IPrefabObject

Category: Space Content Service

Interface

A prefab object interface that allows for read and write access to the object. Setting values on the object requires the local client to have ownership of the object.

## Methods

| Method | Description |
| --- | --- |
| SetAsset(AssetType, string) | Sets the asset used by the prefab object. Successfully changing the asset will cause the gameObjects representing this prefab in the scene to be re-instantiated for all clients. The IPrefabObject instances will remain the same. |

## Inherited Members

| Member | Description |
| --- | --- |
| assetID | ID of the Spatial asset currently used by the prefab object. |
| assetType | The type of Spatial asset currently used by the prefab object. |
| isDisposed | Returns true when the component or its parent space object has been destroyed. |
| spaceObject | The space object the component is attached to. |
| spaceObjectID | The id of the spaceObject the component is attached to. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;

public class PrefabObjectManager : MonoBehaviour
{
    // Reference to the prefab object we want to modify
    private IPrefabObject myPrefabObject;
    
    private void Start()
    {
        // Example: Get a prefab object reference from a space object spawned by the service
        var spawnRequest = SpatialBridge.spaceContentService.SpawnPrefabObject(
            AssetType.SpatialAvatar,  // Type of asset
            "avatar_asset_id",        // ID of the asset
            Vector3.zero,             // Position
            Quaternion.identity,      // Rotation
            Vector3.one               // Scale
        );
        
        spawnRequest.SetCompletedEvent((request) => {
            if (request.succeeded)
            {
                // Store the prefab object reference
                myPrefabObject = request.prefabObject;
                Debug.Log("Successfully spawned prefab object with asset ID: " + myPrefabObject.assetID);
            }
        });
    }
    
    public void ChangeAsset(string newAssetID)
    {
        // Check if we own the object before trying to change it
        if (myPrefabObject != null && myPrefabObject.spaceObject.isMine)
        {
            // Change the asset - this will update for all clients
            myPrefabObject.SetAsset(AssetType.SpatialAvatar, newAssetID);
            Debug.Log("Changed prefab asset to: " + newAssetID);
        }
        else
        {
            // Try to take ownership first
            if (myPrefabObject != null && myPrefabObject.spaceObject.canTakeOwnership)
            {
                SpatialBridge.spaceContentService.TakeOwnership(myPrefabObject.spaceObject)
                    .SetCompletedEvent((request) => {
                        if (request.succeeded)
                        {
                            // Now we can change the asset
                            myPrefabObject.SetAsset(AssetType.SpatialAvatar, newAssetID);
                            Debug.Log("Took ownership and changed prefab asset to: " + newAssetID);
                        }
                    });
            }
            else
            {
                Debug.LogWarning("Cannot change asset - don't have ownership and can't take it");
            }
        }
    }
}
```

## Best Practices

1. Always check if you have ownership of the space object before attempting to modify the prefab object.
2. Use `TakeOwnership` if you need to modify a prefab object that your client doesn't currently own.
3. Be aware that changing the asset will cause the visual representation to be re-instantiated for all clients.
4. Remember that changing the asset doesn't destroy the IPrefabObject instance itself; the same reference will remain valid.
5. Release ownership when you're done making changes to allow other clients to modify the object.

## Common Use Cases

1. Dynamically changing the appearance of objects in the space based on player interactions.
2. Creating customizable items that can change their visual representation.
3. Building systems that allow users to swap between different model variations.
4. Creating interactive objects that transform into different objects when triggered.
5. Implementing progression systems where objects evolve or change as players complete tasks.

## Completed: March 9, 2025