# SpatialAvatarAttachment.AttachmentAnimatorType

Category: Core Components

Type: Enum

`SpatialAvatarAttachment.AttachmentAnimatorType` is an enumeration used to specify what type of animator should be used for avatar attachments in the Spatial SDK. It defines the animation behavior of objects attached to avatars and how they interact with the avatar's movement and actions.

## Properties/Fields

| Value | Description |
| --- | --- |
| None | No animator is used for the attachment. The attachment will not have its own animations and will simply follow the attached bone without any additional movement. |
| Standard | Uses the standard avatar attachment animator which provides basic animations that synchronize with common avatar movements and actions. This is the recommended setting for most attachments. |
| Custom | Uses a custom animator defined specifically for this attachment. This allows for completely customized animation behaviors beyond the standard set provided by the Spatial platform. |

## Usage Examples

### Setting Basic Attachment Animator Type

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class BackpackAttachmentSetup : MonoBehaviour
{
    public SpatialAvatarAttachment backpackAttachment;
    
    private void Start()
    {
        if (backpackAttachment != null)
        {
            // Configure a backpack attachment to use the standard animator
            backpackAttachment.attachmentAnimatorType = SpatialAvatarAttachment.AttachmentAnimatorType.Standard;
            
            Debug.Log($"Backpack attachment using animator type: {backpackAttachment.attachmentAnimatorType}");
        }
    }
}
```

### Creating Static Decorative Attachment

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class AuraEffectSetup : MonoBehaviour
{
    public SpatialAvatarAttachment auraAttachment;
    
    private void ConfigureAuraEffect()
    {
        if (auraAttachment != null)
        {
            // For a simple effect like an aura, we don't need animation control
            auraAttachment.prettyName = "Energy Aura";
            auraAttachment.category = SpatialAvatarAttachment.Category.Aura;
            auraAttachment.primarySlot = SpatialAvatarAttachment.Slot.Aura;
            
            // No animator needed, the effect will be handled by other components
            auraAttachment.attachmentAnimatorType = SpatialAvatarAttachment.AttachmentAnimatorType.None;
            
            Debug.Log("Aura effect configured without animator");
        }
    }
}
```

### Implementing Custom Animated Attachment

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class DragonPetSetup : MonoBehaviour
{
    public SpatialAvatarAttachment petAttachment;
    public Animator customPetAnimator;
    public AnimationClip idleAnimation;
    public AnimationClip followAnimation;
    
    private void ConfigureDragonPet()
    {
        if (petAttachment == null || customPetAnimator == null)
            return;
        
        // Configure a pet attachment with custom animation behavior
        petAttachment.prettyName = "Dragon Pet";
        petAttachment.category = SpatialAvatarAttachment.Category.Pet;
        petAttachment.primarySlot = SpatialAvatarAttachment.Slot.Pet;
        
        // Use custom animator for advanced pet behavior
        petAttachment.attachmentAnimatorType = SpatialAvatarAttachment.AttachmentAnimatorType.Custom;
        
        // Set up the custom animator with our specific clips
        RuntimeAnimatorController animController = customPetAnimator.runtimeAnimatorController;
        
        // In a real implementation, you would set up an animator controller
        // with appropriate parameters and transitions between animations
        
        Debug.Log("Dragon pet configured with custom animator");
    }
}
```

## Best Practices

1. **Default to Standard**: For most attachments, the Standard animator type provides sufficient animation control and synchronizes well with avatar movements. Only use Custom when you need specific behaviors not covered by the Standard animator.

2. **Use None for Static Objects**: For purely decorative attachments that don't need to animate, use the None type to save resources and simplify setup.

3. **Custom Animator Setup**: When using the Custom animator type, ensure your animator controller is properly configured with parameters that respond to avatar state changes to maintain synchronization.

4. **Performance Considerations**: Custom animators may have higher performance costs. Test thoroughly on target platforms to ensure they don't negatively impact frame rates.

5. **Animation Testing**: Test your attachments with all avatar animation states (walking, running, jumping, etc.) to ensure they behave correctly regardless of animator type selected.

## Common Use Cases

1. **Standard Animator**: Used for common attachments like clothing, accessories, and tools that need simple movement in sync with the avatar.

2. **No Animator**: Used for static decorations, simple visual effects, or objects that have their own particle systems or shader-based animations.

3. **Custom Animator**: Used for complex interactive attachments like pets with unique behaviors, weapons with special attack animations, or vehicles with specific movement patterns.

4. **Emote-Specific Animations**: Custom animators are often used for attachments that need to perform specific animations during emotes or other special actions.

5. **Dynamic Response Attachments**: Custom animators enable attachments to respond to specific game events or environmental conditions beyond the standard avatar animations.

## Related Components

- [SpatialAvatarAttachment](./SpatialAvatarAttachment.md): The main component for creating avatar attachments.
- [SpatialAttachmentAvatarAnimSettings](./SpatialAttachmentAvatarAnimSettings.md): Settings for how attachments interact with avatar animations.
- [SpatialAvatarAnimation](./SpatialAvatarAnimation.md): Component for creating custom avatar animations.
- [SpatialAvatarAnimOverrides](./SpatialAvatarAnimOverrides.md): Component for overriding default avatar animations.