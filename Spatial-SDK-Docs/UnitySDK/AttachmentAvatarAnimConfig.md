# AttachmentAvatarAnimConfig

Category: Other Classes

Class: Animation Configuration

## Overview
The `AttachmentAvatarAnimConfig` class is a configuration container used to define how avatar attachments interact with animation clips. It provides settings to determine which animations to override, whether to show or hide the attachment during specific animations, and whether to disable IK (Inverse Kinematics) for certain animations. This class is particularly important for creating attachments that modify avatar behaviors when equipped.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| attachmentVisible | bool | Determines whether the attachment should be visible during the specified animation. When set to false, the attachment will be hidden while this animation plays. |
| clipType | AvatarAnimationClipType | Specifies which standard avatar animation type this configuration applies to (e.g., Idle, Walk, Run, Jump, etc.). |
| disableIK | bool | When set to true, disables Inverse Kinematics during this animation, preventing the avatar's hands and feet from adapting to the environment while the animation plays. |
| overrideClip | AnimationClip | The animation clip to use as an override for the specified clipType. This is the default clip used for all avatar body types. |
| overrideClipMale | AnimationClip | An optional animation clip specifically for male avatars. If provided, this clip will be used instead of overrideClip when the attachment is equipped on a male avatar. |

## Usage Example

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections.Generic;

public class AttachmentAnimationConfigExample : MonoBehaviour
{
    [SerializeField] private SpatialAvatarAttachment attachment;
    [SerializeField] private AnimationClip customIdleAnimation;
    [SerializeField] private AnimationClip customRunAnimation;
    [SerializeField] private AnimationClip customMaleIdleAnimation; // Gender-specific animation
    
    private void Start()
    {
        if (attachment != null)
        {
            ConfigureAttachmentAnimations();
        }
    }
    
    private void ConfigureAttachmentAnimations()
    {
        // Create animation settings list
        var animSettings = new List<AttachmentAvatarAnimConfig>();
        
        // Create idle animation config - keep attachment visible
        var idleConfig = new AttachmentAvatarAnimConfig
        {
            clipType = AvatarAnimationClipType.Idle,
            overrideClip = customIdleAnimation,
            overrideClipMale = customMaleIdleAnimation, // Gender-specific variation
            attachmentVisible = true,
            disableIK = false // Keep IK enabled for idle
        };
        animSettings.Add(idleConfig);
        
        // Create run animation config - keep attachment visible but disable IK
        var runConfig = new AttachmentAvatarAnimConfig
        {
            clipType = AvatarAnimationClipType.Run,
            overrideClip = customRunAnimation,
            attachmentVisible = true,
            disableIK = true // Disable IK during running to prevent issues with fast movement
        };
        animSettings.Add(runConfig);
        
        // Create jump config - hide attachment during jumps
        var jumpConfig = new AttachmentAvatarAnimConfig
        {
            clipType = AvatarAnimationClipType.JumpInAir,
            overrideClip = null, // No override, use default jump animation
            attachmentVisible = false, // Hide attachment during jumps
            disableIK = true
        };
        animSettings.Add(jumpConfig);
        
        // Apply animation settings to the attachment
        // Note: In a real implementation, you would set these on SpatialAttachmentAvatarAnimSettings
        Debug.Log($"Configured {animSettings.Count} animation overrides for attachment: {attachment.prettyName}");
        
        // Example of how this might be applied
        if (attachment.avatarAnimSettings != null)
        {
            // This is conceptual - actual implementation would depend on Spatial's API
            // attachment.avatarAnimSettings.SetAnimationConfigs(animSettings);
        }
    }
    
    // Method to demonstrate how these settings might be used at runtime
    public void OnAvatarAnimationChanged(AvatarAnimationClipType newAnimType)
    {
        // Find the config for this animation type
        AttachmentAvatarAnimConfig config = null;
        
        // In a real implementation, you would retrieve this from the attachment
        // config = attachment.avatarAnimSettings.GetConfigForType(newAnimType);
        
        if (config != null)
        {
            // Apply visibility setting
            SetAttachmentVisibility(config.attachmentVisible);
            
            // Apply IK setting
            SetAvatarIK(!config.disableIK);
            
            Debug.Log($"Animation changed to {newAnimType}: Visibility={config.attachmentVisible}, IK={!config.disableIK}");
        }
    }
    
    private void SetAttachmentVisibility(bool visible)
    {
        // Implementation to show/hide attachment
        if (attachment != null)
        {
            // This is conceptual - actual implementation would depend on Spatial's API
            // attachment.gameObject.SetActive(visible);
        }
    }
    
    private void SetAvatarIK(bool enabled)
    {
        // Implementation to enable/disable avatar IK
        if (SpatialBridge.actorService.localActor.hasAvatar)
        {
            // This is conceptual - actual implementation would depend on Spatial's API
            // SpatialBridge.actorService.localActor.avatar.SetIKEnabled(enabled);
        }
    }
}
```

## Best Practices

1. Use overrideClip sparingly and only when the standard animation would look incorrect with your attachment
2. Consider hiding attachments during animations where they might clip through the avatar's body
3. Disable IK for animations where precise hand/foot positioning might look unnatural with the attachment
4. Test your attachment animations with different avatar body types to ensure they work well
5. Provide gender-specific variations (overrideClipMale) only when necessary for anatomical differences
6. Keep override animations consistent with the Spatial animation style for a cohesive look
7. Use attachmentVisible = false for animations where the attachment would interfere with the movement
8. Ensure your overrideClip animations have the same timing and general flow as the default animations

## Common Use Cases

1. Configuring how weapons and tools are displayed during different animations
2. Creating special idle poses when certain items are equipped (e.g., a shield causing a defensive stance)
3. Hiding backpacks or capes during jumping or climbing to prevent clipping
4. Disabling IK for animations where precise hand positioning would break immersion
5. Providing gender-specific animations for items that should be held or worn differently
6. Creating special run animations for items that would affect the avatar's movement (e.g., heavy objects)
7. Setting up configuration for attachments that change the avatar's posture or stance
8. Defining visibility rules for accessories that might interfere with certain movements

## Completed: March 10, 2025
