# Spatial Top-down Shooter Sample Template

## Template Overview
The Spatial Top-down Shooter Sample template demonstrates a simple top-down shooter game where the player automatically shoots at the nearest enemy. Enemies spawn around the player and move towards them. The template showcases particle system collision detection, floating text for damage numbers, and custom camera angles using Spatial Virtual Camera.

## Template Information
- Location: `E:\1-ECHO-WORKING-FOLDER\Spatial-Templates\spatial-example-topdown-shooter`
- Repository URL: https://github.com/spatialsys/spatial-example-topdown-shooter
- Key Topics:
  - Custom Camera Angle using Spatial Virtual Camera
  - Enemies that follow the player
  - Instantiating and destroying gameobjects
  - Using particle system collision detection
  - Floating Text for damage numbers

## Directory Structure
```
E:\1-ECHO-WORKING-FOLDER\Spatial-Templates\spatial-example-topdown-shooter\
├── LICENSE
├── README.md
└── spatial-example-topdown-shooter-unity\
    ├── Assets\
    │   ├── Art\
    │   ├── Enemy.prefab
    │   ├── Scripts\
    │   │   ├── Enemies\
    │   │   │   ├── EnemiesManager.cs
    │   │   │   ├── EnemyControl.cs
    │   │   │   ├── EnemyHitReaction.cs
    │   │   │   └── MoveTowardsPlayer.cs
    │   │   ├── EnemySpawner.cs
    │   │   ├── IDamageable.cs
    │   │   ├── PlayerWeapon.cs
    │   │   ├── SnapToLocalAvatar.cs
    │   │   ├── TopdownShooterScripts.asmdef
    │   │   └── VFXManager.cs
    │   ├── Spatial\
    │   ├── TextMesh Pro\
    │   └── TopdownShooter.unity
    ├── Packages\
    └── ProjectSettings\
```

## C# Scripts
### Core Game Scripts

#### PlayerWeapon.cs
```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

[RequireComponent(typeof(ParticleSystem))]
public class PlayerWeapon : MonoBehaviour
{
    public Vector2Int damageRange;
    [Tooltip("Rounds per second")]
    public float rps;
    public int bullets = 1;

    private ParticleSystem ps;
    private float shotTimer;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        transform.position = SpatialBridge.actorService.localActor.avatar.position + Vector3.up;

        if (EnemiesManager.enemies.Count == 0)
            return;

        //find closest enemy (slow)
        float minDistance = float.MaxValue;
        EnemyControl closestEnemy = null;
        foreach (var enemy in EnemiesManager.enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }

        shotTimer += Time.deltaTime;
        if (shotTimer >= 1f / rps)
        {
            shotTimer -= 1f / rps;
            if (closestEnemy != null)
            {
                Vector3 dir = closestEnemy.transform.position - transform.position;
                dir.y = 0;
                transform.rotation = Quaternion.LookRotation(dir);
                ps.Emit(bullets);
            }
        }
    }

    // read collision events from attached particle system
    private void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.Hit(Random.Range(damageRange.x, damageRange.y));
        }
    }
}
```

**Purpose**: Controls the player's weapon, which automatically targets and shoots at the nearest enemy.

**Spatial SDK Usage**: 
- Uses `SpatialBridge.actorService.localActor.avatar.position` to position the weapon

**Key Features**:
- Automatically finds the closest enemy
- Rotates to face the target
- Fires at a configurable rate
- Uses particle system collision for hit detection
- Applies random damage within a specified range

#### EnemySpawner.cs
```csharp
using SpatialSys.UnitySDK;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawnPerSecond = 5f;
    [Tooltip("How far from the player the enemies will spawn.")]
    public float spawnRadius = 10f;
    public int maxEnemies = 10;
    public GameObject enemyPrefab;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f / spawnPerSecond)
        {
            timer -= 1f / spawnPerSecond;
            if (EnemiesManager.enemies.Count < maxEnemies)
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        Vector3 pos = SpatialBridge.actorService.localActor.avatar.position;
        Vector2 randomPos = Random.onUnitSphere * spawnRadius;
        pos.x += randomPos.x;
        pos.z += randomPos.y;
        pos.y = 0f;

        GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
    }
}
```

**Purpose**: Spawns enemies around the player at a configurable rate, up to a maximum limit.

**Spatial SDK Usage**: 
- Uses `SpatialBridge.actorService.localActor.avatar.position` to determine spawn positions

**Key Features**:
- Controls enemy spawn rate
- Maintains maximum enemy count
- Spawns enemies at a configurable distance from the player
- Distributes enemies in a circle around the player

#### VFXManager.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using SpatialSys.UnitySDK;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager instance;
    public ParticleSystem enemyHitVFX;
    public ParticleSystem enemyDieVFX;

    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public static void HitVFX(Vector3 position)
    {
        instance.enemyHitVFX.transform.position = position;
        instance.enemyHitVFX.transform.rotation = Quaternion.LookRotation(position - SpatialBridge.actorService.localActor.avatar.position);
        instance.enemyHitVFX.Emit(5);
    }

    public static void DieVFX(Vector3 position)
    {
        instance.enemyDieVFX.transform.position = position;
        instance.enemyDieVFX.Emit(1);
    }
}
```

**Purpose**: Manages visual effects for enemy hits and deaths.

**Spatial SDK Usage**: 
- Uses `SpatialBridge.actorService.localActor.avatar.position` to orient hit effects

**Key Features**:
- Singleton pattern for global access
- Plays particle effects at enemy positions
- Orients hit effects toward the player

#### SnapToLocalAvatar.cs
```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class SnapToLocalAvatar : MonoBehaviour
{
    private IAvatar localAvatar => SpatialBridge.actorService.localActor.avatar;
    void LateUpdate()
    {
        if (SpatialBridge.actorService.localActor == null || localAvatar == null) return;
        transform.position = localAvatar.position;
    }
}
```

**Purpose**: Makes a GameObject follow the player's avatar position.

**Spatial SDK Usage**: 
- Uses `SpatialBridge.actorService.localActor.avatar` to access the local avatar
- Uses `localAvatar.position` to get the avatar's position

**Key Features**:
- Updates object position in LateUpdate (after normal Update)
- Null-checks the avatar to avoid errors

### Enemy Scripts

#### IDamageable.cs
```csharp
public interface IDamageable
{
    void Hit(int damage);
}
```

**Purpose**: Interface for objects that can take damage.

#### EnemiesManager.cs
```csharp
using System.Collections.Generic;
using UnityEngine;

// Keep track of all active enemies.
// Using a monoBehavior so we can clear static data when the game is destroyed
public class EnemiesManager : MonoBehaviour
{
    public static List<EnemyControl> enemies { get; private set; }

    private void Awake()
    {
        enemies = new List<EnemyControl>();
    }

    private void OnDestroy()
    {
        // Always clear static memory when the game is destroyed
        enemies = null;
    }

    public static void RegisterEnemy(EnemyControl enemy)
    {
        enemies.Add(enemy);
    }

    public static void DeregisterEnemy(EnemyControl enemy)
    {
        if (enemies != null)
        {
            enemies.Remove(enemy);
        }
    }
}
```

**Purpose**: Manages a global registry of all active enemies.

**Key Features**:
- Maintains a static list of all enemies in the scene
- Properly cleans up static references on destroy
- Provides registration/deregistration methods

#### EnemyControl.cs
```csharp
using System;
using SpatialSys.UnitySDK;
using UnityEngine;

public class EnemyControl : MonoBehaviour, IDamageable
{
    public int initialHealth = 10;
    private int health;

    public Action OnTakeDamage;
    public Action OnDie;

    private void OnEnable()
    {
        health = initialHealth;
        EnemiesManager.RegisterEnemy(this);
    }

    private void OnDisable()
    {
        EnemiesManager.DeregisterEnemy(this);
    }

    public void Hit(int damage)
    {
        health -= damage;
        OnTakeDamage?.Invoke();
        VFXManager.HitVFX(transform.position);
        // Damage numbers
        SpatialBridge.vfxService.CreateFloatingText("<size=20><b>" + damage.ToString(), FloatingTextAnimStyle.Bouncy, transform.position + UnityEngine.Random.insideUnitSphere , Vector3.up * 13f, Color.white);
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDie?.Invoke();
        VFXManager.DieVFX(transform.position);
        // Fake XP earned
        SpatialBridge.vfxService.CreateFloatingText("<color=yellow><b><i><size=14>+15xp", FloatingTextAnimStyle.Bouncy, transform.position + UnityEngine.Random.insideUnitSphere , Vector3.up * 3f, Color.white);
        GameObject.Destroy(gameObject);
    }
}
```

**Purpose**: Controls enemy health, damage, and death.

**Spatial SDK Usage**: 
- Uses `SpatialBridge.vfxService.CreateFloatingText` to show damage numbers and XP gain

**Key Features**:
- Implements IDamageable interface
- Handles health management
- Registers with EnemiesManager
- Triggers visual effects and floating text
- Destroys itself on death

#### EnemyHitReaction.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make the enemy flash when hit
public class EnemyHitReaction : MonoBehaviour
{
    public Material defaultMaterial;
    public Material hitMaterial;
    public MeshRenderer meshRenderer;

    public EnemyControl enemyControl;

    public float hitDuration = 0.1f;

    private Coroutine hitCoroutine;
    private float hitTimer;

    private void OnEnable()
    {
        enemyControl.OnTakeDamage += OnDamage;
    }

    private void OnDisable()
    {
        enemyControl.OnTakeDamage -= OnDamage;
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }
    }

    private void OnDamage()
    {
        hitTimer = hitDuration;
        if (hitCoroutine == null)
        {
            hitCoroutine = StartCoroutine(HitCoroutine());
        }
    }

    private IEnumerator HitCoroutine()
    {
        meshRenderer.material = hitMaterial;
        while (hitTimer > 0)
        {
            hitTimer -= Time.deltaTime;
            yield return null;
        }
        meshRenderer.material = defaultMaterial;
    }
}
```

**Purpose**: Creates a visual flash effect when an enemy is hit.

**Key Features**:
- Subscribes to the enemy's damage event
- Temporarily changes the enemy's material
- Uses coroutine for timed effect
- Properly cleans up on disable

#### MoveTowardsPlayer.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;

public class MoveTowardsPlayer : MonoBehaviour
{
    public float moveSpeed = 3f;

    private IAvatar localAvatar => SpatialBridge.actorService.localActor.avatar;

    void Update()
    {
        if (SpatialBridge.actorService.localActor == null || localAvatar == null) return;
        
        Vector3 move = (localAvatar.position - transform.position).normalized * moveSpeed * Time.deltaTime;
        move.y = 0;
        transform.position += move;
    }
}
```

**Purpose**: Makes enemies move toward the player's avatar.

**Spatial SDK Usage**: 
- Uses `SpatialBridge.actorService.localActor.avatar` to access the local avatar
- Uses `localAvatar.position` to get the avatar's position

**Key Features**:
- Moves at configurable speed
- Moves only on the XZ plane (no vertical movement)
- Normalizes direction for consistent speed regardless of distance

## Key System Features

### 1. Player Weapon System
- Automatically targets the closest enemy
- Uses particle system for bullets
- Implements collision detection via OnParticleCollision
- Rotates to face the current target
- Configurable fire rate and damage range

### 2. Enemy Management
- Central registry of all active enemies
- Spawn system with configurable rate and maximum count
- Health and damage system
- Visual feedback for hits and deaths
- Movement AI that follows the player

### 3. Visual Effects
- Particle effects for hits and death
- Material changes for hit feedback
- Floating text for damage numbers using SpatialBridge.vfxService
- XP gain notifications on enemy death

### 4. Avatar Integration
- Game elements follow the player's avatar
- Enemies spawn relative to avatar position
- Weapon position and rotation based on avatar

## Integration with Spatial SDK
The template uses several Spatial SDK features:

1. **Actor/Avatar System**
   - `SpatialBridge.actorService.localActor.avatar` to get the local player's avatar
   - `avatar.position` to position game elements relative to the player

2. **VFX Service**
   - `SpatialBridge.vfxService.CreateFloatingText` for displaying damage numbers and XP gain
   - Configures various text styles, sizes, and colors
   - Uses different animation styles (Bouncy)

3. **Virtual Camera**
   - Template uses Spatial Virtual Camera for top-down view
   - Camera follows the player's avatar

## How to Use This Template

### Core Components
1. **TopdownShooter.unity**: Main scene containing the complete setup
2. **Enemy.prefab**: Prefab for spawnable enemies
3. **PlayerWeapon**: Component attached to the player's avatar that handles shooting

### Customization Options
1. **Enemies**:
   - Adjust spawn rate, maximum number, and spawn radius in EnemySpawner
   - Modify enemy health and movement speed
   - Customize hit reactions and visual effects

2. **Weapon**:
   - Adjust fire rate (rps), damage range, and bullet count
   - Modify the particle system for different visual effects
   - Change targeting logic in the Update method

3. **Visual Effects**:
   - Replace particle effects for hits and deaths
   - Customize floating text appearance and animation
   - Modify hit reaction materials and duration

## Source Verification
- Documentation created on: March 22, 2025
- Source files last accessed: March 22, 2025
- Documentation matches source: ✓
