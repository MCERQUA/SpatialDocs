## Overview
The ItemType enum defines the different types of items that can exist in a user's inventory within the Spatial platform. These types categorize items based on their function and behavior within the system.

## Properties
- **Avatar**: Represents an avatar item that changes the user's appearance
- **AvatarAttachment**: Represents an attachment that can be equipped on an avatar
- **Emote**: Represents an animation or expression that can be triggered by the user
- **Generic**: Represents a general purpose item without specific predefined behavior
- **PrefabObject**: Represents a prefab object that can be instantiated in the world

## Usage Example
```csharp
public class ItemTypeManager : MonoBehaviour
{
    // UI elements for different item type sections
    public Transform avatarItemsContainer;
    public Transform attachmentsContainer;
    public Transform emotesContainer;
    public Transform genericItemsContainer;
    public Transform prefabsContainer;
    
    // UI Prefab for item display
    public GameObject itemDisplayPrefab;
    
    private void Start()
    {
        // Clear existing items
        ClearAllContainers();
        
        // Organize and display items by type
        OrganizeInventoryByType();
        
        // Example of disabling a specific type of items
        DisableItemTypeTemporarily();
    }
    
    private void ClearAllContainers()
    {
        // Clear all existing items from containers
        foreach (Transform child in avatarItemsContainer) Destroy(child.gameObject);
        foreach (Transform child in attachmentsContainer) Destroy(child.gameObject);
        foreach (Transform child in emotesContainer) Destroy(child.gameObject);
        foreach (Transform child in genericItemsContainer) Destroy(child.gameObject);
        foreach (Transform child in prefabsContainer) Destroy(child.gameObject);
    }
    
    private void OrganizeInventoryByType()
    {
        // Go through all items in inventory and organize by type
        foreach (var itemPair in SpatialBridge.inventoryService.items)
        {
            // Create a display element for this item
            GameObject itemDisplay = CreateItemDisplay(itemPair.Value);
            
            // Place it in the appropriate container based on its type
            Transform targetContainer = GetContainerForItemType(itemPair.Key);
            if (targetContainer != null)
            {
                itemDisplay.transform.SetParent(targetContainer, false);
            }
        }
    }
    
    private GameObject CreateItemDisplay(IInventoryItem item)
    {
        // Create an instance of our item display prefab
        GameObject display = Instantiate(itemDisplayPrefab);
        
        // Configure the display (this would depend on your UI implementation)
        ItemDisplayComponent displayComponent = display.GetComponent<ItemDisplayComponent>();
        if (displayComponent != null)
        {
            displayComponent.SetupForItem(item);
        }
        
        return display;
    }
    
    private Transform GetContainerForItemType(string itemId)
    {
        // Assuming we can determine the item type from the item ID
        // In a real implementation, you might need to check item metadata or properties
        
        // Example implementation that parses item type from ID format
        if (itemId.StartsWith("avatar_"))
        {
            return avatarItemsContainer;
        }
        else if (itemId.StartsWith("attachment_"))
        {
            return attachmentsContainer;
        }
        else if (itemId.StartsWith("emote_"))
        {
            return emotesContainer;
        }
        else if (itemId.StartsWith("prefab_"))
        {
            return prefabsContainer;
        }
        else
        {
            return genericItemsContainer;
        }
    }
    
    public void DisableItemTypeTemporarily()
    {
        // Example: Temporarily disable all prefab objects during a cutscene
        SpatialBridge.inventoryService.SetItemTypeEnabled(ItemType.PrefabObject, false, "Prefabs disabled during cutscene");
        
        Debug.Log("Prefab objects temporarily disabled");
        
        // Re-enable them after 10 seconds
        StartCoroutine(ReenableItemTypeAfterDelay(ItemType.PrefabObject, 10f));
    }
    
    private IEnumerator ReenableItemTypeAfterDelay(ItemType itemType, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Re-enable the item type
        SpatialBridge.inventoryService.SetItemTypeEnabled(itemType, true, null);
        
        Debug.Log($"{itemType} items re-enabled");
    }
    
    // Method called when filters are applied in the UI
    public void FilterByItemType(int itemTypeIndex)
    {
        // Convert the int to the enum value
        ItemType selectedType = (ItemType)itemTypeIndex;
        
        // Hide all containers
        avatarItemsContainer.gameObject.SetActive(false);
        attachmentsContainer.gameObject.SetActive(false);
        emotesContainer.gameObject.SetActive(false);
        genericItemsContainer.gameObject.SetActive(false);
        prefabsContainer.gameObject.SetActive(false);
        
        // Show only the selected container
        switch (selectedType)
        {
            case ItemType.Avatar:
                avatarItemsContainer.gameObject.SetActive(true);
                break;
            case ItemType.AvatarAttachment:
                attachmentsContainer.gameObject.SetActive(true);
                break;
            case ItemType.Emote:
                emotesContainer.gameObject.SetActive(true);
                break;
            case ItemType.Generic:
                genericItemsContainer.gameObject.SetActive(true);
                break;
            case ItemType.PrefabObject:
                prefabsContainer.gameObject.SetActive(true);
                break;
        }
    }
}

// Example helper component for item display
public class ItemDisplayComponent : MonoBehaviour
{
    public Text itemNameText;
    public Text itemTypeText;
    public Image itemIcon;
    public Button useButton;
    
    public void SetupForItem(IInventoryItem item)
    {
        // Set item name
        itemNameText.text = item.itemID; // In a real app, you might have a display name
        
        // Determine the item type from ID and set type text
        ItemType type = DetermineItemType(item.itemID);
        itemTypeText.text = type.ToString();
        
        // Add button click handler
        useButton.onClick.AddListener(() => {
            item.Use();
        });
    }
    
    private ItemType DetermineItemType(string itemId)
    {
        // Simplified example - in a real app you might get this from metadata
        if (itemId.StartsWith("avatar_")) return ItemType.Avatar;
        if (itemId.StartsWith("attachment_")) return ItemType.AvatarAttachment;
        if (itemId.StartsWith("emote_")) return ItemType.Emote;
        if (itemId.StartsWith("prefab_")) return ItemType.PrefabObject;
        return ItemType.Generic;
    }
}
```

## Best Practices
- Use ItemType for organizing inventory displays in your UI
- Group related items by their type to create intuitive inventory categories
- Use `SetItemTypeEnabled` to temporarily disable certain types of items during specific game states
- Create consistent naming conventions for item IDs to make it easier to identify their types
- When filtering inventory displays, use ItemType as the basis for categorization
- Consider providing different UI layouts optimized for each item type
- Remember that each item type may require different handling in your UI and gameplay logic

## Common Use Cases
- Creating tabbed inventory UIs with separate sections for different item types
- Implementing item filters that allow users to view specific categories
- Disabling certain item types in situations where they shouldn't be used (e.g., disabling emotes during cutscenes)
- Creating specialized inventory management for different item categories
- Building type-specific item interactions (e.g., avatar customization for Avatar items)
- Implementing type-based restrictions for certain gameplay areas
- Creating specialized tutorial flows for different item types

## Completed: March 9, 2025