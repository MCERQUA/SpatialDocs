# EquipAttachmentRequest

Category: Actor Service

Class: Async Operation

`EquipAttachmentRequest` is a class that represents the result of a request to equip an attachment on an avatar. It inherits from `SpatialAsyncOperation` and provides information about whether the equip operation succeeded.

## Properties/Fields

| Property | Description |
| --- | --- |
| succeeded | A boolean indicating whether the request to equip an attachment was successful. |

## Methods

This class does not define any custom methods beyond those inherited from `SpatialAsyncOperation`.

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
| SetCompletedEvent<T>(T, Action<T>) | Sets the completion event, same as setting the event using the completed property, but returns the operation itself for easier chaining. |

## Usage Examples

### Basic Attachment Equipping

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class AttachmentEquipper : MonoBehaviour
{
    [SerializeField] private string attachmentId = "hat_0001";
    
    public void EquipAttachment()
    {
        // Make sure local actor has an avatar
        if (!SpatialBridge.actorService.localActor.hasAvatar)
        {
            Debug.LogWarning("Cannot equip attachment: local actor does not have an avatar");
            return;
        }
        
        // Request to equip the attachment
        SpatialBridge.actorService.localActor.avatar.EquipAttachment(attachmentId)
            .SetCompletedEvent((request) => {
                if (request.succeeded)
                {
                    Debug.Log($"Successfully equipped attachment: {attachmentId}");
                    SpatialBridge.coreGUIService.DisplayToastMessage("Attachment equipped!");
                }
                else
                {
                    Debug.LogWarning($"Failed to equip attachment: {attachmentId}");
                    SpatialBridge.coreGUIService.DisplayToastMessage("Failed to equip attachment");
                }
            });
    }
}
```

### Inventory System with Attachment Management

```csharp
using UnityEngine;
using UnityEngine.UI;
using SpatialSys.UnitySDK;
using System.Collections;
using System.Collections.Generic;

public class AttachmentInventoryManager : MonoBehaviour
{
    [System.Serializable]
    public class AttachmentItem
    {
        public string id;
        public string displayName;
        public string category;
        public Sprite icon;
    }
    
    [SerializeField] private List<AttachmentItem> availableAttachments = new List<AttachmentItem>();
    [SerializeField] private Transform inventoryItemContainer;
    [SerializeField] private GameObject inventoryItemPrefab;
    
    // Dictionary to track equipped items by category
    private Dictionary<string, string> equippedAttachments = new Dictionary<string, string>();
    
    private void Start()
    {
        PopulateInventory();
    }
    
    private void PopulateInventory()
    {
        // Clear existing items
        foreach (Transform child in inventoryItemContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create UI elements for each attachment
        foreach (var attachment in availableAttachments)
        {
            GameObject itemObj = Instantiate(inventoryItemPrefab, inventoryItemContainer);
            InventoryItemUI itemUI = itemObj.GetComponent<InventoryItemUI>();
            
            if (itemUI != null)
            {
                // Set up UI
                itemUI.SetAttachmentInfo(attachment.displayName, attachment.category, attachment.icon);
                
                // Set up button click handler
                Button button = itemObj.GetComponent<Button>();
                if (button != null)
                {
                    string attachmentId = attachment.id; // Create a local copy for lambda
                    button.onClick.AddListener(() => ToggleAttachment(attachmentId, attachment.category));
                }
            }
        }
    }
    
    public void ToggleAttachment(string attachmentId, string category)
    {
        // Check if already equipped in this category
        if (equippedAttachments.TryGetValue(category, out string equippedId) && equippedId == attachmentId)
        {
            // It's already equipped, unequip it
            StartCoroutine(UnequipAttachmentCoroutine(attachmentId, category));
        }
        else
        {
            // It's not equipped, equip it (replacing any existing item in that category)
            StartCoroutine(EquipAttachmentCoroutine(attachmentId, category));
        }
    }
    
    private IEnumerator EquipAttachmentCoroutine(string attachmentId, string category)
    {
        // Make sure local actor has an avatar
        if (!SpatialBridge.actorService.localActor.hasAvatar)
        {
            Debug.LogWarning("Cannot equip attachment: local actor does not have an avatar");
            yield break;
        }
        
        // Show loading indicator
        ShowLoadingIndicator(true);
        
        // Request to equip the attachment
        var request = SpatialBridge.actorService.localActor.avatar.EquipAttachment(attachmentId);
        
        // Wait for the request to complete
        yield return request;
        
        // Hide loading indicator
        ShowLoadingIndicator(false);
        
        if (request.succeeded)
        {
            // Update equipped items dictionary
            equippedAttachments[category] = attachmentId;
            
            // Update UI to show as equipped
            UpdateInventoryUI();
            
            Debug.Log($"Successfully equipped attachment: {attachmentId}");
            SpatialBridge.coreGUIService.DisplayToastMessage("Item equipped!");
        }
        else
        {
            Debug.LogWarning($"Failed to equip attachment: {attachmentId}");
            SpatialBridge.coreGUIService.DisplayToastMessage("Failed to equip item");
        }
    }
    
    private IEnumerator UnequipAttachmentCoroutine(string attachmentId, string category)
    {
        // Make sure local actor has an avatar
        if (!SpatialBridge.actorService.localActor.hasAvatar)
        {
            Debug.LogWarning("Cannot unequip attachment: local actor does not have an avatar");
            yield break;
        }
        
        // Show loading indicator
        ShowLoadingIndicator(true);
        
        // Request to unequip the attachment (by equipping an empty string)
        var request = SpatialBridge.actorService.localActor.avatar.EquipAttachment("");
        
        // Wait for the request to complete
        yield return request;
        
        // Hide loading indicator
        ShowLoadingIndicator(false);
        
        if (request.succeeded)
        {
            // Remove from equipped items dictionary
            equippedAttachments.Remove(category);
            
            // Update UI to show as unequipped
            UpdateInventoryUI();
            
            Debug.Log($"Successfully unequipped attachment: {attachmentId}");
            SpatialBridge.coreGUIService.DisplayToastMessage("Item unequipped!");
        }
        else
        {
            Debug.LogWarning($"Failed to unequip attachment: {attachmentId}");
            SpatialBridge.coreGUIService.DisplayToastMessage("Failed to unequip item");
        }
    }
    
    private void UpdateInventoryUI()
    {
        // Implementation depends on your UI system
        // Update visual state of inventory items
        Debug.Log("Updating inventory UI");
    }
    
    private void ShowLoadingIndicator(bool visible)
    {
        // Implementation depends on your UI system
        Debug.Log($"Loading indicator: {visible}");
    }
}

// Example UI component for inventory items
public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text categoryText;
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject equippedIndicator;
    
    public void SetAttachmentInfo(string name, string category, Sprite icon)
    {
        nameText.text = name;
        categoryText.text = category;
        iconImage.sprite = icon;
    }
    
    public void SetEquippedState(bool equipped)
    {
        equippedIndicator.SetActive(equipped);
    }
}
```

## Best Practices

1. **Always check the succeeded property**: Verify that the request was successful before assuming the attachment was equipped.

2. **Handle equipment failures gracefully**: Provide appropriate feedback to the user when an attachment fails to equip, such as a UI message or visual indication.

3. **Consider using coroutines**: For more complex equipment sequences, coroutines can make the code more readable and manageable when waiting for async operations.

4. **Use the SetCompletedEvent extension method**: This provides a cleaner syntax than manually subscribing to the completed event.

5. **Implement category management**: Typically, avatars can only have one attachment per category (like hats, glasses, etc.). Keep track of equipped items by category to handle replacements properly.

6. **Verify avatar existence**: Always check if the local actor has an avatar (using `hasAvatar`) before attempting to equip attachments.

7. **Provide visual feedback during loading**: Since equipping attachments can take time, especially for the first load, provide appropriate loading indicators to improve user experience.

## Common Use Cases

1. **Cosmetic systems**: Allow players to customize their avatars with various cosmetic items like hats, glasses, clothing, and accessories.

2. **Equipment systems**: Implement functional equipment like weapons, tools, or special items that provide gameplay benefits.

3. **Achievement rewards**: Award special attachments or cosmetics as rewards for completing achievements or challenges.

4. **Role indication**: Use attachments to visually indicate roles or status, such as team membership or special privileges.

5. **Seasonal content**: Implement seasonal or event-specific attachments for holidays or special occasions.

6. **Virtual economy**: Create a marketplace for purchasing, trading, or earning attachments as part of a virtual economy.

7. **Character progression**: Use attachments to visually represent character progression or level in games or educational experiences.

## Related Components

- [IAvatar](./IAvatar.md): Interface that provides methods for controlling avatars, including the EquipAttachment method.
- [SpatialAsyncOperation](./SpatialAsyncOperation.md): Base class for asynchronous operations in Spatial.
- [IActor](./IActor.md): Interface that represents an actor in the Spatial environment, which owns an avatar.