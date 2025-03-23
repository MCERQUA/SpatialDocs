# DataStoreHasAnyVariableRequest

Category: User World Data Store Service Related

Class: Request to check if the data store has any variables.

The DataStoreHasAnyVariableRequest class allows developers to check if the user's world data store contains any variables. This is useful for determining if a user is new to an experience, checking if data needs to be initialized, or for debugging purposes.

## Properties/Fields

| Property | Description |
| --- | --- |
| hasAnyVariable | Whether the data store has any variables or not. |

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

public class NewUserDetection : MonoBehaviour
{
    private IUserWorldDataStoreService dataStore;
    
    void Start()
    {
        dataStore = SpatialBridge.userWorldDataStoreService;
        
        // Check if this is a new user
        CheckUserDataExists();
    }
    
    private void CheckUserDataExists()
    {
        // Create a request to check if any variables exist in the data store
        DataStoreHasAnyVariableRequest request = dataStore.HasAnyVariable();
        
        // Use the SetCompletedEvent extension method to handle the result
        request.SetCompletedEvent((operation) => {
            if (operation.succeeded)
            {
                bool hasData = operation.hasAnyVariable;
                
                if (hasData)
                {
                    Debug.Log("Existing user detected - loading user data...");
                    LoadUserData();
                }
                else
                {
                    Debug.Log("New user detected - initializing default data...");
                    InitializeNewUserData();
                }
            }
            else
            {
                Debug.LogError($"Failed to check if user has data: {operation.responseCode}");
                // Use fallback approach
                AttemptToLoadUserDataAnyway();
            }
        });
        
        // Alternative: Use coroutine approach
        StartCoroutine(CheckUserDataCoroutine());
    }
    
    private IEnumerator CheckUserDataCoroutine()
    {
        var request = dataStore.HasAnyVariable();
        
        // Wait for the operation to complete
        yield return request;
        
        if (request.succeeded)
        {
            if (request.hasAnyVariable)
            {
                Debug.Log("User has existing data");
                ShowWelcomeBackMessage();
            }
            else
            {
                Debug.Log("First-time user");
                ShowTutorial();
            }
        }
        else
        {
            Debug.LogError($"Error checking user data: {request.responseCode}");
        }
    }
    
    private void InitializeNewUserData()
    {
        // Set up default values for a new user
        dataStore.SetVariable("firstVisitTime", System.DateTime.UtcNow);
        dataStore.SetVariable("playerName", "New Player");
        dataStore.SetVariable("playerLevel", 1);
        dataStore.SetVariable("tutorialComplete", false);
        dataStore.SetVariable("currency", 100);
        
        // Show first-time user experience
        ShowTutorial();
    }
    
    private void LoadUserData()
    {
        // Load existing user data
        // Implementation would retrieve specific variables
        Debug.Log("Loading existing user data...");
    }
    
    private void ShowTutorial()
    {
        // Show tutorial for new users
        Debug.Log("Showing tutorial for new user");
    }
    
    private void ShowWelcomeBackMessage()
    {
        // Show welcome back message for returning users
        Debug.Log("Showing welcome back message");
    }
    
    private void AttemptToLoadUserDataAnyway()
    {
        // Fallback approach when HasAnyVariable fails
        Debug.Log("Attempting to load specific variables as fallback");
        
        // Try to load a specific variable that should exist if the user has data
        dataStore.GetVariable("firstVisitTime", System.DateTime.MinValue)
            .SetCompletedEvent((response) => {
                bool isNewUser = response.dateTimeValue == System.DateTime.MinValue;
                
                if (isNewUser)
                {
                    InitializeNewUserData();
                }
                else
                {
                    LoadUserData();
                }
            });
    }
}
```

## Best Practices

1. Use HasAnyVariable as an initial check before performing more specific operations
2. Implement fallback mechanisms in case the operation fails
3. Use the result to determine whether to initialize default data for new users
4. Consider caching the result if checking multiple times in a short period
5. Handle operation failures gracefully to ensure a smooth user experience
6. Follow up with more specific variable checks when necessary

## Common Use Cases

1. Detecting first-time users vs. returning users
2. Initializing default data for new users
3. Deciding whether to show tutorials or onboarding experiences
4. Checking if data migration or initialization is needed
5. Debugging to verify if data is being saved properly
6. Implementing "reset progress" functionality by checking before and after clearing data
7. Determining whether to display "continue" options in a game or experience
