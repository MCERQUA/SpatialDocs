# IBadgeService

Badge Service Interface

Service for managing badge and achievement systems in Spatial spaces. This service provides functionality for rewarding badges to users and handling badge-related notifications.

## Methods

### Badge Management
| Method | Description |
| --- | --- |
| RewardBadge(string) | Award badge to user |

## Usage Examples

```csharp
// Example: Badge Manager
public class BadgeManager : MonoBehaviour
{
    private IBadgeService badgeService;
    private Dictionary<string, BadgeState> badgeStates;
    private bool isInitialized;

    private class BadgeState
    {
        public bool isAwarded;
        public float awardTime;
        public Dictionary<string, object> metadata;
        public Dictionary<string, object> progress;
    }

    void Start()
    {
        badgeService = SpatialBridge.badgeService;
        badgeStates = new Dictionary<string, BadgeState>();
        InitializeManager();
    }

    private void InitializeManager()
    {
        InitializeBadges();
    }

    private void InitializeBadges()
    {
        // Example badge definitions
        InitializeBadge("explorer", new Dictionary<string, object>
        {
            { "name", "Explorer" },
            { "description", "Discover all areas" },
            { "type", "achievement" },
            { "points", 100 }
        });

        InitializeBadge("collector", new Dictionary<string, object>
        {
            { "name", "Collector" },
            { "description", "Collect all items" },
            { "type", "achievement" },
            { "points", 200 }
        });

        InitializeBadge("master", new Dictionary<string, object>
        {
            { "name", "Master" },
            { "description", "Complete all challenges" },
            { "type", "achievement" },
            { "points", 500 }
        });
    }

    public void AwardBadge(
        string badgeId,
        Dictionary<string, object> context = null
    )
    {
        try
        {
            if (!badgeStates.TryGetValue(badgeId, out var state))
            {
                Debug.LogError($"Badge {badgeId} not found");
                return;
            }

            if (state.isAwarded)
            {
                Debug.LogWarning($"Badge {badgeId} already awarded");
                return;
            }

            badgeService.RewardBadge(badgeId);
            
            state.isAwarded = true;
            state.awardTime = Time.time;
            
            if (context != null)
            {
                foreach (var kvp in context)
                {
                    state.metadata[kvp.Key] = kvp.Value;
                }
            }

            OnBadgeAwarded(badgeId, state);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error awarding badge {badgeId}: {e.Message}");
        }
    }

    public void UpdateBadgeProgress(
        string badgeId,
        string progressKey,
        object value
    )
    {
        try
        {
            if (!badgeStates.TryGetValue(badgeId, out var state))
            {
                Debug.LogError($"Badge {badgeId} not found");
                return;
            }

            if (state.isAwarded)
            {
                Debug.LogWarning($"Badge {badgeId} already awarded");
                return;
            }

            state.progress[progressKey] = value;
            state.metadata["lastUpdate"] = DateTime.UtcNow;

            CheckBadgeCompletion(badgeId, state);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating badge progress: {e.Message}");
        }
    }

    private void InitializeBadge(
        string badgeId,
        Dictionary<string, object> metadata
    )
    {
        var state = new BadgeState
        {
            isAwarded = false,
            awardTime = 0f,
            metadata = new Dictionary<string, object>(metadata),
            progress = new Dictionary<string, object>()
        };

        badgeStates[badgeId] = state;
    }

    private void CheckBadgeCompletion(
        string badgeId,
        BadgeState state
    )
    {
        // Example completion checks
        switch (badgeId)
        {
            case "explorer":
                CheckExplorerBadge(state);
                break;
            case "collector":
                CheckCollectorBadge(state);
                break;
            case "master":
                CheckMasterBadge(state);
                break;
        }
    }

    private void CheckExplorerBadge(BadgeState state)
    {
        if (!state.progress.TryGetValue("areasDiscovered", out var value))
            return;

        var discovered = (int)value;
        var total = 10; // Example total areas

        if (discovered >= total)
        {
            AwardBadge("explorer", new Dictionary<string, object>
            {
                { "areasDiscovered", discovered },
                { "completionTime", Time.time }
            });
        }
    }

    private void CheckCollectorBadge(BadgeState state)
    {
        if (!state.progress.TryGetValue("itemsCollected", out var value))
            return;

        var collected = (int)value;
        var total = 50; // Example total items

        if (collected >= total)
        {
            AwardBadge("collector", new Dictionary<string, object>
            {
                { "itemsCollected", collected },
                { "completionTime", Time.time }
            });
        }
    }

    private void CheckMasterBadge(BadgeState state)
    {
        if (!state.progress.TryGetValue("challengesCompleted", out var value))
            return;

        var completed = (int)value;
        var total = 20; // Example total challenges

        if (completed >= total)
        {
            AwardBadge("master", new Dictionary<string, object>
            {
                { "challengesCompleted", completed },
                { "completionTime", Time.time }
            });
        }
    }

    private void OnBadgeAwarded(
        string badgeId,
        BadgeState state
    )
    {
        // Handle post-award logic
        var points = (int)state.metadata["points"];
        UpdatePlayerScore(points);

        // Check for meta-achievements
        CheckMetaAchievements();
    }

    private void UpdatePlayerScore(int points)
    {
        // Implementation for updating player score
    }

    private void CheckMetaAchievements()
    {
        var awardedCount = badgeStates.Count(x => x.Value.isAwarded);
        var totalBadges = badgeStates.Count;

        if (awardedCount == totalBadges)
        {
            // Award completion achievement
            AwardBadge("completionist", new Dictionary<string, object>
            {
                { "totalBadges", totalBadges },
                { "completionTime", Time.time }
            });
        }
    }
}

// Example: Badge Progress Tracker
public class BadgeProgressTracker : MonoBehaviour
{
    private BadgeManager badgeManager;
    private Dictionary<string, ProgressState> progressStates;
    private bool isInitialized;

    private class ProgressState
    {
        public bool isTracking;
        public float lastUpdateTime;
        public Dictionary<string, object> metrics;
    }

    void Start()
    {
        badgeManager = GetComponent<BadgeManager>();
        progressStates = new Dictionary<string, ProgressState>();
        InitializeTracker();
    }

    private void InitializeTracker()
    {
        InitializeProgress("exploration", new Dictionary<string, object>
        {
            { "updateInterval", 5.0f },
            { "checkRadius", 10.0f }
        });

        InitializeProgress("collection", new Dictionary<string, object>
        {
            { "updateInterval", 1.0f },
            { "checkDistance", 2.0f }
        });
    }

    private void InitializeProgress(
        string trackerId,
        Dictionary<string, object> metrics
    )
    {
        var state = new ProgressState
        {
            isTracking = true,
            lastUpdateTime = Time.time,
            metrics = metrics
        };

        progressStates[trackerId] = state;
    }

    private void UpdateProgress()
    {
        foreach (var state in progressStates.Values)
        {
            if (!state.isTracking)
                continue;

            var interval = (float)state.metrics["updateInterval"];
            if (Time.time - state.lastUpdateTime >= interval)
            {
                UpdateProgressMetrics(state);
                state.lastUpdateTime = Time.time;
            }
        }
    }

    private void UpdateProgressMetrics(ProgressState state)
    {
        // Implementation for progress metric updates
    }

    void Update()
    {
        UpdateProgress();
    }
}
```

## Best Practices

1. Badge Management
   - Track progress
   - Handle states
   - Validate awards
   - Cache data

2. Progress Tracking
   - Monitor metrics
   - Update states
   - Handle timing
   - Validate completion

3. Performance
   - Batch updates
   - Cache states
   - Handle timing
   - Optimize checks

4. Error Handling
   - Validate badges
   - Handle failures
   - Recover states
   - Log issues

## Common Use Cases

1. Badge Types
   - Achievements
   - Milestones
   - Challenges
   - Collections

2. Badge Features
   - Progress tracking
   - State management
   - Notifications
   - Rewards

3. Badge Systems
   - Meta achievements
   - Point systems
   - Leaderboards
   - Rewards

4. Badge Processing
   - Progress validation
   - State management
   - Event handling
   - Reward distribution
