## Overview
An enumeration that defines the different types of input that can be captured when using vehicle input mode. These flags are used when starting vehicle input capture to specify which input types should be captured and forwarded to the vehicle input listener.

## Properties

| Property | Description |
| --- | --- |
| None | No vehicle inputs will be captured. This is the default state. |
| Throttle | Vehicle throttle (acceleration) input will be captured. |
| Steer1D | Vehicle steering input will be captured in one dimension. |
| Reverse | Vehicle reverse input will be captured. |
| PrimaryAction | Primary action button input will be captured (e.g., horn, lights, boost). |
| SecondaryAction | Secondary action button input will be captured (e.g., weapon systems, special abilities). |
| Exit | Exit vehicle button input will be captured. |

## Usage Example

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using UnityEngine.UI;

public class VehicleInputManager : MonoBehaviour, IVehicleInputActionsListener
{
    [SerializeField] private Sprite accelerateIcon;
    [SerializeField] private Sprite brakeIcon;
    
    // Vehicle reference
    [SerializeField] private GameObject vehicleObject;
    private Rigidbody vehicleRigidbody;
    
    // Input state tracking
    private Vector2 steerInput;
    private float throttleInput;
    private float reverseInput;
    
    // Vehicle settings
    [SerializeField] private float maxThrottleForce = 15f;
    [SerializeField] private float maxReverseForce = 7.5f;
    [SerializeField] private float maxSteeringAngle = 35f;
    
    // Input flags configuration
    [SerializeField] private bool enableThrottle = true;
    [SerializeField] private bool enableSteering = true;
    [SerializeField] private bool enableReverse = true;
    [SerializeField] private bool enablePrimaryAction = true;
    [SerializeField] private bool enableSecondaryAction = false;
    [SerializeField] private bool enableExit = true;
    
    private void Awake()
    {
        if (vehicleObject != null)
        {
            vehicleRigidbody = vehicleObject.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogError("Vehicle object reference is missing!");
        }
    }
    
    private void Start()
    {
        // Start vehicle input capture with configured flags
        StartVehicleInputCapture();
    }
    
    private void OnDestroy()
    {
        ReleaseVehicleInput();
    }
    
    // Method to start vehicle input capture with configured flags
    public void StartVehicleInputCapture()
    {
        // Build flags based on configuration
        VehicleInputFlags flags = VehicleInputFlags.None;
        
        if (enableThrottle) flags |= VehicleInputFlags.Throttle;
        if (enableSteering) flags |= VehicleInputFlags.Steer1D;
        if (enableReverse) flags |= VehicleInputFlags.Reverse;
        if (enablePrimaryAction) flags |= VehicleInputFlags.PrimaryAction;
        if (enableSecondaryAction) flags |= VehicleInputFlags.SecondaryAction;
        if (enableExit) flags |= VehicleInputFlags.Exit;
        
        // Print the configured flags for debugging
        Debug.Log($"Starting vehicle input capture with flags: {flags}");
        
        try
        {
            SpatialBridge.inputService.StartVehicleInputCapture(
                flags,
                enableThrottle ? accelerateIcon : null,
                enableReverse ? brakeIcon : null,
                this
            );
            
            Debug.Log("Vehicle input capture started successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error starting vehicle input capture: {e.Message}");
        }
    }
    
    // Method to release vehicle input
    public void ReleaseVehicleInput()
    {
        try
        {
            SpatialBridge.inputService.ReleaseInputCapture(this);
            Debug.Log("Vehicle input capture released");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error releasing vehicle input capture: {e.Message}");
        }
    }
    
    // IVehicleInputActionsListener implementation
    public void OnVehicleSteerInput(InputPhase phase, Vector2 input)
    {
        // Only process this input if steering is enabled
        if (enableSteering)
        {
            steerInput = input;
            
            // Additional phase-specific handling if needed
            if (phase == InputPhase.OnPressed)
            {
                Debug.Log($"Steering started: {input}");
            }
        }
    }
    
    public void OnVehicleThrottleInput(InputPhase phase, float input)
    {
        // Only process this input if throttle is enabled
        if (enableThrottle)
        {
            throttleInput = input;
            
            // Additional phase-specific handling if needed
            if (phase == InputPhase.OnPressed)
            {
                Debug.Log($"Throttle started: {input}");
            }
        }
    }
    
    public void OnVehicleReverseInput(InputPhase phase, float input)
    {
        // Only process this input if reverse is enabled
        if (enableReverse)
        {
            reverseInput = input;
            
            // Additional phase-specific handling if needed
            if (phase == InputPhase.OnPressed)
            {
                Debug.Log($"Reverse started: {input}");
            }
        }
    }
    
    public void OnVehiclePrimaryActionInput(InputPhase phase)
    {
        // Only process this input if primary action is enabled
        if (enablePrimaryAction && phase == InputPhase.OnPressed)
        {
            Debug.Log("Primary action activated");
            ActivatePrimaryAction();
        }
    }
    
    public void OnVehicleSecondaryActionInput(InputPhase phase)
    {
        // Only process this input if secondary action is enabled
        if (enableSecondaryAction && phase == InputPhase.OnPressed)
        {
            Debug.Log("Secondary action activated");
            ActivateSecondaryAction();
        }
    }
    
    public void OnVehicleExitInput()
    {
        // Only process this input if exit is enabled
        if (enableExit)
        {
            Debug.Log("Exit vehicle requested");
            ExitVehicle();
        }
    }
    
    // IInputActionsListener implementation
    public void OnInputCaptureStarted(InputCaptureType captureType)
    {
        Debug.Log($"Input capture started: {captureType}");
    }
    
    public void OnInputCaptureStopped(InputCaptureType captureType)
    {
        Debug.Log($"Input capture stopped: {captureType}");
        
        // Reset input values
        steerInput = Vector2.zero;
        throttleInput = 0f;
        reverseInput = 0f;
    }
    
    // Custom actions implementation
    private void ActivatePrimaryAction()
    {
        // Example: Sound horn or activate lights
        Debug.Log("Primary action effect triggered");
    }
    
    private void ActivateSecondaryAction()
    {
        // Example: Fire weapon or activate special ability
        Debug.Log("Secondary action effect triggered");
    }
    
    private void ExitVehicle()
    {
        // Implementation for exiting the vehicle
        Debug.Log("Exiting vehicle");
        
        // Release input capture
        ReleaseVehicleInput();
        
        // Additional vehicle exit logic...
    }
    
    // Apply vehicle movement in FixedUpdate for physics-based movement
    private void FixedUpdate()
    {
        if (vehicleRigidbody != null)
        {
            // Only apply movement if we have a valid rigidbody
            ApplyVehicleMovement();
        }
    }
    
    private void ApplyVehicleMovement()
    {
        // Calculate net throttle (throttle minus reverse)
        float netThrottle = 0f;
        
        if (enableThrottle && enableReverse)
        {
            netThrottle = throttleInput - reverseInput;
        }
        else if (enableThrottle)
        {
            netThrottle = throttleInput;
        }
        else if (enableReverse)
        {
            netThrottle = -reverseInput;
        }
        
        // Apply forward/reverse force
        float forceAmount = netThrottle >= 0 
            ? netThrottle * maxThrottleForce 
            : Mathf.Abs(netThrottle) * maxReverseForce;
            
        if (Mathf.Abs(forceAmount) > 0.01f)
        {
            Vector3 forceDirection = vehicleObject.transform.forward * Mathf.Sign(netThrottle);
            vehicleRigidbody.AddForce(forceDirection * forceAmount, ForceMode.Force);
        }
        
        // Apply steering if enabled
        if (enableSteering && Mathf.Abs(steerInput.x) > 0.01f)
        {
            float steerAngle = steerInput.x * maxSteeringAngle;
            
            // Apply rotation (simplified example)
            Quaternion steerRotation = Quaternion.Euler(0, steerAngle * Time.fixedDeltaTime, 0);
            vehicleRigidbody.MoveRotation(vehicleRigidbody.rotation * steerRotation);
        }
    }
    
    // Toggle individual input flags for dynamic control configuration
    public void ToggleThrottleInput(bool enable)
    {
        enableThrottle = enable;
        // Restart input capture to apply changes
        ReleaseVehicleInput();
        StartVehicleInputCapture();
    }
    
    public void ToggleSteeringInput(bool enable)
    {
        enableSteering = enable;
        // Restart input capture to apply changes
        ReleaseVehicleInput();
        StartVehicleInputCapture();
    }
    
    public void ToggleReverseInput(bool enable)
    {
        enableReverse = enable;
        // Restart input capture to apply changes
        ReleaseVehicleInput();
        StartVehicleInputCapture();
    }
    
    public void TogglePrimaryActionInput(bool enable)
    {
        enablePrimaryAction = enable;
        // Restart input capture to apply changes
        ReleaseVehicleInput();
        StartVehicleInputCapture();
    }
    
    public void ToggleSecondaryActionInput(bool enable)
    {
        enableSecondaryAction = enable;
        // Restart input capture to apply changes
        ReleaseVehicleInput();
        StartVehicleInputCapture();
    }
    
    public void ToggleExitInput(bool enable)
    {
        enableExit = enable;
        // Restart input capture to apply changes
        ReleaseVehicleInput();
        StartVehicleInputCapture();
    }
}
```

## Best Practices

1. **Selective Flag Usage**: Only enable the input flags that your vehicle actually requires. This keeps the control scheme focused and prevents confusion.

2. **Input Type Consistency**: Keep the control scheme consistent with the vehicle type. For example, aircraft might use different controls than ground vehicles.

3. **Flag Combinations**: Use bitwise OR operations (`|`) to combine multiple flags when you need to capture several input types.

4. **Default Values**: Consider providing reasonable default flag configurations for common vehicle types to simplify setup.

5. **Dynamic Flag Adjustment**: Allow for runtime toggling of input flags to accommodate different vehicle states or damage conditions.

6. **UI Integration**: When using custom icons for throttle and brake controls, ensure they match the vehicle's theme and are clearly recognizable.

7. **Error Handling**: Implement appropriate error handling when starting input capture, as conflicts may arise with other input systems.

8. **Input Release**: Always release input capture when the vehicle is no longer active to ensure other systems can use input.

## Common Use Cases

1. **Ground Vehicles**: Typically use Throttle, Steer1D, Reverse, and Exit flags for basic car or truck controls.

2. **Combat Vehicles**: Add PrimaryAction and SecondaryAction for weapons or special abilities in addition to basic movement controls.

3. **Simplified Controls**: For beginner-friendly vehicles, you might only enable Throttle and Steer1D for simpler control.

4. **Advanced Vehicles**: High-performance or simulation vehicles might use all available flags for complex control systems.

5. **Vehicle Damage Simulation**: Dynamically disable certain input flags when vehicle parts are damaged (e.g., disabling Reverse when the transmission is damaged).

6. **Tutorial Progression**: Gradually enable additional input flags as players progress through vehicle tutorials.

7. **Accessibility Options**: Configure different flag combinations based on player accessibility needs or preferences.

8. **Vehicle Types**: Different vehicle categories (car, boat, aircraft, etc.) can use different flag combinations appropriate to their movement style.
