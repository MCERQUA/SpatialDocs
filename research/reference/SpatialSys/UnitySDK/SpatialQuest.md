# SpatialQuest

Category: Core Components

Interface/Class/Enum: Class

The SpatialQuest component enables developers to create quest-based gameplay experiences in Spatial environments. It allows for the creation of structured tasks that users can complete to earn rewards, providing a framework for guided activities, challenges, and achievements. Quests can be designed with ordered or unordered tasks and can track user progress across sessions.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| LATEST_VERSION | int | Static constant representing the latest version of the SpatialQuest component. |
| MAX_NAME_LENGTH | int | Static constant defining the maximum allowed length for quest names. |
| MAX_DESCRIPTION_LENGTH | int | Static constant defining the maximum allowed length for quest descriptions. |
| id | string | Unique identifier for this quest. |
| questName | string | Name of the quest displayed to users. |
| description | string | Detailed description of the quest. |
| tasks | List<Task> | Collection of tasks that need to be completed as part of this quest. |
| tasksAreOrdered | bool | Determines whether tasks must be completed in sequence (true) or can be completed in any order (false). |
| questRewards | List<Reward> | Collection of rewards given to users upon completion of the quest. |
| status | QuestStatus | The current status of the quest (None, InProgress, Completed). |
| quest | IQuest | Interface to the underlying quest service object. |
| startAutomatically | bool | If true, the quest will automatically start when the user enters the space. |
| saveUserProgress | bool | If true, the quest progress will be saved and persist between user sessions. |
| celebrateOnComplete | bool | If true, triggers a celebration effect when the quest is completed. |
| hideToastMessageOnComplete | bool | If true, no toast message will be shown when the quest is completed. |
| isExperimental | bool | Indicates if this quest uses experimental features. |
| version | int | The version of this component instance. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |
| documentationURL | string | URL to documentation for this component. |

## Methods

| Method | Description |
| --- | --- |
| StartQuest() | Starts the quest, initializing all tasks and setting the status to InProgress. |
| CompleteQuest() | Completes the quest, awarding any configured rewards and setting the status to Completed. |
| ResetQuest() | Resets the quest, clearing all task progress and setting the status back to None. |

## Events

| Event | Description |
| --- | --- |
| onStartedEvent | Event triggered when the quest is started. |
| onCompletedEvent | Event triggered when the quest is completed. |
| onResetEvent | Event triggered when the quest is reset. |
| onPreviouslyCompleted | Event triggered when a user enters with a previously completed quest (if saveUserProgress is true). |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using UnityEngine.Events;

public class QuestManager : MonoBehaviour
{
    // Reference to quests in the scene
    [SerializeField] private SpatialQuest mainQuest;
    [SerializeField] private SpatialQuest[] sideQuests;
    
    // UI for quest information
    [SerializeField] private QuestUIController questUI;
    
    // Quest interaction trigger
    [SerializeField] private SpatialInteractable questGiver;
    
    private void Start()
    {
        // Initialize quest manager
        InitializeQuests();
        
        // Set up quest giver interaction
        if (questGiver != null)
        {
            questGiver.onInteract.AddListener(OfferMainQuest);
        }
    }
    
    private void InitializeQuests()
    {
        // Set up main quest event listeners
        if (mainQuest != null)
        {
            mainQuest.onStartedEvent.AddListener(OnMainQuestStarted);
            mainQuest.onCompletedEvent.AddListener(OnMainQuestCompleted);
            mainQuest.onPreviouslyCompleted.AddListener(OnMainQuestPreviouslyCompleted);
            
            // Set up event listeners for each task in the main quest
            SetupTaskListeners(mainQuest);
            
            Debug.Log($"Main quest '{mainQuest.questName}' initialized with {mainQuest.tasks.Count} tasks");
        }
        
        // Set up side quests event listeners
        if (sideQuests != null)
        {
            foreach (var sideQuest in sideQuests)
            {
                if (sideQuest != null)
                {
                    sideQuest.onStartedEvent.AddListener(() => OnSideQuestStarted(sideQuest));
                    sideQuest.onCompletedEvent.AddListener(() => OnSideQuestCompleted(sideQuest));
                    
                    // Set up event listeners for each task in the side quest
                    SetupTaskListeners(sideQuest);
                    
                    Debug.Log($"Side quest '{sideQuest.questName}' initialized with {sideQuest.tasks.Count} tasks");
                }
            }
        }
    }
    
    private void SetupTaskListeners(SpatialQuest quest)
    {
        if (quest == null || quest.tasks == null)
            return;
            
        foreach (var task in quest.tasks)
        {
            task.onStartedEvent.AddListener(() => OnTaskStarted(quest, task));
            task.onCompletedEvent.AddListener(() => OnTaskCompleted(quest, task));
        }
    }
    
    // Quest giver interaction
    public void OfferMainQuest()
    {
        if (mainQuest == null)
            return;
            
        // Check if the quest is already started or completed
        if (mainQuest.status == QuestStatus.None)
        {
            // Display quest offer UI
            if (questUI != null)
            {
                questUI.DisplayQuestOffer(mainQuest);
            }
            else
            {
                // Auto-accept if no UI is available
                AcceptMainQuest();
            }
        }
        else if (mainQuest.status == QuestStatus.InProgress)
        {
            // Show quest progress
            if (questUI != null)
            {
                questUI.DisplayQuestProgress(mainQuest);
            }
            else
            {
                Debug.Log($"Quest '{mainQuest.questName}' is in progress. {GetTaskProgressSummary(mainQuest)}");
            }
        }
        else if (mainQuest.status == QuestStatus.Completed)
        {
            // Show completion message
            if (questUI != null)
            {
                questUI.DisplayQuestCompletion(mainQuest);
            }
            else
            {
                Debug.Log($"Quest '{mainQuest.questName}' has already been completed!");
            }
        }
    }
    
    // Accept main quest
    public void AcceptMainQuest()
    {
        if (mainQuest != null && mainQuest.status == QuestStatus.None)
        {
            mainQuest.StartQuest();
            
            Debug.Log($"Accepted quest: {mainQuest.questName}");
            SpatialBridge.coreGUIService.DisplayToastMessage($"Quest Started: {mainQuest.questName}");
        }
    }
    
    // Decline main quest
    public void DeclineMainQuest()
    {
        Debug.Log("Declined quest offer.");
        
        // Could set a flag to prevent re-offering the quest immediately
    }
    
    // Event handlers
    
    private void OnMainQuestStarted()
    {
        Debug.Log($"Main quest started: {mainQuest.questName}");
        
        // Update UI
        if (questUI != null)
        {
            questUI.UpdateQuestLog();
        }
        
        // Start first task if tasks are ordered
        if (mainQuest.tasksAreOrdered && mainQuest.tasks.Count > 0)
        {
            mainQuest.tasks[0].StartTask();
        }
    }
    
    private void OnMainQuestCompleted()
    {
        Debug.Log($"Main quest completed: {mainQuest.questName}");
        
        // Update UI
        if (questUI != null)
        {
            questUI.UpdateQuestLog();
            questUI.DisplayQuestCompletion(mainQuest);
        }
        
        // Log rewards
        LogQuestRewards(mainQuest);
    }
    
    private void OnMainQuestPreviouslyCompleted()
    {
        Debug.Log($"User has previously completed main quest: {mainQuest.questName}");
        
        // Update UI to reflect completed state
        if (questUI != null)
        {
            questUI.UpdateQuestLog();
        }
    }
    
    private void OnSideQuestStarted(SpatialQuest quest)
    {
        Debug.Log($"Side quest started: {quest.questName}");
        
        // Update UI
        if (questUI != null)
        {
            questUI.UpdateQuestLog();
        }
        
        // Start first task if tasks are ordered
        if (quest.tasksAreOrdered && quest.tasks.Count > 0)
        {
            quest.tasks[0].StartTask();
        }
    }
    
    private void OnSideQuestCompleted(SpatialQuest quest)
    {
        Debug.Log($"Side quest completed: {quest.questName}");
        
        // Update UI
        if (questUI != null)
        {
            questUI.UpdateQuestLog();
            questUI.DisplayQuestCompletion(quest);
        }
        
        // Log rewards
        LogQuestRewards(quest);
    }
    
    private void OnTaskStarted(SpatialQuest quest, SpatialQuest.Task task)
    {
        Debug.Log($"Task started: {task.name} (Quest: {quest.questName})");
        
        // Update UI
        if (questUI != null)
        {
            questUI.UpdateQuestLog();
            questUI.HighlightActiveTask(quest, task);
        }
    }
    
    private void OnTaskCompleted(SpatialQuest quest, SpatialQuest.Task task)
    {
        Debug.Log($"Task completed: {task.name} (Quest: {quest.questName})");
        
        // Update UI
        if (questUI != null)
        {
            questUI.UpdateQuestLog();
        }
        
        // Start next task if tasks are ordered
        if (quest.tasksAreOrdered)
        {
            int currentIndex = quest.tasks.IndexOf(task);
            if (currentIndex >= 0 && currentIndex < quest.tasks.Count - 1)
            {
                quest.tasks[currentIndex + 1].StartTask();
            }
        }
        
        // Check if all tasks are completed
        bool allTasksCompleted = true;
        foreach (var questTask in quest.tasks)
        {
            if (questTask.status != QuestStatus.Completed)
            {
                allTasksCompleted = false;
                break;
            }
        }
        
        // Auto-complete quest if all tasks are done
        if (allTasksCompleted && quest.status != QuestStatus.Completed)
        {
            quest.CompleteQuest();
        }
    }
    
    // Helper methods
    
    private string GetTaskProgressSummary(SpatialQuest quest)
    {
        if (quest == null || quest.tasks == null || quest.tasks.Count == 0)
            return "No tasks available.";
            
        int completedTasks = 0;
        foreach (var task in quest.tasks)
        {
            if (task.status == QuestStatus.Completed)
                completedTasks++;
        }
        
        return $"Completed {completedTasks} of {quest.tasks.Count} tasks.";
    }
    
    private void LogQuestRewards(SpatialQuest quest)
    {
        if (quest.questRewards == null || quest.questRewards.Count == 0)
        {
            Debug.Log("No rewards specified for this quest.");
            return;
        }
        
        Debug.Log($"Quest rewards for '{quest.questName}':");
        foreach (var reward in quest.questRewards)
        {
            Debug.Log($"- {reward.type}: {reward.amount}");
        }
    }
    
    // Create a quest programmatically
    public SpatialQuest CreateExplorationQuest(string questName, string description, Transform[] locations)
    {
        // Create a new GameObject for the quest
        GameObject questObject = new GameObject($"Quest_{questName}");
        
        // Add the SpatialQuest component
        SpatialQuest newQuest = questObject.AddComponent<SpatialQuest>();
        
        // Set basic quest properties
        newQuest.questName = questName;
        newQuest.description = description;
        newQuest.tasksAreOrdered = false; // Allow visiting locations in any order
        newQuest.saveUserProgress = true; // Save progress between sessions
        newQuest.celebrateOnComplete = true; // Celebration effect when completed
        
        // Create tasks for each location
        foreach (var location in locations)
        {
            // Create a new task
            SpatialQuest.Task task = new SpatialQuest.Task();
            task.name = $"Visit {location.name}";
            task.id = System.Guid.NewGuid().ToString();
            task.type = QuestTaskType.Location;
            
            // Add the task to the quest
            if (newQuest.tasks == null)
                newQuest.tasks = new System.Collections.Generic.List<SpatialQuest.Task>();
            
            newQuest.tasks.Add(task);
            
            // Create a trigger at the location
            GameObject trigger = new GameObject($"QuestTrigger_{location.name}");
            trigger.transform.position = location.position;
            
            // Add a collider
            SphereCollider collider = trigger.AddComponent<SphereCollider>();
            collider.radius = 3.0f; // Detection radius
            collider.isTrigger = true;
            
            // Add a trigger event
            SpatialTriggerEvent triggerEvent = trigger.AddComponent<SpatialTriggerEvent>();
            
            // Configure the trigger to complete the task when the player enters
            int taskIndex = newQuest.tasks.Count - 1;
            triggerEvent.onPlayerEnter.AddListener(() => {
                if (newQuest.status == QuestStatus.InProgress && 
                    newQuest.tasks[taskIndex].status != QuestStatus.Completed)
                {
                    newQuest.tasks[taskIndex].CompleteTask();
                }
            });
        }
        
        // Add a reward
        SpatialQuest.Reward reward = new SpatialQuest.Reward();
        reward.type = RewardType.Currency;
        reward.amount = 100;
        reward.id = "exploration_reward";
        
        if (newQuest.questRewards == null)
            newQuest.questRewards = new System.Collections.Generic.List<SpatialQuest.Reward>();
        
        newQuest.questRewards.Add(reward);
        
        // Set up quest events
        newQuest.onStartedEvent.AddListener(() => {
            Debug.Log($"Exploration Quest Started: {questName}");
            SpatialBridge.coreGUIService.DisplayToastMessage($"Quest Started: {questName}");
        });
        
        newQuest.onCompletedEvent.AddListener(() => {
            Debug.Log($"Exploration Quest Completed: {questName}");
            SpatialBridge.coreGUIService.DisplayToastMessage($"Quest Completed: {questName}");
        });
        
        Debug.Log($"Created new exploration quest: {questName} with {locations.Length} locations to visit");
        
        // Register with the quest manager
        if (sideQuests == null)
        {
            sideQuests = new SpatialQuest[1] { newQuest };
        }
        else
        {
            SpatialQuest[] newSideQuests = new SpatialQuest[sideQuests.Length + 1];
            System.Array.Copy(sideQuests, newSideQuests, sideQuests.Length);
            newSideQuests[newSideQuests.Length - 1] = newQuest;
            sideQuests = newSideQuests;
        }
        
        // Initialize the quest (set up event listeners)
        SetupTaskListeners(newQuest);
        newQuest.onStartedEvent.AddListener(() => OnSideQuestStarted(newQuest));
        newQuest.onCompletedEvent.AddListener(() => OnSideQuestCompleted(newQuest));
        
        return newQuest;
    }
}
```

## Best Practices

1. Give quests clear and descriptive names and descriptions so users understand what they need to do.
2. For ordered tasks (when tasksAreOrdered is true), ensure the sequence is logical and provides a good progression.
3. Use the saveUserProgress property for longer quests that users might not complete in a single session.
4. Consider using the startAutomatically property for tutorial quests or mandatory experiences.
5. Design quest rewards that are appropriate for the difficulty and time investment of the quest.
6. Use event listeners (onStartedEvent, onCompletedEvent, etc.) to trigger appropriate feedback and progression.
7. Balance the number of tasks in a quest - too few may not feel substantial, while too many might become tedious.
8. For location-based tasks, ensure the target areas are clearly marked or described to prevent user frustration.
9. Use the celebrateOnComplete property to provide satisfying visual feedback for quest completion.
10. Consider creating quest chains by starting follow-up quests in the onCompletedEvent of a prerequisite quest.

## Common Use Cases

1. Tutorial sequences that guide new users through the features of your space.
2. Scavenger hunts where users need to find specific objects or visit certain locations.
3. Achievement systems that reward users for completing challenges.
4. Progression mechanics that unlock new areas or features as users complete quests.
5. Narrative experiences where quests advance a storyline.
6. Daily or weekly challenges that encourage repeated visits to your space.
7. Educational activities where tasks represent learning objectives.
8. Team building or collaborative activities where multiple users work together on shared quests.
9. Time-limited events with special rewards.
10. Onboarding experiences that introduce users to community guidelines or space-specific rules.

## Completed: March 10, 2025