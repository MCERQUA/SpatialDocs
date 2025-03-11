# Gem Collection Game Template

## Overview
The Gem Collection Game template provides a complete, ready-to-use implementation of a gem collection game where players explore an environment to find and collect gems. The template includes all necessary systems for gem spawning, collection mechanics, player scoring, and visual feedback. It demonstrates effective implementation of collectible gameplay mechanics and can serve as a foundation for exploration-based games, treasure hunts, or resource gathering mechanics.

## Features
- **Gem Collection System**: Core mechanics for collecting gems with visual and audio feedback
- **Scoring System**: Tracks player progress with a customizable UI
- **Multiple Gem Types**: Different gem types with varying point values and rarity
- **Spawning System**: Configurable spawning of gems in predefined locations or procedurally
- **Visual Effects**: Particle systems for collection events and gem highlighting
- **Audio Feedback**: Sound effects for gem collection and milestone achievements
- **Time Challenge Mode**: Optional time-limited collection challenges
- **Persistence**: Optional progress saving between play sessions

## Included Components

### 1. Gem System
Complete implementation of collectible gems:
- Different gem types with customizable properties
- Rotation and hover animations for visual appeal
- Glow effects and particle systems for visibility
- Proximity highlight system to draw player attention

### 2. Collection Mechanics
Core mechanics for detecting and processing gem collection:
- Trigger-based collection system
- Customizable collection radius
- Auto-collection option for accessibility
- Magnetic attraction to nearby player

### 3. UI System
User interface components for tracking progress:
- Gem counter with animated updates
- Score display with point accumulation effects
- Time remaining display for challenge mode
- Collection milestone notifications

### 4. Spawning System
Tools for placing gems in the game world:
- Manual placement with editor tools
- Pattern-based spawn system
- Random spawning within defined areas
- Respawn timer options for renewable resources

## Integration with SDK Components
The template integrates with these key SDK components:

| SDK Component | Usage in Template |
|---------------|-------------------|
| IActorService | Detecting player proximity to gems |
| IAvatar | Tracking player movement and position |
| SpatialTriggerEvent | For gem collection collision detection |
| IUserWorldDataStoreService | For saving collection progress (optional) |
| IVFXService | For creating collection effects and floating score text |
| IAudioService | For gem collection sounds and feedback |

## When to Use
Use this template when:
- Creating exploration-based gameplay
- Implementing collectible mechanics in any game genre
- Building treasure hunt or scavenger hunt experiences
- Developing resource gathering systems
- Creating educational games with collection objectives
- Implementing achievement-based progression systems

## Implementation Details

### Gem Implementation
The base gem class that handles core functionality:

```csharp
public class CollectibleGem : MonoBehaviour
{
    [Header("Gem Properties")]
    [SerializeField] private int pointValue = 10;
    [SerializeField] private GemType gemType = GemType.Common;
    [SerializeField] private bool rotateGem = true;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private bool hoverEffect = true;
    [SerializeField] private float hoverHeight = 0.2f;
    [SerializeField] private float hoverSpeed = 1f;
    
    [Header("Collection Effects")]
    [SerializeField] private ParticleSystem collectionEffect;
    [SerializeField] private AudioClip collectionSound;
    [SerializeField] private string collectionTextColor = "white";
    
    private Vector3 startPosition;
    private bool isCollected = false;
    private float hoverOffset = 0f;
    
    private void Start()
    {
        startPosition = transform.position;
        
        // Randomize starting position in hover cycle
        if (hoverEffect)
        {
            hoverOffset = Random.Range(0f, Mathf.PI * 2);
        }
    }
    
    private void Update()
    {
        if (isCollected)
            return;
            
        // Rotation effect
        if (rotateGem)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        
        // Hover effect
        if (hoverEffect)
        {
            float newY = startPosition.y + Mathf.Sin((Time.time + hoverOffset) * hoverSpeed) * hoverHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (isCollected)
            return;
            
        // Check if it's a player avatar
        if (other.TryGetComponent<SpatialAvatar>(out var avatar))
        {
            if (avatar.actorId == SpatialBridge.actorService.localActor.actorNumber)
            {
                CollectGem();
            }
        }
    }
    
    private void CollectGem()
    {
        isCollected = true;
        
        // Visual feedback
        if (collectionEffect != null)
        {
            Instantiate(collectionEffect, transform.position, Quaternion.identity);
        }
        
        // Audio feedback
        if (collectionSound != null)
        {
            SpatialBridge.audioService.PlaySoundEffect(collectionSound);
        }
        
        // Score popup
        SpatialBridge.vfxService.CreateFloatingText(
            $"<color={collectionTextColor}>+{pointValue}</color>",
            FloatingTextAnimStyle.Bouncy,
            transform.position,
            Vector3.up * 2f,
            Color.white
        );
        
        // Notify collection manager
        GemCollectionManager.Instance.GemCollected(gemType, pointValue);
        
        // Hide and destroy the gem
        gameObject.SetActive(false);
        Destroy(gameObject, 1f); // Delayed to allow effects to play
    }
}
```

### Collection Manager
The central system for tracking collected gems and managing game state:

```csharp
public class GemCollectionManager : MonoBehaviour
{
    public static GemCollectionManager Instance { get; private set; }
    
    [Header("Collection Settings")]
    [SerializeField] private int targetGemCount = 50;
    [SerializeField] private float timeLimit = 300f; // 5 minutes
    [SerializeField] private bool useTimeLimit = false;
    [SerializeField] private bool saveBetweenSessions = false;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI gemCountText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject timeUpPanel;
    
    // Collection tracking
    private int totalGemsCollected = 0;
    private int currentScore = 0;
    private Dictionary<GemType, int> gemsByType = new Dictionary<GemType, int>();
    private float remainingTime;
    private bool gameCompleted = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Initialize gem type tracking
        foreach (GemType type in Enum.GetValues(typeof(GemType)))
        {
            gemsByType[type] = 0;
        }
        
        // Initialize timer
        remainingTime = timeLimit;
        
        // Initialize UI
        UpdateUI();
        
        // Hide end game panels
        if (victoryPanel) victoryPanel.SetActive(false);
        if (timeUpPanel) timeUpPanel.SetActive(false);
    }
    
    private void Start()
    {
        if (saveBetweenSessions)
        {
            LoadProgress();
        }
    }
    
    private void Update()
    {
        if (gameCompleted)
            return;
            
        if (useTimeLimit)
        {
            remainingTime -= Time.deltaTime;
            
            if (remainingTime <= 0)
            {
                remainingTime = 0;
                TimeUp();
            }
            
            UpdateTimerDisplay();
        }
    }
    
    public void GemCollected(GemType type, int points)
    {
        if (gameCompleted)
            return;
            
        // Update counters
        totalGemsCollected++;
        currentScore += points;
        gemsByType[type]++;
        
        // Update UI
        UpdateUI();
        
        // Check for completion
        if (totalGemsCollected >= targetGemCount)
        {
            CompleteGame();
        }
        
        // Save progress if enabled
        if (saveBetweenSessions)
        {
            SaveProgress();
        }
    }
    
    private void UpdateUI()
    {
        if (gemCountText)
        {
            gemCountText.text = $"{totalGemsCollected} / {targetGemCount}";
        }
        
        if (scoreText)
        {
            scoreText.text = currentScore.ToString();
        }
    }
    
    private void UpdateTimerDisplay()
    {
        if (timerText)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
    
    private void CompleteGame()
    {
        gameCompleted = true;
        
        if (victoryPanel)
        {
            victoryPanel.SetActive(true);
        }
        
        // Additional victory logic here
    }
    
    private void TimeUp()
    {
        gameCompleted = true;
        
        if (timeUpPanel)
        {
            timeUpPanel.SetActive(true);
        }
        
        // Additional time-up logic here
    }
    
    private void SaveProgress()
    {
        // Save progress using IUserWorldDataStoreService
        var saveData = new Dictionary<string, object>
        {
            ["gems_collected"] = totalGemsCollected,
            ["current_score"] = currentScore,
            ["gems_by_type"] = gemsByType
        };
        
        // In a real implementation, this would save to the data store
    }
    
    private void LoadProgress()
    {
        // Load progress using IUserWorldDataStoreService
        // In a real implementation, this would load from the data store
    }
}
```

### Gem Spawner System
The system for spawning gems in the game world:

```csharp
public class GemSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] gemPrefabs;
    [SerializeField] private int gemsToSpawn = 50;
    [SerializeField] private bool useSpawnPoints = true;
    [SerializeField] private Transform[] spawnPoints;
    
    [Header("Random Spawn Settings")]
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(10, 0, 10);
    [SerializeField] private float minDistanceBetweenGems = 1.5f;
    [SerializeField] private float heightOffset = 1.0f;
    
    private List<Vector3> usedPositions = new List<Vector3>();
    
    private void Start()
    {
        SpawnGems();
    }
    
    private void SpawnGems()
    {
        if (useSpawnPoints && spawnPoints.Length > 0)
        {
            SpawnGemsAtDefinedPoints();
        }
        else
        {
            SpawnGemsRandomly();
        }
    }
    
    private void SpawnGemsAtDefinedPoints()
    {
        // Shuffle spawn points to randomize placement
        var shuffledPoints = spawnPoints.OrderBy(x => Random.value).ToArray();
        
        int pointCount = Mathf.Min(gemsToSpawn, shuffledPoints.Length);
        
        for (int i = 0; i < pointCount; i++)
        {
            SpawnGemAt(shuffledPoints[i].position);
        }
    }
    
    private void SpawnGemsRandomly()
    {
        for (int i = 0; i < gemsToSpawn; i++)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            SpawnGemAt(spawnPos);
        }
    }
    
    private void SpawnGemAt(Vector3 position)
    {
        // Choose random gem prefab
        int prefabIndex = Random.Range(0, gemPrefabs.Length);
        GameObject gemPrefab = gemPrefabs[prefabIndex];
        
        // Instantiate gem
        Instantiate(gemPrefab, position, Quaternion.identity, transform);
    }
    
    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 position;
        bool validPosition = false;
        int attempts = 0;
        
        do
        {
            // Get random position within spawn area
            float xPos = Random.Range(-spawnAreaSize.x/2, spawnAreaSize.x/2);
            float zPos = Random.Range(-spawnAreaSize.z/2, spawnAreaSize.z/2);
            
            position = transform.position + new Vector3(xPos, heightOffset, zPos);
            
            // Check if position is far enough from other gems
            validPosition = !usedPositions.Any(pos => Vector3.Distance(pos, position) < minDistanceBetweenGems);
            
            attempts++;
            
            // Safety check to prevent infinite loops
            if (attempts > 100) 
            {
                Debug.LogWarning("Too many spawn attempts, reducing distance constraint");
                minDistanceBetweenGems *= 0.9f;
                attempts = 0;
            }
        } 
        while (!validPosition);
        
        usedPositions.Add(position);
        return position;
    }
}
```

## Best Practices
- **Gem Visibility**: Ensure gems are clearly visible in the environment with glow effects or particle systems
- **Distribution Balance**: Spread gems throughout the level to encourage exploration
- **Difficulty Curve**: Place more valuable gems in harder-to-reach locations
- **Audio Feedback**: Use distinct audio cues for different gem types
- **Visual Polish**: Add particle effects and animations to make collection satisfying
- **Performance**: Optimize particle systems and limit active gem count for web performance
- **User Experience**: Add clear visual cues for collection radius and nearby gems
- **Progression**: Consider implementing a progression system where collecting certain gems unlocks new areas
- **Accessibility**: Include auto-collection options and clear visual differentiation between gem types

## Related Templates
- [Obby (Obstacle Course) Game](./ObbyGame.md) - For combining gem collection with platforming challenges
- [Top Down Shooter](./TopDownShooter.md) - For adding combat elements to your gem collection game
- [Daily/Weekly Rewards](../UX/DailyRewards.md) - For creating reward systems based on gem collection

## Additional Resources
- [GitHub Repository](https://github.com/spatialsys/spatial-gem-collection-game)
- [Spatial Trigger Events Documentation](../../SpatialSys/UnitySDK/SpatialTriggerEvent.md)
- [Floating Text Documentation](https://cs.spatial.io/api/SpatialSys.UnitySDK.IVFXService.CreateFloatingText.html)
