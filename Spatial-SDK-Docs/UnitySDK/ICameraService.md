# ICameraService

Camera Service Interface

Service for managing camera functionality in Spatial spaces. This service provides comprehensive control over camera behavior, including main camera state, player camera settings, effects, and targeting.

## Properties

### Camera Settings
| Property | Description |
| --- | --- |
| firstPersonFov | First person FOV |
| thirdPersonFov | Third person FOV |
| minZoomDistance | Min zoom range |
| maxZoomDistance | Max zoom range |
| zoomDistance | Current zoom level |
| thirdPersonOffset | Camera offset |
| rotationMode | Camera control type |

### Camera State
| Property | Description |
| --- | --- |
| position | World position |
| rotation | World rotation |
| forward | Forward direction |
| velocity | Movement speed |
| targetOverride | Override target |

### Camera Effects
| Property | Description |
| --- | --- |
| shakeAmplitude | Shake intensity |
| shakeFrequency | Shake speed |
| wobbleAmplitude | Wobble intensity |
| wobbleFrequency | Wobble speed |
| kickDecay | Kick return speed |

### Camera Control
| Property | Description |
| --- | --- |
| forceFirstPerson | Force FPS mode |
| lockCameraRotation | Lock rotation |
| virtualCameraBlendTime | Blend duration |

### XR Settings
| Property | Description |
| --- | --- |
| xrCameraMode | XR view mode |
| allowPlayerToSwitchXRCameraMode | Allow XR switching |

### Camera Matrices
| Property | Description |
| --- | --- |
| projectionMatrix | Projection matrix |
| worldToCameraMatrix | World to camera |
| cameraToWorldMatrix | Camera to world |

### Camera Properties
| Property | Description |
| --- | --- |
| rect | Camera rectangle |
| pixelWidth | Screen width |
| pixelHeight | Screen height |
| scaledPixelWidth | Scaled width |
| scaledPixelHeight | Scaled height |

## Methods

### Camera Control
| Method | Description |
| --- | --- |
| SetTargetOverride(...) | Override camera target |
| ClearTargetOverride() | Clear target override |

### Camera Effects
| Method | Description |
| --- | --- |
| Shake(Vector3) | Apply camera shake |
| Wobble(Vector3) | Apply camera wobble |
| Kick(Vector2) | Apply camera kick |

### Coordinate Transforms
| Method | Description |
| --- | --- |
| WorldToScreenPoint(...) | World to screen |
| ScreenToWorldPoint(...) | Screen to world |
| WorldToViewportPoint(...) | World to viewport |
| ViewportToWorldPoint(...) | Viewport to world |
| ScreenToViewportPoint(...) | Screen to viewport |
| ViewportToScreenPoint(...) | Viewport to screen |

### Ray Casting
| Method | Description |
| --- | --- |
| ScreenPointToRay(...) | Screen ray cast |
| ViewportPointToRay(...) | Viewport ray cast |

### Camera Utilities
| Method | Description |
| --- | --- |
| CopyFromMainCamera(...) | Copy camera settings |
| CalculateFrustumCorners(...) | Get frustum corners |
| CalculateFrustumPlanes() | Get frustum planes |
| CalculateObliqueMatrix(...) | Get oblique matrix |

### Stereo Methods
| Method | Description |
| --- | --- |
| GetStereoViewMatrix(...) | Get stereo view |
| GetStereoProjectionMatrix(...) | Get stereo projection |

## Usage Examples

```csharp
// Example: Camera Controller
public class CameraController : MonoBehaviour
{
    private ICameraService cameraService;
    private Dictionary<string, CameraState> cameraStates;
    private bool isInitialized;

    private class CameraState
    {
        public bool isActive;
        public float lastUpdateTime;
        public Dictionary<string, object> settings;
        public Dictionary<string, object> effects;
    }

    void Start()
    {
        cameraService = SpatialBridge.cameraService;
        cameraStates = new Dictionary<string, CameraState>();
        InitializeController();
    }

    private void InitializeController()
    {
        InitializeCameraSettings();
        InitializeCameraEffects();
    }

    private void InitializeCameraSettings()
    {
        cameraService.firstPersonFov = 80f;
        cameraService.thirdPersonFov = 60f;
        cameraService.minZoomDistance = 2f;
        cameraService.maxZoomDistance = 10f;
        cameraService.thirdPersonOffset = new Vector3(0f, 1.5f, 0f);
        cameraService.virtualCameraBlendTime = 0.5f;
    }

    private void InitializeCameraEffects()
    {
        cameraService.shakeAmplitude = 0.5f;
        cameraService.shakeFrequency = 10f;
        cameraService.wobbleAmplitude = 0.3f;
        cameraService.wobbleFrequency = 2f;
        cameraService.kickDecay = 5f;
    }

    public void SetCameraTarget(
        Transform target,
        SpatialCameraMode mode = SpatialCameraMode.ThirdPerson
    )
    {
        try
        {
            cameraService.SetTargetOverride(target, mode);
            InitializeCameraState("target", new Dictionary<string, object>
            {
                { "transform", target },
                { "mode", mode },
                { "startTime", Time.time }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error setting camera target: {e.Message}");
        }
    }

    public void ClearCameraTarget()
    {
        try
        {
            cameraService.ClearTargetOverride();
            
            if (cameraStates.TryGetValue("target", out var state))
            {
                state.isActive = false;
                state.settings["endTime"] = Time.time;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error clearing camera target: {e.Message}");
        }
    }

    public void ApplyCameraShake(
        Vector3 intensity,
        bool useWobble = false
    )
    {
        try
        {
            if (useWobble)
            {
                cameraService.Wobble(intensity);
            }
            else
            {
                cameraService.Shake(intensity);
            }

            var effectId = System.Guid.NewGuid().ToString();
            InitializeCameraState(effectId, new Dictionary<string, object>
            {
                { "type", useWobble ? "wobble" : "shake" },
                { "intensity", intensity },
                { "startTime", Time.time }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error applying camera shake: {e.Message}");
        }
    }

    public void ApplyCameraKick(
        Vector2 angle,
        float duration = 0.5f
    )
    {
        try
        {
            cameraService.kickDecay = 1f / duration;
            cameraService.Kick(angle);

            var effectId = System.Guid.NewGuid().ToString();
            InitializeCameraState(effectId, new Dictionary<string, object>
            {
                { "type", "kick" },
                { "angle", angle },
                { "duration", duration },
                { "startTime", Time.time }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error applying camera kick: {e.Message}");
        }
    }

    private void InitializeCameraState(
        string stateId,
        Dictionary<string, object> settings
    )
    {
        var state = new CameraState
        {
            isActive = true,
            lastUpdateTime = Time.time,
            settings = settings,
            effects = new Dictionary<string, object>()
        };

        cameraStates[stateId] = state;
    }

    private void UpdateCameraStates()
    {
        var time = Time.time;
        foreach (var state in cameraStates.Values)
        {
            if (!state.isActive)
                continue;

            var startTime = (float)state.settings["startTime"];
            var duration = state.settings.ContainsKey("duration") ? 
                (float)state.settings["duration"] : 0f;

            if (duration > 0f && time - startTime >= duration)
            {
                state.isActive = false;
                state.settings["endTime"] = time;
            }
        }
    }

    void Update()
    {
        UpdateCameraStates();
    }
}

// Example: Camera Effects Manager
public class CameraEffectsManager : MonoBehaviour
{
    private ICameraService cameraService;
    private Dictionary<string, EffectState> effectStates;
    private bool isInitialized;

    private class EffectState
    {
        public bool isActive;
        public float lastTriggerTime;
        public Dictionary<string, object> settings;
    }

    void Start()
    {
        cameraService = SpatialBridge.cameraService;
        effectStates = new Dictionary<string, EffectState>();
        InitializeEffects();
    }

    private void InitializeEffects()
    {
        InitializeEffect("impact", new Dictionary<string, object>
        {
            { "shakeIntensity", new Vector3(0.3f, 0.3f, 0.3f) },
            { "shakeDuration", 0.3f },
            { "cooldown", 0.1f }
        });

        InitializeEffect("explosion", new Dictionary<string, object>
        {
            { "wobbleIntensity", new Vector3(0.5f, 0.5f, 0.5f) },
            { "wobbleDuration", 1.0f },
            { "cooldown", 0.5f }
        });

        InitializeEffect("recoil", new Dictionary<string, object>
        {
            { "kickAngle", new Vector2(2f, 1f) },
            { "kickDuration", 0.2f },
            { "cooldown", 0.05f }
        });
    }

    private void InitializeEffect(
        string effectId,
        Dictionary<string, object> settings
    )
    {
        var state = new EffectState
        {
            isActive = true,
            lastTriggerTime = 0f,
            settings = settings
        };

        effectStates[effectId] = state;
    }

    public void TriggerImpact(float intensity = 1f)
    {
        if (!CanTriggerEffect("impact"))
            return;

        var state = effectStates["impact"];
        var settings = state.settings;
        
        var shakeIntensity = (Vector3)settings["shakeIntensity"] * intensity;
        cameraService.Shake(shakeIntensity);
        
        state.lastTriggerTime = Time.time;
    }

    public void TriggerExplosion(
        float intensity = 1f,
        bool addShake = true
    )
    {
        if (!CanTriggerEffect("explosion"))
            return;

        var state = effectStates["explosion"];
        var settings = state.settings;
        
        var wobbleIntensity = (Vector3)settings["wobbleIntensity"] * intensity;
        cameraService.Wobble(wobbleIntensity);
        
        if (addShake)
        {
            cameraService.Shake(wobbleIntensity * 0.5f);
        }
        
        state.lastTriggerTime = Time.time;
    }

    public void TriggerRecoil(float intensity = 1f)
    {
        if (!CanTriggerEffect("recoil"))
            return;

        var state = effectStates["recoil"];
        var settings = state.settings;
        
        var kickAngle = (Vector2)settings["kickAngle"] * intensity;
        var duration = (float)settings["kickDuration"];
        
        cameraService.kickDecay = 1f / duration;
        cameraService.Kick(kickAngle);
        
        state.lastTriggerTime = Time.time;
    }

    private bool CanTriggerEffect(string effectId)
    {
        if (!effectStates.TryGetValue(effectId, out var state))
            return false;

        if (!state.isActive)
            return false;

        var cooldown = (float)state.settings["cooldown"];
        return Time.time - state.lastTriggerTime >= cooldown;
    }
}
```

## Best Practices

1. Camera Control
   - Smooth transitions
   - Handle collisions
   - Validate targets
   - Track states

2. Camera Effects
   - Layer effects
   - Time durations
   - Handle cooldowns
   - Clean removal

3. Performance
   - Cache transforms
   - Batch updates
   - Handle timing
   - Optimize raycasts

4. Error Handling
   - Validate params
   - Handle failures
   - Recover states
   - Log issues

## Common Use Cases

1. Camera Modes
   - First person
   - Third person
   - Fixed camera
   - Orbit camera

2. Camera Effects
   - Screen shake
   - Impact wobble
   - Recoil kick
   - Smooth transitions

3. Camera Features
   - Target following
   - Collision avoidance
   - FOV control
   - Zoom control

4. Camera Systems
   - Combat camera
   - Cinematic camera
   - Security camera
   - Spectator camera
