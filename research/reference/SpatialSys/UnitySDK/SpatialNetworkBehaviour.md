## Overview
SpatialNetworkBehaviour is a component that allows you to define custom behaviors for network objects in Spatial. It must always be paired with a SpatialNetworkObject component, which can be on the same GameObject or on one of its parents. This class serves as the foundation for creating multiplayer gameplay elements and networked interactions in Spatial experiences.

## Properties

| Property | Description |
|----------|-------------|
| hasControl | Returns true if the local client can control this network behavior. |
| isMine | Returns true if the local client is the owner of this network behavior. |
| networkObject | Reference to the SpatialNetworkObject component this behavior is associated with. |
| objectID | The unique identifier for this network object. |
| ownerActorNumber | The actor number of the owner of this network behavior. |

## Methods

| Method | Description |
|--------|-------------|
| Spawned() | Called when the network object is spawned. Use this method instead of Start. Accessing and modifying the network object's state is only allowed after this method is called. |
| Despawned() | Called before the network object is despawned. Use this method instead of OnDestroy. After this method is called, the network object's state can no longer be modified, and the underlying space object will be marked as disposed. |

## Usage Example

### Basic Network Behavior

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class NetworkedCube : SpatialNetworkBehaviour
{
    [SerializeField] private Material ownedMaterial;
    [SerializeField] private Material otherMaterial;
    
    private Renderer cubeRenderer;
    
    // Use Spawned instead of Start for initialization
    public override void Spawned()
    {
        // Initialize components
        cubeRenderer = GetComponent<Renderer>();
        
        // Set appearance based on ownership
        UpdateVisuals();
        
        // Only the owner should handle input
        if (isMine)
        {
            Debug.Log($"I am the owner of this cube (Actor: {ownerActorNumber})");
        }
    }
    
    // Use Despawned instead of OnDestroy for cleanup
    public override void Despawned()
    {
        Debug.Log("Cube is being despawned");
        // Perform any necessary cleanup here
    }
    
    private void Update()
    {
        // Only the owner can move the cube
        if (isMine)
        {
            // Handle input and movement
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * Time.deltaTime);
            }
        }
    }
    
    private void UpdateVisuals()
    {
        // Change appearance based on ownership
        cubeRenderer.material = isMine ? ownedMaterial : otherMaterial;
    }
}
```

### Interactable Network Object

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

[RequireComponent(typeof(Collider))]
public class NetworkedButton : SpatialNetworkBehaviour
{
    // Custom events to synchronize button state
    public SpatialNetworkEvent onButtonPressed = new SpatialNetworkEvent(false);
    
    [SerializeField] private Transform buttonTop;
    [SerializeField] private float pressDistance = 0.05f;
    [SerializeField] private float resetTime = 1.0f;
    
    private Vector3 startPosition;
    private bool isPressed = false;
    private float pressTimer = 0;
    
    public override void Spawned()
    {
        // Store the initial position
        startPosition = buttonTop.localPosition;
        
        // Listen for the button press event
        onButtonPressed.AddListener(OnButtonPressedEvent);
    }
    
    public override void Despawned()
    {
        // Clean up event listeners
        onButtonPressed.RemoveListener(OnButtonPressedEvent);
    }
    
    private void Update()
    {
        // Reset the button after the timer expires
        if (isPressed)
        {
            pressTimer += Time.deltaTime;
            
            if (pressTimer >= resetTime)
            {
                ResetButton();
            }
        }
    }
    
    // Called when a player clicks on the button
    private void OnMouseDown()
    {
        // Only process clicks for clients with control
        if (hasControl && !isPressed)
        {
            // Notify all clients about the button press
            onButtonPressed.Invoke();
        }
    }
    
    // This will be called on all clients when the button is pressed
    private void OnButtonPressedEvent()
    {
        // Visually press the button
        buttonTop.localPosition = startPosition - new Vector3(0, pressDistance, 0);
        isPressed = true;
        pressTimer = 0;
        
        // Play effects, trigger game logic, etc.
        Debug.Log("Button was pressed!");
    }
    
    private void ResetButton()
    {
        // Reset the button state
        buttonTop.localPosition = startPosition;
        isPressed = false;
    }
}
```

## Best Practices

- Always use Spawned() and Despawned() instead of Start() and OnDestroy() for network-related initialization and cleanup.
- Check isMine or hasControl before modifying network object properties or handling input.
- Keep network state changes minimal and optimized to reduce bandwidth usage.
- Use SpatialNetworkEvent, SpatialNetworkProperty, and similar network-aware components for synchronizing state across clients.
- Remember that your network behavior must be attached to a GameObject that also has a SpatialNetworkObject component (either directly or on a parent).
- For objects that many players will interact with, consider using hasControl checks rather than isMine to allow for temporary control transfers.
- Use appropriate authority checks to prevent unauthorized state modifications.

## Common Use Cases

- Interactive objects like buttons, levers, and doors
- Player-owned items and tools
- Vehicles and movable platforms
- Gameplay elements like collectibles and power-ups
- Multiplayer game mechanics like scoreboards and timers
- Synchronized animations and effects
- Team-based objects and territory controls

## Network Authority Handling

Understanding the different authority checks is crucial for proper network behavior:

- **isMine**: True only for the client who owns the object. Use for persistent player-owned objects.
- **hasControl**: True for the client that currently has control (which may differ from ownership). Use for temporarily controllable objects.
- **ownerActorNumber**: The actor number of the owner, useful for identifying who owns an object.

Different use cases may require different authority models:

```csharp
// Player-owned persistent item (ownership-based)
if (isMine)
{
    // Only the owner can use their personal item
    UsePersonalItem();
}

// Temporarily controllable object (control-based)
if (hasControl)
{
    // Any player with temporary control can operate this door
    OperateDoor();
}

// Authority verification
if (ownerActorNumber == SpatialBridge.actorService.localActor.actorNumber)
{
    // Double-check that we are the owner
    ModifyImportantState();
}
```

## Completed: March 10, 2025
