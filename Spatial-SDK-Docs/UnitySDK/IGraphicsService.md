# IGraphicsService

Graphics Service Interface

Service for managing graphics and rendering functionality in Spatial spaces. This service provides comprehensive control over render pipeline settings, including lighting, shadows, post-processing, and quality settings.

## Properties

### Lighting Settings
| Property | Description |
| --- | --- |
| maxAdditionalLightsCount | Additional lights limit |
| shadowDistance | Shadow render distance |
| shadowDepthBias | Shadow push distance |
| shadowNormalBias | Surface shrink distance |
| cascadeBorder | Last cascade distance |

### Quality Settings
| Property | Description |
| --- | --- |
| msaaSampleCount | Anti-aliasing level |
| supportsHDR | HDR rendering toggle |
| opaqueDownsampling | Downsample method |
| mainLightShadowmapResolution | Shadow resolution |

### Post-Processing
| Property | Description |
| --- | --- |
| colorGradingMode | Color grading type |
| colorGradingLutSize | LUT texture size |

### Camera Features
| Property | Description |
| --- | --- |
| supportsCameraDepthTexture | Depth texture support |
| supportsCameraOpaqueTexture | Opaque texture support |

## Events

### Rendering Events
| Event | Description |
| --- | --- |
| beginMainCameraRendering | Camera render start |

## Usage Examples

```csharp
// Example: Graphics Manager
public class GraphicsManager : MonoBehaviour
{
    private IGraphicsService graphicsService;
    private Dictionary<string, GraphicsState> graphicsStates;
    private bool isInitialized;

    private class GraphicsState
    {
        public bool isActive;
        public float lastUpdateTime;
        public Dictionary<string, object> settings;
        public Dictionary<string, object> metrics;
    }

    void Start()
    {
        graphicsService = SpatialBridge.graphicsService;
        graphicsStates = new Dictionary<string, GraphicsState>();
        InitializeManager();
        SubscribeToEvents();
    }

    private void InitializeManager()
    {
        InitializeQualitySettings();
        InitializeShadowSettings();
        InitializePostProcessing();
    }

    private void SubscribeToEvents()
    {
        graphicsService.beginMainCameraRendering += HandleBeginCameraRendering;
    }

    private void InitializeQualitySettings()
    {
        try
        {
            graphicsService.msaaSampleCount = 4;
            graphicsService.supportsHDR = true;
            graphicsService.opaqueDownsampling = DownsamplingMethod.None;

            InitializeGraphicsState("quality", new Dictionary<string, object>
            {
                { "msaa", 4 },
                { "hdr", true },
                { "downsampling", "none" }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error initializing quality settings: {e.Message}");
        }
    }

    private void InitializeShadowSettings()
    {
        try
        {
            graphicsService.shadowDistance = 30f;
            graphicsService.shadowDepthBias = 1f;
            graphicsService.shadowNormalBias = 1f;
            graphicsService.cascadeBorder = 0.2f;
            graphicsService.mainLightShadowmapResolution = 2048;

            InitializeGraphicsState("shadows", new Dictionary<string, object>
            {
                { "distance", 30f },
                { "depthBias", 1f },
                { "normalBias", 1f },
                { "cascadeBorder", 0.2f },
                { "resolution", 2048 }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error initializing shadow settings: {e.Message}");
        }
    }

    private void InitializePostProcessing()
    {
        try
        {
            graphicsService.colorGradingMode = ColorGradingMode.HighDynamicRange;
            graphicsService.colorGradingLutSize = 32;

            InitializeGraphicsState("postProcess", new Dictionary<string, object>
            {
                { "colorGradingMode", "hdr" },
                { "lutSize", 32 }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error initializing post-processing: {e.Message}");
        }
    }

    public void SetQualityLevel(
        QualityLevel level
    )
    {
        try
        {
            switch (level)
            {
                case QualityLevel.Low:
                    graphicsService.msaaSampleCount = 1;
                    graphicsService.supportsHDR = false;
                    graphicsService.shadowDistance = 15f;
                    graphicsService.mainLightShadowmapResolution = 1024;
                    break;

                case QualityLevel.Medium:
                    graphicsService.msaaSampleCount = 2;
                    graphicsService.supportsHDR = true;
                    graphicsService.shadowDistance = 30f;
                    graphicsService.mainLightShadowmapResolution = 2048;
                    break;

                case QualityLevel.High:
                    graphicsService.msaaSampleCount = 4;
                    graphicsService.supportsHDR = true;
                    graphicsService.shadowDistance = 50f;
                    graphicsService.mainLightShadowmapResolution = 4096;
                    break;
            }

            UpdateGraphicsState("quality", new Dictionary<string, object>
            {
                { "level", level },
                { "updateTime", Time.time }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error setting quality level: {e.Message}");
        }
    }

    private void InitializeGraphicsState(
        string stateId,
        Dictionary<string, object> settings
    )
    {
        var state = new GraphicsState
        {
            isActive = true,
            lastUpdateTime = Time.time,
            settings = settings,
            metrics = new Dictionary<string, object>()
        };

        graphicsStates[stateId] = state;
    }

    private void UpdateGraphicsState(
        string stateId,
        Dictionary<string, object> metrics
    )
    {
        if (!graphicsStates.TryGetValue(stateId, out var state))
            return;

        state.lastUpdateTime = Time.time;
        foreach (var kvp in metrics)
        {
            state.metrics[kvp.Key] = kvp.Value;
        }
    }

    private void HandleBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        UpdateRenderingMetrics(camera);
    }

    private void UpdateRenderingMetrics(Camera camera)
    {
        var metrics = new Dictionary<string, object>
        {
            { "resolution", new Vector2(camera.pixelWidth, camera.pixelHeight) },
            { "aspectRatio", camera.aspect },
            { "fieldOfView", camera.fieldOfView },
            { "renderTime", Time.time }
        };

        UpdateGraphicsState("rendering", metrics);
    }

    void OnDestroy()
    {
        if (graphicsService != null)
        {
            graphicsService.beginMainCameraRendering -= HandleBeginCameraRendering;
        }
    }
}

// Example: Graphics Monitor
public class GraphicsMonitor : MonoBehaviour
{
    private GraphicsManager graphicsManager;
    private Dictionary<string, MonitorState> monitorStates;
    private bool isInitialized;

    private class MonitorState
    {
        public bool isActive;
        public float lastUpdateTime;
        public Dictionary<string, object> settings;
    }

    void Start()
    {
        graphicsManager = GetComponent<GraphicsManager>();
        monitorStates = new Dictionary<string, MonitorState>();
        InitializeMonitors();
    }

    private void InitializeMonitors()
    {
        InitializeMonitor("performance", new Dictionary<string, object>
        {
            { "updateInterval", 1.0f },
            { "fpsThreshold", 30.0f },
            { "msaaThreshold", 4 }
        });

        InitializeMonitor("quality", new Dictionary<string, object>
        {
            { "updateInterval", 5.0f },
            { "shadowDistance", 30.0f },
            { "lutSize", 32 }
        });
    }

    private void InitializeMonitor(
        string monitorId,
        Dictionary<string, object> settings
    )
    {
        var state = new MonitorState
        {
            isActive = true,
            lastUpdateTime = Time.time,
            settings = settings
        };

        monitorStates[monitorId] = state;
    }

    private void UpdateMonitors()
    {
        foreach (var state in monitorStates.Values)
        {
            if (!state.isActive)
                continue;

            var interval = (float)state.settings["updateInterval"];
            if (Time.time - state.lastUpdateTime >= interval)
            {
                UpdateMonitorMetrics(state);
                state.lastUpdateTime = Time.time;
            }
        }
    }

    private void UpdateMonitorMetrics(MonitorState state)
    {
        // Implementation for monitor metric updates
    }

    void Update()
    {
        UpdateMonitors();
    }
}
```

## Best Practices

1. Quality Management
   - Balance performance
   - Scale settings
   - Profile impact
   - Cache states

2. Shadow Control
   - Optimize distance
   - Handle cascades
   - Manage bias
   - Monitor quality

3. Performance
   - Monitor FPS
   - Track metrics
   - Handle timing
   - Optimize settings

4. Error Handling
   - Validate settings
   - Handle failures
   - Recover states
   - Log issues

## Common Use Cases

1. Quality Levels
   - Low settings
   - Medium settings
   - High settings
   - Custom profiles

2. Graphics Features
   - Shadow quality
   - Anti-aliasing
   - Post-processing
   - HDR rendering

3. Performance Systems
   - Quality scaling
   - Feature toggling
   - State monitoring
   - Metric tracking

4. Graphics Processing
   - Setting validation
   - State management
   - Event handling
   - Metric analysis
