# IUserWorldDataStoreService

User World Data Store Service Interface

Service for managing persistent user data across spaces within the same world. This service provides functionality for storing and retrieving user-specific data that persists between sessions.

## Methods

### Variable Management
| Method | Description |
| --- | --- |
| SetVariable(string, object) | Store variable value |
| GetVariable(string, object) | Retrieve variable value |
| DeleteVariable(string) | Remove variable |
| ClearAllVariables() | Delete all variables |

### Variable Checking
| Method | Description |
| --- | --- |
| HasVariable(string) | Check variable exists |
| HasAnyVariable() | Check store empty |
| DumpVariablesAsJSON() | Debug data dump |

## Usage Examples

```csharp
// Example: User Data Manager
public class UserDataManager : MonoBehaviour
{
    private IUserWorldDataStoreService dataStore;
    private Dictionary<string, DataState> dataStates;
    private bool isInitialized;

    private class DataState
    {
        public bool isLoaded;
        public float lastUpdateTime;
        public object currentValue;
        public Dictionary<string, object> metadata;
    }

    void Start()
    {
        dataStore = SpatialBridge.userWorldDataStoreService;
        dataStates = new Dictionary<string, DataState>();
        InitializeManager();
    }

    private void InitializeManager()
    {
        LoadStoredData();
    }

    public void SaveUserData<T>(
        string key,
        T value
    )
    {
        try
        {
            dataStore.SetVariable(key, value);
            UpdateDataState(key, value);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving data {key}: {e.Message}");
        }
    }

    public void LoadUserData<T>(
        string key,
        T defaultValue,
        Action<T> onLoaded = null
    )
    {
        try
        {
            dataStore.GetVariable(key, defaultValue)
                .SetCompletedEvent((response) => {
                    var value = (T)response.value;
                    UpdateDataState(key, value);
                    onLoaded?.Invoke(value);
                });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading data {key}: {e.Message}");
            onLoaded?.Invoke(defaultValue);
        }
    }

    public void DeleteUserData(string key)
    {
        try
        {
            dataStore.DeleteVariable(key);
            
            if (dataStates.TryGetValue(key, out var state))
            {
                state.isLoaded = false;
                state.metadata["deleteTime"] = Time.time;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error deleting data {key}: {e.Message}");
        }
    }

    public void ClearAllUserData()
    {
        try
        {
            dataStore.ClearAllVariables();
            
            foreach (var state in dataStates.Values)
            {
                state.isLoaded = false;
                state.metadata["deleteTime"] = Time.time;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error clearing data: {e.Message}");
        }
    }

    private void LoadStoredData()
    {
        if (!dataStore.HasAnyVariable())
            return;

        var json = dataStore.DumpVariablesAsJSON();
        var data = JsonUtility.FromJson<Dictionary<string, object>>(json);
        
        foreach (var kvp in data)
        {
            InitializeDataState(kvp.Key, kvp.Value);
        }
    }

    private void InitializeDataState(
        string key,
        object initialValue
    )
    {
        var state = new DataState
        {
            isLoaded = true,
            lastUpdateTime = Time.time,
            currentValue = initialValue,
            metadata = new Dictionary<string, object>
            {
                { "createTime", Time.time },
                { "updateCount", 0 },
                { "version", 1 }
            }
        };

        dataStates[key] = state;
    }

    private void UpdateDataState(
        string key,
        object value
    )
    {
        if (!dataStates.TryGetValue(key, out var state))
        {
            InitializeDataState(key, value);
            return;
        }

        state.currentValue = value;
        state.lastUpdateTime = Time.time;
        state.metadata["updateCount"] = (int)state.metadata["updateCount"] + 1;
        state.metadata["version"] = (int)state.metadata["version"] + 1;
    }
}

// Example: Game Progress Manager
public class GameProgressManager : MonoBehaviour
{
    private UserDataManager dataManager;
    private Dictionary<string, ProgressState> progressStates;
    private bool isInitialized;

    private class ProgressState
    {
        public bool isLoaded;
        public float lastUpdateTime;
        public Dictionary<string, object> data;
    }

    void Start()
    {
        dataManager = GetComponent<UserDataManager>();
        progressStates = new Dictionary<string, ProgressState>();
        InitializeProgress();
    }

    private void InitializeProgress()
    {
        LoadPlayerProgress();
        LoadGameSettings();
        LoadAchievements();
    }

    public void SavePlayerProgress(
        int level,
        int score,
        float playTime
    )
    {
        var progress = new Dictionary<string, object>
        {
            { "level", level },
            { "score", score },
            { "playTime", playTime },
            { "lastSave", DateTime.UtcNow }
        };

        dataManager.SaveUserData("playerProgress", progress);
        UpdateProgressState("progress", progress);
    }

    public void LoadPlayerProgress(Action<Dictionary<string, object>> onLoaded = null)
    {
        var defaultProgress = new Dictionary<string, object>
        {
            { "level", 1 },
            { "score", 0 },
            { "playTime", 0f }
        };

        dataManager.LoadUserData("playerProgress", defaultProgress, (progress) => {
            UpdateProgressState("progress", progress);
            onLoaded?.Invoke(progress);
        });
    }

    public void SaveGameSettings(
        float musicVolume,
        float sfxVolume,
        bool tutorialComplete
    )
    {
        var settings = new Dictionary<string, object>
        {
            { "musicVolume", musicVolume },
            { "sfxVolume", sfxVolume },
            { "tutorialComplete", tutorialComplete }
        };

        dataManager.SaveUserData("gameSettings", settings);
        UpdateProgressState("settings", settings);
    }

    public void LoadGameSettings(Action<Dictionary<string, object>> onLoaded = null)
    {
        var defaultSettings = new Dictionary<string, object>
        {
            { "musicVolume", 1f },
            { "sfxVolume", 1f },
            { "tutorialComplete", false }
        };

        dataManager.LoadUserData("gameSettings", defaultSettings, (settings) => {
            UpdateProgressState("settings", settings);
            onLoaded?.Invoke(settings);
        });
    }

    private void UpdateProgressState(
        string key,
        Dictionary<string, object> data
    )
    {
        if (!progressStates.TryGetValue(key, out var state))
        {
            state = new ProgressState
            {
                isLoaded = true,
                lastUpdateTime = Time.time,
                data = new Dictionary<string, object>()
            };
            progressStates[key] = state;
        }

        state.lastUpdateTime = Time.time;
        state.data = new Dictionary<string, object>(data);
    }

    public void ResetProgress()
    {
        dataManager.ClearAllUserData();
        progressStates.Clear();
        InitializeProgress();
    }
}
```

## Best Practices

1. Data Management
   - Validate data
   - Handle types
   - Track changes
   - Cache values

2. Error Handling
   - Check existence
   - Handle failures
   - Provide defaults
   - Log issues

3. Performance
   - Batch updates
   - Cache data
   - Handle timing
   - Optimize loads

4. State Management
   - Track loading
   - Handle updates
   - Monitor changes
   - Clean disposal

## Common Use Cases

1. Game Progress
   - Save states
   - Player stats
   - Achievements
   - Settings

2. User Preferences
   - Game options
   - UI settings
   - Control configs
   - Audio settings

3. Player Data
   - Inventory items
   - Character stats
   - Quest progress
   - Currency amounts

4. Analytics
   - Play metrics
   - User behavior
   - Game stats
   - Performance data
