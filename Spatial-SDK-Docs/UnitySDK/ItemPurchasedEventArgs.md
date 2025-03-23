# ItemPurchasedEventArgs

Category: Marketplace Service Related

Structure: Struct

Arguments for the `onItemPurchased` event in the [`IMarketplaceService`](./IMarketplaceService.md). Contains information about a purchased item.

## Properties

| Property | Description |
| --- | --- |
| itemID | The unique identifier of the item that was purchased by the local user. |

## Usage Examples

```csharp
private void OnEnable()
{
    // Subscribe to the marketplace item purchased event
    SpatialBridge.marketplaceService.onItemPurchased += HandleItemPurchased;
}

private void OnDisable()
{
    // Unsubscribe from the event when disabled
    SpatialBridge.marketplaceService.onItemPurchased -= HandleItemPurchased;
}

private void HandleItemPurchased(ItemPurchasedEventArgs args)
{
    // Get the purchased item ID
    string purchasedItemID = args.itemID;
    
    // Log the purchase
    Debug.Log($"Item purchased successfully: {purchasedItemID}");
    
    // Example: Grant the item to the player
    GiveItemToPlayer(purchasedItemID);
    
    // Example: Display a confirmation message
    SpatialBridge.coreGUIService.DisplayToastMessage($"Thanks for your purchase: {purchasedItemID}!");
}

private void GiveItemToPlayer(string itemID)
{
    // Implement your item granting logic here
    // For example:
    // - Add to inventory
    // - Unlock a feature
    // - Apply cosmetic changes
    // - Provide in-game benefits
    
    // This would be your custom implementation based on the item type
}
```

## Best Practices

1. Always validate the itemID when handling a purchase to ensure it's an expected item
2. Create a robust item management system that can handle all your marketplace items
3. Maintain a central registry of item IDs and their corresponding effects or benefits
4. Consider caching purchased items for offline use when appropriate
5. Implement proper error handling for cases where the item may not be recognized
6. Use the onItemPurchased event to provide immediate feedback to the user about their purchase

## Common Use Cases

1. In-game item shops where players can purchase cosmetics, boosts, or special abilities
2. Premium feature unlocking mechanisms for experiences with tiered access
3. Virtual merchandise stores for digital collectibles and avatar customizations
4. Season pass or battle pass type systems with purchasable tiers
5. Economic simulation games with virtual goods marketplaces
6. Tracking purchase analytics and player spending patterns 
7. Offering special "thank you" rewards or bonuses after purchases are completed

## Completed: March 9, 2025
