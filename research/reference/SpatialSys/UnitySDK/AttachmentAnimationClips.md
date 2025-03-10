# AttachmentAnimationClips

Category: Other Classes

Class: Animation

## Overview
The `AttachmentAnimationClips` class is a specialized extension of `SpatialAvatarAnimOverrides` that provides custom animation clip management for avatar attachments. This class is used to define and manage custom action animations that can be played when an attachment is equipped by an avatar, allowing attachments to have their own unique animation behaviors.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| customActions | AnimationClip[] | An array of custom animation clips that can be triggered for the attachment. These animations can be used for special actions or interactions specific to the attachment. |

## Inherited Members
This class inherits from SpatialAvatarAnimOverrides and includes all standard avatar animation override properties:

| Member | Description |
| --- | --- |
| AllOverrideClips() | Returns all animation clips that have been assigned as overrides. |
| climbEndTop | The animation clip for ending a climb at the top of a climbable object. |
| climbIdle | The animation clip for idling while on a climbable surface. |
| climbUp | The animation clip for climbing upward on a climbable surface. |
| fall | The animation clip for falling. |
| idle | The animation clip for standing idle. |
| jog | The animation clip for jogging (medium speed movement). |
| jumpInAir | The animation clip for being in the air during a jump. |
| jumpLandHigh | The animation clip for landing from a high jump. |
| jumpLandRunning | The animation clip for landing while running. |
| jumpLandStanding | The animation clip for landing from a standing jump. |
| jumpLandWalking | The animation clip for landing while walking. |
| jumpMultiple | The animation clip for performing multiple jumps in succession. |
| jumpStartIdle | The animation clip for starting a jump from an idle position. |
| jumpStartMoving | The animation clip for starting a jump while in motion. |
| lookup | The animation clip for looking upward. |
| run | The animation clip for running (fast movement). |
| sit | The animation clip for sitting. |
| walk | The animation clip for walking (slow movement). |

## Usage Example

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;

public class CustomAttachmentAnimationExample : MonoBehaviour
{
    [SerializeField] private SpatialAvatarAttachment attachment;
    [SerializeField] private AnimationClip[] customActionClips;
    
    private void Start()
    {
        if (attachment != null)
        {
            // Create a new AttachmentAnimationClips instance for the attachment
            AttachmentAnimationClips animClips = new AttachmentAnimationClips();
            
            // Assign custom action animations
            animClips.customActions = customActionClips;
            
            // Override standard avatar animations as needed
            animClips.idle = CreateIdleOverrideForAttachment();
            animClips.run = CreateRunOverrideForAttachment();
            
            // Apply the animation clips to the attachment
            attachment.attachmentAnimClips = animClips;
            
            Debug.Log($"Configured {customActionClips.Length} custom actions for attachment: {attachment.prettyName}");
        }
    }
    
    private AnimationClip CreateIdleOverrideForAttachment()
    {
        // Create or reference a custom idle animation for when the attachment is equipped
        // This is just a placeholder - you would normally reference an existing animation clip
        return Resources.Load<AnimationClip>("Animations/CustomIdleWithAttachment");
    }
    
    private AnimationClip CreateRunOverrideForAttachment()
    {
        // Create or reference a custom run animation for when the attachment is equipped
        return Resources.Load<AnimationClip>("Animations/CustomRunWithAttachment");
    }
    
    public void TriggerCustomAction(int actionIndex)
    {
        if (attachment != null && 
            attachment.attachmentAnimClips is AttachmentAnimationClips animClips && 
            animClips.customActions != null && 
            actionIndex < animClips.customActions.Length)
        {
            // In a real implementation, you would use SpatialBridge to trigger the animation
            Debug.Log($"Triggering custom action: {animClips.customActions[actionIndex].name}");
            
            // Example of how this might be triggered
            if (SpatialBridge.actorService.localActor.hasAvatar)
            {
                var avatar = SpatialBridge.actorService.localActor.avatar;
                // This is pseudocode - the actual implementation would depend on Spatial's API
                // avatar.PlayCustomAnimation(animClips.customActions[actionIndex]);
            }
        }
    }
}
```

## Best Practices

1. Keep custom action animations short and focused for responsive gameplay
2. Ensure animation clips are properly rigged to work with the Spatial avatar skeleton
3. Test animations with different avatar body types to ensure proper alignment
4. Use descriptive names for custom actions to make them easier to identify
5. Consider creating animation transitions between standard avatar animations and custom actions
6. Keep the number of custom actions reasonable to avoid performance issues and UI clutter
7. Make sure custom animations have proper entry and exit points to blend smoothly with standard animations

## Common Use Cases

1. Creating unique interaction animations for tools and weapons (swinging, shooting, etc.)
2. Adding special emotes or gestures specific to equipped items
3. Modifying standard movement animations while holding or wearing certain attachments
4. Creating sitting or idle poses specific to an attachment (like sitting differently with a backpack)
5. Implementing vehicle or mount-specific animations
6. Creating tool-specific work animations (hammering, sawing, etc.)
7. Adding dance moves or special performances tied to musical instruments or props

## Completed: March 10, 2025
