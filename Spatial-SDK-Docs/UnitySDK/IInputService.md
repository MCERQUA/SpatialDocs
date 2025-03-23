# IInputService

Input Service Interface

Service for managing input handling and control systems in Spatial spaces. This service provides functionality for capturing and processing various types of input, including avatar control, vehicle control, and custom input handling.

## Methods

### Input Capture
| Method | Description |
| --- | --- |
| StartAvatarInputCapture(...) | Begin avatar input |
| StartVehicleInputCapture(...) | Begin vehicle input |
| StartCompleteCustomInputCapture(...) | Begin custom input |
| ReleaseInputCapture(...) | Stop input capture |

### Input Control
| Method | Description |
| --- | --- |
| SetEmoteBindingsEnabled(...) | Toggle emote controls |
| PlayVibration(...) | Trigger haptic feedback |

## Events

### Input State
| Event | Description |
| --- | --- |
| onInputCaptureStarted | Capture begin notification |
| onInputCaptureStopped | Capture end notification |

## Usage Examples

```csharp
// Example: Input Manager
public class InputManager : MonoBehaviour, IAvatarInputActionsListener
{
    private IInputService inputService;
    private Dictionary<string, InputState> inputStates;
    private bool isInitialized;

    private class InputState
    {
        public bool isActive;
        public float lastUpdateTime;
        public Dictionary<string, object> settings;
        public Dictionary<string, object> actions;
    }

    void Start()
    {
        inputService = SpatialBridge.inputService;
        inputStates = new Dictionary<string, InputState>();
        InitializeManager();
        SubscribeToEvents();
    }

    private void InitializeManager()
    {
        StartAvatarInput();
    }

    private void SubscribeToEvents()
    {
        inputService.onInputCaptureStarted += HandleInputCaptureStarted;
        inputService.onInputCaptureStopped += HandleInputCaptureStopped;
    }

    public void StartAvatarInput(
        bool overrideMovement = true,
        bool overrideJump = true,
        bool overrideSprint = true,
        bool overrideCrouch = true
    )
    {
        try
        {
            inputService.StartAvatarInputCapture(
                overrideMovement,
                overrideJump,
                overrideSprint,
                overrideCrouch,
                this
            );

            InitializeInputState("avatar", new Dictionary<string, object>
            {
                { "type", "avatar" },
                { "overrideMovement", overrideMovement },
                { "overrideJump", overrideJump },
                { "overrideSprint", overrideSprint },
                { "overrideCrouch", overrideCrouch }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error starting avatar input: {e.Message}");
        }
    }

    public void StartVehicleInput(
        VehicleInputFlags flags,
        Sprite accelerateIcon = null,
        Sprite brakeIcon = null
    )
    {
        try
        {
            inputService.StartVehicleInputCapture(
                flags,
                accelerateIcon,
                brakeIcon,
                new VehicleInputHandler()
            );

            InitializeInputState("vehicle", new Dictionary<string, object>
            {
                { "type", "vehicle" },
                { "flags", flags },
                { "hasAccelerateIcon", accelerateIcon != null },
                { "hasBrakeIcon", brakeIcon != null }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error starting vehicle input: {e.Message}");
        }
    }

    public void StartCustomInput()
    {
        try
        {
            inputService.StartCompleteCustomInputCapture(
                new CustomInputHandler()
            );

            InitializeInputState("custom", new Dictionary<string, object>
            {
                { "type", "custom" },
                { "startTime", Time.time }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error starting custom input: {e.Message}");
        }
    }

    public void StopInput(string inputType)
    {
        if (!inputStates.TryGetValue(inputType, out var state))
            return;

        try
        {
            inputService.ReleaseInputCapture(GetListenerForType(inputType));
            
            state.isActive = false;
            state.settings["endTime"] = Time.time;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error stopping {inputType} input: {e.Message}");
        }
    }

    public void TriggerHapticFeedback(
        float frequency = 1f,
        float amplitude = 1f,
        float duration = 0.1f
    )
    {
        try
        {
            inputService.PlayVibration(frequency, amplitude, duration);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error triggering haptic feedback: {e.Message}");
        }
    }

    private void InitializeInputState(
        string inputType,
        Dictionary<string, object> settings
    )
    {
        var state = new InputState
        {
            isActive = true,
            lastUpdateTime = Time.time,
            settings = settings,
            actions = new Dictionary<string, object>()
        };

        inputStates[inputType] = state;
    }

    private IInputActionsListener GetListenerForType(string inputType)
    {
        switch (inputType)
        {
            case "avatar":
                return this;
            case "vehicle":
                return new VehicleInputHandler();
            case "custom":
                return new CustomInputHandler();
            default:
                throw new ArgumentException($"Unknown input type: {inputType}");
        }
    }

    private void HandleInputCaptureStarted()
    {
        // Implementation for input capture start handling
    }

    private void HandleInputCaptureStopped()
    {
        // Implementation for input capture stop handling
    }

    void OnDestroy()
    {
        if (inputService != null)
        {
            inputService.onInputCaptureStarted -= HandleInputCaptureStarted;
            inputService.onInputCaptureStopped -= HandleInputCaptureStopped;
        }
    }

    // IAvatarInputActionsListener Implementation
    public void OnMove(Vector2 input)
    {
        if (!inputStates.TryGetValue("avatar", out var state))
            return;

        state.actions["lastMove"] = input;
        state.lastUpdateTime = Time.time;
    }

    public void OnJump()
    {
        if (!inputStates.TryGetValue("avatar", out var state))
            return;

        state.actions["lastJump"] = Time.time;
        state.lastUpdateTime = Time.time;
    }

    public void OnSprint(bool isSprinting)
    {
        if (!inputStates.TryGetValue("avatar", out var state))
            return;

        state.actions["isSprinting"] = isSprinting;
        state.lastUpdateTime = Time.time;
    }

    public void OnCrouch(bool isCrouching)
    {
        if (!inputStates.TryGetValue("avatar", out var state))
            return;

        state.actions["isCrouching"] = isCrouching;
        state.lastUpdateTime = Time.time;
    }
}

// Example: Vehicle Input Handler
public class VehicleInputHandler : IVehicleInputActionsListener
{
    public void OnAccelerate(float input)
    {
        // Implementation for acceleration input
    }

    public void OnBrake(float input)
    {
        // Implementation for brake input
    }

    public void OnSteer(float input)
    {
        // Implementation for steering input
    }
}

// Example: Custom Input Handler
public class CustomInputHandler : IInputActionsListener
{
    public void OnInputAction(
        string actionName,
        object value
    )
    {
        // Implementation for custom input actions
    }
}
```

## Best Practices

1. Input Management
   - Handle states
   - Track changes
   - Validate input
   - Cache data

2. Input Control
   - Smooth input
   - Handle timing
   - Process events
   - Clean removal

3. Performance
   - Batch updates
   - Cache states
   - Handle timing
   - Optimize checks

4. Error Handling
   - Validate input
   - Handle failures
   - Recover states
   - Log issues

## Common Use Cases

1. Input Types
   - Avatar control
   - Vehicle control
   - Custom control
   - Mixed input

2. Input Features
   - Movement input
   - Action input
   - State tracking
   - Event handling

3. Control Systems
   - Character control
   - Vehicle control
   - Camera control
   - UI interaction

4. Input Processing
   - Input smoothing
   - Input validation
   - State management
   - Event handling
