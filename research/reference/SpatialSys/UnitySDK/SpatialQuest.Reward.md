# SpatialQuest.Reward

Category: Core Components

Interface/Class/Enum: Class

The SpatialQuest.Reward class defines rewards that are awarded to users upon the completion of a quest in a Spatial environment. It allows developers to specify different types of rewards, such as currency, inventory items, or experience points, along with the amount to be awarded. Rewards are an important part of the quest system as they provide incentives for users to complete quests and a sense of accomplishment.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| id | string | Unique identifier for this reward. |
| type | RewardType | The type of reward (Currency, Item, Experience, etc.). |
| amount | int | The quantity of the reward to be awarded. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections.Generic;

public class QuestRewardManager : MonoBehaviour
{
    // Reference to quests in the scene
    [SerializeField] private SpatialQuest[] quests;
    
    // UI reference for displaying rewards
    [SerializeField] private RewardUIController rewardUI;
    
    private void Start()
    {
        // Initialize quest reward listeners
        InitializeQuestRewardListeners();
    }
    
    private void InitializeQuestRewardListeners()
    {
        if (quests == null)
            return;
            
        foreach (var quest in quests)
        {
            if (quest != null)
            {
                // Subscribe to the quest completion event to handle rewards
                quest.onCompletedEvent.AddListener(() => HandleQuestCompletion(quest));
            }
        }
    }
    
    // Handle quest completion and rewards
    private void HandleQuestCompletion(SpatialQuest quest)
    {
        Debug.Log($"Quest completed: {quest.questName}");
        
        // Process rewards
        if (quest.questRewards != null && quest.questRewards.Count > 0)
        {
            List<RewardSummary> rewardSummaries = new List<RewardSummary>();
            
            foreach (var reward in quest.questRewards)
            {
                // Process each reward
                ProcessReward(reward);
                
                // Create a summary for UI display
                RewardSummary summary = new RewardSummary
                {
                    rewardType = reward.type,
                    amount = reward.amount,
                    rewardId = reward.id
                };
                
                rewardSummaries.Add(summary);
            }
            
            // Show rewards UI if available
            if (rewardUI != null)
            {
                rewardUI.DisplayRewards(quest.questName, rewardSummaries);
            }
            else
            {
                // Log rewards if no UI is available
                LogRewards(quest.questName, quest.questRewards);
            }
        }
        else
        {
            Debug.Log($"No rewards specified for quest: {quest.questName}");
        }
    }
    
    // Process a reward
    private void ProcessReward(SpatialQuest.Reward reward)
    {
        if (reward == null)
            return;
            
        switch (reward.type)
        {
            case RewardType.Currency:
                AwardCurrency(reward);
                break;
                
            case RewardType.Item:
                AwardItem(reward);
                break;
                
            case RewardType.Experience:
                AwardExperience(reward);
                break;
                
            case RewardType.Badge:
                AwardBadge(reward);
                break;
                
            default:
                Debug.LogWarning($"Unknown reward type: {reward.type}");
                break;
        }
    }
    
    // Award currency
    private void AwardCurrency(SpatialQuest.Reward reward)
    {
        Debug.Log($"Awarding currency: {reward.amount}");
        
        // Use the inventory service to award currency
        var request = SpatialBridge.inventoryService.AwardWorldCurrency(reward.amount);
        
        // Handle async operation completion
        StartCoroutine(HandleCurrencyAward(request, reward));
    }
    
    // Handle currency award async operation
    private System.Collections.IEnumerator HandleCurrencyAward(
        AwardWorldCurrencyRequest request, SpatialQuest.Reward reward)
    {
        // Wait for the request to complete
        yield return request;
        
        if (request.succeeded)
        {
            Debug.Log($"Successfully awarded {reward.amount} currency. New balance: {request.newBalance}");
            
            // Show currency award animation or effect
            if (rewardUI != null)
            {
                rewardUI.PlayCurrencyAwardAnimation(reward.amount);
            }
        }
        else
        {
            Debug.LogError($"Failed to award currency: {request.errorMessage}");
        }
    }
    
    // Award item
    private void AwardItem(SpatialQuest.Reward reward)
    {
        Debug.Log($"Awarding item: {reward.id} x{reward.amount}");
        
        // Use the inventory service to award the item
        var request = SpatialBridge.inventoryService.AddInventoryItem(reward.id, reward.amount);
        
        // Handle async operation completion
        StartCoroutine(HandleItemAward(request, reward));
    }
    
    // Handle item award async operation
    private System.Collections.IEnumerator HandleItemAward(
        AddInventoryItemRequest request, SpatialQuest.Reward reward)
    {
        // Wait for the request to complete
        yield return request;
        
        if (request.succeeded)
        {
            Debug.Log($"Successfully awarded item: {reward.id} x{reward.amount}");
            
            // Show item award animation or effect
            if (rewardUI != null)
            {
                rewardUI.PlayItemAwardAnimation(reward.id, reward.amount);
            }
        }
        else
        {
            Debug.LogError($"Failed to award item: {request.errorMessage}");
        }
    }
    
    // Award experience
    private void AwardExperience(SpatialQuest.Reward reward)
    {
        Debug.Log($"Awarding experience: {reward.amount} XP");
        
        // For experience, you might use data store or custom system
        // This is a simplified example
        
        // Update player experience in data store
        StartCoroutine(UpdatePlayerExperience(reward.amount));
    }
    
    // Update player experience in data store
    private System.Collections.IEnumerator UpdatePlayerExperience(int amount)
    {
        // Get current XP
        var getRequest = SpatialBridge.userWorldDataStoreService.GetVariable("playerXP");
        yield return getRequest;
        
        int currentXP = 0;
        if (getRequest.succeeded)
        {
            // Parse the current value
            if (int.TryParse(getRequest.value, out int parsedValue))
            {
                currentXP = parsedValue;
            }
        }
        
        // Calculate new XP
        int newXP = currentXP + amount;
        
        // Save the new value
        var setRequest = SpatialBridge.userWorldDataStoreService.SetVariable("playerXP", newXP.ToString());
        yield return setRequest;
        
        if (setRequest.succeeded)
        {
            Debug.Log($"Successfully updated player XP to {newXP}");
            
            // Show XP award animation or effect
            if (rewardUI != null)
            {
                rewardUI.PlayXPAwardAnimation(amount, newXP);
            }
        }
        else
        {
            Debug.LogError($"Failed to update player XP: {setRequest.errorMessage}");
        }
    }
    
    // Award badge
    private void AwardBadge(SpatialQuest.Reward reward)
    {
        Debug.Log($"Awarding badge: {reward.id}");
        
        // Use the badge service to award the badge
        SpatialBridge.badgeService.UnlockBadge(reward.id);
        
        // Show badge award animation or effect
        if (rewardUI != null)
        {
            rewardUI.PlayBadgeAwardAnimation(reward.id);
        }
    }
    
    // Log rewards for debugging
    private void LogRewards(string questName, List<SpatialQuest.Reward> rewards)
    {
        Debug.Log($"Rewards for quest: {questName}");
        
        foreach (var reward in rewards)
        {
            Debug.Log($"- {reward.type}: {reward.amount} (ID: {reward.id})");
        }
    }
    
    // Helper class for UI reward display
    [System.Serializable]
    public class RewardSummary
    {
        public RewardType rewardType;
        public int amount;
        public string rewardId;
    }
    
    // Create a reward programmatically
    public SpatialQuest.Reward CreateReward(RewardType type, int amount, string id = null)
    {
        SpatialQuest.Reward reward = new SpatialQuest.Reward();
        reward.type = type;
        reward.amount = amount;
        reward.id = string.IsNullOrEmpty(id) ? System.Guid.NewGuid().ToString() : id;
        
        return reward;
    }
    
    // Add a reward to a quest
    public void AddRewardToQuest(SpatialQuest quest, SpatialQuest.Reward reward)
    {
        if (quest == null || reward == null)
            return;
            
        // Initialize rewards list if needed
        if (quest.questRewards == null)
        {
            quest.questRewards = new List<SpatialQuest.Reward>();
        }
        
        // Add the reward
        quest.questRewards.Add(reward);
        
        Debug.Log($"Added {reward.type} reward ({reward.amount}) to quest: {quest.questName}");
    }
    
    // Create a multi-reward package for a quest
    public void SetupQuestRewards(SpatialQuest quest, bool isMainQuest)
    {
        if (quest == null)
            return;
            
        // Clear existing rewards
        if (quest.questRewards != null)
        {
            quest.questRewards.Clear();
        }
        else
        {
            quest.questRewards = new List<SpatialQuest.Reward>();
        }
        
        // Main quests get better rewards
        if (isMainQuest)
        {
            // Add currency reward
            SpatialQuest.Reward currencyReward = CreateReward(RewardType.Currency, 500, "main_quest_currency");
            quest.questRewards.Add(currencyReward);
            
            // Add item reward
            SpatialQuest.Reward itemReward = CreateReward(RewardType.Item, 1, "rare_item_001");
            quest.questRewards.Add(itemReward);
            
            // Add experience reward
            SpatialQuest.Reward xpReward = CreateReward(RewardType.Experience, 1000, "main_quest_xp");
            quest.questRewards.Add(xpReward);
            
            // Add badge reward
            SpatialQuest.Reward badgeReward = CreateReward(RewardType.Badge, 1, "quest_master_badge");
            quest.questRewards.Add(badgeReward);
        }
        else
        {
            // Side quests get smaller rewards
            // Add currency reward
            SpatialQuest.Reward currencyReward = CreateReward(RewardType.Currency, 100, "side_quest_currency");
            quest.questRewards.Add(currencyReward);
            
            // Add experience reward
            SpatialQuest.Reward xpReward = CreateReward(RewardType.Experience, 250, "side_quest_xp");
            quest.questRewards.Add(xpReward);
        }
        
        Debug.Log($"Set up {quest.questRewards.Count} rewards for quest: {quest.questName}");
    }
}
```

## Best Practices

1. Always assign unique IDs to rewards to make them easily identifiable in logs and analytics.
2. Use appropriate reward types based on what players value in your experience - consider the progression system you've designed.
3. Scale the amount of rewards in proportion to the difficulty and time investment required to complete the quest.
4. Consider creating reward packages (combinations of different reward types) for more interesting quest completion experiences.
5. Include visual and audio feedback when awarding rewards to enhance player satisfaction.
6. For item rewards, make sure the item ID exists and is correctly configured in your inventory system.
7. Consider using different reward strategies for different types of quests (main quests vs. side quests, daily quests vs. achievement quests).
8. Design rewards that fit your world's theme and narrative context.
9. Test the balance of rewards to ensure they promote the desired player behavior without creating economic imbalance.
10. Use the UserWorldDataStore service to track long-term reward history for analytics and player progression tracking.

## Common Use Cases

1. Currency rewards for in-game economies where players can spend on cosmetics, upgrades, or other virtual goods.
2. Item rewards that give players useful tools, cosmetics, or collectibles upon quest completion.
3. Experience points that contribute to player level progression systems.
4. Badge or achievement rewards that showcase player accomplishments and can be displayed on profiles.
5. Access rewards that unlock new areas, features, or capabilities within the experience.
6. Cosmetic rewards that allow players to customize their avatars or personal spaces.
7. Tiered reward systems where more difficult quests provide more valuable rewards.
8. Daily login or recurring quest rewards to encourage regular engagement.
9. Special or limited-time event rewards that are only available during specific periods.
10. Social rewards that enhance a player's status or visibility within the community.

## Completed: March 10, 2025