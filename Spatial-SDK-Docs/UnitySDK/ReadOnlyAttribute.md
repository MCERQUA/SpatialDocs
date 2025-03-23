# ReadOnlyAttribute

Category: Attributes

Interface/Class/Enum: Class

An attribute used to mark fields as read-only in the Unity Inspector. This attribute allows developers to display values in the inspector without allowing them to be modified, which is useful for debugging and informational purposes.

## Inheritance

ReadOnlyAttribute inherits from `PropertyAttribute`, which is Unity's base class for custom property attributes.

## Usage Examples

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class PlayerStatistics : MonoBehaviour
{
    // This field will be displayed in the inspector but cannot be modified
    [ReadOnly]
    public int totalPlayTime;
    
    // Regular fields can be modified as usual
    public string playerName;
    
    // ReadOnly can be combined with other attributes like SerializeField
    [SerializeField, ReadOnly]
    private int internalPlayerScore;
    
    // Can be used with different data types
    [ReadOnly]
    public Vector3 lastCheckpointPosition;
    
    private void Update()
    {
        // Update the read-only fields internally
        totalPlayTime = (int)Time.time;
        
        if (transform.position.y < -10f)
        {
            // Record position when player falls
            lastCheckpointPosition = transform.position;
        }
    }
    
    public void AddScore(int points)
    {
        internalPlayerScore += points;
    }
}
```

## Inspector Appearance

When using the ReadOnlyAttribute, fields will appear in the Unity Inspector with a grayed-out appearance, indicating they cannot be modified directly. However, the values will still update in real-time as they change during gameplay or in edit mode.

## Best Practices

1. Use ReadOnlyAttribute for debug values that you want to monitor in the inspector but shouldn't be modified manually.
2. Combine with `[SerializeField]` for private fields that need to be visible but not editable.
3. Use for displaying calculated values, statistics, or state information that's determined by code.
4. Consider using ReadOnlyAttribute for configuration values that should be visible to designers but maintained programmatically.
5. Remember that ReadOnlyAttribute only affects the Unity Inspector; fields can still be modified through code.

## Common Use Cases

1. Displaying runtime statistics like player score, health, or time played
2. Showing calculated values like distances, speeds, or derived properties
3. Viewing internal state variables without risking accidental modification
4. Presenting debug information during development
5. Displaying configuration values that should be visible but not editable in the inspector

## Implementation Notes

The ReadOnlyAttribute is implemented in the Spatial SDK to work with a custom property drawer that renders fields with a disabled appearance. While the attribute itself is simple, it requires the corresponding property drawer to function correctly in the Unity Editor.

## Limitations

1. Only affects the Unity Inspector; fields can still be modified through code
2. May not work with all complex types or custom property drawers
3. Has no effect in builds, as it's strictly an editor-time feature

## Completed: March 10, 2025