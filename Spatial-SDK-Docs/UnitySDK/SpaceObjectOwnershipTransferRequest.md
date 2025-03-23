# SpaceObjectOwnershipTransferRequest

Category: Space Content Service

Class

Represents the result of a request to transfer ownership of a space object from one actor to another. This class inherits from SpatialAsyncOperation and provides information about the ownership transfer operation.

## Properties

| Property | Description |
| --- | --- |
| newOwnerActor | The actor that is receiving ownership of the space object. |
| oldOwnerActor | The actor that previously owned the space object. |
| spaceObjectID | The ID of the space object whose ownership is being transferred. |
| succeeded | Whether the ownership transfer was successful. |

## Inherited Members

| Member | Description |
| --- | --- |
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

public class OwnershipManager : MonoBehaviour
{
    // Example: Taking ownership of a space object
    public void TakeOwnershipOfObject(ISpaceObject spaceObject)
    {
        if (!spaceObject.isMine && spaceObject.canTakeOwnership)
        {
            Debug.Log($"Attempting to take ownership of space object #{spaceObject.objectID}");
            
            SpatialBridge.spaceContentService.TakeOwnership(spaceObject)
                .SetCompletedEvent(HandleOwnershipTransferResult);
        }
        else if (spaceObject.isMine)
        {
            Debug.Log($"Already own space object #{spaceObject.objectID}");
        }
        else
        {
            Debug.LogWarning($"Cannot take ownership of space object #{spaceObject.objectID}");
        }
    }
    
    // Example: Giving ownership to another actor
    public void GiveOwnershipToActor(ISpaceObject spaceObject, int actorNumber)
    {
        if (spaceObject.isMine)
        {
            Debug.Log($"Attempting to give ownership of space object #{spaceObject.objectID} to actor #{actorNumber}");
            
            SpatialBridge.spaceContentService.GiveOwnership(spaceObject, actorNumber)
                .SetCompletedEvent(HandleOwnershipTransferResult);
        }
        else
        {
            Debug.LogWarning($"Cannot give ownership of space object #{spaceObject.objectID} - not the owner");
        }
    }
    
    // Example: Using in a coroutine
    public IEnumerator TakeOwnershipAndModify(ISpaceObject spaceObject)
    {
        if (!spaceObject.isMine && spaceObject.canTakeOwnership)
        {
            Debug.Log($"Attempting to take ownership of space object #{spaceObject.objectID}");
            
            var request = SpatialBridge.spaceContentService.TakeOwnership(spaceObject);
            yield return request; // Wait for the operation to complete
            
            if (request.succeeded)
            {
                Debug.Log($"Successfully took ownership from actor #{request.oldOwnerActor.actorNumber}");
                
                // Now we can modify the object since we have ownership
                spaceObject.position = new Vector3(0, 1, 0);
                spaceObject.rotation = Quaternion.identity;
                
                // Optionally give ownership back when done
                yield return new WaitForSeconds(5);
                
                // Give ownership back to original owner if they're still connected
                if (request.oldOwnerActor != null && !request.oldOwnerActor.isDisposed)
                {
                    SpatialBridge.spaceContentService.GiveOwnership(spaceObject, request.oldOwnerActor.actorNumber);
                }
            }
            else
            {
                Debug.LogWarning($"Failed to take ownership of space object #{spaceObject.objectID}");
            }
        }
    }
    
    // Handler for ownership transfer completion
    private void HandleOwnershipTransferResult(SpaceObjectOwnershipTransferRequest request)
    {
        if (request.succeeded)
        {
            Debug.Log($"Ownership transfer successful for space object #{request.spaceObjectID}");
            Debug.Log($"Previous owner: Actor #{request.oldOwnerActor.actorNumber}");
            Debug.Log($"New owner: Actor #{request.newOwnerActor.actorNumber}");
            
            // Check if we are the new owner
            if (request.newOwnerActor.actorNumber == SpatialBridge.actorService.localActor.actorNumber)
            {
                Debug.Log("We now own this object!");
                
                // Now we can modify the object
                var spaceObject = SpatialBridge.spaceContentService.GetSpaceObject(request.spaceObjectID);
                if (spaceObject != null && !spaceObject.isDisposed)
                {
                    // Make modifications to the space object
                    // Example: Set a variable
                    spaceObject.SetVariable(0, "Claimed by new owner");
                }
            }
        }
        else
        {
            Debug.LogWarning($"Ownership transfer failed for space object #{request.spaceObjectID}");
        }
    }
    
    // Example: Creating a system to temporarily take ownership
    public void TemporarilyTakeOwnership(ISpaceObject spaceObject, System.Action onComplete)
    {
        if (!spaceObject.isMine && spaceObject.canTakeOwnership)
        {
            StartCoroutine(TemporaryOwnershipRoutine(spaceObject, onComplete));
        }
        else if (spaceObject.isMine)
        {
            // Already own it, just execute the callback
            onComplete?.Invoke();
        }
    }
    
    private IEnumerator TemporaryOwnershipRoutine(ISpaceObject spaceObject, System.Action onComplete)
    {
        int originalOwnerActorNumber = spaceObject.ownerActorNumber;
        bool shouldReturnOwnership = originalOwnerActorNumber != SpatialBridge.actorService.localActor.actorNumber;
        
        // Take ownership
        var takeRequest = SpatialBridge.spaceContentService.TakeOwnership(spaceObject);
        yield return takeRequest;
        
        if (takeRequest.succeeded)
        {
            // We now have ownership
            Debug.Log($"Temporarily took ownership of space object #{spaceObject.objectID}");
            
            // Execute the callback while we have ownership
            onComplete?.Invoke();
            
            // Wait a bit to ensure any changes are processed
            yield return new WaitForSeconds(0.2f);
            
            // Return ownership if needed
            if (shouldReturnOwnership && !takeRequest.oldOwnerActor.isDisposed)
            {
                Debug.Log($"Returning ownership of space object #{spaceObject.objectID} to actor #{originalOwnerActorNumber}");
                
                var giveRequest = SpatialBridge.spaceContentService.GiveOwnership(spaceObject, originalOwnerActorNumber);
                yield return giveRequest;
                
                if (giveRequest.succeeded)
                {
                    Debug.Log("Successfully returned ownership");
                }
                else
                {
                    Debug.LogWarning("Failed to return ownership");
                }
            }
        }
        else
        {
            Debug.LogWarning($"Failed to take temporary ownership of space object #{spaceObject.objectID}");
        }
    }
}
```

## Best Practices

1. Always check the `succeeded` property before assuming the ownership transfer was successful.
2. Verify that actors referenced in `newOwnerActor` and `oldOwnerActor` are not disposed before using them.
3. Use `SetCompletedEvent` for a cleaner callback approach when handling the result of ownership transfer operations.
4. Remember that you can only modify a space object when you have ownership (when `spaceObject.isMine` is true).
5. Consider temporary ownership patterns where you take ownership, make changes, and then return ownership when done.
6. Always check `canTakeOwnership` before attempting to take ownership of a space object that doesn't belong to you.
7. Implement proper error handling for cases where ownership transfer fails.

## Common Use Cases

1. Taking ownership of space objects to modify their properties or variables.
2. Implementing turn-based systems where control passes between different actors.
3. Creating collaborative editing tools where multiple users can take control of objects.
4. Building systems where a single controller manages multiple objects temporarily.
5. Implementing "cleanup" operations where a server or designated client takes control of abandoned objects.
6. Creating ownership delegation systems for team-based activities.
7. Implementing authority transfer patterns for physics-based interactions.

## Completed: March 9, 2025