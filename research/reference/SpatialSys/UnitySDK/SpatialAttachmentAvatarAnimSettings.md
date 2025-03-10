# SpatialAttachmentAvatarAnimSettings

Category: Core Components

Type: Class

`SpatialAttachmentAvatarAnimSettings` is a class that manages animation settings for avatar attachments in the Spatial SDK. It provides a collection of properties corresponding to different avatar animation states, allowing developers to configure how an attachment behaves during each animation.

## Properties/Fields

| Property | Description |
| --- | --- |
| idle | Configuration for the default idle animation when the avatar is stationary. Returns AttachmentAvatarAnimConfig. |
| walk | Configuration for the walking animation. Returns AttachmentAvatarAnimConfig. |
| jog | Configuration for the jogging animation. Returns AttachmentAvatarAnimConfig. |
| run | Configuration for the running animation. Returns AttachmentAvatarAnimConfig. |
| fall | Configuration for the falling animation. Returns AttachmentAvatarAnimConfig. |
| jumpStartIdle | Configuration for the jump start from idle position animation. Returns AttachmentAvatarAnimConfig. |
| jumpStartMoving | Configuration for the jump start while moving animation. Returns AttachmentAvatarAnimConfig. |
| jumpInAir | Configuration for the in-air jump animation. Returns AttachmentAvatarAnimConfig. |
| jumpLandStanding | Configuration for the landing from jump to standing position animation. Returns AttachmentAvatarAnimConfig. |
| jumpLandWalking | Configuration for the landing from jump to walking animation. Returns AttachmentAvatarAnimConfig. |
| jumpLandRunning | Configuration for the landing from jump to running animation. Returns AttachmentAvatarAnimConfig. |
| jumpLandHigh | Configuration for the landing from a high jump animation. Returns AttachmentAvatarAnimConfig. |
| jumpMultiple | Configuration for the multiple jumps in succession animation. Returns AttachmentAvatarAnimConfig. |
| climbIdle | Configuration for the idle while climbing animation. Returns AttachmentAvatarAnimConfig. |
| climbUp | Configuration for the climbing upward animation. Returns AttachmentAvatarAnimConfig. |
| climbEndTop | Configuration for the reaching the top of a climb animation. Returns AttachmentAvatarAnimConfig. |
| sit | Configuration for the sitting animation. Returns AttachmentAvatarAnimConfig. |
| emote | Configuration for the emote animations. Returns AttachmentAvatarAnimConfig. |
| lookup | Configuration for the looking up animation. Returns AttachmentAvatarAnimConfig. |
| customActions | Configuration for custom action animations. Returns AttachmentAvatarAnimConfig. |

## Methods

| Method | Description |
| --- | --- |
| AllSettings() | Returns an enumerable collection of all animation settings as tuples of (string name, AvatarAnimationClipType type, AttachmentAvatarAnimConfig config). Useful for iterating through all animation settings. |
| Init() | Initializes the animation settings with default values. |

## Usage Examples

### Basic Attachment Animation Configuration

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class CustomAttachmentController : MonoBehaviour
{
    // Reference to your attachment component
    public SpatialAvatarAttachment avatarAttachment;
    
    private void Start()
    {
        // Get access to the attachment's animation settings
        SpatialAttachmentAvatarAnimSettings animSettings = avatarAttachment.GetAvatarAnimSettings();
        
        // Configure the run animation to hide the attachment when running
        animSettings.run.attachmentVisible = false;
        
        // Configure the idle animation to show the attachment and use a custom animation clip
        animSettings.idle.attachmentVisible = true;
        animSettings.idle.overrideClip = GetCustomIdleAnimation();
    }
    
    private AnimationClip GetCustomIdleAnimation()
    {
        // Return your custom animation clip for idle state
        return Resources.Load<AnimationClip>("Animations/CustomIdleAnimation");
    }
}
```

### Iterating Through All Animation Settings

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class AttachmentAnimationManager : MonoBehaviour
{
    public SpatialAvatarAttachment avatarAttachment;
    
    private void ConfigureAttachmentAnimations()
    {
        SpatialAttachmentAvatarAnimSettings animSettings = avatarAttachment.GetAvatarAnimSettings();
        
        // Process all animation settings in a loop
        foreach (var (name, clipType, config) in animSettings.AllSettings())
        {
            // Make the attachment visible for all animations
            config.attachmentVisible = true;
            
            // Disable IK for jumping animations
            if (name.Contains("jump"))
            {
                config.disableIK = true;
            }
            
            Debug.Log($"Configured animation: {name}, ClipType: {clipType}");
        }
    }
}
```

### Setting Gender-Specific Animation Overrides

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class GenderSpecificAttachmentAnimations : MonoBehaviour
{
    public SpatialAvatarAttachment avatarAttachment;
    public AnimationClip femaleRunAnimation;
    public AnimationClip maleRunAnimation;
    
    private void ConfigureGenderSpecificAnimations()
    {
        SpatialAttachmentAvatarAnimSettings animSettings = avatarAttachment.GetAvatarAnimSettings();
        
        // Set different animation clips based on gender
        animSettings.run.overrideClip = femaleRunAnimation;      // Default/feminine animation
        animSettings.run.overrideClipMale = maleRunAnimation;    // Masculine animation
    }
}
```

## Best Practices

1. **Initialize settings before configuring**: Ensure the animation settings are properly initialized before making changes to individual animations.

2. **Configure visibility carefully**: Use the `attachmentVisible` property to control when attachments appear or disappear based on animation states. This can prevent clipping and improve visual quality.

3. **Use gender-specific overrides when appropriate**: For attachments that need different animations based on the avatar's gender, utilize both `overrideClip` (feminine) and `overrideClipMale` (masculine) properties.

4. **Respect avatar animations**: When creating custom animations for attachments, ensure they work well with the corresponding avatar animations to prevent visual disconnects.

5. **Use IK disabling selectively**: The `disableIK` property can be used to prevent Inverse Kinematics from affecting certain animations, but use it only when necessary to avoid unnatural movement.

## Common Use Cases

1. **Clothing attachments**: Configure clothing items to move naturally with the avatar during different animations.

2. **Handheld items**: Implement proper positioning and animation of tools, weapons, or other items held by the avatar.

3. **Backpacks and accessories**: Ensure accessories like backpacks or wings behave appropriately during movement and interactions.

4. **Emote-specific attachments**: Create attachments that only appear during specific emotes or custom actions.

5. **Animation-reactive items**: Design attachments that change behavior or appearance based on the avatar's current animation state.

## Related Components

- [SpatialAvatarAttachment](./SpatialAvatarAttachment.md): Component for attaching objects to avatars.
- [AttachmentAvatarAnimConfig](./AttachmentAvatarAnimConfig.md): Configuration class for individual attachment animations.
- [AvatarAnimationClipType](./AvatarAnimationClipType.md): Enum defining different avatar animation types.
- [SpatialAvatar](./SpatialAvatar.md): Main component representing an avatar in Spatial.
- [SpatialAvatarAnimation](./SpatialAvatarAnimation.md): Component that handles avatar animations.