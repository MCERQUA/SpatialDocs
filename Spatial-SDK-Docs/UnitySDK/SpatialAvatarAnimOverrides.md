# SpatialAvatarAnimOverrides

Category: Core Components

Type: Class

`SpatialAvatarAnimOverrides` is a component in the Spatial SDK that allows developers to override default avatar animations with custom animation clips. It inherits from AttachmentAnimationClips and provides properties for each animation state that an avatar can have.

## Properties/Fields

| Property | Description |
| --- | --- |
| idle | Override for the default idle animation when the avatar is stationary. |
| walk | Override for the walking animation. |
| jog | Override for the jogging animation. |
| run | Override for the running animation. |
| fall | Override for the falling animation. |
| jumpStartIdle | Override for the jump start from idle position animation. |
| jumpStartMoving | Override for the jump start while moving animation. |
| jumpInAir | Override for the in-air jump animation. |
| jumpLandStanding | Override for the landing from jump to standing position animation. |
| jumpLandWalking | Override for the landing from jump to walking animation. |
| jumpLandRunning | Override for the landing from jump to running animation. |
| jumpLandHigh | Override for the landing from a high jump animation. |
| jumpMultiple | Override for the multiple jumps in succession animation. |
| climbIdle | Override for the idle while climbing animation. |
| climbUp | Override for the climbing upward animation. |
| climbEndTop | Override for the reaching the top of a climb animation. |
| sit | Override for the sitting animation. |
| lookup | Override for the looking up animation. |

## Methods

| Method | Description |
| --- | --- |
| AllOverrideClips() | Returns an enumerable collection of all animation override clips as tuples of (string name, AnimationClip clip). Useful for iterating through all animation overrides. |

## Usage Examples

### Basic Animation Overrides

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class CustomAvatarAnimationSetup : MonoBehaviour
{
    public SpatialAvatar avatar;
    public AnimationClip customIdleAnimation;
    public AnimationClip customRunAnimation;
    public AnimationClip customJumpAnimation;
    
    private void Start()
    {
        // Ensure the avatar has the animation overrides component
        if (avatar.animOverrides == null)
        {
            avatar.animOverrides = avatar.gameObject.AddComponent<SpatialAvatarAnimOverrides>();
        }
        
        // Set custom animations
        avatar.animOverrides.idle = customIdleAnimation;
        avatar.animOverrides.run = customRunAnimation;
        avatar.animOverrides.jumpInAir = customJumpAnimation;
        
        Debug.Log("Custom animations applied to avatar");
    }
}
```

### Creating a Complete Animation Set

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class CompleteAnimationOverrides : MonoBehaviour
{
    [System.Serializable]
    public class AnimationSet
    {
        public AnimationClip idle;
        public AnimationClip walk;
        public AnimationClip run;
        public AnimationClip jump;
        public AnimationClip fall;
        public AnimationClip sit;
    }
    
    public SpatialAvatar avatar;
    public AnimationSet femaleAnimations;
    public AnimationSet maleAnimations;
    
    public void ApplyAnimationSet(bool useMaleSet)
    {
        if (avatar.animOverrides == null)
        {
            avatar.animOverrides = avatar.gameObject.AddComponent<SpatialAvatarAnimOverrides>();
        }
        
        AnimationSet selectedSet = useMaleSet ? maleAnimations : femaleAnimations;
        
        // Apply the selected animation set
        avatar.animOverrides.idle = selectedSet.idle;
        avatar.animOverrides.walk = selectedSet.walk;
        avatar.animOverrides.run = selectedSet.run;
        avatar.animOverrides.jumpInAir = selectedSet.jump;
        avatar.animOverrides.fall = selectedSet.fall;
        avatar.animOverrides.sit = selectedSet.sit;
        
        string setType = useMaleSet ? "male" : "female";
        Debug.Log($"Applied {setType} animation set to avatar");
    }
}
```

### Iterating Through All Animation Overrides

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class AnimationOverrideManager : MonoBehaviour
{
    public SpatialAvatar avatar;
    public List<AnimationClip> standardAnimations;
    
    private void ApplyStandardAnimations()
    {
        if (avatar.animOverrides == null || standardAnimations.Count < 5)
        {
            Debug.LogError("Missing required components or animations");
            return;
        }
        
        // Apply standard animations
        avatar.animOverrides.idle = standardAnimations[0];
        avatar.animOverrides.walk = standardAnimations[1];
        avatar.animOverrides.run = standardAnimations[2];
        avatar.animOverrides.jumpInAir = standardAnimations[3];
        avatar.animOverrides.fall = standardAnimations[4];
        
        // List all configured overrides
        Debug.Log("Current animation overrides:");
        foreach (var (name, clip) in avatar.animOverrides.AllOverrideClips())
        {
            if (clip != null)
            {
                Debug.Log($"- {name}: {clip.name}");
            }
        }
    }
    
    private void ClearUnusedOverrides()
    {
        // Example of how to identify and clear unused animation overrides
        List<string> animationsToReset = new List<string>();
        
        foreach (var (name, clip) in avatar.animOverrides.AllOverrideClips())
        {
            // Check if the clip is not one of our standard animations
            bool isStandardAnimation = false;
            foreach (var standardClip in standardAnimations)
            {
                if (clip == standardClip)
                {
                    isStandardAnimation = true;
                    break;
                }
            }
            
            if (!isStandardAnimation && clip != null)
            {
                animationsToReset.Add(name);
            }
        }
        
        // Reset any non-standard animations (example only - would need reflection or direct property access)
        Debug.Log($"Would reset {animationsToReset.Count} non-standard animation overrides");
    }
}
```

## Best Practices

1. **Consistent Animation Style**: Maintain a consistent style across all animation overrides to ensure smooth transitions and a cohesive look.

2. **Complete Sets**: When possible, create complete sets of animation overrides rather than overriding just a few animations to prevent jarring transitions between default and custom animations.

3. **Animation Testing**: Test your animation overrides with different avatar configurations to ensure they work well with various body types and in different environments.

4. **Performance Considerations**: Be mindful of animation complexity and file size, especially when deploying to platforms with limited resources.

5. **Animation Transitions**: Pay special attention to animations that commonly transition into each other (like walk to run, or jump to land) to ensure smooth movement.

6. **Property Initialization**: Always check if the animOverrides component exists before trying to assign animations to it, and initialize it if necessary.

## Common Use Cases

1. **Character Customization**: Creating unique movement styles for different avatar characters.

2. **Themed Movement Sets**: Implementing themed animation sets that match a specific environment or setting.

3. **Specialized Avatars**: Developing avatars with specialized movement animations for specific roles or abilities.

4. **Gender-Specific Animations**: Creating different animation sets for masculine and feminine avatars.

5. **Game-Specific Movements**: Implementing custom animations that suit specific gameplay mechanics or interactions.

6. **Stylized Characters**: Creating exaggerated or stylized movement animations for cartoon-like or non-realistic avatars.

## Related Components

- [SpatialAvatar](./SpatialAvatar.md): The main component representing an avatar in the Spatial platform.
- [SpatialAvatarAnimation](./SpatialAvatarAnimation.md): Component for creating custom avatar animations.
- [AvatarAnimationClipType](./AvatarAnimationClipType.md): Enum defining different avatar animation types.
- [SpatialAvatarDefaultAnimSetType](./SpatialAvatarDefaultAnimSetType.md): Enum defining different animation set styles.
- [AttachmentAnimationClips](./AttachmentAnimationClips.md): Base class for animation clip collections.
- [SpatialAttachmentAvatarAnimSettings](./SpatialAttachmentAvatarAnimSettings.md): Settings for avatar attachment animations.