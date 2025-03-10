# IQuestTask

Category: Quest Service Related

Interface

## Overview
The IQuestTask interface represents an individual task within a quest in the Spatial platform. Tasks are the building blocks of quests, defining specific objectives that users need to complete. Tasks can be simple "check" tasks for one-time objectives or "progress bar" tasks that require multiple steps to complete.

## Properties

| Property | Description |
| --- | --- |
| id | Unique ID of the task. |
| name | Name of the task that describes the objective. |
| progress | The current progress of the task (how many steps have been completed). |
| progressSteps | The number of steps required to complete the task. |
| status | Gets the status of the task (None, InProgress, or Completed). |
| taskMarkers | GameObject markers associated with this task, typically used to visually indicate objectives in the world. |
| type | Type of task (Check or ProgressBar). |

## Methods

| Method | Description |
| --- | --- |
| Complete() | Completes the task. |
| SetOnCompleted(Action) | Sets the onCompleted action and returns the task. |
| SetOnPreviouslyCompleted(Action) | Sets the onPreviouslyCompleted action and returns the task. |
| SetOnStarted(Action) | Sets the onStarted action and returns the task. |
| Start() | Starts the task. |

## Events

| Event | Description |
| --- | --- |
| onCompleted | Event called when this task is completed. |
| onPreviouslyCompleted | Event triggered when the user loads into a space where this task was previously completed. Only used when the parent quest's saveUserProgress is set to true. Allows for "fast-forwarding" any settings that should be enabled if a task was previously completed. |
| onStarted | Event called when this task is started. |

## Usage Examples

### Working with Check Tasks

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class TaskTriggerExample : MonoBehaviour
{
    public string questId = "explorer_quest";
    public uint taskId = 0; // First task in the quest
    
    private IQuestTask task;
    private bool playerInTrigger = false;
    
    void Start()
    {
        // Get the task reference
        IQuest quest = SpatialBridge.questService.GetQuest(questId);
        if (quest != null)
        {
            task = quest.GetTaskByID(taskId);
            
            // Set up event handlers
            if (task != null)
            {
                task.SetOnStarted(() => {
                    Debug.Log($"Task '{task.name}' started!");
                    // Make task markers visible or highlight them
                    SetTaskMarkersVisibility(true);
                });
                
                task.SetOnCompleted(() => {
                    Debug.Log($"Task '{task.name}' completed!");
                    // Hide or change task markers
                    SetTaskMarkersVisibility(false);
                    // Maybe show a celebration effect
                    ShowCompletionEffect();
                });
                
                task.SetOnPreviouslyCompleted(() => {
                    Debug.Log($"Task '{task.name}' was previously completed");
                    // Ensure the world state matches the completed task
                    SetTaskMarkersVisibility(false);
                    SetDoorUnlocked(true);
                });
            }
        }
    }
    
    // Trigger when the player enters the objective area
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            
            // Show UI prompt
            SpatialBridge.coreGUIService.DisplayToastMessage("Press E to complete objective");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }
    
    void Update()
    {
        // Check for player input while in trigger area
        if (playerInTrigger && task != null && task.status == QuestStatus.InProgress)
        {
            if (SpatialBridge.inputService.GetKeyDown(KeyCode.E))
            {
                // Complete the task
                task.Complete();
                
                // Perform any additional actions
                SetDoorUnlocked(true);
            }
        }
    }
    
    private void SetTaskMarkersVisibility(bool visible)
    {
        if (task != null && task.taskMarkers != null)
        {
            foreach (GameObject marker in task.taskMarkers)
            {
                if (marker != null)
                {
                    marker.SetActive(visible);
                }
            }
        }
    }
    
    private void ShowCompletionEffect()
    {
        // Show particle effects, play sound, etc.
    }
    
    private void SetDoorUnlocked(bool unlocked)
    {
        // Unlock a door or change something in the world
        // based on task completion
    }
}
```

### Working with Progress Bar Tasks

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class CollectiblesManager : MonoBehaviour
{
    public string questId = "collection_quest";
    public uint taskId = 0;
    
    private IQuestTask collectTask;
    private List<GameObject> collectedItems = new List<GameObject>();
    
    void Start()
    {
        // Get reference to the collection task
        IQuest quest = SpatialBridge.questService.GetQuest(questId);
        if (quest != null)
        {
            collectTask = quest.GetTaskByID(taskId);
            
            if (collectTask != null)
            {
                Debug.Log($"Collection task found. Progress: {collectTask.progress}/{collectTask.progressSteps}");
                
                // Set up event handlers
                collectTask.SetOnCompleted(() => {
                    Debug.Log("Collection complete!");
                    // Maybe unlock a special item or area
                    UnlockReward();
                });
                
                collectTask.SetOnPreviouslyCompleted(() => {
                    Debug.Log("Collection was previously completed");
                    // Ensure collectibles are already collected in the scene
                    HideAllCollectibles();
                    UnlockReward();
                });
            }
        }
    }
    
    // Call this when player finds a collectible
    public void ItemCollected(GameObject collectible)
    {
        if (collectTask != null && collectTask.status == QuestStatus.InProgress)
        {
            // Make sure we haven't already collected this item
            if (!collectedItems.Contains(collectible))
            {
                collectedItems.Add(collectible);
                
                // Hide or destroy the collectible
                collectible.SetActive(false);
                
                // Increment task progress
                SpatialBridge.questService.AddTaskProgress(questId, collectTask.id, 1);
                
                // Show progress to the player
                int remaining = collectTask.progressSteps - collectTask.progress;
                if (remaining > 0)
                {
                    SpatialBridge.coreGUIService.DisplayToastMessage($"Item collected! {remaining} more to find.");
                }
            }
        }
    }
    
    private void HideAllCollectibles()
    {
        // Find and hide all collectible objects in the scene
        // This would be called when the task was previously completed
        GameObject[] allCollectibles = GameObject.FindGameObjectsWithTag("Collectible");
        foreach (GameObject collectible in allCollectibles)
        {
            collectible.SetActive(false);
        }
    }
    
    private void UnlockReward()
    {
        // Unlock some reward when all collectibles are found
    }
}
```

## Best Practices

1. **Clear Naming and Visual Guidance**
   - Give tasks descriptive names that clearly explain the objective.
   - Use task markers effectively to guide users to task locations.
   - Consider using different marker styles for different task types.

2. **Appropriate Task Type Selection**
   - Use QuestTaskType.Check for simple, one-time objectives.
   - Use QuestTaskType.ProgressBar for collection or multi-step objectives.
   - Match the task type to the gameplay mechanic (e.g., don't use a Check task for collecting multiple items).

3. **Proper Progress Tracking**
   - For Check tasks, call Complete() when the objective is achieved.
   - For ProgressBar tasks, use SpatialBridge.questService.AddTaskProgress() to increment progress.
   - Verify task status before modifying progress to avoid errors.

4. **Responsive Event Handling**
   - Implement onStarted to set up the task environment properly.
   - Use onCompleted to trigger immediate rewards or changes in the world.
   - Implement onPreviouslyCompleted to correctly restore world state for returning users.

5. **User Feedback**
   - Provide clear feedback when task progress is made.
   - Consider using SpatialBridge.coreGUIService.DisplayToastMessage() for important updates.
   - For ProgressBar tasks, show the current progress (e.g., "3/5 collected").

6. **Prevent Progress Blocking**
   - Ensure tasks can always be completed, even if the user takes an unexpected approach.
   - Consider providing hints for difficult tasks.
   - Test tasks thoroughly to ensure they can be completed in all scenarios.

## Common Use Cases

1. **Location-Based Objectives**
   - Use Check tasks with task markers to guide users to specific locations.
   - Trigger completion when users reach the designated area.
   - Common in exploration quests or guided tours.

2. **Interaction Objectives**
   - Create tasks that require users to interact with specific objects or NPCs.
   - Use Check tasks for single interactions or ProgressBar tasks for multiple interactions.
   - Provide clear prompts for how to interact (e.g., "Press E to talk").

3. **Collection Objectives**
   - Use ProgressBar tasks to track the collection of multiple items.
   - Update progress as each item is collected.
   - Show remaining count to help users understand their progress.

4. **Puzzle-Solving Objectives**
   - Break complex puzzles into clear task steps.
   - Use ProgressBar tasks for multi-step puzzles.
   - Provide subtle hints through task naming or markers.

5. **Timed Challenges**
   - Create time-limited objectives using Check tasks.
   - Start timers when the task begins.
   - Complete the task automatically if finished in time, or fail/reset if time expires.

6. **Sequential Instructions**
   - Use ordered tasks in a quest to provide step-by-step instructions.
   - Ensure each task clearly guides to the next step.
   - Ideal for tutorials or onboarding experiences.

7. **Achievement Milestones**
   - Create ProgressBar tasks with high step counts for long-term achievements.
   - Example: "Explore 10 different areas" or "Collect 100 tokens".
   - Consider persistent saving for these longer-term objectives.
