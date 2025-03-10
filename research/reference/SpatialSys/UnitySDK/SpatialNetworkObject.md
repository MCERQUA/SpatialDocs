# SpatialNetworkObject

Category: Core Components

Interface/Class/Enum: Class

This component enables a GameObject to be synchronized across the network. A network object handles synchronizing the transform, rigidbody, and custom variables defined through visual scripting or NetworkVariables within SpatialNetworkBehaviour.

Network objects can be either embedded in a scene or instantiated from a prefab.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| behaviours | List<SpatialNetworkBehaviour> | Collection of SpatialNetworkBehaviour components attached to this object. |
| hasControl | bool | Returns true if the local client has control over this object. |
| isMine | bool | Returns true if the local client owns this object. |
| nestedObjects | List<SpatialNetworkObject> | Collection of child SpatialNetworkObject components. |
| networkPrefabGuid | string | Unique ID for the prefab that this NetworkObject is attached to. This is used to identify which prefab to instantiate on other clients when this object is spawned. Only assigned to the root NetworkObject in a prefab. |
| objectFlags | SpaceObjectFlags | Flags that determine special handling of the network object. |
| objectID | int | Unique identifier for this network object within the space. |
| ownerActorNumber | int | Actor number of the current owner of this object. |
| parentObject | SpatialNetworkObject | Reference to the parent network object, if this is a nested network object. |
| prettyName | string | Display name shown in the Unity Inspector. |
| rootObject | SpatialNetworkObject | Reference to the root network object in the hierarchy. |
| sceneObjectGuid | string | Unique ID for NetworkObjects embedded in a scene. Guaranteed to be unique within the scene. |
| spaceObject | ISpaceObject | Interface to the underlying space object. |
| syncFlags | SyncFlags | Flags determining which components to synchronize (transform, rigidbody, etc.). |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |
| documentationURL | string | URL to documentation for this component. |

## Methods

| Method | Description |
| --- | --- |
| RequestOwnership() | Requests ownership of this network object. Returns a SpatialAsyncOperation that can be used to check if the request was successful. |
| ReleaseOwnership() | Releases ownership of this network object, making it owned by the server. |
| TransferOwnership(int actorNumber) | Transfers ownership of this network object to another actor. |
| TryFindObject(int objectId, out SpatialNetworkObject obj) | Static method to try to find a network object by its ID. Returns true if found. |

## Events

| Event | Description |
| --- | --- |
| onDestroy | Event triggered when this network object is destroyed. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections;

public class NetworkObjectExample : MonoBehaviour
{
    // Reference to the SpatialNetworkObject component
    private SpatialNetworkObject networkObject;
    
    // Color materials for visual feedback
    [SerializeField] private Material ownedMaterial;
    [SerializeField] private Material unownedMaterial;
    [SerializeField] private Material requestingMaterial;
    
    // Reference to the renderer for visual updates
    private Renderer objectRenderer;
    
    private void Awake()
    {
        // Get references
        networkObject = GetComponent<SpatialNetworkObject>();
        objectRenderer = GetComponent<Renderer>();
        
        if (networkObject == null)
        {
            Debug.LogError("SpatialNetworkObject component is missing!");
            return;
        }
    }
    
    private void Start()
    {
        // Subscribe to destruction event
        networkObject.onDestroy += OnNetworkObjectDestroyed;
        
        // Update visual state
        UpdateVisualState();
    }
    
    private void Update()
    {
        // Update visual state based on ownership
        UpdateVisualState();
        
        // Example: Request ownership when pressing E key
        if (Input.GetKeyDown(KeyCode.E) && !networkObject.isMine)
        {
            RequestObjectOwnership();
        }
        
        // Example: Release ownership when pressing R key
        if (Input.GetKeyDown(KeyCode.R) && networkObject.isMine)
        {
            ReleaseObjectOwnership();
        }
        
        // Show object information when pressing I key
        if (Input.GetKeyDown(KeyCode.I))
        {
            DisplayObjectInfo();
        }
        
        // Example: If we own the object, we can move it
        if (networkObject.isMine)
        {
            // Simple movement example
            float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * 3f;
            float vertical = Input.GetAxis("Vertical") * Time.deltaTime * 3f;
            
            transform.Translate(new Vector3(horizontal, 0, vertical));
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (networkObject != null)
        {
            networkObject.onDestroy -= OnNetworkObjectDestroyed;
        }
    }
    
    private void UpdateVisualState()
    {
        if (objectRenderer == null || ownedMaterial == null || unownedMaterial == null)
            return;
            
        // Change material based on ownership
        objectRenderer.material = networkObject.isMine ? ownedMaterial : unownedMaterial;
    }
    
    public void RequestObjectOwnership()
    {
        Debug.Log("Requesting ownership of network object...");
        
        // Change to requesting material if available
        if (requestingMaterial != null && objectRenderer != null)
        {
            objectRenderer.material = requestingMaterial;
        }
        
        // Request ownership
        var request = networkObject.RequestOwnership();
        StartCoroutine(WaitForOwnershipRequest(request));
    }
    
    private IEnumerator WaitForOwnershipRequest(SpatialAsyncOperation request)
    {
        // Wait for the request to complete
        yield return request;
        
        if (networkObject.isMine)
        {
            Debug.Log("Successfully acquired ownership!");
        }
        else
        {
            Debug.Log("Failed to acquire ownership.");
        }
        
        // Update visual state
        UpdateVisualState();
    }
    
    public void ReleaseObjectOwnership()
    {
        if (!networkObject.isMine)
        {
            Debug.Log("Cannot release ownership - we don't own this object.");
            return;
        }
        
        Debug.Log("Releasing ownership of network object...");
        networkObject.ReleaseOwnership();
        
        // Update visual state
        UpdateVisualState();
    }
    
    public void TransferObjectOwnership(int targetActorNumber)
    {
        if (!networkObject.isMine)
        {
            Debug.Log("Cannot transfer ownership - we don't own this object.");
            return;
        }
        
        Debug.Log($"Transferring ownership to actor {targetActorNumber}...");
        networkObject.TransferOwnership(targetActorNumber);
    }
    
    private void OnNetworkObjectDestroyed()
    {
        Debug.Log("Network object is being destroyed.");
    }
    
    private void DisplayObjectInfo()
    {
        if (networkObject == null)
            return;
            
        string ownerInfo = networkObject.isMine ? "You own this object." : $"Owned by actor {networkObject.ownerActorNumber}";
        
        Debug.Log($"Network Object Info:\n" +
                  $"Object ID: {networkObject.objectID}\n" +
                  $"Ownership: {ownerInfo}\n" +
                  $"Has Control: {networkObject.hasControl}\n" +
                  $"Prefab GUID: {networkObject.networkPrefabGuid}\n" +
                  $"Scene GUID: {networkObject.sceneObjectGuid}");
    }
    
    // Example of finding a network object by ID
    public static SpatialNetworkObject FindNetworkObject(int objectId)
    {
        SpatialNetworkObject result;
        if (SpatialNetworkObject.TryFindObject(objectId, out result))
        {
            return result;
        }
        
        Debug.LogWarning($"Could not find network object with ID {objectId}");
        return null;
    }
}
```

## Best Practices

1. Use SpatialBridge.spaceContentService.Spawn to spawn a network object for all clients, rather than GameObject.Instantiate which only creates a local instance.
2. Use Object.Destroy or SpatialBridge.spaceContentService.DestroySpaceObject to despawn a network object for all clients.
3. Set the appropriate syncFlags to avoid unnecessarily synchronizing components that don't need to be networked.
4. Request ownership only when needed, and release it when done to avoid performance overhead.
5. Consider the network implications when modifying objects owned by others - generally, you should only modify objects you own.
6. Properly handle ownership changes to ensure consistent behavior across all clients.
7. Use SpatialNetworkObject.TryFindObject when you need to reference network objects by ID.
8. Be cautious when nesting network objects, as parent-child relationships can affect synchronization behavior.
9. Understand the difference between ownership (who can modify the object) and control (who can interact with the object).
10. For objects that need frequent ownership changes, consider setting up a proper ownership request and transfer workflow.

## Common Use Cases

1. Interactive objects that players can pick up, move, or manipulate.
2. Dynamically spawned game elements like projectiles, power-ups, or obstacles.
3. Player-created structures or placed objects in building games.
4. Shared tools or resources in collaborative experiences.
5. Vehicles that can be controlled by different players.
6. Doors, switches, or other interactive environment elements.
7. Game state objects that track scores, progress, or other game information.
8. Synchronized puzzle elements in multiplayer puzzle games.
9. Dynamic environment changes like destructible objects or terrain modifications.
10. Physics-based objects that need to maintain consistent behavior across all clients.

## Completed: March 10, 2025