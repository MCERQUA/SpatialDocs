# AvatarAnimationClipType

Category: Core Components

Type: Enum

`AvatarAnimationClipType` is an enumeration that defines all the animation clips that are part of the avatar animation controller in the Spatial SDK. This enum provides a standardized way to reference different avatar animations throughout your Spatial application.

## Properties/Fields

| Value | Description |
| --- | --- |
| Idle | Default idle animation when the avatar is stationary. |
| Walk | Animation played when the avatar is walking at a normal pace. |
| Jog | Animation played when the avatar is moving at a jogging pace (faster than walking). |
| Run | Animation played when the avatar is running (fastest movement). |
| Fall | Animation played when the avatar is falling through the air. |
| JumpStartIdle | Animation played at the beginning of a jump when starting from an idle position. |
| JumpStartMoving | Animation played at the beginning of a jump when starting while in motion. |
| JumpInAir | Animation played while the avatar is in the air during a jump. |
| JumpLandStanding | Animation played when landing from a jump into a standing position. |
| JumpLandWalking | Animation played when landing from a jump into a walking motion. |
| JumpLandRunning | Animation played when landing from a jump into a running motion. |
| JumpLandHigh | Animation played when landing from a high jump or fall. |
| JumpMultiple | Animation played when performing multiple jumps in succession. |
| ClimbIdle | Animation played when the avatar is stationary on a climbable surface. |
| ClimbUp | Animation played when the avatar is climbing upward. |
| ClimbEndTop | Animation played when the avatar reaches the top of a climbable object. |
| Sit | Animation played when the avatar is sitting. |
| Emote | Generic animation slot used for emotes and custom animations. |
| Count | Special value representing the total number of animation types (not an actual animation). |

## Usage Examples

### Playing a Specific Animation

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class AvatarAnimationExample : MonoBehaviour
{
    public void PlaySitAnimation()
    {
        // Get reference to the local avatar from the actor service
        var avatar = SpatialBridge.actorService.localActor.avatar;
        
        // Play the sit animation - typically this is handled automatically by the avatar controller
        // but shown here for demonstration purposes
        if (avatar != null)
        {
            // This is a simplified example - actual implementation may vary
            Debug.Log($"Playing animation: {AvatarAnimationClipType.Sit}");
        }
    }
}
```

### Creating Custom Avatar Attachments with Animation Override

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class CustomAvatarAttachment : MonoBehaviour
{
    public SpatialAvatarAttachment attachment;
    
    private void Start()
    {
        // Example of setting up animation overrides for an attachment
        if (attachment != null)
        {
            // Override the run animation with a custom animation for this attachment
            var animSettings = attachment.GetAvatarAnimSettings();
            
            // Here we're referencing the Run animation type from the enum
            Debug.Log($"Setting up override for animation: {AvatarAnimationClipType.Run}");
            
            // In a real implementation, you would configure the override animation clip
        }
    }
}
```

### Checking Current Animation State

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class AvatarAnimationStateMonitor : MonoBehaviour
{
    private void Update()
    {
        var avatar = SpatialBridge.actorService.localActor.avatar;
        if (avatar != null)
        {
            // This is a conceptual example - the actual API to check current animation may differ
            // Check if avatar is currently running
            bool isRunning = avatar.IsRunning;
            if (isRunning)
            {
                Debug.Log($"Avatar is using animation type: {AvatarAnimationClipType.Run}");
            }
            
            // Check if avatar is jumping
            bool isJumping = avatar.IsJumping;
            if (isJumping)
            {
                Debug.Log($"Avatar is using a jump animation (could be one of several jump types)");
                // Could be JumpStartIdle, JumpInAir, etc. depending on jump phase
            }
        }
    }
}
```

## Best Practices

1. **Use the enum values consistently**: Always reference animations using this enum rather than hardcoded strings or integers to ensure compatibility with future SDK updates.

2. **Consider animation transitions**: When switching between animations, be aware that transitions between certain animation states may have specific requirements or limitations.

3. **Custom animations**: When creating custom animations for avatars, use the standard animation states as references for timing and style to maintain a consistent look and feel.

4. **Performance considerations**: Be mindful that frequent animation state changes can impact performance, especially in spaces with many avatars.

5. **Animation events**: When working with avatar animations, consider using animation events to trigger actions at specific points in the animation sequence.

## Common Use Cases

1. **Custom avatar attachments**: Creating attachments that modify or override standard avatar animations.

2. **Animation-driven gameplay mechanics**: Implementing game mechanics that depend on the avatar's current animation state.

3. **User interaction feedback**: Providing visual feedback for user actions through appropriate animation selections.

4. **Avatar customization**: Creating custom animation sets for different avatar types or player roles.

5. **Emote systems**: Implementing social interaction features through custom emote animations.

## Related Components

- [SpatialAvatar](./SpatialAvatar.md): The main component representing an avatar in the Spatial environment.
- [SpatialAvatarAnimation](./SpatialAvatarAnimation.md): Component that handles avatar animations.
- [SpatialAvatarAnimOverrides](./SpatialAvatarAnimOverrides.md): Component for overriding standard avatar animations.
- [SpatialAvatarAttachment](./SpatialAvatarAttachment.md): Component for attaching objects to avatars.
- [SpatialAvatarDefaultAnimSetType](./SpatialAvatarDefaultAnimSetType.md): Enum defining different sets of default animations.