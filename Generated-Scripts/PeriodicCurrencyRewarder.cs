using UnityEngine;
using System.Collections;
using SpatialSys.UnitySDK;

/// <summary>
/// This script awards world currency to the player periodically.
/// It replicates the Visual Scripting graph that awards currency on scene initialization
/// and then waits for a period of time before repeating.
/// </summary>
public class PeriodicCurrencyRewarder : MonoBehaviour
{
    [Tooltip("Amount of currency to award each time")]
    [SerializeField] private int currencyAmount = 1;

    [Tooltip("Time in seconds to wait between currency awards")]
    [SerializeField] private float waitTime = 120f;

    [Tooltip("Whether the currency reward loop should continue running")]
    [SerializeField] private bool continueRewarding = true;

    private void Start()
    {
        // Start the currency reward coroutine
        StartCoroutine(CurrencyRewardLoop());
    }

    private IEnumerator CurrencyRewardLoop()
    {
        // Award currency immediately when the scene initializes
        AwardCurrency();

        // Continue awarding currency periodically as long as the flag is true
        while (continueRewarding)
        {
            // Wait for the specified time
            yield return new WaitForSeconds(waitTime);

            // Award currency again if we should continue
            if (continueRewarding)
            {
                AwardCurrency();
            }
        }
    }

    private void AwardCurrency()
    {
        // Use the inventoryService to award world currency to the player
        AwardWorldCurrencyRequest request = SpatialBridge.inventoryService.AwardWorldCurrency((ulong)currencyAmount);
        
        // Handle the completion of the request
        request.SetCompletedEvent((completedRequest) => {
            if (completedRequest.succeeded)
            {
                Debug.Log($"Successfully awarded {completedRequest.amount} currency");
            }
            else
            {
                Debug.LogWarning($"Failed to award currency");
            }
        });
    }

    // Public method to stop the currency rewarding loop
    public void StopRewarding()
    {
        continueRewarding = false;
    }

    // Public method to restart the currency rewarding loop
    public void StartRewarding()
    {
        if (!continueRewarding)
        {
            continueRewarding = true;
            StartCoroutine(CurrencyRewardLoop());
        }
    }
}