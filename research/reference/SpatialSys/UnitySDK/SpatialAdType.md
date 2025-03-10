# SpatialAdType

Category: Ad Service

Enum: Type of ad to show.

`SpatialAdType` is an enumeration that defines the different types of advertisements that can be requested and displayed through the Spatial advertisement service.

## Values

| Value | Description |
| --- | --- |
| None | No specific ad type. This is a default value and typically not used when making ad requests. |
| Rewarded | Rewarded ad type. These ads provide players with in-game rewards when they watch the complete ad. Typically these are opt-in by the player. |
| MidGame | Mid-game ad type. These are interstitial ads that appear during natural breaks in gameplay. |

## Usage Examples

### Requesting a Rewarded Ad

```csharp
public void RequestRewardedAd()
{
    if (!SpatialBridge.adService.isSupported)
        return;
    
    SpatialBridge.adService.RequestAd(SpatialAdType.Rewarded).SetCompletedEvent((request) => {
        if (request.succeeded)
        {
            // Ad was successfully watched to completion
            GivePlayerReward();
        }
    });
}

private void GivePlayerReward()
{
    // Give the player their reward for watching the ad
    // For example: in-game currency, extra lives, etc.
}
```

### Requesting a Mid-Game Ad Between Levels

```csharp
public void OnLevelCompleted()
{
    SaveGameProgress();
    
    // Show an interstitial ad between levels
    if (SpatialBridge.adService.isSupported)
    {
        SpatialBridge.adService.RequestAd(SpatialAdType.MidGame).SetCompletedEvent((request) => {
            // Regardless of success or failure, load the next level when the ad is done
            LoadNextLevel();
        });
    }
    else
    {
        // If ads aren't supported, just go straight to the next level
        LoadNextLevel();
    }
}

private void SaveGameProgress()
{
    // Save the player's progress
}

private void LoadNextLevel()
{
    // Load the next level
}
```

## Best Practices

1. **Use appropriate ad types for specific scenarios**: Rewarded ads should be opt-in and offer clear value, while mid-game ads should appear at natural breaks.

2. **Clearly communicate ad types to users**: When offering a rewarded ad, clearly explain what reward the user will receive for watching it.

3. **Handle all ad types appropriately**: Different ad types have different user expectations - make sure your implementation matches those expectations.

4. **Respect user experience**: Don't overuse mid-game ads, as they can interrupt gameplay. Place them at natural transition points.

5. **Test on all supported platforms**: Ad behavior might vary slightly on different platforms, so test thoroughly.

## Common Use Cases

1. **Rewarded ads for premium content**: Allow players to unlock premium features or items by watching ads instead of paying.

2. **Mid-game ads at level transitions**: Show interstitial ads when players complete a level or reach a checkpoint.

3. **Rewarded ads for extra lives/continues**: Let players continue after failure by watching an ad instead of waiting or paying.

4. **Rewarded ads for in-game currency**: Offer virtual currency rewards for watching ads, which can then be spent in-game.

5. **Mid-game ads after extended play sessions**: Show ads after the player has been playing for a certain amount of time.

## Related Components

- [AdRequest](./AdRequest.md): Class that represents an asynchronous operation to request and display an advertisement.
- [IAdService](./IAdService.md): Interface that provides methods for showing ads and checking ad service availability.
- [SpatialBridge](./SpatialBridge.md): Main access point for all Spatial services, including the ad service.