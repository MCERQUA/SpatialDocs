# QuestTaskType

Category: Quest Service Related

Enum

## Overview
The QuestTaskType enum defines the different types of tasks that can be created for quests in the Spatial platform. Each task type has distinct completion requirements and is suitable for different gameplay scenarios. The type determines how progress is tracked and how the task is completed.

## Properties

| Property | Description |
| --- | --- |
| Check | One-time task that is either complete or not. Used for simple objectives like "Go to this point of interest" or "Talk to this NPC". Meant to be completed using task.Complete() or similar methods. |
| ProgressBar | Multi-step task that tracks progress toward a goal. Used for objectives like "Collect 5 items" or "Interact with an object 3 times". Meant to be completed using SpatialBridge.questService.AddTaskProgress() the number of times specified in the progressSteps property. |

## Usage Examples

### Creating Different Task Types

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class QuestCreationExample : MonoBehaviour
{
    // Reference objects to use as task markers
    public GameObject talkToNPCMarker;
    public GameObject[] collectibleMarkers;
    public GameObject bossMarker;
    
    void Start()
    {
        // Create a new quest
        IQuest adventureQuest = SpatialBridge.questService.CreateQuest(
            "adventure_quest",
            "Forest Adventure",
            "Explore the magical forest and defeat the ancient guardian."
        );
        
        // Configure quest settings
        adventureQuest.saveUserProgress = true;
        adventureQuest.tasksAreOrdered = true; // Tasks must be completed in order
        
        // Add a simple check task - just needs to be completed once
        IQuestTask talkTask = adventureQuest.AddTask(
            "Talk to the Forest Guide", 
            QuestTaskType.Check, 
            1, 
            new GameObject[] { talkToNPCMarker }
        );
        
        // Add a progress bar task - requires multiple steps to complete
        IQuestTask collectTask = adventureQuest.AddTask(
            "Collect 5 magical herbs", 
            QuestTaskType.ProgressBar, 
            5, // 5 steps required to complete
            collectibleMarkers
        );
        
        // Add another check task
        IQuestTask bossTask = adventureQuest.AddTask(
            "Defeat the Ancient Guardian", 
            QuestTaskType.Check, 
            1, 
            new GameObject[] { bossMarker }
        );
        
        // Add rewards
        adventureQuest.AddBadgeReward("forest_explorer");
        adventureQuest.AddItemReward("magic_staff", 1);
        
        // Start the quest
        adventureQuest.Start();
    }
}
```

### Handling Different Task Types

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class TaskCompletionHandler : MonoBehaviour
{
    public string questId = "adventure_quest";
    
    // Called when player talks to the Forest Guide NPC
    public void OnTalkToGuide()
    {
        IQuest quest = SpatialBridge.questService.GetQuest(questId);
        if (quest == null) return;
        
        // Find the talk task
        IQuestTask talkTask = quest.GetTaskByID(0); // First task
        
        if (talkTask != null && talkTask.status == QuestStatus.InProgress)
        {
            // For Check type tasks, simply call Complete()
            talkTask.Complete();
            
            SpatialBridge.coreGUIService.DisplayToastMessage("You've received guidance from the Forest Guide!");
        }
    }
    
    // Called when player collects a magical herb
    public void OnHerbCollected(GameObject herb)
    {
        IQuest quest = SpatialBridge.questService.GetQuest(questId);
        if (quest == null) return;
        
        // Find the collection task
        IQuestTask collectTask = quest.GetTaskByID(1); // Second task
        
        if (collectTask != null && collectTask.status == QuestStatus.InProgress)
        {
            // For ProgressBar type tasks, add progress
            SpatialBridge.questService.AddTaskProgress(questId, collectTask.id, 1);
            
            // Disable the collected herb
            herb.SetActive(false);
            
            // Show progress to the player
            int remaining = collectTask.progressSteps - collectTask.progress;
            if (remaining > 0)
            {
                SpatialBridge.coreGUIService.DisplayToastMessage($"Herb collected! {remaining} more to find.");
            }
        }
    }
    
    // Called when player defeats the boss
    public void OnBossDefeated()
    {
        IQuest quest = SpatialBridge.questService.GetQuest(questId);
        if (quest == null) return;
        
        // Find the boss task
        IQuestTask bossTask = quest.GetTaskByID(2); // Third task
        
        if (bossTask != null && bossTask.status == QuestStatus.InProgress)
        {
            // For Check type tasks, simply call Complete()
            bossTask.Complete();
            
            SpatialBridge.coreGUIService.DisplayToastMessage("You've defeated the Ancient Guardian!");
        }
    }
}
```

### Creating a Custom Task Handler for Different Types

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    private Dictionary<uint, List<GameObject>> taskObjects = new Dictionary<uint, List<GameObject>>();
    
    public void RegisterTaskObjects(IQuestTask task, List<GameObject> objects)
    {
        taskObjects[task.id] = objects;
        
        // Set up different behavior based on task type
        switch (task.type)
        {
            case QuestTaskType.Check:
                SetupCheckTask(task, objects);
                break;
                
            case QuestTaskType.ProgressBar:
                SetupProgressBarTask(task, objects);
                break;
        }
        
        // Common setup for any task type
        task.SetOnStarted(() => {
            ShowTaskObjects(task.id, true);
        });
        
        task.SetOnCompleted(() => {
            HandleTaskCompletion(task);
        });
        
        task.SetOnPreviouslyCompleted(() => {
            HandlePreviouslyCompletedTask(task);
        });
        
        // Initially hide objects if task isn't started yet
        if (task.status == QuestStatus.None)
        {
            ShowTaskObjects(task.id, false);
        }
    }
    
    private void SetupCheckTask(IQuestTask task, List<GameObject> objects)
    {
        // For Check tasks, set up single interaction point
        foreach (GameObject obj in objects)
        {
            // Add components needed for simple completion
            InteractionPoint interactionPoint = obj.AddComponent<InteractionPoint>();
            interactionPoint.SetTask(task);
        }
    }
    
    private void SetupProgressBarTask(IQuestTask task, List<GameObject> objects)
    {
        // For ProgressBar tasks, set up collectibles or interaction counters
        string questId = GetQuestIdForTask(task);
        
        foreach (GameObject obj in objects)
        {
            // Add components needed for incremental progress
            CollectibleItem collectible = obj.AddComponent<CollectibleItem>();
            collectible.SetTask(task, questId);
        }
    }
    
    private void HandleTaskCompletion(IQuestTask task)
    {
        // Different completion effects based on task type
        if (task.type == QuestTaskType.Check)
        {
            // Simple task completion effects
            SpatialBridge.coreGUIService.DisplayToastMessage($"Task completed: {task.name}");
            ShowTaskObjects(task.id, false);
        }
        else if (task.type == QuestTaskType.ProgressBar)
        {
            // Collection task completion effects
            SpatialBridge.coreGUIService.DisplayToastMessage($"Collection complete: {task.name}");
            // All objects should already be hidden as they were collected
        }
    }
    
    private void HandlePreviouslyCompletedTask(IQuestTask task)
    {
        // Different setup for previously completed tasks
        if (task.type == QuestTaskType.Check)
        {
            // Make sure objective is in completed state
            ShowTaskObjects(task.id, false);
        }
        else if (task.type == QuestTaskType.ProgressBar)
        {
            // Make sure all collectibles are already collected
            ShowTaskObjects(task.id, false);
        }
    }
    
    private void ShowTaskObjects(uint taskId, bool visible)
    {
        if (taskObjects.TryGetValue(taskId, out List<GameObject> objects))
        {
            foreach (GameObject obj in objects)
            {
                if (obj != null)
                {
                    obj.SetActive(visible);
                }
            }
        }
    }
    
    private string GetQuestIdForTask(IQuestTask task)
    {
        // Logic to find the quest ID that contains this task
        // This would be implemented based on how you're organizing quests
        // For simplicity, you might store this in a dictionary when registering tasks
        return "some_quest_id";
    }
    
    // Example helper components that would be defined elsewhere
    
    private class InteractionPoint : MonoBehaviour
    {
        private IQuestTask task;
        
        public void SetTask(IQuestTask task)
        {
            this.task = task;
        }
        
        // Implementation for interaction logic
    }
    
    private class CollectibleItem : MonoBehaviour
    {
        private IQuestTask task;
        private string questId;
        
        public void SetTask(IQuestTask task, string questId)
        {
            this.task = task;
            this.questId = questId;
        }
        
        // Implementation for collection logic
    }
}
```

## Best Practices

1. **Task Type Selection**
   - Use QuestTaskType.Check for simple, one-time objectives like:
     - Visiting a location
     - Talking to an NPC
     - Activating a mechanism
     - Completing a puzzle
   - Use QuestTaskType.ProgressBar for multi-step objectives like:
     - Collecting a set number of items
     - Defeating multiple enemies
     - Activating several switches
     - Completing repeatable actions

2. **Appropriate Progress Steps**
   - For QuestTaskType.Check tasks, always set progressSteps to 1.
   - For QuestTaskType.ProgressBar tasks, set progressSteps to the total number of steps required.
   - Choose appropriate step counts that balance challenge with attainability.

3. **Tracking and Feedback**
   - Provide clear visual tracking for ProgressBar tasks (e.g., "3/5 collected").
   - Give immediate feedback when progress is made.
   - Show task completion status in UI elements (quest log, HUD, etc.).

4. **Task Marker Usage**
   - Use task markers effectively for both task types.
   - For Check tasks, markers typically indicate the location of the objective.
   - For ProgressBar tasks, markers often represent each collectible or interaction point.

5. **Proper Task Completion**
   - Use task.Complete() for QuestTaskType.Check tasks.
   - Use SpatialBridge.questService.AddTaskProgress() for QuestTaskType.ProgressBar tasks.
   - Validate task status and type before attempting to complete or increment progress.

## Common Use Cases

1. **Check Task Use Cases**
   - **Tutorial Steps**: Simple instructions to teach game mechanics.
   - **Story Progression**: Key plot points that advance the narrative.
   - **Location Discovery**: Marking points of interest as visited.
   - **NPC Interactions**: Conversations with characters.
   - **Activation Events**: Pressing buttons, pulling levers, etc.

2. **ProgressBar Task Use Cases**
   - **Collection Quests**: Gathering resources or items scattered throughout the world.
   - **Defeat Quests**: Eliminating a specific number of enemies.
   - **Training Activities**: Repeating an action multiple times to master it.
   - **Resource Gathering**: Mining minerals, harvesting plants, etc.
   - **Multi-Part Puzzles**: Solving puzzles with multiple similar components.
