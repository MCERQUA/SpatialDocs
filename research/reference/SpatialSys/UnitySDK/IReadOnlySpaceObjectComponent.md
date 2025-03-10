# IReadOnlySpaceObjectComponent

Category: Space Content Service

Interface

A component of a space object. This interface provides read-only access to the component and its associated space object.

## Properties

| Property | Description |
| --- | --- |
| isDisposed | Returns true when the component or its parent space object has been destroyed. |
| spaceObject | The space object the component is attached to. |
| spaceObjectID | The id of the spaceObject the component is attached to. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections.Generic;

public class SpaceObjectComponentTracker : MonoBehaviour
{
    // Track components by their associated space object ID
    private Dictionary<int, List<IReadOnlySpaceObjectComponent>> componentsMap = 
        new Dictionary<int, List<IReadOnlySpaceObjectComponent>>();
    
    private void OnEnable()
    {
        // Subscribe to space object events to track components
        SpatialBridge.spaceContentService.onSpaceObjectCreated += HandleSpaceObjectCreated;
        SpatialBridge.spaceContentService.onSpaceObjectDestroyed += HandleSpaceObjectDestroyed;
        
        // Scan for existing space objects
        foreach (var spaceObject in SpatialBridge.spaceContentService.spaceObjects)
        {
            HandleSpaceObjectCreated(spaceObject);
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        SpatialBridge.spaceContentService.onSpaceObjectCreated -= HandleSpaceObjectCreated;
        SpatialBridge.spaceContentService.onSpaceObjectDestroyed -= HandleSpaceObjectDestroyed;
        
        // Clear our tracking
        componentsMap.Clear();
    }
    
    private void HandleSpaceObjectCreated(IReadOnlySpaceObject spaceObject)
    {
        if (!spaceObject.isDisposed)
        {
            List<IReadOnlySpaceObjectComponent> components = new List<IReadOnlySpaceObjectComponent>();
            
            // Check for different component types based on the object type
            if (spaceObject.objectType == SpaceObjectType.PrefabObject)
            {
                // Get the prefab object component
                var prefabObj = SpatialBridge.spaceContentService.GetPrefabObject(spaceObject.objectID);
                if (prefabObj != null)
                {
                    components.Add(prefabObj);
                    Debug.Log($"Added prefab object component to space object #{spaceObject.objectID}");
                }
            }
            
            // Add any other component checks here based on your needs
            
            if (components.Count > 0)
            {
                componentsMap[spaceObject.objectID] = components;
                
                // Log component details
                Debug.Log($"Space object #{spaceObject.objectID} has {components.Count} components");
                foreach (var component in components)
                {
                    Debug.Log($"  Component type: {component.GetType().Name}");
                    Debug.Log($"  Is disposed: {component.isDisposed}");
                    Debug.Log($"  Space object ID: {component.spaceObjectID}");
                }
            }
        }
    }
    
    private void HandleSpaceObjectDestroyed(IReadOnlySpaceObject spaceObject)
    {
        // When a space object is destroyed, remove its components from our tracking
        if (componentsMap.ContainsKey(spaceObject.objectID))
        {
            Debug.Log($"Removing components for destroyed space object #{spaceObject.objectID}");
            componentsMap.Remove(spaceObject.objectID);
        }
    }
    
    // Example: Check if any components have been disposed
    public void ValidateComponents()
    {
        List<int> objectsToRemove = new List<int>();
        
        foreach (var kvp in componentsMap)
        {
            bool allDisposed = true;
            
            foreach (var component in kvp.Value)
            {
                if (component.isDisposed)
                {
                    Debug.Log($"Component of space object #{kvp.Key} is disposed");
                }
                else
                {
                    allDisposed = false;
                }
            }
            
            if (allDisposed)
            {
                objectsToRemove.Add(kvp.Key);
            }
        }
        
        // Clean up any entries where all components are disposed
        foreach (int objectID in objectsToRemove)
        {
            componentsMap.Remove(objectID);
        }
    }
    
    // Example: Find all components of a specific type
    public List<T> FindComponentsOfType<T>() where T : class, IReadOnlySpaceObjectComponent
    {
        List<T> result = new List<T>();
        
        foreach (var componentsList in componentsMap.Values)
        {
            foreach (var component in componentsList)
            {
                if (!component.isDisposed && component is T typedComponent)
                {
                    result.Add(typedComponent);
                }
            }
        }
        
        return result;
    }
    
    // Example: Get all components for a specific space object
    public List<IReadOnlySpaceObjectComponent> GetComponentsForSpaceObject(int spaceObjectID)
    {
        if (componentsMap.TryGetValue(spaceObjectID, out List<IReadOnlySpaceObjectComponent> components))
        {
            // Filter out any disposed components
            return components.FindAll(c => !c.isDisposed);
        }
        
        return new List<IReadOnlySpaceObjectComponent>();
    }
}
```

## Best Practices

1. Always check `isDisposed` before accessing any properties or methods on a space object component.
2. Don't cache component references for too long without validating they're still valid using `isDisposed`.
3. When storing component references, organize them by their `spaceObjectID` for efficient management.
4. Remember that the `spaceObject` property provides access to the parent space object, which gives you additional context and capabilities.
5. Use interface inheritance hierarchy appropriately - components may implement more specific interfaces that provide additional functionality.
6. Remove references to components when their parent space object is destroyed to prevent memory leaks.

## Common Use Cases

1. Tracking components across multiple space objects in a centralized system.
2. Building component-based interaction systems that respond to different component types.
3. Creating monitoring and debugging tools to inspect the state of space object components.
4. Implementing systems that need to query all components of a specific type across multiple space objects.
5. Building component filters that can check for specific properties or capabilities across space objects.
6. Creating component-based event systems that depend on component lifecycle events.

## Completed: March 9, 2025