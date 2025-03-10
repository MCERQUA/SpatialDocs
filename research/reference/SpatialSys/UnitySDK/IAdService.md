# IAdService

Ad Service Interface

Service for managing advertisement functionality in Spatial spaces. This service provides functionality for handling ad integration, including platform support checking and ad request management.

## Properties

### Platform Support
| Property | Description |
| --- | --- |
| isSupported | Platform ad support |

## Methods

### Ad Management
| Method | Description |
| --- | --- |
| RequestAd(SpatialAdType) | Request ad display |

## Usage Examples

```csharp
// Example: Ad Manager
public class AdManager : MonoBehaviour
{
    private IAdService adService;
    private Dictionary<string, AdState> adStates;
    private bool isInitialized;

    private class AdState
    {
        public bool isActive;
        public float lastShowTime;
        public Dictionary<string, object> settings;
        public Dictionary<string, object> metrics;
    }

    void Start()
    {
        adService = SpatialBridge.adService;
        adStates = new Dictionary<string, AdState>();
        InitializeManager();
    }

    private void InitializeManager()
    {
        if (!adService.isSupported)
        {
            Debug.LogWarning("Ad service not supported on this platform");
            return;
        }

        InitializeAdState("rewarded", new Dictionary<string, object>
        {
            { "type", SpatialAdType.Rewarded },
            { "cooldown", 300f },
            { "minReward", 100 },
            { "maxReward", 500 }
        });

        InitializeAdState("interstitial", new Dictionary<string, object>
        {
            { "type", SpatialAdType.Interstitial },
            { "cooldown", 180f },
            { "probability", 0.5f }
        });
    }

    public void ShowRewardedAd(
        Action<int> onRewardGranted = null,
        Action onAdFailed = null
    )
    {
        try
        {
            if (!CanShowAd("rewarded"))
            {
                onAdFailed?.Invoke();
                return;
            }

            var state = adStates["rewarded"];
            var request = adService.RequestAd(SpatialAdType.Rewarded);

            request.SetCompletedEvent((result) =>
            {
                if (result.succeeded)
                {
                    var reward = CalculateReward(state);
                    HandleAdCompletion("rewarded", reward);
                    onRewardGranted?.Invoke(reward);
                }
                else
                {
                    HandleAdFailure("rewarded", result.error);
                    onAdFailed?.Invoke();
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error showing rewarded ad: {e.Message}");
            onAdFailed?.Invoke();
        }
    }

    public void ShowInterstitialAd(
        Action onAdComplete = null,
        Action onAdFailed = null
    )
    {
        try
        {
            if (!CanShowAd("interstitial"))
            {
                onAdFailed?.Invoke();
                return;
            }

            var state = adStates["interstitial"];
            var request = adService.RequestAd(SpatialAdType.Interstitial);

            request.SetCompletedEvent((result) =>
            {
                if (result.succeeded)
                {
                    HandleAdCompletion("interstitial");
                    onAdComplete?.Invoke();
                }
                else
                {
                    HandleAdFailure("interstitial", result.error);
                    onAdFailed?.Invoke();
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error showing interstitial ad: {e.Message}");
            onAdFailed?.Invoke();
        }
    }

    private void InitializeAdState(
        string adType,
        Dictionary<string, object> settings
    )
    {
        var state = new AdState
        {
            isActive = true,
            lastShowTime = -float.MaxValue,
            settings = settings,
            metrics = new Dictionary<string, object>
            {
                { "showCount", 0 },
                { "successCount", 0 },
                { "failureCount", 0 },
                { "totalRewards", 0 }
            }
        };

        adStates[adType] = state;
    }

    private bool CanShowAd(string adType)
    {
        if (!adService.isSupported)
            return false;

        if (!adStates.TryGetValue(adType, out var state))
            return false;

        if (!state.isActive)
            return false;

        var cooldown = (float)state.settings["cooldown"];
        var timeSinceLastShow = Time.time - state.lastShowTime;

        return timeSinceLastShow >= cooldown;
    }

    private int CalculateReward(AdState state)
    {
        var minReward = (int)state.settings["minReward"];
        var maxReward = (int)state.settings["maxReward"];
        
        return UnityEngine.Random.Range(minReward, maxReward + 1);
    }

    private void HandleAdCompletion(
        string adType,
        int reward = 0
    )
    {
        if (!adStates.TryGetValue(adType, out var state))
            return;

        state.lastShowTime = Time.time;
        state.metrics["showCount"] = (int)state.metrics["showCount"] + 1;
        state.metrics["successCount"] = (int)state.metrics["successCount"] + 1;

        if (reward > 0)
        {
            state.metrics["totalRewards"] = (int)state.metrics["totalRewards"] + reward;
        }

        UpdateAdMetrics(adType, new Dictionary<string, object>
        {
            { "lastResult", "success" },
            { "lastReward", reward },
            { "completionTime", DateTime.UtcNow }
        });
    }

    private void HandleAdFailure(
        string adType,
        string error
    )
    {
        if (!adStates.TryGetValue(adType, out var state))
            return;

        state.metrics["showCount"] = (int)state.metrics["showCount"] + 1;
        state.metrics["failureCount"] = (int)state.metrics["failureCount"] + 1;

        UpdateAdMetrics(adType, new Dictionary<string, object>
        {
            { "lastResult", "failure" },
            { "lastError", error },
            { "failureTime", DateTime.UtcNow }
        });
    }

    private void UpdateAdMetrics(
        string adType,
        Dictionary<string, object> metrics
    )
    {
        if (!adStates.TryGetValue(adType, out var state))
            return;

        foreach (var kvp in metrics)
        {
            state.metrics[kvp.Key] = kvp.Value;
        }
    }
}

// Example: Ad Monitor
public class AdMonitor : MonoBehaviour
{
    private AdManager adManager;
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
        adManager = GetComponent<AdManager>();
        monitorStates = new Dictionary<string, MonitorState>();
        InitializeMonitors();
    }

    private void InitializeMonitors()
    {
        InitializeMonitor("performance", new Dictionary<string, object>
        {
            { "updateInterval", 60.0f },
            { "successThreshold", 0.9f }
        });

        InitializeMonitor("revenue", new Dictionary<string, object>
        {
            { "updateInterval", 300.0f },
            { "revenueTarget", 1000 }
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

1. Ad Management
   - Check platform
   - Handle states
   - Track metrics
   - Cache data

2. Ad Control
   - Manage timing
   - Handle rewards
   - Track completion
   - Handle failures

3. Performance
   - Monitor metrics
   - Cache states
   - Handle timing
   - Optimize checks

4. Error Handling
   - Validate requests
   - Handle failures
   - Recover states
   - Log issues

## Common Use Cases

1. Ad Types
   - Rewarded ads
   - Interstitial ads
   - Banner ads
   - Native ads

2. Ad Features
   - Reward systems
   - Cooldown timers
   - Completion tracking
   - Metric analysis

3. Ad Systems
   - Revenue tracking
   - Performance monitoring
   - User engagement
   - A/B testing

4. Ad Processing
   - Request handling
   - State management
   - Event processing
   - Metric tracking
