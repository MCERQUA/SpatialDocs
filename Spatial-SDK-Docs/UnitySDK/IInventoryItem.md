## Overview
The IInventoryItem interface represents an item in the user's inventory or backpack in the Spatial platform. It provides properties and methods for interacting with inventory items, including information about ownership, amount, consumable status, and usage capabilities.

## Properties
- **amount**: The amount of the item owned in the inventory
- **consumableCooldownRemaining**: Cooldown time remaining before the item can be used again
- **consumableDurationRemaining**: How long the consumable item's effects remain active
- **isConsumable**: True if the item is consumable
- **isConsumeActive**: True if the item is being consumed (consumableDurationRemaining > 0)
- **isOnCooldown**: True if the item is on cooldown (consumableCooldownRemaining > 0)
- **isOwned**: Does the user have this item in their inventory?
- **itemID**: Item ID. This can be found on Spatial Studio

## Methods
- **SetEnabled(bool enabled, string reasonIfDisabled)**: Set the enabled state for an item in the user's inventory. This can be useful to prevent users from equipping or consuming certain items based on logic within your experience. All items in the user's inventory are enabled by default.
- **Use()**: Use the item. This will trigger the OnItemUsed event.

## Events
- **onConsumableItemDurationExpired**: Triggered when a consumable item's effects finish (consumableDurationRemaining == 0)
- **onItemAmountChanged**: Triggered when an item's amount has changed
- **onItemConsumed**: Triggered when user uses a consumable item (item's amount decreases by 1) before consume duration timer starts
- **onItemUsed**: Triggered when the user uses an item

## Usage Example
```csharp
public class InventoryManager : MonoBehaviour
{
    // Reference to a specific inventory item we want to track
    private IInventoryItem healthPotion;
    
    // UI elements
    public Text potionCountText;
    public Button usePotionButton;
    public Image cooldownImage;
    
    private void Start()
    {
        // Initialize UI
        UpdateUI();
        
        // Find our health potion in the inventory (assume we know the item ID)
        string healthPotionId = "health_potion_001";
        
        // Check if we already have it in the inventory cache
        if (SpatialBridge.inventoryService.items.TryGetValue(healthPotionId, out IInventoryItem item))
        {
            SetupHealthPotion(item);
        }
        else
        {
            // If not in cache, make a request to check
            GetInventoryItemRequest request = SpatialBridge.inventoryService.GetItem(healthPotionId);
            request.SetCompletedEvent((completedRequest) => {
                if (completedRequest.succeeded)
                {
                    if (SpatialBridge.inventoryService.items.TryGetValue(healthPotionId, out IInventoryItem foundItem))
                    {
                        SetupHealthPotion(foundItem);
                    }
                }
                else
                {
                    Debug.Log("Health potion not found in inventory");
                    usePotionButton.interactable = false;
                    potionCountText.text = "0";
                }
            });
        }
        
        // Also subscribe to the general inventory service events
        SpatialBridge.inventoryService.onItemUsed += OnAnyItemUsed;
        SpatialBridge.inventoryService.onItemConsumed += OnAnyItemConsumed;
    }
    
    private void SetupHealthPotion(IInventoryItem potion)
    {
        healthPotion = potion;
        
        // Subscribe to this specific item's events
        healthPotion.onItemAmountChanged += OnPotionAmountChanged;
        healthPotion.onItemConsumed += OnPotionConsumed;
        healthPotion.onConsumableItemDurationExpired += OnPotionEffectExpired;
        
        // Update UI based on current state
        UpdateUI();
    }
    
    private void OnDestroy()
    {
        // Always unsubscribe from events when the component is destroyed
        if (healthPotion != null)
        {
            healthPotion.onItemAmountChanged -= OnPotionAmountChanged;
            healthPotion.onItemConsumed -= OnPotionConsumed;
            healthPotion.onConsumableItemDurationExpired -= OnPotionEffectExpired;
        }
        
        SpatialBridge.inventoryService.onItemUsed -= OnAnyItemUsed;
        SpatialBridge.inventoryService.onItemConsumed -= OnAnyItemConsumed;
    }
    
    public void UseHealthPotion()
    {
        if (healthPotion != null && healthPotion.isOwned && healthPotion.amount > 0 && !healthPotion.isOnCooldown)
        {
            healthPotion.Use();
            // Note: We don't need to update UI here since we'll get the onItemUsed event
        }
        else
        {
            Debug.Log("Cannot use health potion");
            if (healthPotion.isOnCooldown)
            {
                SpatialBridge.coreGUIService.DisplayToastMessage($"Potion on cooldown: {Mathf.Ceil(healthPotion.consumableCooldownRemaining)}s");
            }
        }
    }
    
    private void UpdateUI()
    {
        if (healthPotion != null)
        {
            // Update potion count
            potionCountText.text = healthPotion.amount.ToString();
            
            // Update button interactability
            usePotionButton.interactable = healthPotion.isOwned && 
                                          healthPotion.amount > 0 && 
                                          !healthPotion.isOnCooldown;
            
            // Update cooldown visual if applicable
            if (healthPotion.isOnCooldown)
            {
                cooldownImage.gameObject.SetActive(true);
                // Assuming cooldownImage is a radial fill image
                cooldownImage.fillAmount = healthPotion.consumableCooldownRemaining / 30f; // Assuming 30s full cooldown
            }
            else
            {
                cooldownImage.gameObject.SetActive(false);
            }
            
            // Show active effect indicator if applicable
            if (healthPotion.isConsumeActive)
            {
                Debug.Log($"Potion effect active for {healthPotion.consumableDurationRemaining} more seconds");
                // Show some UI element indicating active effect
            }
        }
        else
        {
            potionCountText.text = "0";
            usePotionButton.interactable = false;
            cooldownImage.gameObject.SetActive(false);
        }
    }
    
    // Event handlers for the specific health potion
    private void OnPotionAmountChanged(ulong newAmount)
    {
        Debug.Log($"Health potion amount changed to {newAmount}");
        UpdateUI();
    }
    
    private void OnPotionConsumed()
    {
        Debug.Log("Health potion consumed");
        // Apply health regeneration effect
        ApplyHealthRegeneration();
        UpdateUI();
    }
    
    private void OnPotionEffectExpired()
    {
        Debug.Log("Health potion effect expired");
        // Stop health regeneration effect
        StopHealthRegeneration();
        UpdateUI();
    }
    
    // Event handlers for any inventory item
    private void OnAnyItemUsed(string itemID)
    {
        Debug.Log($"An item was used: {itemID}");
    }
    
    private void OnAnyItemConsumed(string itemID)
    {
        Debug.Log($"An item was consumed: {itemID}");
    }
    
    // Example game logic methods
    private void ApplyHealthRegeneration()
    {
        // Example: Apply a health regeneration effect
        Debug.Log("Applying health regeneration effect");
    }
    
    private void StopHealthRegeneration()
    {
        // Example: Stop the health regeneration effect
        Debug.Log("Stopping health regeneration effect");
    }
}
```

## Best Practices
- Always check `isOwned` and `amount` before attempting to use an item
- Check `isOnCooldown` before allowing players to use consumable items
- Subscribe to events like `onItemAmountChanged` to keep UI elements synchronized with inventory state
- Unsubscribe from all events when components are destroyed to prevent memory leaks
- Use `SetEnabled` to temporarily prevent items from being used in certain contexts
- For consumable items, provide clear visual feedback about cooldown and active duration
- Check both `consumableCooldownRemaining` and `consumableDurationRemaining` to understand the full state of consumable items
- Remember that item events are also available at the service level (IInventoryService) for when you need to track all items

## Common Use Cases
- Creating item inventory UIs that display owned items and their quantities
- Implementing consumable items like power-ups with duration and cooldown mechanics
- Building equip/unequip systems for wearable items and attachments
- Creating crafting systems that check for and consume required materials
- Implementing quest systems that track collected items
- Building shop interfaces that show what items the player already owns
- Creating cooldown timers and visual effects for consumable items
- Implementing item-based progression systems where specific items unlock content

## Completed: March 9, 2025