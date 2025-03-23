# AdRequest

Category: Ad Service

Class: Operation to request an ad to show up

`AdRequest` is a class that represents an asynchronous operation to request and display an advertisement in a Spatial space. It inherits from `SpatialAsyncOperation` and provides additional functionality specific to ad requests.

## Properties/Fields

| Property | Description |
| --- | --- |
| adType | The type of ad that was requested (SpatialAdType). |
| hasStarted | Returns true if the request has started. |
| succeeded | True if the ad request succeeded. |

## Methods

| Method | Description |
| --- | --- |
| InvokeStartedEvent() | Invokes the started event. |
| SetStartedEvent(Action<SpatialAsyncOperation>) | Sets the started event, same as setting the event using the started property, but returns the operation itself. |

## Events

| Event | Description |
| --- | --- |
| started | Event that is invoked when the operation has started. |

## Inherited Members

| Member | Description |
| --- | --- |
| InvokeCompletionEvent() | Invokes the completion event. |
| completed | Event that is invoked when the operation is completed. |
| isDone | Returns true if the operation is done. |
| keepWaiting | Returns true if the operation is not done. |

## Extension Methods

| Method | Description |
| --- | --- |
| SetCompletedEvent<T>(T, Action<T>) | Sets the completion event, same as setting the event using the completed property, but returns the operation itself. |

## Usage Examples

### Basic Ad Request

```csharp
public void WatchAd()
{
    // Check if ads are supported in the current environment
    if (!SpatialBridge.adService.isSupported)
        return;
    
    // Request a rewarded ad and set a callback for when it completes
    SpatialBridge.adService.RequestAd(SpatialAdType.Rewarded).SetCompletedEvent((request) => {
        if (request.succeeded)
        {
            // Ad was successfully watched
            Debug.Log("Ad succeeded");
            
            // Reward the player here
            GivePlayerReward();
        }
        else
        {
            // Ad failed to display or was not completed
            Debug.Log("Ad failed or was skipped");
        }
    });
}

private void GivePlayerReward()
{
    // Give the player their reward for watching the ad
    // For example: in-game currency, extra lives, etc.
}
```

### Checking Ad Status During the Request

```csharp
public void RequestAdWithStatusTracking()
{
    if (!SpatialBridge.adService.isSupported)
        return;
    
    var adRequest = SpatialBridge.adService.RequestAd(SpatialAdType.MidGame);
    
    // Track when the ad starts
    adRequest.SetStartedEvent((request) => {
        Debug.Log("Ad has started");
        
        // Perhaps pause your game here or mute audio
        PauseGameplay();
    });
    
    // Track when the ad completes
    adRequest.SetCompletedEvent((request) => {
        Debug.Log("Ad has completed");
        
        if (request.succeeded)
        {
            Debug.Log("Ad was successfully viewed");
        }
        
        // Resume your game here
        ResumeGameplay();
    });
}

private void PauseGameplay()
{
    // Pause game logic
}

private void ResumeGameplay()
{
    // Resume game logic
}
```

## Best Practices

1. **Always check ad service support**: Use `SpatialBridge.adService.isSupported` before attempting to show ads, as ads may not be available on all platforms or environments.

2. **Handle both success and failure cases**: Always provide handling for both when an ad succeeds and when it fails or is skipped.

3. **Implement proper UX around ads**: Inform users before showing ads and provide clear messaging about rewards they'll receive for watching.

4. **Pause gameplay during ads**: Make sure to pause your game's logic and audio while an ad is being displayed.

5. **Use rewarded ads sparingly**: Avoid overwhelming users with too many ads. Place them strategically at natural breaks in gameplay.

6. **Test ad flows thoroughly**: Test the full ad experience, including what happens if ads fail to load or if users skip them.

## Common Use Cases

1. **Rewarded videos for in-game currency**: Allow players to earn in-game currency or tokens by watching rewarded video ads.

2. **Extra lives or continues**: Offer players the ability to continue gameplay after failure by watching an ad.

3. **Unlocking bonus content**: Use ads as a way to unlock bonus levels, cosmetic items, or other non-essential content.

4. **Mid-game breaks**: Show interstitial ads at natural break points in gameplay, such as between levels.

5. **Alternative to in-app purchases**: Provide ad-based alternatives to paid content, giving users options for how they want to engage.

## Related Components

- [SpatialAdType](./SpatialAdType.md): Enum that defines the types of ads that can be requested.
- [IAdService](./IAdService.md): Interface that provides methods for showing ads and checking ad service availability.
- [SpatialBridge](./SpatialBridge.md): Main access point for all Spatial services, including the ad service.