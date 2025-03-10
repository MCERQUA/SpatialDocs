# SpatialCoreGUITypeFlags

Category: Core GUI Service Related

Enum: Provides a flags representation of core GUI types for batch operations.

## Overview
SpatialCoreGUITypeFlags is a flags enum that represents different types of core GUI elements in the Spatial platform. It's primarily used with the ICoreGUIService to enable, disable, open, or close multiple GUI elements in a single operation. This flags enum corresponds to the SpatialCoreGUIType enum but allows for combining multiple values using bitwise operations.

## Properties/Fields

| Value | Description |
| --- | --- |
| None | No GUI types selected. Used as a default value. |
| Backpack | Flag for the backpack (inventory) GUI. |
| Chat | Flag for the chat window GUI. |
| Emote | Flag for the emote system GUI. |
| ParticipantsList | Flag for the participants list GUI. |
| QuestSystem | Flag for the quest system GUI. |
| UniversalShop | Flag for the universal shop GUI (for platform-wide items). |
| WorldShop | Flag for the world shop GUI (for space-specific items). |
| Shop | Combined flag for both UniversalShop and WorldShop. |
| All | Combined flag representing all available GUI types. |

## Usage Examples

```csharp
// Example 1: Managing multiple GUI elements at once
// This example demonstrates how to enable, disable, open, or close multiple GUI elements

public class GUIController : MonoBehaviour
{
    public void DisableAllShopInterfaces()
    {
        // Disable both shop interfaces at once using the Shop flag
        SpatialBridge.coreGUIService.SetCoreGUIEnabled(SpatialCoreGUITypeFlags.Shop, false);
    }
    
    public void SetupCombatMode()
    {
        // Disable non-essential GUIs during combat
        SpatialCoreGUITypeFlags nonEssentialGUIs = 
            SpatialCoreGUITypeFlags.Backpack | 
            SpatialCoreGUITypeFlags.Chat | 
            SpatialCoreGUITypeFlags.Emote | 
            SpatialCoreGUITypeFlags.ParticipantsList | 
            SpatialCoreGUITypeFlags.Shop;
        
        SpatialBridge.coreGUIService.SetCoreGUIEnabled(nonEssentialGUIs, false);
    }
    
    public void SetupExplorationMode()
    {
        // Enable all GUIs for exploration
        SpatialBridge.coreGUIService.SetCoreGUIEnabled(SpatialCoreGUITypeFlags.All, true);
    }
    
    public void MinimizeAllGUIs()
    {
        // Close all open GUIs at once
        SpatialBridge.coreGUIService.SetCoreGUIOpen(SpatialCoreGUITypeFlags.All, false);
    }
}
```

```csharp
// Example 2: Creating custom GUI profiles
// This example shows how to create and apply profiles of GUI visibility

public class GUIProfileManager : MonoBehaviour
{
    // Define GUI profiles as flag combinations
    private SpatialCoreGUITypeFlags minimalProfile = SpatialCoreGUITypeFlags.QuestSystem;
    
    private SpatialCoreGUITypeFlags socialProfile = 
        SpatialCoreGUITypeFlags.Chat | 
        SpatialCoreGUITypeFlags.Emote | 
        SpatialCoreGUITypeFlags.ParticipantsList;
    
    private SpatialCoreGUITypeFlags shoppingProfile = 
        SpatialCoreGUITypeFlags.Backpack | 
        SpatialCoreGUITypeFlags.Shop;
    
    private SpatialCoreGUITypeFlags fullProfile = SpatialCoreGUITypeFlags.All;
    
    // Apply a specific profile
    public void ApplyGUIProfile(string profileName)
    {
        // First disable all GUIs
        SpatialBridge.coreGUIService.SetCoreGUIEnabled(SpatialCoreGUITypeFlags.All, false);
        
        // Then enable only those in the selected profile
        SpatialCoreGUITypeFlags profileFlags = SpatialCoreGUITypeFlags.None;
        
        switch (profileName.ToLower())
        {
            case "minimal":
                profileFlags = minimalProfile;
                break;
            case "social":
                profileFlags = socialProfile;
                break;
            case "shopping":
                profileFlags = shoppingProfile;
                break;
            case "full":
                profileFlags = fullProfile;
                break;
        }
        
        // Apply the profile
        SpatialBridge.coreGUIService.SetCoreGUIEnabled(profileFlags, true);
        
        // Also open the enabled GUIs
        SpatialBridge.coreGUIService.SetCoreGUIOpen(profileFlags, true);
    }
    
    // Create a custom profile from individual GUI types
    public SpatialCoreGUITypeFlags CreateCustomProfile(bool includeBackpack, bool includeChat, 
                                                       bool includeEmote, bool includeParticipants, 
                                                       bool includeQuests, bool includeShops)
    {
        SpatialCoreGUITypeFlags customProfile = SpatialCoreGUITypeFlags.None;
        
        if (includeBackpack) customProfile |= SpatialCoreGUITypeFlags.Backpack;
        if (includeChat) customProfile |= SpatialCoreGUITypeFlags.Chat;
        if (includeEmote) customProfile |= SpatialCoreGUITypeFlags.Emote;
        if (includeParticipants) customProfile |= SpatialCoreGUITypeFlags.ParticipantsList;
        if (includeQuests) customProfile |= SpatialCoreGUITypeFlags.QuestSystem;
        if (includeShops) customProfile |= SpatialCoreGUITypeFlags.Shop;
        
        return customProfile;
    }
}
```

## Best Practices

1. **Use flags for batch operations** - Use SpatialCoreGUITypeFlags for operations affecting multiple GUI elements at once rather than making individual calls.

2. **Use bitwise operations effectively** - Combine flags using the bitwise OR operator (|) and check for specific flags using the bitwise AND operator (&).

3. **Consider using preset combinations** - The built-in combinations like SpatialCoreGUITypeFlags.Shop and SpatialCoreGUITypeFlags.All can simplify your code.

4. **Create meaningful flag groups** - Group related GUI elements into logical combinations that make sense for your specific use cases.

5. **Be consistent with state changes** - When using flags to change multiple GUI states at once, ensure the operation makes logical sense for all affected elements.

6. **Test flag combinations** - Verify that your flag combinations work as expected in all scenarios, especially when dealing with complex combinations.

## Common Use Cases

1. **UI Profiles** - Creating different UI configurations for different gameplay modes or player preferences.

2. **Batch state changes** - Enabling, disabling, opening, or closing multiple related GUI elements in a single operation.

3. **Game state transitions** - Changing UI visibility during transitions between different game states, such as exploration, combat, or cutscenes.

4. **UI cleanup** - Quickly hiding or showing all UI elements with a single call using SpatialCoreGUITypeFlags.All.

5. **Category management** - Managing related UI elements together, such as handling all shop interfaces using SpatialCoreGUITypeFlags.Shop.

6. **Progressive UI** - Gradually introducing UI elements to new players by starting with a minimal set and expanding as they progress.

## Related Components

- [ICoreGUIService](./ICoreGUIService.md) - The service that uses SpatialCoreGUITypeFlags for batch operations on GUI elements.
- [SpatialCoreGUIType](./SpatialCoreGUIType.md) - The non-flags enum that corresponds to these flags, used for checking individual GUI states.
- [SpatialCoreGUIState](./SpatialCoreGUIState.md) - Defines the possible states for GUI elements (enabled/open).
- [CoreGUIUtility](./CoreGUIUtility.md) - Provides the ToFlag() method to convert from SpatialCoreGUIType to SpatialCoreGUITypeFlags.