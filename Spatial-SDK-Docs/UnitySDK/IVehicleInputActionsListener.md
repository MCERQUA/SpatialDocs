## Overview
Interface for listening to vehicle input capture events. This interface is used to receive input events when capturing vehicle controls through the Input Service. Only events overridden with `StartVehicleInputCapture` will be triggered.

## Methods

| Method | Description |
| --- | --- |
| OnVehicleSteerInput(InputPhase, Vector2) | Called when the user steers the vehicle. The Vector2 parameter represents the steering direction and magnitude. |
| OnVehicleThrottleInput(InputPhase, float) | Called when the user throttles the vehicle. The float parameter represents the throttle intensity (0 to 1). |
| OnVehicleReverseInput(InputPhase, float) | Called when the user reverses the vehicle. The float parameter represents the reverse intensity (0 to 1). |
| OnVehiclePrimaryActionInput(InputPhase) | Called when the user presses the primary action button. This can be used for vehicle-specific actions like horn, lights, or weapon systems. |
| OnVehicleSecondaryActionInput(InputPhase) | Called when the user presses the secondary action button. This can be used for additional vehicle-specific actions. |
| OnVehicleExitInput() | Called when the user presses the exit vehicle button. This is typically used to trigger the exit process for the vehicle. |

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

public class CustomVehicleController : MonoBehaviour, IVehicleInputActionsListener
{
    [SerializeField] private float maxThrottleForce = 20f;
    [SerializeField] private float maxReverseForce = 10f;
    [SerializeField] private float maxSteeringAngle = 30f;
    
    private Rigidbody vehicleRigidbody;
    private Transform vehicleTransform;
    
    // Input state tracking
    private Vector2 currentSteerInput;
    private float currentThrottleInput;
    private float currentReverseInput;
    private bool isPrimaryActionActive;
    private bool isSecondaryActionActive;
    
    private void Awake()
    {
        vehicleRigidbody = GetComponent<Rigidbody>();
        vehicleTransform = transform;
    }
    
    private void Start()
    {
        // Start capturing vehicle input with this listener
        VehicleInputFlags flags = VehicleInputFlags.Throttle | 
                                 VehicleInputFlags.Steer1D | 
                                 VehicleInputFlags.Reverse | 
                                 VehicleInputFlags.PrimaryAction | 
                                 VehicleInputFlags.SecondaryAction | 
                                 VehicleInputFlags.Exit;
                                 
        try
        {
            SpatialBridge.inputService.StartVehicleInputCapture(
                flags, 
                null,   // Accelerate icon (optional)
                null,   // Brake icon (optional)
                this
            );
            
            Debug.Log("Vehicle input capture started");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error starting vehicle input capture: {e.Message}");
        }
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
    
    // IVehicleInputActionsListener implementation
    public void OnVehicleSteerInput(InputPhase phase, Vector2 input)
    {
        currentSteerInput = input;
        
        Debug.Log($"Steer input: {input}, Phase: {phase}");
        
        // Additional phase-specific handling if needed
        switch (phase)
        {
            case InputPhase.OnPressed:
                // Handle initial steering input
                break;
                
            case InputPhase.OnHold:
                // Handle continuous steering
                break;
                
            case InputPhase.OnReleased:
                // Reset steering when released
                break;
        }
    }
    
    public void OnVehicleThrottleInput(InputPhase phase, float input)
    {
        currentThrottleInput = input;
        
        Debug.Log($"Throttle input: {input}, Phase: {phase}");
        
        // Additional phase-specific handling if needed
        switch (phase)
        {
            case InputPhase.OnPressed:
                // Start engine sound or particle effects
                break;
                
            case InputPhase.OnHold:
                // Continuous throttle effects
                break;
                
            case InputPhase.OnReleased:
                // Stop engine effects
                break;
        }
    }
    
    public void OnVehicleReverseInput(InputPhase phase, float input)
    {
        currentReverseInput = input;
        
        Debug.Log($"Reverse input: {input}, Phase: {phase}");
        
        // Additional phase-specific handling if needed
        switch (phase)
        {
            case InputPhase.OnPressed:
                // Start reverse indicator or sound
                break;
                
            case InputPhase.OnHold:
                // Continuous reverse effects
                break;
                
            case InputPhase.OnReleased:
                // Stop reverse effects
                break;
        }
    }
    
    public void OnVehiclePrimaryActionInput(InputPhase phase)
    {
        switch (phase)
        {
            case InputPhase.OnPressed:
                isPrimaryActionActive = true;
                Debug.Log("Primary action activated");
                // Activate primary action (e.g., horn, lights, boost)
                TriggerPrimaryAction();
                break;
                
            case InputPhase.OnHold:
                // Continuous primary action if applicable
                break;
                
            case InputPhase.OnReleased:
                isPrimaryActionActive = false;
                Debug.Log("Primary action deactivated");
                // Deactivate primary action if it's a toggle
                break;
        }
    }
    
    public void OnVehicleSecondaryActionInput(InputPhase phase)
    {
        switch (phase)
        {
            case InputPhase.OnPressed:
                isSecondaryActionActive = true;
                Debug.Log("Secondary action activated");
                // Activate secondary action (e.g., weapon, special ability)
                TriggerSecondaryAction();
                break;
                
            case InputPhase.OnHold:
                // Continuous secondary action if applicable
                break;
                
            case InputPhase.OnReleased:
                isSecondaryActionActive = false;
                Debug.Log("Secondary action deactivated");
                // Deactivate secondary action if it's a toggle
                break;
        }
    }
    
    public void OnVehicleExitInput()
    {
        Debug.Log("Exit vehicle requested");
        
        // Implement logic to exit the vehicle
        ExitVehicle();
    }
    
    // IInputActionsListener implementation
    public void OnInputCaptureStarted(InputCaptureType captureType)
    {
        Debug.Log($"Input capture started: {captureType}");
    }
    
    public void OnInputCaptureStopped(InputCaptureType captureType)
    {
        Debug.Log($"Input capture stopped: {captureType}");
        
        // Reset all input states
        currentSteerInput = Vector2.zero;
        currentThrottleInput = 0f;
        currentReverseInput = 0f;
        isPrimaryActionActive = false;
        isSecondaryActionActive = false;
    }
    
    // Implementation of vehicle actions
    private void TriggerPrimaryAction()
    {
        // Example: Sound horn
        Debug.Log("Horn sounded!");
        
        // Implementation details for your specific vehicle's primary action
    }
    
    private void TriggerSecondaryAction()
    {
        // Example: Toggle headlights
        Debug.Log("Headlights toggled!");
        
        // Implementation details for your specific vehicle's secondary action
    }
    
    private void ExitVehicle()
    {
        // Implementation for exiting the vehicle
        Debug.Log("Exiting vehicle...");
        
        // Release input capture before exiting
        try
        {
            SpatialBridge.inputService.ReleaseInputCapture(this);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error releasing input capture: {e.Message}");
        }
        
        // Additional exit logic
        // ...
    }
    
    // Use the captured input in FixedUpdate for physics-based movement
    private void FixedUpdate()
    {
        // Apply movement based on input
        ApplyVehicleMovement();
    }
    
    private void ApplyVehicleMovement()
    {
        // Calculate and apply forward/reverse force
        float netThrottle = currentThrottleInput - currentReverseInput;
        float forceAmount = netThrottle >= 0 
            ? netThrottle * maxThrottleForce 
            : netThrottle * maxReverseForce;
            
        Vector3 forceDirection = vehicleTransform.forward * forceAmount;
        vehicleRigidbody.AddForce(forceDirection, ForceMode.Force);
        
        // Apply steering based on input
        float steerAngle = currentSteerInput.x * maxSteeringAngle;
        
        // Apply rotation (simplified example)
        if (Mathf.Abs(steerAngle) > 0.1f)
        {
            Quaternion steerRotation = Quaternion.Euler(0, steerAngle * Time.fixedDeltaTime, 0);
            vehicleRigidbody.MoveRotation(vehicleRigidbody.rotation * steerRotation);
        }
    }
}
```

## Best Practices

1. **Complete Input Handling**: Implement all required interface methods, even if some won't be used, to ensure compatibility with the Input Service.

2. **Physics-Based Movement**: Apply vehicle movement forces in FixedUpdate rather than directly in the input callbacks for smooth, consistent physics behavior.

3. **Input State Tracking**: Cache input values from callbacks and use them in your physics calculations rather than applying forces directly in the callbacks.

4. **Proper Cleanup**: Always release input capture when your vehicle controller is disabled or destroyed to allow other systems to use input.

5. **Error Handling**: Implement try-catch blocks when interacting with the Input Service to handle potential errors gracefully.

6. **Input Configuration**: Carefully select which vehicle input flags to enable based on your vehicle's functionality to provide the appropriate control scheme.

7. **Phase-Specific Handling**: Use InputPhase to differentiate between initial press, continuous hold, and release actions for more responsive controls.

8. **Customization Support**: Consider providing options for input sensitivity, steering assistance, or other vehicle-specific settings to enhance the player experience.

## Common Use Cases

1. **Ground Vehicles**: Cars, trucks, and tanks with steering and throttle controls.

2. **Aircraft**: Planes and helicopters with more complex control schemes.

3. **Watercraft**: Boats and submarines with specialized movement characteristics.

4. **Special Vehicles**: Fantasy or futuristic vehicles with unique control mechanics.

5. **Vehicle Weapons Systems**: Using primary and secondary actions to control vehicle-mounted weapons.

6. **Multi-Mode Vehicles**: Vehicles that can switch between different movement modes (e.g., a car that can also fly).

7. **Vehicle Animations**: Triggering animations or effects based on vehicle input, such as tire tracks, engine particles, or suspension movements.

8. **Realistic Physics**: Creating realistic vehicle physics models with response to throttle, steering, and braking inputs.
