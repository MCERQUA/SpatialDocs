# SpatialAvatarAttachment.Category

Category: Core Components

Type: Enum

`SpatialAvatarAttachment.Category` is an enumeration that defines the categorical purpose of an avatar attachment in the Spatial SDK. It helps organize and classify different types of attachments based on their function and appearance, which can influence how they're presented in the UI and how they behave within the Spatial platform.

## Properties/Fields

| Value | Description |
| --- | --- |
| Unspecified | Default category when no specific category has been assigned. Used when the attachment doesn't clearly fit into other categories. |
| Accessory | Decorative items that enhance avatar appearance, such as hats, glasses, jewelry, and other wearable cosmetics. |
| Tool | Functional items that avatars can use for specific purposes, such as weapons, implements, devices, or other interactive objects. |
| Aura | Visual effects that surround or emanate from the avatar, such as particle systems, glow effects, or other ambient visuals. |
| Pet | Companion creatures or objects that follow or interact with the avatar. Typically animated and may have their own behaviors. |
| Rideable | Objects that avatars can mount and ride, such as vehicles, mounts, or other transportation items. May use IK to position the avatar appropriately. |

## Usage Examples

### Setting Attachment Categories

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class AttachmentCategoryExample : MonoBehaviour
{
    public SpatialAvatarAttachment hatAttachment;
    public SpatialAvatarAttachment swordAttachment;
    public SpatialAvatarAttachment glowEffectAttachment;
    public SpatialAvatarAttachment dragonPetAttachment;
    public SpatialAvatarAttachment motorcycleAttachment;
    
    private void ConfigureAttachments()
    {
        // Configure a hat as an accessory
        if (hatAttachment != null)
        {
            hatAttachment.prettyName = "Wizard Hat";
            hatAttachment.category = SpatialAvatarAttachment.Category.Accessory;
            hatAttachment.primarySlot = SpatialAvatarAttachment.Slot.Hat;
        }
        
        // Configure a sword as a tool
        if (swordAttachment != null)
        {
            swordAttachment.prettyName = "Energy Sword";
            swordAttachment.category = SpatialAvatarAttachment.Category.Tool;
            swordAttachment.primarySlot = SpatialAvatarAttachment.Slot.RightHand;
        }
        
        // Configure a glow effect as an aura
        if (glowEffectAttachment != null)
        {
            glowEffectAttachment.prettyName = "Mystic Aura";
            glowEffectAttachment.category = SpatialAvatarAttachment.Category.Aura;
            glowEffectAttachment.primarySlot = SpatialAvatarAttachment.Slot.Aura;
        }
        
        // Configure a dragon as a pet
        if (dragonPetAttachment != null)
        {
            dragonPetAttachment.prettyName = "Baby Dragon";
            dragonPetAttachment.category = SpatialAvatarAttachment.Category.Pet;
            dragonPetAttachment.primarySlot = SpatialAvatarAttachment.Slot.Pet;
        }
        
        // Configure a motorcycle as a rideable
        if (motorcycleAttachment != null)
        {
            motorcycleAttachment.prettyName = "Hover Bike";
            motorcycleAttachment.category = SpatialAvatarAttachment.Category.Rideable;
            motorcycleAttachment.primarySlot = SpatialAvatarAttachment.Slot.None; // Rideables often don't use standard slots
        }
    }
}
```

### Creating a Category-Specific Attachment System

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class AttachmentCatalog : MonoBehaviour
{
    // List of all available attachments
    public List<SpatialAvatarAttachment> availableAttachments = new List<SpatialAvatarAttachment>();
    
    // Get all attachments of a specific category
    public List<SpatialAvatarAttachment> GetAttachmentsByCategory(SpatialAvatarAttachment.Category category)
    {
        List<SpatialAvatarAttachment> filteredAttachments = new List<SpatialAvatarAttachment>();
        
        foreach (var attachment in availableAttachments)
        {
            if (attachment.category == category)
            {
                filteredAttachments.Add(attachment);
            }
        }
        
        return filteredAttachments;
    }
    
    // Example usage of the filtering function
    public void DisplayAccessoryCatalog()
    {
        List<SpatialAvatarAttachment> accessories = GetAttachmentsByCategory(SpatialAvatarAttachment.Category.Accessory);
        
        Debug.Log($"Found {accessories.Count} accessories:");
        foreach (var accessory in accessories)
        {
            Debug.Log($"- {accessory.prettyName} (Slot: {accessory.primarySlot})");
        }
    }
}
```

## Best Practices

1. **Appropriate Categorization**: Choose the category that best represents the attachment's primary function to ensure it's presented appropriately in the UI.

2. **Unspecified Usage**: Only use the Unspecified category when the attachment truly doesn't fit into any other category. This should be rare as most attachments have a clear purpose.

3. **Category Consistency**: Maintain consistent categorization across similar attachments to create a logical organization system for users.

4. **UI Considerations**: Remember that the category may influence how the attachment appears in menus and catalogs, so choose carefully to ensure users can find attachments easily.

5. **Functionality Alignment**: Ensure the attachment's functionality aligns with its category. For example, a Tool should generally have some interactive elements, while an Accessory might be purely cosmetic.

## Common Use Cases

1. **Accessory Category**: Used for clothing items, decorative accessories, and cosmetic enhancements that don't have functional gameplay effects.

2. **Tool Category**: Used for weapons, implements, gadgets, and other items that the avatar can interact with or use for specific purposes.

3. **Aura Category**: Used for particle effects, glows, trailing effects, and other visual enhancements that surround the avatar.

4. **Pet Category**: Used for companion creatures, robots, or other entities that follow or interact with the avatar in some way.

5. **Rideable Category**: Used for vehicles, mounts, and other transportation objects that the avatar can sit on or operate.

6. **Organization Systems**: Creating filtered views or catalogs of attachments based on their categories to improve user experience.

## Related Components

- [SpatialAvatarAttachment](./SpatialAvatarAttachment.md): The main component for creating avatar attachments.
- [SpatialAvatarAttachment.Slot](./SpatialAvatarAttachment.Slot.md): Enum defining the slot on an avatar where an attachment can be placed.
- [SpatialAvatarAttachment.SlotMask](./SpatialAvatarAttachment.SlotMask.md): Enum used as a bitmask to represent multiple slots.
- [SpatialAvatarAttachment.AttachmentAnimatorType](./SpatialAvatarAttachment.AttachmentAnimatorType.md): Enum defining the type of animator to use for an attachment.