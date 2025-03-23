# IReward

Category: Quest Service Related

Interface

## Overview
The IReward interface represents a reward granted to users upon completion of a quest in the Spatial platform. Rewards provide users with tangible benefits or recognition for their accomplishments, enhancing user engagement and motivation. The Spatial platform currently supports two types of rewards: badges (for recognition and achievement display) and items (for functional or cosmetic use within the space).

## Properties

| Property | Description |
| --- | --- |
| amount | Amount of items (if reward type is Item). For badges, this value is typically 1. |
| id | ID of the reward. For badges, this is the badge ID. For items, this is the item ID. |
| type | Type of reward (Badge or Item). |

## Usage Examples

### Adding Rewards to a Quest

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class RewardExample : MonoBehaviour
{
    private IQuest explorationQuest;
    
    void Start()
    {
        // Create a new quest
        explorationQuest = SpatialBridge.questService.CreateQuest(
            "exploration_quest", 
            "World Explorer", 
            "Discover all secret areas of the island."
        );
        
        // Add tasks to the quest...
        explorationQuest.AddTask("Find the hidden cave", QuestTaskType.Check, 1, null);
        explorationQuest.AddTask("Discover the ancient temple", QuestTaskType.Check, 1, null);
        explorationQuest.AddTask("Reach the mountain peak", QuestTaskType.Check, 1, null);
        
        // Add a badge reward
        explorationQuest.AddBadgeReward("explorer_badge");
        
        // Add an item reward with a quantity of 5
        explorationQuest.AddItemReward("treasure_map", 5);
        
        // Set up completion handler to display rewards
        explorationQuest.SetOnCompleted(() => {
            DisplayRewardsSummary(explorationQuest);
        });
        
        // Start the quest
        explorationQuest.Start();
    }
    
    private void DisplayRewardsSummary(IQuest quest)
    {
        if (quest.rewards != null && quest.rewards.Length > 0)
        {
            string rewardText = "You earned:";
            
            foreach (IReward reward in quest.rewards)
            {
                if (reward.type == RewardType.Badge)
                {
                    rewardText += $"\n• Badge: {reward.id}";
                    
                    // You might want to show a badge icon here
                    ShowBadgeIcon(reward.id);
                }
                else if (reward.type == RewardType.Item)
                {
                    rewardText += $"\n• {reward.amount}x {reward.id}";
                    
                    // You might want to show item icons here
                    ShowItemIcon(reward.id, reward.amount);
                }
            }
            
            // Display the rewards summary to the user
            SpatialBridge.coreGUIService.DisplayToastMessage(rewardText);
        }
    }
    
    private void ShowBadgeIcon(string badgeId)
    {
        // Implementation to show badge icon
        // Could use SpatialBridge.badgeService to get badge details
    }
    
    private void ShowItemIcon(string itemId, int amount)
    {
        // Implementation to show item icon
        // Could use inventory service to get item details
    }
}
```

### Accessing Reward Properties in Quest Completion

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class QuestRewardManager : MonoBehaviour
{
    private Dictionary<string, GameObject> rewardDisplayPrefabs = new Dictionary<string, GameObject>();
    
    void Awake()
    {
        // Initialize reward display prefabs
        // This would be where you associate reward IDs with visual prefabs
        // rewardDisplayPrefabs.Add("explorer_badge", explorerBadgePrefab);
        // rewardDisplayPrefabs.Add("treasure_map", treasureMapPrefab);
    }
    
    public void RegisterQuestCompletionHandler(IQuest quest)
    {
        quest.SetOnCompleted(() => {
            StartCoroutine(ShowRewardSequence(quest.rewards));
        });
    }
    
    private System.Collections.IEnumerator ShowRewardSequence(IReward[] rewards)
    {
        if (rewards == null || rewards.Length == 0)
        {
            yield break;
        }
        
        // First, show a general completion message
        SpatialBridge.coreGUIService.DisplayToastMessage("Quest completed! Rewards earned:");
        
        yield return new WaitForSeconds(1.5f);
        
        // Then show each reward individually with animation
        foreach (IReward reward in rewards)
        {
            // Display different messages based on reward type
            if (reward.type == RewardType.Badge)
            {
                SpatialBridge.coreGUIService.DisplayToastMessage($"New Badge: {reward.id}");
                SpawnRewardVisual(reward);
                
                // Play special effects for badge
                PlayBadgeAwardEffect(reward.id);
            }
            else if (reward.type == RewardType.Item)
            {
                SpatialBridge.coreGUIService.DisplayToastMessage($"Received: {reward.amount}x {reward.id}");
                SpawnRewardVisual(reward);
                
                // Play special effects for item
                PlayItemAwardEffect(reward.id, reward.amount);
            }
            
            // Wait between rewards for dramatic effect
            yield return new WaitForSeconds(2.0f);
        }
    }
    
    private void SpawnRewardVisual(IReward reward)
    {
        if (rewardDisplayPrefabs.TryGetValue(reward.id, out GameObject prefab))
        {
            GameObject rewardVisual = Instantiate(prefab, transform.position + Vector3.up, Quaternion.identity);
            
            // Add floating animation, particles, etc.
            StartCoroutine(AnimateRewardVisual(rewardVisual));
        }
    }
    
    private System.Collections.IEnumerator AnimateRewardVisual(GameObject visual)
    {
        // Implement floating animation
        float duration = 3.0f;
        float elapsed = 0f;
        Vector3 startPos = visual.transform.position;
        
        while (elapsed < duration)
        {
            visual.transform.position = startPos + Vector3.up * Mathf.Sin(elapsed * 2f) * 0.5f;
            visual.transform.Rotate(0, 90 * Time.deltaTime, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        Destroy(visual);
    }
    
    private void PlayBadgeAwardEffect(string badgeId)
    {
        // Implementation for badge award visual/audio effects
    }
    
    private void PlayItemAwardEffect(string itemId, int amount)
    {
        // Implementation for item award visual/audio effects
    }
}
```

## Best Practices

1. **Balanced Reward Design**
   - Ensure rewards are commensurate with the difficulty and time investment of quests.
   - Use badges for achievements and recognition that persists across sessions.
   - Use items for functional benefits or cosmetic customization options.

2. **Clear Reward Communication**
   - Clearly communicate what rewards users will receive before they start a quest.
   - Show visual representations of rewards when possible.
   - Celebrate reward acquisition with appropriate visual and audio feedback.

3. **Meaningful Badges**
   - Design badges that have significance within your space's theme or narrative.
   - Consider allowing users to display earned badges on profiles or avatars.
   - Make badge rewards feel special by using them sparingly for meaningful achievements.

4. **Strategic Item Rewards**
   - Consider the utility and value of items before setting them as rewards.
   - Use items that enhance the user experience or unlock new capabilities.
   - Balance quantity for consumable items (not too few to feel insignificant, not too many to devalue them).

5. **Visual Presentation**
   - Create distinctive visual representations for different rewards.
   - Use animations or special effects when awarding significant rewards.
   - Consider a dedicated UI for showcasing earned rewards.

## Common Use Cases

1. **Achievement Recognition**
   - Award badges for completing challenging quests or discovery milestones.
   - Use badges to recognize special accomplishments like "First Explorer" or "Master Puzzle Solver."
   - Create collectible badge sets that encourage completion of related quests.

2. **Functional Rewards**
   - Provide items that unlock new areas or capabilities.
   - Award tools or equipment that enhance the user's abilities in the space.
   - Give consumable items that provide temporary advantages or special effects.

3. **Cosmetic Rewards**
   - Offer customization items for avatars or personal spaces.
   - Provide special visual effects or animations as rewards.
   - Award decorative items that users can display to showcase their achievements.

4. **Economic System Integration**
   - Use item rewards as part of a broader in-space economy.
   - Award currency or tradable items that have value within your space's systems.
   - Create rare or limited items as special quest rewards to drive engagement.

5. **Progression Systems**
   - Use rewards to signify progression through your space's content.
   - Design badge tiers that show mastery levels (Bronze, Silver, Gold).
   - Create item sets that encourage completion of related quest chains.

6. **Social Engagement**
   - Design badges that are visible to other users to encourage social recognition.
   - Create rewards that facilitate social interaction (e.g., special emotes or gestures).
   - Consider collaborative quests with shared rewards to encourage group activities.
