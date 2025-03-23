# Golf Course Driving

## Overview
The Golf Course Driving template provides a complete implementation of a golf cart driving experience within a beautifully designed golf course environment. It offers players the ability to navigate through a detailed course with realistic physics-based driving, multiplayer support, and integrated course features like flags, bunkers, and water hazards.

## Features
- **Detailed Golf Course Environment**: Complete with fairways, greens, bunkers, water hazards, and clubhouse
- **Realistic Golf Cart Physics**: Tuned vehicle physics system with appropriate acceleration, handling, and terrain response
- **Multiple Vehicle Support**: Implementation for multiple golf carts with multiplayer synchronization
- **Passenger System**: Support for driver and passenger roles with seamless transitions
- **Path Network**: Predefined cart paths with optional navigation assistance
- **Time of Day System**: Dynamic lighting with day/night cycle transitions
- **Environment Effects**: Weather system with rain and wind effects
- **Score Tracking**: Optional golf scoring system integration

## Included Components

### 1. Golf Cart Vehicle
The centerpiece of the template featuring:
- Fully rigged and animated golf cart model with interior details
- Physics-based driving system with terrain adaptation
- Detailed audio system with engine, suspension, and impact sounds
- Visual effects for tire marks, dust, and water splashes
- Enter/exit animations and passenger system
- Networked synchronization for multiplayer use
- First and third-person camera options
- Customizable colors and accessories

### 2. Golf Course Environment
A comprehensive golf course setting with:
- Complete 9-hole golf course with varied terrain
- Detailed fairways, roughs, greens, and sand bunkers
- Water hazards with interactive physics
- Clubhouse and maintenance structures
- Optimized landscaping with LOD support
- Environmental audio ambiance
- Navigation markers and course signage

### 3. Cart Path System
Smart navigation system including:
- Spline-based cart paths throughout the course
- Path detection and optional driving assistance
- Speed limit zones with automatic enforcement
- Directional guidance and wrong-way detection
- Intersection management for multiple vehicles
- Visual indicators for path recognition

### 4. Time and Weather Controller
Environment management system with:
- Dynamic time of day cycle with realistic lighting
- Weather transition system (clear, cloudy, rainy)
- Environment-responsive audio effects
- Visual effects for different weather conditions
- Performance optimization for all conditions

### 5. Golf Gameplay Integration
Optional golf game elements including:
- Ball physics interaction with vehicle
- Flag and hole interaction
- Score tracking integration
- Club selection and bag storage on cart
- Shot assistance from cart locations

## Integration with SDK Components
The template integrates with these key Spatial SDK components:

| SDK Component | Usage in Template |
|---------------|-------------------|
| SpatialBridge | Central access point for all services |
| [SpatialNetworkObject](../../SpatialSys/UnitySDK/SpatialNetworkObject.md) | Vehicle networking and synchronization |
| [SpatialVirtualCamera](../../SpatialSys/UnitySDK/SpatialVirtualCamera.md) | Camera transitions between views |
| [IActorService](../../SpatialSys/UnitySDK/IActorService.md) | Player management and assignment |
| [IInputService](../../SpatialSys/UnitySDK/IInputService.md) | Vehicle control inputs |
| [SpatialInteractable](../../SpatialSys/UnitySDK/SpatialInteractable.md) | Vehicle entry/exit interaction |
| [ISpaceContentService](../../SpatialSys/UnitySDK/ISpaceContentService.md) | Course object spawning and management |
| [IAudioService](../../SpatialSys/UnitySDK/IAudioService.md) | Environment and vehicle audio |
| [ICameraService](../../SpatialSys/UnitySDK/ICameraService.md) | Camera perspective switching |
| [SpatialMovementMaterialSurface](../../SpatialSys/UnitySDK/SpatialMovementMaterialSurface.md) | Surface-specific vehicle physics |

## When to Use
- Creating golf course or resort experiences
- Implementing casual driving games with beautiful environments
- Building multiplayer exploration experiences
- Developing tour guide or showcase applications
- Creating educational content about golf courses or landscape design
- Building relaxing driving simulators
- Implementing any vehicle-based experience that requires terrain variety

## Implementation Details

### Vehicle Controller Implementation

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class GolfCartController : SpatialNetworkBehaviour, IVehicleInputActionsListener
{
    [Header("Vehicle References")]
    [SerializeField] private WheelCollider[] driveWheels;
    [SerializeField] private WheelCollider[] steeringWheels;
    [SerializeField] private Transform[] wheelMeshes;
    [SerializeField] private Transform steeringWheelMesh;
    [SerializeField] private AudioSource engineAudioSource;
    [SerializeField] private AudioSource impactAudioSource;
    [SerializeField] private ParticleSystem[] dustParticles;
    
    [Header("Vehicle Settings")]
    [SerializeField] private float maxMotorTorque = 300f;
    [SerializeField] private float maxSteeringAngle = 25f;
    [SerializeField] private float maxBrakeTorque = 1000f;
    [SerializeField] private float idleEngineVolume = 0.2f;
    [SerializeField] private float maxEngineVolume = 0.6f;
    [SerializeField] private float maxSpeed = 6f; // Meters per second (~12 mph)

    [Header("Path Detection")]
    [SerializeField] private LayerMask cartPathLayer;
    [SerializeField] private float pathAssistStrength = 0.5f;
    [SerializeField] private bool autonomousPathFollowing = false;

    [SpatialNetworkVariable]
    private NetworkVariable<bool> isEngineRunning = new NetworkVariable<bool>(false);
    
    [SpatialNetworkVariable]
    private NetworkVariable<int> currentDriverActorNumber = new NetworkVariable<int>(-1);
    
    [SpatialNetworkVariable]
    private NetworkVariable<int> currentPassengerActorNumber = new NetworkVariable<int>(-1);
    
    private float currentSpeed = 0f;
    private float currentSteeringInput = 0f;
    private float currentMotorInput = 0f;
    private float currentBrakeInput = 0f;
    private bool isOnCartPath = false;
    private Vector3 cartPathDirection = Vector3.zero;
    
    private Rigidbody vehicleRigidbody;
    private SpatialInteractable interactable;

    private void Awake()
    {
        vehicleRigidbody = GetComponent<Rigidbody>();
        interactable = GetComponent<SpatialInteractable>();
    }
    
    private void Start()
    {
        SpatialBridge.inputService.RegisterInputActionsListener(this);
    }

    private void OnDestroy()
    {
        if (SpatialBridge.inputService != null)
        {
            SpatialBridge.inputService.UnregisterInputActionsListener(this);
        }
    }

    public void EnterAsDriver(int actorNumber)
    {
        if (SpatialBridge.spaceContentService.TryClaimOwnership(this) && 
            currentDriverActorNumber.Value == -1)
        {
            currentDriverActorNumber.Value = actorNumber;
            isEngineRunning.Value = true;
            
            if (actorNumber == SpatialBridge.actorService.localActor.actorNumber)
            {
                // Switch to first person vehicle camera
                SpatialBridge.cameraService.SetCameraMode(SpatialCameraMode.FirstPerson);
            }
        }
    }
    
    public void EnterAsPassenger(int actorNumber)
    {
        if (SpatialBridge.spaceContentService.TryClaimOwnership(this) && 
            currentPassengerActorNumber.Value == -1 &&
            currentDriverActorNumber.Value != actorNumber)
        {
            currentPassengerActorNumber.Value = actorNumber;
            
            if (actorNumber == SpatialBridge.actorService.localActor.actorNumber)
            {
                // Switch to passenger camera view
                SpatialBridge.cameraService.SetCameraMode(SpatialCameraMode.FirstPerson);
            }
        }
    }
    
    public void ExitVehicle(int actorNumber)
    {
        if (SpatialBridge.spaceContentService.TryClaimOwnership(this))
        {
            if (currentDriverActorNumber.Value == actorNumber)
            {
                currentDriverActorNumber.Value = -1;
                
                // If no driver, stop engine
                if (currentDriverActorNumber.Value == -1)
                {
                    isEngineRunning.Value = false;
                }
            }
            
            if (currentPassengerActorNumber.Value == actorNumber)
            {
                currentPassengerActorNumber.Value = -1;
            }
            
            if (actorNumber == SpatialBridge.actorService.localActor.actorNumber)
            {
                // Return to normal third person camera
                SpatialBridge.cameraService.SetCameraMode(SpatialCameraMode.ThirdPerson);
            }
        }
    }
    
    private void Update()
    {
        UpdateWheelVisuals();
        UpdateEngineAudio();
        UpdatePathDetection();
        UpdateDustParticles();
    }
    
    private void FixedUpdate()
    {
        if (isEngineRunning.Value && currentDriverActorNumber.Value != -1)
        {
            ApplyVehiclePhysics();
        }
        else
        {
            ApplyBrakeWhenStopped();
        }
    }
    
    private void ApplyVehiclePhysics()
    {
        float motorTorque = maxMotorTorque * currentMotorInput;
        float brakeTorque = maxBrakeTorque * currentBrakeInput;
        float steeringAngle = maxSteeringAngle * GetAdjustedSteeringInput();
        
        // Apply steering
        foreach (var wheel in steeringWheels)
        {
            wheel.steerAngle = steeringAngle;
        }
        
        // Apply motor and brake torque
        foreach (var wheel in driveWheels)
        {
            wheel.motorTorque = motorTorque;
            wheel.brakeTorque = brakeTorque;
        }
        
        // Calculate current speed (in m/s)
        currentSpeed = vehicleRigidbody.velocity.magnitude;
    }
    
    private float GetAdjustedSteeringInput()
    {
        if (isOnCartPath && pathAssistStrength > 0)
        {
            // Apply path assistance by blending user input with path direction
            Vector3 forwardOnPlane = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            Vector3 pathDirOnPlane = Vector3.ProjectOnPlane(cartPathDirection, Vector3.up).normalized;
            
            float angleToPath = Vector3.SignedAngle(forwardOnPlane, pathDirOnPlane, Vector3.up);
            float pathSteerInput = Mathf.Clamp(angleToPath / maxSteeringAngle, -1f, 1f);
            
            // Blend between user input and path guidance
            return Mathf.Lerp(currentSteeringInput, pathSteerInput, pathAssistStrength);
        }
        
        return currentSteeringInput;
    }
    
    private void ApplyBrakeWhenStopped()
    {
        foreach (var wheel in driveWheels)
        {
            wheel.motorTorque = 0;
            wheel.brakeTorque = maxBrakeTorque;
        }
    }
    
    private void UpdateWheelVisuals()
    {
        for (int i = 0; i < wheelMeshes.Length; i++)
        {
            WheelCollider collider = (i < 2) ? steeringWheels[i] : driveWheels[i - 2];
            Transform wheelMesh = wheelMeshes[i];
            
            Vector3 position;
            Quaternion rotation;
            collider.GetWorldPose(out position, out rotation);
            
            wheelMesh.position = position;
            wheelMesh.rotation = rotation;
        }
        
        // Update steering wheel rotation
        if (steeringWheelMesh != null)
        {
            steeringWheelMesh.localRotation = Quaternion.Euler(0, 0, -currentSteeringInput * 120f);
        }
    }
    
    private void UpdateEngineAudio()
    {
        if (isEngineRunning.Value)
        {
            float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);
            float targetVolume = Mathf.Lerp(idleEngineVolume, maxEngineVolume, speedRatio);
            engineAudioSource.volume = Mathf.Lerp(engineAudioSource.volume, targetVolume, Time.deltaTime * 3f);
            
            float targetPitch = Mathf.Lerp(0.8f, 1.2f, speedRatio);
            engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, targetPitch, Time.deltaTime * 3f);
            
            if (!engineAudioSource.isPlaying)
            {
                engineAudioSource.Play();
            }
        }
        else
        {
            if (engineAudioSource.isPlaying)
            {
                engineAudioSource.Stop();
            }
        }
    }
    
    private void UpdatePathDetection()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 2f, cartPathLayer))
        {
            isOnCartPath = true;
            
            // Get path direction from path spline or surface normal
            if (hit.collider.TryGetComponent<CartPathSpline>(out var pathSpline))
            {
                cartPathDirection = pathSpline.GetPathDirectionAtPoint(hit.point);
            }
            else
            {
                // Fallback: use forward direction projected on the surface
                Vector3 surfaceNormal = hit.normal;
                cartPathDirection = Vector3.ProjectOnPlane(transform.forward, surfaceNormal).normalized;
            }
        }
        else
        {
            isOnCartPath = false;
        }
    }
    
    private void UpdateDustParticles()
    {
        bool isMoving = currentSpeed > 0.5f;
        bool isOnDirt = false;
        
        // Check surface type
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
        {
            if (hit.collider.TryGetComponent<SpatialMovementMaterialSurface>(out var surface))
            {
                isOnDirt = surface.movementMaterial.name.Contains("Dirt") || 
                          surface.movementMaterial.name.Contains("Sand");
            }
        }
        
        // Enable dust particles only when moving on dirt
        foreach (var particles in dustParticles)
        {
            var emission = particles.emission;
            emission.enabled = isMoving && isOnDirt;
            
            if (isMoving && isOnDirt)
            {
                // Adjust emission rate based on speed
                var emissionRate = emission.rateOverTime;
                emissionRate.constant = Mathf.Lerp(5f, 20f, currentSpeed / maxSpeed);
            }
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 2f)
        {
            float volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 10f);
            impactAudioSource.volume = volume;
            impactAudioSource.Play();
        }
    }

    // IVehicleInputActionsListener implementation
    public void OnVehicleAccelerate(float value)
    {
        if (currentDriverActorNumber.Value == SpatialBridge.actorService.localActor.actorNumber)
        {
            currentMotorInput = value;
            currentBrakeInput = 0f;
        }
    }

    public void OnVehicleBrake(float value)
    {
        if (currentDriverActorNumber.Value == SpatialBridge.actorService.localActor.actorNumber)
        {
            currentBrakeInput = value;
            
            // If braking, stop accelerating
            if (value > 0.1f)
            {
                currentMotorInput = 0f;
            }
        }
    }

    public void OnVehicleSteering(float value)
    {
        if (currentDriverActorNumber.Value == SpatialBridge.actorService.localActor.actorNumber)
        {
            currentSteeringInput = value;
        }
    }
    
    public void OnVehicleHandbrake(bool active)
    {
        // Golf cart doesn't use handbrake functionality
    }
}
```

### Cart Path Spline Implementation

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

[RequireComponent(typeof(SplineComponent))]
public class CartPathSpline : MonoBehaviour
{
    [Header("Path Settings")]
    [SerializeField] private float pathWidth = 3f;
    [SerializeField] private bool isOneWay = false;
    [SerializeField] private float speedLimit = 5f; // in m/s
    
    [Header("Visualization")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color pathColor = new Color(0.7f, 0.7f, 0.7f);
    [SerializeField] private Color directionColor = Color.blue;
    
    private SplineComponent spline;
    
    private void Awake()
    {
        spline = GetComponent<SplineComponent>();
    }
    
    public Vector3 GetPathDirectionAtPoint(Vector3 worldPoint)
    {
        // Find the closest point on the spline
        float splineDistance;
        Vector3 closestPoint = spline.GetClosestPointOnSpline(worldPoint, out splineDistance);
        
        // Get the direction at that point
        return spline.GetTangentAtDistance(splineDistance);
    }
    
    public float GetSpeedLimitAtPoint(Vector3 worldPoint)
    {
        // Could be extended to allow variable speed limits along the path
        return speedLimit;
    }
    
    public bool IsOneWay()
    {
        return isOneWay;
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmos || spline == null) return;
        
        int segments = spline.PointCount * 10;
        float totalLength = spline.TotalLength;
        
        Gizmos.color = pathColor;
        
        for (int i = 0; i < segments; i++)
        {
            float distance1 = (i / (float)segments) * totalLength;
            float distance2 = ((i + 1) / (float)segments) * totalLength;
            
            Vector3 point1 = spline.GetPointAtDistance(distance1);
            Vector3 point2 = spline.GetPointAtDistance(distance2);
            
            Gizmos.DrawLine(point1, point2);
            
            // Draw direction indicators occasionally
            if (i % 20 == 0)
            {
                Vector3 tangent = spline.GetTangentAtDistance(distance1).normalized;
                Vector3 normal = Vector3.up;
                Vector3 binormal = Vector3.Cross(tangent, normal).normalized;
                
                Gizmos.color = directionColor;
                Gizmos.DrawLine(point1, point1 + tangent * 1f);
                
                // Draw path width indicators
                Gizmos.color = pathColor;
                Gizmos.DrawLine(point1 + binormal * pathWidth * 0.5f, 
                                point1 - binormal * pathWidth * 0.5f);
            }
        }
    }
}
```

### Time of Day System Implementation

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

[RequireComponent(typeof(SpatialNetworkObject))]
public class GolfCourseTimeController : SpatialNetworkBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private float dayLengthInMinutes = 20f;
    [SerializeField] private float startTimeHours = 8f; // 24-hour format
    
    [Header("Sky References")]
    [SerializeField] private Light sunLight;
    [SerializeField] private Material skyboxMaterial;
    [SerializeField] private Color daySkyColor = new Color(0.5f, 0.7f, 1f);
    [SerializeField] private Color nightSkyColor = new Color(0.05f, 0.05f, 0.1f);
    [SerializeField] private Color sunriseColor = new Color(1f, 0.5f, 0.3f);
    [SerializeField] private Color sunsetColor = new Color(1f, 0.3f, 0.2f);
    
    [Header("Lighting References")]
    [SerializeField] private Light[] nightLights;
    [SerializeField] private GameObject[] daytimeObjects;
    [SerializeField] private GameObject[] nighttimeObjects;
    
    [SpatialNetworkVariable]
    private NetworkVariable<float> currentTimeOfDay = new NetworkVariable<float>(0f); // 0-24 hours
    
    private float timeSpeed;
    private bool isServer;
    
    private void Start()
    {
        isServer = SpatialBridge.networkingService.isServer;
        timeSpeed = 24f / (dayLengthInMinutes * 60f);
        
        if (isServer)
        {
            currentTimeOfDay.Value = startTimeHours;
        }
        
        // Initialize lighting
        UpdateLighting();
    }
    
    private void Update()
    {
        if (isServer)
        {
            // Advance time
            currentTimeOfDay.Value = (currentTimeOfDay.Value + Time.deltaTime * timeSpeed) % 24f;
        }
        
        // Update lighting based on time
        UpdateLighting();
    }
    
    private void UpdateLighting()
    {
        float time = currentTimeOfDay.Value;
        
        // Sun rotation
        float sunRotation = (time - 6f) * 15f; // 15 degrees per hour, starting at 6am
        sunLight.transform.rotation = Quaternion.Euler(sunRotation, 170f, 0f);
        
        // Sun intensity based on time of day
        float dayNightTransition = Mathf.Clamp01(Mathf.Sin((time - 6f) / 12f * Mathf.PI));
        sunLight.intensity = Mathf.Lerp(0.1f, 1f, dayNightTransition);
        
        // Skybox color
        Color skyColor;
        if (time < 6f) // Night to sunrise
        {
            float t = Mathf.Clamp01((time + 0.5f) / 2f);
            skyColor = Color.Lerp(nightSkyColor, sunriseColor, t);
        }
        else if (time < 8f) // Sunrise to day
        {
            float t = Mathf.Clamp01((time - 6f) / 2f);
            skyColor = Color.Lerp(sunriseColor, daySkyColor, t);
        }
        else if (time < 17f) // Day
        {
            skyColor = daySkyColor;
        }
        else if (time < 19f) // Day to sunset
        {
            float t = Mathf.Clamp01((time - 17f) / 2f);
            skyColor = Color.Lerp(daySkyColor, sunsetColor, t);
        }
        else if (time < 21f) // Sunset to night
        {
            float t = Mathf.Clamp01((time - 19f) / 2f);
            skyColor = Color.Lerp(sunsetColor, nightSkyColor, t);
        }
        else // Night
        {
            skyColor = nightSkyColor;
        }
        
        skyboxMaterial.SetColor("_SkyTint", skyColor);
        
        // Night lights
        bool isNight = time < 6f || time > 19f;
        foreach (var light in nightLights)
        {
            light.enabled = isNight;
        }
        
        // Toggle day/night objects
        foreach (var obj in daytimeObjects)
        {
            obj.SetActive(!isNight);
        }
        
        foreach (var obj in nighttimeObjects)
        {
            obj.SetActive(isNight);
        }
    }
    
    // Public method to set the time of day manually (for admin controls)
    public void SetTimeOfDay(float timeHours)
    {
        if (isServer)
        {
            currentTimeOfDay.Value = Mathf.Clamp(timeHours, 0f, 24f);
        }
    }
}
```

## Best Practices
- **Optimize terrain textures**: Use texture atlases to reduce draw calls for the golf course terrain
- **Implement LOD system**: Create multiple detail levels for distant objects like trees and buildings
- **Use object pooling**: Pool common effects like water splashes and impact particles for better performance
- **Set up occlusion culling**: Occlusion culling is essential for the varied terrain of a golf course
- **Balance physics accuracy**: Fine-tune the golf cart physics for playability rather than perfect simulation
- **Precalculate path data**: Calculate path splines and guidance data at startup rather than at runtime
- **Apply surface-specific effects**: Use different effects and sounds based on the surface type (grass, sand, path)
- **Optimize for mobile**: Provide alternative control schemes and simplified visuals for mobile platforms
- **Focus on multiplayer sync**: Ensure smooth network synchronization of multiple vehicles on the course

## Related Templates
- [Simple Car Controller](./SimpleCarController.md) - For more advanced vehicle controls and physics
- [Camera Modes](../Camera/CameraModes.md) - For enhanced camera implementations with the vehicle
- [HTTP Request Demo](../Technical/HTTPRequestDemo.md) - For integrating online features like leaderboards

## Additional Resources
- [GitHub Repository](https://github.com/spatialsys/spatial-unity-sdk-examples) - For complete source code
- [Spatial Creator Toolkit](https://toolkit.spatial.io/templates) - For more template examples
- [Unity Vehicle Documentation](https://docs.unity3d.com/Manual/WheelColliderTutorial.html) - For further vehicle physics understanding
