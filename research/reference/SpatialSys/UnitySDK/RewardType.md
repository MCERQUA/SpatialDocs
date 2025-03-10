# RewardType

Category: Quest Service Related

Enum

## Overview
The RewardType enum defines the different types of rewards that can be granted to users upon completion of quests in the Spatial platform. It categorizes rewards based on their nature and functionality, allowing developers to create diverse incentive systems for their experiences.

## Properties

| Property | Description |
| --- | --- |
| Badge | A badge reward that recognizes achievement. Badges are typically displayed on user profiles and serve as status symbols or accomplishments. |
| Item | An item reward that provides functionality or value. Items can be used, equipped, or displayed within the space and represent tangible benefits. |

## Usage Examples

### Creating Different Reward Types

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class QuestRewardExample : MonoBehaviour
{
    void Start()
    {
        // Create a beginner quest with a badge reward
        IQuest beginnerQuest = SpatialBridge.questService.CreateQuest(
            "beginner_quest",
            "First Steps",
            "Complete the beginner tutorial."
        );
        
        // Add a badge reward
        beginnerQuest.AddBadgeReward("tutorial_complete");
        
        // Create an advanced quest with both badge and item rewards
        IQuest advancedQuest = SpatialBridge.questService.CreateQuest(
            "advanced_quest",
            "Master Explorer",
            "Find all hidden locations in the world."
        );
        
        // Add a badge reward for recognition
        advancedQuest.AddBadgeReward("master_explorer");
        
        // Add item rewards for functionality
        advancedQuest.AddItemReward("rare_compass", 1);
        advancedQuest.AddItemReward("map_fragment", 5);
        
        // Start both quests
        beginnerQuest.Start();
        advancedQuest.Start();
    }
}
```

### Processing Rewards Based on Type

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class RewardProcessor : MonoBehaviour
{
    // Dictionary to store reward display prefabs
    private Dictionary<string, GameObject> badgePrefabs = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> itemPrefabs = new Dictionary<string, GameObject>();
    
    // Reference to UI containers
    public Transform badgeDisplayContainer;
    public Transform itemDisplayContainer;
    
    void Start()
    {
        // Register for quest completion events
        RegisterQuestCompletionHandlers();
    }
    
    private void RegisterQuestCompletionHandlers()
    {
        // Get all active quests
        IQuest[] quests = SpatialBridge.questService.GetAllQuests();
        
        foreach (IQuest quest in quests)
        {
            quest.SetOnCompleted(() => {
                ProcessQuestRewards(quest);
            });
        }
    }
    
    private void ProcessQuestRewards(IQuest quest)
    {
        if (quest.rewards == null || quest.rewards.Length == 0)
        {
            Debug.Log($"Quest '{quest.name}' completed, but has no rewards.");
            return;
        }
        
        List<IReward> badgeRewards = new List<IReward>();
        List<IReward> itemRewards = new List<IReward>();
        
        // Categorize rewards by type
        foreach (IReward reward in quest.rewards)
        {
            switch (reward.type)
            {
                case RewardType.Badge:
                    badgeRewards.Add(reward);
                    break;
                    
                case RewardType.Item:
                    itemRewards.Add(reward);
                    break;
            }
        }
        
        // Process badge rewards
        if (badgeRewards.Count > 0)
        {
            Debug.Log($"Processing {badgeRewards.Count} badge rewards...");
            foreach (IReward badge in badgeRewards)
            {
                DisplayBadgeReward(badge.id);
                // You might update a badge collection UI here
            }
        }
        
        // Process item rewards
        if (itemRewards.Count > 0)
        {
            Debug.Log($"Processing {itemRewards.Count} item rewards...");
            foreach (IReward item in itemRewards)
            {
                DisplayItemReward(item.id, item.amount);
                // You might update an inventory UI here
            }
        }
    }
    
    private void DisplayBadgeReward(string badgeId)
    {
        Debug.Log($"Earned badge: {badgeId}");
        
        // Show badge notification
        SpatialBridge.coreGUIService.DisplayToastMessage($"New Badge: {badgeId}");
        
        // Create badge visual in UI if we have a prefab for it
        if (badgePrefabs.TryGetValue(badgeId, out GameObject badgePrefab))
        {
            Instantiate(badgePrefab, badgeDisplayContainer);
        }
        
        // Add custom badge unlock effects
        PlayBadgeUnlockEffects(badgeId);
    }
    
    private void DisplayItemReward(string itemId, int amount)
    {
        Debug.Log($"Received item: {amount}x {itemId}");
        
        // Show item notification
        SpatialBridge.coreGUIService.DisplayToastMessage($"Received: {amount}x {itemId}");
        
        // Create item visual in UI if we have a prefab for it
        if (itemPrefabs.TryGetValue(itemId, out GameObject itemPrefab))
        {
            GameObject itemVisual = Instantiate(itemPrefab, itemDisplayContainer);
            
            // Update item count display
            ItemDisplay display = itemVisual.GetComponent<ItemDisplay>();
            if (display != null)
            {
                display.SetAmount(amount);
            }
        }
        
        // Add custom item acquisition effects
        PlayItemAcquisitionEffects(itemId, amount);
    }
    
    private void PlayBadgeUnlockEffects(string badgeId)
    {
        // Implementation for badge unlock effects
        // Could include particle effects, sound, animation, etc.
    }
    
    private void PlayItemAcquisitionEffects(string itemId, int amount)
    {
        // Implementation for item acquisition effects
        // Could include particle effects, sound, animation, etc.
    }
    
    // Example component that would be attached to item display prefabs
    private class ItemDisplay : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI amountText;
        
        public void SetAmount(int amount)
        {
            if (amountText != null)
            {
                amountText.text = amount.ToString();
            }
        }
    }
}
```

### Creating a Reward Management System

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class RewardManager : MonoBehaviour
{
    [System.Serializable]
    public class BadgeData
    {
        public string badgeId;
        public string displayName;
        public string description;
        public Sprite icon;
    }
    
    [System.Serializable]
    public class ItemData
    {
        public string itemId;
        public string displayName;
        public string description;
        public Sprite icon;
        public bool isConsumable;
        public bool isEquippable;
    }
    
    // Lists of available badges and items
    public List<BadgeData> availableBadges = new List<BadgeData>();
    public List<ItemData> availableItems = new List<ItemData>();
    
    // Dictionaries for quick lookup
    private Dictionary<string, BadgeData> badgeDatabase = new Dictionary<string, BadgeData>();
    private Dictionary<string, ItemData> itemDatabase = new Dictionary<string, ItemData>();
    
    // UI references
    public GameObject badgeDisplayPrefab;
    public GameObject itemDisplayPrefab;
    public Transform rewardDisplayPanel;
    
    void Awake()
    {
        // Build databases for quick lookup
        foreach (BadgeData badge in availableBadges)
        {
            badgeDatabase[badge.badgeId] = badge;
        }
        
        foreach (ItemData item in availableItems)
        {
            itemDatabase[item.itemId] = item;
        }
    }
    
    void Start()
    {
        // Register for quest completion events
        RegisterForQuestCompletions();
    }
    
    private void RegisterForQuestCompletions()
    {
        IQuest[] quests = SpatialBridge.questService.GetAllQuests();
        foreach (IQuest quest in quests)
        {
            quest.SetOnCompleted(() => {
                ShowQuestRewards(quest);
            });
        }
    }
    
    public void ShowQuestRewards(IQuest quest)
    {
        StartCoroutine(DisplayRewardsSequence(quest));
    }
    
    private System.Collections.IEnumerator DisplayRewardsSequence(IQuest quest)
    {
        yield return new WaitForSeconds(1.0f); // Brief delay
        
        // Display quest completion message
        SpatialBridge.coreGUIService.DisplayToastMessage($"Quest Completed: {quest.name}");
        
        yield return new WaitForSeconds(1.5f);
        
        // Show reward header
        if (quest.rewards != null && quest.rewards.Length > 0)
        {
            SpatialBridge.coreGUIService.DisplayToastMessage("Rewards earned:");
            
            yield return new WaitForSeconds(0.5f);
            
            // Display rewards one by one with appropriate effects
            foreach (IReward reward in quest.rewards)
            {
                switch (reward.type)
                {
                    case RewardType.Badge:
                        yield return DisplayBadgeReward(reward.id);
                        break;
                        
                    case RewardType.Item:
                        yield return DisplayItemReward(reward.id, reward.amount);
                        break;
                }
                
                yield return new WaitForSeconds(1.0f);
            }
        }
    }
    
    private System.Collections.IEnumerator DisplayBadgeReward(string badgeId)
    {
        if (badgeDatabase.TryGetValue(badgeId, out BadgeData badge))
        {
            // Show badge notification
            SpatialBridge.coreGUIService.DisplayToastMessage($"New Badge: {badge.displayName}");
            
            // Create visual representation
            GameObject badgeDisplay = Instantiate(badgeDisplayPrefab, rewardDisplayPanel);
            BadgeDisplayComponent displayComponent = badgeDisplay.GetComponent<BadgeDisplayComponent>();
            
            if (displayComponent != null)
            {
                displayComponent.SetBadgeData(badge);
                displayComponent.PlayUnlockAnimation();
            }
            
            // Play badge-specific effects if desired
            
            yield return new WaitForSeconds(2.0f);
            
            // Cleanup or keep as needed
            Destroy(badgeDisplay, 3.0f);
        }
    }
    
    private System.Collections.IEnumerator DisplayItemReward(string itemId, int amount)
    {
        if (itemDatabase.TryGetValue(itemId, out ItemData item))
        {
            // Show item notification
            SpatialBridge.coreGUIService.DisplayToastMessage($"Received: {amount}x {item.displayName}");
            
            // Create visual representation
            GameObject itemDisplay = Instantiate(itemDisplayPrefab, rewardDisplayPanel);
            ItemDisplayComponent displayComponent = itemDisplay.GetComponent<ItemDisplayComponent>();
            
            if (displayComponent != null)
            {
                displayComponent.SetItemData(item, amount);
                displayComponent.PlayAcquisitionAnimation();
            }
            
            // Play item-specific effects if desired
            
            yield return new WaitForSeconds(2.0f);
            
            // Cleanup or keep as needed
            Destroy(itemDisplay, 3.0f);
        }
    }
    
    // Example component for badge display
    private class BadgeDisplayComponent : MonoBehaviour
    {
        public void SetBadgeData(BadgeData data)
        {
            // Implementation to set up the badge display
        }
        
        public void PlayUnlockAnimation()
        {
            // Implementation for badge unlock animation
        }
    }
    
    // Example component for item display
    private class ItemDisplayComponent : MonoBehaviour
    {
        public void SetItemData(ItemData data, int amount)
        {
            // Implementation to set up the item display
        }
        
        public void PlayAcquisitionAnimation()
        {
            // Implementation for item acquisition animation
        }
    }
}
```

## Best Practices

1. **Reward Type Selection**
   - Use Badge rewards for achievements, milestones, and recognition.
   - Use Item rewards for functional benefits, consumables, and equipment.
   - Consider combining both types for important quests to provide both recognition and utility.

2. **Badge Implementation**
   - Make badges visually distinctive and thematically appropriate.
   - Use badges to recognize meaningful achievements rather than trivial actions.
   - Consider creating badge collections or sets that encourage completion of related quests.
   - Allow users to display or showcase their badges in profiles or personal spaces.

3. **Item Implementation**
   - Ensure items have clear utility or value within your experience.
   - Use appropriate quantities for item rewards (not too few to feel insignificant, not too many to devalue them).
   - Consider item rarity and balance when designing quest rewards.
   - Provide clear information about what items do and how they can be used.

4. **Reward Presentation**
   - Create distinct visual and audio feedback for different reward types.
   - Celebrate significant rewards with special effects or animations.
   - Display reward information clearly, including what was earned and why.
   - Consider dedicated UI elements for displaying and managing earned rewards.

5. **Balance and Progression**
   - Match reward types and values to quest difficulty and time investment.
   - Create a progression system where later quests provide more valuable rewards.
   - Use badges for prestigious accomplishments and items for ongoing gameplay benefits.
   - Plan your reward economy carefully to maintain balance over time.

## Common Use Cases

1. **Badge Reward Use Cases**
   - **Achievement Recognition**: Acknowledge completion of difficult challenges.
   - **Milestone Markers**: Celebrate progression milestones like "Reached Level 10" or "Explored 100% of Map".
   - **Participation Rewards**: Recognize participation in special events or limited-time activities.
   - **Collection Completion**: Reward users for completing sets of collectibles or challenges.
   - **Status Symbols**: Provide exclusive badges for exceptional achievements that other users can see.

2. **Item Reward Use Cases**
   - **Functional Equipment**: Provide tools, weapons, or abilities that enhance the user's capabilities.
   - **Cosmetic Items**: Offer visual customization options for avatars, spaces, or objects.
   - **Consumable Resources**: Grant expendable items that provide temporary benefits.
   - **Currency**: Award in-experience currency that can be spent on other items or services.
   - **Access Keys**: Provide items that unlock new areas or content within the experience.
   - **Crafting Materials**: Reward users with components needed for crafting or building activities.

3. **Combined Reward Strategies**
   - **Tiered Reward Systems**: Offer better rewards for higher difficulty quests or challenges.
   - **Progression-Based Rewards**: Structure rewards to support a sense of advancing through the experience.
   - **Specialization Paths**: Create quest lines with rewards that support different playstyles or roles.
   - **Seasonal Events**: Design special time-limited quests with unique rewards to encourage participation.
