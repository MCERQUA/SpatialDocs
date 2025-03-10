# SpatialRenderPipelineSettingsOverrides

Category: Core Components

Interface/Class/Enum: Class

The SpatialRenderPipelineSettingsOverrides component allows developers to customize and override rendering settings for a Spatial experience. It provides control over visual quality, rendering features, and performance options that affect how the environment is rendered. This component is particularly useful for optimizing visual fidelity and performance based on the specific needs of your space.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| _limitOnePerScene | bool | Internal property that enforces only one instance of this component per scene. |
| overrideSettings | bool | Determines whether the custom render pipeline settings should be applied. |
| renderPipelineSettings | RenderPipelineSettings | Contains all the customizable rendering options and settings. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |
| documentationURL | string | URL to documentation for this component. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;

public class RenderingSettingsManager : MonoBehaviour
{
    // Reference to the SpatialRenderPipelineSettingsOverrides component
    [SerializeField] private SpatialRenderPipelineSettingsOverrides renderSettings;
    
    // Environment state references
    [SerializeField] private GameObject dayEnvironment;
    [SerializeField] private GameObject nightEnvironment;
    [SerializeField] private Light mainDirectionalLight;
    
    // Performance monitoring
    private float frameRateCheckInterval = 5f;
    private float nextCheckTime = 0f;
    private int frameCount = 0;
    private float timeElapsed = 0f;
    private float averageFrameRate = 0f;
    
    private void Start()
    {
        // Ensure we have a reference to the render settings
        if (renderSettings == null)
        {
            renderSettings = FindObjectOfType<SpatialRenderPipelineSettingsOverrides>();
            
            if (renderSettings == null)
            {
                Debug.LogError("No SpatialRenderPipelineSettingsOverrides found in the scene!");
                return;
            }
        }
        
        // Initialize with default high quality settings
        ApplyHighQualitySettings();
    }
    
    private void Update()
    {
        // Monitor performance
        MonitorPerformance();
        
        // Example: Toggle environment based on key press
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleDayNightEnvironment();
        }
        
        // Example: Quality setting adjustments based on key press
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ApplyLowQualitySettings();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ApplyMediumQualitySettings();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ApplyHighQualitySettings();
        }
    }
    
    // Monitor performance and adjust settings if needed
    private void MonitorPerformance()
    {
        frameCount++;
        timeElapsed += Time.deltaTime;
        
        if (Time.time > nextCheckTime)
        {
            // Calculate average frame rate
            averageFrameRate = frameCount / timeElapsed;
            nextCheckTime = Time.time + frameRateCheckInterval;
            
            // Log current performance
            Debug.Log($"Average FPS: {averageFrameRate}");
            
            // Auto-adjust quality settings based on performance
            if (averageFrameRate < 20f)
            {
                Debug.Log("Low performance detected - applying low quality settings");
                ApplyLowQualitySettings();
            }
            else if (averageFrameRate < 45f)
            {
                Debug.Log("Medium performance detected - applying medium quality settings");
                ApplyMediumQualitySettings();
            }
            
            // Reset counters
            frameCount = 0;
            timeElapsed = 0f;
        }
    }
    
    // Toggle between day and night environment
    public void ToggleDayNightEnvironment()
    {
        if (dayEnvironment != null && nightEnvironment != null)
        {
            bool isDayActive = dayEnvironment.activeSelf;
            
            // Toggle environments
            dayEnvironment.SetActive(!isDayActive);
            nightEnvironment.SetActive(isDayActive);
            
            // Apply appropriate render settings
            if (isDayActive)
            {
                // Apply night settings
                ApplyNightRenderSettings();
            }
            else
            {
                // Apply day settings
                ApplyDayRenderSettings();
            }
        }
    }
    
    // Apply settings optimized for daytime
    private void ApplyDayRenderSettings()
    {
        if (renderSettings == null) return;
        
        // Enable render pipeline overrides
        renderSettings.overrideSettings = true;
        
        // Apply day-specific settings
        var settings = renderSettings.renderPipelineSettings;
        
        // Example settings - adjust for your specific needs
        settings.bloom = true;
        settings.bloomIntensity = 0.2f;
        settings.vignette = false;
        settings.ambientOcclusion = true;
        
        // Apply shadow settings
        if (mainDirectionalLight != null)
        {
            mainDirectionalLight.shadows = LightShadows.Soft;
            mainDirectionalLight.shadowStrength = 0.7f;
        }
        
        Debug.Log("Applied day render settings");
    }
    
    // Apply settings optimized for nighttime
    private void ApplyNightRenderSettings()
    {
        if (renderSettings == null) return;
        
        // Enable render pipeline overrides
        renderSettings.overrideSettings = true;
        
        // Apply night-specific settings
        var settings = renderSettings.renderPipelineSettings;
        
        // Example settings - adjust for your specific needs
        settings.bloom = true;
        settings.bloomIntensity = 0.5f;
        settings.vignette = true;
        settings.ambientOcclusion = true;
        
        // Apply shadow settings
        if (mainDirectionalLight != null)
        {
            mainDirectionalLight.shadows = LightShadows.Soft;
            mainDirectionalLight.shadowStrength = 0.9f;
        }
        
        Debug.Log("Applied night render settings");
    }
    
    // Apply low quality settings for performance
    public void ApplyLowQualitySettings()
    {
        if (renderSettings == null) return;
        
        // Enable render pipeline overrides
        renderSettings.overrideSettings = true;
        
        // Apply low quality settings
        var settings = renderSettings.renderPipelineSettings;
        
        // Example low quality settings
        settings.shadowDistance = 50f;
        settings.shadowResolution = 1024;
        settings.shadowCascades = 2;
        
        settings.bloom = false;
        settings.depthOfField = false;
        settings.motionBlur = false;
        settings.ambientOcclusion = false;
        settings.volumetricFog = false;
        
        settings.reflections = false;
        settings.antiAliasing = false;
        
        // Apply shadow settings
        if (mainDirectionalLight != null)
        {
            mainDirectionalLight.shadows = LightShadows.Hard;
            mainDirectionalLight.shadowResolution = LightShadowResolution.Low;
        }
        
        Debug.Log("Applied low quality render settings");
    }
    
    // Apply medium quality settings
    public void ApplyMediumQualitySettings()
    {
        if (renderSettings == null) return;
        
        // Enable render pipeline overrides
        renderSettings.overrideSettings = true;
        
        // Apply medium quality settings
        var settings = renderSettings.renderPipelineSettings;
        
        // Example medium quality settings
        settings.shadowDistance = 100f;
        settings.shadowResolution = 2048;
        settings.shadowCascades = 2;
        
        settings.bloom = true;
        settings.bloomIntensity = 0.3f;
        settings.depthOfField = false;
        settings.motionBlur = false;
        settings.ambientOcclusion = true;
        settings.volumetricFog = false;
        
        settings.reflections = true;
        settings.antiAliasing = true;
        
        // Apply shadow settings
        if (mainDirectionalLight != null)
        {
            mainDirectionalLight.shadows = LightShadows.Soft;
            mainDirectionalLight.shadowResolution = LightShadowResolution.Medium;
        }
        
        Debug.Log("Applied medium quality render settings");
    }
    
    // Apply high quality settings
    public void ApplyHighQualitySettings()
    {
        if (renderSettings == null) return;
        
        // Enable render pipeline overrides
        renderSettings.overrideSettings = true;
        
        // Apply high quality settings
        var settings = renderSettings.renderPipelineSettings;
        
        // Example high quality settings
        settings.shadowDistance = 150f;
        settings.shadowResolution = 4096;
        settings.shadowCascades = 4;
        
        settings.bloom = true;
        settings.bloomIntensity = 0.3f;
        settings.depthOfField = true;
        settings.motionBlur = true;
        settings.ambientOcclusion = true;
        settings.volumetricFog = true;
        
        settings.reflections = true;
        settings.antiAliasing = true;
        
        // Apply shadow settings
        if (mainDirectionalLight != null)
        {
            mainDirectionalLight.shadows = LightShadows.Soft;
            mainDirectionalLight.shadowResolution = LightShadowResolution.High;
        }
        
        Debug.Log("Applied high quality render settings");
    }
    
    // Disable custom render settings and revert to defaults
    public void RevertToDefaultSettings()
    {
        if (renderSettings == null) return;
        
        // Disable render pipeline overrides to use system defaults
        renderSettings.overrideSettings = false;
        
        Debug.Log("Reverted to default render settings");
    }
}
```

## Best Practices

1. Use render pipeline overrides judiciously, as they can impact performance across different devices.
2. Consider implementing dynamic quality settings that adjust based on device performance.
3. Test your render settings across multiple device types to ensure consistent visual quality and performance.
4. Balance visual quality with performance requirements, especially for mobile or VR experiences.
5. Only override settings that are necessary for your specific visual style, leaving others at their defaults.
6. Document your render settings choices to make it easier to understand and maintain your project.
7. Group related settings changes together when switching between different environment states (day/night, indoor/outdoor).
8. Consider the impact of render settings on battery life for mobile devices.
9. Use the _limitOnePerScene property to ensure only one instance of the component affects the scene.
10. Implement a way for users to adjust quality settings based on their device's capabilities.

## Common Use Cases

1. Optimizing performance for different device capabilities (mobile vs. desktop).
2. Creating distinct visual atmospheres for different environments or areas within your space.
3. Implementing day/night cycles with appropriate lighting and post-processing.
4. Enhancing specific scenes with custom post-processing effects for dramatic moments.
5. Creating cinematic experiences with depth of field, motion blur, and other film-like effects.
6. Optimizing for VR by carefully balancing quality and performance.
7. Creating underwater or other special environmental effects with specific render settings.
8. Implementing weather systems with appropriate visual effects.
9. Creating horror or atmospheric experiences with vignetting, color grading, and other mood-enhancing effects.
10. Implementing accessibility options by providing simplified visual settings for users who may be sensitive to certain effects.

## Completed: March 10, 2025