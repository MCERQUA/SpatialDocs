# SpatialThumbnailCamera

Category: Core Components

Interface/Class/Enum: Class

The SpatialThumbnailCamera component defines a camera view used to generate thumbnail images of a Spatial environment. This component allows developers to specify the exact perspective from which thumbnail images are captured. These thumbnails are used in various places throughout the Spatial platform, such as space listings, search results, and sharing previews, making this component crucial for creating an appealing first impression of your space.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| fieldOfView | float | The field of view of the thumbnail camera in degrees. Controls how wide or narrow the camera's perspective is. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |
| documentationURL | string | URL to documentation for this component. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections.Generic;

public class ThumbnailManager : MonoBehaviour
{
    // Reference to the thumbnail camera
    [SerializeField] private SpatialThumbnailCamera thumbnailCamera;
    
    // Key locations for thumbnail capture
    [SerializeField] private List<Transform> thumbnailPositions = new List<Transform>();
    
    // Key objects to ensure visibility in thumbnails
    [SerializeField] private List<GameObject> keyObjects = new List<GameObject>();
    
    // Visual settings
    [SerializeField] private Light[] thumbnailLights;
    [SerializeField] private GameObject[] visualEffects;
    
    private int currentThumbnailIndex = 0;
    
    private void Start()
    {
        // Find thumbnail camera if not assigned
        if (thumbnailCamera == null)
        {
            thumbnailCamera = FindObjectOfType<SpatialThumbnailCamera>();
            if (thumbnailCamera == null)
            {
                Debug.LogWarning("No SpatialThumbnailCamera found in the scene!");
            }
        }
        
        // Initialize thumbnail manager
        if (thumbnailCamera != null)
        {
            Debug.Log($"Thumbnail camera found with FOV: {thumbnailCamera.fieldOfView}");
        }
    }
    
    // Set up the thumbnail camera at a specific position
    public void PositionThumbnailCamera(Transform position)
    {
        if (thumbnailCamera == null || position == null)
            return;
            
        // Move the thumbnail camera to the specified position
        thumbnailCamera.transform.position = position.position;
        thumbnailCamera.transform.rotation = position.rotation;
        
        Debug.Log($"Positioned thumbnail camera at {position.name}");
    }
    
    // Cycle through predefined thumbnail positions
    public void CycleToNextThumbnailPosition()
    {
        if (thumbnailCamera == null || thumbnailPositions.Count == 0)
            return;
            
        // Move to next position in the list
        currentThumbnailIndex = (currentThumbnailIndex + 1) % thumbnailPositions.Count;
        Transform nextPosition = thumbnailPositions[currentThumbnailIndex];
        
        // Position the camera
        PositionThumbnailCamera(nextPosition);
    }
    
    // Adjust field of view for thumbnail camera
    public void AdjustThumbnailFieldOfView(float newFieldOfView)
    {
        if (thumbnailCamera == null)
            return;
            
        // Clamp FOV to reasonable values
        newFieldOfView = Mathf.Clamp(newFieldOfView, 30f, 90f);
        
        // Set the field of view
        thumbnailCamera.fieldOfView = newFieldOfView;
        
        Debug.Log($"Adjusted thumbnail camera FOV to {newFieldOfView}");
    }
    
    // Optimize the scene for thumbnail capture
    public void PrepareSceneForThumbnailCapture()
    {
        // Enable special lighting for thumbnail capture
        if (thumbnailLights != null)
        {
            foreach (Light light in thumbnailLights)
            {
                if (light != null)
                {
                    light.enabled = true;
                }
            }
        }
        
        // Enable visual effects for thumbnail capture
        if (visualEffects != null)
        {
            foreach (GameObject effect in visualEffects)
            {
                if (effect != null)
                {
                    effect.SetActive(true);
                }
            }
        }
        
        // Ensure key objects are visible
        EnsureKeyObjectsVisible();
        
        Debug.Log("Scene prepared for thumbnail capture");
    }
    
    // Restore normal scene settings after thumbnail capture
    public void RestoreNormalSceneSettings()
    {
        // Disable special thumbnail lighting
        if (thumbnailLights != null)
        {
            foreach (Light light in thumbnailLights)
            {
                if (light != null)
                {
                    light.enabled = false;
                }
            }
        }
        
        // Disable visual effects for thumbnail
        if (visualEffects != null)
        {
            foreach (GameObject effect in visualEffects)
            {
                if (effect != null)
                {
                    effect.SetActive(false);
                }
            }
        }
        
        Debug.Log("Restored normal scene settings");
    }
    
    // Ensure all key objects are visible in the thumbnail
    private void EnsureKeyObjectsVisible()
    {
        if (thumbnailCamera == null || keyObjects.Count == 0)
            return;
            
        // For this example, we'll just make sure objects are active
        // In a real implementation, you might want to adjust their positions
        // or the camera position to ensure they're in frame
        foreach (GameObject obj in keyObjects)
        {
            if (obj != null && !obj.activeSelf)
            {
                obj.SetActive(true);
                Debug.Log($"Activated key object for thumbnail: {obj.name}");
            }
        }
    }
    
    // Create a thumbnail camera programmatically
    public SpatialThumbnailCamera CreateThumbnailCamera(Vector3 position, Quaternion rotation, float fieldOfView = 60f)
    {
        // Create a new GameObject for the thumbnail camera
        GameObject cameraObject = new GameObject("ThumbnailCamera");
        cameraObject.transform.position = position;
        cameraObject.transform.rotation = rotation;
        
        // Add the SpatialThumbnailCamera component
        SpatialThumbnailCamera thumbnailCam = cameraObject.AddComponent<SpatialThumbnailCamera>();
        thumbnailCam.fieldOfView = fieldOfView;
        
        Debug.Log($"Created new thumbnail camera at position {position}");
        
        // If we already have a thumbnail camera, log a warning
        if (FindObjectsOfType<SpatialThumbnailCamera>().Length > 1)
        {
            Debug.LogWarning("Multiple thumbnail cameras detected in the scene. Only one will be used for thumbnail generation.");
        }
        
        return thumbnailCam;
    }
    
    // Calculate the best position for a thumbnail based on scene contents
    public Vector3 CalculateBestThumbnailPosition()
    {
        if (keyObjects.Count == 0)
            return Vector3.zero;
            
        // Calculate the center point of all key objects
        Vector3 center = Vector3.zero;
        foreach (GameObject obj in keyObjects)
        {
            if (obj != null)
            {
                center += obj.transform.position;
            }
        }
        center /= keyObjects.Count;
        
        // Calculate a position that's offset from the center
        // For this example, we'll position the camera 5 units away on the Z axis
        // and 2 units up on the Y axis, looking at the center point
        Vector3 cameraPosition = center + new Vector3(0, 2, -5);
        
        Debug.Log($"Calculated best thumbnail position at {cameraPosition}");
        return cameraPosition;
    }
    
    // Position thumbnail camera to frame specific objects
    public void FrameObjectsInThumbnail(GameObject[] objectsToFrame, float padding = 1.2f)
    {
        if (thumbnailCamera == null || objectsToFrame == null || objectsToFrame.Length == 0)
            return;
            
        // Calculate bounds that encompass all objects
        Bounds bounds = new Bounds();
        bool boundsInitialized = false;
        
        foreach (GameObject obj in objectsToFrame)
        {
            if (obj != null)
            {
                // Get all renderers in the object and its children
                Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
                
                foreach (Renderer renderer in renderers)
                {
                    if (!boundsInitialized)
                    {
                        bounds = renderer.bounds;
                        boundsInitialized = true;
                    }
                    else
                    {
                        bounds.Encapsulate(renderer.bounds);
                    }
                }
            }
        }
        
        if (!boundsInitialized)
            return;
            
        // Apply padding to the bounds
        bounds.Expand(bounds.size * (padding - 1.0f));
        
        // Calculate the camera position
        Vector3 cameraDirection = thumbnailCamera.transform.forward;
        float objectSize = bounds.size.magnitude;
        float distanceToObject = objectSize * 2.0f; // Adjust multiplier as needed
        
        Vector3 cameraPosition = bounds.center - cameraDirection * distanceToObject;
        
        // Position the camera
        thumbnailCamera.transform.position = cameraPosition;
        thumbnailCamera.transform.LookAt(bounds.center);
        
        Debug.Log($"Framed {objectsToFrame.Length} objects in thumbnail view");
    }
}
```

## Best Practices

1. Position the thumbnail camera to capture the most visually appealing and representative view of your space.
2. Include key landmarks or distinctive features in the thumbnail view to make your space recognizable.
3. Consider lighting carefully, as good lighting can dramatically improve the quality of the thumbnail.
4. Test your thumbnail view in the editor to ensure it captures the essence of your space.
5. Avoid placing the thumbnail camera too close to objects, which can result in clipping or distortion.
6. Frame the thumbnail view to include a balanced composition with foreground, middle ground, and background elements.
7. The field of view should be set to show appropriate context — not too narrow (which limits visibility) or too wide (which distorts perspective).
8. Consider creating multiple thumbnail cameras in different positions and choosing the best one for your final build.
9. Ensure that the thumbnail view doesn't include temporary, debug, or unfinished elements of your space.
10. Update your thumbnail camera position if you make significant changes to your space's appearance or layout.

## Common Use Cases

1. Creating an attractive preview for space listings in the Spatial directory.
2. Generating thumbnails for social media sharing and promotion of your space.
3. Providing preview images for search results within the Spatial platform.
4. Showcasing the most distinctive or attractive features of your environment.
5. Highlighting interactive elements or special features of your space.
6. Creating a first impression that accurately represents the style and theme of your space.
7. Showing off custom architecture, landscapes, or interior design.
8. Capturing dynamic elements like special effects or lighting in the thumbnail.
9. Highlighting branding elements for corporate or promotional spaces.
10. Providing consistent visual identity across multiple related spaces or experiences.

## Completed: March 10, 2025