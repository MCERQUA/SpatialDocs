# SpatialAvatarDefaultAnimSetType

Category: Core Components

Interface/Class/Enum: Enum

SpatialAvatarDefaultAnimSetType is an enumeration that defines different animation sets for avatars based on gender presentation. This allows developers to select appropriate animation sets for avatar characters in Spatial experiences.

## Properties/Fields

| Property | Description |
| --- | --- |
| Unset | Default animation set value when no specific set is selected. |
| Feminine | Animation set with feminine movement characteristics. |
| Masculine | Animation set with masculine movement characteristics. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;

public class AvatarAnimationController : MonoBehaviour
{
    [SerializeField] 
    private SpatialAvatarDefaultAnimSetType animSetType = SpatialAvatarDefaultAnimSetType.Unset;
    
    // Set animation type based on user preference
    public void SetAnimationType(bool useFeminineAnimations)
    {
        if (useFeminineAnimations)
        {
            animSetType = SpatialAvatarDefaultAnimSetType.Feminine;
        }
        else
        {
            animSetType = SpatialAvatarDefaultAnimSetType.Masculine;
        }
        
        ApplyAnimationSettings();
    }
    
    private void ApplyAnimationSettings()
    {
        // Apply animation settings to avatar here
        Debug.Log($"Setting avatar animation type to: {animSetType}");
        
        // Example of using the animation set type with SpatialAvatar component
        if (TryGetComponent<SpatialAvatar>(out var avatar))
        {
            // In a real implementation, you would set the animation type on the avatar
            Debug.Log($"Applied {animSetType} animations to avatar");
        }
    }
}
```

## Best Practices

1. Allow users to select their preferred animation set rather than automatically assigning based on avatar appearance.
2. Consider using the Unset value as a default, which allows the system to choose an appropriate animation set.
3. When switching between animation sets at runtime, ensure smooth transitions between animation states.

## Common Use Cases

1. Character customization screens where users can select their preferred animation style.
2. NPCs or pre-configured avatars with specific movement styles.
3. Games or experiences where character movement style is relevant to gameplay or narrative.

## Completed: March 09, 2025