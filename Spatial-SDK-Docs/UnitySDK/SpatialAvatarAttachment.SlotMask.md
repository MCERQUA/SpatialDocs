# SpatialAvatarAttachment.SlotMask

Category: Core Components

Type: Enum

`SpatialAvatarAttachment.SlotMask` is a flag-based enumeration in the Spatial SDK that represents a set of avatar attachment slots. It is designed to work as a bitmask, allowing multiple slots to be combined into a single value. This is particularly useful for defining which slots an attachment occupies beyond its primary slot, as well as for checking slot availability and conflicts.

## Properties/Fields

| Value | Description |
| --- | --- |
| None | No slots are selected. Value: 0. |
| Hat | Mask for the hat slot (top of head). |
| FaceFront | Mask for the face front slot (front of face). |
| Neck | Mask for the neck slot. |
| LeftShoulder | Mask for the left shoulder slot. |
| RightShoulder | Mask for the right shoulder slot. |
| LeftHand | Mask for the left hand slot. |
| RightHand | Mask for the right hand slot. |
| BodyFront | Mask for the body front slot (chest area). |
| BodyBack | Mask for the body back slot (back area). |
| WaistFront | Mask for the waist front slot. |
| WaistCenter | Mask for the waist center slot. |
| WaistBack | Mask for the waist back slot. |
| LeftFoot | Mask for the left foot slot. |
| RightFoot | Mask for the right foot slot. |
| Pet | Mask for the pet slot. |
| Aura | Mask for the aura slot. |

## Usage Examples

### Setting Additional Slots for an Attachment

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class MultiSlotAttachmentExample : MonoBehaviour
{
    public SpatialAvatarAttachment armorAttachment;
    
    private void ConfigureArmorAttachment()
    {
        if (armorAttachment == null)
            return;
        
        // Configure basic properties
        armorAttachment.prettyName = "Full Plate Armor";
        armorAttachment.category = SpatialAvatarAttachment.Category.Accessory;
        
        // Set the primary slot to the body front
        armorAttachment.primarySlot = SpatialAvatarAttachment.Slot.BodyFront;
        
        // This armor occupies multiple slots - body front and back, shoulders, and waist
        armorAttachment.additionalSlots = 
            SpatialAvatarAttachment.SlotMask.BodyBack | 
            SpatialAvatarAttachment.SlotMask.LeftShoulder | 
            SpatialAvatarAttachment.SlotMask.RightShoulder |
            SpatialAvatarAttachment.SlotMask.WaistCenter;
        
        Debug.Log($"Armor configured to occupy slots: {armorAttachment.occupiedSlots}");
    }
}
```

### Checking for Slot Conflicts Between Attachments

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class SlotConflictChecker : MonoBehaviour
{
    // List of currently equipped attachments
    public List<SpatialAvatarAttachment> equippedAttachments = new List<SpatialAvatarAttachment>();
    
    // Get all occupied slot masks combined
    public SpatialAvatarAttachment.SlotMask GetOccupiedSlots()
    {
        SpatialAvatarAttachment.SlotMask occupiedSlots = SpatialAvatarAttachment.SlotMask.None;
        
        foreach (var attachment in equippedAttachments)
        {
            // Add the primary slot
            occupiedSlots |= attachment.primarySlot.ToSlotMask();
            
            // Add any additional slots
            occupiedSlots |= attachment.additionalSlots;
        }
        
        return occupiedSlots;
    }
    
    // Check if an attachment can be equipped without conflicts
    public bool CanEquipAttachment(SpatialAvatarAttachment attachment)
    {
        if (attachment == null)
            return false;
        
        // Get currently occupied slots
        SpatialAvatarAttachment.SlotMask occupiedSlots = GetOccupiedSlots();
        
        // Calculate all slots this attachment would occupy
        SpatialAvatarAttachment.SlotMask attachmentSlots = 
            attachment.primarySlot.ToSlotMask() | attachment.additionalSlots;
        
        // Check for any overlap between occupied slots and attachment slots
        bool hasConflict = (occupiedSlots & attachmentSlots) != SpatialAvatarAttachment.SlotMask.None;
        
        if (hasConflict)
        {
            Debug.Log($"Cannot equip {attachment.prettyName} - slot conflict detected");
            return false;
        }
        
        Debug.Log($"Can equip {attachment.prettyName} - no slot conflicts");
        return true;
    }
}
```

### Creating a Slot Management System

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class AvatarSlotManager : MonoBehaviour
{
    // Dictionary to track which attachments are in which slots
    private Dictionary<SpatialAvatarAttachment.SlotMask, SpatialAvatarAttachment> slotAssignments = 
        new Dictionary<SpatialAvatarAttachment.SlotMask, SpatialAvatarAttachment>();
    
    // Equip an attachment, replacing any conflicting attachments
    public void EquipAttachment(SpatialAvatarAttachment attachment)
    {
        if (attachment == null)
            return;
        
        // Get all slots this attachment will occupy
        SpatialAvatarAttachment.SlotMask newSlots = 
            attachment.primarySlot.ToSlotMask() | attachment.additionalSlots;
        
        // Check for and remove any attachments with conflicting slots
        List<SpatialAvatarAttachment.SlotMask> conflictingSlots = new List<SpatialAvatarAttachment.SlotMask>();
        
        foreach (var entry in slotAssignments)
        {
            // If there's any overlap between this registered slot and the new attachment's slots
            if ((entry.Key & newSlots) != SpatialAvatarAttachment.SlotMask.None)
            {
                UnequipAttachment(entry.Value);
                conflictingSlots.Add(entry.Key);
            }
        }
        
        // Remove conflicting entries
        foreach (var slot in conflictingSlots)
        {
            slotAssignments.Remove(slot);
        }
        
        // Register the new attachment with its slots
        slotAssignments[newSlots] = attachment;
        
        Debug.Log($"Equipped {attachment.prettyName} in slots: {newSlots}");
    }
    
    // Unequip a specific attachment
    public void UnequipAttachment(SpatialAvatarAttachment attachment)
    {
        if (attachment == null)
            return;
        
        // Find the slot mask this attachment is registered under
        SpatialAvatarAttachment.SlotMask? foundSlot = null;
        
        foreach (var entry in slotAssignments)
        {
            if (entry.Value == attachment)
            {
                foundSlot = entry.Key;
                break;
            }
        }
        
        // Remove the attachment if found
        if (foundSlot.HasValue)
        {
            slotAssignments.Remove(foundSlot.Value);
            Debug.Log($"Unequipped {attachment.prettyName} from slots: {foundSlot.Value}");
        }
    }
}
```

## Best Practices

1. **Mask Usage for Multiple Slots**: Use SlotMask with bitwise operations to efficiently represent and check multiple slots simultaneously.

2. **Primary Slot Conversion**: Always use the ToSlotMask() extension method to convert a Slot to its corresponding SlotMask when combining with additional slots.

3. **Conflict Detection**: When working with multiple attachments, implement proper slot conflict detection using bitwise AND operations with slot masks.

4. **Comprehensive Slot Tracking**: Track all occupied slots (both primary and additional) to ensure accurate conflict detection and management.

5. **Clear Slot Requirements**: When designing attachments, clearly define which slots they need to occupy beyond their primary slot.

6. **Minimal Slot Usage**: Try to use the minimum necessary slots for each attachment to reduce potential conflicts with other attachments.

7. **Bitwise Operations**: Become familiar with bitwise operations (|, &, ^, ~) for effective slot mask manipulation and checking.

## Common Use Cases

1. **Multi-Slot Attachments**: Creating armor, outfits, or large accessories that naturally occupy multiple avatar slots.

2. **Slot Availability Checking**: Determining if an attachment can be equipped by checking if its required slots are available.

3. **Conflict Resolution**: Implementing systems to resolve conflicts when a new attachment requires slots already occupied by existing attachments.

4. **Equipment Management**: Building inventory and equipment systems that intelligently manage avatar attachment slots.

5. **Outfit Presets**: Creating preset configurations of compatible attachments that don't have slot conflicts.

6. **Attachment Requirements**: Defining which slots must be empty for an attachment to be equipped properly.

7. **Layered Clothing Systems**: Implementing complex layered clothing systems where different clothing pieces occupy different but potentially overlapping slots.

## Related Components

- [SpatialAvatarAttachment](./SpatialAvatarAttachment.md): The main component for creating avatar attachments.
- [SpatialAvatarAttachment.Slot](./SpatialAvatarAttachment.Slot.md): Enum defining the slot on an avatar where an attachment can be placed.
- [SpatialAvatarAttachmentSlotMaskExtensions](./SpatialAvatarAttachmentSlotMaskExtensions.md): Helper extensions for working with slots and slot masks.
- [SpatialAvatarAttachment.Category](./SpatialAvatarAttachment.Category.md): Enum defining the category of an attachment.