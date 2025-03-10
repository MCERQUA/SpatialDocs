# XRCameraMode

Category: Camera Service Related

Enum: Defines the camera's point of view for a player in an XR headset, controlling how the user's physical movements relate to their avatar in the virtual world.

## Properties/Fields

| Value | Description |
| --- | --- |
| Default | This will maintain the currently set mode if player controlled switching modes is allowed. Otherwise, defaults to first person. This is useful when you want to respect user preferences while having a fallback. |
| FirstPerson | Player embodies their avatar. The avatar's head and hand movements map to the player's own physical movements, creating an immersive experience where the user feels they are inside their avatar. |
| ThirdPerson | Player's point of view is placed behind their avatar. The avatar's head and hand movements do not map to the player's own physical movements, creating a detached view similar to traditional third-person games. |

## Usage Examples

```csharp
// Example 1: Setting the XR camera mode
// This example shows how to change the XR camera mode and allow player switching

public class XRCameraController : MonoBehaviour
{
    private void Start()
    {
        // Allow players to switch between first and third person modes
        SpatialBridge.cameraService.allowPlayerToSwitchXRCameraMode = true;
        
        // Set the initial camera mode to first person
        SpatialBridge.cameraService.xrCameraMode = XRCameraMode.FirstPerson;
    }
    
    // Method to toggle between first and third person modes
    public void ToggleXRCameraMode()
    {
        if (SpatialBridge.cameraService.xrCameraMode == XRCameraMode.FirstPerson)
        {
            SpatialBridge.cameraService.xrCameraMode = XRCameraMode.ThirdPerson;
        }
        else
        {
            SpatialBridge.cameraService.xrCameraMode = XRCameraMode.FirstPerson;
        }
    }
}
```

```csharp
// Example 2: Setting XR camera mode based on the experience type
// This example shows how different experiences might require different camera modes

public class XRExperienceManager : MonoBehaviour
{
    [SerializeField] private bool isImmersiveExperience = true;
    
    private void Start()
    {
        // For immersive experiences, use first person and disable player switching
        if (isImmersiveExperience)
        {
            SpatialBridge.cameraService.allowPlayerToSwitchXRCameraMode = false;
            SpatialBridge.cameraService.xrCameraMode = XRCameraMode.FirstPerson;
        }
        // For avatar-focused experiences, allow player choice but default to third person
        else
        {
            SpatialBridge.cameraService.allowPlayerToSwitchXRCameraMode = true;
            SpatialBridge.cameraService.xrCameraMode = XRCameraMode.ThirdPerson;
        }
    }
    
    // Method to reset to default behavior
    public void ResetToDefaultMode()
    {
        SpatialBridge.cameraService.xrCameraMode = XRCameraMode.Default;
    }
}
```

## Best Practices

1. **Consider the experience type** - Choose the appropriate XR camera mode based on the type of experience you're creating. FirstPerson is better for immersive interaction, while ThirdPerson is better for avatar customization and social experiences.

2. **Respect user preferences** - When possible, set `allowPlayerToSwitchXRCameraMode` to true to let users choose their preferred view, as comfort in XR varies significantly between users.

3. **Provide visual feedback when changing modes** - When switching XR camera modes, provide visual or audio feedback to help users understand the change, especially if it affects controls.

4. **Test with actual XR hardware** - Different XR headsets may have slightly different behaviors with these camera modes, so test on target hardware.

5. **Be careful with rapid mode switching** - Avoid rapid or automatic switching between modes as it can be disorienting in XR. Allow time for users to adjust.

6. **Consider accessibility** - Some users may experience motion sickness in FirstPerson mode but be comfortable in ThirdPerson mode, or vice versa. Providing options improves accessibility.

## Common Use Cases

1. **Immersive Interaction** - Using FirstPerson mode for experiences that require precise hand interaction with virtual objects.

2. **Avatar Customization** - Using ThirdPerson mode to allow users to see and customize their avatar appearance.

3. **Social Spaces** - Using ThirdPerson mode in social spaces to allow users to see their own avatar in relation to others.

4. **Mixed Experiences** - Allowing users to switch between modes based on their current activity within the experience.

5. **Guided Tours** - Using FirstPerson for immersion in virtual environments during guided tours or educational experiences.

## Related Components

- [ICameraService](./ICameraService.md) - The main camera service interface that exposes the xrCameraMode property and allowPlayerToSwitchXRCameraMode property.
- [SpatialCameraMode](./SpatialCameraMode.md) - Defines the overall camera behavior mode.
- [SpatialCameraRotationMode](./SpatialCameraRotationMode.md) - Defines how camera rotation is controlled.
- [SpatialCameraPassthrough](./SpatialCameraPassthrough.md) - Component for mixed reality camera passthrough functionality.