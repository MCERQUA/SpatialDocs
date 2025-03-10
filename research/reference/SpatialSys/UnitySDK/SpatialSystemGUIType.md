# SpatialSystemGUIType

Category: Core GUI Service Related

Enum: Defines the types of system GUI elements available in the Spatial platform.

## Overview
SpatialSystemGUIType is an enum that defines the different types of system-level GUI elements in the Spatial platform. These are specialized UI components that are managed by the system rather than directly by the ICoreGUIService. The enum is used to identify specific system GUI types when interacting with other system services.

## Properties/Fields

| Value | Description |
| --- | --- |
| None | No system GUI type specified. Used as a default value. |
| QuestSystem | The system-level quest interface. Used to access and manage quests throughout the Spatial experience. |

## Usage Examples

```csharp
// Example 1: Working with the Quest System
// This example demonstrates how to work with the quest system GUI

public class QuestController : MonoBehaviour
{
    private void Start()
    {
        // Check if we're dealing with the quest system
        SpatialSystemGUIType questSystemType = SpatialSystemGUIType.QuestSystem;
        
        // We would use this enum value when interacting with system services
        // that need to identify which system GUI we're working with
        
        // For actual GUI operations with the quest system, we would use 
        // SpatialCoreGUIType.QuestSystem instead, as shown below:
        
        // Check if the quest GUI is currently enabled
        SpatialCoreGUIState questState = SpatialBridge.coreGUIService.GetCoreGUIState(SpatialCoreGUIType.QuestSystem);
        bool isQuestSystemEnabled = (questState & SpatialCoreGUIState.Enabled) != 0;
        
        Debug.Log($"Quest system is {(isQuestSystemEnabled ? "enabled" : "disabled")}");
        
        // If needed, convert to flags for use with ICoreGUIService
        SpatialCoreGUITypeFlags questFlag = SpatialCoreGUIType.QuestSystem.ToFlag();
        
        // For example, we might want to ensure the quest system is visible
        // when a new quest becomes available
        SpatialBridge.coreGUIService.SetCoreGUIOpen(questFlag, true);
    }
}
```

```csharp
// Example 2: Creating a quest notification system
// This example shows a system that monitors quests and manages the quest GUI

public class QuestNotificationSystem : MonoBehaviour
{
    private QuestManager questManager; // Your custom quest management script
    
    private void Start()
    {
        questManager = GetComponent<QuestManager>();
        
        // Subscribe to quest updates
        if (questManager != null)
        {
            questManager.OnQuestAdded += HandleNewQuest;
            questManager.OnQuestUpdated += HandleQuestUpdate;
            questManager.OnQuestCompleted += HandleQuestCompletion;
        }
    }
    
    private void HandleNewQuest(IQuest newQuest)
    {
        // When a new quest is added, make sure the quest system is visible
        SpatialCoreGUITypeFlags questFlag = SpatialCoreGUIType.QuestSystem.ToFlag();
        
        // First, check if it's already open
        SpatialCoreGUIState questState = SpatialBridge.coreGUIService.GetCoreGUIState(SpatialCoreGUIType.QuestSystem);
        bool isQuestSystemOpen = (questState & SpatialCoreGUIState.Open) != 0;
        
        if (!isQuestSystemOpen)
        {
            // If not open, make sure it's enabled and then open it
            SpatialBridge.coreGUIService.SetCoreGUIEnabled(questFlag, true);
            SpatialBridge.coreGUIService.SetCoreGUIOpen(questFlag, true);
            
            // Also display a toast notification about the new quest
            SpatialBridge.coreGUIService.DisplayToastMessage($"New Quest: {newQuest.title}", 3.0f);
        }
    }
    
    private void HandleQuestUpdate(IQuest updatedQuest)
    {
        // For quest updates, just show a toast notification
        // without opening the quest GUI
        SpatialBridge.coreGUIService.DisplayToastMessage($"Quest Update: {updatedQuest.title}", 2.0f);
    }
    
    private void HandleQuestCompletion(IQuest completedQuest)
    {
        // For quest completion, show a toast and open the quest GUI to show the rewards
        SpatialCoreGUITypeFlags questFlag = SpatialCoreGUIType.QuestSystem.ToFlag();
        SpatialBridge.coreGUIService.SetCoreGUIEnabled(questFlag, true);
        SpatialBridge.coreGUIService.SetCoreGUIOpen(questFlag, true);
        
        SpatialBridge.coreGUIService.DisplayToastMessage($"Quest Completed: {completedQuest.title}", 4.0f);
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (questManager != null)
        {
            questManager.OnQuestAdded -= HandleNewQuest;
            questManager.OnQuestUpdated -= HandleQuestUpdate;
            questManager.OnQuestCompleted -= HandleQuestCompletion;
        }
    }
}
```

## Best Practices

1. **Understand the distinction from SpatialCoreGUIType** - SpatialSystemGUIType refers to system-level GUI components, while SpatialCoreGUIType is used with ICoreGUIService for controlling GUI visibility.

2. **Use appropriate enums for each context** - When working with system services, use SpatialSystemGUIType. When controlling GUI visibility, use SpatialCoreGUIType.

3. **Consider relationships between system components** - System GUI components may have relationships with other system components. For example, the QuestSystem GUI is related to the IQuestService.

4. **Handle system GUI responsively** - When system events occur (like quest updates), consider the appropriate way to update or show the related system GUI.

5. **Respect user preferences** - Be mindful of when to automatically show system GUIs. Balance user notifications with respect for user control over the interface.

## Common Use Cases

1. **Quest notifications** - Showing the quest system GUI when a new quest becomes available or is updated.

2. **Tutorial integration** - Using the quest system as part of guided tutorials for new users.

3. **Achievement systems** - Integrating quest-based achievements that leverage the quest system GUI.

4. **Game progression** - Using the quest system to guide players through the main storyline or progression path.

5. **UI state management** - Managing the visibility and state of system GUIs based on game context or user preferences.

6. **Cross-service coordination** - Coordinating between system services (like quest service) and their associated GUI elements.

7. **Dynamic UI adaptation** - Adapting the interface to show the most relevant system GUIs based on current player activities.

## Related Components

- [ICoreGUIService](./ICoreGUIService.md) - The service that manages GUI visibility, including system GUIs.
- [SpatialCoreGUIType](./SpatialCoreGUIType.md) - The core GUI enum used with ICoreGUIService that includes a QuestSystem value corresponding to SpatialSystemGUIType.QuestSystem.
- [SpatialCoreGUITypeFlags](./SpatialCoreGUITypeFlags.md) - Flag representation of core GUI types used with ICoreGUIService methods.
- [IQuestService](./IQuestService.md) - The service interface for quest-related functionality that works with the QuestSystem GUI.
- [SpatialCoreGUIState](./SpatialCoreGUIState.md) - Defines the possible states for GUI elements.