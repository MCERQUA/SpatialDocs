# DataStoreOperationRequest

Category: User World Data Store Service Related

Class: Base class for operations performed on the data store.

DataStoreOperationRequest is the base class for all data store operation requests in the Spatial SDK. It provides common properties and functionality for operations like getting variables, checking for variables, and dumping variable contents.

## Properties/Fields

| Property | Description |
| --- | --- |
| succeeded | Whether the operation succeeded or not. |
| responseCode | The response code for the operation, of type DataStoreResponseCode. |

## Inherited Properties

From SpatialAsyncOperation:
| Property | Description |
| --- | --- |
| completed | Event that is invoked when the operation is completed. |
| isDone | Returns true if the operation is done. |
| keepWaiting | Returns true if the operation is not done. |

## Methods

From SpatialAsyncOperation:
| Method | Description |
| --- | --- |
| InvokeCompletionEvent() | Invokes the completion event. |

## Extension Methods

| Method | Description |
| --- | --- |
| SetCompletedEvent<T>(T, Action<T>) | Sets the completion event and returns the operation itself for chaining. |

## Usage Example

```csharp
using SpatialSys.UnitySDK;
using System;
using UnityEngine;

public class DataStorageExample : MonoBehaviour
{
    private IUserWorldDataStoreService dataStore;
    
    void Start()
    {
        dataStore = SpatialBridge.userWorldDataStoreService;
        
        // Basic usage pattern for data store operations
        CheckIfVariableExists();
    }
    
    private void CheckIfVariableExists()
    {
        // All data store operations return a DataStoreOperationRequest or derived class
        DataStoreHasVariableRequest request = dataStore.HasVariable("lastLoginTime");
        
        // You can use the SetCompletedEvent extension method for cleaner code
        request.SetCompletedEvent((operation) => {
            if (operation.succeeded)
            {
                Debug.Log($"Operation completed successfully. Response: {operation.responseCode}");
                
                // Cast to specific request type to access additional properties
                bool exists = ((DataStoreHasVariableRequest)operation).hasVariable;
                Debug.Log($"Variable exists: {exists}");
                
                if (exists)
                {
                    LoadLastLoginTime();
                }
                else
                {
                    SaveCurrentLoginTime();
                }
            }
            else
            {
                Debug.LogError($"Operation failed. Response: {operation.responseCode}");
                HandleError(operation.responseCode);
            }
        });
        
        // Alternative: You can use it as a coroutine
        StartCoroutine(WaitForOperation(request));
    }
    
    private System.Collections.IEnumerator WaitForOperation(DataStoreOperationRequest request)
    {
        // Wait for the operation to complete
        yield return request;
        
        // Check the result
        if (request.succeeded)
        {
            Debug.Log("Operation completed successfully");
        }
        else
        {
            Debug.LogError($"Operation failed: {request.responseCode}");
        }
    }
    
    private void LoadLastLoginTime()
    {
        dataStore.GetVariable("lastLoginTime", DateTime.MinValue)
            .SetCompletedEvent((response) => {
                if (response.succeeded)
                {
                    DateTime lastLogin = response.dateTimeValue;
                    Debug.Log($"Last login: {lastLogin}");
                }
            });
    }
    
    private void SaveCurrentLoginTime()
    {
        dataStore.SetVariable("lastLoginTime", DateTime.UtcNow);
        Debug.Log("Saved current login time");
    }
    
    private void HandleError(DataStoreResponseCode errorCode)
    {
        switch (errorCode)
        {
            case DataStoreResponseCode.VariableDoesNotExist:
                Debug.Log("Variable does not exist");
                break;
            case DataStoreResponseCode.InternalServerError:
                Debug.LogError("Server error occurred");
                break;
            default:
                Debug.LogError($"Unknown error: {errorCode}");
                break;
        }
    }
}
```

## Best Practices

1. Always check the `succeeded` property before accessing other properties
2. Handle operation failures by checking the `responseCode`
3. Use the `SetCompletedEvent` extension method for clean callback-based code
4. Implement timeout handling for operations that might take time
5. Consider using coroutines for operations that need to coordinate with other systems
6. Cast to the specific request type when you need to access specialized properties

## Common Use Cases

1. Base class for more specific data store operation requests
2. Common error handling for data store operations
3. Asynchronous data storage and retrieval
4. Persistent player data management
5. Game state saving and loading
6. User preferences storage
7. Achievement and progress tracking
