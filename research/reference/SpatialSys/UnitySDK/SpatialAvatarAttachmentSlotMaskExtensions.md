# SpatialAvatarAttachmentSlotMaskExtensions

Category: Core Components

Type: Class

`SpatialAvatarAttachmentSlotMaskExtensions` is a utility class in the Spatial SDK that provides extension methods for working with avatar attachment slots and slot masks. It offers convenient conversion functionality between the `SpatialAvatarAttachment.Slot` and `SpatialAvatarAttachment.SlotMask` enum types, facilitating the process of working with and combining avatar attachment slots.

## Methods

| Method | Description |
| --- | --- |
| ToSlotMask(this SpatialAvatarAttachment.Slot slot) | Converts a single Slot enum value to its equivalent SlotMask enum value. This enables seamless conversion between the two related enum types, allowing Slot values to be used in bitwise operations with SlotMask values. |

## Usage Examples

### Basic Slot to SlotMask Conversion

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class SlotConversionExample : MonoBehaviour
{
    public void DemonstrateSlotConversion()
    {
        // Convert individual slots to their mask equivalents
        SpatialAvatarAttachment.Slot hatSlot = SpatialAvatarAttachment.Slot.Hat;
        SpatialAvatarAttachment.SlotMask hatMask = hatSlot.ToSlotMask();
        
        SpatialAvatarAttachment.Slot rightHandSlot = SpatialAvatarAttachment.Slot.RightHand;
        SpatialAvatarAttachment.SlotMask rightHandMask = rightHandSlot.ToSlotMask();
        
        Debug.Log($"Hat slot converted to mask: {hatMask}");
        Debug.Log($"Right hand slot converted to mask: {rightHandMask}");
        
        // Convert the None slot
        SpatialAvatarAttachment.SlotMask noneMask = SpatialAvatarAttachment.Slot.None.ToSlotMask();
        Debug.Log($"None slot converted to mask: {noneMask}");
    }
}
```

### Combining Primary and Additional Slots

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class CombinedSlotExample : MonoBehaviour
{
    public SpatialAvatarAttachment wingAttachment;
    
    private void ConfigureWings()
    {
        if (wingAttachment == null)
            return;
            
        // Set up basic wing attachment properties
        wingAttachment.prettyName = "Dragon Wings";
        wingAttachment.category = SpatialAvatarAttachment.Category.Accessory;
        
        // Set primary slot to the body back
        wingAttachment.primarySlot = SpatialAvatarAttachment.Slot.BodyBack;
        
        // Convert primary slot to SlotMask and combine with additional slots
        SpatialAvatarAttachment.SlotMask primarySlotMask = wingAttachment.primarySlot.ToSlotMask();
        
        // Wings are large and also occupy the shoulders
        wingAttachment.additionalSlots = primarySlotMask | 
                                         SpatialAvatarAttachment.SlotMask.LeftShoulder | 
                                         SpatialAvatarAttachment.SlotMask.RightShoulder;
        
        Debug.Log($"Wings occupy slots: {wingAttachment.occupiedSlots}");
    }
}
```

### Creating Slot Conflict Detection System

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class AttachmentManager : MonoBehaviour
{
    private List<SpatialAvatarAttachment> equippedAttachments = new List<SpatialAvatarAttachment>();
    
    // Check if an attachment conflicts with already equipped attachments
    public bool HasSlotConflict(SpatialAvatarAttachment newAttachment)
    {
        if (newAttachment == null)
            return false;
        
        // Calculate total occupied slots from existing attachments
        SpatialAvatarAttachment.SlotMask occupiedSlots = SpatialAvatarAttachment.SlotMask.None;
        
        foreach (var attachment in equippedAttachments)
        {
            // Combine occupied slots with primarySlot converted to SlotMask
            occupiedSlots |= attachment.primarySlot.ToSlotMask();
            
            // Add any additional slots
            occupiedSlots |= attachment.additionalSlots;
        }
        
        // Calculate slots needed by new attachment
        SpatialAvatarAttachment.SlotMask newAttachmentSlots = 
            newAttachment.primarySlot.ToSlotMask() | newAttachment.additionalSlots;
        
        // Check for overlap between occupied and new slots
        bool hasOverlap = (occupiedSlots & newAttachmentSlots) != SpatialAvatarAttachment.SlotMask.None;
        
        if (hasOverlap)
            Debug.Log($"Attachment {newAttachment.prettyName} has slot conflicts");
        else
            Debug.Log($"Attachment {newAttachment.prettyName} has no slot conflicts");
            
        return hasOverlap;
    }
    
    // Try to equip an attachment, succeeding only if there are no conflicts
    public bool TryEquipAttachment(SpatialAvatarAttachment attachment)
    {
        if (attachment == null)
            return false;
            
        if (!HasSlotConflict(attachment))
        {
            equippedAttachments.Add(attachment);
            Debug.Log($"Successfully equipped {attachment.prettyName}");
            return true;
        }
        
        Debug.Log($"Could not equip {attachment.prettyName} due to slot conflicts");
        return false;
    }
}
```

## Best Practices

1. **Consistent Conversion**: Always use the ToSlotMask() extension method when converting between Slot and SlotMask to ensure consistent behavior.

2. **Combining with Additional Slots**: When working with both primary and additional slots, convert the primary slot to a SlotMask before combining with additional slots using bitwise operations.

3. **Comprehensive Slot Tracking**: Use ToSlotMask() in conjunction with additionalSlots to track all slots an attachment occupies, both primary and additional.

4. **Slot Conflict Management**: Implement robust slot conflict detection using ToSlotMask() to ensure attachments don't occupy the same slots.

5. **Clear Code**: Using ToSlotMask() explicitly makes it clear in your code that you're converting between the two enum types, improving readability.

## Common Use Cases

1. **Attachment Configuration**: Converting primary slots to slot masks when configuring avatar attachments with multiple occupied slots.

2. **Equipment Systems**: Creating equipment management systems that track occupied slots and prevent conflicts.

3. **Slot Availability Checking**: Checking if specific slots are available for new attachments.

4. **Combined Slot Operations**: Performing bitwise operations on collections of slots from different sources.

5. **Attachment Rules**: Implementing rules about which combinations of attachments can be equipped simultaneously.

6. **UI Systems**: Creating user interfaces that visualize which attachment slots are occupied and which are available.

## Related Components

- [SpatialAvatarAttachment](./SpatialAvatarAttachment.md): The main component for creating avatar attachments.
- [SpatialAvatarAttachment.Slot](./SpatialAvatarAttachment.Slot.md): Enum defining the slot on an avatar where an attachment can be placed.
- [SpatialAvatarAttachment.SlotMask](./SpatialAvatarAttachment.SlotMask.md): Enum used as a bitmask to represent multiple slots.
- [SpatialAvatarAttachment.Category](./SpatialAvatarAttachment.Category.md): Enum defining the category of an attachment.