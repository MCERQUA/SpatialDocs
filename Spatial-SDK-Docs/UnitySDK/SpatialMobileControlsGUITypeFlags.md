# SpatialMobileControlsGUITypeFlags

Category: Core GUI Service Related

Enum: Provides flags for controlling mobile-specific GUI elements.

## Overview
SpatialMobileControlsGUITypeFlags is a flags enum that defines the different types of mobile control GUI elements that can be enabled or disabled in the Spatial platform. This enum is specifically designed for mobile platforms and allows developers to customize the mobile control experience by enabling or disabling specific control elements.

## Properties/Fields

| Value | Description |
| --- | --- |
| None | No mobile control GUI elements. Used to disable all mobile controls. |
| AvatarMoveControls | Controls for moving the avatar on mobile devices (typically virtual joystick). |
| AvatarJumpButton | The jump button displayed on mobile devices. |
| All | All mobile control GUI elements combined. Used to enable or disable all mobile controls at once. |

## Usage Examples

```csharp
// Example 1: Customizing mobile controls based on game state
// This example shows how to enable/disable specific mobile controls for different scenarios

public class MobileControlsManager : MonoBehaviour
{
    public void SetupStandardControls()
    {
        // Enable standard movement and jump controls
        SpatialBridge.coreGUIService.SetMobileControlsGUIEnabled(
            SpatialMobileControlsGUITypeFlags.AvatarMoveControls | 
            SpatialMobileControlsGUITypeFlags.AvatarJumpButton, 
            true);
    }
    
    public void SetupFlightMode()
    {
        // In flight mode, disable jump button but keep movement controls
        SpatialBridge.coreGUIService.SetMobileControlsGUIEnabled(
            SpatialMobileControlsGUITypeFlags.AvatarJumpButton, 
            false);
        
        SpatialBridge.coreGUIService.SetMobileControlsGUIEnabled(
            SpatialMobileControlsGUITypeFlags.AvatarMoveControls, 
            true);
    }
    
    public void EnterCutscene()
    {
        // During cutscenes, disable all mobile controls
        SpatialBridge.coreGUIService.SetMobileControlsGUIEnabled(
            SpatialMobileControlsGUITypeFlags.All, 
            false);
    }
    
    public void ExitCutscene()
    {
        // After cutscene, restore all mobile controls
        SpatialBridge.coreGUIService.SetMobileControlsGUIEnabled(
            SpatialMobileControlsGUITypeFlags.All, 
            true);
    }
}
```

```csharp
// Example 2: Creating custom control schemes for different game modes
// This example demonstrates implementing different control schemes

public class GameModeController : MonoBehaviour
{
    [SerializeField] private bool isMovementEnabled = true;
    [SerializeField] private bool isJumpEnabled = true;
    
    private void Start()
    {
        // Apply initial control settings
        UpdateMobileControls();
    }
    
    public void SetExplorationMode()
    {
        // Full controls for exploration
        isMovementEnabled = true;
        isJumpEnabled = true;
        UpdateMobileControls();
    }
    
    public void SetPuzzleMode()
    {
        // During puzzles, movement is allowed but jumping is disabled
        isMovementEnabled = true;
        isJumpEnabled = false;
        UpdateMobileControls();
    }
    
    public void SetDialogueMode()
    {
        // During dialogue, all movement controls are disabled
        isMovementEnabled = false;
        isJumpEnabled = false;
        UpdateMobileControls();
    }
    
    private void UpdateMobileControls()
    {
        // Create the control flags based on current settings
        SpatialMobileControlsGUITypeFlags controlFlags = SpatialMobileControlsGUITypeFlags.None;
        
        if (isMovementEnabled)
        {
            controlFlags |= SpatialMobileControlsGUITypeFlags.AvatarMoveControls;
        }
        
        if (isJumpEnabled)
        {
            controlFlags |= SpatialMobileControlsGUITypeFlags.AvatarJumpButton;
        }
        
        // Disable all controls first
        SpatialBridge.coreGUIService.SetMobileControlsGUIEnabled(
            SpatialMobileControlsGUITypeFlags.All, 
            false);
        
        // Then enable only the ones we want
        if (controlFlags != SpatialMobileControlsGUITypeFlags.None)
        {
            SpatialBridge.coreGUIService.SetMobileControlsGUIEnabled(controlFlags, true);
        }
    }
}
```

## Best Practices

1. **Consider platform differences** - Remember that mobile controls are only visible on mobile platforms. Your code should handle both mobile and non-mobile platforms gracefully.

2. **Match controls to gameplay** - Enable only the controls that make sense for the current gameplay context. For example, disable jump controls when the player is in a menu or dialog.

3. **Combine with other GUI management** - Coordinate mobile control changes with other GUI element changes for a consistent user experience across the entire interface.

4. **Test on actual devices** - Always test your mobile control configurations on real mobile devices to ensure they provide a good user experience.

5. **Provide feedback when disabling controls** - When disabling controls, consider providing visual or audio feedback to help users understand why the controls are unavailable.

6. **Consider custom control overlays** - If the standard mobile controls don't meet your needs, you can disable them and implement custom control overlays.

## Common Use Cases

1. **Contextual controls** - Enabling or disabling specific controls based on the current game context (e.g., disabling jump in swimming areas).

2. **Cinematic sequences** - Disabling all mobile controls during cutscenes or scripted sequences.

3. **Tutorial guidance** - Progressively enabling controls as users learn game mechanics during tutorials.

4. **UI focus** - Disabling movement controls when users are interacting with menus or other UI elements.

5. **Accessibility options** - Allowing users to customize which mobile controls are displayed based on their preferences.

6. **Game mode transitions** - Changing available controls when transitioning between different game modes or activities.

7. **Vehicle or special movement modes** - Customizing controls when players enter vehicles or special movement modes.

## Related Components

- [ICoreGUIService](./ICoreGUIService.md) - The service that provides the SetMobileControlsGUIEnabled method for controlling mobile GUI elements.
- [SpatialCoreGUITypeFlags](./SpatialCoreGUITypeFlags.md) - Similar flags enum for standard GUI elements (not mobile-specific).
- [SpatialCoreGUIType](./SpatialCoreGUIType.md) - Enum defining the different types of core GUI elements.
- [SpatialCoreGUIState](./SpatialCoreGUIState.md) - Defines the possible states for GUI elements.