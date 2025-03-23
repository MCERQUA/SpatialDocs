# Camera Modes Template

## Overview
The Camera Modes template showcases various camera views and perspectives that can be implemented in Spatial using the SpatialVirtualCamera component. It provides ready-to-use implementations of popular camera perspectives including first-person, selfie, side-scroller, top-down, over-the-shoulder, and close-up views. The template is available in both C# and Visual Scripting implementations.

## Features
- **Multiple Camera Perspectives**: Seven different camera modes in one template
- **Dual Implementation**: Available in both C# and Visual Scripting
- **Easy Mode Switching**: Simple system for switching between camera modes
- **Customizable Parameters**: Adjustable settings for each camera mode
- **Event-Based Switching**: Camera transitions triggered by events or player input
- **Performance Optimized**: Designed for web performance
- **Mobile Compatible**: Works on mobile devices

## Included Camera Modes

### 1. First Person
Provides an immersive view directly through the avatar's eyes.
- Positions camera at avatar's head position
- Follows avatar rotation directly
- Provides settings for head offset and rotation constraints

### 2. Selfie Station
Allows players to take in-game selfies and capture memorable moments.
- Front-facing camera that shows the avatar
- Includes UI for triggering photo capture
- Configurable camera distance and angle
- Optional "selfie stick" visual element

### 3. Camera Shake/Wobble
Demonstrates how to create dynamic camera movement for action sequences.
- Procedural camera shaking effect
- Adjustable intensity and frequency
- Trigger-based activation 
- Smooth transition in/out of shake effect

### 4. Side-Scroller
Implements a classic 2D-style perspective in a 3D world.
- Fixed position on one axis (typically Z)
- Follows player movement on horizontal and vertical axes
- Configurable dead zone for smoother following
- Optional parallax effect for background elements

### 5. Over-the-Shoulder
Provides a focused third-person perspective similar to modern action games.
- Camera positioned behind and slightly above the avatar
- Offset to one side (configurable left or right)
- Adjustable distance and height
- Look target adjustment for aiming/focus

### 6. Top-Down
Offers a bird's eye perspective on the environment.
- Camera positioned directly above avatar
- Follows avatar movement with configurable smoothing
- Adjustable height and rotation
- Optional slight tilt for better depth perception

### 7. Close-Up
Provides a magnified view for detailed examination of objects or environments.
- Zoomed perspective with narrow field of view
- Focus target system for looking at specific objects
- Depth of field effect option
- Smooth transition between normal and close-up view

## Integration with SDK Components
The template integrates with these key SDK components:

| SDK Component | Usage in Template |
|---------------|-------------------|
| ICameraService | Core service for camera management |
| SpatialVirtualCamera | Primary component for custom camera views |
| SpatialCameraMode | Enum for built-in camera modes |
| IInputService | Used for camera input handling |
| SpatialTriggerEvent | For trigger-based camera switching zones |

## When to Use
Use this template when:
- You need custom camera perspectives beyond Spatial's defaults
- Creating games that require specific camera angles (platformers, top-down games)
- Building experiences where camera perspective enhances gameplay
- Implementing photography features in your space
- Creating cinematic sequences or guided tours

## Implementation Details

### Camera Mode Switching System
The template includes a camera manager script that handles switching between different camera modes:

```csharp
public class CameraModeManager : MonoBehaviour
{
    [SerializeField] private List<SpatialVirtualCamera> cameraModes = new List<SpatialVirtualCamera>();
    private int activeCameraIndex = 0;
    
    public void SwitchToNextCamera()
    {
        // Deactivate current camera
        cameraModes[activeCameraIndex].gameObject.SetActive(false);
        
        // Update index
        activeCameraIndex = (activeCameraIndex + 1) % cameraModes.Count;
        
        // Activate new camera
        cameraModes[activeCameraIndex].gameObject.SetActive(true);
        SpatialBridge.cameraService.SetVirtualCamera(cameraModes[activeCameraIndex]);
    }
    
    public void SwitchToCamera(int index)
    {
        if (index < 0 || index >= cameraModes.Count)
            return;
            
        // Deactivate current camera
        cameraModes[activeCameraIndex].gameObject.SetActive(false);
        
        // Update index
        activeCameraIndex = index;
        
        // Activate new camera
        cameraModes[activeCameraIndex].gameObject.SetActive(true);
        SpatialBridge.cameraService.SetVirtualCamera(cameraModes[activeCameraIndex]);
    }
}
```

### First Person Camera Setup
Example of how the first-person camera is configured:

```csharp
public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] private SpatialVirtualCamera virtualCamera;
    [SerializeField] private Transform headTarget;
    [SerializeField] private float headHeight = 1.7f;
    
    private void Start()
    {
        if (headTarget == null)
        {
            // Create a target at the avatar's head position
            headTarget = new GameObject("Head Target").transform;
            headTarget.SetParent(transform);
            headTarget.localPosition = new Vector3(0, headHeight, 0);
        }
        
        // Configure the virtual camera
        virtualCamera.followTarget = headTarget;
        virtualCamera.lookAtTarget = null; // First person doesn't use look target
        virtualCamera.followOffset = Vector3.zero;
        virtualCamera.rotationType = SpatialCameraRotationMode.FollowTargetRotation;
    }
}
```

### Selfie Camera Implementation
Example of how the selfie camera is implemented:

```csharp
public class SelfieCamera : MonoBehaviour
{
    [SerializeField] private SpatialVirtualCamera virtualCamera;
    [SerializeField] private float cameraDistance = 1.5f;
    [SerializeField] private float cameraHeight = 0.2f;
    
    private Transform avatarTransform;
    
    private void Start()
    {
        // Get reference to local avatar
        if (SpatialBridge.actorService.localActor.hasAvatar)
        {
            avatarTransform = SpatialBridge.actorService.localActor.avatar.transform;
            
            // Configure the virtual camera
            virtualCamera.followTarget = avatarTransform;
            virtualCamera.lookAtTarget = avatarTransform;
            virtualCamera.followOffset = new Vector3(0, cameraHeight, -cameraDistance);
            virtualCamera.rotationType = SpatialCameraRotationMode.LookAtTarget;
        }
    }
    
    public void TakeSelfie()
    {
        // In a real implementation, this would capture the screen
        // and save it or share it
        Debug.Log("Selfie taken!");
    }
}
```

## Visual Scripting Implementation
The template includes a complete Visual Scripting implementation with:

- State machine for managing camera modes
- Events for camera switching
- Configurable variable nodes for camera parameters
- Example camera trigger zones

## Best Practices
- **Smooth Transitions**: Implement smooth transitions between camera modes
- **Input Feedback**: Provide clear UI/feedback when camera modes change
- **Mobile Considerations**: Adjust camera sensitivity for mobile input
- **Performance**: Disable unused camera features when not active
- **Customize Parameters**: Adjust camera settings based on your specific space

## Related Templates
- [Cinemachine](./Cinemachine.md) - For integration with Unity's Cinemachine system
- [Avatar Input Control](../Technical/AvatarInputControl.md) - For custom avatar control to pair with camera modes

## Additional Resources
- [GitHub Repository](https://github.com/spatialsys/spatial-example-camera-modes)
- [Live Demo](https://www.spatial.io/s/Camera-Modes-Sample-Scene-65f1c03f27709aee7203421b?share=2684842950468053389)
- [Spatial Camera Documentation](../../SpatialSys/UnitySDK/ICameraService.md)
- [SpatialVirtualCamera Documentation](../../SpatialSys/UnitySDK/SpatialVirtualCamera.md)
