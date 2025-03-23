# Simple Car Controller Template

## Overview
The Simple Car Controller template demonstrates how to integrate a third-party vehicle controller from the Unity Asset Store into Spatial. While this specific template uses the "Simple Car Controller" asset, the integration techniques can be applied to other vehicle controller assets or custom-built vehicle systems. The template provides a framework for vehicle input handling, camera integration, and network synchronization.

## Features
- **Vehicle Integration**: Complete implementation of a third-party vehicle controller
- **Input Handling**: Properly maps Spatial input to vehicle controls
- **Camera System**: Vehicle-specific camera setup with multiple views
- **Multiplayer Support**: Synchronized vehicle movement across all clients
- **Enter/Exit System**: Avatar-to-vehicle transition system
- **Audio Integration**: Engine and tire sounds integrated with the physics system
- **Visual Feedback**: Visual effects for tire skids, exhausts, and damage

## Included Components

### 1. Vehicle Input System
Maps Spatial input to vehicle controls:
- Accelerator and brake inputs
- Steering controls
- Gear shifting (if applicable)
- Handbrake functionality
- Special function triggers (horn, lights, etc.)

### 2. Vehicle Camera System
Provides specialized camera modes for vehicles:
- Third-person follow camera
- First-person interior view
- Orbit camera for vehicle inspection
- Cinematic chase camera
- Smooth transitions between camera modes

### 3. Vehicle Physics Wrapper
Connects the third-party physics system to Spatial:
- Adapts physics calculations to work with Spatial's environment
- Optimizes performance for web deployment
- Ensures consistent behavior across platforms
- Handles collisions with Spatial objects

### 4. Network Synchronization
Ensures all players see the same vehicle state:
- Position and rotation synchronization
- Vehicle state synchronization (gear, speed, etc.)
- Visual effect synchronization (lights, damage, etc.)
- Ownership handling for driver controls

### 5. Avatar Integration
Manages the relationship between avatars and vehicles:
- Avatar positioning in driver seat
- Avatar animation while driving
- Enter/exit transitions
- Avatar visibility within vehicle

## Integration with SDK Components
The template integrates with these key SDK components:

| SDK Component | Usage in Template |
|---------------|-------------------|
| IInputService | Capturing vehicle input controls |
| IVehicleInputActionsListener | Interface for vehicle input handling |
| VehicleInputFlags | Mapping specific vehicle input actions |
| ICameraService | Managing vehicle-specific cameras |
| SpatialVirtualCamera | Implementing custom vehicle camera views |
| SpatialNetworkObject | Synchronizing vehicle state across the network |
| SpatialNetworkVariables | Tracking synchronized vehicle properties |
| SpatialTriggerEvent | For vehicle entry/exit zones |

## When to Use
Use this template when:
- Implementing any type of vehicle in your Spatial project
- Integrating a third-party vehicle controller from the Asset Store
- Creating a custom vehicle system that needs proper Spatial integration
- Building experiences where vehicles are a central gameplay element
- Designing multiplayer vehicle-based games or experiences

## Implementation Details

### Vehicle Integration Script
The core integration script that connects the third-party controller to Spatial:

```csharp
public class Spatial_SCC : MonoBehaviour, IVehicleInputActionsListener
{
    [SerializeField] private SimpleCC_Car carController;
    [SerializeField] private SpatialVirtualCamera thirdPersonCamera;
    [SerializeField] private Transform driverSeatTransform;
    
    [SpatialNetworkVariable]
    private NetworkVariable<float> steering = new NetworkVariable<float>(0f);
    
    [SpatialNetworkVariable]
    private NetworkVariable<float> throttle = new NetworkVariable<float>(0f);
    
    [SpatialNetworkVariable]
    private NetworkVariable<bool> braking = new NetworkVariable<bool>(false);
    
    private bool isPlayerInVehicle = false;
    private Transform originalAvatarParent;
    private Vector3 originalAvatarPosition;
    private Quaternion originalAvatarRotation;
    
    public void OnVehicleInputUpdate(VehicleInputFlags flags)
    {
        // Only process input when player is in vehicle and we own the vehicle
        if (!isPlayerInVehicle || !SpatialBridge.spaceContentService.IsOwner(gameObject))
            return;
        
        // Process steering input
        if ((flags & VehicleInputFlags.Left) != 0)
            steering.Value = -1f;
        else if ((flags & VehicleInputFlags.Right) != 0)
            steering.Value = 1f;
        else
            steering.Value = 0f;
        
        // Process throttle/brake input
        if ((flags & VehicleInputFlags.Forward) != 0)
        {
            throttle.Value = 1f;
            braking.Value = false;
        }
        else if ((flags & VehicleInputFlags.Backward) != 0)
        {
            throttle.Value = 0f;
            braking.Value = true;
        }
        else
        {
            throttle.Value = 0f;
            braking.Value = false;
        }
    }
    
    private void Update()
    {
        // Apply network variables to car controller
        if (carController != null)
        {
            carController.SetInput(steering.Value, throttle.Value, braking.Value);
        }
    }
    
    public void EnterVehicle()
    {
        if (isPlayerInVehicle)
            return;
            
        var localAvatar = SpatialBridge.actorService.localActor.avatar;
        
        // Request ownership of the vehicle
        if (SpatialBridge.spaceContentService.TryClaimOwnership(gameObject))
        {
            // Store original avatar state
            originalAvatarParent = localAvatar.transform.parent;
            originalAvatarPosition = localAvatar.transform.position;
            originalAvatarRotation = localAvatar.transform.rotation;
            
            // Disable player movement and position in car
            localAvatar.visibleRemotely = false;
            localAvatar.transform.parent = driverSeatTransform;
            localAvatar.transform.localPosition = Vector3.zero;
            localAvatar.transform.localRotation = Quaternion.identity;
            
            // Activate vehicle camera
            SpatialBridge.cameraService.SetVirtualCamera(thirdPersonCamera);
            
            isPlayerInVehicle = true;
        }
    }
    
    public void ExitVehicle()
    {
        if (!isPlayerInVehicle)
            return;
            
        var localAvatar = SpatialBridge.actorService.localActor.avatar;
        
        // Reset avatar to original state
        localAvatar.transform.parent = originalAvatarParent;
        localAvatar.transform.position = transform.position + transform.right * 2f;
        localAvatar.transform.rotation = originalAvatarRotation;
        localAvatar.visibleRemotely = true;
        
        // Reset inputs
        steering.Value = 0f;
        throttle.Value = 0f;
        braking.Value = false;
        
        // Return to default camera
        SpatialBridge.cameraService.ResetVirtualCamera();
        
        isPlayerInVehicle = false;
    }
}
```

### Vehicle Entry Trigger
Example implementation of a trigger zone for entering vehicles:

```csharp
public class VehicleEntryTrigger : MonoBehaviour
{
    [SerializeField] private Spatial_SCC vehicleController;
    [SerializeField] private string promptText = "Press E to enter vehicle";
    
    private bool isPlayerInTrigger = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (IsLocalAvatar(other.gameObject))
        {
            isPlayerInTrigger = true;
            SpatialBridge.coreGUIService.DisplayInteractionPrompt(promptText);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (IsLocalAvatar(other.gameObject))
        {
            isPlayerInTrigger = false;
            SpatialBridge.coreGUIService.HideInteractionPrompt();
        }
    }
    
    private void Update()
    {
        if (isPlayerInTrigger && SpatialBridge.inputService.GetKeyDown(KeyCode.E))
        {
            vehicleController.EnterVehicle();
            SpatialBridge.coreGUIService.HideInteractionPrompt();
        }
    }
    
    private bool IsLocalAvatar(GameObject obj)
    {
        if (obj.TryGetComponent<SpatialAvatar>(out var avatar))
        {
            return avatar.actorId == SpatialBridge.actorService.localActor.actorNumber;
        }
        return false;
    }
}
```

### Vehicle Camera System
Example of the vehicle camera implementation:

```csharp
public class VehicleCameraController : MonoBehaviour
{
    [SerializeField] private SpatialVirtualCamera thirdPersonCamera;
    [SerializeField] private SpatialVirtualCamera firstPersonCamera;
    [SerializeField] private SpatialVirtualCamera orbitCamera;
    
    private int currentCameraIndex = 0;
    private SpatialVirtualCamera[] cameras;
    
    private void Start()
    {
        cameras = new SpatialVirtualCamera[] 
        {
            thirdPersonCamera,
            firstPersonCamera,
            orbitCamera
        };
        
        // Make sure only one camera is active initially
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(i == currentCameraIndex);
        }
    }
    
    public void SwitchCamera()
    {
        // Deactivate current camera
        cameras[currentCameraIndex].gameObject.SetActive(false);
        
        // Switch to next camera
        currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;
        
        // Activate new camera
        cameras[currentCameraIndex].gameObject.SetActive(true);
        SpatialBridge.cameraService.SetVirtualCamera(cameras[currentCameraIndex]);
    }
}
```

## Best Practices
- **Choose the Right Controller**: Select a vehicle controller that performs well in WebGL
- **Optimize Physics**: Reduce physics complexity for better web performance
- **Camera Tuning**: Adjust camera settings for smooth vehicle following
- **Input Buffering**: Implement input smoothing for better control
- **Audio Performance**: Use fewer, more optimized audio sources
- **Vehicle Exit Safety**: Ensure avatars have a safe spawn position when exiting vehicles
- **Visual Optimization**: Use LODs and optimize vehicle models for web

## Related Templates
- [Camera Modes](../Camera/CameraModes.md) - For additional camera perspectives
- [Golf Course Driving](./GolfCourseDriving.md) - For another vehicle implementation example
- [Avatar Input Control](../Technical/AvatarInputControl.md) - For advanced input handling

## Additional Resources
- [GitHub Repository](https://github.com/spatialsys/spatial-car-controller-scc)
- [Live Demo](https://www.spatial.io/s/SCC-Spatial-65ea444799b626e200d4cacb?share=5933831193146589842)
- [Simple Car Controller Asset](https://assetstore.unity.com/packages/tools/physics/simple-car-controller-258020) (Unity Asset Store)
- [Spatial Vehicle Documentation](https://toolkit.spatial.io/docs/vehicles) (Spatial Creator Toolkit)
- [IVehicleInputActionsListener Documentation](../../SpatialSys/UnitySDK/IVehicleInputActionsListener.md)
