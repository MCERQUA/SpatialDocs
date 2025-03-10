# CoreGUIUtility

Category: Core GUI Service Related

Class: Provides utility functions for working with Core GUI components.

## Overview
CoreGUIUtility is a utility class that provides helper methods for working with the Core GUI system in Spatial. It offers functionality to convert between different GUI type representations, making it easier to work with the GUI system.

## Methods

| Method | Description |
| --- | --- |
| ToFlag(SpatialCoreGUIType guiType) | Converts a SpatialCoreGUIType value to its equivalent SpatialCoreGUITypeFlags representation. This is useful when working with methods that require flags instead of enum values. |

## Usage Examples

```csharp
// Example 1: Converting a SpatialCoreGUIType to SpatialCoreGUITypeFlags
// This example shows how to convert a GUI type to a flag for use with ICoreGUIService methods

public class GUIController : MonoBehaviour
{
    public void ToggleChatVisibility(bool visible)
    {
        // Convert the Chat enum value to its flag representation
        SpatialCoreGUITypeFlags chatFlag = SpatialCoreGUIType.Chat.ToFlag();
        
        // Use the flag with the ICoreGUIService methods
        SpatialBridge.coreGUIService.SetCoreGUIOpen(chatFlag, visible);
    }
    
    public void DisableMultipleGUIs()
    {
        // You can convert individual types and combine them using bitwise OR
        SpatialCoreGUITypeFlags chatFlag = SpatialCoreGUIType.Chat.ToFlag();
        SpatialCoreGUITypeFlags backpackFlag = SpatialCoreGUIType.Backpack.ToFlag();
        
        // Disable both the chat and backpack GUIs
        SpatialBridge.coreGUIService.SetCoreGUIEnabled(chatFlag | backpackFlag, false);
    }
}
```

```csharp
// Example 2: Working with GUI state checking
// This example demonstrates checking GUI states using the utility method

public class GUIStateMonitor : MonoBehaviour
{
    private void Update()
    {
        // Check if the chat is currently open
        SpatialCoreGUIState chatState = SpatialBridge.coreGUIService.GetCoreGUIState(SpatialCoreGUIType.Chat);
        
        if ((chatState & SpatialCoreGUIState.Open) != 0)
        {
            Debug.Log("Chat is currently open");
        }
        
        // Check if the backpack is enabled
        SpatialCoreGUIState backpackState = SpatialBridge.coreGUIService.GetCoreGUIState(SpatialCoreGUIType.Backpack);
        
        if ((backpackState & SpatialCoreGUIState.Enabled) != 0)
        {
            Debug.Log("Backpack GUI is currently enabled");
        }
        else
        {
            // Convert the backpack type to a flag for re-enabling
            SpatialCoreGUITypeFlags backpackFlag = SpatialCoreGUIType.Backpack.ToFlag();
            SpatialBridge.coreGUIService.SetCoreGUIEnabled(backpackFlag, true);
        }
    }
}
```

## Best Practices

1. **Use ToFlag() for flag-based methods** - Always use the ToFlag() extension method when working with ICoreGUIService methods that require SpatialCoreGUITypeFlags.

2. **Combine flags efficiently** - When working with multiple GUI elements, convert each type to a flag and combine them using bitwise OR (|) for more efficient code.

3. **Maintain consistency** - When managing GUI states, be consistent in how you enable, disable, open, and close GUI elements to avoid confusion.

4. **Check states before making changes** - Always check the current state of GUI elements before changing them to avoid unnecessary calls to the service.

5. **Consider user experience** - When enabling or disabling GUI elements, consider the impact on user experience and provide appropriate feedback.

## Common Use Cases

1. **Managing multiple GUI elements** - ToFlag() is useful when you need to enable, disable, open, or close multiple GUI elements at once by combining flags.

2. **Custom GUI management systems** - When building custom GUI management systems that interact with the core GUI, ToFlag() provides a convenient way to convert between types and flags.

3. **GUI state monitoring** - Used in conjunction with GetCoreGUIState to track and respond to changes in the state of various GUI elements.

4. **Temporary GUI modifications** - When temporarily modifying GUI visibility or functionality, such as during cutscenes or special game states.

5. **UI customization features** - When implementing features that allow players to customize which UI elements they want visible.

## Related Components

- [ICoreGUIService](./ICoreGUIService.md) - The main service interface for interacting with core GUI elements.
- [SpatialCoreGUIType](./SpatialCoreGUIType.md) - Enum defining the different types of core GUI elements.
- [SpatialCoreGUITypeFlags](./SpatialCoreGUITypeFlags.md) - Flags representation of core GUI types.
- [SpatialCoreGUIState](./SpatialCoreGUIState.md) - Enum defining the possible states of core GUI elements.