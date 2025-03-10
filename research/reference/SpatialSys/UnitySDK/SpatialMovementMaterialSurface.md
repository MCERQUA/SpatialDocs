# SpatialMovementMaterialSurface

Category: Core Components

Interface/Class/Enum: Class

The SpatialMovementMaterialSurface component allows developers to define special movement properties for surfaces in a Spatial environment. When an avatar moves on a surface with this component, their movement characteristics are modified according to the assigned movement material.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| movementMaterial | SpatialMovementMaterial | The movement material asset that defines how avatars move on this surface. |
| isExperimental | bool | Indicates whether this component is still in experimental status and may be subject to changes or limitations. |
| documentationURL | string | URL to documentation for this component. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;

public class TerrainSurfaceManager : MonoBehaviour
{
    [SerializeField] private SpatialMovementMaterial iceMaterial;
    [SerializeField] private SpatialMovementMaterial mudMaterial;
    [SerializeField] private SpatialMovementMaterial bouncyMaterial;
    
    [SerializeField] private GameObject iceArea;
    [SerializeField] private GameObject mudArea;
    [SerializeField] private GameObject bouncyArea;
    
    private void Start()
    {
        // Configure special movement surfaces
        ConfigureMovementSurfaces();
    }
    
    private void ConfigureMovementSurfaces()
    {
        // Configure ice surface
        if (iceArea != null && iceMaterial != null)
        {
            ConfigureSingleSurface(iceArea, iceMaterial, "Ice");
        }
        
        // Configure mud surface
        if (mudArea != null && mudMaterial != null)
        {
            ConfigureSingleSurface(mudArea, mudMaterial, "Mud");
        }
        
        // Configure bouncy surface
        if (bouncyArea != null && bouncyMaterial != null)
        {
            ConfigureSingleSurface(bouncyArea, bouncyMaterial, "Bouncy");
        }
    }
    
    private void ConfigureSingleSurface(GameObject surfaceObject, SpatialMovementMaterial material, string surfaceType)
    {
        // Ensure the object has a collider
        if (!surfaceObject.TryGetComponent<Collider>(out _))
        {
            surfaceObject.AddComponent<BoxCollider>();
            Debug.Log($"Added BoxCollider to {surfaceObject.name} for movement surface");
        }
        
        // Add SpatialMovementMaterialSurface component if not already present
        SpatialMovementMaterialSurface surface = surfaceObject.GetComponent<SpatialMovementMaterialSurface>();
        if (surface == null)
        {
            surface = surfaceObject.AddComponent<SpatialMovementMaterialSurface>();
        }
        
        // Assign movement material
        surface.movementMaterial = material;
        
        Debug.Log($"Configured {surfaceType} movement material on {surfaceObject.name}");
    }
    
    // Method to create a new movement surface programmatically
    public GameObject CreateMovementSurface(Vector3 position, Vector3 size, SpatialMovementMaterial material, string surfaceType)
    {
        // Create new GameObject for the surface
        GameObject surface = new GameObject($"{surfaceType}_Surface");
        surface.transform.position = position;
        
        // Add visual mesh
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.transform.parent = surface.transform;
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = size;
        
        // Remove the collider from the visual (we'll add one to the parent)
        DestroyImmediate(visual.GetComponent<Collider>());
        
        // Add collider to the parent
        BoxCollider collider = surface.AddComponent<BoxCollider>();
        collider.size = size;
        
        // Add movement material surface component
        SpatialMovementMaterialSurface movementSurface = surface.AddComponent<SpatialMovementMaterialSurface>();
        movementSurface.movementMaterial = material;
        
        // Customize the visual appearance based on surface type
        Renderer renderer = visual.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material visualMaterial = new Material(Shader.Find("Standard"));
            
            switch (surfaceType.ToLower())
            {
                case "ice":
                    visualMaterial.color = new Color(0.8f, 0.9f, 1.0f, 0.8f);
                    visualMaterial.SetFloat("_Glossiness", 0.9f);
                    break;
                case "mud":
                    visualMaterial.color = new Color(0.3f, 0.2f, 0.1f, 1.0f);
                    visualMaterial.SetFloat("_Glossiness", 0.2f);
                    break;
                case "bouncy":
                    visualMaterial.color = new Color(0.8f, 0.3f, 0.3f, 1.0f);
                    visualMaterial.SetFloat("_Glossiness", 0.6f);
                    break;
                default:
                    visualMaterial.color = Color.white;
                    break;
            }
            
            renderer.material = visualMaterial;
        }
        
        Debug.Log($"Created {surfaceType} movement surface at {position} with size {size}");
        return surface;
    }
    
    // Method to create different surface types for a parkour course
    public void CreateParkourCourse(Vector3 startPosition, int segments)
    {
        float segmentLength = 3f;
        float segmentWidth = 2f;
        float gap = 1f;
        
        Vector3 currentPosition = startPosition;
        
        for (int i = 0; i < segments; i++)
        {
            SpatialMovementMaterial material;
            string surfaceType;
            
            // Cycle through different surface types
            switch (i % 3)
            {
                case 0:
                    material = iceMaterial;
                    surfaceType = "Ice";
                    break;
                case 1:
                    material = mudMaterial;
                    surfaceType = "Mud";
                    break;
                case 2:
                    material = bouncyMaterial;
                    surfaceType = "Bouncy";
                    break;
                default:
                    material = null;
                    surfaceType = "Default";
                    break;
            }
            
            if (material != null)
            {
                CreateMovementSurface(
                    currentPosition, 
                    new Vector3(segmentWidth, 0.3f, segmentLength),
                    material,
                    surfaceType
                );
            }
            
            // Move to next segment position
            currentPosition += new Vector3(0, 0, segmentLength + gap);
        }
        
        Debug.Log($"Created parkour course with {segments} segments starting at {startPosition}");
    }
}
```

## Best Practices

1. Always assign a valid SpatialMovementMaterial to the component, as null references may cause unexpected behavior.
2. Ensure the object has an appropriate collider that matches the visual representation of the surface.
3. Consider using visual cues to indicate different surface types to users (ice could be blue and shiny, mud could be brown and rough, etc.).
4. Test movement surfaces with different avatar types to ensure they provide a consistent experience.
5. Use movement surfaces strategically to add gameplay variety and challenges.
6. Remember that the `isExperimental` flag indicates this feature may evolve in future SDK versions.
7. Consider the impact on user experience when designing movement surfaces, especially for VR users who may be sensitive to changes in movement.
8. For networked experiences, consider how movement surfaces affect synchronization across clients.

## Common Use Cases

1. Ice or slippery surfaces with reduced friction for sliding puzzles or challenges.
2. Mud or sticky surfaces that slow down avatar movement.
3. Bouncy surfaces that increase jump height or add rebound effects.
4. Speed-boosting surfaces for racing or timed challenges.
5. Surfaces with custom footstep sounds based on material type.
6. Parkour courses with varied movement surfaces to create diverse challenges.
7. Puzzle elements where specific movement properties are needed to reach objectives.
8. Terrain differentiation in open-world environments (beaches, rocky areas, grassy fields).

## Completed: March 09, 2025