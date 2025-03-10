# SpatialScriptableObjectBase

Category: Scriptable Objects

Class

SpatialScriptableObjectBase is the base class for all Spatial SDK scriptable objects. It provides common properties that all Spatial scriptable objects inherit.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| documentationURL | string | URL to external documentation or resources for this scriptable object. |
| isExperimental | bool | Indicates whether this scriptable object is experimental and may be subject to changes. |
| prettyName | string | A user-friendly display name for the scriptable object. |
| tooltip | string | Descriptive text that appears when hovering over the scriptable object in the editor. |

## Usage Examples

```csharp
// Creating a custom Spatial scriptable object
using UnityEngine;
using SpatialSys.UnitySDK;

[CreateAssetMenu(fileName = "NewCustomScriptableObject", menuName = "Spatial/Custom/MyScriptableObject")]
public class MyCustomScriptableObject : SpatialScriptableObjectBase
{
    [SerializeField] private string customProperty;
    
    // Custom implementation
    public void Initialize()
    {
        // Use the base class properties
        Debug.Log($"Initializing {prettyName}");
        
        if (isExperimental)
        {
            Debug.LogWarning($"{prettyName} is experimental and may change in future updates.");
        }
    }
}
```

## Best Practices

1. Always provide a meaningful prettyName and tooltip to improve usability in the editor.
2. Use the isExperimental flag when implementing features that might change in the future.
3. Consider providing a documentationURL to help users understand how to use the scriptable object.
4. When extending SpatialScriptableObjectBase, respect the established naming conventions for better integration with the Spatial ecosystem.

## Common Use Cases

1. Creating custom configuration assets for Spatial experiences
2. Defining reusable data and behavior across multiple prefabs
3. Building editor tools for designers to configure Spatial experiences without coding
4. Creating modular content systems with interchangeable assets