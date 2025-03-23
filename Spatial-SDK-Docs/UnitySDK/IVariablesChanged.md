# IVariablesChanged

Category: Interfaces

Interface

An interface to handle network variable changes in network objects. Implementing this interface allows a class to receive notifications when network variables are changed.

## Methods

| Method | Description |
| --- | --- |
| OnVariablesChanged(NetworkObjectVariablesChangedEventArgs args) | Called when one or more network variables on the object change. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections.Generic;

// Example: Network object with synchronized variables
public class SynchronizedColorObject : SpatialNetworkBehaviour, IVariablesChanged
{
    private Color objectColor = Color.white;
    private MeshRenderer meshRenderer;
    
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateVisuals();
    }
    
    public override void Spawned()
    {
        base.Spawned();
        
        // If we're the owner, initialize with a random color
        if (IsOwner)
        {
            SetRandomColor();
        }
    }
    
    // Implement the IVariablesChanged interface method
    public void OnVariablesChanged(NetworkObjectVariablesChangedEventArgs args)
    {
        // Check if any color components have changed
        bool colorChanged = false;
        
        if (args.changedVariables.TryGetValue("colorR", out var r))
        {
            objectColor.r = (float)r;
            colorChanged = true;
        }
        
        if (args.changedVariables.TryGetValue("colorG", out var g))
        {
            objectColor.g = (float)g;
            colorChanged = true;
        }
        
        if (args.changedVariables.TryGetValue("colorB", out var b))
        {
            objectColor.b = (float)b;
            colorChanged = true;
        }
        
        if (args.changedVariables.TryGetValue("colorA", out var a))
        {
            objectColor.a = (float)a;
            colorChanged = true;
        }
        
        // Update visual appearance if any color component changed
        if (colorChanged)
        {
            UpdateVisuals();
            
            // Log which client made the change
            int changerActorNumber = Owner;
            Debug.Log($"Color changed by Actor {changerActorNumber} to {objectColor}");
        }
    }
    
    // Update the visual appearance based on current color
    private void UpdateVisuals()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.color = objectColor;
        }
    }
    
    // Set a new random color (owner-only operation)
    public void SetRandomColor()
    {
        if (IsOwner)
        {
            Color newColor = new Color(
                Random.value,
                Random.value,
                Random.value,
                1.0f
            );
            
            SetNetworkVariable("colorR", newColor.r);
            SetNetworkVariable("colorG", newColor.g);
            SetNetworkVariable("colorB", newColor.b);
            SetNetworkVariable("colorA", newColor.a);
        }
        else
        {
            Debug.LogWarning("Cannot change color: not the owner of this object");
        }
    }
}

// Example: Network object that tracks player state
public class PlayerStateTracker : SpatialNetworkBehaviour, IVariablesChanged
{
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject powerupIndicator;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    
    private int health = 100;
    private int score = 0;
    private bool hasPowerup = false;
    
    public override void Spawned()
    {
        base.Spawned();
        
        // Initialize network variables if we're the owner
        if (IsOwner)
        {
            SetNetworkVariable("health", health);
            SetNetworkVariable("score", score);
            SetNetworkVariable("hasPowerup", hasPowerup);
        }
        
        UpdateUI();
    }
    
    public void OnVariablesChanged(NetworkObjectVariablesChangedEventArgs args)
    {
        bool uiNeedsUpdate = false;
        
        if (args.changedVariables.TryGetValue("health", out var newHealth))
        {
            health = (int)newHealth;
            uiNeedsUpdate = true;
            
            // Show damage or healing effect
            ShowHealthChangeEffect(health);
        }
        
        if (args.changedVariables.TryGetValue("score", out var newScore))
        {
            int oldScore = score;
            score = (int)newScore;
            uiNeedsUpdate = true;
            
            // Show score change effect
            if (score > oldScore)
            {
                ShowScoreIncreaseEffect(score - oldScore);
            }
        }
        
        if (args.changedVariables.TryGetValue("hasPowerup", out var newPowerupState))
        {
            bool oldPowerupState = hasPowerup;
            hasPowerup = (bool)newPowerupState;
            uiNeedsUpdate = true;
            
            // Show powerup effect
            if (!oldPowerupState && hasPowerup)
            {
                ShowPowerupActivatedEffect();
            }
            else if (oldPowerupState && !hasPowerup)
            {
                ShowPowerupDeactivatedEffect();
            }
        }
        
        if (uiNeedsUpdate)
        {
            UpdateUI();
        }
    }
    
    private void UpdateUI()
    {
        if (healthBar != null)
        {
            // Update health bar scale
            Vector3 scale = healthBar.transform.localScale;
            scale.x = Mathf.Clamp01(health / 100f);
            healthBar.transform.localScale = scale;
        }
        
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
        
        if (powerupIndicator != null)
        {
            powerupIndicator.SetActive(hasPowerup);
        }
    }
    
    // Example of modifying a network variable
    public void TakeDamage(int amount)
    {
        if (IsOwner)
        {
            health = Mathf.Max(0, health - amount);
            SetNetworkVariable("health", health);
            
            // Game logic for health reaching zero
            if (health <= 0)
            {
                Die();
            }
        }
    }
    
    // Example of modifying multiple network variables
    public void ActivatePowerup()
    {
        if (IsOwner && !hasPowerup)
        {
            hasPowerup = true;
            SetNetworkVariable("hasPowerup", true);
            
            // Schedule powerup deactivation
            StartCoroutine(DeactivatePowerupAfterDelay(10f));
        }
    }
    
    private System.Collections.IEnumerator DeactivatePowerupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (IsOwner && hasPowerup)
        {
            hasPowerup = false;
            SetNetworkVariable("hasPowerup", false);
        }
    }
    
    // Visual effect methods (simplified examples)
    private void ShowHealthChangeEffect(int newHealth)
    {
        // Create a floating text effect to indicate health change
        SpatialBridge.vfxService.CreateFloatingText(
            newHealth.ToString(),
            FloatingTextAnimStyle.Bouncy,
            transform.position + Vector3.up * 1.5f,
            Vector3.up,
            newHealth < 50 ? Color.red : Color.green,
            true
        );
    }
    
    private void ShowScoreIncreaseEffect(int amount)
    {
        // Create a floating text effect to indicate score increase
        SpatialBridge.vfxService.CreateFloatingText(
            $"+{amount}",
            FloatingTextAnimStyle.Simple,
            transform.position + Vector3.up * 2f,
            Vector3.up,
            Color.yellow,
            true
        );
    }
    
    private void ShowPowerupActivatedEffect()
    {
        // Create a particle effect and floating text for powerup activation
        Debug.Log("Powerup activated!");
    }
    
    private void ShowPowerupDeactivatedEffect()
    {
        // Visual indication that powerup has expired
        Debug.Log("Powerup expired!");
    }
    
    private void Die()
    {
        Debug.Log("Player died!");
        // Respawn logic, etc.
    }
}
```

## Best Practices

1. **Variable Handling**
   - Group related variables in the change handler for efficient updates
   - Cache old values to detect and respond to specific changes
   - Use meaningful variable names for better code readability
   - Consider using prefixes for different variable categories

2. **Performance Considerations**
   - Avoid expensive operations in the `OnVariablesChanged` callback
   - Batch visual updates to minimize performance impact
   - Only update UI or visual elements when relevant variables change
   - Remember that the `changedVariables` dictionary is reused between events, don't cache it

3. **Architecture Tips**
   - Separate network variable handling from game logic
   - Use private methods for specific variable type handling
   - Consider implementing state machines for complex state transitions
   - Use flags or counters to track when multiple related variables have changed

4. **Debugging**
   - Log significant variable changes during development
   - Implement visual debugging helpers for network variables
   - Create debug UI to display current network variable values
   - Use conditional logging based on development builds

## Common Use Cases

1. **Character State Synchronization**
   - Health and status effects
   - Inventory and equipment state
   - Character appearance customization
   - Ability cooldowns and resource values

2. **Interactive Object Synchronization**
   - Object appearance and properties
   - Puzzle state and progress
   - Door/switch states
   - Collectible status

3. **Game Systems**
   - Score and progress tracking
   - Team affiliation and status
   - Match state and timers
   - Environmental conditions

4. **UI and Feedback**
   - Player nameplates and status indicators
   - Progress bars and counters
   - Notification systems
   - Leaderboards and stats displays

## Completed: March 9, 2025
