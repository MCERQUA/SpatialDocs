### Event-Based Premium Reward Track Implementation (Continued from DailyRewards-Part2.md)

```csharp
                    reward.attachmentId
                );
                
                if (!attachmentRequest.succeeded)
                {
                    Debug.LogError("Failed to equip attachment");
                }
                break;
                
            case PremiumRewardType.Badge:
                // Award badge
                var badgeRequest = await SpatialBridge.badgeService.AwardBadgeAsync(
                    reward.badgeId
                );
                
                if (!badgeRequest.succeeded)
                {
                    Debug.LogError("Failed to award badge");
                }
                break;
                
            case PremiumRewardType.Custom:
                // Trigger custom reward logic
                if (reward.customRewardAction != null)
                {
                    reward.customRewardAction.Invoke();
                }
                break;
        }
        
        // Show notification
        SpatialBridge.coreGUIService.DisplayToastMessage($"Claimed Premium Reward!");
    }
    
    // Increment the progress on the premium track
    public void IncrementProgress(int amount)
    {
        currentProgress += amount;
        
        // Check if any new rewards are available
        CheckForAvailableRewards();
        
        // Save player data
        SavePlayerData();
        
        // Update UI
        UpdateUI();
    }
    
    // Check for available rewards and notify the player
    private void CheckForAvailableRewards()
    {
        if (!hasPremiumAccess)
        {
            return;
        }
        
        bool foundAvailable = false;
        
        // Check each reward to see if it's newly available
        for (int i = 0; i < rewardConfig.rewards.Count; i++)
        {
            if (IsRewardAvailable(i))
            {
                foundAvailable = true;
                break;
            }
        }
        
        if (foundAvailable)
        {
            // Show notification that rewards are available
            SpatialBridge.coreGUIService.DisplayToastMessage("Premium rewards available to claim!");
            
            // Optionally show a UI indicator
            if (rewardUI != null)
            {
                rewardUI.ShowRewardAvailableNotification();
            }
        }
    }
    
    // Update the UI
    private void UpdateUI()
    {
        if (rewardUI != null)
        {
            rewardUI.UpdateUI(
                hasPremiumAccess,
                currentProgress,
                claimedRewardIndices,
                rewardConfig
            );
        }
    }
    
    // Purchase premium access
    public async void PurchasePremiumAccess()
    {
        // This would typically involve a marketplace transaction
        // For this example, we'll just simulate a successful purchase
        
        // In a real implementation, you would use the MarketplaceService:
        var purchaseRequest = await SpatialBridge.marketplaceService.PurchaseItemAsync(
            rewardConfig.premiumAccessItemId
        );
        
        if (purchaseRequest.succeeded)
        {
            hasPremiumAccess = true;
            SavePlayerData();
            UpdateUI();
            
            // Show notification
            SpatialBridge.coreGUIService.DisplayToastMessage("Premium access purchased!");
            
            // Check for available rewards
            CheckForAvailableRewards();
        }
        else
        {
            Debug.LogError($"Failed to purchase premium access: {purchaseRequest.responseCode}");
        }
    }
    
    // Save player data
    private async void SavePlayerData()
    {
        try
        {
            // Save premium access status
            await SpatialBridge.userWorldDataStoreService.SetDataAsync(
                HasPremiumKey,
                hasPremiumAccess.ToString()
            );
            
            // Save progress
            await SpatialBridge.userWorldDataStoreService.SetDataAsync(
                LastProgressKey,
                currentProgress.ToString()
            );
            
            // Save claimed rewards
            string claimedRewardsJson = JsonUtility.ToJson(new IntList { values = claimedRewardIndices });
            await SpatialBridge.userWorldDataStoreService.SetDataAsync(
                ClaimedRewardsKey,
                claimedRewardsJson
            );
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save player data: {e.Message}");
        }
    }
    
    // Load player data
    private async void LoadPlayerData()
    {
        try
        {
            // Load premium access status
            var premiumRequest = await SpatialBridge.userWorldDataStoreService.GetDataAsync(HasPremiumKey);
            if (premiumRequest.succeeded && !string.IsNullOrEmpty(premiumRequest.value))
            {
                hasPremiumAccess = bool.Parse(premiumRequest.value);
            }
            else
            {
                hasPremiumAccess = false;
            }
            
            // Load progress
            var progressRequest = await SpatialBridge.userWorldDataStoreService.GetDataAsync(LastProgressKey);
            if (progressRequest.succeeded && !string.IsNullOrEmpty(progressRequest.value))
            {
                currentProgress = int.Parse(progressRequest.value);
            }
            else
            {
                currentProgress = 0;
            }
            
            // Load claimed rewards
            var claimedRequest = await SpatialBridge.userWorldDataStoreService.GetDataAsync(ClaimedRewardsKey);
            if (claimedRequest.succeeded && !string.IsNullOrEmpty(claimedRequest.value))
            {
                IntList loadedList = JsonUtility.FromJson<IntList>(claimedRequest.value);
                claimedRewardIndices = loadedList.values;
            }
            else
            {
                claimedRewardIndices = new List<int>();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load player data: {e.Message}");
            
            // Initialize with defaults
            hasPremiumAccess = false;
            currentProgress = 0;
            claimedRewardIndices = new List<int>();
        }
    }
}

// Enum for premium reward types
public enum PremiumRewardType
{
    Currency,
    Item,
    AvatarAttachment,
    Badge,
    Custom
}

// Class for defining a premium reward
[Serializable]
public class PremiumReward
{
    public string rewardName;
    public PremiumRewardType rewardType;
    public Sprite rewardIcon;
    public int requiredProgress;
    
    // Currency reward fields
    public int currencyAmount;
    
    // Item reward fields
    public string itemId;
    public int itemCount = 1;
    
    // Avatar attachment reward fields
    public string attachmentId;
    
    // Badge reward fields
    public string badgeId;
    
    // Custom reward fields
    public UnityEngine.Events.UnityEvent customRewardAction;
}

// Scriptable object for configuring the premium reward track
[CreateAssetMenu(fileName = "PremiumRewardConfig", menuName = "Spatial/Daily Rewards/Premium Config")]
public class PremiumRewardConfig : ScriptableObject
{
    public string premiumAccessItemId;
    public List<PremiumReward> rewards = new List<PremiumReward>();
    public int totalProgressRequired;
    public string trackName = "Premium Rewards";
    public string trackDescription = "Unlock exclusive rewards by purchasing premium access.";
}
```

## Best Practices

- **Server-Side Validation**: Implement server-side time validation to prevent users from manipulating device clocks
- **Grace Periods**: Include configurable grace periods to maintain streaks despite occasional missed days
- **Clear Visual Communication**: Design UI that clearly shows reward progress and available claims
- **Balanced Rewards**: Create reward schedules with escalating value for longer streaks
- **Strategic Milestone Rewards**: Place high-value rewards at key milestone days (7, 14, 30) for retention
- **Cross-Platform Persistence**: Ensure reward state and streaks persist across different devices
- **Seamless UX**: Make claiming rewards quick and satisfying with minimal friction
- **Default Fall-Back**: Have contingency plans for when server-time validation fails
- **Testing Different Time Zones**: Verify behavior works correctly for global user base
- **Gradual Introduction**: Explain the system to new users without overwhelming them
- **Tiered Reward Tracks**: Consider both free and premium reward tracks where appropriate
- **Respect Player Time**: Design the system to be generous with grace periods and catch-up mechanics
- **Personalized Experience**: Tailor rewards to player progression when possible
- **Analytics Integration**: Track claim patterns and adjust reward schedules based on data
- **Adaptive Reminders**: Use non-intrusive notifications scaled to user engagement patterns

## Related Templates
- [Avatar Input Control](../Technical/AvatarInputControl.md) - Useful for creating custom reward claim interactions
- [HTTP Request Demo](../Technical/HTTPRequestDemo.md) - For implementing server-time validation
- [Obby (Obstacle Course) Game](../Games/ObbyGame.md) - Example of integrating progressive rewards with gameplay

## Additional Resources
- [Spatial Creator Toolkit Documentation](https://toolkit.spatial.io/docs)
- [Spatial Inventory Service Documentation](../../SpatialSys/UnitySDK/IInventoryService.md)
- [Spatial User World Data Store Documentation](../../SpatialSys/UnitySDK/IUserWorldDataStoreService.md)
- [GitHub Repository](https://github.com/spatialsys/spatial-example-daily-rewards)
- [Live Demo](https://spatial.io/s/Daily-Rewards-Demo)
