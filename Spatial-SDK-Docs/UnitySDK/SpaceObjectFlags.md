## Overview
The SpaceObjectFlags enum defines flags for controlling the behavior and lifecycle of objects within a Spatial space. These flags determine how objects should be handled in multiplayer contexts, particularly regarding ownership and destruction events.

## Properties
- **None**: Default state with no special flags.
- **MasterClientObject**: Flag indicating that the object should always be owned by the master client.
- **DestroyWhenOwnerLeaves**: Flag indicating that the object should be destroyed when the owner leaves the space.
- **DestroyWhenCreatorLeaves**: Flag indicating that the object should be destroyed when the creator of the object leaves the space.
- **AllowOwnershipTransfer**: Flag indicating that the object can be transferred to another client.

## Usage Example
```csharp
using SpatialSys.UnitySDK;

// Creating a space object with flags for multiplayer behavior
public void CreateTemporaryObject(GameObject prefab, Vector3 position)
{
    // Create an object that will be destroyed when the creator leaves
    SpaceObject spaceObj = SpatialBridge.spaceContentService.CreateSpaceObject(
        prefab,
        position,
        Quaternion.identity,
        SpaceObjectFlags.DestroyWhenCreatorLeaves
    );
    
    Debug.Log("Created temporary object that will be removed when you leave");
}

// Creating a persistent object that the master client controls
public void CreatePersistentObject(GameObject prefab, Vector3 position)
{
    // Create an object that will always be controlled by the master client
    SpaceObject spaceObj = SpatialBridge.spaceContentService.CreateSpaceObject(
        prefab,
        position,
        Quaternion.identity,
        SpaceObjectFlags.MasterClientObject
    );
    
    Debug.Log("Created persistent master-controlled object");
}

// Creating an object that can change ownership
public void CreateTransferableObject(GameObject prefab, Vector3 position)
{
    // Create an object that allows ownership transfer
    SpaceObject spaceObj = SpatialBridge.spaceContentService.CreateSpaceObject(
        prefab,
        position,
        Quaternion.identity,
        SpaceObjectFlags.AllowOwnershipTransfer
    );
    
    Debug.Log("Created object that can be transferred between clients");
}
```

## Best Practices
- Use **MasterClientObject** for objects that need to persist and maintain consistent state across all clients, like score counters or game managers
- Use **DestroyWhenOwnerLeaves** for temporary objects that should only exist while a specific player is present
- Use **DestroyWhenCreatorLeaves** for objects created by a player that should be removed when that player exits
- Use **AllowOwnershipTransfer** when creating objects that need to be passed between different clients
- Consider combining flags with bitwise operations when multiple behaviors are needed (e.g., `SpaceObjectFlags.AllowOwnershipTransfer | SpaceObjectFlags.DestroyWhenOwnerLeaves`)
- For objects that must remain in the space regardless of player presence, use **None** or **MasterClientObject**

## Common Use Cases
- Controlling the lifecycle of player-created objects in multiplayer spaces
- Managing ownership of interactive objects that can be controlled by different users
- Creating persistent objects that survive player disconnections
- Implementing temporary objects that are automatically cleaned up when no longer needed
- Building multiplayer games with proper object ownership and authority systems
- Creating shared experiences where objects can be passed between users

## Completed: March 10, 2025
