## Overview
The ClampAttribute is used to constrain float and integer values in the Unity Inspector between specified minimum and maximum values. This attribute is useful for properties that need to stay within a certain range.

## Properties
- **min**: The minimum value that the property can be set to in the inspector
- **max**: The maximum value that the property can be set to in the inspector

## Usage Example
```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class SpeedController : MonoBehaviour
{
    [Clamp(1.0f, 10.0f)]
    public float speed = 5.0f;
    
    [Clamp(0, 100)]
    public int healthPoints = 50;
    
    private void Update()
    {
        // The speed value will always be between 1.0 and 10.0
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        
        // The healthPoints value will always be between 0 and 100
        if (healthPoints <= 0)
        {
            Debug.Log("Game Over!");
        }
    }
}
```

## Best Practices
- Use ClampAttribute when you want to ensure that a value never goes below or above specified thresholds
- This attribute works with both float and int types
- Set sensible minimum and maximum values that reflect the practical range of the property
- Consider using Range attribute from UnityEngine for properties that need a slider in the inspector
- ClampAttribute only affects the inspector value - if you set values through code, you need to perform your own clamping

## Common Use Cases
- Limiting speeds, forces, or movement values to prevent extreme behavior
- Constraining health, energy, or resource values to valid ranges
- Setting minimum and maximum values for timers or cooldowns
- Restricting scale or size parameters to prevent objects from becoming too small or too large
- Setting reasonable limits for audio volumes, distances, or intensities

## Completed: March 10, 2025