# SpatialClimbable

Category: Core Components

Interface/Class/Enum: Class

The SpatialClimbable component designates objects that avatars can climb in the Spatial environment. When applied to an object with a collider, it turns that object into a surface or structure that users can climb by grabbing with their hands.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| isExperimental | bool | Indicates whether this component is still in experimental status and may be subject to changes or limitations. |
| documentationURL | string | URL to documentation for this component. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;

public class ClimbingSystemManager : MonoBehaviour
{
    [SerializeField] private GameObject[] climbableSurfaces;
    
    private void Start()
    {
        ConfigureClimbableSurfaces();
    }
    
    private void ConfigureClimbableSurfaces()
    {
        foreach (GameObject surface in climbableSurfaces)
        {
            // Ensure the object has a collider
            if (!surface.TryGetComponent<Collider>(out _))
            {
                surface.AddComponent<BoxCollider>();
                Debug.Log($"Added BoxCollider to {surface.name} for climbing interaction");
            }
            
            // Add SpatialClimbable component if not already present
            if (!surface.TryGetComponent<SpatialClimbable>(out _))
            {
                surface.AddComponent<SpatialClimbable>();
                Debug.Log($"Made {surface.name} climbable");
            }
        }
    }
    
    // Method to create a climbable ladder
    public GameObject CreateClimbableLadder(Vector3 position, float height, int rungCount)
    {
        GameObject ladder = new GameObject("Climbable_Ladder");
        ladder.transform.position = position;
        
        // Create ladder sides
        GameObject leftSide = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        leftSide.transform.parent = ladder.transform;
        leftSide.transform.localPosition = new Vector3(-0.4f, height/2, 0);
        leftSide.transform.localScale = new Vector3(0.1f, height, 0.1f);
        leftSide.AddComponent<SpatialClimbable>();
        
        GameObject rightSide = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rightSide.transform.parent = ladder.transform;
        rightSide.transform.localPosition = new Vector3(0.4f, height/2, 0);
        rightSide.transform.localScale = new Vector3(0.1f, height, 0.1f);
        rightSide.AddComponent<SpatialClimbable>();
        
        // Create ladder rungs
        float rungSpacing = height / (rungCount + 1);
        for (int i = 1; i <= rungCount; i++)
        {
            GameObject rung = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rung.transform.parent = ladder.transform;
            rung.transform.localPosition = new Vector3(0, i * rungSpacing, 0);
            rung.transform.localRotation = Quaternion.Euler(0, 0, 90);
            rung.transform.localScale = new Vector3(0.1f, 0.9f, 0.1f);
            rung.AddComponent<SpatialClimbable>();
        }
        
        Debug.Log($"Created climbable ladder with {rungCount} rungs at {position}");
        return ladder;
    }
}
```

## Best Practices

1. Always ensure objects with SpatialClimbable have appropriate colliders that match their visual representation.
2. Consider the physical layout of climbable objects to ensure they provide a natural climbing experience.
3. For ladder-like structures, place rungs at appropriate distances for avatar hand positioning.
4. Use visual cues to indicate that objects are climbable (like highlighting or special materials).
5. Test climbing surfaces with different avatar types to ensure accessibility.
6. Remember that the `isExperimental` flag indicates this feature may evolve in future SDK versions.
7. Combine with other interactive components thoughtfully to avoid conflicting interactions.

## Common Use Cases

1. Ladders for vertical traversal in multi-level environments.
2. Rock climbing walls for recreational activities.
3. Vines or poles for natural environment traversal.
4. Scaffolding for construction-themed environments.
5. Obstacle course elements in gameplay scenarios.
6. Emergency escape routes in simulation environments.
7. Climbing challenges in adventure games.

## Completed: March 09, 2025