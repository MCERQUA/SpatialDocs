# IAudioService

Audio Service Interface

Service for managing audio functionality in Spatial spaces. This service provides functionality for playing sound effects with various configurations and spatial audio support.

## Methods

### Sound Effects
| Method | Description |
| --- | --- |
| PlaySFX(SpatialSFX, AudioSource, float, float) | Play SFX with source |
| PlaySFX(SpatialSFX, Vector3, float, float) | Play SFX at position |
| PlaySFX(SpatialSFX, float, float) | Play SFX with cache |

## Usage Examples

```csharp
// Example: Audio Manager
public class AudioManager : MonoBehaviour
{
    private IAudioService audioService;
    private Dictionary<string, AudioState> audioStates;
    private bool isInitialized;

    private class AudioState
    {
        public bool isActive;
        public float lastPlayTime;
        public Dictionary<string, object> settings;
        public Dictionary<string, object> metrics;
    }

    void Start()
    {
        audioService = SpatialBridge.audioService;
        audioStates = new Dictionary<string, AudioState>();
        InitializeManager();
    }

    private void InitializeManager()
    {
        InitializeAudioState("effects", new Dictionary<string, object>
        {
            { "volume", 1.0f },
            { "pitch", 1.0f },
            { "cooldown", 0.1f }
        });

        InitializeAudioState("ambient", new Dictionary<string, object>
        {
            { "volume", 0.5f },
            { "pitch", 1.0f },
            { "fadeTime", 2.0f }
        });
    }

    public void PlaySoundEffect(
        SpatialSFX sfx,
        Vector3? position = null,
        AudioSource source = null,
        float? volume = null,
        float? pitch = null
    )
    {
        try
        {
            if (!CanPlaySound("effects"))
                return;

            var state = audioStates["effects"];
            var settings = state.settings;

            var finalVolume = volume ?? (float)settings["volume"];
            var finalPitch = pitch ?? (float)settings["pitch"];

            if (source != null)
            {
                audioService.PlaySFX(sfx, source, finalVolume, finalPitch);
            }
            else if (position.HasValue)
            {
                audioService.PlaySFX(sfx, position.Value, finalVolume, finalPitch);
            }
            else
            {
                audioService.PlaySFX(sfx, finalVolume, finalPitch);
            }

            HandleSoundPlayed("effects", new Dictionary<string, object>
            {
                { "sfx", sfx },
                { "volume", finalVolume },
                { "pitch", finalPitch },
                { "position", position },
                { "source", source != null }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error playing sound effect: {e.Message}");
        }
    }

    public void PlayAmbientSound(
        SpatialSFX sfx,
        Vector3 position,
        float fadeInTime = 0f
    )
    {
        try
        {
            if (!CanPlaySound("ambient"))
                return;

            var state = audioStates["ambient"];
            var settings = state.settings;

            var volume = (float)settings["volume"];
            var pitch = (float)settings["pitch"];

            audioService.PlaySFX(sfx, position, volume, pitch);

            HandleSoundPlayed("ambient", new Dictionary<string, object>
            {
                { "sfx", sfx },
                { "position", position },
                { "fadeTime", fadeInTime }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error playing ambient sound: {e.Message}");
        }
    }

    private void InitializeAudioState(
        string audioType,
        Dictionary<string, object> settings
    )
    {
        var state = new AudioState
        {
            isActive = true,
            lastPlayTime = -float.MaxValue,
            settings = settings,
            metrics = new Dictionary<string, object>
            {
                { "playCount", 0 },
                { "totalPlayTime", 0f },
                { "lastDuration", 0f }
            }
        };

        audioStates[audioType] = state;
    }

    private bool CanPlaySound(string audioType)
    {
        if (!audioStates.TryGetValue(audioType, out var state))
            return false;

        if (!state.isActive)
            return false;

        if (!state.settings.ContainsKey("cooldown"))
            return true;

        var cooldown = (float)state.settings["cooldown"];
        var timeSinceLastPlay = Time.time - state.lastPlayTime;

        return timeSinceLastPlay >= cooldown;
    }

    private void HandleSoundPlayed(
        string audioType,
        Dictionary<string, object> playData
    )
    {
        if (!audioStates.TryGetValue(audioType, out var state))
            return;

        state.lastPlayTime = Time.time;
        state.metrics["playCount"] = (int)state.metrics["playCount"] + 1;

        UpdateAudioMetrics(audioType, new Dictionary<string, object>
        {
            { "lastPlayed", DateTime.UtcNow },
            { "lastPlayData", playData }
        });
    }

    private void UpdateAudioMetrics(
        string audioType,
        Dictionary<string, object> metrics
    )
    {
        if (!audioStates.TryGetValue(audioType, out var state))
            return;

        foreach (var kvp in metrics)
        {
            state.metrics[kvp.Key] = kvp.Value;
        }
    }
}

// Example: Audio Monitor
public class AudioMonitor : MonoBehaviour
{
    private AudioManager audioManager;
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
        audioManager = GetComponent<AudioManager>();
        monitorStates = new Dictionary<string, MonitorState>();
        InitializeMonitors();
    }

    private void InitializeMonitors()
    {
        InitializeMonitor("performance", new Dictionary<string, object>
        {
            { "updateInterval", 1.0f },
            { "maxConcurrent", 10 }
        });

        InitializeMonitor("quality", new Dictionary<string, object>
        {
            { "updateInterval", 5.0f },
            { "checkDistance", 20.0f }
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

1. Audio Management
   - Handle states
   - Track metrics
   - Control timing
   - Cache data

2. Sound Control
   - Manage volume
   - Handle pitch
   - Track position
   - Control sources

3. Performance
   - Monitor metrics
   - Cache states
   - Handle timing
   - Optimize playback

4. Error Handling
   - Validate sounds
   - Handle failures
   - Recover states
   - Log issues

## Common Use Cases

1. Sound Types
   - Sound effects
   - Ambient sounds
   - Music tracks
   - Voice overs

2. Audio Features
   - Spatial audio
   - Volume control
   - Pitch shifting
   - Fade effects

3. Audio Systems
   - Sound pooling
   - Distance culling
   - Priority queuing
   - State management

4. Audio Processing
   - Effect mixing
   - State tracking
   - Event handling
   - Metric analysis
