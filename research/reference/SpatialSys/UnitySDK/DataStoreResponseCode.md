# DataStoreResponseCode

Category: User World Data Store Service Related

Enum: Response code for data store operations.

Provides status codes for operations performed on the data store, enabling proper error handling and flow control for data storage operations.

## Properties/Fields

| Value | Description |
| --- | --- |
| Ok | Operation was successful. |
| VariableDoesNotExist | Variable does not exist. |
| VariableKeyInvalid | Variable key is invalid. |
| VariableKeyTooLong | Variable key is too long. |
| VariableKeyInvalidCharacters | Variable key contains invalid characters. |
| VariableNameTooLong | Variable name is too long. |
| VariableNameInvalidCharacters | Variable name contains invalid characters. |
| UnsupportedValueType | Value type is unsupported. |
| UnsupportedDictionaryKeyType | Dictionary key type is unsupported. |
| ValueTypeUnknown | Value type is unknown. |
| VariableDepthTooDeep | Variable depth is too deep. |
| InternalError | Fault in Spatial code. |
| InternalServerError | The server encountered an error. |
| OperationCancelled | The operation was cancelled. |
| UnknownError | Unexpected error; Typically when it is not one of the other errors. |

## Usage Example

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;

public class DataStoreManager : MonoBehaviour
{
    private IUserWorldDataStoreService dataStore;

    void Start()
    {
        dataStore = SpatialBridge.userWorldDataStoreService;
        SavePlayerScore(100);
    }

    public void SavePlayerScore(int score)
    {
        try
        {
            dataStore.SetVariable("playerScore", score);
            Debug.Log("Score saved successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save score: {e.Message}");
        }
    }

    public void LoadPlayerProgress()
    {
        dataStore.GetVariable("playerScore", 0)
            .SetCompletedEvent((response) => {
                if (response.succeeded)
                {
                    int score = response.intValue;
                    Debug.Log($"Player score loaded: {score}");
                }
                else
                {
                    // Handle the specific error based on the response code
                    switch (response.responseCode)
                    {
                        case DataStoreResponseCode.VariableDoesNotExist:
                            Debug.Log("No previous score found, using default of 0.");
                            break;
                        case DataStoreResponseCode.VariableKeyInvalid:
                        case DataStoreResponseCode.VariableKeyTooLong:
                        case DataStoreResponseCode.VariableKeyInvalidCharacters:
                            Debug.LogError("Invalid variable key format!");
                            break;
                        case DataStoreResponseCode.InternalServerError:
                            Debug.LogError("Server error! Please try again later.");
                            break;
                        default:
                            Debug.LogError($"Unknown error: {response.responseCode}");
                            break;
                    }
                }
            });
    }
}
```

## Best Practices

1. Always check the response code when handling DataStoreOperationRequest operations
2. Provide appropriate user feedback based on the response code
3. Consider implementing retry logic for server-related errors (InternalServerError)
4. Use default values when handling VariableDoesNotExist errors
5. Add validation before saving values to prevent key-related errors

## Common Use Cases

1. Error handling in data store operations
2. Providing user feedback for data operations
3. Creating robust save/load systems
4. Implementing graceful failure handling
5. Determining if retry logic should be applied
6. Validating variable names and keys before attempting operations
