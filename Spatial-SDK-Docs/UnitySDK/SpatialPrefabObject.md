# SpatialPrefabObject

Category: Core Components

Interface/Class/Enum: Class

The SpatialPrefabObject component is used to define an asset that can be spawned in a Spatial environment as a prefab. This component serves as a container for various interactive and synchronized elements within the prefab, allowing the prefab to function within the networked environment. SpatialPrefabObject inherits from SpatialPackageAsset, which means it can be packaged and published to the Spatial marketplace.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| interactables | List<SpatialInteractable> | Collection of interactable components within this prefab object. |
| pointsOfInterest | List<SpatialPointOfInterest> | Collection of point of interest components within this prefab object. |
| seats | List<SpatialSeatHotspot> | Collection of seat hotspot components within this prefab object. |
| spatialEvents | List<SpatialEvent> | Collection of spatial event components within this prefab object. |
| syncedAnimators | List<SpatialSyncedAnimator> | Collection of synchronized animator components within this prefab object. |
| syncedObjects | List<SpatialSyncedObject> | Collection of synchronized object components within this prefab object. |
| triggerEvents | List<SpatialTriggerEvent> | Collection of trigger event components within this prefab object. |
| unsyncedAnimators | List<Animator> | Collection of non-synchronized animators within this prefab object. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |
| documentationURL | string | URL to documentation for this component. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections.Generic;

public class PrefabSpawnerExample : MonoBehaviour
{
    // Reference to the prefab to spawn
    [SerializeField] private GameObject prefabToSpawn;
    
    // Spawn points
    [SerializeField] private Transform[] spawnPoints;
    
    // Tracking spawned objects
    private List<GameObject> spawnedObjects = new List<GameObject>();
    
    public void SpawnPrefabAtNextAvailablePoint()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogError("No prefab assigned to spawn!");
            return;
        }
        
        // Find a SpatialPrefabObject component in the prefab
        SpatialPrefabObject prefabObject = prefabToSpawn.GetComponent<SpatialPrefabObject>();
        if (prefabObject == null)
        {
            Debug.LogError("The prefab does not have a SpatialPrefabObject component!");
            return;
        }
        
        // Get next available spawn point
        Transform spawnPoint = GetNextSpawnPoint();
        if (spawnPoint == null)
        {
            Debug.LogWarning("No available spawn points!");
            return;
        }
        
        // Create spawn options
        SpawnPrefabObjectRequest.Options options = new SpawnPrefabObjectRequest.Options
        {
            position = spawnPoint.position,
            rotation = spawnPoint.rotation
        };
        
        // Spawn the prefab using the SpaceContentService
        SpawnPrefabObjectRequest request = SpatialBridge.spaceContentService.SpawnPrefabObject(prefabToSpawn, options);
        
        // Handle the asynchronous operation
        StartCoroutine(HandleSpawnRequest(request, spawnPoint));
    }
    
    private System.Collections.IEnumerator HandleSpawnRequest(SpawnPrefabObjectRequest request, Transform spawnPoint)
    {
        // Wait for the request to complete
        yield return request;
        
        if (request.succeeded && request.prefabObject != null)
        {
            Debug.Log($"Successfully spawned prefab at {spawnPoint.name}");
            
            // Add to our tracking list
            spawnedObjects.Add(request.prefabObject.gameObject);
            
            // Example: Log information about the prefab components
            LogPrefabComponents(request.prefabObject);
        }
        else
        {
            Debug.LogError($"Failed to spawn prefab: {request.errorMessage}");
        }
    }
    
    private Transform GetNextSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return null;
            
        // Simple round-robin selection
        int index = spawnedObjects.Count % spawnPoints.Length;
        return spawnPoints[index];
    }
    
    private void LogPrefabComponents(SpatialPrefabObject prefabObject)
    {
        Debug.Log($"Prefab Components Summary for {prefabObject.name}:");
        
        // Log interactables
        if (prefabObject.interactables != null && prefabObject.interactables.Count > 0)
        {
            Debug.Log($"- Interactables: {prefabObject.interactables.Count}");
            foreach (var interactable in prefabObject.interactables)
            {
                Debug.Log($"  * {interactable.name}");
            }
        }
        
        // Log points of interest
        if (prefabObject.pointsOfInterest != null && prefabObject.pointsOfInterest.Count > 0)
        {
            Debug.Log($"- Points of Interest: {prefabObject.pointsOfInterest.Count}");
            foreach (var poi in prefabObject.pointsOfInterest)
            {
                Debug.Log($"  * {poi.title}");
            }
        }
        
        // Log seats
        if (prefabObject.seats != null && prefabObject.seats.Count > 0)
        {
            Debug.Log($"- Seats: {prefabObject.seats.Count}");
        }
        
        // Log synced animators
        if (prefabObject.syncedAnimators != null && prefabObject.syncedAnimators.Count > 0)
        {
            Debug.Log($"- Synchronized Animators: {prefabObject.syncedAnimators.Count}");
        }
        
        // Log synced objects
        if (prefabObject.syncedObjects != null && prefabObject.syncedObjects.Count > 0)
        {
            Debug.Log($"- Synchronized Objects: {prefabObject.syncedObjects.Count}");
        }
    }
    
    public void DespawnAllPrefabs()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
                // Destroy will automatically handle the network despawn
                Destroy(obj);
            }
        }
        
        // Clear the list
        spawnedObjects.Clear();
        Debug.Log("Despawned all prefabs");
    }
    
    // Example: Create a customized prefab programmatically
    public GameObject CreateCustomPrefab(string name, Color color)
    {
        // Create a new GameObject
        GameObject newPrefab = new GameObject(name);
        
        // Add required components
        SpatialPrefabObject prefabObject = newPrefab.AddComponent<SpatialPrefabObject>();
        
        // Add a visual representation
        GameObject visualObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visualObject.transform.SetParent(newPrefab.transform);
        visualObject.transform.localPosition = Vector3.zero;
        visualObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        
        // Set the material color
        Renderer renderer = visualObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.color = color;
            renderer.material = material;
        }
        
        // Add an interactable component
        SpatialInteractable interactable = visualObject.AddComponent<SpatialInteractable>();
        interactable.title = name;
        interactable.description = $"A custom {color.ToString()} prefab";
        
        // Add a point of interest
        SpatialPointOfInterest poi = newPrefab.AddComponent<SpatialPointOfInterest>();
        poi.title = name;
        poi.description = $"This is a custom prefab created with the color {color.ToString()}";
        
        // Add a synced object for network synchronization
        SpatialSyncedObject syncedObject = newPrefab.AddComponent<SpatialSyncedObject>();
        
        Debug.Log($"Created custom prefab: {name}");
        return newPrefab;
    }
}
```

## Best Practices

1. Always use SpatialBridge.spaceContentService.SpawnPrefabObject to spawn prefab objects to ensure they are properly synchronized across the network.
2. Use Object.Destroy or SpatialBridge.spaceContentService.DestroySpaceObject to properly despawn prefabs when they are no longer needed.
3. Keep your prefab hierarchies organized and avoid deeply nested structures to improve performance.
4. Properly configure any interactive or synchronized components (interactables, synced objects, etc.) before spawning the prefab.
5. Use prefabs for reusable objects that may need to be spawned multiple times rather than placing individual objects directly in the scene.
6. Consider performance implications when designing prefabs, especially those with many synchronized components.
7. Test your prefabs in multiplayer scenarios to ensure they behave correctly across the network.
8. Make use of the various component collections (interactables, pointsOfInterest, etc.) to organize and manage the functionality of your prefab.
9. When creating prefabs for the marketplace, ensure they are well-tested, optimized, and have clear documentation.
10. For complex prefabs, consider breaking them down into smaller, modular prefabs that can be combined as needed.

## Common Use Cases

1. Spawnable furniture, decorations, or props in customizable environments.
2. Interactive objects like buttons, levers, doors, or other mechanisms.
3. Vehicles or other transportation methods that can be spawned on demand.
4. Tools, weapons, or other items that users can interact with.
5. Dynamic obstacles or challenges in game environments.
6. Informational displays or exhibits with points of interest.
7. Complex interactive systems broken down into modular, reusable components.
8. Player-created content that can be saved, shared, and reused.
9. Marketplace assets that can be published and purchased by other creators.
10. Template objects for rapid creation of customized variations during runtime.

## Completed: March 10, 2025