# SpawnNetworkObjectRequest

Category: Space Content Service

Class

Represents the result of a request to spawn a network object in the space. This class inherits from SpawnSpaceObjectRequest and provides access to the spawned network object.

## Properties

| Property | Description |
| --- | --- |
| gameObject | The Unity GameObject that was created for the network object. Will be null if the request failed. |
| networkObject | The network object that was spawned as a result of the request. Will be null if the request failed. |

## Inherited Members

| Member | Description |
| --- | --- |
| spaceObject | The space object that was created. Will be null if the request failed. |
| succeeded | Whether the spawn operation was successful. |
| InvokeCompletionEvent() | Invokes the completion event. |
| completed | Event that is invoked when the operation is completed. |
| isDone | Returns true if the operation is done. |
| keepWaiting | Returns true if the operation is not done. |

## Extension Methods

| Method | Description |
| --- | --- |
| SetCompletedEvent<T>(T, Action<T>) | Sets the completion event, same as setting the event using the completed property, but returns the operation itself for method chaining. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections;

public class NetworkObjectSpawner : MonoBehaviour
{
    // Reference to a prefab that has a SpatialNetworkObject component
    public GameObject networkObjectPrefab;
    
    // Example: Spawn a network object at a specific position
    public void SpawnNetworkObjectAtPosition(Vector3 position, Quaternion rotation)
    {
        Debug.Log($"Spawning network object at position {position}");
        
        SpatialBridge.spaceContentService.SpawnNetworkObject(networkObjectPrefab, position, rotation)
            .SetCompletedEvent(HandleNetworkObjectSpawnResult);
    }
    
    // Handler for network object spawn completion
    private void HandleNetworkObjectSpawnResult(SpawnNetworkObjectRequest request)
    {
        if (request.succeeded)
        {
            Debug.Log($"Network object successfully spawned with space object ID: {request.spaceObject.objectID}");
            
            // Access the spawned network object's game object
            GameObject objInstance = request.gameObject;
            Debug.Log($"Spawned GameObject: {objInstance.name}");
            
            // Example: Access components on the spawned object
            Renderer renderer = objInstance.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Modify the material to distinguish this instance
                renderer.material.color = Random.ColorHSV();
            }
            
            // Example: Add components or configure the game object
            objInstance.AddComponent<RotateObject>();
            
            // Example: Set variables on the space object
            request.spaceObject.SetVariable(0, "CreationTime");
            request.spaceObject.SetVariable(1, System.DateTime.Now.ToString());
        }
        else
        {
            Debug.LogWarning("Failed to spawn network object");
        }
    }
    
    // Example: Spawn multiple network objects in a grid
    public void SpawnObjectGrid(int rows, int columns, float spacing = 2.0f)
    {
        Vector3 basePosition = transform.position;
        
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 position = basePosition + new Vector3(col * spacing, 0, row * spacing);
                Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                
                SpatialBridge.spaceContentService.SpawnNetworkObject(networkObjectPrefab, position, rotation)
                    .SetCompletedEvent((request) => {
                        if (request.succeeded)
                        {
                            // Configure each instance differently
                            request.gameObject.name = $"GridObject_{row}_{col}";
                            
                            // Example: Set a unique color based on grid position
                            Renderer renderer = request.gameObject.GetComponent<Renderer>();
                            if (renderer != null)
                            {
                                float hue = (float)(row * columns + col) / (rows * columns);
                                renderer.material.color = Color.HSVToRGB(hue, 0.8f, 0.8f);
                            }
                            
                            // Example: Store the grid coordinates in space object variables
                            request.spaceObject.SetVariable(0, "GridPosition");
                            request.spaceObject.SetVariable(1, row);
                            request.spaceObject.SetVariable(2, col);
                        }
                    });
            }
        }
    }
    
    // Example: Spawn a network object and modify it using coroutines
    public IEnumerator SpawnAndAnimateObject(Vector3 position)
    {
        Debug.Log("Starting network object spawn process...");
        
        // Request to spawn the network object
        var request = SpatialBridge.spaceContentService.SpawnNetworkObject(
            networkObjectPrefab, 
            position, 
            Quaternion.identity
        );
        
        // Wait for the operation to complete
        yield return request;
        
        if (request.succeeded)
        {
            Debug.Log("Network object spawn successful!");
            
            // Get references
            GameObject obj = request.gameObject;
            ISpaceObject spaceObject = request.spaceObject;
            
            // Example: Animate the object
            Vector3 startPosition = obj.transform.position;
            float animDuration = 2.0f;
            float elapsed = 0;
            
            while (elapsed < animDuration)
            {
                float t = elapsed / animDuration;
                
                // Example animation: Bob up and down
                obj.transform.position = startPosition + new Vector3(0, Mathf.Sin(t * Mathf.PI * 2) * 0.5f, 0);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Reset position
            obj.transform.position = startPosition;
            
            Debug.Log("Animation complete!");
        }
        else
        {
            Debug.LogWarning("Network object spawn failed!");
        }
    }
    
    // Example: Spawn and destroy objects over time
    public IEnumerator SpawnAndDestroySequence(int count, float spawnDelay, float lifetime)
    {
        Debug.Log($"Starting spawn/destroy sequence for {count} objects");
        
        for (int i = 0; i < count; i++)
        {
            // Calculate position in a circle
            float angle = i * (360f / count);
            float radian = angle * Mathf.Deg2Rad;
            Vector3 position = transform.position + new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian)) * 5f;
            
            // Spawn the object
            var request = SpatialBridge.spaceContentService.SpawnNetworkObject(
                networkObjectPrefab,
                position,
                Quaternion.Euler(0, angle, 0)
            );
            
            yield return request;
            
            if (request.succeeded)
            {
                Debug.Log($"Spawned object {i+1}/{count}");
                
                // Store reference for later destruction
                ISpaceObject spaceObj = request.spaceObject;
                
                // Schedule destruction after lifetime
                StartCoroutine(DestroyAfterDelay(spaceObj, lifetime));
            }
            
            // Wait before spawning next object
            yield return new WaitForSeconds(spawnDelay);
        }
    }
    
    // Helper to destroy an object after a delay
    private IEnumerator DestroyAfterDelay(ISpaceObject spaceObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (spaceObject != null && !spaceObject.isDisposed)
        {
            Debug.Log($"Destroying space object #{spaceObject.objectID} after {delay} seconds");
            
            SpatialBridge.spaceContentService.DestroySpaceObject(spaceObject)
                .SetCompletedEvent((request) => {
                    if (request.succeeded)
                    {
                        Debug.Log($"Successfully destroyed space object #{spaceObject.objectID}");
                    }
                });
        }
    }
    
    // Example helper component for rotation
    private class RotateObject : MonoBehaviour
    {
        public float rotationSpeed = 30f;
        
        private void Update()
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}
```

## Best Practices

1. Always check the `succeeded` property before accessing the `gameObject`, `networkObject`, or `spaceObject` properties.
2. Use `SetCompletedEvent` for clean callback handling of network object spawn operations.
3. Remember that the `gameObject` property gives you direct access to the Unity GameObject, allowing you to add or modify components.
4. Use space object variables to store persistent data related to the network object that should be synchronized across clients.
5. Consider using coroutines for complex sequences involving multiple network objects.
6. Remember that all clients will see the same network objects, so design your spawn logic accordingly.
7. Be mindful of performance when spawning many network objects - consider batching or limiting the number of objects.

## Common Use Cases

1. Creating interactive objects that can be modified by multiple users.
2. Spawning items, collectibles, or power-ups in multiplayer games.
3. Building dynamic environments with objects that can be created and destroyed at runtime.
4. Implementing physics-based puzzles or challenges with synchronizing objects.
5. Creating shared tools or resources that multiple users can interact with.
6. Building systems for user-generated content within the space.
7. Implementing gameplay mechanics that involve spawning and manipulating objects in real-time.

## Completed: March 9, 2025