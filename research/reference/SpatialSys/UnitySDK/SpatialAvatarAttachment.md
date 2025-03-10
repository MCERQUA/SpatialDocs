# SpatialAvatarAttachment

Category: Core Components

Type: Class

`SpatialAvatarAttachment` is a core component in the Spatial SDK that enables custom objects to be attached to avatars. It provides a comprehensive framework for defining how objects are positioned, animated, and interact with different parts of an avatar. The component inherits from SpatialPackageAsset and offers extensive customization options for creating accessories, tools, pets, and other avatar enhancements.

## Properties/Fields

| Property | Description |
| --- | --- |
| primarySlot | The main attachment slot on the avatar where this attachment will be placed. Uses SpatialAvatarAttachment.Slot enum. |
| additionalSlots | Additional attachment slots that this attachment will occupy, specified as a bitmask using SpatialAvatarAttachment.SlotMask enum. |
| category | The category of this attachment, which defines its general purpose (e.g., Accessory, Tool, Pet). Uses SpatialAvatarAttachment.Category enum. |
| occupiedSlots | Read-only property returning a SlotMask that combines the primary slot and all additional slots to show all occupied avatar slots. |
| attachToBone | When enabled, the attachment will be directly attached to a specified bone target on the avatar. |
| attachToBoneFeatureAvailable | Indicates whether the attach-to-bone feature is available for this attachment. |
| attachBoneTarget | Target bone on the avatar where this attachment will be connected when attachToBone is enabled. Uses Unity's HumanBodyBones enum. |
| attachBoneOffset | Position offset to apply to the attachment relative to the target bone position. |
| attachBoneRotationOffset | Rotation offset to apply to the attachment relative to the target bone rotation. |
| isSkinnedToHumanoidSkeleton | Indicates if this attachment uses a skinned mesh that is rigged to the humanoid skeleton. |
| skinningFeatureAvailable | Indicates whether skinning features are available for this attachment. |
| overrideAvatarAnimations | When enabled, this attachment can override the avatar's default animations with custom animations. |
| avatarAnimSettings | Settings that define how this attachment interacts with avatar animations. Returns a SpatialAttachmentAvatarAnimSettings object. |
| attachmentAnimClips | Collection of custom animation clips designed for this attachment. |
| attachmentAnimatorType | The type of animator to use for this attachment. Uses SpatialAvatarAttachment.AttachmentAnimatorType enum. |
| animatorFeatureAvailable | Indicates whether animator features are available for this attachment. |
| customActionsEnabled | When enabled, this attachment can provide custom actions for the avatar. |
| customActionsCount | The number of custom actions configured for this attachment. |
| customActionsFeatureAvailable | Indicates whether custom action features are available for this attachment. |
| ikFeatureActive | Indicates whether Inverse Kinematics (IK) features are currently active for this attachment. |
| ikFeatureAvailable | Indicates whether IK features are available for this attachment. |
| ikTargetsEnabled | Enables or disables IK targets for this attachment. |
| ikLeftHandTarget | The transform to use as the IK target for the avatar's left hand. |
| ikRightHandTarget | The transform to use as the IK target for the avatar's right hand. |
| ikLeftFootTarget | The transform to use as the IK target for the avatar's left foot. |
| ikRightFootTarget | The transform to use as the IK target for the avatar's right foot. |
| prettyName | Display name for the attachment in the Spatial platform UI. Inherited from SpatialPackageAsset. |
| tooltip | Descriptive tooltip text for the attachment. Inherited from SpatialPackageAsset. |
| documentationURL | URL to documentation for this attachment. Inherited from SpatialPackageAsset. |
| version | Version number of this attachment. |

## Usage Examples

### Basic Avatar Attachment Configuration

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class HatAttachmentSetup : MonoBehaviour
{
    public SpatialAvatarAttachment attachment;
    
    private void Start()
    {
        if (attachment != null)
        {
            // Configure the attachment properties
            attachment.prettyName = "Cowboy Hat";
            attachment.tooltip = "A classic cowboy hat for your avatar";
            
            // Set the attachment slot and category
            attachment.primarySlot = SpatialAvatarAttachment.Slot.Hat;
            attachment.category = SpatialAvatarAttachment.Category.Accessory;
            
            // Position the attachment on the head bone
            attachment.attachToBone = true;
            attachment.attachBoneTarget = HumanBodyBones.Head;
            attachment.attachBoneOffset = new Vector3(0, 0.1f, 0); // 10cm above the head
            attachment.attachBoneRotationOffset = Quaternion.Euler(0, 0, 0);
            
            Debug.Log($"Hat attachment configured: {attachment.prettyName}");
            Debug.Log($"Attached to: {attachment.attachBoneTarget}");
        }
    }
}
```

### Advanced Animation and IK Configuration

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class WeaponAttachmentSetup : MonoBehaviour
{
    public SpatialAvatarAttachment attachment;
    public Animator attachmentAnimator;
    public Transform rightHandIKTarget;
    public Transform leftHandIKTarget;
    
    private void ConfigureWeaponAttachment()
    {
        if (attachment == null)
            return;
        
        // Set up the attachment as a tool in the right hand
        attachment.prettyName = "Laser Sword";
        attachment.category = SpatialAvatarAttachment.Category.Tool;
        attachment.primarySlot = SpatialAvatarAttachment.Slot.RightHand;
        
        // Configure the animator for custom animations
        attachment.attachmentAnimatorType = SpatialAvatarAttachment.AttachmentAnimatorType.Custom;
        
        // Set up IK to position avatar hands on the weapon
        if (attachment.ikFeatureAvailable)
        {
            attachment.ikFeatureActive = true;
            attachment.ikTargetsEnabled = true;
            attachment.ikRightHandTarget = rightHandIKTarget;
            attachment.ikLeftHandTarget = leftHandIKTarget;
        }
        
        // Configure animation override settings
        if (attachment.overrideAvatarAnimations)
        {
            // Get animation settings for the attachment
            var animSettings = attachment.avatarAnimSettings;
            
            // Make the attachment invisible during jump animations
            animSettings.jump.attachmentVisible = false;
            
            // Use custom animation clips for specific actions
            animSettings.idle.overrideClip = Resources.Load<AnimationClip>("WeaponIdle");
            animSettings.run.overrideClip = Resources.Load<AnimationClip>("WeaponRun");
        }
    }
}
```

### Multi-Slot Attachment Example

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class WingsAttachmentSetup : MonoBehaviour
{
    public SpatialAvatarAttachment attachment;
    
    private void SetupWings()
    {
        if (attachment == null)
            return;
        
        // Basic attachment configuration
        attachment.prettyName = "Angel Wings";
        attachment.category = SpatialAvatarAttachment.Category.Accessory;
        
        // Primary attachment point is the back
        attachment.primarySlot = SpatialAvatarAttachment.Slot.BodyBack;
        
        // Set additional slots to make sure conflicting attachments aren't equipped
        // This ensures other back attachments won't be used at the same time
        attachment.additionalSlots = SpatialAvatarAttachment.SlotMask.BodyBack;
        
        // Connect to spine bone with offset positioning
        attachment.attachToBone = true;
        attachment.attachBoneTarget = HumanBodyBones.Spine;
        attachment.attachBoneOffset = new Vector3(0, 0.05f, -0.1f);
        attachment.attachBoneRotationOffset = Quaternion.Euler(0, 180, 0);
        
        // Verify configuration
        Debug.Log($"Wings attachment uses slots: {attachment.occupiedSlots}");
    }
}
```

### Creating Attachments with Custom Actions

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class PetAttachmentSetup : MonoBehaviour
{
    public SpatialAvatarAttachment attachment;
    
    private void ConfigurePet()
    {
        if (attachment == null || !attachment.customActionsFeatureAvailable)
            return;
        
        // Setup basic pet configuration
        attachment.prettyName = "Companion Dragon";
        attachment.category = SpatialAvatarAttachment.Category.Pet;
        attachment.primarySlot = SpatialAvatarAttachment.Slot.Pet;
        
        // Enable custom actions for the pet
        attachment.customActionsEnabled = true;
        
        // In a real implementation, you would configure the custom actions
        // through the Spatial Editor or by setting up animation events
        
        Debug.Log($"Pet attachment configured with {attachment.customActionsCount} custom actions");
        
        // Get animation settings and make the pet perform special animations
        // during certain avatar movements
        var animSettings = attachment.avatarAnimSettings;
        
        // Make the pet perform a flying animation when the player jumps
        animSettings.jumpInAir.attachmentVisible = true;
        animSettings.jumpInAir.overrideClip = Resources.Load<AnimationClip>("DragonFly");
        
        // Make the pet hide when player sits
        animSettings.sit.attachmentVisible = false;
    }
}
```

## Best Practices

1. **Slot Selection**: Choose the appropriate primary slot for the attachment based on its purpose. For example, weapons usually go in hand slots, hats on the head slot, etc.

2. **Bone Attachment**: When attaching to bones, use proper offset values to ensure the attachment sits correctly on different avatar body types. Test with different avatar proportions.

3. **Animation Integration**: When overriding avatar animations, ensure a smooth transition between standard animations and custom animations. Test all animation states to ensure proper blending.

4. **IK Configuration**: When using IK features, carefully position IK targets to create natural-looking poses. Avoid extreme rotations that could break immersion.

5. **Multiple Slots**: When an attachment occupies multiple slots, clearly define all of them in additionalSlots to prevent conflicts with other attachments.

6. **Category Assignment**: Choose the appropriate category for your attachment as it influences how the attachment behaves and is presented in the platform UI.

7. **Performance Considerations**: Keep attachment complexity appropriate for the target platform. Avoid excessive polygon counts or overly complex animations.

8. **Testing**: Test attachments with different avatar types, animations, and in various environments to ensure they display and behave correctly.

## Common Use Cases

1. **Cosmetic Accessories**: Hats, jewelry, glasses, and other wearable items that enhance avatar appearance without affecting functionality.

2. **Tools and Weapons**: Interactive items held in hands or attached to the body that may have functional purposes in gameplay.

3. **Backpacks and Wearables**: Larger items worn on the body that may contain storage or have other functional elements.

4. **Pets and Companions**: Animated creatures that follow or interact with the avatar.

5. **Visual Effects**: Auras, particle systems, or other visual enhancements attached to the avatar.

6. **Rideable Mounts**: Vehicles or creatures that avatars can ride, typically using IK to position the avatar correctly.

7. **Custom Animation Integrations**: Attachments that modify how the avatar moves or behaves in certain situations.

8. **Emote Props**: Special items that appear or animate during emotes or other specific actions.

## Related Components

- [SpatialAvatarAttachment.AttachmentAnimatorType](./SpatialAvatarAttachment.AttachmentAnimatorType.md): Enum defining the type of animator to use for an attachment.
- [SpatialAvatarAttachment.Category](./SpatialAvatarAttachment.Category.md): Enum defining the category of an attachment.
- [SpatialAvatarAttachment.Slot](./SpatialAvatarAttachment.Slot.md): Enum defining the slot on an avatar where an attachment can be placed.
- [SpatialAvatarAttachment.SlotMask](./SpatialAvatarAttachment.SlotMask.md): Enum used as a bitmask to represent multiple slots.
- [SpatialAvatarAttachmentSlotMaskExtensions](./SpatialAvatarAttachmentSlotMaskExtensions.md): Helper extensions for working with slots and slot masks.
- [SpatialAttachmentAvatarAnimSettings](./SpatialAttachmentAvatarAnimSettings.md): Settings for how attachments interact with avatar animations.
- [SpatialAvatar](./SpatialAvatar.md): The main component representing an avatar in the Spatial platform.
- [SpatialAvatarAnimation](./SpatialAvatarAnimation.md): Component for creating custom avatar animations.
- [SpatialAvatarAnimOverrides](./SpatialAvatarAnimOverrides.md): Component for overriding default avatar animations.