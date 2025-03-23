# SpatialSyncedObject

Category: Core Components

Interface/Class/Enum: Class

The SpatialSyncedObject component enables synchronization of GameObjects across the network in a Spatial environment. This component allows objects to maintain consistent states for all connected users, with properties like position, rotation, and custom variables being synchronized. SpatialSyncedObject provides ownership management, allowing specific users to control an object while ensuring all clients see the same state.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| AssetID | string | The asset ID of this synced object. |
| InstanceID | string | The instance ID of this synced object. |
| destroyOnCreatorDisconnect | bool | If true, the object will be destroyed when the user who created it disconnects. |
| destroyOnOwnerDisconnect | bool | If true, the object will be destroyed when the current owner disconnects. |
| hasControl | bool | Returns true if the local client has control over this synced object. |
| isLocallyOwned | bool | Returns true if this synced object is owned by the local client. |
| isMasterClientObject | bool | Indicates if this object is controlled by the master client. |
| isSynced | bool | Returns true if this synced object is synchronized across clients. |
| obsoleteMessage | string | Message indicating if this component is obsolete. |
| ownerActorID | int | The actor ID of the current owner of this synced object. |
| saveWithSpace | bool | If true, the object will be saved with the space and persist between sessions. |
| syncRigidbody | bool | If true, the Rigidbody component on this object will be synchronized. |
| syncTransform | bool | If true, the Transform component on this object will be synchronized. |
| syncedObjectID | string | The unique ID of this synced object. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |
| documentationURL | string | URL to documentation for this component. |

## Methods

| Method | Description |
| --- | --- |
| TakeoverOwnership() | Takes ownership of this synced object for the local client. |

## Events

| Event | Description |
| --- | --- |
| onObjectInitialized | Event triggered when this synced object is initialized. |
| onOwnerChanged | Event triggered when the owner of this synced object changes. |
| onVariableChanged | Event triggered when a network variable on this synced object changes. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections;

public class InteractiveSyncedObject : MonoBehaviour
{
    // Reference to the SpatialSyncedObject component
    private SpatialSyncedObject syncedObject;
    
    // Object properties
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private Color ownerColor = Color.green;
    [SerializeField] private Color otherColor = Color.red;
    
    // References
    [SerializeField] private Renderer objectRenderer;
    [SerializeField] private Rigidbody objectRigidbody;
    
    // Network variables (these would be defined in a SpatialNetworkVariables component)
    private NetworkVariable<Color> colorVariable;
    private NetworkVariable<bool> isActiveVariable;
    
    private void Awake()
    {
        // Get reference to the SpatialSyncedObject component
        syncedObject = GetComponent<SpatialSyncedObject>();
        
        if (syncedObject == null)
        {
            Debug.LogError("SpatialSyncedObject component not found!");
            return;
        }
        
        // Ensure we have required references
        if (objectRenderer == null)
        {
            objectRenderer = GetComponent<Renderer>();
        }
        
        if (objectRigidbody == null)
        {
            objectRigidbody = GetComponent<Rigidbody>();
        }
    }
    
    private void Start()
    {
        // Set up event listeners
        syncedObject.onObjectInitialized.AddListener(OnObjectInitialized);
        syncedObject.onOwnerChanged.AddListener(OnOwnerChanged);
        syncedObject.onVariableChanged.AddListener(OnVariableChanged);
        
        // Initialize network variables (assuming they are already set up in a SpatialNetworkVariables component)
        InitializeNetworkVariables();
        
        // Update visual state
        UpdateVisualState();
    }
    
    private void Update()
    {
        // Only process input if we own the object
        if (syncedObject.isLocallyOwned)
        {
            HandleOwnerInput();
        }
        
        // Always update visual state (color, etc)
        UpdateVisualState();
    }
    
    private void OnDestroy()
    {
        // Remove event listeners
        if (syncedObject != null)
        {
            syncedObject.onObjectInitialized.RemoveListener(OnObjectInitialized);
            syncedObject.onOwnerChanged.RemoveListener(OnOwnerChanged);
            syncedObject.onVariableChanged.RemoveListener(OnVariableChanged);
        }
        
        // Unsubscribe from network variable events
        if (colorVariable != null)
        {
            colorVariable.OnValueChanged -= OnColorChanged;
        }
        
        if (isActiveVariable != null)
        {
            isActiveVariable.OnValueChanged -= OnActiveStateChanged;
        }
    }
    
    // Initialize network variables
    private void InitializeNetworkVariables()
    {
        // This assumes you have a SpatialNetworkVariables component with these variables defined
        
        // Get reference to color variable
        if (SpatialBridge.networkingService.TryGetNetworkVariable(gameObject, "color", out NetworkVariable<Color> color))
        {
            colorVariable = color;
            colorVariable.OnValueChanged += OnColorChanged;
            
            // Initialize with default color
            if (syncedObject.isLocallyOwned)
            {
                colorVariable.Value = ownerColor;
            }
        }
        
        // Get reference to active state variable
        if (SpatialBridge.networkingService.TryGetNetworkVariable(gameObject, "isActive", out NetworkVariable<bool> active))
        {
            isActiveVariable = active;
            isActiveVariable.OnValueChanged += OnActiveStateChanged;
            
            // Initialize with default state
            if (syncedObject.isLocallyOwned)
            {
                isActiveVariable.Value = true;
            }
        }
    }
    
    // Handle input when we own the object
    private void HandleOwnerInput()
    {
        // Movement example
        if (syncedObject.syncTransform)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            Vector3 movement = new Vector3(horizontal, 0, vertical) * movementSpeed * Time.deltaTime;
            transform.Translate(movement);
        }
        
        // Toggle active state on key press
        if (Input.GetKeyDown(KeyCode.Space) && isActiveVariable != null)
        {
            isActiveVariable.Value = !isActiveVariable.Value;
        }
        
        // Change color on key press
        if (Input.GetKeyDown(KeyCode.C) && colorVariable != null)
        {
            // Generate a random color
            Color randomColor = new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)
            );
            
            colorVariable.Value = randomColor;
        }
    }
    
    // Update the visual state of the object
    private void UpdateVisualState()
    {
        if (objectRenderer != null)
        {
            // Use the synchronized color if available
            if (colorVariable != null)
            {
                objectRenderer.material.color = colorVariable.Value;
            }
            else
            {
                // Otherwise use ownership color
                objectRenderer.material.color = syncedObject.isLocallyOwned ? ownerColor : otherColor;
            }
        }
        
        // Update active state
        if (isActiveVariable != null)
        {
            gameObject.SetActive(isActiveVariable.Value);
        }
    }
    
    // Event handler for object initialization
    private void OnObjectInitialized()
    {
        Debug.Log($"Synced object initialized: {syncedObject.syncedObjectID}");
        
        // Initialize any object-specific state here
    }
    
    // Event handler for owner changes
    private void OnOwnerChanged()
    {
        Debug.Log($"Synced object owner changed to: {syncedObject.ownerActorID}");
        
        // Update visual state or behavior based on ownership
        if (syncedObject.isLocallyOwned)
        {
            Debug.Log("You now have ownership of this object");
        }
        else
        {
            Debug.Log("You no longer have ownership of this object");
        }
    }
    
    // Event handler for variable changes
    private void OnVariableChanged()
    {
        Debug.Log("A network variable has changed on this object");
        
        // This is a general event for any variable change
        // For specific variables, use the OnValueChanged event on the variable itself
    }
    
    // Color variable change handler
    private void OnColorChanged(Color oldColor, Color newColor)
    {
        Debug.Log($"Color changed from {oldColor} to {newColor}");
        
        // You could add a visual effect here for the color change
    }
    
    // Active state variable change handler
    private void OnActiveStateChanged(bool oldValue, bool newValue)
    {
        Debug.Log($"Active state changed from {oldValue} to {newValue}");
        
        // You could add an effect when the object is activated/deactivated
        if (newValue)
        {
            // Play activation effect
            StartCoroutine(PlayActivationEffect());
        }
    }
    
    // Example of a visual effect when the object is activated
    private IEnumerator PlayActivationEffect()
    {
        if (objectRenderer == null)
            yield break;
            
        // Store original color
        Color originalColor = objectRenderer.material.color;
        
        // Flash white briefly
        objectRenderer.material.color = Color.white;
        
        // Wait a short time
        yield return new WaitForSeconds(0.2f);
        
        // Restore original color
        objectRenderer.material.color = originalColor;
    }
    
    // Public method to request ownership of this object
    public void RequestOwnership()
    {
        if (!syncedObject.isLocallyOwned)
        {
            Debug.Log("Requesting ownership of synced object");
            syncedObject.TakeoverOwnership();
        }
        else
        {
            Debug.Log("You already own this object");
        }
    }
    
    // Create a synced object programmatically
    public static GameObject CreateSyncedObject(Vector3 position, bool saveWithSpace = false)
    {
        // Create a new GameObject
        GameObject newObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newObject.transform.position = position;
        newObject.name = "SyncedObject_" + System.Guid.NewGuid().ToString().Substring(0, 8);
        
        // Add a SpatialSyncedObject component
        SpatialSyncedObject syncedObject = newObject.AddComponent<SpatialSyncedObject>();
        syncedObject.syncTransform = true;
        syncedObject.syncRigidbody = true;
        syncedObject.saveWithSpace = saveWithSpace;
        
        // Add a Rigidbody if we're synchronizing it
        if (syncedObject.syncRigidbody)
        {
            Rigidbody rb = newObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.mass = 1.0f;
        }
        
        // Add a SpatialNetworkVariables component for custom variables
        SpatialNetworkVariables networkVars = newObject.AddComponent<SpatialNetworkVariables>();
        
        // Add our example component
        InteractiveSyncedObject interactive = newObject.AddComponent<InteractiveSyncedObject>();
        
        Debug.Log($"Created new synced object at position {position}");
        return newObject;
    }
}
```

## Best Practices

1. Determine which components need to be synchronized (transform, rigidbody) and enable only the necessary sync flags to reduce network traffic.
2. Properly manage ownership of synced objects, especially for interactive elements that multiple users might want to control.
3. Use the destroyOnCreatorDisconnect and destroyOnOwnerDisconnect properties appropriately based on your object's purpose.
4. Consider the saveWithSpace flag for persistent objects that should remain in the space between sessions.
5. Subscribe to the onObjectInitialized, onOwnerChanged, and onVariableChanged events to respond appropriately to state changes.
6. When synchronizing a Rigidbody, be aware of the performance implications and consider using kinematic rigidbodies for complex movements.
7. Use network variables through SpatialNetworkVariables for custom synchronized properties beyond position and rotation.
8. Implement visual feedback to indicate ownership or interaction capabilities to users.
9. Avoid making frequent ownership changes as this creates additional network traffic.
10. For objects that need to be manipulated by multiple users, consider implementing a request-release ownership system instead of direct ownership takeover.

## Common Use Cases

1. Interactive objects like buttons, levers, or controls that can be manipulated by users.
2. Movable furniture or props that users can reposition within the space.
3. Shared whiteboards or collaborative tools with synchronized content.
4. Physics-based games or activities where object positions and velocities need to remain consistent.
5. Door or mechanism states that should be synchronized across all clients.
6. Collectible or inventory items in multi-user games.
7. User-created content that persists in the space across sessions.
8. Dynamic environmental features like weather effects or time-of-day changes.
9. Shared vehicles or transportation that multiple users can interact with.
10. Game state objects that track scores, progress, or other information for all users.

## Completed: March 10, 2025