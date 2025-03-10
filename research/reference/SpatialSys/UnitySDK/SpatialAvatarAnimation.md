# SpatialAvatarAnimation

Category: Core Components

Type: Class

`SpatialAvatarAnimation` is a component in the Spatial SDK that defines custom animations for avatars. It inherits from SpatialPackageAsset and provides a way to create and configure animations that can be applied to avatars in the Spatial platform.

## Properties/Fields

| Property | Description |
| --- | --- |
| animator | Reference to the Animator component that contains the animation clip. This animator holds the animation data that will be used by the avatar. |
| targetClip | The animation clip type that this animation targets. Specifies which standard avatar animation this custom animation will replace. |
| prettyName | Display name for the animation in the Spatial platform UI. Inherited from SpatialPackageAsset. |
| tooltip | Descriptive tooltip text for the animation. Inherited from SpatialPackageAsset. |
| documentationURL | URL to documentation for this animation. Inherited from SpatialPackageAsset. |

## Usage Examples

### Creating a Custom Avatar Animation

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class CustomAnimationSetup : MonoBehaviour
{
    // Reference to the avatar animation component
    public SpatialAvatarAnimation customAnimation;
    
    private void Start()
    {
        if (customAnimation != null)
        {
            // Configure the animation properties
            customAnimation.prettyName = "Victory Dance";
            customAnimation.tooltip = "A celebratory dance animation";
            
            // Set which animation type this will replace
            customAnimation.targetClip = AvatarAnimationClipType.Emote;
            
            // Ensure the animator component is assigned
            if (customAnimation.animator == null)
            {
                customAnimation.animator = customAnimation.gameObject.GetComponent<Animator>();
                if (customAnimation.animator == null)
                {
                    Debug.LogError("No Animator component found on the game object");
                }
            }
            
            Debug.Log($"Custom animation configured: {customAnimation.prettyName}");
        }
    }
}
```

### Setting Up Multiple Custom Animations

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class AvatarAnimationLibrary : MonoBehaviour
{
    // List of custom animations
    public List<SpatialAvatarAnimation> customAnimations = new List<SpatialAvatarAnimation>();
    
    // Animation prefabs
    public GameObject idleAnimationPrefab;
    public GameObject runAnimationPrefab;
    public GameObject jumpAnimationPrefab;
    
    private void CreateAnimationLibrary()
    {
        // Create custom idle animation
        GameObject idleObj = Instantiate(idleAnimationPrefab, transform);
        SpatialAvatarAnimation idleAnim = idleObj.AddComponent<SpatialAvatarAnimation>();
        idleAnim.prettyName = "Relaxed Idle";
        idleAnim.targetClip = AvatarAnimationClipType.Idle;
        idleAnim.animator = idleObj.GetComponent<Animator>();
        customAnimations.Add(idleAnim);
        
        // Create custom run animation
        GameObject runObj = Instantiate(runAnimationPrefab, transform);
        SpatialAvatarAnimation runAnim = runObj.AddComponent<SpatialAvatarAnimation>();
        runAnim.prettyName = "Sprint Run";
        runAnim.targetClip = AvatarAnimationClipType.Run;
        runAnim.animator = runObj.GetComponent<Animator>();
        customAnimations.Add(runAnim);
        
        // Create custom jump animation
        GameObject jumpObj = Instantiate(jumpAnimationPrefab, transform);
        SpatialAvatarAnimation jumpAnim = jumpObj.AddComponent<SpatialAvatarAnimation>();
        jumpAnim.prettyName = "Flip Jump";
        jumpAnim.targetClip = AvatarAnimationClipType.JumpInAir;
        jumpAnim.animator = jumpObj.GetComponent<Animator>();
        customAnimations.Add(jumpAnim);
        
        Debug.Log($"Created animation library with {customAnimations.Count} animations");
    }
}
```

### Applying Custom Animations to an Avatar

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class ApplyCustomAnimations : MonoBehaviour
{
    public SpatialAvatar targetAvatar;
    public SpatialAvatarAnimation customIdleAnimation;
    public SpatialAvatarAnimation customRunAnimation;
    
    private void ApplyAnimations()
    {
        // Ensure the avatar has animation overrides component
        if (targetAvatar.animOverrides == null)
        {
            targetAvatar.animOverrides = targetAvatar.gameObject.AddComponent<SpatialAvatarAnimOverrides>();
        }
        
        // This is a conceptual example - in actual implementation, 
        // you would use the animation clips from the SpatialAvatarAnimation components
        
        // Get the animation clips from the SpatialAvatarAnimation components
        AnimationClip idleClip = GetAnimationClipFromSpatialAnimation(customIdleAnimation);
        AnimationClip runClip = GetAnimationClipFromSpatialAnimation(customRunAnimation);
        
        // Apply the clips to the avatar's animation overrides
        if (idleClip != null)
        {
            targetAvatar.animOverrides.idle = idleClip;
            Debug.Log($"Applied custom idle animation: {customIdleAnimation.prettyName}");
        }
        
        if (runClip != null)
        {
            targetAvatar.animOverrides.run = runClip;
            Debug.Log($"Applied custom run animation: {customRunAnimation.prettyName}");
        }
    }
    
    private AnimationClip GetAnimationClipFromSpatialAnimation(SpatialAvatarAnimation spatialAnimation)
    {
        // This is a simplified example - in a real implementation, 
        // you would need to extract the actual animation clip from the animator
        if (spatialAnimation != null && spatialAnimation.animator != null)
        {
            // For demonstration purposes only
            // In reality, you would access the specific clip from the animator's runtime controller
            return spatialAnimation.animator.runtimeAnimatorController as AnimationClip;
        }
        return null;
    }
}
```

## Best Practices

1. **Animation Compatibility**: Ensure custom animations are compatible with the avatar's rig and skeleton structure to prevent deformation or clipping issues.

2. **Animation Transitions**: When creating custom animations, consider how they will transition to and from other animations to create smooth movement.

3. **Animation Timing**: Match the timing and pacing of custom animations with standard animations to maintain a consistent feel.

4. **Organization**: Use descriptive names and tooltips for animations to make them easily identifiable in the Spatial platform UI.

5. **Animation Testing**: Test custom animations with different avatar types to ensure they work correctly across all intended use cases.

6. **Performance Considerations**: Keep animation complexity appropriate for the target platform to maintain good performance.

## Common Use Cases

1. **Unique Character Movements**: Creating distinctive movement styles for specific avatar characters.

2. **Custom Emotes**: Implementing unique emote animations for social interaction.

3. **Game-Specific Animations**: Adding animations for game-specific actions or abilities.

4. **Themed Animation Sets**: Creating sets of animations that match a specific theme or setting.

5. **Specialized Interaction Animations**: Implementing animations for interacting with specific objects or environments.

6. **Character Personality**: Using animations to convey personality traits or emotional states.

## Related Components

- [SpatialAvatar](./SpatialAvatar.md): Main component representing an avatar in the Spatial platform.
- [SpatialAvatarAnimOverrides](./SpatialAvatarAnimOverrides.md): Component for overriding default avatar animations.
- [AvatarAnimationClipType](./AvatarAnimationClipType.md): Enum defining different avatar animation types.
- [SpatialAvatarDefaultAnimSetType](./SpatialAvatarDefaultAnimSetType.md): Enum defining different animation set styles.
- [SpatialAttachmentAvatarAnimSettings](./SpatialAttachmentAvatarAnimSettings.md): Settings for avatar attachment animations.