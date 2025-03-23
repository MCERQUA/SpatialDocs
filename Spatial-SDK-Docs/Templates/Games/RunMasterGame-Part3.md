# Run Master Game Template (Part 3)

_This is a continuation from [RunMasterGame-Part2.md](./RunMasterGame-Part2.md)_

## Multiplayer Race Manager (continued)

```csharp
[Rpc(RpcTarget.Server)]
private void UpdateProgress(float progress)
{
    int actorId = RpcSender.ActorId;
    
    if (playerInfos.TryGetValue(actorId, out PlayerRaceInfo info))
    {
        info.progress = progress;
        
        // Update info for all clients
        SynchronizePlayerInfo(actorId, info);
    }
}

[Rpc(RpcTarget.Server)]
private void NotifyFinished(float finishTime)
{
    int actorId = RpcSender.ActorId;
    
    if (playerInfos.TryGetValue(actorId, out PlayerRaceInfo info))
    {
        info.finishTime = finishTime;
        
        // Update info for all clients
        SynchronizePlayerInfo(actorId, info);
        
        // Check if all players finished
        CheckAllPlayersFinished();
    }
}

private void CheckAllPlayersFinished()
{
    bool allFinished = true;
    
    foreach (var player in playerInfos.Values)
    {
        if (player.finishTime < 0)
        {
            allFinished = false;
            break;
        }
    }
    
    if (allFinished)
    {
        // All players have finished
        currentRaceState.Value = RaceState.Finished;
        
        // Calculate race results
        CalculateRaceResults();
    }
}

private void CalculateRaceResults()
{
    // Sort players by finish time
    List<PlayerRaceInfo> sortedPlayers = playerInfos.Values.ToList();
    sortedPlayers.Sort((a, b) => a.finishTime.CompareTo(b.finishTime));
    
    // Assign places
    for (int i = 0; i < sortedPlayers.Count; i++)
    {
        sortedPlayers[i].place = i + 1;
        
        // Update for all clients
        SynchronizePlayerInfo(sortedPlayers[i].actorId, sortedPlayers[i]);
    }
}

private void UpdateRaceStatusUI()
{
    if (raceStatusText == null)
        return;
        
    StringBuilder sb = new StringBuilder();
    
    // Sort players by progress or finish time
    List<PlayerRaceInfo> sortedPlayers = playerInfos.Values.ToList();
    
    if (currentRaceState.Value == RaceState.Racing || currentRaceState.Value == RaceState.Waiting)
    {
        // Sort by progress
        sortedPlayers.Sort((a, b) => b.progress.CompareTo(a.progress));
        
        sb.AppendLine("RACE PROGRESS:");
        
        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            PlayerRaceInfo player = sortedPlayers[i];
            
            if (player.finishTime >= 0)
            {
                // Player has finished
                sb.AppendLine($"{i+1}. {player.playerName}: FINISHED ({player.finishTime:F2}s)");
            }
            else
            {
                // Player still racing
                sb.AppendLine($"{i+1}. {player.playerName}: {player.progress:P0}");
            }
        }
    }
    else if (currentRaceState.Value == RaceState.Finished)
    {
        // Sort by finish time
        sortedPlayers.Sort((a, b) => a.finishTime.CompareTo(b.finishTime));
        
        sb.AppendLine("RACE RESULTS:");
        
        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            PlayerRaceInfo player = sortedPlayers[i];
            sb.AppendLine($"{i+1}. {player.playerName}: {player.finishTime:F2}s");
        }
    }
    
    raceStatusText.text = sb.ToString();
}

private void OnRaceStartedChanged(bool oldValue, bool newValue)
{
    if (newValue)
    {
        // Race has started
        localStartTime = Time.time;
        
        // Start the run
        RunMasterGameManager.Instance.StartGame();
    }
}

private void OnRaceStateChanged(RaceState oldValue, RaceState newValue)
{
    // Update UI based on race state
    switch (newValue)
    {
        case RaceState.Waiting:
            if (countdownText != null) countdownText.gameObject.SetActive(false);
            break;
            
        case RaceState.Countdown:
            if (countdownText != null) countdownText.gameObject.SetActive(true);
            break;
            
        case RaceState.Racing:
            if (countdownText != null) 
            {
                countdownText.text = "GO!";
                
                // Hide countdown after a short delay
                StartCoroutine(HideCountdownAfterDelay(1f));
            }
            break;
            
        case RaceState.Finished:
            // Show race results
            break;
    }
}

private IEnumerator HideCountdownAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    
    if (countdownText != null) 
    {
        countdownText.gameObject.SetActive(false);
    }
}

[System.Serializable]
private class PlayerRaceInfo
{
    public string playerName;
    public int actorId;
    public float progress;
    public float finishTime;
    public int place;
}

public enum RaceState
{
    Waiting,
    Countdown,
    Racing,
    Finished
}
```

## Analytics System

The Run Master Game template includes a comprehensive analytics system to track player behavior and game performance:

```csharp
public class RunnerAnalytics : MonoBehaviour
{
    [Header("Analytics Settings")]
    [SerializeField] private bool enableAnalytics = true;
    [SerializeField] private float sessionTimeThreshold = 60f; // 1 minute
    
    // Session tracking
    private float sessionStartTime;
    private float lastSessionLength;
    private int sessionsPlayed;
    private int runsCompleted;
    
    // Game metrics
    private int totalGems;
    private int totalDistance;
    private int highestScore;
    private int powerupsCollected;
    private Dictionary<string, int> obstacleCollisions = new Dictionary<string, int>();
    
    // Retention metrics
    private string firstPlayDate;
    private string lastPlayDate;
    private int consecutiveDays;
    
    private const string ANALYTICS_SAVE_KEY = "run_master_analytics";
    
    private void Start()
    {
        // Load analytics data
        LoadAnalyticsData();
        
        // Record session start
        sessionStartTime = Time.time;
        
        // Record play date
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        
        if (string.IsNullOrEmpty(firstPlayDate))
        {
            firstPlayDate = today;
        }
        
        // Check consecutive days
        if (lastPlayDate != today)
        {
            if (IsConsecutiveDay(lastPlayDate, today))
            {
                consecutiveDays++;
            }
            else
            {
                consecutiveDays = 1;
            }
            
            lastPlayDate = today;
        }
        
        // Save analytics data
        SaveAnalyticsData();
        
        // Register for game events
        RegisterForGameEvents();
    }
    
    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            // Record session data before app goes to background
            RecordSessionData();
        }
        else
        {
            // Coming back from background, start new session
            sessionStartTime = Time.time;
        }
    }
    
    private void OnApplicationQuit()
    {
        // Record session data before app quits
        RecordSessionData();
    }
    
    private void RecordSessionData()
    {
        if (!enableAnalytics)
            return;
            
        // Calculate session length
        float sessionLength = Time.time - sessionStartTime;
        
        // Only count sessions longer than threshold
        if (sessionLength >= sessionTimeThreshold)
        {
            lastSessionLength = sessionLength;
            sessionsPlayed++;
            
            // Save analytics data
            SaveAnalyticsData();
        }
    }
    
    private void RegisterForGameEvents()
    {
        // Listen for game events
        RunMasterGameManager.Instance.OnGameOver += OnGameOver;
        RunMasterGameManager.Instance.OnGemCollected += OnGemCollected;
        RunMasterGameManager.Instance.OnPowerupCollected += OnPowerupCollected;
        RunMasterGameManager.Instance.OnObstacleHit += OnObstacleHit;
    }
    
    private void OnGameOver(int score, int distance)
    {
        if (!enableAnalytics)
            return;
            
        // Record run stats
        runsCompleted++;
        totalDistance += distance;
        
        if (score > highestScore)
        {
            highestScore = score;
        }
        
        // Save analytics data
        SaveAnalyticsData();
        
        // Send run completion event
        SendAnalyticsEvent("run_completed", new Dictionary<string, object>
        {
            { "score", score },
            { "distance", distance },
            { "gems_collected", totalGems },
            { "powerups_collected", powerupsCollected },
            { "obstacle_collisions", obstacleCollisions }
        });
    }
    
    private void OnGemCollected()
    {
        if (!enableAnalytics)
            return;
            
        totalGems++;
    }
    
    private void OnPowerupCollected(PowerupType type)
    {
        if (!enableAnalytics)
            return;
            
        powerupsCollected++;
    }
    
    private void OnObstacleHit(string obstacleType)
    {
        if (!enableAnalytics)
            return;
            
        // Track obstacle collisions by type
        if (obstacleCollisions.ContainsKey(obstacleType))
        {
            obstacleCollisions[obstacleType]++;
        }
        else
        {
            obstacleCollisions.Add(obstacleType, 1);
        }
    }
    
    private bool IsConsecutiveDay(string lastDate, string currentDate)
    {
        if (string.IsNullOrEmpty(lastDate))
            return false;
            
        // Parse dates
        if (DateTime.TryParse(lastDate, out DateTime last) && 
            DateTime.TryParse(currentDate, out DateTime current))
        {
            // Check if current date is exactly one day after last date
            return (current - last).TotalDays == 1;
        }
        
        return false;
    }
    
    private void SaveAnalyticsData()
    {
        // Create analytics data object
        AnalyticsData data = new AnalyticsData
        {
            firstPlayDate = firstPlayDate,
            lastPlayDate = lastPlayDate,
            sessionsPlayed = sessionsPlayed,
            runsCompleted = runsCompleted,
            totalGems = totalGems,
            totalDistance = totalDistance,
            highestScore = highestScore,
            powerupsCollected = powerupsCollected,
            obstacleCollisions = obstacleCollisions,
            lastSessionLength = lastSessionLength,
            consecutiveDays = consecutiveDays
        };
        
        // Convert to JSON and save
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(ANALYTICS_SAVE_KEY, json);
        PlayerPrefs.Save();
    }
    
    private void LoadAnalyticsData()
    {
        // Load analytics data from JSON
        string json = PlayerPrefs.GetString(ANALYTICS_SAVE_KEY, "");
        
        if (!string.IsNullOrEmpty(json))
        {
            AnalyticsData data = JsonUtility.FromJson<AnalyticsData>(json);
            
            if (data != null)
            {
                firstPlayDate = data.firstPlayDate;
                lastPlayDate = data.lastPlayDate;
                sessionsPlayed = data.sessionsPlayed;
                runsCompleted = data.runsCompleted;
                totalGems = data.totalGems;
                totalDistance = data.totalDistance;
                highestScore = data.highestScore;
                powerupsCollected = data.powerupsCollected;
                obstacleCollisions = data.obstacleCollisions;
                lastSessionLength = data.lastSessionLength;
                consecutiveDays = data.consecutiveDays;
            }
        }
    }
    
    private void SendAnalyticsEvent(string eventName, Dictionary<string, object> parameters)
    {
        // In a real implementation, this would send the event to a analytics service
        // like Firebase Analytics, GameAnalytics, or a custom backend
        
        // For Spatial, we could use the IUserWorldDataStoreService to store analytics
        // events for later processing
    }
    
    [System.Serializable]
    private class AnalyticsData
    {
        public string firstPlayDate;
        public string lastPlayDate;
        public int sessionsPlayed;
        public int runsCompleted;
        public int totalGems;
        public int totalDistance;
        public int highestScore;
        public int powerupsCollected;
        public Dictionary<string, int> obstacleCollisions;
        public float lastSessionLength;
        public int consecutiveDays;
    }
}
```

## In-App Purchase Integration

The Run Master Game template includes a simple in-app purchase system:

```csharp
public class RunnerIAPManager : MonoBehaviour
{
    [System.Serializable]
    public class IAPItem
    {
        public string id;
        public string displayName;
        public string description;
        public string price;
        public int gemAmount;
        public Sprite icon;
    }
    
    [Header("IAP Items")]
    [SerializeField] private IAPItem[] iapItems;
    
    [Header("UI References")]
    [SerializeField] private Transform iapItemContainer;
    [SerializeField] private GameObject iapItemPrefab;
    
    // Cached Spatial Bridge reference
    private ISpatialBridge bridge;
    
    private void Start()
    {
        // Get Spatial Bridge reference
        bridge = SpatialBridge.GetInstance();
        
        // Initialize UI
        PopulateIAPItems();
    }
    
    private void PopulateIAPItems()
    {
        // Clear existing items
        foreach (Transform child in iapItemContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Add IAP items
        foreach (var item in iapItems)
        {
            GameObject itemGO = Instantiate(iapItemPrefab, iapItemContainer);
            IAPItemUI itemUI = itemGO.GetComponent<IAPItemUI>();
            
            if (itemUI != null)
            {
                // Set up UI
                itemUI.SetItem(item);
                
                // Register click event
                itemUI.OnPurchaseClicked += () => PurchaseItem(item);
            }
        }
    }
    
    private void PurchaseItem(IAPItem item)
    {
        // In a real implementation, this would use the platform's IAP APIs
        // For now, just use a mock purchase that always succeeds
        
        // Show purchase confirmation
        ShowPurchaseConfirmation(item, () => {
            // Mock successful purchase
            OnPurchaseCompleted(item);
        });
    }
    
    private void ShowPurchaseConfirmation(IAPItem item, Action onConfirm)
    {
        // Show confirmation UI
        // ...
        
        // For now, just confirm immediately
        onConfirm?.Invoke();
    }
    
    private void OnPurchaseCompleted(IAPItem item)
    {
        // Award gems to player
        RunMasterGameManager.Instance.AddCurrency(item.gemAmount);
        
        // Show purchase success message
        ShowPurchaseSuccessMessage(item);
        
        // Log purchase analytics
        LogPurchaseAnalytics(item);
    }
    
    private void ShowPurchaseSuccessMessage(IAPItem item)
    {
        // Show success message
        // ...
    }
    
    private void LogPurchaseAnalytics(IAPItem item)
    {
        // Log purchase analytics
        // In a real implementation, this would send data to an analytics service
    }
}
```

## Mobile Input System

A specialized input system for mobile platforms:

```csharp
public class RunnerMobileInput : MonoBehaviour
{
    [Header("Swipe Settings")]
    [SerializeField] private float minSwipeDistance = 50f;
    [SerializeField] private float maxSwipeTime = 0.5f;
    [SerializeField] private float swipeCooldown = 0.1f;
    
    [Header("Tap Settings")]
    [SerializeField] private float maxTapDistance = 10f;
    [SerializeField] private float maxTapTime = 0.5f;
    
    // Swipe detection
    private Vector2 startTouchPosition;
    private float startTouchTime;
    private bool isSwiping = false;
    private float lastSwipeTime = 0f;
    
    // Events
    public event Action OnJump;
    public event Action OnSlide;
    public event Action<int> OnLaneChange; // -1 for left, 1 for right
    
    private void Update()
    {
        // Skip input handling if game is not playing
        if (RunMasterGameManager.Instance.GameState != GameState.Playing)
            return;
            
        // Handle touch input
        HandleTouchInput();
    }
    
    private void HandleTouchInput()
    {
        // Check for new touches
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Start tracking potential swipe
                    startTouchPosition = touch.position;
                    startTouchTime = Time.time;
                    isSwiping = true;
                    break;
                    
                case TouchPhase.Moved:
                    // Check for swipe
                    if (isSwiping && Time.time - lastSwipeTime > swipeCooldown)
                    {
                        Vector2 deltaPosition = touch.position - startTouchPosition;
                        float deltaTime = Time.time - startTouchTime;
                        
                        // Horizontal swipe detection
                        if (Mathf.Abs(deltaPosition.x) > minSwipeDistance && 
                            Mathf.Abs(deltaPosition.x) > Mathf.Abs(deltaPosition.y) &&
                            deltaTime < maxSwipeTime)
                        {
                            // Determine direction
                            int direction = deltaPosition.x > 0 ? 1 : -1; // 1 for right, -1 for left
                            
                            // Fire lane change event
                            OnLaneChange?.Invoke(direction);
                            
                            // Reset swipe tracking
                            isSwiping = false;
                            lastSwipeTime = Time.time;
                        }
                        
                        // Vertical swipe detection
                        else if (Mathf.Abs(deltaPosition.y) > minSwipeDistance && 
                                Mathf.Abs(deltaPosition.y) > Mathf.Abs(deltaPosition.x) &&
                                deltaTime < maxSwipeTime)
                        {
                            // Determine direction
                            bool isSwipeUp = deltaPosition.y > 0;
                            
                            if (isSwipeUp)
                            {
                                // Swipe up = jump
                                OnJump?.Invoke();
                            }
                            else
                            {
                                // Swipe down = slide
                                OnSlide?.Invoke();
                            }
                            
                            // Reset swipe tracking
                            isSwiping = false;
                            lastSwipeTime = Time.time;
                        }
                    }
                    break;
                    
                case TouchPhase.Ended:
                    // Check for tap
                    if (isSwiping)
                    {
                        Vector2 deltaPosition = touch.position - startTouchPosition;
                        float deltaTime = Time.time - startTouchTime;
                        
                        if (deltaPosition.magnitude < maxTapDistance && deltaTime < maxTapTime)
                        {
                            // This was a tap, not a swipe
                            // Tap = jump
                            OnJump?.Invoke();
                        }
                        
                        // Reset swipe tracking
                        isSwiping = false;
                    }
                    break;
            }
        }
    }
}
```

## Performance Optimization Tips

Here are additional performance optimization tips for using the Run Master Game template:

1. **Memory Management**:
   - Pre-instantiate objects during loading screens to avoid stuttering during gameplay
   - Limit texture sizes based on target platform (512×512 for low-end devices)

2. **Rendering Optimization**:
   - Enable occlusion culling for environment objects
   - Use distance-based LOD (Level of Detail) for complex objects
   - Implement frustum culling for objects outside camera view

3. **Mobile Battery Considerations**:
   - Adjust physics timestep on mobile platforms (use Time.fixedDeltaTime = 0.02f)
   - Implement an adaptive quality system based on device performance
   - Reduce particle effects on low-end devices

4. **Advanced Pooling Strategies**:
   - Implement a two-level object pool (active and reserve pools)
   - Pre-warm pools during level loading
   - Reset object state completely when returning to pool

5. **Draw Call Batching**:
   - Group similar materials to reduce draw calls
   - Use texture atlases for environment objects
   - Implement GPU instancing for repeated objects

## Extending the Template

The Run Master Game template is designed to be easily extended. Here are some ways to extend its functionality:

1. **Additional Game Modes**:
   - Time Trial Mode: Finish a fixed distance in the shortest time
   - Survival Mode: Last as long as possible with increasing difficulty
   - Collection Mode: Collect specific items while avoiding obstacles

2. **Social Features**:
   - Friend leaderboards
   - Daily challenges
   - Asynchronous multiplayer races against ghost players

3. **Progression Systems**:
   - Character skill upgrades
   - Environment unlocks
   - Special ability upgrades

4. **Monetization Options**:
   - Premium character skins
   - Booster packs
   - Remove ads option
   - Daily reward multipliers

## Example Customization

Here's an example of how to extend the template with a new obstacle type:

```csharp
public class MovingObstacle : Obstacle
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] private bool horizontal = true;
    
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float progress = 0f;
    private int direction = 1;
    
    protected override void Start()
    {
        base.Start();
        
        // Calculate movement positions
        startPosition = transform.position;
        
        if (horizontal)
        {
            endPosition = startPosition + Vector3.right * moveDistance;
        }
        else
        {
            endPosition = startPosition + Vector3.up * moveDistance;
        }
    }
    
    private void Update()
    {
        // Update movement
        progress += moveSpeed * Time.deltaTime * direction;
        
        // Check bounds
        if (progress > 1f)
        {
            progress = 1f;
            direction = -1;
        }
        else if (progress < 0f)
        {
            progress = 0f;
            direction = 1;
        }
        
        // Apply movement
        transform.position = Vector3.Lerp(startPosition, endPosition, progress);
    }
    
    protected override void OnHitPlayer(RunnerController player)
    {
        // Custom behavior: push player back instead of killing
        player.PushBack(5f);
        
        // Apply slow effect
        RunMasterGameManager.Instance.SlowDownPlayer(0.7f, 2f);
    }
}
```

## Final Tips

1. **Start Small**: Begin with a simplified version of the game before adding advanced features
2. **Test Frequently**: Test on target platforms early and often, especially mobile devices
3. **Optimize First**: Get the core gameplay running smoothly before adding visual polish
4. **User Feedback**: Gather player feedback to fine-tune difficulty and controls
5. **Analytics Integration**: Use analytics to identify player pain points and behavior patterns

## Conclusion

The Run Master Game template provides a solid foundation for creating endless runner games with the Spatial SDK. By following the best practices outlined here and leveraging the template's modular design, you can create engaging, high-performance endless runner games for both desktop and mobile platforms.

For more information, consult the full Spatial SDK documentation and the template's GitHub repository.
