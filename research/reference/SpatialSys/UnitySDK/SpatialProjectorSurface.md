# SpatialProjectorSurface

Category: Core Components

Interface/Class/Enum: Class

The SpatialProjectorSurface component defines a surface that can be used for projecting content in a Spatial environment. This component creates a designated area where images, videos, or other visual content can be displayed. Projector surfaces are useful for creating interactive displays, digital signage, presentation areas, or any other surface that needs to display dynamically changeable content.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| LATEST_VERSION | int | Static property representing the latest version of the SpatialProjectorSurface component. |
| version | int | The version of this component instance. |
| size | Vector3 | The three-dimensional size of the projector surface. |
| size2D | Vector2 | The two-dimensional size (width and height) of the projector surface. |
| dotsVisible | bool | Determines whether the projector surface displays alignment dots for visual reference. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |
| documentationURL | string | URL to documentation for this component. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections;

public class ProjectorSurfaceManager : MonoBehaviour
{
    // References to projector surfaces
    [SerializeField] private SpatialProjectorSurface mainDisplay;
    [SerializeField] private SpatialProjectorSurface secondaryDisplay;
    
    // Textures to display
    [SerializeField] private Texture2D[] presentationSlides;
    [SerializeField] private Texture2D backgroundTexture;
    
    // Materials for rendering content
    private Material mainDisplayMaterial;
    private Material secondaryDisplayMaterial;
    
    // Current slide index
    private int currentSlideIndex = 0;
    
    private void Start()
    {
        // Set up projector surface materials
        InitializeDisplayMaterials();
        
        // Configure projector surfaces
        ConfigureProjectorSurfaces();
        
        // Start with the first slide
        DisplaySlide(0);
    }
    
    private void InitializeDisplayMaterials()
    {
        // Get or create materials for the displays
        Renderer mainRenderer = mainDisplay.GetComponent<Renderer>();
        if (mainRenderer != null)
        {
            mainDisplayMaterial = mainRenderer.material;
        }
        else
        {
            Debug.LogError("Main display has no renderer component!");
        }
        
        Renderer secondaryRenderer = secondaryDisplay.GetComponent<Renderer>();
        if (secondaryRenderer != null)
        {
            secondaryDisplayMaterial = secondaryRenderer.material;
        }
        else
        {
            Debug.LogError("Secondary display has no renderer component!");
        }
        
        // Set the background texture for the secondary display
        if (secondaryDisplayMaterial != null && backgroundTexture != null)
        {
            secondaryDisplayMaterial.mainTexture = backgroundTexture;
        }
    }
    
    private void ConfigureProjectorSurfaces()
    {
        // Configure main display
        if (mainDisplay != null)
        {
            // Set size based on aspect ratio of presentation slides
            if (presentationSlides.Length > 0)
            {
                Texture2D firstSlide = presentationSlides[0];
                float aspectRatio = (float)firstSlide.width / firstSlide.height;
                
                // Set a standard height and adjust width by aspect ratio
                float height = 2.0f;
                float width = height * aspectRatio;
                
                mainDisplay.size2D = new Vector2(width, height);
            }
            else
            {
                // Default size if no slides are available
                mainDisplay.size2D = new Vector2(3.0f, 2.0f);
            }
            
            // Turn off the alignment dots for the main display
            mainDisplay.dotsVisible = false;
        }
        
        // Configure secondary display (for notes, controls, etc.)
        if (secondaryDisplay != null)
        {
            // Set a smaller size for the secondary display
            secondaryDisplay.size2D = new Vector2(1.5f, 1.0f);
            
            // Keep alignment dots visible for the secondary display
            secondaryDisplay.dotsVisible = true;
        }
    }
    
    // Display a specific slide on the main display
    public void DisplaySlide(int slideIndex)
    {
        if (presentationSlides == null || presentationSlides.Length == 0)
        {
            Debug.LogWarning("No presentation slides available!");
            return;
        }
        
        // Validate the slide index
        if (slideIndex < 0 || slideIndex >= presentationSlides.Length)
        {
            Debug.LogWarning($"Invalid slide index: {slideIndex}. Valid range is 0-{presentationSlides.Length - 1}");
            return;
        }
        
        // Update the current slide index
        currentSlideIndex = slideIndex;
        
        // Display the slide
        if (mainDisplayMaterial != null)
        {
            mainDisplayMaterial.mainTexture = presentationSlides[slideIndex];
            
            // Update a text on the secondary display to show current slide number
            UpdateSlideCounter();
        }
    }
    
    // Navigate to the next slide
    public void NextSlide()
    {
        int nextIndex = (currentSlideIndex + 1) % presentationSlides.Length;
        DisplaySlide(nextIndex);
    }
    
    // Navigate to the previous slide
    public void PreviousSlide()
    {
        int prevIndex = (currentSlideIndex - 1 + presentationSlides.Length) % presentationSlides.Length;
        DisplaySlide(prevIndex);
    }
    
    // Update the slide counter on the secondary display
    private void UpdateSlideCounter()
    {
        // In a real implementation, this would update text on the secondary display
        Debug.Log($"Slide {currentSlideIndex + 1} of {presentationSlides.Length}");
    }
    
    // Create a projector surface programmatically
    public SpatialProjectorSurface CreateProjectorSurface(Vector3 position, Vector2 size, bool showDots = true)
    {
        // Create a new GameObject
        GameObject surfaceObject = new GameObject("Projector Surface");
        surfaceObject.transform.position = position;
        
        // Add mesh components
        MeshFilter meshFilter = surfaceObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = surfaceObject.AddComponent<MeshRenderer>();
        
        // Create a simple quad mesh
        meshFilter.mesh = CreateQuadMesh();
        
        // Create a material
        Material material = new Material(Shader.Find("Standard"));
        material.color = Color.white;
        meshRenderer.material = material;
        
        // Add and configure the SpatialProjectorSurface component
        SpatialProjectorSurface projectorSurface = surfaceObject.AddComponent<SpatialProjectorSurface>();
        projectorSurface.size2D = size;
        projectorSurface.dotsVisible = showDots;
        
        // Set the rotation to face forward
        surfaceObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        
        Debug.Log($"Created new projector surface at {position} with size {size}");
        return projectorSurface;
    }
    
    // Helper method to create a simple quad mesh
    private Mesh CreateQuadMesh()
    {
        Mesh mesh = new Mesh();
        
        // Define vertices
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-0.5f, -0.5f, 0),
            new Vector3(0.5f, -0.5f, 0),
            new Vector3(-0.5f, 0.5f, 0),
            new Vector3(0.5f, 0.5f, 0)
        };
        
        // Define UVs
        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        
        // Define triangles
        int[] triangles = new int[6]
        {
            0, 2, 1,
            2, 3, 1
        };
        
        // Set mesh data
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }
}
```

## Best Practices

1. Size the projector surface appropriately for the content it will display, considering aspect ratios to avoid distortion.
2. Use the dotsVisible property during setup and development to help with alignment, but turn it off for the final experience.
3. Consider the viewing angles and position the projector surface where it can be easily seen by users.
4. For presentations or sequential content, create intuitive navigation controls near the projector surface.
5. Ensure the material used on the projector surface is suitable for displaying your content (e.g., non-reflective for better visibility).
6. When creating multiple projector surfaces, organize them logically in the scene to create a cohesive experience.
7. For larger displays, consider breaking them into multiple projector surfaces to maintain better texture resolution.
8. Test your projector surfaces in different lighting conditions to ensure visibility.
9. When updating content dynamically, ensure that new textures have the same dimensions as the original to maintain consistency.
10. If using projector surfaces for UI elements, ensure they are positioned at comfortable viewing and interaction distances.

## Common Use Cases

1. Digital signage and information displays in virtual environments.
2. Presentation screens for virtual meetings and classrooms.
3. Interactive museum exhibits displaying images and information.
4. Digital art galleries where artworks can be displayed and changed.
5. Control panels or dashboards displaying dynamic information.
6. Menu boards for virtual restaurants or establishments.
7. Interactive maps or wayfinding displays.
8. Video screens for watching content in virtual spaces.
9. Virtual whiteboards for collaborative work.
10. Customizable decorative elements that can display different patterns or designs.

## Completed: March 10, 2025