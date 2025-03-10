# SpatialCameraPassthrough

Category: Core Components

Interface/Class/Enum: Class

The SpatialCameraPassthrough component allows GameObjects to be rendered in front of the camera without being occluded by other objects. This is useful for UI elements, special effects, or objects that should always be visible to the user regardless of other geometry in the scene.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| applyToChildren | bool | When enabled, the passthrough effect is applied to all child GameObjects as well as the GameObject this component is attached to. |
| documentationURL | string | URL to documentation for this component. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;

public class UIElementManager : MonoBehaviour
{
    [SerializeField] private GameObject uiContainer;
    [SerializeField] private GameObject[] specialEffects;
    
    private void Start()
    {
        // Ensure UI elements are always visible by adding camera passthrough
        ConfigureUIPassthrough();
        
        // Configure special effects to show through geometry
        ConfigureSpecialEffectsPassthrough();
    }
    
    private void ConfigureUIPassthrough()
    {
        if (!uiContainer.TryGetComponent<SpatialCameraPassthrough>(out var passthrough))
        {
            passthrough = uiContainer.AddComponent<SpatialCameraPassthrough>();
        }
        
        // Enable passthrough for all UI child elements
        passthrough.applyToChildren = true;
        
        Debug.Log("UI container configured with camera passthrough for all children");
    }
    
    private void ConfigureSpecialEffectsPassthrough()
    {
        foreach (GameObject effect in specialEffects)
        {
            if (!effect.TryGetComponent<SpatialCameraPassthrough>(out var passthrough))
            {
                passthrough = effect.AddComponent<SpatialCameraPassthrough>();
            }
            
            // For effects, decide whether to include children based on hierarchy
            bool hasImportantChildren = effect.transform.childCount > 0;
            passthrough.applyToChildren = hasImportantChildren;
            
            Debug.Log($"Special effect {effect.name} configured with camera passthrough (including children: {hasImportantChildren})");
        }
    }
}
```

## Best Practices

1. Use SpatialCameraPassthrough sparingly to avoid visual clutter and performance overhead.
2. Only apply to objects that genuinely need to be visible at all times.
3. Consider the visual impact when objects appear in front of everything else in the scene.
4. Be mindful of the hierarchy when using `applyToChildren = true` to avoid unintended objects being affected.
5. For complex UI hierarchies, consider adding SpatialCameraPassthrough to parent containers with `applyToChildren = true` rather than adding it to each individual element.
6. Test in different environments to ensure your passthrough objects don't create visual problems.

## Common Use Cases

1. UI elements that should always be visible to the user.
2. Pointer or cursor objects that need to appear in front of everything.
3. Important indicators or markers that should never be obscured.
4. Tutorial elements that guide the user through the space.
5. Special effects that need to be visible regardless of environment.
6. Critical gameplay elements that should not be occluded.

## Completed: March 09, 2025