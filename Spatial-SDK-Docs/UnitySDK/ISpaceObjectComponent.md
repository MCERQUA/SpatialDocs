# ISpaceObjectComponent

Category: Space Content Service

Interface

A component of a space object. This interface provides read and write access to the component and its associated space object, extending the functionality offered by IReadOnlySpaceObjectComponent.

## Properties

| Property | Description |
| --- | --- |
| spaceObject | The space object the component is attached to. Provides read and write access to the space object. |

## Inherited Members

| Member | Description |
| --- | --- |
| isDisposed | Returns true when the component or its parent space object has been destroyed. |
| spaceObjectID | The id of the spaceObject the component is attached to. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceObjectComponentManager : MonoBehaviour
{
    // Store references to space object components we need to modify
    private List<ISpaceObjectComponent> managedComponents = new List<ISpaceObjectComponent>();
    
    private void OnEnable()
    {
        // Subscribe to space object events
        SpatialBridge.spaceContentService.onSpaceObjectCreated += HandleSpaceObjectCreated;
        SpatialBridge.spaceContentService.onSpaceObjectDestroyed += HandleSpaceObjectDestroyed;
        
        // Scan for existing prefab objects
        foreach (var spaceObject in SpatialBridge.spaceContentService.spaceObjects)
        {
            if (spaceObject.objectType == SpaceObjectType.PrefabObject && !spaceObject.isDisposed)
            {
                var prefabObj = SpatialBridge.spaceContentService.GetPrefabObject(spaceObject.objectID);
                if (prefabObj != null)
                {
                    // Add to our managed components if it's writable
                    AddComponentIfWritable(prefabObj);
                }
            }
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        SpatialBridge.spaceContentService.onSpaceObjectCreated -= HandleSpaceObjectCreated;
        SpatialBridge.spaceContentService.onSpaceObjectDestroyed -= HandleSpaceObjectDestroyed;
        
        // Clear references
        managedComponents.Clear();
    }
    
    private void HandleSpaceObjectCreated(IReadOnlySpaceObject spaceObject)
    {
        if (spaceObject.objectType == SpaceObjectType.PrefabObject && !spaceObject.isDisposed)
        {
            var prefabObj = SpatialBridge.spaceContentService.GetPrefabObject(spaceObject.objectID);
            if (prefabObj != null)
            {
                AddComponentIfWritable(prefabObj);
            }
        }
    }
    
    private void HandleSpaceObjectDestroyed(IReadOnlySpaceObject spaceObject)
    {
        // Remove any components associated with the destroyed space object
        managedComponents.RemoveAll(c => c.spaceObjectID == spaceObject.objectID);
    }
    
    private void AddComponentIfWritable(IReadOnlySpaceObjectComponent component)
    {
        // Check if this is a writable component (implements ISpaceObjectComponent)
        if (component is ISpaceObjectComponent writableComponent)
        {
            managedComponents.Add(writableComponent);
            Debug.Log($"Added writable component for space object #{writableComponent.spaceObjectID}");
        }
    }
    
    // Example: Take ownership of multiple space objects via their components
    public void TakeOwnershipOfAll()
    {
        StartCoroutine(TakeOwnershipSequence());
    }
    
    private IEnumerator TakeOwnershipSequence()
    {
        foreach (var component in managedComponents)
        {
            if (!component.isDisposed && !component.spaceObject.isMine && component.spaceObject.canTakeOwnership)
            {
                Debug.Log($"Attempting to take ownership of space object #{component.spaceObjectID}");
                
                var request = SpatialBridge.spaceContentService.TakeOwnership(component.spaceObject);
                yield return request;
                
                if (request.succeeded)
                {
                    Debug.Log($"Successfully took ownership of space object #{component.spaceObjectID}");
                    
                    // Example: Modify the space object now that we have ownership
                    // Set a variable on the space object
                    var spaceObject = component.spaceObject;
                    spaceObject.SetVariable(0, "Modified by manager");
                    
                    // Example: Update position
                    spaceObject.position = new Vector3(
                        spaceObject.position.x,
                        spaceObject.position.y + 0.5f,
                        spaceObject.position.z
                    );
                }
                else
                {
                    Debug.LogWarning($"Failed to take ownership of space object #{component.spaceObjectID}");
                }
                
                // Wait a frame before processing next component
                yield return null;
            }
        }
        
        Debug.Log("Finished processing all manageable components");
    }
    
    // Example: Pass ownership to another player
    public void PassOwnershipToPlayer(int actorNumber)
    {
        foreach (var component in managedComponents)
        {
            if (!component.isDisposed && component.spaceObject.isMine)
            {
                Debug.Log($"Passing ownership of space object #{component.spaceObjectID} to actor #{actorNumber}");
                
                SpatialBridge.spaceContentService.GiveOwnership(component.spaceObject, actorNumber)
                    .SetCompletedEvent((request) => {
                        if (request.succeeded)
                        {
                            Debug.Log($"Successfully transferred ownership of space object #{component.spaceObjectID}");
                        }
                        else
                        {
                            Debug.LogWarning($"Failed to transfer ownership of space object #{component.spaceObjectID}");
                        }
                    });
            }
        }
    }
    
    // Example: Modify a prefab component if it's a specific type
    public void ModifyPrefabIfOwned(string newAssetId)
    {
        foreach (var component in managedComponents)
        {
            if (!component.isDisposed && component.spaceObject.isMine && component is IPrefabObject prefabObject)
            {
                Debug.Log($"Changing asset of prefab object #{component.spaceObjectID}");
                prefabObject.SetAsset(prefabObject.assetType, newAssetId);
            }
        }
    }
}
```

## Best Practices

1. Always check `isDisposed` before accessing component or space object properties.
2. Verify that you have ownership of the space object (check `spaceObject.isMine`) before attempting to make modifications.
3. Use `TakeOwnership` when you need to modify a space object that your client doesn't currently own.
4. Release ownership when you're done making changes to allow other clients to modify the object.
5. Group related modifications together to minimize network traffic and ownership transfers.
6. Use specific component interfaces (like `IPrefabObject`) when you need to access or modify component-specific properties.
7. Remember that modifications to the space object will be synchronized to all clients.

## Common Use Cases

1. Modifying component properties that require ownership verification.
2. Building systems that manage multiple space object components centrally.
3. Implementing ownership transfer mechanisms for collaborative experiences.
4. Creating dynamic environments where components can be modified at runtime.
5. Developing turn-based systems where control transfers between different actors.
6. Building UI systems that allow users to modify space object properties.
7. Creating management tools that can batch process components based on specific criteria.

## Completed: March 9, 2025