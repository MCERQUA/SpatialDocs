# Top Down Shooter Template

## Overview
The Top Down Shooter template provides a ready-to-use implementation of a simple shooter game with a top-down perspective. It features automatic weapon targeting, enemy AI that follows the player, and a damage/health system. The template showcases a custom camera angle using Spatial's Virtual Camera system and demonstrates how to implement basic game mechanics like projectile collision detection and enemy spawning.

## Features
- **Custom Top-Down Camera**: Angled view of the gameplay area using Spatial's Virtual Camera system
- **Automatic Targeting System**: Player automatically targets and shoots at the closest enemy
- **Enemy AI**: Simple enemy behavior with pathfinding toward the player
- **Damage System**: Health tracking and damage application with visual feedback
- **Visual Effects**: Particle systems for weapons and hit reactions
- **Floating Text**: Damage numbers and XP notifications using Spatial's VFX service
- **Enemy Spawning**: Configurable enemy spawning system
- **Performance Optimized**: Built for web performance and scalability

## Included Components

### 1. Player Weapon System
The core weapon system for the player:
- Automatic targeting of the closest enemy
- Configurable fire rate (rounds per second)
- Customizable damage range
- Particle system-based projectiles
- Collision detection for damage application

### 2. Enemy Management
Complete system for enemy behavior and tracking:
- Central manager for tracking active enemies
- Health and damage system
- Hit reactions and death effects
- AI movement toward the player
- Registration/deregistration of enemies

### 3. Visual Feedback
Comprehensive visual feedback system:
- Floating damage numbers
- Hit effects
- Death effects
- XP gain notifications
- Particle-based weapon effects

### 4. Camera System
Top-down camera implementation:
- Fixed-angle view of the action
- Follows player movement
- Utilizes Spatial's Virtual Camera system
- Provides clear visibility of gameplay area

## Integration with SDK Components
The template integrates with these key SDK components:

| SDK Component | Usage in Template |
|---------------|-------------------|
| IActorService | Accessing the local player's avatar and position |
| IAvatar | Controlling and tracking player position |
| IVFXService | Creating floating text for damage numbers and notifications |
| SpatialBridge | Core connectivity to Spatial's systems |
| ParticleSystem | Weapon projectiles and visual effects |

## When to Use
Use this template when:
- Creating arcade-style shooter games
- Implementing games with automatic combat mechanics
- Building games with enemy AI that tracks the player
- Developing top-down perspective games
- Creating games with visual feedback for combat events
- Implementing projectile-based combat systems

## Implementation Details

### Weapon System Implementation
The player's weapon automatically targets and shoots at the closest enemy:

```csharp
public class PlayerWeapon : MonoBehaviour
{
    public Vector2Int damageRange;
    [Tooltip("Rounds per second")]
    public float rps;
    public int bullets = 1;

    private ParticleSystem ps;
    private float shotTimer;

    private void Update()
    {
        transform.position = SpatialBridge.actorService.localActor.avatar.position + Vector3.up;

        if (EnemiesManager.enemies.Count == 0)
            return;

        // Find closest enemy
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

    // Read collision events from attached particle system
    private void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.Hit(Random.Range(damageRange.x, damageRange.y));
        }
    }
}
```

### Enemy Movement Implementation
Enemies move toward the player using a simple AI system:

```csharp
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

### Damage System
The template includes a damage system for enemies:

```csharp
public class EnemyControl : MonoBehaviour, IDamageable
{
    public int initialHealth = 10;
    private int health;

    public Action OnTakeDamage;
    public Action OnDie;

    public void Hit(int damage)
    {
        health -= damage;
        OnTakeDamage?.Invoke();
        VFXManager.HitVFX(transform.position);
        
        // Damage numbers
        SpatialBridge.vfxService.CreateFloatingText(
            "<size=20><b>" + damage.ToString(), 
            FloatingTextAnimStyle.Bouncy, 
            transform.position + UnityEngine.Random.insideUnitSphere,
            Vector3.up * 13f, 
            Color.white
        );
        
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
        SpatialBridge.vfxService.CreateFloatingText(
            "<color=yellow><b><i><size=14>+15xp", 
            FloatingTextAnimStyle.Bouncy, 
            transform.position + UnityEngine.Random.insideUnitSphere,
            Vector3.up * 3f, 
            Color.white
        );
        
        GameObject.Destroy(gameObject);
    }
}
```

## Best Practices
- **Optimize Enemy Count**: Limit the number of active enemies based on your target platform
- **Balance Difficulty**: Adjust enemy movement speed and spawn rates for appropriate challenge
- **Use Spatial's VFX Service**: Leverage the built-in floating text for feedback rather than creating custom UI
- **Extend the Combat System**: Consider adding power-ups, different weapon types, or enemy variations
- **Add Visual Polish**: Enhance the experience with additional particle effects for impacts and deaths
- **Implement Scoring**: Add a scoring system to track player progress
- **Consider Multiplayer**: Extend the template to support multiplayer by synchronizing enemy positions and health
- **Audio Feedback**: Add sound effects for shooting, hits, and enemy deaths to enhance the experience

## Related Templates
- [Camera Modes](../Camera/CameraModes.md) - For alternative camera perspectives in your shooter
- [Gem Collection Game](./GemCollectionGame.md) - For implementing collectible items in the game world
- [Daily/Weekly Rewards](../UX/DailyRewards.md) - For adding reward systems to your shooter

## Additional Resources
- [GitHub Repository](https://github.com/spatialsys/spatial-example-topdown-shooter)
- [Spatial Virtual Camera Documentation](https://docs.spatial.io/virtual-camera)
- [Floating Text Documentation](https://cs.spatial.io/api/SpatialSys.UnitySDK.IVFXService.CreateFloatingText.html)
