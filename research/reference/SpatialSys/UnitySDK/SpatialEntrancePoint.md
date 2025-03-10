# SpatialEntrancePoint

Category: Core Components

Interface/Class/Enum: Class

The SpatialEntrancePoint component defines locations where users can spawn when entering a Spatial environment. When a user joins a space, they will appear at one of the entrance points based on the space's configuration.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| radius | float | The radius around the entrance point where avatars can spawn. |
| documentationURL | string | URL to documentation for this component. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;

public class EntranceManager : MonoBehaviour
{
    [SerializeField] private Transform[] entranceLocations;
    [SerializeField] private float defaultRadius = 2f;
    
    private void Start()
    {
        // Create entrance points at specified locations
        CreateEntrancePoints();
    }
    
    private void CreateEntrancePoints()
    {
        if (entranceLocations == null || entranceLocations.Length == 0)
        {
            Debug.LogWarning("No entrance locations specified!");
            return;
        }
        
        for (int i = 0; i < entranceLocations.Length; i++)
        {
            Transform location = entranceLocations[i];
            
            // Create new GameObject for entrance point
            GameObject entrancePoint = new GameObject($"Entrance_Point_{i + 1}");
            entrancePoint.transform.parent = transform;
            entrancePoint.transform.position = location.position;
            entrancePoint.transform.rotation = location.rotation;
            
            // Add SpatialEntrancePoint component
            SpatialEntrancePoint entrance = entrancePoint.AddComponent<SpatialEntrancePoint>();
            entrance.radius = defaultRadius;
            
            // Optional: Add visual indicator for the entrance point (in editor only)
            #if UNITY_EDITOR
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visual.name = "EntranceVisual";
            visual.transform.parent = entrancePoint.transform;
            visual.transform.localPosition = new Vector3(0, 0.05f, 0);
            visual.transform.localScale = new Vector3(entrance.radius * 2, 0.1f, entrance.radius * 2);
            
            // Make it semi-transparent
            Renderer renderer = visual.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = new Color(0, 1, 0, 0.3f); // Semi-transparent green
                renderer.material = mat;
            }
            #endif
            
            Debug.Log($"Created entrance point at {location.position} with radius {entrance.radius}");
        }
    }
    
    // Function to create an entrance point programmatically
    public SpatialEntrancePoint CreateEntrancePoint(Vector3 position, Quaternion rotation, float radius)
    {
        GameObject entrancePoint = new GameObject("Entrance_Point");
        entrancePoint.transform.parent = transform;
        entrancePoint.transform.position = position;
        entrancePoint.transform.rotation = rotation;
        
        SpatialEntrancePoint entrance = entrancePoint.AddComponent<SpatialEntrancePoint>();
        entrance.radius = radius;
        
        Debug.Log($"Created entrance point at {position} with radius {radius}");
        return entrance;
    }
    
    // Disable specific entrance points (useful for conditional entrances)
    public void DisableEntrancePoint(GameObject entrancePoint)
    {
        if (entrancePoint.TryGetComponent<SpatialEntrancePoint>(out var entrance))
        {
            entrancePoint.SetActive(false);
            Debug.Log($"Disabled entrance point: {entrancePoint.name}");
        }
    }
}
```

## Best Practices

1. Place entrance points in open areas where avatars won't spawn inside geometry or get stuck.
2. Consider the initial view and experience when a user spawns at each entrance point.
3. Set an appropriate radius to prevent users from spawning too close to each other.
4. Position entrance points strategically to guide users to important areas of your space.
5. For larger spaces, consider creating multiple entrance points throughout the environment.
6. Orient the entrance point's forward direction to face important elements or the expected direction of movement.
7. Test entrance points with different device types to ensure good spawning experiences for all users.

## Common Use Cases

1. Main entrance point for users joining a space.
2. Multiple entrance points for different sections or areas of a large environment.
3. Special entrance points for VIP or privileged users.
4. Respawn points for users after falling off the map or triggering a respawn.
5. Conditional entrance points that become active based on game state or event progress.
6. Entrance points linked to specific activities or experiences within a space.
7. Tutorial or introduction areas for first-time visitors.

## Completed: March 09, 2025