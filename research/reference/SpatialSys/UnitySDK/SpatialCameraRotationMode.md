# SpatialCameraRotationMode

Category: Camera Service Related

Enum: Defines the method of rotation for the actor camera with respect to player inputs. Controls how the camera should respond to user interactions.

## Properties/Fields

| Value | Description |
| --- | --- |
| AutoRotate | Automatic rotation as player moves. The camera will automatically rotate to follow the direction of player movement. |
| DragToRotate | Left mouse button or touch rotation. The camera only rotates when the player specifically drags with the left mouse button or touch input. |
| PointerLock_Locked | Locked to the cursor. Note that this mode is meant to be used for desktop/mouse-based devices (such as Web). Browsers don't allow you to enter PointerLock mode without a user interaction, so this mode will show a UI element to the user to click on to enter PointerLock mode. |
| PointerLock_Unlocked | Cursor is unlocked, but next non-UI click returns to PointerLock. Allows temporary cursor freedom while maintaining easy return to camera control. |

## Usage Examples

```csharp
// Example 1: Setting the camera rotation mode
// This example shows how to change the camera rotation mode based on user preference

public class CameraSettingsManager : MonoBehaviour
{
    public void SetAutoRotateMode()
    {
        // Set the camera to automatically rotate as the player moves
        SpatialBridge.cameraService.rotationMode = SpatialCameraRotationMode.AutoRotate;
    }
    
    public void SetDragToRotateMode()
    {
        // Set the camera to only rotate when the player drags
        SpatialBridge.cameraService.rotationMode = SpatialCameraRotationMode.DragToRotate;
    }
    
    public void SetPointerLockMode()
    {
        // Set the camera to use pointer lock for rotation (good for desktop/FPS-style)
        SpatialBridge.cameraService.rotationMode = SpatialCameraRotationMode.PointerLock_Locked;
    }
}
```

```csharp
// Example 2: Dynamically switching camera rotation modes based on device
// This example shows how to adapt the camera rotation mode to different devices

public class DeviceAdaptiveCameraController : MonoBehaviour
{
    private void Start()
    {
        // Check if we're on a touch device
        if (Input.touchSupported && Input.multiTouchEnabled)
        {
            // Set drag-to-rotate for touch devices
            SpatialBridge.cameraService.rotationMode = SpatialCameraRotationMode.DragToRotate;
        }
        else
        {
            // Set pointer lock for desktop
            SpatialBridge.cameraService.rotationMode = SpatialCameraRotationMode.PointerLock_Locked;
        }
    }
    
    // Method to temporarily unlock the pointer (e.g., when opening a menu)
    public void TemporarilyUnlockPointer()
    {
        if (SpatialBridge.cameraService.rotationMode == SpatialCameraRotationMode.PointerLock_Locked)
        {
            SpatialBridge.cameraService.rotationMode = SpatialCameraRotationMode.PointerLock_Unlocked;
        }
    }
    
    // Method to re-lock the pointer (e.g., when closing a menu)
    public void RelockPointer()
    {
        if (SpatialBridge.cameraService.rotationMode == SpatialCameraRotationMode.PointerLock_Unlocked)
        {
            SpatialBridge.cameraService.rotationMode = SpatialCameraRotationMode.PointerLock_Locked;
        }
    }
}
```

## Best Practices

1. **Select rotation mode based on platform** - Choose the appropriate rotation mode based on the target platform. Use PointerLock for desktop, DragToRotate for mobile, and consider AutoRotate for casual experiences.

2. **Provide user options** - Allow users to choose their preferred rotation mode in settings, as different players may have different preferences or accessibility needs.

3. **Handle pointer lock UI appropriately** - When using PointerLock_Locked, ensure your UI is designed to guide users to click to activate pointer lock, as browsers require user interaction.

4. **Use PointerLock_Unlocked for UI interactions** - Switch to PointerLock_Unlocked temporarily when users need to interact with UI elements, then switch back to PointerLock_Locked for gameplay.

5. **Consider mobile usability** - For touch devices, DragToRotate is usually more intuitive, but ensure the drag area is large enough for comfortable interaction.

6. **Test all modes with users** - Different rotation modes can significantly impact usability and comfort, so test all modes with real users on target platforms.

## Common Use Cases

1. **First-Person Shooters** - Using PointerLock_Locked to provide accurate and responsive camera control for aiming and looking around.

2. **Mobile AR Experiences** - Using DragToRotate to allow users to look around their environment with intuitive touch controls.

3. **Casual Exploration Games** - Using AutoRotate to simplify camera controls, allowing players to focus on movement rather than camera management.

4. **Menu Interactions** - Temporarily switching to PointerLock_Unlocked to allow players to interact with UI elements before returning to locked camera controls.

5. **Multi-platform Games** - Dynamically switching between rotation modes based on the detected platform and input methods available.

## Related Components

- [ICameraService](./ICameraService.md) - The main camera service interface that exposes the rotationMode property.
- [SpatialCameraMode](./SpatialCameraMode.md) - Defines the overall camera behavior mode.
- [XRCameraMode](./XRCameraMode.md) - Defines camera modes specific to XR experiences.
- [SpatialVirtualCamera](./SpatialVirtualCamera.md) - Component used to create custom virtual cameras in the scene.