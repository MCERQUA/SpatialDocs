# IQuestService

Quest Service Interface

Service for managing quests and objectives in Spatial spaces. This service provides functionality for creating, tracking, and managing quest progression.

## Properties

### Quest State
| Property | Description |
| --- | --- |
| currentQuest | Active quest |
| currentQuestID | Active quest ID |
| quests | All available quests |

## Methods

### Quest Management
| Method | Description |
| --- | --- |
| CreateQuest(...) | Create new quest |

## Events

### Quest Updates
| Event | Description |
| --- | --- |
| onQuestAdded | Quest creation notification |
| onQuestRemoved | Quest removal notification |
| onCurrentQuestChanged | Active quest updates |

## Usage Examples

```csharp
// Example: Quest Manager
public class QuestManager : MonoBehaviour
{
    private IQuestService questService;
    private Dictionary<string, QuestState> questStates;
    private bool isInitialized;

    private class QuestState
    {
        public bool isActive;
        public float startTime;
        public Dictionary<string, object> progress;
        public Dictionary<string, object> metadata;
    }

    void Start()
    {
        questService = SpatialBridge.questService;
        questStates = new Dictionary<string, QuestState>();
        InitializeManager();
        SubscribeToEvents();
    }

    private void InitializeManager()
    {
        foreach (var quest in questService.quests)
        {
            InitializeQuestState(quest.Key, quest.Value);
        }
    }

    private void SubscribeToEvents()
    {
        questService.onQuestAdded += HandleQuestAdded;
        questService.onQuestRemoved += HandleQuestRemoved;
        questService.onCurrentQuestChanged += HandleCurrentQuestChanged;
    }

    public void CreateNewQuest(
        string questId,
        string title,
        bool isRepeatable = false,
        bool autoActivate = false,
        bool showNotification = true,
        bool trackProgress = true
    )
    {
        try
        {
            questService.CreateQuest(
                questId,
                title,
                isRepeatable,
                autoActivate,
                showNotification,
                trackProgress
            );
        }
        catch (Exception e)
        {
            Debug.LogError($"Error creating quest {questId}: {e.Message}");
        }
    }

    private void InitializeQuestState(
        string questId,
        IQuest quest
    )
    {
        var state = new QuestState
        {
            isActive = quest == questService.currentQuest,
            startTime = Time.time,
            progress = new Dictionary<string, object>(),
            metadata = new Dictionary<string, object>
            {
                { "createTime", Time.time },
                { "completionCount", 0 },
                { "lastCompletionTime", 0f }
            }
        };

        questStates[questId] = state;
    }

    private void HandleQuestAdded(IQuest quest)
    {
        if (!questStates.ContainsKey(quest.questID))
        {
            InitializeQuestState(quest.questID, quest);
        }
        OnQuestAdded(quest);
    }

    private void HandleQuestRemoved(IQuest quest)
    {
        if (questStates.TryGetValue(quest.questID, out var state))
        {
            state.isActive = false;
            state.metadata["removeTime"] = Time.time;
        }
        OnQuestRemoved(quest);
    }

    private void HandleCurrentQuestChanged(IQuest quest)
    {
        if (quest != null)
        {
            if (questStates.TryGetValue(quest.questID, out var state))
            {
                state.isActive = true;
                state.startTime = Time.time;
                state.metadata["activationCount"] = 
                    (int)state.metadata.GetValueOrDefault("activationCount", 0) + 1;
            }
        }

        var previousQuest = questService.currentQuest;
        if (previousQuest != null && 
            questStates.TryGetValue(previousQuest.questID, out var prevState))
        {
            prevState.isActive = false;
        }

        OnCurrentQuestChanged(quest);
    }

    private void OnQuestAdded(IQuest quest)
    {
        // Implementation for quest addition handling
    }

    private void OnQuestRemoved(IQuest quest)
    {
        // Implementation for quest removal handling
    }

    private void OnCurrentQuestChanged(IQuest quest)
    {
        // Implementation for current quest change handling
    }

    void OnDestroy()
    {
        if (questService != null)
        {
            questService.onQuestAdded -= HandleQuestAdded;
            questService.onQuestRemoved -= HandleQuestRemoved;
            questService.onCurrentQuestChanged -= HandleCurrentQuestChanged;
        }
    }
}

// Example: Quest Progress Tracker
public class QuestProgressTracker : MonoBehaviour
{
    private IQuestService questService;
    private Dictionary<string, ProgressState> progressStates;
    private bool isInitialized;

    private class ProgressState
    {
        public bool isTracking;
        public float lastUpdateTime;
        public Dictionary<string, object> objectives;
    }

    void Start()
    {
        questService = SpatialBridge.questService;
        progressStates = new Dictionary<string, ProgressState>();
        InitializeTracker();
    }

    private void InitializeTracker()
    {
        if (questService.currentQuest != null)
        {
            StartTracking(questService.currentQuest);
        }

        questService.onCurrentQuestChanged += (quest) => {
            if (quest != null)
            {
                StartTracking(quest);
            }
        };
    }

    private void StartTracking(IQuest quest)
    {
        var state = new ProgressState
        {
            isTracking = true,
            lastUpdateTime = Time.time,
            objectives = new Dictionary<string, object>()
        };

        foreach (var objective in quest.objectives)
        {
            state.objectives[objective.id] = new Dictionary<string, object>
            {
                { "progress", 0 },
                { "isComplete", false },
                { "startTime", Time.time }
            };
        }

        progressStates[quest.questID] = state;
        StartCoroutine(TrackProgress(quest));
    }

    private IEnumerator TrackProgress(IQuest quest)
    {
        while (progressStates.TryGetValue(quest.questID, out var state) && 
               state.isTracking)
        {
            UpdateQuestProgress(quest, state);
            yield return new WaitForSeconds(1f);
        }
    }

    private void UpdateQuestProgress(
        IQuest quest,
        ProgressState state
    )
    {
        foreach (var objective in quest.objectives)
        {
            if (!state.objectives.TryGetValue(objective.id, out var data))
                continue;

            var objectiveData = data as Dictionary<string, object>;
            var currentProgress = CheckObjectiveProgress(objective);
            
            if (currentProgress != (float)objectiveData["progress"])
            {
                objectiveData["progress"] = currentProgress;
                objectiveData["lastUpdateTime"] = Time.time;
                
                if (currentProgress >= 1.0f && 
                    !(bool)objectiveData["isComplete"])
                {
                    objectiveData["isComplete"] = true;
                    objectiveData["completeTime"] = Time.time;
                    OnObjectiveCompleted(quest, objective);
                }
            }
        }

        CheckQuestCompletion(quest, state);
    }

    private float CheckObjectiveProgress(IQuestObjective objective)
    {
        // Implementation for objective progress checking
        return 0f;
    }

    private void CheckQuestCompletion(
        IQuest quest,
        ProgressState state
    )
    {
        var isComplete = true;
        foreach (var objective in quest.objectives)
        {
            if (state.objectives.TryGetValue(objective.id, out var data))
            {
                var objectiveData = data as Dictionary<string, object>;
                if (!(bool)objectiveData["isComplete"])
                {
                    isComplete = false;
                    break;
                }
            }
        }

        if (isComplete)
        {
            OnQuestCompleted(quest);
        }
    }

    private void OnObjectiveCompleted(
        IQuest quest,
        IQuestObjective objective
    )
    {
        // Implementation for objective completion handling
    }

    private void OnQuestCompleted(IQuest quest)
    {
        // Implementation for quest completion handling
    }
}
```

## Best Practices

1. Quest Management
   - Track progress
   - Handle states
   - Validate completion
   - Cache data

2. Objective Control
   - Monitor progress
   - Update states
   - Handle completion
   - Track timing

3. Performance
   - Batch updates
   - Cache data
   - Handle timing
   - Optimize checks

4. Error Handling
   - Validate quests
   - Handle failures
   - Recover states
   - Log issues

## Common Use Cases

1. Quest Types
   - Main quests
   - Side quests
   - Daily quests
   - Achievement quests

2. Quest Features
   - Progress tracking
   - State management
   - Reward systems
   - Completion handling

3. Objective Types
   - Collection tasks
   - Combat goals
   - Exploration targets
   - Time challenges

4. Quest Systems
   - Progress tracking
   - State management
   - Event handling
   - Reward distribution
