# DataStoreHasVariableRequest

Category: User World Data Store Service Related

Class: Request to check if a variable exists in the data store.

The DataStoreHasVariableRequest class allows developers to check if a specific variable exists in the user's world data store. This is useful for conditional logic, preventing unnecessary operations, or validating data before attempting to access it.

## Properties/Fields

| Property | Description |
| --- | --- |
| hasVariable | Whether the variable exists or not. |

## Inherited Properties

From DataStoreOperationRequest:
| Property | Description |
| --- | --- |
| succeeded | Whether the operation succeeded or not. |
| responseCode | The response code for the operation. |

From SpatialAsyncOperation:
| Property | Description |
| --- | --- |
| completed | Event that is invoked when the operation is completed. |
| isDone | Returns true if the operation is done. |
| keepWaiting | Returns true if the operation is not done. |

## Usage Example

```csharp
using SpatialSys.UnitySDK;
using System.Collections;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    private IUserWorldDataStoreService dataStore;
    
    void Start()
    {
        dataStore = SpatialBridge.userWorldDataStoreService;
        
        // Check if player has completed the tutorial
        CheckTutorialStatus();
        
        // Check for saved progress before attempting to load
        CheckAndLoadGameProgress();
    }
    
    private void CheckTutorialStatus()
    {
        // Check if the tutorialCompleted variable exists
        dataStore.HasVariable("tutorialCompleted")
            .SetCompletedEvent((request) => {
                if (request.succeeded)
                {
                    bool exists = request.hasVariable;
                    
                    if (exists)
                    {
                        // Get the value to determine if tutorial is completed
                        LoadTutorialCompletionStatus();
                    }
                    else
                    {
                        // Variable doesn't exist, player has never completed the tutorial
                        Debug.Log("Tutorial status not found - showing tutorial");
                        ShowTutorial();
                        
                        // Initialize the variable for future checks
                        dataStore.SetVariable("tutorialCompleted", false);
                    }
                }
                else
                {
                    Debug.LogError($"Failed to check tutorial status: {request.responseCode}");
                    // Fallback to showing tutorial to be safe
                    ShowTutorial();
                }
            });
    }
    
    private void LoadTutorialCompletionStatus()
    {
        dataStore.GetVariable("tutorialCompleted", false)
            .SetCompletedEvent((response) => {
                bool tutorialCompleted = response.boolValue;
                
                if (tutorialCompleted)
                {
                    Debug.Log("Tutorial already completed - skipping tutorial");
                    SkipTutorial();
                }
                else
                {
                    Debug.Log("Tutorial not yet completed - showing tutorial");
                    ShowTutorial();
                }
            });
    }
    
    private void CheckAndLoadGameProgress()
    {
        // Building a more complex check - multiple variables
        StartCoroutine(LoadProgressIfExists());
    }
    
    private IEnumerator LoadProgressIfExists()
    {
        // Check if player level exists
        var levelRequest = dataStore.HasVariable("playerLevel");
        yield return levelRequest;
        
        // Check if player inventory exists
        var inventoryRequest = dataStore.HasVariable("inventory");
        yield return inventoryRequest;
        
        // Check if quest progress exists
        var questsRequest = dataStore.HasVariable("questProgress");
        yield return questsRequest;
        
        // Determine if we have enough data to restore a game session
        bool hasLevel = levelRequest.succeeded && levelRequest.hasVariable;
        bool hasInventory = inventoryRequest.succeeded && inventoryRequest.hasVariable;
        bool hasQuests = questsRequest.succeeded && questsRequest.hasVariable;
        
        bool canRestoreSession = hasLevel && hasInventory;
        
        if (canRestoreSession)
        {
            Debug.Log("Found existing game progress - loading game state");
            LoadGameState();
        }
        else
        {
            Debug.Log("Could not find complete game progress - starting new game");
            StartNewGame();
        }
        
        // Log what's missing if partial data exists
        if (hasLevel && !hasInventory)
        {
            Debug.LogWarning("Found player level but inventory is missing - possible data corruption");
        }
        
        if (!hasQuests)
        {
            Debug.Log("No quest progress found - initializing new quest system");
            InitializeQuestSystem();
        }
    }
    
    private void ShowTutorial()
    {
        // Implementation to show tutorial
        Debug.Log("Showing tutorial UI and guides");
    }
    
    private void SkipTutorial()
    {
        // Implementation to skip tutorial
        Debug.Log("Skipping tutorial and proceeding to main game");
    }
    
    private void LoadGameState()
    {
        // Implementation to load the game state from variables
        Debug.Log("Loading saved game state from data store");
    }
    
    private void StartNewGame()
    {
        // Implementation to start a new game
        Debug.Log("Starting new game with default values");
    }
    
    private void InitializeQuestSystem()
    {
        // Implementation to initialize the quest system
        Debug.Log("Setting up default quest progress");
        dataStore.SetVariable("questProgress", new System.Collections.Generic.Dictionary<string, object>());
    }
    
    // This method would be called when the player completes the tutorial
    public void CompleteTutorial()
    {
        dataStore.SetVariable("tutorialCompleted", true);
        Debug.Log("Tutorial marked as completed!");
    }
}
```

## Best Practices

1. Use HasVariable to check for specific variables before attempting to access them
2. Handle the case when the variable doesn't exist by creating it or using a default
3. Validate critical game data using multiple HasVariable checks
4. Use HasVariable in combination with GetVariable for better error handling
5. Consider checking related variables together to ensure data consistency
6. Cache results when performing multiple checks on the same variable in a short time
7. Implement fallbacks when HasVariable operations fail

## Common Use Cases

1. Checking if a user has completed specific game milestones
2. Validating if settings or preferences have been configured
3. Determining if a feature has been unlocked or discovered
4. Verifying if specific progress data exists before attempting to load it
5. Implementing conditional logic based on the existence of variables
6. Debugging to check if specific data was properly saved
7. Detecting partial or corrupted data by checking for expected variables
8. Feature flagging and toggle management
