# SpawnAvatarRequest

Category: Space Content Service

Class

Represents the result of a request to spawn an avatar in the space. This class inherits from SpawnSpaceObjectRequest and provides access to the spawned avatar.

## Properties

| Property | Description |
| --- | --- |
| avatar | The avatar that was spawned as a result of the request. Will be null if the request failed. |

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

public class AvatarSpawner : MonoBehaviour
{
    // Example: Spawn an avatar at a specific position
    public void SpawnAvatarAtPosition(Vector3 position, Quaternion rotation)
    {
        Debug.Log($"Spawning avatar at position {position}");
        
        SpatialBridge.spaceContentService.SpawnAvatar(position, rotation)
            .SetCompletedEvent(HandleAvatarSpawnResult);
    }
    
    // Handler for avatar spawn completion
    private void HandleAvatarSpawnResult(SpawnAvatarRequest request)
    {
        if (request.succeeded)
        {
            Debug.Log($"Avatar successfully spawned with space object ID: {request.spaceObject.objectID}");
            
            // Access the spawned avatar
            IAvatar avatar = request.avatar;
            
            // Example: Configure the avatar
            avatar.nametagVisible = true;
            avatar.nametagSubtext = "Spawned Avatar";
            avatar.displayName = "Custom Avatar";
            
            // Example: Set avatar movement parameters
            avatar.walkSpeed = 3.0f;
            avatar.runSpeed = 6.0f;
            avatar.jumpHeight = 1.5f;
            
            // Example: Subscribe to avatar events
            avatar.onJump += () => Debug.Log("Avatar jumped!");
            avatar.onLanded += () => Debug.Log("Avatar landed!");
            avatar.onIsGroundedChanged += (isGrounded) => 
                Debug.Log($"Avatar grounded state changed: {isGrounded}");
        }
        else
        {
            Debug.LogWarning("Failed to spawn avatar");
        }
    }
    
    // Example: Spawn an avatar and modify its properties in a coroutine
    public IEnumerator SpawnAndConfigureAvatar(Vector3 position)
    {
        Debug.Log("Starting avatar spawn process...");
        
        // Request to spawn the avatar
        var request = SpatialBridge.spaceContentService.SpawnAvatar(position, Quaternion.identity);
        
        // Wait for the operation to complete
        yield return request;
        
        if (request.succeeded)
        {
            Debug.Log("Avatar spawn successful!");
            
            // Get references to the space object and avatar
            ISpaceObject spaceObject = request.spaceObject;
            IAvatar avatar = request.avatar;
            
            // Configure the avatar
            avatar.nametagVisible = true;
            avatar.nametagSubtext = "Coroutine Spawned";
            
            // Set a variable on the space object for demonstration
            spaceObject.SetVariable(0, "SpawnedAt");
            spaceObject.SetVariable(1, System.DateTime.Now.ToString());
            
            // Example: Wait for the avatar to be in a grounded state
            while (!avatar.isGrounded)
            {
                Debug.Log("Waiting for avatar to be grounded...");
                yield return new WaitForSeconds(0.5f);
            }
            
            Debug.Log("Avatar is now grounded!");
            
            // Example: Spawn multiple avatars in sequence
            yield return SpawnAdditionalAvatar(position + new Vector3(2, 0, 0));
            yield return SpawnAdditionalAvatar(position + new Vector3(-2, 0, 0));
            
            Debug.Log("All avatars spawned successfully!");
        }
        else
        {
            Debug.LogWarning("Avatar spawn failed!");
        }
    }
    
    // Helper method to spawn additional avatars
    private IEnumerator SpawnAdditionalAvatar(Vector3 position)
    {
        var request = SpatialBridge.spaceContentService.SpawnAvatar(position, Quaternion.identity);
        yield return request;
        
        if (request.succeeded)
        {
            Debug.Log($"Additional avatar spawned at {position}");
            
            // Configure this avatar differently
            request.avatar.nametagSubtext = "Additional";
            request.avatar.visibleRemotely = true;
        }
    }
    
    // Example: Spawn multiple avatars with different configurations
    public void SpawnMultipleAvatars(int count, float spacing = 2.0f)
    {
        Debug.Log($"Spawning {count} avatars...");
        
        for (int i = 0; i < count; i++)
        {
            Vector3 position = transform.position + new Vector3(i * spacing, 0, 0);
            Quaternion rotation = Quaternion.Euler(0, i * (360f / count), 0);
            
            SpatialBridge.spaceContentService.SpawnAvatar(position, rotation)
                .SetCompletedEvent((request) => {
                    if (request.succeeded)
                    {
                        Debug.Log($"Avatar {request.spaceObject.objectID} spawned");
                        
                        // Configure each avatar with different settings
                        request.avatar.nametagSubtext = $"Avatar {i}";
                        request.avatar.visibleLocally = true;
                        request.avatar.walkSpeed = 2.0f + i * 0.5f;
                        
                        // Customize color or appearance for visual distinction
                        if (request.avatar.bodyMaterials.Count > 0)
                        {
                            var material = request.avatar.bodyMaterials[0];
                            if (material != null)
                            {
                                // Set a different color for each avatar
                                Color color = Color.HSVToRGB(i * (1.0f / count), 0.8f, 0.8f);
                                material.color = color;
                            }
                        }
                    }
                });
        }
    }
    
    // Example: Create an avatar with custom respawn behavior
    public void CreateAvatarWithRespawnHandling()
    {
        Vector3 spawnPosition = new Vector3(0, 5, 0); // Intentionally spawning in the air
        
        SpatialBridge.spaceContentService.SpawnAvatar(spawnPosition, Quaternion.identity)
            .SetCompletedEvent((request) => {
                if (request.succeeded)
                {
                    Debug.Log("Avatar spawned in the air");
                    
                    // Subscribe to respawn events
                    request.avatar.onRespawned += (args) => {
                        if (args.isFirstSpawn)
                        {
                            Debug.Log("This is the first spawn of the avatar");
                        }
                        else
                        {
                            Debug.Log("Avatar has respawned");
                        }
                        
                        // You could update any game state here that needs to be reset on respawn
                    };
                    
                    // Subscribe to landing event
                    request.avatar.onLanded += () => {
                        Debug.Log("Avatar has landed after spawning");
                    };
                    
                    // Subscribe to collision events
                    request.avatar.onColliderHit += (hit) => {
                        Debug.Log($"Avatar collided with {hit.collider.name}");
                    };
                }
            });
    }
}
```

## Best Practices

1. Always check the `succeeded` property before accessing the `avatar` or `spaceObject` properties.
2. Use `SetCompletedEvent` for clean callback handling of avatar spawn operations.
3. When working with multiple avatars, keep track of references to manage them effectively.
4. Subscribe to avatar events like `onRespawned`, `onJump`, and `onLanded` to create responsive interactions.
5. Consider using coroutines for complex sequences that involve spawning and configuring multiple avatars.
6. Set descriptive `nametagSubtext` or `displayName` values to distinguish between multiple spawned avatars.
7. Remember that spawned avatars are owned by the local client and can be controlled programmatically.

## Common Use Cases

1. Creating NPC (non-player character) avatars for games or interactive experiences.
2. Spawning placeholder avatars for players who haven't yet joined a session.
3. Implementing avatar-based cinematics or scripted sequences.
4. Building systems that involve multiple avatars with different behaviors.
5. Creating demonstrations or tutorials with avatar examples.
6. Implementing avatar-based puzzle mechanics or challenges.
7. Building avatar control systems for games that involve commanding multiple characters.

## Completed: March 9, 2025