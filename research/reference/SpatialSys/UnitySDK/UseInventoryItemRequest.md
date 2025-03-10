## Overview
The UseInventoryItemRequest class represents the result of a request to use an item from the user's inventory. It inherits from SpatialAsyncOperation and provides information about whether the item was successfully used.

## Properties
- **itemID**: The ID of the item to use
- **succeeded**: True if the item was used successfully

## Inherited Members
- **InvokeCompletionEvent()**: Invokes the completion event
- **completed**: Event that is invoked when the operation is completed
- **isDone**: Returns true if the operation is done
- **keepWaiting**: Returns true if the operation is not done

## Usage Example
```csharp
public class ItemUseManager : MonoBehaviour
{
    // A dictionary to map item IDs to their usage methods
    private Dictionary<string, Action> itemUsageActions = new Dictionary<string, Action>();
    
    private void Start()
    {
        // Register usage actions for different items
        InitializeItemActions();
        
        // Subscribe to the general item used event
        SpatialBridge.inventoryService.onItemUsed += HandleItemUsed;
    }
    
    private void OnDestroy()
    {
        // Always unsubscribe from events
        SpatialBridge.inventoryService.onItemUsed -= HandleItemUsed;
    }
    
    private void InitializeItemActions()
    {
        // Register different actions for different items
        itemUsageActions.Add("health_potion", UseHealthPotion);
        itemUsageActions.Add("speed_boost", UseSpeedBoost);
        itemUsageActions.Add("shield_generator", UseShieldGenerator);
        // Add more items as needed
    }
    
    public void UseItem(string itemId)
    {
        // Check if we have the item in the inventory
        if (!SpatialBridge.inventoryService.items.TryGetValue(itemId, out IInventoryItem item) || item.amount <= 0)
        {
            Debug.LogWarning($"Cannot use item {itemId}: not in inventory or amount is 0");
            SpatialBridge.coreGUIService.DisplayToastMessage("Item not available");
            return;
        }
        
        // Check if the item is on cooldown (for consumables)
        if (item.isConsumable && item.isOnCooldown)
        {
            Debug.LogWarning($"Cannot use item {itemId}: on cooldown for {item.consumableCooldownRemaining} seconds");
            SpatialBridge.coreGUIService.DisplayToastMessage($"Item on cooldown: {Mathf.Ceil(item.consumableCooldownRemaining)}s");
            return;
        }
        
        // Use the item through the Inventory Service
        UseInventoryItemRequest request = SpatialBridge.inventoryService.UseItem(itemId);
        
        // Option 1: Use SetCompletedEvent extension method for a callback
        request.SetCompletedEvent((completedRequest) => {
            if (completedRequest.succeeded)
            {
                Debug.Log($"Successfully used item {completedRequest.itemID}");
                // Note: We don't need to apply effects here since we'll get the onItemUsed event
            }
            else
            {
                Debug.LogWarning($"Failed to use item {completedRequest.itemID}");
                SpatialBridge.coreGUIService.DisplayToastMessage("Failed to use item");
            }
        });
    }
    
    // Event handler for the item used event
    private void HandleItemUsed(string itemId)
    {
        Debug.Log($"Item used event received: {itemId}");
        
        // If we have a registered action for this item, execute it
        if (itemUsageActions.TryGetValue(itemId, out Action itemAction))
        {
            itemAction.Invoke();
        }
        else
        {
            Debug.Log($"No specific handling registered for item {itemId}");
        }
    }
    
    // Usage handlers for specific items
    private void UseHealthPotion()
    {
        Debug.Log("Health potion used - applying healing effect");
        // Example: Restore player health
        PlayerHealth.instance.RestoreHealth(50);
        
        // Play effects
        PlayHealingEffect();
        
        // Update UI
        UpdateHealthUI();
    }
    
    private void UseSpeedBoost()
    {
        Debug.Log("Speed boost used - applying movement speed increase");
        // Example: Increase movement speed
        PlayerMovement.instance.ApplySpeedBoost(1.5f, 30f); // 50% boost for 30 seconds
        
        // Play effects
        PlaySpeedEffect();
    }
    
    private void UseShieldGenerator()
    {
        Debug.Log("Shield generator used - activating damage shield");
        // Example: Add a damage shield
        PlayerDefense.instance.ActivateShield(100, 60f); // 100 damage shield for 60 seconds
        
        // Play effects
        PlayShieldEffect();
    }
    
    // Example effect methods
    private void PlayHealingEffect()
    {
        // Play healing particle effect and sound
        Debug.Log("Playing healing effect");
    }
    
    private void PlaySpeedEffect()
    {
        // Play speed boost particle effect and sound
        Debug.Log("Playing speed effect");
    }
    
    private void PlayShieldEffect()
    {
        // Play shield activation particle effect and sound
        Debug.Log("Playing shield effect");
    }
    
    private void UpdateHealthUI()
    {
        // Update health display
        Debug.Log("Updating health UI");
    }
}

// Example player components referenced in the item use effects
public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;
    
    private void Awake()
    {
        instance = this;
    }
    
    public void RestoreHealth(int amount)
    {
        // Implementation for restoring player health
        Debug.Log($"Restored {amount} health");
    }
}

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;
    
    private void Awake()
    {
        instance = this;
    }
    
    public void ApplySpeedBoost(float multiplier, float duration)
    {
        // Implementation for boosting player speed
        Debug.Log($"Applied speed boost: {multiplier}x for {duration} seconds");
    }
}

public class PlayerDefense : MonoBehaviour
{
    public static PlayerDefense instance;
    
    private void Awake()
    {
        instance = this;
    }
    
    public void ActivateShield(int shieldAmount, float duration)
    {
        // Implementation for activating a damage shield
        Debug.Log($"Activated shield: {shieldAmount} damage absorption for {duration} seconds");
    }
}
```

## Best Practices
- Always check if an item exists in the inventory and has a quantity greater than zero before attempting to use it
- For consumable items, check if they are on cooldown before attempting to use them
- Provide clear feedback to users when an item cannot be used (on cooldown, not owned, etc.)
- Use the `onItemUsed` event from IInventoryService to trigger effects when items are successfully used
- Create a system to map item IDs to their specific usage behaviors for clean, maintainable code
- Consider implementing visual and audio feedback for item usage to enhance user experience
- For important items, implement safeguards like confirmation prompts before use
- Remember that the UseInventoryItemRequest is asynchronous; don't assume the item is used immediately after calling UseItem

## Common Use Cases
- Implementing consumable items like health potions, buffs, or power-ups
- Creating usable items that trigger specific game effects or abilities
- Building interactive inventory systems where users can select and use items
- Implementing cooldown-based ability systems using consumable items
- Creating systems for using collectible or crafted items
- Implementing quest mechanics where using specific items advances the quest
- Building economy systems where items can be used, consumed, or transformed

## Completed: March 9, 2025