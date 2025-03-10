# SpatialEmptyFrame

Category: Core Components

Interface/Class/Enum: Class

The SpatialEmptyFrame component creates an invisible reference frame with a defined size. This is useful for organizing space, defining boundaries, or creating reference volumes without visible geometry.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| size | Vector3 | The size of the frame in local space (width, height, depth). |
| documentationURL | string | URL to documentation for this component. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;

public class SpaceOrganizer : MonoBehaviour
{
    [SerializeField] private float roomWidth = 5f;
    [SerializeField] private float roomHeight = 3f;
    [SerializeField] private float roomDepth = 5f;
    
    [SerializeField] private bool showDebugVisuals = true;
    
    private void Start()
    {
        // Create room frame
        CreateRoomFrame();
        
        // Create trigger volume using empty frame
        CreateTriggerVolume(new Vector3(10, 0, 0), new Vector3(3, 2, 3));
    }
    
    private GameObject CreateRoomFrame()
    {
        GameObject roomFrame = new GameObject("Room_Frame");
        roomFrame.transform.parent = transform;
        roomFrame.transform.localPosition = Vector3.zero;
        
        // Add empty frame component and set size
        SpatialEmptyFrame frame = roomFrame.AddComponent<SpatialEmptyFrame>();
        frame.size = new Vector3(roomWidth, roomHeight, roomDepth);
        
        if (showDebugVisuals)
        {
            // Create visual representation for debugging
            GameObject visual = new GameObject("Frame_Visual");
            visual.transform.parent = roomFrame.transform;
            visual.transform.localPosition = Vector3.zero;
            
            // Add wireframe cube for visualization only
            LineRenderer lineRenderer = visual.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.positionCount = 16; // Corners + connections for a cube
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.yellow;
            lineRenderer.endColor = Color.yellow;
            
            // Draw cube outline
            Vector3 extents = frame.size * 0.5f;
            lineRenderer.SetPositions(new Vector3[] {
                new Vector3(-extents.x, -extents.y, -extents.z),
                new Vector3(extents.x, -extents.y, -extents.z),
                new Vector3(extents.x, -extents.y, extents.z),
                new Vector3(-extents.x, -extents.y, extents.z),
                new Vector3(-extents.x, -extents.y, -extents.z),
                new Vector3(-extents.x, extents.y, -extents.z),
                new Vector3(extents.x, extents.y, -extents.z),
                new Vector3(extents.x, -extents.y, -extents.z),
                new Vector3(extents.x, extents.y, -extents.z),
                new Vector3(extents.x, extents.y, extents.z),
                new Vector3(extents.x, -extents.y, extents.z),
                new Vector3(extents.x, extents.y, extents.z),
                new Vector3(-extents.x, extents.y, extents.z),
                new Vector3(-extents.x, -extents.y, extents.z),
                new Vector3(-extents.x, extents.y, extents.z),
                new Vector3(-extents.x, extents.y, -extents.z)
            });
        }
        
        Debug.Log($"Created room frame with dimensions: {roomWidth} x {roomHeight} x {roomDepth}");
        return roomFrame;
    }
    
    private GameObject CreateTriggerVolume(Vector3 position, Vector3 size)
    {
        GameObject triggerVolume = new GameObject("Trigger_Volume");
        triggerVolume.transform.parent = transform;
        triggerVolume.transform.position = position;
        
        // Add empty frame to define the size
        SpatialEmptyFrame frame = triggerVolume.AddComponent<SpatialEmptyFrame>();
        frame.size = size;
        
        // Add box collider using the same size
        BoxCollider collider = triggerVolume.AddComponent<BoxCollider>();
        collider.size = size;
        collider.isTrigger = true;
        
        // Add simple trigger script
        triggerVolume.AddComponent<TriggerDetector>();
        
        Debug.Log($"Created trigger volume at {position} with size {size}");
        return triggerVolume;
    }
}

// Simple class to detect triggers
public class TriggerDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Object entered trigger volume: {other.name}");
    }
    
    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"Object exited trigger volume: {other.name}");
    }
}
```

## Best Practices

1. Use SpatialEmptyFrame to create logical divisions or boundaries in your space without visual clutter.
2. When using for organizational purposes, consider adding visual debug tools that can be disabled in production.
3. Combine with colliders when you need to create invisible trigger zones or boundaries.
4. When creating a hierarchy of spaces, use nested SpatialEmptyFrames to clearly define parent-child relationships.
5. Remember that the frame has no physical or visual presence in the final experience unless you add additional components.
6. Use meaningful names for GameObjects with SpatialEmptyFrame to maintain clarity in complex scenes.

## Common Use Cases

1. Defining room boundaries or layout zones without visible geometry.
2. Creating invisible trigger volumes for event activation.
3. Setting up organizational structure for complex space layouts.
4. Establishing spatial reference for dynamically positioned objects.
5. Creating volume-based spatial queries (when combined with colliders).
6. Planning and prototyping space layouts before adding visual content.
7. Defining spawn areas or regions for procedural content placement.

## Completed: March 09, 2025