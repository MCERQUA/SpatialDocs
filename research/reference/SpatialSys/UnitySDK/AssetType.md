## Overview
The AssetType enum represents various types of visual components that a space object can have in Spatial. It categorizes the different types of assets that can be used within the Spatial platform.

## Properties
- **None**: Represents no asset type assigned.
- **EmbeddedAsset**: Represents an asset that is embedded directly within the space.
- **Package**: Represents a packaged asset that can be loaded into the space.
- **BackpackItem**: Represents an item that can be stored in the user's backpack inventory.

## Usage Example
```csharp
// Example of checking an asset type
public void CheckAssetType(SpatialAsset asset)
{
    if (asset.assetType == AssetType.BackpackItem)
    {
        Debug.Log("This is a backpack item asset");
    }
    else if (asset.assetType == AssetType.Package)
    {
        Debug.Log("This is a package asset");
    }
}
```

## Best Practices
- Use this enum to correctly categorize assets when creating or managing them in your Spatial project
- When building user interfaces or systems that interact with assets, filter by AssetType to ensure you're handling the correct asset category
- For inventory systems, specifically filter for BackpackItem types

## Common Use Cases
- Determining how to display or handle different types of assets in the UI
- Filtering assets by type in collection or management systems
- Setting the appropriate asset type when creating new assets
- Implementing inventory systems that work with backpack items

## Completed: March 10, 2025
