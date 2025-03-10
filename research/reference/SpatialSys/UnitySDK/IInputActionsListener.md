## Overview
Interface for listening to input capture events. This is the base interface for all input listeners in the Spatial SDK, providing fundamental methods for tracking when input capture begins and ends.

## Methods

| Method | Description |
| --- | --- |
| OnInputCaptureStarted(InputCaptureType) | Called when input capture has started. The parameter indicates what type of input capture has begun. |
| OnInputCaptureStopped(InputCaptureType) | Called when input capture has stopped. The parameter indicates what type of input capture has ended. |

## Usage Example

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;

public class BasicInputListener : MonoBehaviour, IInputActionsListener
{
    private bool isCapturingInput = false;
    private InputCaptureType currentCaptureType = InputCaptureType.None;

    private void Start()
    {
        // Register as a basic input listener
        Debug.Log("Input listener initialized and ready");
    }

    private void OnDestroy()
    {
        // If still capturing input, make sure to release it
        if (isCapturingInput)
        {
            try
            {
                SpatialBridge.inputService.ReleaseInputCapture(this);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error releasing input capture: {e.Message}");
            }
        }
    }

    // Implement IInputActionsListener methods
    public void OnInputCaptureStarted(InputCaptureType captureType)
    {
        isCapturingInput = true;
        currentCaptureType = captureType;
        
        Debug.Log($"Input capture started: {captureType}");
        
        // Initialize any state needed for this input capture type
        switch (captureType)
        {
            case InputCaptureType.Avatar:
                Debug.Log("Avatar input capture started");
                break;
            case InputCaptureType.Vehicle:
                Debug.Log("Vehicle input capture started");
                break;
            case InputCaptureType.Custom:
                Debug.Log("Custom input capture started");
                break;
        }
    }

    public void OnInputCaptureStopped(InputCaptureType captureType)
    {
        isCapturingInput = false;
        currentCaptureType = InputCaptureType.None;
        
        Debug.Log($"Input capture stopped: {captureType}");
        
        // Clean up any state related to this input capture type
        switch (captureType)
        {
            case InputCaptureType.Avatar:
                Debug.Log("Avatar input capture stopped");
                break;
            case InputCaptureType.Vehicle:
                Debug.Log("Vehicle input capture stopped");
                break;
            case InputCaptureType.Custom:
                Debug.Log("Custom input capture stopped");
                break;
        }
    }

    // Example of a custom method to start capturing input
    public void StartCustomInputCapture()
    {
        if (!isCapturingInput)
        {
            Debug.Log("Starting custom input capture");
            SpatialBridge.inputService.StartCompleteCustomInputCapture(this);
        }
        else
        {
            Debug.LogWarning($"Already capturing input of type: {currentCaptureType}");
        }
    }

    // Example of a custom method to stop capturing input
    public void StopInputCapture()
    {
        if (isCapturingInput)
        {
            Debug.Log("Stopping input capture");
            SpatialBridge.inputService.ReleaseInputCapture(this);
        }
    }
}
```

## Best Practices

1. **Always Release Input**: When your component is disabled or destroyed, make sure to release any active input capture to prevent input conflicts with other systems.

2. **Track Input State**: Maintain an internal state that tracks whether your component is currently capturing input and what type of capture is active.

3. **Error Handling**: Wrap input service calls in try-catch blocks to gracefully handle potential errors, especially during scene transitions or when the input service might not be available.

4. **Clean Resource Management**: Use the OnInputCaptureStopped method to clean up any resources or state that was initialized during input capture.

5. **Check Before Capturing**: Before attempting to capture input, check if your component is already capturing input to avoid unexpected behavior.

6. **Appropriate Initialization**: Initialize your input capture at the appropriate time in your component lifecycle, typically after ensuring that the SpatialBridge is fully initialized.

7. **Debugging Support**: Include sufficient logging to help debug input-related issues, especially during development.

## Common Use Cases

1. **Input State Management**: Track when input capture begins and ends to properly manage your component's state.

2. **Custom Input Systems**: Serve as the foundation for building specialized input handling systems.

3. **Input Conflict Resolution**: Manage transitions between different input modes in your application.

4. **Debug Logging**: Monitor and log input capture events for debugging purposes.

5. **UI Mode Transitions**: Switch between gameplay and UI modes with appropriate input handling for each.

6. **Input Tutorials**: Create tutorial systems that guide users through different input methods.

7. **Accessibility Features**: Implement alternative input methods or input modification for accessibility purposes.

8. **Input Testing**: Create test harnesses for validating input behavior in different scenarios.
