# SpatialEnvironmentSettingsOverrides

Category: Core Components

Interface/Class/Enum: Class

The SpatialEnvironmentSettingsOverrides component allows developers to customize environmental settings for a Spatial experience. This component can modify lighting, atmosphere, and other environmental parameters to create a unique visual style for a space.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| environmentSettings | Not Specified | Contains various environment settings that can be overridden in the space. |
| _limitOnePerScene | bool | Internal property that limits this component to one instance per scene. |
| documentationURL | string | URL to documentation for this component. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private Light directionalLight;
    [SerializeField] private Material skyboxMaterial;
    
    private SpatialEnvironmentSettingsOverrides environmentOverrides;
    
    private void Awake()
    {
        // Find or create environment settings overrides
        environmentOverrides = FindObjectOfType<SpatialEnvironmentSettingsOverrides>();
        
        if (environmentOverrides == null)
        {
            GameObject settingsObject = new GameObject("Environment_Settings_Overrides");
            environmentOverrides = settingsObject.AddComponent<SpatialEnvironmentSettingsOverrides>();
            Debug.Log("Created new SpatialEnvironmentSettingsOverrides component");
        }
    }
    
    // Method to apply daytime environment settings
    public void ApplyDaytimeSettings()
    {
        if (environmentOverrides == null)
        {
            Debug.LogError("Environment overrides component not found!");
            return;
        }
        
        // Note: The actual properties of environmentSettings would need to be 
        // configured according to the SDK's specific implementation.
        // The following code demonstrates the concept but may need adaptation.
        
        // Example of how settings might be configured (conceptual)
        /*
        environmentOverrides.environmentSettings.skyboxSettings.skyboxMaterial = skyboxMaterial;
        environmentOverrides.environmentSettings.lightingSettings.ambientColor = new Color(0.5f, 0.5f, 0.6f);
        environmentOverrides.environmentSettings.lightingSettings.ambientIntensity = 1.2f;
        environmentOverrides.environmentSettings.fogSettings.enabled = true;
        environmentOverrides.environmentSettings.fogSettings.fogColor = new Color(0.8f, 0.9f, 1.0f);
        environmentOverrides.environmentSettings.fogSettings.fogDensity = 0.01f;
        */
        
        // Update directional light to match environment settings
        if (directionalLight != null)
        {
            directionalLight.color = new Color(1.0f, 0.95f, 0.8f);
            directionalLight.intensity = 1.2f;
            directionalLight.transform.rotation = Quaternion.Euler(50, 30, 0);
        }
        
        Debug.Log("Applied daytime environment settings");
    }
    
    // Method to apply nighttime environment settings
    public void ApplyNighttimeSettings()
    {
        if (environmentOverrides == null)
        {
            Debug.LogError("Environment overrides component not found!");
            return;
        }
        
        // Example of how settings might be configured (conceptual)
        /*
        environmentOverrides.environmentSettings.skyboxSettings.skyboxMaterial = nightSkyboxMaterial;
        environmentOverrides.environmentSettings.lightingSettings.ambientColor = new Color(0.1f, 0.1f, 0.2f);
        environmentOverrides.environmentSettings.lightingSettings.ambientIntensity = 0.5f;
        environmentOverrides.environmentSettings.fogSettings.enabled = true;
        environmentOverrides.environmentSettings.fogSettings.fogColor = new Color(0.05f, 0.05f, 0.1f);
        environmentOverrides.environmentSettings.fogSettings.fogDensity = 0.03f;
        */
        
        // Update directional light to match environment settings
        if (directionalLight != null)
        {
            directionalLight.color = new Color(0.1f, 0.1f, 0.3f);
            directionalLight.intensity = 0.3f;
            directionalLight.transform.rotation = Quaternion.Euler(-30, 220, 0);
        }
        
        Debug.Log("Applied nighttime environment settings");
    }
    
    // Method to reset to default environment settings
    public void ResetEnvironmentSettings()
    {
        if (environmentOverrides == null)
        {
            Debug.LogError("Environment overrides component not found!");
            return;
        }
        
        // Reset to default settings (conceptual)
        /*
        environmentOverrides.environmentSettings.Reset();
        */
        
        Debug.Log("Reset environment settings to default");
    }
}
```

## Best Practices

1. Remember that only one SpatialEnvironmentSettingsOverrides component can exist in a scene due to the `_limitOnePerScene` restriction.
2. Test your environment settings on different devices to ensure consistent visual quality.
3. Be mindful of performance impacts when adding complex environmental effects like fog or extensive post-processing.
4. Consider how your environment settings affect the readability of UI and visibility of important gameplay elements.
5. Create transitions between different environment settings rather than abrupt changes for better user experience.
6. Use environment settings to establish mood and atmosphere that complements your space's theme and purpose.
7. Remember that environmental settings affect the entire space and should be cohesive with your overall visual direction.

## Common Use Cases

1. Creating distinctive day/night cycles in persistent world spaces.
2. Establishing specific moods for different areas or zones in an experience.
3. Highlighting special events or achievements with environmental changes.
4. Weather systems that modify environmental settings based on time or triggers.
5. Story-driven experiences where environment changes reflect narrative progression.
6. Matching the environment to specific themes or seasons (holiday events, special occasions).
7. Creating cinematic moments by dynamically adjusting environment settings during key sequences.

## Completed: March 09, 2025