# SpaceObjectType

Category: Space Content Service Related

Interface/Class/Enum: Enum

An enum used to describe what type of visual component a space object has. This helps identify and categorize different types of space objects in the Spatial environment.

## Properties/Fields

| Value | Description |
| --- | --- |
| Empty | The space object has no visual component. For example: an empty object with variables. |
| PrefabObject | The space object is a Spatial prefab. |
| NetworkObject | The space object represents a network object embedded in a scene or spawned at runtime. |
| Avatar | The space object is an avatar. |
| Unknown | The type of component is unknown. This is likely an internal spatial object not yet exposed via the SDK. |

## Usage Examples

```csharp
// Example: Object Type Manager
public class ObjectTypeManager : MonoBehaviour
{
    private ISpaceContentService contentService;
    private Dictionary<SpaceObjectType, List<int>> objectsByType;
    
    void Start()
    {
        contentService = SpatialBridge.spaceContentService;
        InitializeManager();
        SubscribeToEvents();
    }
    
    private void InitializeManager()
    {
        // Initialize collections for each object type
        objectsByType = new Dictionary<SpaceObjectType, List<int>>();
        foreach (SpaceObjectType type in Enum.GetValues(typeof(SpaceObjectType)))
        {
            objectsByType[type] = new List<int>();
        }
        
        // Categorize existing objects
        foreach (var obj in contentService.allObjects.Values)
        {
            AddObjectToTypeList(obj);
        }
    }
    
    private void SubscribeToEvents()
    {
        contentService.onObjectSpawned += HandleObjectSpawned;
        contentService.onObjectDestroyed += HandleObjectDestroyed;
    }
    
    private void HandleObjectSpawned(IReadOnlySpaceObject obj)
    {
        AddObjectToTypeList(obj);
    }
    
    private void HandleObjectDestroyed(IReadOnlySpaceObject obj)
    {
        RemoveObjectFromTypeList(obj);
    }
    
    private void AddObjectToTypeList(IReadOnlySpaceObject obj)
    {
        if (objectsByType.TryGetValue(obj.objectType, out var list))
        {
            if (!list.Contains(obj.objectID))
            {
                list.Add(obj.objectID);
                Debug.Log($"Added object ID {obj.objectID} of type {obj.objectType}");
            }
        }
    }
    
    private void RemoveObjectFromTypeList(IReadOnlySpaceObject obj)
    {
        if (objectsByType.TryGetValue(obj.objectType, out var list))
        {
            if (list.Contains(obj.objectID))
            {
                list.Remove(obj.objectID);
                Debug.Log($"Removed object ID {obj.objectID} of type {obj.objectType}");
            }
        }
    }
    
    // Get all objects of a specific type
    public List<IReadOnlySpaceObject> GetObjectsByType(SpaceObjectType type)
    {
        var objects = new List<IReadOnlySpaceObject>();
        
        if (objectsByType.TryGetValue(type, out var idList))
        {
            foreach (var id in idList)
            {
                if (contentService.allObjects.TryGetValue(id, out var obj))
                {
                    objects.Add(obj);
                }
            }
        }
        
        return objects;
    }
    
    // Print statistics for all object types
    public void PrintObjectTypeStatistics()
    {
        Debug.Log("== Space Object Type Statistics ==");
        
        foreach (SpaceObjectType type in Enum.GetValues(typeof(SpaceObjectType)))
        {
            int count = objectsByType[type].Count;
            Debug.Log($"{type}: {count} object(s)");
        }
        
        Debug.Log("===============================");
    }
    
    void OnDestroy()
    {
        if (contentService != null)
        {
            contentService.onObjectSpawned -= HandleObjectSpawned;
            contentService.onObjectDestroyed -= HandleObjectDestroyed;
        }
    }
}
```

## Best Practices

1. Use SpaceObjectType for conditional logic when handling different types of space objects
2. Implement type-specific handling for different object types to ensure proper behavior
3. Check the object type before performing type-specific operations
4. Remember that the Unknown type may be used for internal Spatial objects and may change in future SDK versions
5. Consider implementing object pools or managers for each object type for better organization

## Common Use Cases

1. Filtering objects in a scene based on their type
2. Implementing type-specific interaction systems
3. Creating management systems that track objects by category
4. Building statistical tools to analyze object usage
5. Setting up specialized handlers for different object types (e.g., avatar-specific features)

## Completed: March 9, 2025
