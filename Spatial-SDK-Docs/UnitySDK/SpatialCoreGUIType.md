# SpatialCoreGUIType

Category: Core GUI Service Related

Enum: Defines the different types of core GUI elements available in the Spatial platform.

## Overview
SpatialCoreGUIType is an enum that defines the different types of built-in GUI elements provided by the Spatial platform. These represent the standard UI components that users can interact with during their experience. This enum is used with ICoreGUIService to reference specific GUI elements when checking their state.

## Properties/Fields

| Value | Description |
| --- | --- |
| None | No GUI type specified. Used as a default value. |
| Backpack | The GUI showing the backpack (the user's inventory). Allows users to access and manage items they've collected. |
| Chat | The GUI showing the chat window. Allows users to communicate with other participants in the space. |
| Emote | The GUI for the emote system, showing the list of available emotes. Allows users to express themselves through animations. |
| ParticipantsList | The GUI showing the list of participants in the space. Shows who is currently present in the experience. |
| QuestSystem | The GUI for the quest system, showing the current quest and progress. Allows users to track objectives and rewards. |
| UniversalShop | The universal shop GUI. This shop is for items that can be used in all spaces on the platform. |
| WorldShop | The in-space shop GUI. This shop is specific to the current space and offers items only usable within that space. |

## Usage Examples

```csharp
// Example 1: Checking and modifying specific GUI states
// This example shows how to check and modify the state of specific GUI elements

public class GUIManager : MonoBehaviour
{
    private void Start()
    {
        // Check if the chat is currently enabled
        SpatialCoreGUIState chatState = SpatialBridge.coreGUIService.GetCoreGUIState(SpatialCoreGUIType.Chat);
        bool isChatEnabled = (chatState & SpatialCoreGUIState.Enabled) != 0;
        
        Debug.Log($"Chat is currently {(isChatEnabled ? "enabled" : "disabled")}");
        
        // Convert the GUI type to a flag for using with service methods
        SpatialCoreGUITypeFlags chatFlag = SpatialCoreGUIType.Chat.ToFlag();
        
        // Toggle the chat's open state
        bool isChatOpen = (chatState & SpatialCoreGUIState.Open) != 0;
        SpatialBridge.coreGUIService.SetCoreGUIOpen(chatFlag, !isChatOpen);
    }
    
    public void OpenInventory()
    {
        // Open the backpack GUI
        SpatialCoreGUITypeFlags backpackFlag = SpatialCoreGUIType.Backpack.ToFlag();
        SpatialBridge.coreGUIService.SetCoreGUIOpen(backpackFlag, true);
    }
    
    public void ToggleParticipantsList()
    {
        // Get current state of participants list
        SpatialCoreGUIState participantsState = SpatialBridge.coreGUIService.GetCoreGUIState(SpatialCoreGUIType.ParticipantsList);
        bool isParticipantsOpen = (participantsState & SpatialCoreGUIState.Open) != 0;
        
        // Toggle the participants list
        SpatialCoreGUITypeFlags participantsFlag = SpatialCoreGUIType.ParticipantsList.ToFlag();
        SpatialBridge.coreGUIService.SetCoreGUIOpen(participantsFlag, !isParticipantsOpen);
    }
}
```

```csharp
// Example 2: Managing GUI visibility during different game modes
// This example demonstrates disabling certain GUIs during specific game states

public class GameModeManager : MonoBehaviour
{
    public void EnterExplorationMode()
    {
        // Enable exploration-related GUIs
        EnableExplorationGUIs(true);
        
        // Make sure quest system is open
        SpatialCoreGUITypeFlags questFlag = SpatialCoreGUIType.QuestSystem.ToFlag();
        SpatialBridge.coreGUIService.SetCoreGUIOpen(questFlag, true);
    }
    
    public void EnterCombatMode()
    {
        // Disable non-combat GUIs during combat
        EnableExplorationGUIs(false);
    }
    
    private void EnableExplorationGUIs(bool enable)
    {
        // Create combined flags for exploration-related GUIs
        SpatialCoreGUITypeFlags explorationGUIs = 
            SpatialCoreGUIType.Backpack.ToFlag() | 
            SpatialCoreGUIType.QuestSystem.ToFlag() | 
            SpatialCoreGUIType.WorldShop.ToFlag();
        
        // Enable or disable the exploration GUIs
        SpatialBridge.coreGUIService.SetCoreGUIEnabled(explorationGUIs, enable);
    }
    
    public void OpenShop(bool useUniversalShop)
    {
        // Open either the universal shop or world shop based on parameter
        SpatialCoreGUIType shopType = useUniversalShop ? 
            SpatialCoreGUIType.UniversalShop : 
            SpatialCoreGUIType.WorldShop;
        
        SpatialCoreGUITypeFlags shopFlag = shopType.ToFlag();
        SpatialBridge.coreGUIService.SetCoreGUIOpen(shopFlag, true);
    }
}
```

## Best Practices

1. **Use ToFlag() for service methods** - Always convert SpatialCoreGUIType enum values to SpatialCoreGUITypeFlags using ToFlag() when using ICoreGUIService methods.

2. **Check state before modifying** - Always check the current state of a GUI element using GetCoreGUIState before toggling its state to avoid unnecessary operations.

3. **Respect user preferences** - Consider that users may have expectations about which GUI elements are available. Disabling core GUIs should have clear in-game justification.

4. **Group related GUI operations** - When enabling or disabling multiple related GUI elements, group them together for clarity and efficiency.

5. **Handle state changes gracefully** - Subscribe to the ICoreGUIService.onCoreGUIOpenStateChanged event to react appropriately when GUI states change.

6. **Use meaningful transitions** - When opening or closing GUI elements, consider the context and provide appropriate visual or audio feedback.

## Common Use Cases

1. **Quest notifications** - Opening the QuestSystem GUI when a new quest becomes available or is updated.

2. **Inventory management** - Opening the Backpack GUI when a player collects an important item.

3. **Social features** - Enabling and disabling Chat and ParticipantsList GUIs based on social context within the space.

4. **Shop interactions** - Opening WorldShop or UniversalShop GUIs when interacting with relevant NPCs or locations.

5. **Game state management** - Disabling certain GUIs during cutscenes, combat, or other special game states.

6. **Tutorial guidance** - Controlling GUI visibility during tutorial sequences to guide players through the interface.

7. **Accessibility options** - Allowing players to enable or disable certain GUI elements based on their preferences.

## Related Components

- [ICoreGUIService](./ICoreGUIService.md) - The service interface that provides methods to interact with core GUI elements.
- [SpatialCoreGUIState](./SpatialCoreGUIState.md) - Defines the possible states (enabled/open) for core GUI elements.
- [SpatialCoreGUITypeFlags](./SpatialCoreGUITypeFlags.md) - Flag representation of core GUI types, used with service methods.
- [CoreGUIUtility](./CoreGUIUtility.md) - Provides utility methods for working with core GUI types, including conversion to flags.