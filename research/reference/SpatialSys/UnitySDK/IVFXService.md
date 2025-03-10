# IVFXService

VFX Service Interface

Service for managing visual effects in Spatial spaces. This service provides functionality for creating and controlling various visual effects to enhance the user experience.

## Methods

### Visual Effects
| Method | Description |
| --- | --- |
| CreateFloatingText(...) | Create floating text VFX |

## Usage Examples

```csharp
// Example: VFX Manager
public class VFXManager : MonoBehaviour
{
    private IVFXService vfxService;
    private Dictionary<string, VFXState> vfxStates;
    private bool isInitialized;

    private class VFXState
    {
        public bool isActive;
        public float createTime;
        public Dictionary<string, object> parameters;
        public Dictionary<string, object> metadata;
    }

    void Start()
    {
        vfxService = SpatialBridge.vfxService;
        vfxStates = new Dictionary<string, VFXState>();
        InitializeManager();
    }

    private void InitializeManager()
    {
        InitializeAnimationCurves();
    }

    public void ShowFloatingText(
        string text,
        Vector3 position,
        Color color,
        FloatingTextAnimStyle style = FloatingTextAnimStyle.Bouncy,
        Vector3? direction = null,
        bool worldSpace = true,
        float duration = 1.0f
    )
    {
        try
        {
            var moveDir = direction ?? Vector3.up;
            var vfxId = System.Guid.NewGuid().ToString();

            var scaleCurve = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0f, 0f, 0f, 2f),
                new Keyframe(0.2f, 1.2f, 0f, 0f),
                new Keyframe(0.3f, 1f, 0f, 0f),
                new Keyframe(0.8f, 1f, 0f, 0f),
                new Keyframe(1f, 0f, -2f, 0f)
            });

            var moveCurve = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0f, 0f, 0f, 1f),
                new Keyframe(0.3f, 0.5f, 1f, 1f),
                new Keyframe(1f, 1f, 0f, 0f)
            });

            vfxService.CreateFloatingText(
                text,
                style,
                position,
                moveDir,
                color,
                worldSpace,
                scaleCurve,
                moveCurve,
                duration
            );

            InitializeVFXState(vfxId, new Dictionary<string, object>
            {
                { "text", text },
                { "position", position },
                { "color", color },
                { "style", style },
                { "direction", moveDir },
                { "duration", duration }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error creating floating text: {e.Message}");
        }
    }

    public void ShowDamageNumber(
        int damage,
        Vector3 position,
        bool isCritical = false
    )
    {
        var color = isCritical ? Color.red : Color.white;
        var scale = isCritical ? 1.5f : 1.0f;
        var text = isCritical ? $"CRIT {damage}!" : damage.ToString();
        var style = isCritical ? 
            FloatingTextAnimStyle.Bouncy : 
            FloatingTextAnimStyle.Smooth;

        ShowFloatingText(
            text,
            position + Vector3.up * scale,
            color,
            style,
            Vector3.up * scale,
            true,
            isCritical ? 1.5f : 1.0f
        );
    }

    public void ShowHealNumber(
        int healing,
        Vector3 position
    )
    {
        ShowFloatingText(
            $"+{healing}",
            position + Vector3.up,
            Color.green,
            FloatingTextAnimStyle.Smooth,
            Vector3.up,
            true,
            1.0f
        );
    }

    public void ShowStatusEffect(
        string status,
        Vector3 position,
        Color color
    )
    {
        ShowFloatingText(
            status,
            position + Vector3.up * 1.5f,
            color,
            FloatingTextAnimStyle.FadeOut,
            Vector3.up * 0.5f,
            true,
            2.0f
        );
    }

    private void InitializeVFXState(
        string vfxId,
        Dictionary<string, object> parameters
    )
    {
        var state = new VFXState
        {
            isActive = true,
            createTime = Time.time,
            parameters = parameters,
            metadata = new Dictionary<string, object>
            {
                { "createTime", Time.time },
                { "updateCount", 0 },
                { "lastUpdateTime", Time.time }
            }
        };

        vfxStates[vfxId] = state;
    }

    private void InitializeAnimationCurves()
    {
        // Store common animation curves for reuse
        animationCurves = new Dictionary<string, AnimationCurve>
        {
            ["bounce"] = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0f, 0f, 0f, 2f),
                new Keyframe(0.2f, 1.2f, 0f, 0f),
                new Keyframe(0.3f, 1f, 0f, 0f),
                new Keyframe(0.8f, 1f, 0f, 0f),
                new Keyframe(1f, 0f, -2f, 0f)
            }),
            
            ["smooth"] = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0f, 0f, 0f, 1f),
                new Keyframe(0.3f, 1f, 1f, 0f),
                new Keyframe(0.7f, 1f, 0f, -1f),
                new Keyframe(1f, 0f, -1f, 0f)
            }),
            
            ["fadeOut"] = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0f, 1f, 0f, 0f),
                new Keyframe(0.7f, 1f, 0f, -1f),
                new Keyframe(1f, 0f, -1f, 0f)
            })
        };
    }
}

// Example: Combat VFX Controller
public class CombatVFXController : MonoBehaviour
{
    private VFXManager vfxManager;
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
        vfxManager = GetComponent<VFXManager>();
        effectStates = new Dictionary<string, EffectState>();
        InitializeEffects();
    }

    private void InitializeEffects()
    {
        InitializeEffect("damage", new Dictionary<string, object>
        {
            { "minScale", 1.0f },
            { "maxScale", 1.5f },
            { "criticalThreshold", 100 },
            { "criticalMultiplier", 1.5f },
            { "cooldown", 0.1f }
        });

        InitializeEffect("heal", new Dictionary<string, object>
        {
            { "scale", 1.0f },
            { "duration", 1.0f },
            { "cooldown", 0.2f }
        });

        InitializeEffect("status", new Dictionary<string, object>
        {
            { "scale", 1.2f },
            { "duration", 2.0f },
            { "cooldown", 1.0f }
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

    public void ShowDamageEffect(
        int damage,
        Vector3 position,
        bool allowCritical = true
    )
    {
        if (!CanTriggerEffect("damage"))
            return;

        var state = effectStates["damage"];
        var settings = state.settings;
        
        var isCritical = allowCritical && 
            damage >= (int)settings["criticalThreshold"];
        
        vfxManager.ShowDamageNumber(damage, position, isCritical);
        state.lastTriggerTime = Time.time;
    }

    public void ShowHealEffect(
        int healing,
        Vector3 position
    )
    {
        if (!CanTriggerEffect("heal"))
            return;

        var state = effectStates["heal"];
        vfxManager.ShowHealNumber(healing, position);
        state.lastTriggerTime = Time.time;
    }

    public void ShowStatusEffect(
        string status,
        Vector3 position,
        Color color
    )
    {
        if (!CanTriggerEffect("status"))
            return;

        var state = effectStates["status"];
        vfxManager.ShowStatusEffect(status, position, color);
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

1. Effect Management
   - Cache curves
   - Pool effects
   - Handle timing
   - Track states

2. Performance
   - Limit effects
   - Batch updates
   - Optimize curves
   - Handle cleanup

3. Visual Quality
   - Consistent style
   - Clear feedback
   - Proper scaling
   - Color coding

4. Error Handling
   - Validate params
   - Handle failures
   - Cleanup resources
   - Log issues

## Common Use Cases

1. Combat Effects
   - Damage numbers
   - Heal indicators
   - Status effects
   - Critical hits

2. UI Feedback
   - Floating text
   - Notifications
   - Highlights
   - Transitions

3. Environmental VFX
   - Particle systems
   - Trail effects
   - Impact effects
   - Ambient effects

4. Character Effects
   - Ability effects
   - Status indicators
   - Movement trails
   - Interaction feedback
