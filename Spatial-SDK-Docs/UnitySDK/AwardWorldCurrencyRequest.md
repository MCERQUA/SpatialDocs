## Overview
The AwardWorldCurrencyRequest class represents the result of a request to award world currency to a user. It inherits from SpatialAsyncOperation and provides information about whether the currency award operation succeeded and the amount of currency awarded.

## Properties
- **amount**: The amount of currency to award
- **succeeded**: True if the award request succeeded

## Inherited Members
- **InvokeCompletionEvent()**: Invokes the completion event
- **completed**: Event that is invoked when the operation is completed
- **isDone**: Returns true if the operation is done
- **keepWaiting**: Returns true if the operation is not done

## Usage Example
```csharp
public void RewardPlayerWithCurrency(ulong currencyAmount)
{
    // Display a message to inform the player about the reward
    SpatialBridge.coreGUIService.DisplayToastMessage($"Awarding you {currencyAmount} currency!");
    
    // Award currency to the player
    AwardWorldCurrencyRequest request = SpatialBridge.inventoryService.AwardWorldCurrency(currencyAmount);
    
    // Option 1: Use SetCompletedEvent extension method for a callback
    request.SetCompletedEvent((completedRequest) => {
        if (completedRequest.succeeded)
        {
            Debug.Log($"Successfully awarded {completedRequest.amount} currency");
            // Update UI to reflect the new currency balance
            UpdateCurrencyDisplay(SpatialBridge.inventoryService.worldCurrencyBalance);
            // Play a success animation or sound
            PlayCurrencyAwardAnimation();
        }
        else
        {
            Debug.LogWarning($"Failed to award {completedRequest.amount} currency");
            // Handle failure case
            SpatialBridge.coreGUIService.DisplayToastMessage("Failed to award currency. Please try again later.");
        }
    });
    
    // Option 2: Use it in a coroutine
    StartCoroutine(AwardCurrencyCoroutine(currencyAmount));
}

private IEnumerator AwardCurrencyCoroutine(ulong currencyAmount)
{
    AwardWorldCurrencyRequest request = SpatialBridge.inventoryService.AwardWorldCurrency(currencyAmount);
    
    // Wait until the operation is complete
    yield return request;
    
    if (request.succeeded)
    {
        Debug.Log($"Successfully awarded {request.amount} currency");
        UpdateCurrencyDisplay(SpatialBridge.inventoryService.worldCurrencyBalance);
        PlayCurrencyAwardAnimation();
    }
    else
    {
        Debug.LogWarning($"Failed to award {request.amount} currency");
        SpatialBridge.coreGUIService.DisplayToastMessage("Failed to award currency. Please try again later.");
    }
}

private void UpdateCurrencyDisplay(ulong newBalance)
{
    // Example method to update UI with new currency balance
    Debug.Log($"New currency balance: {newBalance}");
}

private void PlayCurrencyAwardAnimation()
{
    // Example method to play a celebratory animation when currency is awarded
    Debug.Log("Playing currency award animation");
}
```

## Best Practices
- Always check the `succeeded` property before assuming the currency was awarded successfully
- Subscribe to the `onWorldCurrencyBalanceChanged` event on the Inventory Service to keep currency displays updated
- Provide clear visual and audio feedback when currency is awarded to enhance user experience
- Consider showing both the awarded amount and the new total balance to the user
- For large currency rewards, consider displaying special effects or animations to make the moment more impactful
- When awarding currency for completing tasks, verify the task is actually completed before awarding
- Store important currency transactions in your game's state if you need to verify them later

## Common Use Cases
- Rewarding players for completing quests or missions
- Providing daily login bonuses
- Compensating players for in-game achievements
- Distributing prizes for contest winners
- Giving currency as part of tutorial completion
- Rewarding social interactions or community participation
- Providing refunds for in-game purchases

## Completed: March 9, 2025