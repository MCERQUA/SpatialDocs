## Overview
Enumeration that defines the phase of an input event. InputPhase is used in input event callbacks to indicate whether an input is being pressed, held, or released. This allows for precise tracking of input states across multiple frames.

## Properties

| Property | Description |
| --- | --- |
| OnPressed | Input was pressed. This occurs at the moment when input is first detected. |
| OnHold | Input is being held. This occurs on frames where the input continues to be active after the initial press. |
| OnReleased | Input was released. This occurs at the moment when input that was previously active stops being detected. |

## Usage Example

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections.Generic;

public class InputPhaseTracker : MonoBehaviour, IAvatarInputActionsListener
{
    // Track the current state of input actions
    private Dictionary<string, InputPhase> inputStates = new Dictionary<string, InputPhase>();
    
    // Track input durations
    private Dictionary<string, float> inputStartTimes = new Dictionary<string, float>();
    
    private void Start()
    {
        // Register as an avatar input listener
        SpatialBridge.inputService.StartAvatarInputCapture(
            true,   // Override movement
            true,   // Override jump
            true,   // Override sprint
            false,  // Don't override crouch
            this
        );
    }
    
    private void OnDestroy()
    {
        // Clean up
        try
        {
            SpatialBridge.inputService.ReleaseInputCapture(this);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error releasing input capture: {e.Message}");
        }
    }
    
    // Implement IAvatarInputActionsListener methods
    public void OnAvatarJumpInput(InputPhase phase)
    {
        HandleInputPhase("Jump", phase);
        
        // Example of phase-specific handling
        switch (phase)
        {
            case InputPhase.OnPressed:
                // Start jump animation or physics
                Debug.Log("Jump started");
                break;
                
            case InputPhase.OnHold:
                // Potentially modify jump height based on hold duration
                float holdDuration = Time.time - inputStartTimes["Jump"];
                Debug.Log($"Jump held for {holdDuration:F2} seconds");
                break;
                
            case InputPhase.OnReleased:
                // Complete jump action
                float totalDuration = Time.time - inputStartTimes["Jump"];
                Debug.Log($"Jump released after {totalDuration:F2} seconds");
                break;
        }
    }
    
    public void OnAvatarSprintInput(InputPhase phase)
    {
        HandleInputPhase("Sprint", phase);
        
        // Example of phase-specific handling
        switch (phase)
        {
            case InputPhase.OnPressed:
                // Start sprint animation or increase movement speed
                Debug.Log("Sprint started");
                break;
                
            case InputPhase.OnHold:
                // Continue sprint, potentially manage stamina
                float holdDuration = Time.time - inputStartTimes["Sprint"];
                Debug.Log($"Sprinting for {holdDuration:F2} seconds");
                break;
                
            case InputPhase.OnReleased:
                // End sprint animation or decrease movement speed
                float totalDuration = Time.time - inputStartTimes["Sprint"];
                Debug.Log($"Sprint ended after {totalDuration:F2} seconds");
                break;
        }
    }
    
    public void OnAvatarActionInput(InputPhase phase)
    {
        HandleInputPhase("Action", phase);
        
        // Example of phase-specific handling
        switch (phase)
        {
            case InputPhase.OnPressed:
                // Start action, such as picking up an object or interacting
                Debug.Log("Action started");
                break;
                
            case InputPhase.OnHold:
                // Continue action, such as charging an ability
                float holdDuration = Time.time - inputStartTimes["Action"];
                Debug.Log($"Action held for {holdDuration:F2} seconds");
                break;
                
            case InputPhase.OnReleased:
                // Complete action, such as releasing a charged ability
                float totalDuration = Time.time - inputStartTimes["Action"];
                Debug.Log($"Action completed after {totalDuration:F2} seconds");
                break;
        }
    }
    
    public void OnAvatarMoveInput(InputPhase phase, Vector2 input)
    {
        HandleInputPhase("Move", phase);
        
        // Example of phase-specific handling with the input value
        switch (phase)
        {
            case InputPhase.OnPressed:
                // Start movement animation
                Debug.Log($"Movement started: {input}");
                break;
                
            case InputPhase.OnHold:
                // Continue movement
                Debug.Log($"Moving: {input}");
                break;
                
            case InputPhase.OnReleased:
                // End movement animation
                Debug.Log("Movement ended");
                break;
        }
    }
    
    public void OnAvatarAutoSprintToggled(bool enabled)
    {
        // Not phase-based, so handled differently
        Debug.Log($"Auto sprint toggled: {enabled}");
    }
    
    // Implementation of IInputActionsListener methods
    public void OnInputCaptureStarted(InputCaptureType captureType)
    {
        Debug.Log($"Input capture started: {captureType}");
    }
    
    public void OnInputCaptureStopped(InputCaptureType captureType)
    {
        Debug.Log($"Input capture stopped: {captureType}");
        
        // Clear all input states when input capture stops
        inputStates.Clear();
        inputStartTimes.Clear();
    }
    
    // Helper method to track input phases
    private void HandleInputPhase(string inputName, InputPhase phase)
    {
        // Store the current input phase
        inputStates[inputName] = phase;
        
        // Track the start time when input is pressed
        if (phase == InputPhase.OnPressed)
        {
            inputStartTimes[inputName] = Time.time;
        }
        // Remove the start time when input is released
        else if (phase == InputPhase.OnReleased)
        {
            if (inputStartTimes.ContainsKey(inputName))
            {
                // We keep the time temporarily for logging but could remove it here
                // inputStartTimes.Remove(inputName);
            }
        }
        
        Debug.Log($"Input '{inputName}' phase: {phase}");
    }
    
    // Example of how to use the tracked input phases in Update
    private void Update()
    {
        // Check if jump is currently being held
        if (inputStates.TryGetValue("Jump", out var jumpPhase) && 
            (jumpPhase == InputPhase.OnPressed || jumpPhase == InputPhase.OnHold))
        {
            // Perform continuous jump-related logic
            float jumpDuration = Time.time - inputStartTimes["Jump"];
            // Example: Modify jump height based on hold duration
            float jumpHeightMultiplier = Mathf.Min(1.0f, jumpDuration * 2.0f);
            Debug.Log($"Jump height multiplier: {jumpHeightMultiplier:F2}");
        }
        
        // Check if sprint is active
        if (inputStates.TryGetValue("Sprint", out var sprintPhase) && 
            (sprintPhase == InputPhase.OnPressed || sprintPhase == InputPhase.OnHold))
        {
            // Apply sprint effects
            // Example: Decrease stamina over time
            float sprintDuration = Time.time - inputStartTimes["Sprint"];
            float staminaDecrease = sprintDuration * 0.05f;
            Debug.Log($"Stamina decrease: {staminaDecrease:F2}");
        }
    }
}
```

## Best Practices

1. **Complete Phase Handling**: Always handle all three phases (OnPressed, OnHold, OnReleased) in your input callbacks to ensure complete input tracking.

2. **State Tracking**: Maintain the current state of inputs by storing the most recent InputPhase for each input type to make informed decisions in your game logic.

3. **Duration Tracking**: Use the OnPressed phase to start timing an input and the OnReleased phase to calculate the total duration, which can be useful for charge-based actions or variable-height jumps.

4. **Proper Cleanup**: Clear all input states when input capture stops or when your component is destroyed to prevent stale data.

5. **Continuous vs. Discrete Actions**: Differentiate between continuous actions (where OnHold is meaningful) and discrete actions (where only OnPressed matters) in your implementation.

6. **Input Validation**: For critical actions, validate that you've seen an OnPressed phase before processing an OnHold or OnReleased phase to prevent edge cases.

7. **Phase Transitions**: Be aware that you might not always receive all phases in order. Network or performance issues might occasionally cause skipped phases.

## Common Use Cases

1. **Button-Based Actions**: Use OnPressed for immediate button actions like shooting, jumping, or interacting with objects.

2. **Charge-Based Mechanics**: Use the duration between OnPressed and OnReleased to determine the strength of a charged action, such as a charged attack or variable-height jump.

3. **Toggle States**: Use OnPressed to toggle between different states, such as walking and running, or equipping different items.

4. **Continuous Movement**: Use OnHold to track continuous movement input, applying movement forces as long as the input is held.

5. **Input Sequences**: Track sequences of input phases to implement combo systems or special moves that require specific input patterns.

6. **Stamina Systems**: Use the duration of held inputs to manage stamina consumption for actions like sprinting or blocking.

7. **UI Interactions**: Differentiate between taps (quick press and release) and holds (press held for longer duration) for different UI interactions.

8. **Input Visualization**: Create visual feedback that changes based on the current input phase, such as button highlights or progress indicators for charged actions.
