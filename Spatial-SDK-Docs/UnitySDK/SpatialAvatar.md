# SpatialAvatar

Category: Core Components

Type: Class

`SpatialAvatar` is a core component in the Spatial SDK that defines an avatar asset in the Spatial platform. It inherits from SpatialPackageAsset and provides configuration options for avatar appearance, animations, and physical characteristics.

## Properties/Fields

| Property | Description |
| --- | --- |
| DEFAULT_CHARACTER_CONTROLLER_HEIGHT | The default height for the avatar's character controller (constant). |
| DEFAULT_CHARACTER_CONTROLLER_RADIUS | The default radius for the avatar's character controller (constant). |
| animOverrides | Reference to the SpatialAvatarAnimOverrides component that allows overriding default avatar animations. |
| characterControllerHeight | The height of the character controller for this avatar. Affects how the avatar interacts with the environment physically. |
| characterControllerRadius | The radius of the character controller for this avatar. Affects the avatar's width for collision detection. |
| defaultAnimSetType | Specifies which animation set (Masculine, Feminine, or Unset) to use as the default for this avatar. Uses SpatialAvatarDefaultAnimSetType enum. |
| hasRagdollSetup | Indicates whether this avatar has a ragdoll physics setup configured. Read-only boolean value. |
| ragdollColliders | Collection of colliders used for the avatar's ragdoll physics simulation. |
| ragdollJoints | Collection of joints used for the avatar's ragdoll physics simulation. |
| ragdollRigidbodies | Collection of rigidbodies used for the avatar's ragdoll physics simulation. |
| prettyName | Display name for the avatar in the Spatial platform UI. Inherited from SpatialPackageAsset. |
| tooltip | Descriptive tooltip text for the avatar. Inherited from SpatialPackageAsset. |
| documentationURL | URL to documentation for this avatar. Inherited from SpatialPackageAsset. |

## Usage Examples

### Basic Avatar Configuration

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class AvatarSetup : MonoBehaviour
{
    // Reference to the avatar component
    public SpatialAvatar avatarAsset;
    
    private void Start()
    {
        if (avatarAsset != null)
        {
            // Configure the avatar properties
            avatarAsset.prettyName = "Explorer Avatar";
            avatarAsset.tooltip = "An avatar designed for exploration";
            
            // Set animation defaults
            avatarAsset.defaultAnimSetType = SpatialAvatarDefaultAnimSetType.Unset; // Use universal animations
            
            // Configure physics properties
            avatarAsset.characterControllerHeight = 1.8f; // 1.8 meters tall
            avatarAsset.characterControllerRadius = 0.3f; // 0.3 meters radius
            
            Debug.Log($"Avatar configured: {avatarAsset.prettyName}");
            Debug.Log($"Has ragdoll: {avatarAsset.hasRagdollSetup}");
        }
    }
}
```

### Setting Up Avatar Animation Overrides

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class AvatarAnimationConfiguration : MonoBehaviour
{
    public SpatialAvatar avatar;
    public AnimationClip customIdleAnimation;
    public AnimationClip customRunAnimation;
    
    private void ConfigureAnimations()
    {
        if (avatar.animOverrides == null)
        {
            // Add animation overrides component if it doesn't exist
            avatar.animOverrides = avatar.gameObject.AddComponent<SpatialAvatarAnimOverrides>();
        }
        
        // Set custom animations
        avatar.animOverrides.idle = customIdleAnimation;
        avatar.animOverrides.run = customRunAnimation;
        
        Debug.Log("Custom animations configured for avatar");
    }
}
```

### Working with Ragdoll Physics

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections;

public class AvatarRagdollController : MonoBehaviour
{
    public SpatialAvatar avatar;
    
    public void EnableRagdoll()
    {
        if (avatar.hasRagdollSetup)
        {
            // Avatar has ragdoll setup configured
            Debug.Log("Enabling ragdoll physics");
            
            // In a real implementation, you would access the avatar instance
            // through SpatialBridge.actorService.localActor.avatar
            // and enable ragdoll physics through it
            
            // Example:
            // SpatialBridge.actorService.localActor.avatar.EnableRagdoll();
            
            // For demonstration purposes - setup the ragdoll directly
            SetupRagdoll(true);
        }
        else
        {
            Debug.LogWarning("Avatar does not have ragdoll setup configured");
        }
    }
    
    private void SetupRagdoll(bool enabled)
    {
        // Enable/disable all ragdoll rigidbodies
        foreach (var rb in avatar.ragdollRigidbodies)
        {
            rb.isKinematic = !enabled;
        }
        
        // Enable/disable all ragdoll colliders
        foreach (var collider in avatar.ragdollColliders)
        {
            collider.enabled = enabled;
        }
    }
}
```

## Best Practices

1. **Character Controller Dimensions**: Set appropriate character controller dimensions based on the avatar's visual size to ensure proper collision detection and movement.

2. **Animation Set Selection**: Choose the appropriate default animation set (Masculine, Feminine, or Unset) based on the avatar's design and intended movement style.

3. **Ragdoll Configuration**: When setting up ragdoll physics, ensure all joints have appropriate limits and rigidbodies have suitable mass values to create realistic physics behavior.

4. **Avoid Runtime Changes**: Avatar configurations are typically set during development rather than changed at runtime. Focus on creating well-configured avatars in the editor.

5. **Animation Testing**: Test your avatar with all animation types to ensure animations look correct and don't have clipping issues, especially when custom animations are used.

6. **Performance Considerations**: Be mindful of the complexity of avatar models and animations, as they can impact performance, especially in scenes with many avatars.

## Common Use Cases

1. **Custom Character Creation**: Designing unique avatar characters for the Spatial platform.

2. **Themed Avatars**: Creating avatars that match the theme or setting of your Spatial experience.

3. **Gameplay-Specific Avatars**: Implementing avatars with specific attributes or animations for certain gameplay mechanics.

4. **Animation Style Variation**: Using different animation sets to provide unique movement styles for different avatar types.

5. **Physics-Based Interactions**: Implementing physics interactions using the avatar's ragdoll capabilities.

6. **Avatar Customization Systems**: Building systems that allow users to customize their avatar appearance and animations.

## Related Components

- [SpatialAvatarAnimation](./SpatialAvatarAnimation.md): Component for managing avatar animations.
- [SpatialAvatarAnimOverrides](./SpatialAvatarAnimOverrides.md): Component for overriding default avatar animations.
- [SpatialAvatarDefaultAnimSetType](./SpatialAvatarDefaultAnimSetType.md): Enum defining different animation set styles.
- [SpatialAvatarAttachment](./SpatialAvatarAttachment.md): Component for attaching objects to avatars.
- [SpatialAttachmentAvatarAnimSettings](./SpatialAttachmentAvatarAnimSettings.md): Settings for avatar attachment animations.
- [AvatarAnimationClipType](./AvatarAnimationClipType.md): Enum defining different avatar animation types.