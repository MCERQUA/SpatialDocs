# SpatialQuest.Task

Category: Core Components

Interface/Class/Enum: Class

The SpatialQuest.Task class represents an individual task or objective within a quest in a Spatial environment. Tasks are the building blocks of quests, defining specific goals that users need to complete. Each task has its own progress tracking, status, and event system, allowing for the creation of complex quest structures with multiple steps.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| id | string | Unique identifier for this task. |
| name | string | Name of the task displayed to users. |
| type | QuestTaskType | The type of task (Custom, Location, Interact, etc.) which defines how the task is completed. |
| status | QuestStatus | Gets the current status of the task (None, InProgress, Completed). |
| progress | float | The current progress of the task, ranging from 0 (not started) to 1 (completed). |
| progressSteps | int | The number of steps or increments required to complete this task. |
| taskMarkers | List<Transform> | Collection of transform markers associated with this task, often used for location-based tasks. |

## Methods

| Method | Description |
| --- | --- |
| StartTask() | Starts the task, setting its status to InProgress. |
| CompleteTask() | Completes the task, setting its status to Completed. |

## Events

| Event | Description |
| --- | --- |
| onStartedEvent | Event triggered when the task is started. |
| onCompletedEvent | Event triggered when the task is completed. |
| onPreviouslyCompleted | Event triggered when a user enters with a previously completed task (if quest's saveUserProgress is true). |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class TaskManager : MonoBehaviour
{
    // Reference to the quest
    [SerializeField] private SpatialQuest mainQuest;
    
    // References to task-related objects in the scene
    [SerializeField] private Transform[] locationMarkers;
    [SerializeField] private SpatialInteractable[] interactables;
    [SerializeField] private GameObject[] collectibles;
    
    // Player detection trigger radius for location tasks
    [SerializeField] private float locationDetectionRadius = 3f;
    
    // UI reference
    [SerializeField] private TaskUIController taskUI;
    
    // Map of active tasks to their related game objects
    private Dictionary<SpatialQuest.Task, GameObject> taskObjects = new Dictionary<SpatialQuest.Task, GameObject>();
    
    private void Start()
    {
        // Initialize task manager
        if (mainQuest != null)
        {
            InitializeTaskListeners();
            SetupTaskObjects();
        }
    }
    
    // Initialize listeners for all tasks in the quest
    private void InitializeTaskListeners()
    {
        if (mainQuest.tasks == null)
            return;
            
        foreach (var task in mainQuest.tasks)
        {
            if (task != null)
            {
                // Set up listeners for task events
                task.onStartedEvent.AddListener(() => OnTaskStarted(task));
                task.onCompletedEvent.AddListener(() => OnTaskCompleted(task));
                task.onPreviouslyCompleted.AddListener(() => OnTaskPreviouslyCompleted(task));
            }
        }
        
        // Listen to quest events
        mainQuest.onStartedEvent.AddListener(OnQuestStarted);
        mainQuest.onCompletedEvent.AddListener(OnQuestCompleted);
    }
    
    // Set up objects associated with each task
    private void SetupTaskObjects()
    {
        if (mainQuest.tasks == null)
            return;
            
        // Process each task and set up associated objects
        int locationIndex = 0;
        int interactableIndex = 0;
        int collectibleIndex = 0;
        
        foreach (var task in mainQuest.tasks)
        {
            switch (task.type)
            {
                case QuestTaskType.Location:
                    // Link location markers to location tasks
                    if (locationIndex < locationMarkers.Length)
                    {
                        SetupLocationTask(task, locationMarkers[locationIndex]);
                        locationIndex++;
                    }
                    break;
                    
                case QuestTaskType.Interact:
                    // Link interactables to interact tasks
                    if (interactableIndex < interactables.Length)
                    {
                        SetupInteractTask(task, interactables[interactableIndex]);
                        interactableIndex++;
                    }
                    break;
                    
                case QuestTaskType.Collection:
                    // Link collectibles to collection tasks
                    if (collectibleIndex < collectibles.Length)
                    {
                        SetupCollectionTask(task, collectibles[collectibleIndex]);
                        collectibleIndex++;
                    }
                    break;
                    
                case QuestTaskType.Custom:
                    // Custom tasks may have special setup
                    SetupCustomTask(task);
                    break;
                    
                default:
                    Debug.LogWarning($"Unsupported task type: {task.type} for task: {task.name}");
                    break;
            }
        }
    }
    
    // Set up a location-based task
    private void SetupLocationTask(SpatialQuest.Task task, Transform locationMarker)
    {
        if (task == null || locationMarker == null)
            return;
            
        // Add the location marker to the task's markers
        if (task.taskMarkers == null)
        {
            task.taskMarkers = new List<Transform>();
        }
        task.taskMarkers.Add(locationMarker);
        
        // Create a trigger area around the location
        GameObject triggerObject = new GameObject($"LocationTrigger_{task.name}");
        triggerObject.transform.position = locationMarker.position;
        
        // Add a sphere collider as a trigger
        SphereCollider triggerCollider = triggerObject.AddComponent<SphereCollider>();
        triggerCollider.radius = locationDetectionRadius;
        triggerCollider.isTrigger = true;
        
        // Add a trigger event component
        SpatialTriggerEvent triggerEvent = triggerObject.AddComponent<SpatialTriggerEvent>();
        
        // Set up the player enter event to complete the task
        triggerEvent.onPlayerEnter.AddListener(() => 
        {
            if (mainQuest.status == QuestStatus.InProgress && task.status == QuestStatus.InProgress)
            {
                task.CompleteTask();
            }
        });
        
        // Store the trigger object associated with this task
        taskObjects[task] = triggerObject;
        
        // Initially disable the trigger
        triggerObject.SetActive(false);
        
        Debug.Log($"Set up location task: {task.name} at position {locationMarker.position}");
    }
    
    // Set up an interact-based task
    private void SetupInteractTask(SpatialQuest.Task task, SpatialInteractable interactable)
    {
        if (task == null || interactable == null)
            return;
            
        // Add an interaction listener
        interactable.onInteract.AddListener(() => 
        {
            if (mainQuest.status == QuestStatus.InProgress && task.status == QuestStatus.InProgress)
            {
                task.CompleteTask();
            }
        });
        
        // Store the interactable object associated with this task
        taskObjects[task] = interactable.gameObject;
        
        // Initially disable the interactable if the quest hasn't started
        if (mainQuest.status == QuestStatus.None)
        {
            interactable.gameObject.SetActive(false);
        }
        
        Debug.Log($"Set up interact task: {task.name} with interactable {interactable.name}");
    }
    
    // Set up a collection-based task
    private void SetupCollectionTask(SpatialQuest.Task task, GameObject collectible)
    {
        if (task == null || collectible == null)
            return;
            
        // Set the number of progress steps based on the number of collectibles
        task.progressSteps = collectibles.Length;
        
        // Add a collectible component if not already present
        Collectible collectibleComponent = collectible.GetComponent<Collectible>();
        if (collectibleComponent == null)
        {
            collectibleComponent = collectible.AddComponent<Collectible>();
        }
        
        // Set up the collectible
        collectibleComponent.taskId = task.id;
        collectibleComponent.OnCollected.AddListener((taskId) => 
        {
            if (taskId == task.id && mainQuest.status == QuestStatus.InProgress && task.status == QuestStatus.InProgress)
            {
                // Increment progress
                IncrementTaskProgress(task);
            }
        });
        
        // Store the collectible object associated with this task
        taskObjects[task] = collectible;
        
        // Initially disable the collectible if the quest hasn't started
        if (mainQuest.status == QuestStatus.None)
        {
            collectible.SetActive(false);
        }
        
        Debug.Log($"Set up collection task: {task.name} with collectible {collectible.name}");
    }
    
    // Set up a custom task
    private void SetupCustomTask(SpatialQuest.Task task)
    {
        if (task == null)
            return;
            
        // Custom tasks are handled through direct calls to StartTask, IncrementTaskProgress, and CompleteTask
        Debug.Log($"Set up custom task: {task.name} - Custom tasks require manual progress tracking");
    }
    
    // Event handlers
    
    private void OnQuestStarted()
    {
        Debug.Log($"Quest started: {mainQuest.questName}");
        
        // Enable task objects for tasks that don't require sequential completion
        if (!mainQuest.tasksAreOrdered)
        {
            foreach (var task in mainQuest.tasks)
            {
                // Start all tasks if they're not ordered
                task.StartTask();
            }
        }
        else if (mainQuest.tasks.Count > 0)
        {
            // Start only the first task if they're ordered
            mainQuest.tasks[0].StartTask();
        }
    }
    
    private void OnQuestCompleted()
    {
        Debug.Log($"Quest completed: {mainQuest.questName}");
        
        // Clean up or disable task objects
        foreach (var taskObj in taskObjects)
        {
            // Could disable or remove task objects when the quest completes
            // This example just keeps them active for now
        }
    }
    
    private void OnTaskStarted(SpatialQuest.Task task)
    {
        Debug.Log($"Task started: {task.name}");
        
        // Enable the associated task object if it exists
        if (taskObjects.TryGetValue(task, out GameObject taskObj) && taskObj != null)
        {
            taskObj.SetActive(true);
        }
        
        // Update UI
        if (taskUI != null)
        {
            taskUI.DisplayActiveTask(task);
        }
    }
    
    private void OnTaskCompleted(SpatialQuest.Task task)
    {
        Debug.Log($"Task completed: {task.name}");
        
        // Disable the associated task object if needed
        if (taskObjects.TryGetValue(task, out GameObject taskObj) && taskObj != null)
        {
            // For location or interact tasks, we might want to disable them
            if (task.type == QuestTaskType.Location || task.type == QuestTaskType.Interact)
            {
                taskObj.SetActive(false);
            }
        }
        
        // Update UI
        if (taskUI != null)
        {
            taskUI.MarkTaskComplete(task);
        }
        
        // Start the next task if tasks are ordered
        if (mainQuest.tasksAreOrdered)
        {
            int currentIndex = mainQuest.tasks.IndexOf(task);
            if (currentIndex >= 0 && currentIndex < mainQuest.tasks.Count - 1)
            {
                SpatialQuest.Task nextTask = mainQuest.tasks[currentIndex + 1];
                nextTask.StartTask();
            }
        }
        
        // Check if all tasks are complete
        bool allComplete = true;
        foreach (var questTask in mainQuest.tasks)
        {
            if (questTask.status != QuestStatus.Completed)
            {
                allComplete = false;
                break;
            }
        }
        
        // Complete the quest if all tasks are done
        if (allComplete && mainQuest.status != QuestStatus.Completed)
        {
            mainQuest.CompleteQuest();
        }
    }
    
    private void OnTaskPreviouslyCompleted(SpatialQuest.Task task)
    {
        Debug.Log($"Task was previously completed: {task.name}");
        
        // Handle previously completed tasks similarly to newly completed ones
        if (taskObjects.TryGetValue(task, out GameObject taskObj) && taskObj != null)
        {
            // For location or interact tasks, we might want to disable them
            if (task.type == QuestTaskType.Location || task.type == QuestTaskType.Interact)
            {
                taskObj.SetActive(false);
            }
        }
        
        // Update UI
        if (taskUI != null)
        {
            taskUI.MarkTaskComplete(task);
        }
    }
    
    // Helper methods
    
    // Increment progress for a task with multiple steps
    public void IncrementTaskProgress(SpatialQuest.Task task)
    {
        if (task == null || task.status != QuestStatus.InProgress)
            return;
            
        // Calculate current progress step
        int currentStep = Mathf.FloorToInt(task.progress * task.progressSteps);
        currentStep++; // Increment by one step
        
        // Calculate new progress
        float newProgress = (float)currentStep / task.progressSteps;
        
        // The progress property might be read-only, so we might need to use a custom approach
        // This is a simplification - in a real implementation, you'd need to handle this based on the actual API
        // task.progress = newProgress;
        
        Debug.Log($"Task {task.name} progress: {newProgress * 100}% ({currentStep}/{task.progressSteps})");
        
        // Update UI
        if (taskUI != null)
        {
            taskUI.UpdateTaskProgress(task, newProgress);
        }
        
        // Complete the task if we've reached 100%
        if (currentStep >= task.progressSteps)
        {
            task.CompleteTask();
        }
    }
    
    // Create a task programmatically
    public SpatialQuest.Task CreateTask(string name, QuestTaskType type, int progressSteps = 1)
    {
        SpatialQuest.Task task = new SpatialQuest.Task();
        task.name = name;
        task.type = type;
        task.id = System.Guid.NewGuid().ToString();
        task.progressSteps = progressSteps;
        
        // Set up event listeners
        task.onStartedEvent.AddListener(() => OnTaskStarted(task));
        task.onCompletedEvent.AddListener(() => OnTaskCompleted(task));
        
        Debug.Log($"Created new task: {name} (Type: {type})");
        return task;
    }
    
    // Add a task to the quest
    public void AddTaskToQuest(SpatialQuest quest, SpatialQuest.Task task)
    {
        if (quest == null || task == null)
            return;
            
        // Initialize task list if needed
        if (quest.tasks == null)
        {
            quest.tasks = new List<SpatialQuest.Task>();
        }
        
        // Add the task
        quest.tasks.Add(task);
        
        Debug.Log($"Added task '{task.name}' to quest: {quest.questName}");
    }
}

// Simple collectible component for collection tasks
public class Collectible : MonoBehaviour
{
    public string taskId;
    public UnityEvent<string> OnCollected = new UnityEvent<string>();
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if it's the player
        if (other.CompareTag("Player"))
        {
            // Invoke the collected event
            OnCollected.Invoke(taskId);
            
            // Disable or destroy the collectible
            gameObject.SetActive(false);
        }
    }
}
```

## Best Practices

1. Give tasks clear, descriptive names that help users understand what they need to do.
2. Use the appropriate QuestTaskType for each task to ensure proper behavior and user expectations.
3. For location-based tasks, make the destination clearly visible or provide guidance to users.
4. For multi-step tasks, use progressSteps to track completion and provide visual feedback on progress.
5. Use the taskMarkers list to associate relevant transforms with the task, especially for location-based tasks.
6. Subscribe to task events (onStartedEvent, onCompletedEvent) to trigger appropriate feedback and progression.
7. For ordered tasks within a quest, ensure each task clearly leads to the next for a smooth progression.
8. Consider using different visual indicators for different task types (e.g., map markers for locations, highlight effects for interactables).
9. Provide clear feedback when a task is completed, both visually and through UI.
10. For collection tasks, display the current count and total required to help users track their progress.

## Common Use Cases

1. Location-based tasks that require users to visit specific places in the environment.
2. Interaction tasks that ask users to interact with specific objects or NPCs.
3. Collection tasks where users need to gather a certain number of items.
4. Multi-step processes broken down into sequential tasks (e.g., crafting something in multiple stages).
5. Tutorial steps that guide users through features or mechanics.
6. Time-limited challenges where users need to complete a task within a time limit.
7. Group activities where multiple users need to coordinate to complete a shared task.
8. Puzzle tasks that require users to solve a specific problem.
9. Achievement-based tasks that reward users for reaching specific milestones.
10. Exploration tasks that encourage users to discover new areas or features in the environment.

## Completed: March 10, 2025