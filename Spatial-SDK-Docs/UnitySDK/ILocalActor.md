# ILocalActor

Category: Actor Service

Interface: Actor Interface

`ILocalActor` is an interface that represents the local actor (the user on the current machine) in the Spatial environment. It extends the `IActor` interface with additional methods and properties specific to the local user, particularly for managing custom properties and controlling the local avatar.

## Properties/Fields

| Property | Description |
| --- | --- |
| avatar | The avatar for the local actor. Unlike the `avatar` property in `IActor`, this reference will never be null, even if the avatar for the local user was hidden or disabled. However, if `hasAvatar` is false, all calls to avatar methods and properties will be no-ops. This reference will always point to the current avatar for the local actor, even if the assigned avatar changes. |

## Methods

| Method | Description |
| --- | --- |
| RemoveCustomProperty(string) | Remove a custom property for this actor. |
| SetCustomProperty(string, object) | Set a custom property for this actor. These properties are synchronized across all clients. An actor's properties are only cleared if they leave the space. Teleporting between servers of the same space will not clear the properties. Supported types: int, bool, float, Vector2, Vector3, string, Color32, byte, double, long, int[] |

## Inherited Members from IActor

| Member | Description |
| --- | --- |
| actorNumber | The unique number of the actor in the current server instance. |
| customProperties | A dictionary of custom properties for the actor. These properties are synchronized across all clients. |
| displayName | The display name for the user. This is displayed in the nametag and on the user's profile page. |
| hasAvatar | Returns true if the actor has an assigned avatar. |
| isDisposed | Whether the actor has been disposed. This will be true if the actor has left the server. |
| isRegistered | Does the user for this actor have a fully completed Spatial account? |
| isSpaceAdministrator | Is the user for this actor an administrator of the space? |
| isSpaceOwner | Is the user for this actor the owner of the space? |
| isTalking | Is the user for this actor currently talking with voice chat? |
| platform | The platform that the actor is currently using to join this space. |
| profileColor | The color of the actor's profile picture background. |
| userID | The user ID of the actor. |
| username | The username for the public profile of the user. |
| GetProfilePicture() | Get the profile picture texture for the actor. |
| onAvatarExistsChanged | Event that is triggered when the avatar for the actor is created. |
| onCustomPropertiesChanged | Event that is triggered when any of the actor's custom properties change. |

## Usage Examples

### Basic Usage of Local Actor Properties

```csharp
using UnityEngine;
using UnityEngine.UI;
using SpatialSys.UnitySDK;

public class LocalActorDisplay : MonoBehaviour
{
    [SerializeField] private Text displayNameText;
    [SerializeField] private Text userIdText;
    [SerializeField] private Text actorNumberText;
    [SerializeField] private RawImage profilePicture;
    [SerializeField] private Image profileBackground;
    [SerializeField] private Text platformText;
    [SerializeField] private GameObject adminBadge;
    [SerializeField] private GameObject ownerBadge;
    
    private void Start()
    {
        UpdateDisplay();
    }
    
    public void UpdateDisplay()
    {
        // Get reference to the local actor
        ILocalActor localActor = SpatialBridge.actorService.localActor;
        
        // Update UI elements with actor information
        displayNameText.text = localActor.displayName;
        userIdText.text = localActor.userID;
        actorNumberText.text = $"#{localActor.actorNumber}";
        
        // Display platform information
        platformText.text = localActor.platform.ToString();
        
        // Show admin/owner badges if applicable
        adminBadge.SetActive(localActor.isSpaceAdministrator);
        ownerBadge.SetActive(localActor.isSpaceOwner);
        
        // Set profile background color
        profileBackground.color = localActor.profileColor;
        
        // Load profile picture asynchronously
        LoadProfilePicture();
    }
    
    private void LoadProfilePicture()
    {
        SpatialBridge.actorService.localActor.GetProfilePicture().SetCompletedEvent((request) => {
            if (request.succeeded)
            {
                profilePicture.texture = request.texture;
            }
            else
            {
                Debug.LogWarning("Failed to load profile picture");
            }
        });
    }
}
```

### Managing Custom Properties

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class PlayerDataManager : MonoBehaviour
{
    // Track if we've initialized player data
    private bool hasInitializedData = false;
    
    private void Start()
    {
        // Check if player data exists, if not initialize it
        InitializePlayerDataIfNeeded();
        
        // Subscribe to custom property changes to keep our local tracking in sync
        SpatialBridge.actorService.localActor.onCustomPropertiesChanged += HandlePropertyChanges;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (SpatialBridge.actorService != null && SpatialBridge.actorService.localActor != null)
        {
            SpatialBridge.actorService.localActor.onCustomPropertiesChanged -= HandlePropertyChanges;
        }
    }
    
    private void InitializePlayerDataIfNeeded()
    {
        ILocalActor localActor = SpatialBridge.actorService.localActor;
        Dictionary<string, object> props = localActor.customProperties;
        
        // Check if the player already has game data initialized
        if (!props.ContainsKey("score") || !props.ContainsKey("level") || !props.ContainsKey("lastLogin"))
        {
            Debug.Log("Initializing new player data");
            
            // Set initial player data
            localActor.SetCustomProperty("score", 0);
            localActor.SetCustomProperty("level", 1);
            localActor.SetCustomProperty("inventory", new int[] { 1001, 1002 }); // Starting inventory items
            localActor.SetCustomProperty("lastLogin", System.DateTime.UtcNow.ToString("o"));
            
            hasInitializedData = true;
        }
        else
        {
            hasInitializedData = true;
            Debug.Log("Player data already exists");
            
            // Update last login time
            localActor.SetCustomProperty("lastLogin", System.DateTime.UtcNow.ToString("o"));
        }
    }
    
    public void AddScore(int points)
    {
        ILocalActor localActor = SpatialBridge.actorService.localActor;
        
        // Get current score
        int currentScore = 0;
        if (localActor.customProperties.TryGetValue("score", out object scoreObj))
        {
            currentScore = (int)scoreObj;
        }
        
        // Update score
        int newScore = currentScore + points;
        localActor.SetCustomProperty("score", newScore);
        
        Debug.Log($"Score updated: {currentScore} → {newScore}");
        
        // Check for level up
        CheckForLevelUp(newScore);
    }
    
    private void CheckForLevelUp(int score)
    {
        ILocalActor localActor = SpatialBridge.actorService.localActor;
        
        // Get current level
        int currentLevel = 1;
        if (localActor.customProperties.TryGetValue("level", out object levelObj))
        {
            currentLevel = (int)levelObj;
        }
        
        // Simple level up formula: level = score / 1000 + 1 (capped at level 10)
        int newLevel = Mathf.Min(10, score / 1000 + 1);
        
        if (newLevel > currentLevel)
        {
            // Level up!
            localActor.SetCustomProperty("level", newLevel);
            Debug.Log($"Level up! {currentLevel} → {newLevel}");
            
            // Grant level up rewards
            GrantLevelUpRewards(newLevel);
        }
    }
    
    private void GrantLevelUpRewards(int level)
    {
        // Implementation for granting level-up rewards
        Debug.Log($"Granting rewards for level {level}");
        
        // Example: Add items to inventory
        AddItemToInventory(2000 + level); // Item ID based on level
    }
    
    public void AddItemToInventory(int itemId)
    {
        ILocalActor localActor = SpatialBridge.actorService.localActor;
        
        // Get current inventory
        int[] inventory = new int[0];
        if (localActor.customProperties.TryGetValue("inventory", out object inventoryObj))
        {
            inventory = (int[])inventoryObj;
        }
        
        // Add new item
        int[] newInventory = new int[inventory.Length + 1];
        System.Array.Copy(inventory, newInventory, inventory.Length);
        newInventory[inventory.Length] = itemId;
        
        // Update inventory
        localActor.SetCustomProperty("inventory", newInventory);
        
        Debug.Log($"Added item {itemId} to inventory");
    }
    
    private void HandlePropertyChanges(ActorCustomPropertiesChangedEventArgs args)
    {
        // Log property changes (this is just for demonstration)
        foreach (var property in args.changedProperties)
        {
            Debug.Log($"Property changed: {property.Key} = {property.Value}");
        }
        
        foreach (var property in args.removedProperties)
        {
            Debug.Log($"Property removed: {property}");
        }
        
        // You could update UI or game state based on specific property changes here
    }
}
```

### Avatar Interaction

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class LocalAvatarController : MonoBehaviour
{
    [SerializeField] private float jumpHeightMultiplier = 1.5f;
    [SerializeField] private float speedMultiplier = 1.2f;
    
    private float defaultJumpHeight;
    private float defaultWalkSpeed;
    private float defaultRunSpeed;
    
    private void Start()
    {
        // Make sure local actor has an avatar
        if (!SpatialBridge.actorService.localActor.hasAvatar)
        {
            Debug.LogWarning("Local actor does not have an avatar");
            
            // Subscribe to avatar exists changed event
            SpatialBridge.actorService.localActor.onAvatarExistsChanged += OnAvatarExistsChanged;
            return;
        }
        
        // Initialize avatar settings
        InitializeAvatarSettings();
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (SpatialBridge.actorService != null && SpatialBridge.actorService.localActor != null)
        {
            SpatialBridge.actorService.localActor.onAvatarExistsChanged -= OnAvatarExistsChanged;
        }
        
        // Reset avatar settings if we modified them
        ResetAvatarSettings();
    }
    
    private void OnAvatarExistsChanged(bool exists)
    {
        if (exists)
        {
            // Avatar now exists, initialize settings
            InitializeAvatarSettings();
        }
    }
    
    private void InitializeAvatarSettings()
    {
        var avatar = SpatialBridge.actorService.localActor.avatar;
        
        // Store default values
        defaultJumpHeight = avatar.jumpHeight;
        defaultWalkSpeed = avatar.walkSpeed;
        defaultRunSpeed = avatar.runSpeed;
        
        Debug.Log($"Default avatar settings - Jump Height: {defaultJumpHeight}, Walk Speed: {defaultWalkSpeed}, Run Speed: {defaultRunSpeed}");
    }
    
    public void ApplySpeedBoost()
    {
        if (!SpatialBridge.actorService.localActor.hasAvatar)
            return;
            
        var avatar = SpatialBridge.actorService.localActor.avatar;
        
        // Apply speed boosts
        avatar.walkSpeed = defaultWalkSpeed * speedMultiplier;
        avatar.runSpeed = defaultRunSpeed * speedMultiplier;
        
        Debug.Log($"Speed boost applied - Walk Speed: {avatar.walkSpeed}, Run Speed: {avatar.runSpeed}");
        SpatialBridge.coreGUIService.DisplayToastMessage("Speed Boost Activated!");
    }
    
    public void ApplyJumpBoost()
    {
        if (!SpatialBridge.actorService.localActor.hasAvatar)
            return;
            
        var avatar = SpatialBridge.actorService.localActor.avatar;
        
        // Apply jump boost
        avatar.jumpHeight = defaultJumpHeight * jumpHeightMultiplier;
        
        Debug.Log($"Jump boost applied - Jump Height: {avatar.jumpHeight}");
        SpatialBridge.coreGUIService.DisplayToastMessage("Jump Boost Activated!");
    }
    
    public void ResetAvatarSettings()
    {
        if (!SpatialBridge.actorService.localActor.hasAvatar)
            return;
            
        var avatar = SpatialBridge.actorService.localActor.avatar;
        
        // Reset to default values
        avatar.jumpHeight = defaultJumpHeight;
        avatar.walkSpeed = defaultWalkSpeed;
        avatar.runSpeed = defaultRunSpeed;
        
        Debug.Log("Avatar settings reset to defaults");
        SpatialBridge.coreGUIService.DisplayToastMessage("Boosts Deactivated");
    }
}
```

## Best Practices

1. **Use for local actor only**: `ILocalActor` is specifically designed for the local user's actor. For other actors, use the standard `IActor` interface.

2. **Handle avatar existence**: Even though the `avatar` property is guaranteed to never be null, always check `hasAvatar` before performing operations on the avatar. When `hasAvatar` is false, all calls to avatar methods and properties will be no-ops.

3. **Manage custom properties cleanly**: Only store data that needs to be synchronized across clients as custom properties. For local-only data, use standard variables in your scripts.

4. **Use appropriate property types**: Use the supported types for custom properties (int, bool, float, Vector2, Vector3, string, Color32, byte, double, long, int[]). Complex data structures should be serialized to a supported format.

5. **Handle property changes**: Subscribe to the `onCustomPropertiesChanged` event to be notified when properties change, especially for properties that affect gameplay or UI elements.

6. **Clean up event subscriptions**: Always unsubscribe from events like `onCustomPropertiesChanged` and `onAvatarExistsChanged` when your components are disabled or destroyed.

7. **Check if properties exist**: When accessing custom properties, always use `TryGetValue()` or check if the key exists before accessing it to avoid errors.

## Common Use Cases

1. **Player Identity Management**: Access and display user information such as display name, user ID, and profile picture.

2. **Persistent Player Data**: Store and synchronize player progress, scores, or statistics across clients using custom properties.

3. **Team or Role Assignment**: Set custom properties to assign players to teams or roles in multiplayer experiences.

4. **Avatar Customization**: Modify the appearance or behavior of the local player's avatar.

5. **Player Status Indicators**: Use custom properties to indicate player status like "ready", "away", or "playing".

6. **Inventory Systems**: Store player inventory or equipment as custom properties to synchronize across clients.

7. **User Preferences**: Store user preferences or settings that should persist between sessions.

## Related Components

- [IActor](./IActor.md): The base interface that ILocalActor extends, representing any actor in the space.
- [IActorService](./IActorService.md): Service that provides access to actors in the space, including the localActor property.
- [IAvatar](./IAvatar.md): Interface for controlling avatars, accessed through the actor's avatar property.
- [ActorCustomPropertiesChangedEventArgs](./ActorCustomPropertiesChangedEventArgs.md): Event arguments for the onCustomPropertiesChanged event.