# DataStoreDumpVariablesRequest

Category: User World Data Store Service Related

Class: Request to dump all variables in the data store as a JSON string.

The DataStoreDumpVariablesRequest class is used to retrieve the complete contents of the user's world data store as a JSON string. This is particularly useful for debugging, creating save systems, or performing bulk operations on stored data.

## Properties/Fields

| Property | Description |
| --- | --- |
| json | The current contents of the data store as a JSON string. |

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
using System.Collections.Generic;
using UnityEngine;

public class DataDumpExample : MonoBehaviour
{
    private IUserWorldDataStoreService dataStore;
    
    void Start()
    {
        dataStore = SpatialBridge.userWorldDataStoreService;
        
        // Save some example data first
        SaveExampleData();
        
        // Then dump all variables
        StartCoroutine(DumpAllVariables());
    }
    
    private void SaveExampleData()
    {
        // Save some example data to demonstrate the dump functionality
        dataStore.SetVariable("playerName", "SpaceExplorer");
        dataStore.SetVariable("playerLevel", 10);
        dataStore.SetVariable("playerPosition", new Vector3(10, 5, 20));
        dataStore.SetVariable("lastLogin", System.DateTime.UtcNow);
        
        // Save a complex object
        Dictionary<string, object> inventory = new Dictionary<string, object>
        {
            { "coins", 500 },
            { "gems", 50 },
            { "items", new string[] { "sword", "shield", "potion" } }
        };
        
        dataStore.SetVariable("playerInventory", inventory);
    }
    
    private IEnumerator DumpAllVariables()
    {
        // Check if there are any variables first
        var checkRequest = dataStore.HasAnyVariable();
        yield return checkRequest;
        
        if (!checkRequest.succeeded || !((DataStoreHasAnyVariableRequest)checkRequest).hasAnyVariable)
        {
            Debug.Log("No variables to dump - data store is empty");
            yield break;
        }
        
        // Request all data as JSON
        var dumpRequest = dataStore.DumpVariablesAsJSON();
        
        // Wait for the operation to complete
        yield return dumpRequest;
        
        if (dumpRequest.succeeded)
        {
            string json = dumpRequest.json;
            Debug.Log("Data Store Contents:");
            Debug.Log(json);
            
            // You could also save this JSON to a file or perform operations on it
            ProcessDumpedData(json);
        }
        else
        {
            Debug.LogError($"Failed to dump variables: {dumpRequest.responseCode}");
        }
    }
    
    private void ProcessDumpedData(string json)
    {
        // Here you could parse the JSON and perform operations
        // For example, count the number of variables
        Dictionary<string, object> variables = JsonUtility.FromJson<Dictionary<string, object>>(json);
        Debug.Log($"Total variables in data store: {variables.Count}");
        
        // Or you could implement a backup system
        SaveBackup(json);
    }
    
    private void SaveBackup(string json)
    {
        // Example: Save a timestamped backup of the user's data
        string timestamp = System.DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        string backupKey = $"backup_{timestamp}";
        
        dataStore.SetVariable(backupKey, json);
        Debug.Log($"Backup created with key: {backupKey}");
    }
}
```

## Best Practices

1. Use DataStoreDumpVariablesRequest primarily for debugging or creating backup/save systems
2. Check if any variables exist before attempting to dump (for efficiency)
3. Handle large data dumps appropriately - they may contain a significant amount of data
4. Consider rate-limiting dumps to prevent performance issues
5. Secure sensitive data - be cautious about logging the full JSON to debug output
6. Process the JSON data efficiently, especially for large data stores

## Common Use Cases

1. Debugging user data issues
2. Creating backup systems for user data
3. Exporting user data for analysis
4. Implementing save/load systems
5. Migrating data between different storage structures
6. Copying data between users or worlds
7. Visualizing all stored data for development purposes
