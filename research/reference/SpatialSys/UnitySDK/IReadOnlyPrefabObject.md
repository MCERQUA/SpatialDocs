# IReadOnlyPrefabObject

Category: Space Content Service

Interface

The data-model for a prefab object as it exists on the server. Prefab objects are a component of SpaceObjects.

## Properties

| Property | Description |
| --- | --- |
| assetID | ID of the Spatial asset currently used by the prefab object. |
| assetType | The type of Spatial asset currently used by the prefab object. |

## Inherited Members

| Member | Description |
| --- | --- |
| isDisposed | Returns true when the component or its parent space object has been destroyed. |
| spaceObject | The space object the component is attached to. |
| spaceObjectID | The id of the spaceObject the component is attached to. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections.Generic;

public class PrefabObjectMonitor : MonoBehaviour
{
    // Track all prefab objects in the space
    private List<IReadOnlyPrefabObject> activePrefabObjects = new List<IReadOnlyPrefabObject>();
    
    private void OnEnable()
    {
        // Subscribe to events to track prefab objects
        SpatialBridge.spaceContentService.onSpaceObjectCreated += HandleSpaceObjectCreated;
        SpatialBridge.spaceContentService.onSpaceObjectDestroyed += HandleSpaceObjectDestroyed;
        
        // Find existing prefab objects
        foreach (var spaceObject in SpatialBridge.spaceContentService.spaceObjects)
        {
            if (spaceObject.objectType == SpaceObjectType.PrefabObject)
            {
                var prefabObj = SpatialBridge.spaceContentService.GetPrefabObject(spaceObject.objectID);
                if (prefabObj != null)
                {
                    activePrefabObjects.Add(prefabObj);
                    Debug.Log($"Found existing prefab object. ID: {prefabObj.spaceObjectID}, Asset Type: {prefabObj.assetType}, Asset ID: {prefabObj.assetID}");
                }
            }
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        SpatialBridge.spaceContentService.onSpaceObjectCreated -= HandleSpaceObjectCreated;
        SpatialBridge.spaceContentService.onSpaceObjectDestroyed -= HandleSpaceObjectDestroyed;
    }
    
    private void HandleSpaceObjectCreated(IReadOnlySpaceObject spaceObject)
    {
        if (spaceObject.objectType == SpaceObjectType.PrefabObject)
        {
            var prefabObj = SpatialBridge.spaceContentService.GetPrefabObject(spaceObject.objectID);
            if (prefabObj != null)
            {
                activePrefabObjects.Add(prefabObj);
                Debug.Log($"New prefab object created. ID: {prefabObj.spaceObjectID}, Asset Type: {prefabObj.assetType}, Asset ID: {prefabObj.assetID}");
            }
        }
    }
    
    private void HandleSpaceObjectDestroyed(IReadOnlySpaceObject spaceObject)
    {
        // Remove from our list if it was a prefab object
        activePrefabObjects.RemoveAll(prefab => prefab.spaceObjectID == spaceObject.objectID);
    }
    
    // Example method to find prefab objects by asset type
    public List<IReadOnlyPrefabObject> FindPrefabObjectsByAssetType(AssetType type)
    {
        List<IReadOnlyPrefabObject> result = new List<IReadOnlyPrefabObject>();
        
        foreach (var prefab in activePrefabObjects)
        {
            if (prefab.assetType == type)
            {
                result.Add(prefab);
            }
        }
        
        return result;
    }
    
    // Example method to display information about all active prefab objects
    public void DisplayPrefabObjectsInfo()
    {
        Debug.Log($"Total active prefab objects: {activePrefabObjects.Count}");
        
        foreach (var prefab in activePrefabObjects)
        {
            if (!prefab.isDisposed)
            {
                Debug.Log($"Prefab Object ID: {prefab.spaceObjectID}");
                Debug.Log($"  Asset Type: {prefab.assetType}");
                Debug.Log($"  Asset ID: {prefab.assetID}");
                Debug.Log($"  Position: {prefab.spaceObject.position}");
                Debug.Log($"  Owner: Actor #{prefab.spaceObject.ownerActorNumber}");
            }
        }
    }
}
```

## Best Practices

1. Use IReadOnlyPrefabObject when you only need to read properties of a prefab object and don't need to modify it.
2. Always check isDisposed before accessing properties to avoid errors with destroyed objects.
3. Store references to prefab objects when tracking them, rather than repeatedly calling GetPrefabObject.
4. When tracking multiple prefab objects, consider organizing them by assetType for more efficient access.
5. Remember that IReadOnlyPrefabObject provides access to the underlying IReadOnlySpaceObject through the spaceObject property.

## Common Use Cases

1. Tracking and monitoring prefab objects in the space without modifying them.
2. Creating inventories or catalogs of available objects based on their asset types.
3. Building UI systems that display information about objects in the space.
4. Implementing trigger systems that react to specific prefab objects.
5. Creating distance-based interaction systems that can locate nearby prefab objects.

## Completed: March 9, 2025