# DataStoreGetVariableRequest

Category: User World Data Store Service Related

Class: Request to get a variable from the data store.

The DataStoreGetVariableRequest class provides access to the value of a requested variable from the user's world data store. It includes properties for retrieving the value in various data types, making it easy to work with different types of stored data.

## Properties/Fields

| Property | Description |
| --- | --- |
| value | The generic value of the variable. |
| boolValue | The value as a boolean. |
| boolArrayValue | The value as a boolean array. |
| intValue | The value as an integer. |
| intArrayValue | The value as an integer array. |
| floatValue | The value as a float. |
| floatArrayValue | The value as a float array. |
| stringValue | The value as a string. |
| stringArrayValue | The value as a string array. |
| longValue | The value as a long. |
| doubleValue | The value as a double. |
| decimalValue | The value as a decimal. |
| dateTimeValue | The value as a DateTime. |
| vector2Value | The value as a Vector2. |
| vector3Value | The value as a Vector3. |
| vector4Value | The value as a Vector4. |
| quaternionValue | The value as a Quaternion. |
| colorValue | The value as a Color. |
| dictionaryValue | The value as a dictionary. |

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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    private IUserWorldDataStoreService dataStore;
    
    [Serializable]
    public class PlayerStats
    {
        public string name;
        public int level;
        public float health;
        public Vector3 lastPosition;
        public Dictionary<string, int> inventory;
    }
    
    void Start()
    {
        dataStore = SpatialBridge.userWorldDataStoreService;
        
        // Load player data from the data store
        LoadAllPlayerData();
    }
    
    private void LoadAllPlayerData()
    {
        // Example demonstrating different data types with DataStoreGetVariableRequest
        LoadPlayerBasicInfo();
        LoadPlayerPosition();
        LoadPlayerInventory();
        LoadPlayerAchievements();
    }
    
    private void LoadPlayerBasicInfo()
    {
        // Loading a string value
        dataStore.GetVariable("playerName", "New Player")
            .SetCompletedEvent((request) => {
                if (request.succeeded)
                {
                    // Access the string value specifically
                    string playerName = request.stringValue;
                    Debug.Log($"Player name: {playerName}");
                }
                else
                {
                    Debug.LogWarning($"Failed to load player name: {request.responseCode}");
                }
            });
            
        // Loading an integer value
        dataStore.GetVariable("playerLevel", 1)
            .SetCompletedEvent((request) => {
                if (request.succeeded)
                {
                    // Access the integer value specifically
                    int playerLevel = request.intValue;
                    Debug.Log($"Player level: {playerLevel}");
                }
                else
                {
                    Debug.LogWarning($"Failed to load player level: {request.responseCode}");
                }
            });
            
        // Loading a float value
        dataStore.GetVariable("playerHealth", 100f)
            .SetCompletedEvent((request) => {
                if (request.succeeded)
                {
                    // Access the float value specifically
                    float playerHealth = request.floatValue;
                    Debug.Log($"Player health: {playerHealth}");
                }
                else
                {
                    Debug.LogWarning($"Failed to load player health: {request.responseCode}");
                }
            });
            
        // Loading a DateTime value
        dataStore.GetVariable("lastLoginTime", DateTime.MinValue)
            .SetCompletedEvent((request) => {
                if (request.succeeded)
                {
                    // Access the DateTime value specifically
                    DateTime lastLogin = request.dateTimeValue;
                    Debug.Log($"Last login time: {lastLogin}");
                }
                else
                {
                    Debug.LogWarning($"Failed to load last login time: {request.responseCode}");
                }
            });
    }
    
    private void LoadPlayerPosition()
    {
        // Loading a Vector3 value
        dataStore.GetVariable("playerPosition", Vector3.zero)
            .SetCompletedEvent((request) => {
                if (request.succeeded)
                {
                    // Access the Vector3 value specifically
                    Vector3 position = request.vector3Value;
                    Debug.Log($"Player position: {position}");
                    
                    // Example: Teleport player to last saved position
                    // transform.position = position;
                }
                else
                {
                    Debug.LogWarning($"Failed to load player position: {request.responseCode}");
                }
            });
    }
    
    private void LoadPlayerInventory()
    {
        // Loading a Dictionary value
        dataStore.GetVariable("inventory", new Dictionary<string, object>())
            .SetCompletedEvent((request) => {
                if (request.succeeded)
                {
                    // Access the Dictionary value specifically
                    Dictionary<string, object> inventory = request.dictionaryValue;
                    
                    Debug.Log("Player inventory:");
                    foreach (var item in inventory)
                    {
                        Debug.Log($"  {item.Key}: {item.Value}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Failed to load player inventory: {request.responseCode}");
                }
            });
    }
    
    private void LoadPlayerAchievements()
    {
        // Loading a string array value
        dataStore.GetVariable("unlockedAchievements", new string[0])
            .SetCompletedEvent((request) => {
                if (request.succeeded)
                {
                    // Access the string array value specifically
                    string[] achievements = request.stringArrayValue;
                    
                    Debug.Log($"Player has unlocked {achievements.Length} achievements:");
                    foreach (string achievement in achievements)
                    {
                        Debug.Log($"  - {achievement}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Failed to load player achievements: {request.responseCode}");
                }
            });
    }
}
```

## Best Practices

1. Always check the `succeeded` property before accessing value properties
2. Provide appropriate default values when retrieving variables that may not exist
3. Access the specific typed value property (e.g., `intValue`, `stringValue`) that matches your expected data type
4. Use the `value` property only when you need to work with it as a generic object
5. Handle type mismatches gracefully - accessing the wrong type property may cause errors
6. Consider implementing caching for frequently accessed values to reduce load times
7. Group related values into dictionaries for more efficient storage and retrieval

## Common Use Cases

1. Loading player profiles and preferences
2. Retrieving game state and progress information
3. Accessing saved settings and configurations
4. Loading player location, inventory, or statistics
5. Retrieving game high scores or achievements
6. Checking unlocked items or features
7. Loading saved building designs or user-created content
