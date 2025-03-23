# Avatar Input Control Template

## Overview
The Avatar Input Control template provides a comprehensive framework for customizing and extending the input system for avatars in Spatial experiences. It allows developers to override default avatar input handling, implement custom controls, and create specialized movement behaviors. This template is particularly useful for creating games with unique movement mechanics, interactive control schemes, or specialized avatar behaviors not covered by Spatial's default controls.

## Features
- **Custom Input Handling**: Override default input mapping and create custom control schemes
- **Extended Input Actions**: Additional input actions beyond standard movement and interaction
- **Input Visualization**: Helper systems for displaying current input state
- **Multi-Platform Support**: Input handling across desktop, mobile, and VR
- **Controller Support**: Gamepad and specialized controller integration
- **Input Rebinding**: Runtime input remapping for accessibility
- **Input Filtering**: Prevent certain inputs in specific game states
- **Event-Based Architecture**: Trigger events based on complex input combinations
- **Avatar Animation Integration**: Tie custom inputs to animation states

## Included Components

### 1. Custom Avatar Controller
The core component for overriding Spatial's default avatar controller:
- Complete replacement for standard movement
- Detection of control mode changes
- Integration with Spatial's avatar system
- Platform-specific input handling
- User preferences for control sensitivity

### 2. Input Visualization
Tools to display current input state to users:
- On-screen control indicators
- Button prompt system
- Mobile touch overlay
- Input debugger for development
- Control guide UI elements

### 3. Input Action Manager
System for managing and triggering input actions:
- Action registration and configuration 
- Input combination detection
- Action priority management
- Context-sensitive actions
- Timing-based input sequences

### 4. Platform-Specific Controls
Specialized handling for different platforms:
- Desktop keyboard/mouse optimization
- Mobile touchscreen controls with customizable layout
- Gamepad support with multiple control schemes
- VR controller integration
- Input method switching based on detected platform

## Integration with SDK Components
The template integrates with these key SDK components:

| SDK Component | Usage in Template |
|---------------|-------------------|
| IActorService | Accessing the local player's avatar identity |
| IAvatar | Controlling avatar position, rotation, and animation state |
| ISpatialInput | Accessing and extending the core input system |
| IUserSettingsService | Storing and retrieving user input preferences |
| ICameraService | Coordinating camera behavior with input changes |
| IUIService | Displaying input prompts and interactive UI elements |

## When to Use
Use this template when:
- Creating games with custom movement mechanics
- Implementing specialized control schemes
- Building experiences with context-sensitive actions
- Creating accessibility options for input
- Implementing multi-platform experiences with optimized controls for each platform
- Coordinating input with animation systems
- Creating timing-based or rhythm game controls
- Implementing multiplayer games with specialized control modes

## Implementation Details

### Custom Avatar Controller Implementation
The core component for replacing the default avatar controller:

```csharp
public class CustomAvatarController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravityMultiplier = 2f;
    
    [Header("Input Smoothing")]
    [SerializeField] private float rotationSmoothTime = 0.12f;
    [SerializeField] private float movementSmoothTime = 0.05f;
    
    private IAvatar avatar;
    private Vector3 currentMoveInput;
    private Vector3 movementVelocity;
    private Vector3 targetMoveInput;
    private float rotationVelocity;
    
    private void Start()
    {
        // Get the local player's avatar
        avatar = SpatialBridge.actorService.localActor.avatar;
        
        // Override the default avatar controller
        if (avatar != null)
        {
            // Disable the default controller (implementation depends on Spatial's system)
            DisableDefaultController();
            
            // Initialize our custom controller
            InitializeCustomController();
        }
    }
    
    private void Update()
    {
        if (avatar == null) return;
        
        // Get input from our custom input manager
        targetMoveInput = GetMovementInput();
        
        // Apply smoothing to movement
        currentMoveInput = Vector3.SmoothDamp(
            currentMoveInput, 
            targetMoveInput, 
            ref movementVelocity, 
            movementSmoothTime
        );
        
        // Get current input state for running
        bool isRunning = InputManager.GetRunInput();
        
        // Calculate movement speed
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        
        // Apply movement to avatar
        MoveAvatar(currentMoveInput * currentSpeed);
        
        // Handle rotation
        if (currentMoveInput.magnitude > 0.1f)
        {
            RotateAvatar(currentMoveInput);
        }
        
        // Handle jumping
        if (InputManager.GetJumpInputDown() && avatar.isGrounded)
        {
            JumpAvatar();
        }
    }
    
    private void DisableDefaultController()
    {
        // Implementation depends on Spatial's API
        // Example: avatar.SetUseDefaultController(false);
    }
    
    private void InitializeCustomController()
    {
        // Set initial values and subscribe to events
        avatar.gravityMultiplier = gravityMultiplier;
    }
    
    private Vector3 GetMovementInput()
    {
        // Get raw input from the input manager
        Vector2 inputVector = InputManager.GetMovementInput();
        
        // Convert to 3D movement vector relative to camera
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        
        // Remove any y component to make movement horizontal
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        return cameraForward * inputVector.y + cameraRight * inputVector.x;
    }
    
    private void MoveAvatar(Vector3 moveVector)
    {
        // Implementation depends on Spatial's API
        // Example: avatar.AddMovement(moveVector * Time.deltaTime);
    }
    
    private void RotateAvatar(Vector3 moveDir)
    {
        float targetRotation = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
        float smoothRotation = Mathf.SmoothDampAngle(
            avatar.transform.eulerAngles.y, 
            targetRotation, 
            ref rotationVelocity, 
            rotationSmoothTime
        );
        
        // Apply rotation
        avatar.transform.rotation = Quaternion.Euler(0, smoothRotation, 0);
    }
    
    private void JumpAvatar()
    {
        // Implementation depends on Spatial's API
        // Example: avatar.Jump(jumpHeight);
    }
}
```

### Input Manager Implementation
The centralized input management system:

```csharp
public static class InputManager
{
    private static Dictionary<string, InputAction> actions = new Dictionary<string, InputAction>();
    private static Dictionary<string, KeyBinding> keyBindings = new Dictionary<string, KeyBinding>();
    
    // Default key bindings initialized at startup
    static InputManager()
    {
        // Initialize default bindings
        keyBindings["MoveForward"] = new KeyBinding(KeyCode.W, KeyCode.UpArrow);
        keyBindings["MoveBackward"] = new KeyBinding(KeyCode.S, KeyCode.DownArrow);
        keyBindings["MoveLeft"] = new KeyBinding(KeyCode.A, KeyCode.LeftArrow);
        keyBindings["MoveRight"] = new KeyBinding(KeyCode.D, KeyCode.RightArrow);
        keyBindings["Jump"] = new KeyBinding(KeyCode.Space);
        keyBindings["Run"] = new KeyBinding(KeyCode.LeftShift, KeyCode.RightShift);
        keyBindings["Interact"] = new KeyBinding(KeyCode.E);
        
        // Register default actions
        RegisterAction("Jump", GetKeyDown, "Jump");
        RegisterAction("Run", GetKey, "Run");
        RegisterAction("Interact", GetKeyDown, "Interact");
    }
    
    // Get the movement input vector
    public static Vector2 GetMovementInput()
    {
        Vector2 input = Vector2.zero;
        
        if (GetKey("MoveForward")) input.y += 1;
        if (GetKey("MoveBackward")) input.y -= 1;
        if (GetKey("MoveLeft")) input.x -= 1;
        if (GetKey("MoveRight")) input.x += 1;
        
        // Normalize the input if it exceeds a magnitude of 1
        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }
        
        return input;
    }
    
    // Get jump input
    public static bool GetJumpInputDown()
    {
        return GetKeyDown("Jump");
    }
    
    // Get run input
    public static bool GetRunInput()
    {
        return GetKey("Run");
    }
    
    // Get interact input
    public static bool GetInteractInputDown()
    {
        return GetKeyDown("Interact");
    }
    
    // Check if a key is currently held down
    public static bool GetKey(string actionName)
    {
        if (keyBindings.TryGetValue(actionName, out KeyBinding binding))
        {
            return Input.GetKey(binding.primaryKey) || 
                   (binding.alternateKey != KeyCode.None && Input.GetKey(binding.alternateKey));
        }
        return false;
    }
    
    // Check if a key was pressed this frame
    public static bool GetKeyDown(string actionName)
    {
        if (keyBindings.TryGetValue(actionName, out KeyBinding binding))
        {
            return Input.GetKeyDown(binding.primaryKey) || 
                   (binding.alternateKey != KeyCode.None && Input.GetKeyDown(binding.alternateKey));
        }
        return false;
    }
    
    // Check if a key was released this frame
    public static bool GetKeyUp(string actionName)
    {
        if (keyBindings.TryGetValue(actionName, out KeyBinding binding))
        {
            return Input.GetKeyUp(binding.primaryKey) || 
                   (binding.alternateKey != KeyCode.None && Input.GetKeyUp(binding.alternateKey));
        }
        return false;
    }
    
    // Register a new action with a delegate
    public static void RegisterAction(string actionName, Func<string, bool> checkMethod, string bindingName)
    {
        actions[actionName] = new InputAction
        {
            name = actionName,
            bindingName = bindingName,
            checkMethod = checkMethod
        };
    }
    
    // Check if an action is triggered
    public static bool GetAction(string actionName)
    {
        if (actions.TryGetValue(actionName, out InputAction action))
        {
            return action.checkMethod(action.bindingName);
        }
        return false;
    }
    
    // Rebind a key
    public static void RebindKey(string actionName, KeyCode primaryKey, KeyCode alternateKey = KeyCode.None)
    {
        keyBindings[actionName] = new KeyBinding(primaryKey, alternateKey);
    }
    
    // Reset bindings to default
    public static void ResetBindingsToDefault()
    {
        // Reset to default bindings
        keyBindings.Clear();
        
        // Re-initialize defaults
        keyBindings["MoveForward"] = new KeyBinding(KeyCode.W, KeyCode.UpArrow);
        keyBindings["MoveBackward"] = new KeyBinding(KeyCode.S, KeyCode.DownArrow);
        keyBindings["MoveLeft"] = new KeyBinding(KeyCode.A, KeyCode.LeftArrow);
        keyBindings["MoveRight"] = new KeyBinding(KeyCode.D, KeyCode.RightArrow);
        keyBindings["Jump"] = new KeyBinding(KeyCode.Space);
        keyBindings["Run"] = new KeyBinding(KeyCode.LeftShift, KeyCode.RightShift);
        keyBindings["Interact"] = new KeyBinding(KeyCode.E);
    }
    
    // Save the current bindings
    public static void SaveBindings()
    {
        // Implementation would use IUserSettingsService to save bindings
        // Example: SpatialBridge.userSettingsService.SetData("input_bindings", SerializeBindings());
    }
    
    // Load saved bindings
    public static void LoadBindings()
    {
        // Implementation would use IUserSettingsService to load bindings
        // Example: string savedBindings = SpatialBridge.userSettingsService.GetData("input_bindings");
        // DeserializeBindings(savedBindings);
    }
}

// Helper class for key bindings
public class KeyBinding
{
    public KeyCode primaryKey;
    public KeyCode alternateKey;
    
    public KeyBinding(KeyCode primary, KeyCode alternate = KeyCode.None)
    {
        primaryKey = primary;
        alternateKey = alternate;
    }
}

// Helper class for input actions
public class InputAction
{
    public string name;
    public string bindingName;
    public Func<string, bool> checkMethod;
}
```

### Mobile Touch Controls Implementation
Implementation for mobile platform support:

```csharp
public class MobileTouchControls : MonoBehaviour
{
    [SerializeField] private RectTransform joystickArea;
    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private RectTransform joystickHandle;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Toggle runToggle;
    
    private Vector2 joystickStartPosition;
    private Vector2 joystickCurrentPosition;
    private bool joystickActive = false;
    private int joystickTouchId = -1;
    
    private Vector2 currentJoystickInput = Vector2.zero;
    private bool jumpPressed = false;
    private bool interactPressed = false;
    private bool runActive = false;
    
    private void Start()
    {
        // Initialize joystick position
        joystickStartPosition = joystickBackground.position;
        
        // Set up button callbacks
        jumpButton.onClick.AddListener(() => jumpPressed = true);
        interactButton.onClick.AddListener(() => interactPressed = true);
        runToggle.onValueChanged.AddListener(value => runActive = value);
        
        // Only enable on mobile platforms
        #if !UNITY_ANDROID && !UNITY_IOS
        gameObject.SetActive(false);
        #endif
    }
    
    private void Update()
    {
        // Process touch input for joystick
        ProcessJoystickInput();
        
        // Reset single-frame input states
        if (jumpPressed) jumpPressed = false;
        if (interactPressed) interactPressed = false;
    }
    
    private void ProcessJoystickInput()
    {
        // Handle touch input for the joystick
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                Vector2 touchPos = touch.position;
                
                // Check if touch is within joystick area
                if (RectTransformUtility.RectangleContainsScreenPoint(joystickArea, touchPos))
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            if (!joystickActive)
                            {
                                joystickActive = true;
                                joystickTouchId = touch.fingerId;
                                joystickBackground.position = touchPos;
                                joystickStartPosition = touchPos;
                            }
                            break;
                            
                        case TouchPhase.Moved:
                        case TouchPhase.Stationary:
                            if (joystickActive && touch.fingerId == joystickTouchId)
                            {
                                joystickCurrentPosition = touchPos;
                                UpdateJoystickHandlePosition();
                            }
                            break;
                            
                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            if (joystickActive && touch.fingerId == joystickTouchId)
                            {
                                ResetJoystick();
                            }
                            break;
                    }
                }
            }
        }
    }
    
    private void UpdateJoystickHandlePosition()
    {
        // Calculate joystick direction and magnitude
        Vector2 direction = joystickCurrentPosition - joystickStartPosition;
        float magnitude = direction.magnitude;
        
        // Limit the joystick movement to the background's radius
        float joystickRadius = joystickBackground.rect.width * 0.5f;
        if (magnitude > joystickRadius)
        {
            direction = direction.normalized * joystickRadius;
        }
        
        // Update handle position
        joystickHandle.position = joystickStartPosition + direction;
        
        // Calculate normalized input value (-1 to 1 range)
        currentJoystickInput = direction / joystickRadius;
    }
    
    private void ResetJoystick()
    {
        joystickActive = false;
        joystickTouchId = -1;
        joystickBackground.position = joystickStartPosition;
        joystickHandle.position = joystickStartPosition;
        currentJoystickInput = Vector2.zero;
    }
    
    // Access method for the custom avatar controller
    public Vector2 GetJoystickInput()
    {
        return currentJoystickInput;
    }
    
    // Check if jump was pressed this frame
    public bool GetJumpPressed()
    {
        return jumpPressed;
    }
    
    // Check if interact was pressed this frame
    public bool GetInteractPressed()
    {
        return interactPressed;
    }
    
    // Check if run is active
    public bool GetRunActive()
    {
        return runActive;
    }
}
```

## Best Practices
- **Test Across Platforms**: Ensure controls work well on all target platforms
- **Provide Visual Feedback**: Always give clear visual indicators for available inputs
- **Consider Accessibility**: Include options for alternative control schemes
- **Gradual Introduction**: Introduce complex control schemes gradually through tutorials
- **Consistent Conventions**: Follow platform conventions for familiar controls
- **Input Buffering**: Implement input buffering for timing-sensitive actions
- **Optimize for Mobile**: Ensure touch controls are large enough and positioned for comfortable use
- **Measure Performance**: Monitor input processing performance, especially on mobile
- **Validate Input**: Always validate input before applying it to avoid unexpected behavior
- **Allow Customization**: Let users rebind keys and adjust sensitivity
- **Provide Defaults**: Include a way to restore default control settings

## Related Templates
- [Camera Modes](../Camera/CameraModes.md) - For implementing cameras that work with custom controls
- [Obby (Obstacle Course) Game](../Games/ObbyGame.md) - Example of specialized movement controls
- [Simple Car Controller](../Vehicles/SimpleCarController.md) - Another example of custom avatar input handling

## Additional Resources
- [Spatial Input System Documentation](https://docs.spatial.io/input-system)
- [GitHub Repository](https://github.com/spatialsys/spatial-example-avatar-input-control)
- [Unity Input System Documentation](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/index.html)
