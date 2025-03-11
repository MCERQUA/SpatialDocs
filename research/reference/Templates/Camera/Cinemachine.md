# Cinemachine

## Overview
The Cinemachine template provides a complete implementation of Unity's Cinemachine camera system for Spatial experiences. It offers advanced camera controls, smooth transitions, procedural noise, and sophisticated framing techniques to create dynamic, cinematic camera behaviors. This template serves as a foundation for creating engaging visual experiences with professional camera work.

## Features
- **Virtual Camera System**: Multiple virtual cameras with priority-based activation
- **Smooth Camera Transitions**: Blended transitions between camera views with customizable parameters
- **Target Following**: Sophisticated target following with various follow modes
- **Camera Composition**: Advanced framing and composition rules
- **Procedural Noise**: Natural camera movement with configurable noise profiles
- **Multi-user Support**: Camera system optimized for multiplayer experiences
- **Timeline Integration**: Ready-to-use Timeline integration for scripted sequences
- **First and Third Person Views**: Pre-configured cameras for both perspectives
- **Performance Optimization**: Camera culling and optimization techniques for WebGL

## Included Components

### 1. Cinemachine Brain Setup
Core component that manages the virtual camera system:
- Properly configured CinemachineBrain component
- Spatial-optimized blending settings
- Performance-tuned update method selection
- Custom noise profiles for different scenarios
- Multiplayer synchronization support
- Cross-platform optimization settings

### 2. Virtual Camera Library
Collection of ready-to-use virtual cameras:
- Third-person follow camera with target offset
- First-person POV camera with head tracking
- Overhead camera with dynamic zoom
- Cinematic sequence cameras
- Dynamic look-at cameras for points of interest
- Orbital cameras for object inspection
- Flythrough cameras for environment showcase

### 3. Camera Rig Systems
Camera control structures for specific scenarios:
- Player-following rig with collision avoidance
- Conversation cameras for NPC interactions
- Object showcase rig with orbital control
- Environment flythrough system
- Action sequence camera system
- Custom input-driven camera control rig

### 4. Timeline Sequences
Pre-built cinematic sequences using Timeline:
- Environment introduction sequence
- Character showcase sequence
- Action sequence with multiple camera angles
- Dynamic transition sequences
- Spatial-optimized Timeline tracks
- Ready-to-use Timeline prefabs

### 5. Input-Driven Camera Controls
User control systems for cameras:
- Mouse orbit control implementation
- Touch-based camera controls for mobile
- Gamepad camera control system
- Keyboard camera adjustment controls
- Input sensitivity customization
- Platform-specific input handling

## Integration with SDK Components
The template integrates with these key Spatial SDK components:

| SDK Component | Usage in Template |
|---------------|-------------------|
| [ICameraService](../../SpatialSys/UnitySDK/ICameraService.md) | Base camera service integration and management |
| [SpatialVirtualCamera](../../SpatialSys/UnitySDK/SpatialVirtualCamera.md) | Virtual camera configuration and control |
| [SpatialCameraMode](../../SpatialSys/UnitySDK/SpatialCameraMode.md) | Camera mode handling and transitions |
| [SpatialCameraRotationMode](../../SpatialSys/UnitySDK/SpatialCameraRotationMode.md) | Camera rotation handling |
| [XRCameraMode](../../SpatialSys/UnitySDK/XRCameraMode.md) | XR camera mode integration |
| [IInputService](../../SpatialSys/UnitySDK/IInputService.md) | Camera input handling |
| [IActorService](../../SpatialSys/UnitySDK/IActorService.md) | Actor tracking and following |
| [IAvatarInputActionsListener](../../SpatialSys/UnitySDK/IAvatarInputActionsListener.md) | Avatar input for camera control |
| [SpatialNetworkObject](../../SpatialSys/UnitySDK/SpatialNetworkObject.md) | Networked camera state synchronization |

## When to Use
Use this template when:
- Creating cinematic experiences with professional camera work
- Building showcase spaces that require smooth camera movement
- Implementing interactive presentations with scripted camera sequences
- Developing games that need sophisticated camera behaviors
- Creating virtual tours with guided camera movement
- Building multiplayer experiences requiring consistent camera behavior
- Implementing conversation or dialogue systems with dynamic cameras
- Creating cutscenes or narrative sequences
- Designing product showcases with orbital inspection cameras

## Implementation Details

### Virtual Camera Configuration

```csharp
using UnityEngine;
using Cinemachine;
using SpatialSys.UnitySDK;

public class SpatialCinemachineManager : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private CinemachineBrain cinemachineBrain;
    [SerializeField] private CinemachineVirtualCamera thirdPersonCamera;
    [SerializeField] private CinemachineVirtualCamera firstPersonCamera;
    [SerializeField] private CinemachineVirtualCamera orbitCamera;
    [SerializeField] private CinemachineVirtualCamera cinematicCamera;
    
    [Header("Camera Settings")]
    [SerializeField] private float defaultBlendTime = 1.0f;
    [SerializeField] private CinemachineBlendDefinition.Style blendStyle = CinemachineBlendDefinition.Style.EaseInOut;
    [SerializeField] private bool useUnscaledTime = true;
    
    [Header("Camera Targets")]
    [SerializeField] private Transform defaultTarget;
    [SerializeField] private Transform lookAtTarget;
    
    private CinemachineVirtualCamera currentActiveCamera;
    
    private void Start()
    {
        // Initialize the Cinemachine brain
        if (cinemachineBrain == null)
        {
            cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
            if (cinemachineBrain == null)
            {
                cinemachineBrain = Camera.main.gameObject.AddComponent<CinemachineBrain>();
            }
        }
        
        // Configure the Cinemachine brain
        cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(
            blendStyle,
            defaultBlendTime
        );
        cinemachineBrain.m_UpdateMethod = useUnscaledTime ? 
            CinemachineBrain.UpdateMethod.LateUpdate : 
            CinemachineBrain.UpdateMethod.SmartUpdate;
        
        // Initially use the third person camera
        ActivateCamera(thirdPersonCamera);
        
        // Setup camera targets based on local player if defaultTarget is not set
        if (defaultTarget == null)
        {
            defaultTarget = SpatialBridge.actorService.localActor.avatar.transform;
        }
        
        // Configure cameras with initial targets
        SetupCameraTargets();
        
        // Subscribe to camera events from Spatial
        SpatialBridge.cameraService.onCameraModeChanged += OnSpatialCameraModeChanged;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from camera events
        if (SpatialBridge.cameraService != null)
        {
            SpatialBridge.cameraService.onCameraModeChanged -= OnSpatialCameraModeChanged;
        }
    }
    
    private void SetupCameraTargets()
    {
        // Configure the third person camera
        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.Follow = defaultTarget;
            thirdPersonCamera.LookAt = lookAtTarget != null ? lookAtTarget : defaultTarget;
            
            // Configure the third person camera body
            if (thirdPersonCamera.GetCinemachineComponent<CinemachineTransposer>() is CinemachineTransposer transposer)
            {
                transposer.m_FollowOffset = new Vector3(0, 1.5f, -4f);
                transposer.m_XDamping = 1f;
                transposer.m_YDamping = 1f;
                transposer.m_ZDamping = 1f;
            }
            
            // Configure the third person camera aim
            if (thirdPersonCamera.GetCinemachineComponent<CinemachineComposer>() is CinemachineComposer composer)
            {
                composer.m_TrackedObjectOffset = new Vector3(0, 0.5f, 0);
                composer.m_DeadZoneWidth = 0.1f;
                composer.m_DeadZoneHeight = 0.1f;
            }
        }
        
        // Configure the first person camera
        if (firstPersonCamera != null)
        {
            firstPersonCamera.Follow = defaultTarget;
            
            // Configure the first person camera body
            if (firstPersonCamera.GetCinemachineComponent<CinemachineTransposer>() is CinemachineTransposer transposer)
            {
                transposer.m_FollowOffset = new Vector3(0, 1.7f, 0);
                transposer.m_XDamping = 0f;
                transposer.m_YDamping = 0f;
                transposer.m_ZDamping = 0f;
            }
        }
        
        // Configure the orbit camera
        if (orbitCamera != null)
        {
            orbitCamera.Follow = defaultTarget;
            orbitCamera.LookAt = defaultTarget;
            
            // Configure the orbit camera body
            if (orbitCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>() is CinemachineOrbitalTransposer orbitalTransposer)
            {
                orbitalTransposer.m_XAxis.m_MaxSpeed = 200f;
                orbitalTransposer.m_XAxis.m_AccelTime = 0.1f;
                orbitalTransposer.m_XAxis.m_DecelTime = 0.1f;
                orbitalTransposer.m_FollowOffset = new Vector3(0, 1.5f, -4f);
            }
        }
    }
    
    private void OnSpatialCameraModeChanged(SpatialCameraMode newMode)
    {
        // Map Spatial camera modes to Cinemachine virtual cameras
        switch (newMode)
        {
            case SpatialCameraMode.ThirdPerson:
                ActivateCamera(thirdPersonCamera);
                break;
            case SpatialCameraMode.FirstPerson:
                ActivateCamera(firstPersonCamera);
                break;
            case SpatialCameraMode.Orbit:
                ActivateCamera(orbitCamera);
                break;
            // Add other camera modes as needed
            default:
                ActivateCamera(thirdPersonCamera);
                break;
        }
    }
    
    /// <summary>
    /// Activates a specific virtual camera by setting its priority higher than others
    /// </summary>
    public void ActivateCamera(CinemachineVirtualCamera camera)
    {
        if (camera == null) return;
        
        // Deactivate the current camera
        if (currentActiveCamera != null)
        {
            currentActiveCamera.Priority = 10;
        }
        
        // Activate the new camera
        camera.Priority = 20;
        currentActiveCamera = camera;
    }
    
    /// <summary>
    /// Sets a new follow target for the specified virtual camera
    /// </summary>
    public void SetCameraTarget(CinemachineVirtualCamera camera, Transform target)
    {
        if (camera == null || target == null) return;
        
        camera.Follow = target;
        
        // If this camera also uses LookAt, update that too
        if (camera.LookAt == null || camera.LookAt == camera.Follow)
        {
            camera.LookAt = target;
        }
    }
    
    /// <summary>
    /// Initiates a cinematic sequence with preset cameras
    /// </summary>
    public void StartCinematicSequence()
    {
        // Disable user input for cameras during cinematic
        SpatialBridge.inputService.SetInputEnabled(false);
        
        // Activate the cinematic camera
        ActivateCamera(cinematicCamera);
        
        // Restore regular camera control after sequence (example: 5 seconds)
        Invoke(nameof(EndCinematicSequence), 5f);
    }
    
    private void EndCinematicSequence()
    {
        // Re-enable user input
        SpatialBridge.inputService.SetInputEnabled(true);
        
        // Return to third person view
        ActivateCamera(thirdPersonCamera);
    }
}
```

### Camera Input Controller Implementation

```csharp
using UnityEngine;
using Cinemachine;
using SpatialSys.UnitySDK;

public class CinemachineInputController : MonoBehaviour, IInputActionsListener
{
    [Header("Camera References")]
    [SerializeField] private CinemachineVirtualCamera orbitCamera;
    [SerializeField] private CinemachineVirtualCamera firstPersonCamera;
    
    [Header("Input Settings")]
    [SerializeField] private float rotationSpeed = 300f;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoomDistance = 1f;
    [SerializeField] private float maxZoomDistance = 10f;
    [SerializeField] private bool invertY = false;
    
    // Current input values
    private Vector2 lookInput;
    private float zoomInput;
    
    // Current camera state
    private float currentOrbitAngle = 0f;
    private float currentDistance = 4f;
    
    private void Start()
    {
        // Register for input events
        SpatialBridge.inputService.RegisterInputActionsListener(this);
        
        // Initial setup
        if (orbitCamera != null && orbitCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>() is CinemachineOrbitalTransposer orbitalTransposer)
        {
            currentDistance = orbitalTransposer.m_FollowOffset.magnitude;
        }
    }
    
    private void OnDestroy()
    {
        // Unregister from input events
        if (SpatialBridge.inputService != null)
        {
            SpatialBridge.inputService.UnregisterInputActionsListener(this);
        }
    }
    
    private void Update()
    {
        // Only process input when input is enabled
        if (!SpatialBridge.inputService.isInputEnabled) return;
        
        // Process camera rotation input
        ProcessCameraRotation();
        
        // Process camera zoom input
        ProcessCameraZoom();
    }
    
    private void ProcessCameraRotation()
    {
        if (lookInput.sqrMagnitude < 0.01f) return;
        
        // Handle rotation for orbit camera
        if (orbitCamera != null && orbitCamera.Priority > 10)
        {
            if (orbitCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>() is CinemachineOrbitalTransposer orbitalTransposer)
            {
                // Update orbit angle
                currentOrbitAngle += lookInput.x * rotationSpeed * Time.deltaTime;
                orbitalTransposer.m_XAxis.Value = currentOrbitAngle;
                
                // Update vertical angle (Handled differently in Cinemachine - typically through a Composer)
                if (orbitCamera.GetCinemachineComponent<CinemachineComposer>() is CinemachineComposer composer)
                {
                    float yInput = invertY ? -lookInput.y : lookInput.y;
                    Vector3 trackedOffset = composer.m_TrackedObjectOffset;
                    trackedOffset.y += yInput * rotationSpeed * 0.01f * Time.deltaTime;
                    trackedOffset.y = Mathf.Clamp(trackedOffset.y, -1f, 2f);
                    composer.m_TrackedObjectOffset = trackedOffset;
                }
            }
        }
        
        // Handle rotation for first person camera
        if (firstPersonCamera != null && firstPersonCamera.Priority > 10)
        {
            // For first person camera, we typically rotate the target/avatar instead
            if (firstPersonCamera.Follow != null)
            {
                // Create a rotation for the horizontal movement
                Quaternion rotationDelta = Quaternion.Euler(0f, lookInput.x * rotationSpeed * Time.deltaTime, 0f);
                firstPersonCamera.Follow.rotation = rotationDelta * firstPersonCamera.Follow.rotation;
                
                // For vertical rotation, you might adjust the aim or use a FreeLook camera
                // This is simplified and would depend on your character controller
            }
        }
    }
    
    private void ProcessCameraZoom()
    {
        if (Mathf.Abs(zoomInput) < 0.01f) return;
        
        // Handle zoom for orbit camera
        if (orbitCamera != null && orbitCamera.Priority > 10)
        {
            if (orbitCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>() is CinemachineOrbitalTransposer orbitalTransposer)
            {
                // Update distance
                currentDistance = Mathf.Clamp(currentDistance - zoomInput * zoomSpeed * Time.deltaTime, 
                                             minZoomDistance, maxZoomDistance);
                
                // Apply the new distance while maintaining direction
                Vector3 followOffset = orbitalTransposer.m_FollowOffset.normalized * currentDistance;
                orbitalTransposer.m_FollowOffset = followOffset;
            }
        }
    }
    
    // IInputActionsListener Implementation
    
    public void OnLook(Vector2 delta)
    {
        lookInput = delta;
    }
    
    public void OnScroll(float delta)
    {
        zoomInput = delta;
    }
    
    // Additional methods to handle specific camera configurations
    
    public void SetOrbitTarget(Transform target)
    {
        if (orbitCamera != null && target != null)
        {
            orbitCamera.Follow = target;
            orbitCamera.LookAt = target;
        }
    }
    
    public void SetFirstPersonTarget(Transform target)
    {
        if (firstPersonCamera != null && target != null)
        {
            firstPersonCamera.Follow = target;
        }
    }
    
    public void ResetCameraPosition()
    {
        currentOrbitAngle = 0f;
        currentDistance = 4f;
        
        if (orbitCamera != null && orbitCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>() is CinemachineOrbitalTransposer orbitalTransposer)
        {
            orbitalTransposer.m_XAxis.Value = currentOrbitAngle;
            orbitalTransposer.m_FollowOffset = orbitalTransposer.m_FollowOffset.normalized * currentDistance;
        }
    }
}
```

### Cinematic Sequence Controller

```csharp
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using SpatialSys.UnitySDK;

public class CinematicSequenceController : SpatialNetworkBehaviour
{
    [Header("Timeline References")]
    [SerializeField] private PlayableDirector introSequence;
    [SerializeField] private PlayableDirector actionSequence;
    [SerializeField] private PlayableDirector conclusionSequence;
    
    [Header("Camera References")]
    [SerializeField] private CinemachineVirtualCamera defaultCamera;
    [SerializeField] private CinemachineVirtualCamera[] sequenceCameras;
    
    [Header("Sequence Settings")]
    [SerializeField] private bool playIntroOnStart = false;
    [SerializeField] private bool disablePlayerControlDuringSequence = true;
    [SerializeField] private float sequenceBlendTime = 1.0f;
    
    [SpatialNetworkVariable]
    private NetworkVariable<bool> isPlayingSequence = new NetworkVariable<bool>(false);
    
    private SpatialCinemachineManager cameraManager;
    private bool wasInputEnabled;
    
    private void Start()
    {
        cameraManager = FindObjectOfType<SpatialCinemachineManager>();
        
        // Setup timeline components
        SetupTimelineComponents();
        
        // Play intro sequence if configured
        if (playIntroOnStart && isServer)
        {
            Invoke(nameof(PlayIntroSequence), 1.0f);
        }
        
        // Subscribe to network variable changes
        isPlayingSequence.onValueChanged += OnSequenceStateChanged;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from network variable changes
        isPlayingSequence.onValueChanged -= OnSequenceStateChanged;
    }
    
    private void SetupTimelineComponents()
    {
        // Configure intro sequence
        if (introSequence != null)
        {
            introSequence.stopped += OnSequenceComplete;
            introSequence.playOnAwake = false;
        }
        
        // Configure action sequence
        if (actionSequence != null)
        {
            actionSequence.stopped += OnSequenceComplete;
            actionSequence.playOnAwake = false;
        }
        
        // Configure conclusion sequence
        if (conclusionSequence != null)
        {
            conclusionSequence.stopped += OnSequenceComplete;
            conclusionSequence.playOnAwake = false;
        }
    }
    
    private void OnSequenceStateChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            // Sequence is starting
            if (disablePlayerControlDuringSequence)
            {
                wasInputEnabled = SpatialBridge.inputService.isInputEnabled;
                SpatialBridge.inputService.SetInputEnabled(false);
            }
            
            // Ensure cinematic cameras are enabled
            foreach (var camera in sequenceCameras)
            {
                if (camera != null)
                {
                    camera.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            // Sequence is ending
            if (disablePlayerControlDuringSequence && wasInputEnabled)
            {
                SpatialBridge.inputService.SetInputEnabled(true);
            }
            
            // Return to default camera
            if (cameraManager != null && defaultCamera != null)
            {
                cameraManager.ActivateCamera(defaultCamera);
            }
            else
            {
                // Fallback: use Spatial camera service
                SpatialBridge.cameraService.SetCameraMode(SpatialCameraMode.ThirdPerson);
            }
        }
    }
    
    /// <summary>
    /// Plays the intro cinematic sequence
    /// </summary>
    public void PlayIntroSequence()
    {
        if (!SpatialBridge.spaceContentService.TryClaimOwnership(this)) return;
        
        StopAllSequences();
        isPlayingSequence.Value = true;
        
        if (introSequence != null)
        {
            introSequence.time = 0;
            introSequence.Play();
        }
    }
    
    /// <summary>
    /// Plays the action cinematic sequence
    /// </summary>
    public void PlayActionSequence()
    {
        if (!SpatialBridge.spaceContentService.TryClaimOwnership(this)) return;
        
        StopAllSequences();
        isPlayingSequence.Value = true;
        
        if (actionSequence != null)
        {
            actionSequence.time = 0;
            actionSequence.Play();
        }
    }
    
    /// <summary>
    /// Plays the conclusion cinematic sequence
    /// </summary>
    public void PlayConclusionSequence()
    {
        if (!SpatialBridge.spaceContentService.TryClaimOwnership(this)) return;
        
        StopAllSequences();
        isPlayingSequence.Value = true;
        
        if (conclusionSequence != null)
        {
            conclusionSequence.time = 0;
            conclusionSequence.Play();
        }
    }
    
    /// <summary>
    /// Stops all cinematic sequences and returns to default camera
    /// </summary>
    public void StopAllSequences()
    {
        if (!SpatialBridge.spaceContentService.TryClaimOwnership(this)) return;
        
        if (introSequence != null) introSequence.Stop();
        if (actionSequence != null) actionSequence.Stop();
        if (conclusionSequence != null) conclusionSequence.Stop();
        
        isPlayingSequence.Value = false;
    }
    
    private void OnSequenceComplete(PlayableDirector director)
    {
        if (!SpatialBridge.spaceContentService.TryClaimOwnership(this)) return;
        
        // If no other sequence is playing, set the state to not playing
        if ((introSequence == null || !introSequence.state.Equals(PlayState.Playing)) &&
            (actionSequence == null || !actionSequence.state.Equals(PlayState.Playing)) &&
            (conclusionSequence == null || !conclusionSequence.state.Equals(PlayState.Playing)))
        {
            isPlayingSequence.Value = false;
        }
    }
    
    /// <summary>
    /// Sets active camera during timeline playback
    /// This method can be called from Timeline signals
    /// </summary>
    public void SetActiveCamera(int cameraIndex)
    {
        if (cameraIndex < 0 || cameraIndex >= sequenceCameras.Length) return;
        
        if (cameraManager != null)
        {
            cameraManager.ActivateCamera(sequenceCameras[cameraIndex]);
        }
    }
}
```

## Best Practices
- **Optimize Camera Count**: Keep active virtual cameras to a minimum for better performance
- **Use Camera Priority**: Manage camera transitions via priority rather than enabling/disabling cameras
- **Configure Update Methods**: Use SmartUpdate for better performance in most cases
- **Manage Blending**: Set appropriate blend times based on the emotional context of transitions
- **Adjust Damping Values**: Fine-tune damping for different platforms (higher for mobile)
- **Control Camera Memory Usage**: Use the object pooling system for complex camera setups
- **Optimize for WebGL**: Simplify noise profiles and reduce camera components for WebGL builds
- **Share Camera Targets**: Use the same camera target references where possible
- **Use Clear Flags Efficiently**: Set camera clear flags to depth only when appropriate
- **Apply Culling Masks**: Use culling masks to optimize rendering for each camera
- **Balance Look Ahead**: Configure appropriate look ahead time for predictive following
- **Test with Multiple Users**: Ensure camera behavior works consistently across multiplayer sessions

## Related Templates
- [Camera Modes](./CameraModes.md) - For simpler camera perspective implementations
- [Simple Car Controller](../Vehicles/SimpleCarController.md) - For integration with vehicle camera systems
- [GPU Particles](../Technical/GPUParticles.md) - For optimized visual effects visible from cameras

## Additional Resources
- [GitHub Repository](https://github.com/spatialsys/spatial-unity-sdk-examples) - For complete source code
- [Unity Cinemachine Documentation](https://docs.unity3d.com/Packages/com.unity.cinemachine@2.8/manual/index.html) - For Cinemachine reference
- [Spatial Creator Toolkit](https://toolkit.spatial.io/templates) - For more template examples
