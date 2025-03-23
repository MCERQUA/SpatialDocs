# FloatingTextAnimStyle

Category: VFX Service Related

Enum

Animation style options for floating text created by the VFX Service. These styles determine how floating text animates when displayed in the world.

## Values

| Value | Description |
| --- | --- |
| Simple | A straightforward animation style with minimal effects. Text appears and fades out with basic movement. |
| Bouncy | An animated style with exaggerated bounce effects. Text initially bounces up before settling and fading away. |
| Custom | Allows for customized animation using provided animation curves for scaling and movement. |

## Usage Examples

```csharp
// Example: Create floating text with different animation styles
using SpatialSys.UnitySDK;
using UnityEngine;

public class TextEffectsController : MonoBehaviour
{
    private void ShowGameplayEffects()
    {
        // Create simple floating text
        SpatialBridge.vfxService.CreateFloatingText(
            text: "+100 Points",
            style: FloatingTextAnimStyle.Simple,
            position: transform.position + Vector3.up,
            moveDirection: Vector3.up,
            color: Color.yellow,
            worldSpace: true
        );
        
        // Create bouncy damage number
        SpatialBridge.vfxService.CreateFloatingText(
            text: "-25",
            style: FloatingTextAnimStyle.Bouncy,
            position: transform.position + Vector3.up * 1.5f,
            moveDirection: Vector3.up,
            color: Color.red,
            worldSpace: true
        );
        
        // Create custom animated status effect
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
        
        SpatialBridge.vfxService.CreateFloatingText(
            text: "STUNNED",
            style: FloatingTextAnimStyle.Custom,
            position: transform.position + Vector3.up * 2f,
            moveDirection: Vector3.up * 0.5f,
            color: Color.cyan,
            worldSpace: true,
            scaleCurve: scaleCurve,
            moveCurve: moveCurve,
            duration: 2.0f
        );
    }
}
```

## Best Practices

1. **Animation Style Selection**
   - Use `Simple` for subtle notifications and frequent updates
   - Use `Bouncy` for important events that need to grab attention
   - Use `Custom` when precise control over animation behavior is required

2. **Performance Considerations**
   - Limit the number of concurrent floating text effects, especially with Custom styles
   - For high-frequency events like damage numbers, consider batching or throttling

3. **Visual Consistency**
   - Maintain consistent styles across similar types of information
   - Use specific styles to indicate different types of events (e.g., Bouncy for critical hits)

4. **Custom Animation Guidelines**
   - When using Custom style, design curves that match the game's visual aesthetic
   - Ensure custom animations are not too distracting or overly long
   - Test custom animations in different lighting and background conditions

## Common Use Cases

1. **Game Feedback**
   - Damage numbers for combat systems
   - Score and point indicators
   - Pickup notifications
   - Achievement unlocks

2. **Status Effects**
   - Buff/debuff indicators
   - Temporary state changes
   - Cooldown notifications
   - Resource gains/losses

3. **Player Guidance**
   - Tutorial hints
   - Objective updates
   - Directional indicators
   - Interaction prompts

4. **UI Enhancement**
   - Confirmation messages
   - Warning notifications
   - Timer countdowns
   - Progress updates

## Completed: March 9, 2025
