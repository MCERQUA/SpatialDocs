# SpatialAvatarAttachment.Slot

Category: Core Components

Type: Enum

`SpatialAvatarAttachment.Slot` is an enumeration that defines the specific positions on an avatar where attachments can be placed in the Spatial SDK. It provides a standardized set of attachment points that correspond to different body parts or areas around the avatar. Each slot represents a unique location that can hold a single primary attachment at a time.

## Properties/Fields

| Value | Description |
| --- | --- |
| None | No specific slot is defined. Used for attachments that don't occupy a standard position or for special cases like rideables. |
| Hat | Top of the head/scalp area for hats, helmets, hair styles, and other head-top accessories. |
| FaceFront | Front of the face for masks, glasses, facial decorations, and other face-worn items. |
| Neck | Neck area for necklaces, collars, scarves, and other neck accessories. |
| LeftShoulder | Left shoulder area for shoulder pads, epaulets, parrot companions, and other left shoulder items. |
| RightShoulder | Right shoulder area for shoulder pads, epaulets, and other right shoulder items. |
| LeftHand | Left hand for handheld items, weapons, tools, and other objects gripped with the left hand. |
| RightHand | Right hand for handheld items, weapons, tools, and other objects gripped with the right hand. |
| BodyFront | Front torso area for shirts, chest armor, medals, and other front body decorations. |
| BodyBack | Back torso area for backpacks, capes, wings, and other back-worn items. |
| WaistFront | Front waist area for belt buckles, aprons, and other front waist decorations. |
| WaistCenter | Center waist area for belts, utility pouches, and other waist-centered items. |
| WaistBack | Back waist area for pouches, weapon sheaths, and other back waist accessories. |
| LeftFoot | Left foot for shoes, boots, leg armor, and other left foot gear. |
| RightFoot | Right foot for shoes, boots, leg armor, and other right foot gear. |
| Pet | Special slot for pet companions that follow the avatar. |
| Aura | Special slot for visual effects and auras that surround the avatar. |

## Extension Methods

| Method | Description |
| --- | --- |
| ToSlotMask(Slot) | Converts a Slot enum value to its corresponding SlotMask enum value. This extension method is defined in SpatialAvatarAttachmentSlotMaskExtensions. |

## Usage Examples

### Basic Slot Assignment for Attachments

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class BasicSlotExample : MonoBehaviour
{
    public SpatialAvatarAttachment crownAttachment;
    public SpatialAvatarAttachment swordAttachment;
    public SpatialAvatarAttachment capeAttachment;
    
    private void AssignBasicSlots()
    {
        // Assign a crown to the hat slot
        if (crownAttachment != null)
        {
            crownAttachment.prettyName = "Royal Crown";
            crownAttachment.primarySlot = SpatialAvatarAttachment.Slot.Hat;
            Debug.Log("Crown assigned to Hat slot");
        }
        
        // Assign a sword to the right hand slot
        if (swordAttachment != null)
        {
            swordAttachment.prettyName = "Magic Sword";
            swordAttachment.primarySlot = SpatialAvatarAttachment.Slot.RightHand;
            Debug.Log("Sword assigned to RightHand slot");
        }
        
        // Assign a cape to the back slot
        if (capeAttachment != null)
        {
            capeAttachment.prettyName = "Heroic Cape";
            capeAttachment.primarySlot = SpatialAvatarAttachment.Slot.BodyBack;
            Debug.Log("Cape assigned to BodyBack slot");
        }
    }
}
```

### Checking for Occupied Slots

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class SlotAvailabilityChecker : MonoBehaviour
{
    // Reference to all currently equipped attachments
    public List<SpatialAvatarAttachment> equippedAttachments = new List<SpatialAvatarAttachment>();
    
    // Check if a specific slot is available
    public bool IsSlotAvailable(SpatialAvatarAttachment.Slot slot)
    {
        foreach (var attachment in equippedAttachments)
        {
            // Check if this is the primary slot for any attachment
            if (attachment.primarySlot == slot)
            {
                Debug.Log($"Slot {slot} is occupied by {attachment.prettyName}");
                return false;
            }
            
            // Check if this slot is included in additional slots
            if ((attachment.additionalSlots & attachment.primarySlot.ToSlotMask()) != 0)
            {
                Debug.Log($"Slot {slot} is occupied as an additional slot by {attachment.prettyName}");
                return false;
            }
        }
        
        Debug.Log($"Slot {slot} is available");
        return true;
    }
    
    // Try to equip an attachment if its slot is available
    public bool TryEquipAttachment(SpatialAvatarAttachment attachment)
    {
        if (attachment == null)
            return false;
        
        if (IsSlotAvailable(attachment.primarySlot))
        {
            equippedAttachments.Add(attachment);
            Debug.Log($"Equipped {attachment.prettyName} in slot {attachment.primarySlot}");
            return true;
        }
        
        Debug.Log($"Cannot equip {attachment.prettyName}, slot {attachment.primarySlot} is occupied");
        return false;
    }
}
```

### Working with Special Slots

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class SpecialSlotExample : MonoBehaviour
{
    public SpatialAvatarAttachment auraEffect;
    public SpatialAvatarAttachment petCompanion;
    public SpatialAvatarAttachment hoverBike;
    
    private void ConfigureSpecialSlots()
    {
        // Configure an aura effect
        if (auraEffect != null)
        {
            auraEffect.prettyName = "Fire Aura";
            auraEffect.category = SpatialAvatarAttachment.Category.Aura;
            auraEffect.primarySlot = SpatialAvatarAttachment.Slot.Aura;
            Debug.Log("Aura effect assigned to Aura slot");
        }
        
        // Configure a pet companion
        if (petCompanion != null)
        {
            petCompanion.prettyName = "Robot Buddy";
            petCompanion.category = SpatialAvatarAttachment.Category.Pet;
            petCompanion.primarySlot = SpatialAvatarAttachment.Slot.Pet;
            Debug.Log("Pet companion assigned to Pet slot");
        }
        
        // Configure a rideable (doesn't use a standard slot)
        if (hoverBike != null)
        {
            hoverBike.prettyName = "Hover Bike";
            hoverBike.category = SpatialAvatarAttachment.Category.Rideable;
            hoverBike.primarySlot = SpatialAvatarAttachment.Slot.None;
            Debug.Log("Hover bike configured with no specific slot");
        }
    }
}
```

## Best Practices

1. **Match Slot to Attachment Type**: Choose the appropriate slot that matches the attachment's visual appearance and purpose. For example, hats should use the Hat slot, while handheld tools should use LeftHand or RightHand slots.

2. **Handle Special Cases**: Use special slots like Pet and Aura for their intended purposes to ensure correct behavior. These slots have specific functionality in the Spatial platform.

3. **Consider Visibility**: Be aware that some slots may be more visible than others depending on the camera angle. Consider this when designing attachments for specific slots.

4. **Standard Positioning**: While attachments can be positioned with offsets, try to design them to look natural in their designated slots without requiring excessive adjustments.

5. **None Slot Usage**: Only use the None slot for special cases like rideables or for custom attachment scenarios where standard slots don't apply.

6. **Slot Conflicts**: Be mindful of slot conflicts when multiple attachments might want to use the same slot. Design your systems to handle these conflicts gracefully.

## Common Use Cases

1. **Cosmetic Equipment**: Using appropriate slots for wearable items like hats, glasses, jewelry, and clothing.

2. **Weapons and Tools**: Assigning handheld items to the LeftHand and RightHand slots for avatars to grip properly.

3. **Back Accessories**: Using the BodyBack slot for items like backpacks, wings, jet packs, and similar accessories.

4. **Special Effects**: Using the Aura slot for particle effects, glows, and other visual enhancements that surround the avatar.

5. **Pet Companions**: Using the Pet slot for companion creatures or objects that follow and interact with the avatar.

6. **Outfit Systems**: Creating complete outfits by assigning multiple attachments to different slots simultaneously.

7. **Slot Availability Checking**: Implementing systems to check if slots are available before attempting to equip new attachments.

## Related Components

- [SpatialAvatarAttachment](./SpatialAvatarAttachment.md): The main component for creating avatar attachments.
- [SpatialAvatarAttachment.SlotMask](./SpatialAvatarAttachment.SlotMask.md): Enum used as a bitmask to represent multiple slots.
- [SpatialAvatarAttachmentSlotMaskExtensions](./SpatialAvatarAttachmentSlotMaskExtensions.md): Helper extensions for working with slots and slot masks.
- [SpatialAvatarAttachment.Category](./SpatialAvatarAttachment.Category.md): Enum defining the category of an attachment.