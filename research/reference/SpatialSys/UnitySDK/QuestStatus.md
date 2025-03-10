# QuestStatus

Category: Quest Service Related

Enum

## Overview
The QuestStatus enum represents the different states that a quest or task can be in within the Spatial platform. It's used to track the progression of quests and tasks, allowing developers to determine the current state and respond accordingly in their applications.

## Properties

| Property | Description |
| --- | --- |
| None | The quest or task has not been started. This is the default state for newly created quests and tasks. |
| InProgress | The quest or task has been started but is not yet completed. The user is actively working on completing the objectives. |
| Completed | The quest or task has been successfully completed. All requirements have been fulfilled. |

## Usage Examples

### Checking Quest Status

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class QuestStatusChecker : MonoBehaviour
{
    public string questId = "main_story_quest";
    
    void Update()
    {
        // Get the quest reference
        IQuest quest = SpatialBridge.questService.GetQuest(questId);
        
        if (quest != null)
        {
            // Check the quest status
            switch (quest.status)
            {
                case QuestStatus.None:
                    // Quest hasn't been started yet
                    ShowQuestAvailableUI();
                    break;
                    
                case QuestStatus.InProgress:
                    // Quest is currently active
                    UpdateQuestProgressUI(quest);
                    break;
                    
                case QuestStatus.Completed:
                    // Quest has been completed
                    ShowQuestCompletedUI();
                    break;
            }
        }
    }
    
    private void ShowQuestAvailableUI()
    {
        // Display UI indicating the quest is available to start
    }
    
    private void UpdateQuestProgressUI(IQuest quest)
    {
        // Update UI showing current quest progress
        int completedTasks = 0;
        
        foreach (IQuestTask task in quest.tasks)
        {
            if (task.status == QuestStatus.Completed)
            {
                completedTasks++;
            }
        }
        
        // Display progress (e.g., "2/5 tasks completed")
    }
    
    private void ShowQuestCompletedUI()
    {
        // Display UI indicating the quest has been completed
    }
}
```

### Managing World State Based on Quest Status

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class QuestStateManager : MonoBehaviour
{
    public string questId = "village_rescue_quest";
    
    // World state elements
    public GameObject villageInDanger;
    public GameObject villageRestored;
    public GameObject[] rescueNPCs;
    public GameObject[] celebrationNPCs;
    
    void Start()
    {
        // Initial setup - hide everything
        villageInDanger.SetActive(false);
        villageRestored.SetActive(false);
        
        foreach (GameObject npc in rescueNPCs)
        {
            npc.SetActive(false);
        }
        
        foreach (GameObject npc in celebrationNPCs)
        {
            npc.SetActive(false);
        }
        
        // Get quest and update world state based on status
        IQuest quest = SpatialBridge.questService.GetQuest(questId);
        if (quest != null)
        {
            UpdateWorldState(quest.status);
            
            // Subscribe to quest state changes
            quest.onStarted += () => UpdateWorldState(QuestStatus.InProgress);
            quest.onCompleted += () => UpdateWorldState(QuestStatus.Completed);
            quest.onReset += () => UpdateWorldState(QuestStatus.None);
        }
    }
    
    private void UpdateWorldState(QuestStatus status)
    {
        switch (status)
        {
            case QuestStatus.None:
                // World hasn't been affected by this quest yet
                // Perhaps show a peaceful village with no problems
                villageInDanger.SetActive(false);
                villageRestored.SetActive(true);
                
                foreach (GameObject npc in rescueNPCs)
                {
                    npc.SetActive(false);
                }
                
                foreach (GameObject npc in celebrationNPCs)
                {
                    npc.SetActive(false);
                }
                break;
                
            case QuestStatus.InProgress:
                // Village is in danger, NPCs need rescue
                villageInDanger.SetActive(true);
                villageRestored.SetActive(false);
                
                foreach (GameObject npc in rescueNPCs)
                {
                    npc.SetActive(true);
                }
                
                foreach (GameObject npc in celebrationNPCs)
                {
                    npc.SetActive(false);
                }
                break;
                
            case QuestStatus.Completed:
                // Village has been saved, celebration is happening
                villageInDanger.SetActive(false);
                villageRestored.SetActive(true);
                
                foreach (GameObject npc in rescueNPCs)
                {
                    npc.SetActive(false);
                }
                
                foreach (GameObject npc in celebrationNPCs)
                {
                    npc.SetActive(true);
                }
                break;
        }
    }
}
```

## Best Practices

1. **Proper Status Transitions**
   - Ensure quests and tasks follow the natural progression: None → InProgress → Completed.
   - Avoid skipping states (e.g., going directly from None to Completed) unless specifically designed that way.
   - Use the Reset() method on quests to properly return them to the None status when needed.

2. **Status-Based UI Updates**
   - Update UI elements to reflect the current status of quests and tasks.
   - Use different visual indicators for each status (e.g., grayed out for None, highlighted for InProgress, check mark for Completed).
   - Provide clear feedback when status changes occur.

3. **World State Synchronization**
   - Adjust the world state to match the quest status (e.g., NPCs, objects, environments).
   - Use the quest.onPreviouslyCompleted event to correctly restore the world when a user returns to a space with completed quests.
   - Implement status checks during initialization to ensure the world starts in the correct state.

4. **Status-Based Progression**
   - Use quest status to gate access to areas or content.
   - Create prerequisites based on the status of other quests (e.g., "Quest B requires Quest A to be Completed").
   - Design branching quest lines based on the status of previous quests.

5. **Persistence Considerations**
   - When using saveUserProgress = true, handle the case where users return with quests in various states.
   - Implement comprehensive onPreviouslyCompleted event handlers to restore the correct world state.
   - Test quest progression across multiple sessions to ensure status is properly maintained.

## Common Use Cases

1. **Quest Log/Journal**
   - Use quest status to categorize quests in a player's journal (Available, Active, Completed).
   - Filter quest lists based on status.
   - Update quest descriptions or objectives based on status.

2. **Progressive World Changes**
   - Modify the environment based on quest status (e.g., a building under construction becomes complete).
   - Change NPC dialogue and behavior based on related quest status.
   - Unlock new areas or features when certain quests reach Completed status.

3. **Status-Dependent UI**
   - Show different UI indicators for quests in different states.
   - Display quest markers on a map with status-specific icons.
   - Update objective text based on current status.

4. **Tutorial Sequences**
   - Track user progress through tutorial sections using quest status.
   - Show different guidance based on which tutorial quests are completed.
   - Unlock advanced features only after tutorial quests reach Completed status.

5. **Save/Load Systems**
   - Store the status of all quests when implementing a save system.
   - Restore the correct world state based on saved quest status during loading.
   - Use status to determine which quest data needs to be saved or restored.

6. **Quest Chains and Prerequisites**
   - Make new quests available only when prerequisite quests reach Completed status.
   - Create branching storylines based on the status of decision-point quests.
   - Design quest chains where each quest must be completed in sequence.
