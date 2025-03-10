# SpatialCameraMode

Category: Camera Service Related

Enum: Defines the available camera modes for controlling how the camera behaves based on different scenarios.

## Properties/Fields

| Value | Description |
| --- | --- |
| Actor | The default player/local actor camera. This camera mode follows the player actor providing a standard third-person or first-person view based on zoom settings. |
| Vehicle | The special camera mode for vehicles. The camera will rotate with the vehicle's movement, providing a more immersive driving experience. |

## Usage Examples

```csharp
// Example 1: Setting a target override with specific camera mode
// This example shows how to temporarily override the camera to follow a specific target
// using the Actor camera mode

public class CameraTargetExample : MonoBehaviour
{
    [SerializeField] private Transform targetToFollow;
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger
        if (other.CompareTag("Player"))
        {
            // Override the camera to follow our target using the Actor camera mode
            SpatialBridge.cameraService.SetTargetOverride(targetToFollow, SpatialCameraMode.Actor);
            
            // Set a timer to return to normal camera behavior after 5 seconds
            StartCoroutine(ResetCameraAfterDelay(5f));
        }
    }
    
    private IEnumerator ResetCameraAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpatialBridge.cameraService.ClearTargetOverride();
    }
}
```

```csharp
// Example 2: Using Vehicle camera mode when a player enters a vehicle
// This example demonstrates switching to Vehicle camera mode when a player enters a vehicle

public class VehicleController : MonoBehaviour
{
    [SerializeField] private Transform vehicleTransform;
    
    public void OnVehicleEntered()
    {
        // Switch to vehicle camera mode when the player enters the vehicle
        SpatialBridge.cameraService.SetTargetOverride(vehicleTransform, SpatialCameraMode.Vehicle);
    }
    
    public void OnVehicleExited()
    {
        // Clear the camera override when the player exits the vehicle
        SpatialBridge.cameraService.ClearTargetOverride();
    }
}
```

## Best Practices

1. **Use Actor mode as the default** - The Actor camera mode should be the default for most user experiences when interacting with the world in Spatial.

2. **Switch to Vehicle mode for any moving platform** - Use Vehicle mode not just for cars or vehicles, but for any moving platform where you want the camera to rotate with movement, like moving platforms, boats, or even flying objects.

3. **Carefully manage transitions between camera modes** - Use the `virtualCameraBlendTime` property of ICameraService to ensure smooth transitions between different camera modes.

4. **Always clear overrides when finished** - Always call `ClearTargetOverride()` when you're done with a camera override to return control to the player and avoid unexpected camera behavior.

5. **Consider user preferences** - Some users may experience motion sickness with certain camera behaviors, especially in Vehicle mode. Consider providing user settings to adjust camera sensitivity.

6. **Use SetTargetOverride consistently** - When switching camera modes, always use the `SetTargetOverride` method rather than trying to manually manipulate the camera transform directly.

## Common Use Cases

1. **Standard Player Navigation** - The Actor mode is used for standard navigation through the world where the player controls their own character.

2. **Driving Vehicles** - Vehicle mode is used when players enter and control vehicles, ensuring the camera rotates appropriately with the vehicle's movement.

3. **Cinematic Sequences** - Camera modes can be temporarily overridden to focus on specific targets during cinematic sequences or cutscenes.

4. **Following Moving Platforms** - When players are on moving platforms that change orientation, using Vehicle mode can provide a more natural camera experience.

5. **Camera Transitions** - Using different camera modes during transitions between different areas or experiences to provide visual cues to the player.

## Related Components

- [ICameraService](./ICameraService.md) - The main camera service interface that provides methods to set and clear camera target overrides.
- [SpatialCameraRotationMode](./SpatialCameraRotationMode.md) - Defines how camera rotation is controlled.
- [XRCameraMode](./XRCameraMode.md) - Defines camera modes specific to XR experiences.
- [SpatialVirtualCamera](./SpatialVirtualCamera.md) - Component used to create custom virtual cameras in the scene.