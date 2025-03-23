# SpatialCoreGUIState

Category: Core GUI Service Related

Enum: Defines possible states of core GUI elements in the Spatial platform.

## Overview
SpatialCoreGUIState is an enum that defines the possible states of core GUI elements in the Spatial platform. It's used with the ICoreGUIService to check whether a GUI element is enabled and/or open.

## Properties/Fields

| Value | Description |
| --- | --- |
| None | No state specified. Used as a default value or to represent the absence of any states. |
| Enabled | Whether the GUI can be opened by the user or by script. When a GUI is not enabled, it cannot be opened by the user via hotkeys or by scripts until it is re-enabled. |
| Open | Whether the GUI is currently open or closed. If a GUI is marked as open but also disabled, while the GUI is disabled it will be temporarily marked as closed. When the GUI is re-enabled, it will be restored to its previous open state. |

## Usage Examples

```csharp
// Example 1: Checking the state of various GUI elements
// This example demonstrates how to check if GUIs are enabled and open

public class GUIStateChecker : MonoBehaviour
{
    private void Start()
    {
        // Check the state of the Chat GUI
        SpatialCoreGUIState chatState = SpatialBridge.coreGUIService.GetCoreGUIState(SpatialCoreGUIType.Chat);
        
        // Check if chat is enabled
        bool chatEnabled = (chatState & SpatialCoreGUIState.Enabled) != 0;
        Debug.Log($"Chat is {(chatEnabled ? "enabled" : "disabled")}");
        
        // Check if chat is open
        bool chatOpen = (chatState & SpatialCoreGUIState.Open) != 0;
        Debug.Log($"Chat is {(chatOpen ? "open" : "closed")}");
        
        // Check if chat is both enabled and open
        bool chatEnabledAndOpen = chatState == (SpatialCoreGUIState.Enabled | SpatialCoreGUIState.Open);
        Debug.Log($"Chat is {(chatEnabledAndOpen ? "enabled and open" : "not both enabled and open")}");
    }
}
```

```csharp
// Example 2: Managing GUI states based on game state
// This example shows how to enable/disable GUIs based on game state changes

public class GameStateManager : MonoBehaviour
{
    public void EnterCombatMode()
    {
        // Disable non-essential GUIs during combat
        SpatialCoreGUITypeFlags nonEssentialGUIs = 
            SpatialCoreGUIType.Backpack.ToFlag() | 
            SpatialCoreGUIType.Chat.ToFlag() | 
            SpatialCoreGUIType.ParticipantsList.ToFlag();
        
        SpatialBridge.coreGUIService.SetCoreGUIEnabled(nonEssentialGUIs, false);
    }
    
    public void ExitCombatMode()
    {
        // Re-enable GUIs after combat ends
        SpatialCoreGUITypeFlags nonEssentialGUIs = 
            SpatialCoreGUIType.Backpack.ToFlag() | 
            SpatialCoreGUIType.Chat.ToFlag() | 
            SpatialCoreGUIType.ParticipantsList.ToFlag();
        
        SpatialBridge.coreGUIService.SetCoreGUIEnabled(nonEssentialGUIs, true);
        
        // GUIs that were open before being disabled will automatically return to their open state
    }
    
    public void ToggleQuestSystemBasedOnState()
    {
        // Get the current state of the quest system GUI
        SpatialCoreGUIState questState = SpatialBridge.coreGUIService.GetCoreGUIState(SpatialCoreGUIType.QuestSystem);
        
        // Toggle the open state based on current state
        bool shouldOpen = (questState & SpatialCoreGUIState.Open) == 0;
        SpatialBridge.coreGUIService.SetCoreGUIOpen(SpatialCoreGUIType.QuestSystem.ToFlag(), shouldOpen);
    }
}
```

## Best Practices

1. **Use bitwise operations for state checking** - Since SpatialCoreGUIState can represent multiple states at once, use bitwise AND (&) to check for specific states.

2. **Check state before making changes** - Always check the current state of GUI elements before modifying them to avoid unnecessary state changes.

3. **Remember disabled GUIs can't be opened** - If a GUI is disabled, attempts to open it will be ignored until it's re-enabled.

4. **Consider state relationships** - Remember that a GUI's open state is preserved when it's disabled and will be restored when re-enabled.

5. **Restore previous states** - When re-enabling GUIs that were temporarily disabled, remember that they will return to their previous open state.

## Common Use Cases

1. **Game state management** - Disabling certain UI elements during specific game states (e.g., combat, cutscenes).

2. **UI customization** - Allowing players to enable or disable specific UI elements based on their preferences.

3. **Conditional UI** - Showing certain UI elements only when they're relevant to the current context.

4. **UI animations and transitions** - Checking the current state of UI elements before initiating animations or transitions.

5. **Tutorial systems** - Guiding users through interfaces by controlling which UI elements are enabled and open at different stages.

## Related Components

- [ICoreGUIService](./ICoreGUIService.md) - The service that manages core GUI elements, providing methods to get and set their states.
- [SpatialCoreGUIType](./SpatialCoreGUIType.md) - Defines the different types of core GUI elements that can have states.
- [SpatialCoreGUITypeFlags](./SpatialCoreGUITypeFlags.md) - Flag representation of GUI types, used with methods to set states.
- [CoreGUIUtility](./CoreGUIUtility.md) - Provides utility methods for working with core GUI types and flags.