## Overview
Interface for listening to avatar input capture events. This interface is used to receive input events when capturing avatar controls through the Input Service. Only events that have been overridden with `StartAvatarInputCapture` will be triggered.

## Methods

| Method | Description |
| --- | --- |
| OnAvatarMoveInput(InputPhase, Vector2) | Called when the user moves the avatar. The Vector2 parameter represents the movement direction and magnitude. |
| OnAvatarJumpInput(InputPhase) | Called when jump input has been captured. |
| OnAvatarSprintInput(InputPhase) | Called when sprint input has been captured. |
| OnAvatarActionInput(InputPhase) | Called when action input has been captured. |
| OnAvatarAutoSprintToggled(bool) | Called when auto sprint has been toggled on. |

## Inherited Members
As this interface inherits from IInputActionsListener, it also includes these methods:

| Method | Description |
| --- | --- |
| OnInputCaptureStarted(InputCaptureType) | Called when input capture has started. |
| OnInputCaptureStopped(InputCaptureType) | Called when input capture has stopped. |

## Usage Example

```csharp
using System;
using SpatialSys.UnitySDK;
using UnityEngine;

public class CustomAvatarController : MonoBehaviour, IAvatarInputActionsListener
{
    private Vector2 lastMoveInput;
    private bool isJumping;
    private bool isSprinting;
    private bool isActionActive;
    private bool isAutoSprinting;

    private void Start()
    {
        // Start capturing avatar input with this listener
        SpatialBridge.inputService.StartAvatarInputCapture(
            true,   // Override movement
            true,   // Override jump
            true,   // Override sprint
            false,  // Don't override crouch
            this    // This object as the listener
        );
    }

    private void OnDestroy()
    {
        // Make sure to release input capture when this object is destroyed
        try
        {
            SpatialBridge.inputService.ReleaseInputCapture(this);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error releasing input capture: {e.Message}");
        }
    }

    // Implementation of IAvatarInputActionsListener methods
    public void OnAvatarMoveInput(InputPhase phase, Vector2 input)
    {
        lastMoveInput = input;
        
        Debug.Log($"Move input: {input}, Phase: {phase}");
        
        // Apply movement to your custom avatar or game object
        if (phase != InputPhase.OnReleased)
        {
            // Example: Move a character controller
            Vector3 moveDirection = new Vector3(input.x, 0, input.y);
            // Apply movement here
        }
    }

    public void OnAvatarJumpInput(InputPhase phase)
    {
        if (phase == InputPhase.OnPressed)
        {
            isJumping = true;
            Debug.Log("Jump started");
            // Implement jump logic
        }
        else if (phase == InputPhase.OnReleased)
        {
            isJumping = false;
            Debug.Log("Jump released");
        }
    }

    public void OnAvatarSprintInput(InputPhase phase)
    {
        if (phase == InputPhase.OnPressed)
        {
            isSprinting = true;
            Debug.Log("Sprint started");
            // Implement sprint logic
        }
        else if (phase == InputPhase.OnReleased)
        {
            isSprinting = false;
            Debug.Log("Sprint released");
        }
    }

    public void OnAvatarActionInput(InputPhase phase)
    {
        if (phase == InputPhase.OnPressed)
        {
            isActionActive = true;
            Debug.Log("Action started");
            // Implement action logic (e.g., interact with objects)
        }
        else if (phase == InputPhase.OnReleased)
        {
            isActionActive = false;
            Debug.Log("Action released");
        }
    }

    public void OnAvatarAutoSprintToggled(bool enabled)
    {
        isAutoSprinting = enabled;
        Debug.Log($"Auto sprint toggled: {enabled}");
        // Implement auto sprint logic
    }

    // Implementation of IInputActionsListener methods
    public void OnInputCaptureStarted(InputCaptureType captureType)
    {
        Debug.Log($"Input capture started: {captureType}");
    }

    public void OnInputCaptureStopped(InputCaptureType captureType)
    {
        Debug.Log($"Input capture stopped: {captureType}");
        
        // Reset all input states
        lastMoveInput = Vector2.zero;
        isJumping = false;
        isSprinting = false;
        isActionActive = false;
    }

    // Use the captured input in Update or FixedUpdate
    private void Update()
    {
        // Example of using the captured input values
        if (lastMoveInput.magnitude > 0.1f)
        {
            float moveSpeed = isSprinting || isAutoSprinting ? 5f : 3f;
            Vector3 movement = new Vector3(lastMoveInput.x, 0, lastMoveInput.y) * moveSpeed * Time.deltaTime;
            transform.position += movement;
        }

        if (isJumping)
        {
            // Implement jumping behavior
        }

        if (isActionActive)
        {
            // Implement action behavior
        }
    }
}
```

## Best Practices

1. Always release input capture when your component is disabled or destroyed to avoid input conflicts.
2. Implement all required interface methods, even if some are not used.
3. Consider the current InputPhase in your implementations to properly handle pressed, held, and released states.
4. Use the OnInputCaptureStarted and OnInputCaptureStopped methods to initialize and clean up any state related to input.
5. Be aware that other systems might also request input capture, so handle cases where your input might be overridden.
6. Cache input values for use in Update or FixedUpdate rather than applying movement directly in the callback methods.
7. Check for null references before accessing SpatialBridge.inputService to avoid errors, especially during scene transitions.

## Common Use Cases

1. **Custom Character Controllers**: Implement unique movement systems or character behaviors beyond the default avatar controller.
2. **Game Mechanics**: Use avatar input for gameplay mechanics like platformer controls or action game inputs.
3. **Vehicle Entry/Exit Systems**: Capture avatar input to determine when a player enters or exits vehicles.
4. **Input Debugging**: Create monitoring systems to track and debug player inputs during testing.
5. **Tutorial Systems**: Create guided experiences that track user inputs to advance tutorial steps.
6. **Custom Input Visualizations**: Display on-screen indicators showing the current input state.
7. **Accessibility Features**: Modify or enhance controls to improve accessibility for different players.
8. **Input Recording/Playback**: Record player inputs for replays or automated testing.
