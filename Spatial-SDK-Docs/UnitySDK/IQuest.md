# IQuest

Category: Quest Service Related

Interface

## Overview
The IQuest interface represents a quest within the Spatial platform. Quests are structured activities that users can complete to earn rewards. They consist of one or more tasks that need to be completed in order to finish the quest. Quests can be saved to persist across sessions and can award various types of rewards upon completion.

## Properties

| Property | Description |
| --- | --- |
| celebrateOnComplete | If enabled, a confetti animation will play when the quest is completed. |
| description | Short description of what the quest objective is. |
| hideToastMessageOnComplete | Affects VR devices only. If true, toast messages will not be displayed when the quest or its tasks are completed. |
| id | Unique ID of the quest. |
| name | Name of the quest. |
| rewards | Rewards for completing the quest. |
| saveUserProgress | If enabled, the user's progress will be saved to the cloud and restored when they rejoin the space. If disabled, the quest progress is reset on the next join. |
| status | Gets the status of the quest (None, InProgress, or Completed). |
| tasks | Tasks that need to be completed to finish the quest. |
| tasksAreOrdered | If true, only the first task will be started and once it's completed, the next task will start. If false, all tasks are automatically started when the quest starts. |

## Methods

| Method | Description |
| --- | --- |
| AddBadgeReward(string) | Adds a badge reward to the quest. |
| AddItemReward(string, int) | Adds an item reward to the quest. |
| AddTask(string, QuestTaskType, int, GameObject[]) | Adds a task to the task list. |
| Complete() | Completes the quest. |
| GetTaskByID(uint) | Gets a task by ID. |
| Reset() | Resets the quest (all tasks are reset and the quest status is marked as None). |
| SetOnCompleted(Action) | Sets the onCompleted action and returns the quest. |
| SetOnPreviouslyCompleted(Action) | Sets the onPreviouslyCompleted action and returns the quest. |
| SetOnReset(Action) | Sets the onReset action and returns the quest. |
| SetOnStarted(Action) | Sets the onStarted action and returns the quest. |
| Start() | Starts the quest. |

## Events

| Event | Description |
| --- | --- |
| onCompleted | Event called when this quest is completed. |
| onPreviouslyCompleted | Event that is triggered when the user loads into a space where a quest was previously completed. Only used when saveUserProgress is set to true. This event allows you to "fast forward" any settings in the scene that should be enabled if a quest was previously completed. |
| onReset | Event called when this quest is reset. |
| onStarted | Event called when this quest is started. |
| onTaskAdded | Event called when a task is added to the quest. |

## Usage Examples

### Basic Quest Setup with Check Tasks

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class TutorialQuestManager : MonoBehaviour
{
    public GameObject[] talkToGuideMarkers;
    public GameObject[] exploreAreaMarkers;
    
    private IQuest tutorialQuest;
    
    void Start()
    {
        // Create a new quest through the quest service
        tutorialQuest = SpatialBridge.questService.CreateQuest("tutorial_quest", "Tutorial Quest", "Complete the tutorial to learn the basics.");
        
        // Set up quest configuration
        tutorialQuest.saveUserProgress = true;
        tutorialQuest.celebrateOnComplete = true;
        tutorialQuest.tasksAreOrdered = true; // Tasks will be completed in sequence
        
        // Add tasks to the quest
        tutorialQuest.AddTask("Talk to the guide", QuestTaskType.Check, 1, talkToGuideMarkers);
        tutorialQuest.AddTask("Explore the main area", QuestTaskType.Check, 1, exploreAreaMarkers);
        
        // Add rewards
        tutorialQuest.AddBadgeReward("tutorial_complete_badge");
        tutorialQuest.AddItemReward("starter_pack", 1);
        
        // Set up event handlers
        tutorialQuest.SetOnStarted(() => {
            Debug.Log("Tutorial quest started!");
            SpatialBridge.coreGUIService.DisplayToastMessage("Tutorial started!");
        });
        
        tutorialQuest.SetOnCompleted(() => {
            Debug.Log("Tutorial quest completed!");
            SpatialBridge.coreGUIService.DisplayToastMessage("Congratulations! You've completed the tutorial!");
            UnlockNewAreas();
        });
        
        tutorialQuest.SetOnPreviouslyCompleted(() => {
            Debug.Log("Tutorial was already completed in a previous session");
            UnlockNewAreas();
        });
        
        // Start the quest
        tutorialQuest.Start();
    }
    
    // This would be called from your guide NPC interaction
    public void CompleteTalkToGuideTask()
    {
        var task = tutorialQuest.GetTaskByID(0); // First task
        if (task != null && task.status == QuestStatus.InProgress)
        {
            task.Complete();
        }
    }
    
    // This would be called when player enters the exploration area
    public void CompleteExploreAreaTask()
    {
        var task = tutorialQuest.GetTaskByID(1); // Second task
        if (task != null && task.status == QuestStatus.InProgress)
        {
            task.Complete();
        }
    }
    
    private void UnlockNewAreas()
    {
        // Code to unlock additional areas after tutorial completion
    }
}
```

### Progress Bar Task Example

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class CollectionQuestManager : MonoBehaviour
{
    public GameObject[] collectionMarkers;
    
    private IQuest collectionQuest;
    private IQuestTask collectionTask;
    
    void Start()
    {
        // Create a collection quest
        collectionQuest = SpatialBridge.questService.CreateQuest("collection_quest", "Treasure Hunter", "Collect 5 treasures hidden around the island.");
        
        // Configure the quest
        collectionQuest.saveUserProgress = true;
        collectionQuest.celebrateOnComplete = true;
        
        // Add a progress bar task that requires 5 steps to complete
        collectionTask = collectionQuest.AddTask("Find hidden treasures", QuestTaskType.ProgressBar, 5, collectionMarkers);
        
        // Add rewards
        collectionQuest.AddBadgeReward("treasure_hunter_badge");
        collectionQuest.AddItemReward("rare_compass", 1);
        
        // Set up event handlers
        collectionQuest.SetOnCompleted(() => {
            SpatialBridge.coreGUIService.DisplayToastMessage("You've found all the treasures!");
            // Maybe give access to a special area or unlock a new quest
        });
        
        // Start the quest
        collectionQuest.Start();
    }
    
    // Call this when player finds a treasure
    public void TreasureFound()
    {
        if (collectionTask.status == QuestStatus.InProgress && collectionTask.progress < collectionTask.progressSteps)
        {
            // Increment progress by 1
            SpatialBridge.questService.AddTaskProgress(collectionQuest.id, collectionTask.id, 1);
            
            SpatialBridge.coreGUIService.DisplayToastMessage($"Treasure found! {collectionTask.progress}/{collectionTask.progressSteps}");
        }
    }
}
```

## Best Practices

1. **Use Descriptive Names and Descriptions**
   - Clear quest and task names help users understand what they need to do.
   - Brief but informative descriptions provide context without overwhelming the user.

2. **Plan Your Quest Structure**
   - Decide whether tasks should be sequential (tasksAreOrdered = true) or can be completed in any order.
   - For complex quests, consider breaking them into multiple smaller quests that unlock progressively.

3. **Appropriate Task Types**
   - Use QuestTaskType.Check for one-time objectives like "visit this location" or "talk to this character."
   - Use QuestTaskType.ProgressBar for collection or repetitive tasks like "find 5 items" or "defeat 10 enemies."

4. **Save Progress Appropriately**
   - Enable saveUserProgress for main storyline quests or important achievements.
   - Consider disabling it for daily challenges or repeatable quests that should reset.

5. **Handle Events Properly**
   - Implement onPreviouslyCompleted to properly restore the world state when a player returns with a completed quest.
   - Use onCompleted to trigger rewards, unlock new areas, or start follow-up quests.

6. **Avoid Blocking Progress**
   - Ensure quest locations remain accessible.
   - Consider providing hints or guidance if users might have trouble finding objectives.

7. **Visual Task Markers**
   - Use task markers effectively to guide players to objectives.
   - Consider toggling marker visibility based on quest status or distance.

## Common Use Cases

1. **Tutorial Quests**
   - Guide new users through basic interactions and movement.
   - Introduce key features of your space progressively.
   - Use ordered tasks to ensure a proper learning sequence.

2. **Exploration Quests**
   - Encourage users to discover different areas of your space.
   - Use Check tasks for visiting points of interest.
   - Reward thorough exploration with badges or exclusive items.

3. **Collection Quests**
   - Hide collectibles around your space for users to find.
   - Use ProgressBar tasks to track collection progress.
   - Consider providing subtle hints or clues for difficult-to-find items.

4. **Achievement Systems**
   - Create quests for special accomplishments.
   - Award unique badges that users can display on their profiles.
   - Use saveUserProgress to ensure achievements persist across sessions.

5. **Storyline Progression**
   - Create a sequence of quests that tell a story.
   - Use quest completion to unlock new areas or narrative elements.
   - Consider using the onPreviouslyCompleted event to allow returning users to continue the story.

6. **Daily Challenges**
   - Create repeatable quests that reset daily.
   - Disable saveUserProgress to ensure the quest resets.
   - Offer currency or consumable items as rewards.

7. **Scavenger Hunts**
   - Combine exploration and collection mechanics.
   - Provide clues that lead to the next objective.
   - Use ProgressBar tasks to track overall completion.
