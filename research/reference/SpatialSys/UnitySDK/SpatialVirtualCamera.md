# SpatialVirtualCamera

Category: Core Components

Interface/Class/Enum: Class

The SpatialVirtualCamera component allows developers to override the default player camera in a Spatial environment, providing full control over camera position, rotation, and field of view. This component serves as a wrapper around Unity's Cinemachine system, enabling smooth camera transitions, custom viewpoints, and specialized camera behaviors without requiring direct access to the main camera.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| priority | int | Determines which virtual camera takes precedence when multiple cameras are active. Higher priority cameras override lower priority ones. |
| fieldOfView | float | The camera's field of view in degrees. Controls how wide or narrow the camera perspective is. |
| nearClipPlane | float | The nearest point relative to the camera where rendering occurs. Objects closer than this distance are not rendered. |
| farClipPlane | float | The farthest point relative to the camera where rendering occurs. Objects beyond this distance are not rendered. |
| documentationURL | string | URL to documentation for this component. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |

## Methods

| Method | Description |
| --- | --- |
| OnDrawGizmosSelected() | Draws camera visualization gizmos in the Scene view when the camera is selected. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    // References to different virtual cameras
    [SerializeField] private SpatialVirtualCamera firstPersonCamera;
    [SerializeField] private SpatialVirtualCamera thirdPersonCamera;
    [SerializeField] private SpatialVirtualCamera cinematicCamera;
    [SerializeField] private SpatialVirtualCamera selfieCamera;
    
    // Reference to interactive trigger zones
    [SerializeField] private SpatialTriggerEvent[] cameraZones;
    
    // Camera transition time
    private float transitionTime = 0.5f;
    
    // Currently active camera
    private SpatialVirtualCamera activeCamera;
    
    private void Start()
    {
        // Initialize camera system
        SetupCameras();
        
        // Set the default active camera
        activeCamera = thirdPersonCamera;
        
        // Set up camera zone triggers
        SetupCameraZones();
    }
    
    private void SetupCameras()
    {
        // Make sure all cameras have lower priority by default
        if (firstPersonCamera != null)
        {
            firstPersonCamera.priority = 10; // Same as default player camera
        }
        
        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.priority = 10; // Same as default player camera
        }
        
        if (cinematicCamera != null)
        {
            cinematicCamera.priority = 10; // Same as default player camera
        }
        
        if (selfieCamera != null)
        {
            selfieCamera.priority = 10; // Same as default player camera
            // Set a wider field of view for the selfie camera
            selfieCamera.fieldOfView = 70f;
        }
        
        // Set the transition blend time through the camera service
        SpatialBridge.cameraService.virtualCameraBlendTime = transitionTime;
    }
    
    private void SetupCameraZones()
    {
        if (cameraZones == null || cameraZones.Length == 0)
            return;
            
        // Set up camera zone triggers
        for (int i = 0; i < cameraZones.Length; i++)
        {
            if (cameraZones[i] == null)
                continue;
                
            // Determine which camera this zone should activate based on index
            SpatialVirtualCamera targetCamera = null;
            
            switch (i)
            {
                case 0:
                    targetCamera = firstPersonCamera;
                    break;
                case 1:
                    targetCamera = thirdPersonCamera;
                    break;
                case 2:
                    targetCamera = cinematicCamera;
                    break;
                case 3:
                    targetCamera = selfieCamera;
                    break;
                default:
                    targetCamera = thirdPersonCamera;
                    break;
            }
            
            if (targetCamera == null)
                continue;
                
            // Store the camera reference for the lambda capture
            SpatialVirtualCamera camera = targetCamera;
            
            // Set up entry trigger
            cameraZones[i].onEnterEvent.AddListener(() => {
                ActivateCamera(camera);
            });
            
            // Optional: Set up exit trigger to return to default camera
            cameraZones[i].onExitEvent.AddListener(() => {
                ActivateCamera(thirdPersonCamera);
            });
            
            Debug.Log($"Set up camera zone {i} for camera {targetCamera.name}");
        }
    }
    
    // Activate a specific camera
    public void ActivateCamera(SpatialVirtualCamera camera)
    {
        if (camera == null)
            return;
            
        // Make sure the new camera has higher priority than others
        SetAllCamerasPriority(10);
        camera.priority = 20;
        
        // Update the active camera reference
        activeCamera = camera;
        
        Debug.Log($"Activated camera: {camera.name}");
    }
    
    // Set all cameras to the same priority
    private void SetAllCamerasPriority(int priority)
    {
        if (firstPersonCamera != null) firstPersonCamera.priority = priority;
        if (thirdPersonCamera != null) thirdPersonCamera.priority = priority;
        if (cinematicCamera != null) cinematicCamera.priority = priority;
        if (selfieCamera != null) selfieCamera.priority = priority;
    }
    
    // Cinematic sequence that uses multiple cameras
    public IEnumerator PlayCinematicSequence()
    {
        Debug.Log("Starting cinematic sequence");
        
        // Activate the cinematic camera
        ActivateCamera(cinematicCamera);
        
        // Wait for a few seconds
        yield return new WaitForSeconds(3f);
        
        // Move to first-person perspective
        ActivateCamera(firstPersonCamera);
        
        // Wait for a few seconds
        yield return new WaitForSeconds(2f);
        
        // End with a selfie shot
        ActivateCamera(selfieCamera);
        
        // Wait for a few seconds
        yield return new WaitForSeconds(2f);
        
        // Return to the default third-person camera
        ActivateCamera(thirdPersonCamera);
        
        Debug.Log("Cinematic sequence completed");
    }
    
    // Add camera shake effect
    public void ShakeCamera(float intensity)
    {
        // Use the built-in camera shake functionality from the camera service
        SpatialBridge.cameraService.Shake(intensity);
        
        Debug.Log($"Applied camera shake with intensity: {intensity}");
    }
    
    // Create and set up a new virtual camera at runtime
    public SpatialVirtualCamera CreateVirtualCamera(string cameraName, Vector3 position, Vector3 lookAtPosition, float fieldOfView = 60f)
    {
        // Create a new game object for the camera
        GameObject cameraObject = new GameObject(cameraName);
        cameraObject.transform.position = position;
        
        // Look at the target position
        cameraObject.transform.LookAt(lookAtPosition);
        
        // Add the virtual camera component
        SpatialVirtualCamera virtualCamera = cameraObject.AddComponent<SpatialVirtualCamera>();
        
        // Configure the camera
        virtualCamera.fieldOfView = fieldOfView;
        virtualCamera.priority = 10; // Default priority
        
        Debug.Log($"Created new virtual camera: {cameraName} at position {position}");
        return virtualCamera;
    }
}
```

## Best Practices

1. Assign meaningful priorities to your virtual cameras to ensure the right camera is active at the right time. The default player camera has a priority of 10, so use values higher than 10 to override it.
2. Use the `virtualCameraBlendTime` property from the `ICameraService` to control the smoothness of transitions between cameras. Longer blend times create smoother transitions.
3. Organize virtual cameras in your scene hierarchy to keep track of different camera perspectives and their purposes.
4. Consider the purpose of each camera when setting field of view values. Wider fields of view (higher values) are good for environmental shots, while narrower fields of view (lower values) work well for focused or detail shots.
5. Be mindful of the near and far clip planes to ensure all relevant objects are rendered while optimizing performance.
6. When creating a side-scroller or specialized view, position the camera appropriately and use a camera controller script to maintain the desired perspective.
7. Use trigger zones with the SpatialTriggerEvent component to smoothly transition between different camera views based on player location.
8. For cinematics, create a sequence of camera activations with well-timed transitions rather than moving a single camera.
9. For first-person views, position the camera at the avatar's head level and set the field of view to around 60-70 degrees for a natural perspective.
10. When implementing specialized camera behaviors like follow or orbit, update the camera's transform in LateUpdate() rather than Update() to ensure smooth motion.

## Common Use Cases

1. **First-Person Perspective**: Position the camera at the player's head level to simulate seeing through their eyes.
2. **Cinematic Sequences**: Use multiple cameras with timed transitions to create dynamic cinematics for storytelling.
3. **Security Camera Views**: Create fixed-position cameras to view different areas of your space.
4. **Selfie Stations**: Implement a front-facing camera for users to take virtual selfies.
5. **Side-Scroller View**: Position the camera to create a classic 2D perspective in a 3D environment.
6. **Top-Down Perspective**: Place the camera above the action for a bird's-eye view.
7. **Over-the-Shoulder View**: Position slightly behind and above the player for a focused third-person perspective.
8. **Presentation Views**: Create cameras focused on presentation areas, stages, or exhibits.
9. **Tour Cameras**: Set up a sequence of cameras to guide users through a virtual tour of your space.
10. **Interactive Viewpoints**: Allow users to switch between different camera perspectives to view your environment from various angles.

## Completed: March 10, 2025