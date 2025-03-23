# IReadOnlySpaceObject

Category: Space Content Service

Interface

The data-model for an object as it exists on the server. Space objects can be created, destroyed, and modified by actors in the space.

## Properties

| Property | Description |
| --- | --- |
| canTakeOwnership | Whether the space object can be taken over by another actor. |
| creatorActorNumber | The actorNumber of the actor that created the space object. |
| flags | Space object behavior flags. |
| hasControl | Indicates whether the local client has control over the object (read-only). |
| hasVariables | Whether the space object has variables attached to it. |
| isDisposed | Whether the space object has been disposed. This will be true if the object has been destroyed. Disposed space objects can still be read from, but can no longer be edited. |
| isMine | Whether the space object is owned by the local actor (localActorNumber == ownerActorNumber). |
| objectID | The unique number of the space object in the current server instance. ObjectID's are allocated when space objects are created and cannot be modified. |
| objectType | Type of object it is, such as an avatar. |
| ownerActorNumber | The actorNumber of the actor that currently owns the space object. |
| position | The position of a space object in world space as it exists on the server. Note: This may not be the same as the position of the visual representation of the object in the scene because the visual representation may be smoothed or extrapolated. |
| rotation | The rotation of a space object in world space as it exists on the server. Note: This may not be the same as the rotation of the visual representation of the object in the scene because the visual representation may be smoothed or extrapolated. |
| scale | The scale of a space object in world space as it exists on the server. Note: This may not be the same as the scale of the visual representation of the object in the scene because the visual representation may be smoothed or extrapolated. |
| variables | A dictionary of variables that are attached to the space object. These are synchronized across all clients. This dictionary is never null, but may be empty. To check if object has any variables, use hasVariables for better performance. |

## Methods

| Method | Description |
| --- | --- |
| TryGetVariable(byte, out object) | Retrieves the value of a variable on the space object. |
| TryGetVariable<T>(byte, out T) | Retrieves the value of a variable on the space object with type casting. |

## Events

| Event | Description |
| --- | --- |
| onOwnerChanged | Event fired when the owner of the space object changes. |
| onVariablesChanged | Event fired when one or more variables on the space object have changed (added, changed, removed). The event is fired for all connected clients when they receive the change. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections.Generic;

public class SpaceObjectTracker : MonoBehaviour
{
    // Dictionary to track space objects by their ID
    private Dictionary<int, IReadOnlySpaceObject> activeSpaceObjects = new Dictionary<int, IReadOnlySpaceObject>();
    
    private void OnEnable()
    {
        // Subscribe to space object events
        SpatialBridge.spaceContentService.onSpaceObjectCreated += HandleSpaceObjectCreated;
        SpatialBridge.spaceContentService.onSpaceObjectDestroyed += HandleSpaceObjectDestroyed;
        
        // Track existing space objects
        foreach (var spaceObject in SpatialBridge.spaceContentService.spaceObjects)
        {
            TrackSpaceObject(spaceObject);
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        SpatialBridge.spaceContentService.onSpaceObjectCreated -= HandleSpaceObjectCreated;
        SpatialBridge.spaceContentService.onSpaceObjectDestroyed -= HandleSpaceObjectDestroyed;
        
        // Clear references
        activeSpaceObjects.Clear();
    }
    
    private void HandleSpaceObjectCreated(IReadOnlySpaceObject spaceObject)
    {
        TrackSpaceObject(spaceObject);
    }
    
    private void HandleSpaceObjectDestroyed(IReadOnlySpaceObject spaceObject)
    {
        // Note: isDisposed will be true for this object now
        if (activeSpaceObjects.ContainsKey(spaceObject.objectID))
        {
            Debug.Log($"Space object #{spaceObject.objectID} was destroyed");
            activeSpaceObjects.Remove(spaceObject.objectID);
        }
    }
    
    private void TrackSpaceObject(IReadOnlySpaceObject spaceObject)
    {
        if (!spaceObject.isDisposed)
        {
            // Store reference to the space object
            activeSpaceObjects[spaceObject.objectID] = spaceObject;
            
            // Subscribe to object events
            spaceObject.onOwnerChanged += (args) => HandleOwnerChanged(spaceObject, args);
            spaceObject.onVariablesChanged += (args) => HandleVariablesChanged(spaceObject, args);
            
            Debug.Log($"Tracking space object #{spaceObject.objectID} of type {spaceObject.objectType}");
            Debug.Log($"  Created by actor #{spaceObject.creatorActorNumber}");
            Debug.Log($"  Currently owned by actor #{spaceObject.ownerActorNumber}");
            Debug.Log($"  Position: {spaceObject.position}");
            
            // Check if it has any variables
            if (spaceObject.hasVariables)
            {
                Debug.Log($"  Has {spaceObject.variables.Count} variables");
                
                // Example of retrieving a specific variable
                if (spaceObject.TryGetVariable<string>(0, out string nameVar))
                {
                    Debug.Log($"  Name: {nameVar}");
                }
                
                if (spaceObject.TryGetVariable<int>(1, out int scoreVar))
                {
                    Debug.Log($"  Score: {scoreVar}");
                }
            }
        }
    }
    
    private void HandleOwnerChanged(IReadOnlySpaceObject spaceObject, SpaceObjectOwnerChangedEventArgs args)
    {
        Debug.Log($"Space object #{spaceObject.objectID} ownership changed:");
        Debug.Log($"  Previous owner: Actor #{args.oldOwnerActorNumber}");
        Debug.Log($"  New owner: Actor #{args.newOwnerActorNumber}");
        
        // If we now own it, we could take some action
        if (spaceObject.isMine)
        {
            Debug.Log("We now own this space object!");
        }
    }
    
    private void HandleVariablesChanged(IReadOnlySpaceObject spaceObject, SpaceObjectVariablesChangedEventArgs args)
    {
        Debug.Log($"Space object #{spaceObject.objectID} variables changed:");
        
        // Check which variables were added or changed
        foreach (byte varId in args.changedVariableIds)
        {
            if (spaceObject.TryGetVariable(varId, out object value))
            {
                Debug.Log($"  Variable #{varId} changed to: {value}");
            }
        }
        
        // Check which variables were removed
        foreach (byte varId in args.removedVariableIds)
        {
            Debug.Log($"  Variable #{varId} was removed");
        }
    }
    
    // Example: Find all space objects of a specific type
    public List<IReadOnlySpaceObject> GetObjectsByType(SpaceObjectType type)
    {
        List<IReadOnlySpaceObject> result = new List<IReadOnlySpaceObject>();
        
        foreach (var kvp in activeSpaceObjects)
        {
            if (kvp.Value.objectType == type)
            {
                result.Add(kvp.Value);
            }
        }
        
        return result;
    }
    
    // Example: Find the closest space object to a position
    public IReadOnlySpaceObject FindClosestObject(Vector3 position, float maxDistance = float.MaxValue)
    {
        IReadOnlySpaceObject closest = null;
        float closestDistSqr = maxDistance * maxDistance;
        
        foreach (var kvp in activeSpaceObjects)
        {
            var spaceObj = kvp.Value;
            float distSqr = Vector3.SqrMagnitude(spaceObj.position - position);
            
            if (distSqr < closestDistSqr)
            {
                closest = spaceObj;
                closestDistSqr = distSqr;
            }
        }
        
        return closest;
    }
}
```

## Best Practices

1. Always check `isDisposed` before accessing properties of a space object, especially after having stored references.
2. Use `hasVariables` to check if an object has variables before accessing the variables dictionary for better performance.
3. Subscribe to `onOwnerChanged` and `onVariablesChanged` events to keep your game state updated with network changes.
4. Use `TryGetVariable<T>` with the appropriate type to safely retrieve typed variables from space objects.
5. Remember that the `position`, `rotation`, and `scale` properties reflect the server state, which may differ from the visual representation in the scene.
6. Store space object references in collections indexed by `objectID` for efficient lookups.
7. Consider the ownership model when interacting with space objects - you may need to take ownership before modifying them.

## Common Use Cases

1. Tracking and monitoring space objects in your game or application.
2. Building spatial awareness systems that respond to objects in the environment.
3. Creating interactive elements that can be owned and modified by different players.
4. Implementing networked physics and transformation systems.
5. Building data synchronization mechanisms using object variables.
6. Creating ownership-based game mechanics where players can take control of objects.
7. Designing multiplayer interactions where objects change state based on player input.

## Completed: March 9, 2025