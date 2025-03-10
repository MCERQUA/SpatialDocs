## Overview
An enumeration that defines the different types of input capture that can be used with the Input Service. This enum is used to identify which type of input is being captured when working with input listeners.

## Properties

| Property | Description |
| --- | --- |
| None | No input is being captured. This is the default state. |
| Avatar | Avatar input is being captured, such as movement, jumping, and other avatar-specific controls. |
| Vehicle | Vehicle input is being captured, such as steering, throttle, and other vehicle-specific controls. |
| Custom | Custom input is being captured, allowing for developer-defined input behavior. |

## Usage Example

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour, IInputActionsListener
{
    // Dictionary to track active listeners by capture type
    private Dictionary<InputCaptureType, IInputActionsListener> activeListeners;
    
    // Track the current state of input capture
    private InputCaptureType currentCaptureType = InputCaptureType.None;
    
    private void Awake()
    {
        activeListeners = new Dictionary<InputCaptureType, IInputActionsListener>();
    }
    
    private void Start()
    {
        // Register as a listener to monitor input capture changes
        Debug.Log("Input manager initialized");
    }
    
    private void OnDestroy()
    {
        // Clean up any active listeners
        foreach (var listener in activeListeners.Values)
        {
            try
            {
                SpatialBridge.inputService.ReleaseInputCapture(listener);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error releasing input capture: {e.Message}");
            }
        }
        
        activeListeners.Clear();
    }
    
    // IInputActionsListener implementation
    public void OnInputCaptureStarted(InputCaptureType captureType)
    {
        currentCaptureType = captureType;
        Debug.Log($"Input capture started: {captureType}");
    }
    
    public void OnInputCaptureStopped(InputCaptureType captureType)
    {
        currentCaptureType = InputCaptureType.None;
        Debug.Log($"Input capture stopped: {captureType}");
        
        // Remove the listener from active listeners
        if (activeListeners.ContainsKey(captureType))
        {
            activeListeners.Remove(captureType);
        }
    }
    
    // Public method to start capturing avatar input
    public void StartAvatarInputCapture(IAvatarInputActionsListener listener)
    {
        if (currentCaptureType != InputCaptureType.None)
        {
            Debug.LogWarning($"Another input type is already active: {currentCaptureType}");
            return;
        }
        
        try
        {
            SpatialBridge.inputService.StartAvatarInputCapture(
                true,   // Override movement
                true,   // Override jump
                true,   // Override sprint
                true,   // Override crouch
                listener
            );
            
            activeListeners[InputCaptureType.Avatar] = listener;
            Debug.Log("Started avatar input capture");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error starting avatar input capture: {e.Message}");
        }
    }
    
    // Public method to start capturing vehicle input
    public void StartVehicleInputCapture(IVehicleInputActionsListener listener)
    {
        if (currentCaptureType != InputCaptureType.None)
        {
            Debug.LogWarning($"Another input type is already active: {currentCaptureType}");
            return;
        }
        
        try
        {
            // Capture all vehicle input types
            VehicleInputFlags flags = VehicleInputFlags.Throttle | 
                                     VehicleInputFlags.Steer1D | 
                                     VehicleInputFlags.Reverse | 
                                     VehicleInputFlags.PrimaryAction |
                                     VehicleInputFlags.SecondaryAction |
                                     VehicleInputFlags.Exit;
                                     
            SpatialBridge.inputService.StartVehicleInputCapture(
                flags,
                null,   // Accelerate icon
                null,   // Brake icon
                listener
            );
            
            activeListeners[InputCaptureType.Vehicle] = listener;
            Debug.Log("Started vehicle input capture");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error starting vehicle input capture: {e.Message}");
        }
    }
    
    // Public method to start capturing custom input
    public void StartCustomInputCapture(IInputActionsListener listener)
    {
        if (currentCaptureType != InputCaptureType.None)
        {
            Debug.LogWarning($"Another input type is already active: {currentCaptureType}");
            return;
        }
        
        try
        {
            SpatialBridge.inputService.StartCompleteCustomInputCapture(listener);
            
            activeListeners[InputCaptureType.Custom] = listener;
            Debug.Log("Started custom input capture");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error starting custom input capture: {e.Message}");
        }
    }
    
    // Public method to stop capturing input of a specific type
    public void StopInputCapture(InputCaptureType captureType)
    {
        if (activeListeners.TryGetValue(captureType, out var listener))
        {
            try
            {
                SpatialBridge.inputService.ReleaseInputCapture(listener);
                Debug.Log($"Released {captureType} input capture");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error releasing {captureType} input capture: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"No active listener for {captureType}");
        }
    }
    
    // Public method to check if a specific input type is currently being captured
    public bool IsCapturingInputType(InputCaptureType captureType)
    {
        return currentCaptureType == captureType;
    }
    
    // Public method to get the current input capture type
    public InputCaptureType GetCurrentCaptureType()
    {
        return currentCaptureType;
    }
}
```

## Best Practices

1. **Type Checking**: Always check the current InputCaptureType before starting a new input capture to avoid conflicts.

2. **State Management**: Keep track of the active InputCaptureType in your application to properly manage transitions between different input modes.

3. **Error Handling**: When releasing input capture, make sure you're releasing the correct type to avoid unexpected behavior.

4. **Clear Transitions**: Plan for clear transitions between different input capture types, providing feedback to the user when input modes change.

5. **Default State**: Always return to InputCaptureType.None when you're done capturing input to ensure other systems can utilize input when needed.

6. **Input Priority**: Consider the priority of different input types in your application. For example, UI input might need to temporarily override avatar input.

7. **Fallback Mechanisms**: Implement fallback mechanisms for cases where the desired input capture type cannot be activated.

## Common Use Cases

1. **Input Mode Switching**: Switch between avatar control, vehicle control, and custom input modes based on gameplay requirements.

2. **UI Interaction**: Use Custom input capture for UI interactions that require specific input handling.

3. **Vehicle Systems**: Use Vehicle input capture when players enter vehicles, providing specialized controls for driving or piloting.

4. **Mini-games**: Implement custom input capture for mini-games that require specific input handling different from standard avatar controls.

5. **Debug Tools**: Create debugging tools that can monitor and display the current input capture type during development.

6. **Input Tutorials**: Build tutorial systems that guide users through different input types one at a time.

7. **Input Management**: Create centralized input management systems that coordinate input capture across multiple game systems.

8. **Accessibility Options**: Implement alternative input methods for accessibility purposes, switching between different input capture types based on user preferences.
