# ActorProfilePictureRequest

Category: Actor Service

Class: Async Operation

`ActorProfilePictureRequest` is a class that represents the result of a request to get an actor's profile picture. It inherits from `SpatialAsyncOperation` and provides access to the actor's profile picture texture once the request is completed.

## Properties/Fields

| Property | Description |
| --- | --- |
| actorNumber | The actor number of the actor that the profile picture is for. |
| succeeded | A boolean indicating whether the request was successful. |
| texture | The profile picture texture for the actor. Will be null if the actor or texture could not be found. |

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

### Basic Profile Picture Loading

```csharp
using UnityEngine;
using UnityEngine.UI;
using SpatialSys.UnitySDK;

public class ProfilePictureLoader : MonoBehaviour
{
    public RawImage profileImage;
    public Image profileBackground;
    
    private void Start()
    {
        LoadLocalPlayerProfilePicture();
    }
    
    public void LoadLocalPlayerProfilePicture()
    {
        // Get the local actor's profile picture
        SpatialBridge.actorService.localActor.GetProfilePicture().SetCompletedEvent((request) => {
            if (request.succeeded)
            {
                // Set the texture to the Raw Image component
                profileImage.texture = request.texture;
                
                // Set the background color to match the actor's profile color
                profileBackground.color = SpatialBridge.actorService.localActor.profileColor;
            }
            else
            {
                Debug.LogWarning("Failed to load profile picture for local actor");
            }
        });
    }
}
```

### Player List with Profile Pictures

```csharp
using UnityEngine;
using UnityEngine.UI;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class PlayerListManager : MonoBehaviour
{
    [SerializeField] private GameObject playerEntryPrefab;
    [SerializeField] private Transform playerListContainer;
    
    // Dictionary to track created player entries
    private Dictionary<int, GameObject> playerEntries = new Dictionary<int, GameObject>();
    
    private void OnEnable()
    {
        // Subscribe to actor events
        SpatialBridge.actorService.onActorJoined += HandleActorJoined;
        SpatialBridge.actorService.onActorLeft += HandleActorLeft;
        
        // Initialize with existing actors
        foreach (var actor in SpatialBridge.actorService.actors.Values)
        {
            CreatePlayerEntry(actor);
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe when disabled
        SpatialBridge.actorService.onActorJoined -= HandleActorJoined;
        SpatialBridge.actorService.onActorLeft -= HandleActorLeft;
        
        // Clean up
        ClearAllEntries();
    }
    
    private void HandleActorJoined(ActorJoinedEventArgs args)
    {
        IActor actor = SpatialBridge.actorService.actors[args.actorNumber];
        CreatePlayerEntry(actor);
    }
    
    private void HandleActorLeft(ActorLeftEventArgs args)
    {
        RemovePlayerEntry(args.actorNumber);
    }
    
    private void CreatePlayerEntry(IActor actor)
    {
        // Don't create duplicate entries
        if (playerEntries.ContainsKey(actor.actorNumber))
        {
            return;
        }
        
        // Instantiate the player entry prefab
        GameObject entryObject = Instantiate(playerEntryPrefab, playerListContainer);
        playerEntries[actor.actorNumber] = entryObject;
        
        // Set up the entry with actor info
        PlayerEntryUI entryUI = entryObject.GetComponent<PlayerEntryUI>();
        if (entryUI != null)
        {
            entryUI.SetPlayerName(actor.displayName);
            
            // Load profile picture asynchronously
            LoadProfilePicture(actor, entryUI);
        }
    }
    
    private void LoadProfilePicture(IActor actor, PlayerEntryUI entryUI)
    {
        // Request the actor's profile picture
        actor.GetProfilePicture().SetCompletedEvent((request) => {
            if (request.succeeded)
            {
                // Update the UI with the profile picture
                entryUI.SetProfilePicture(request.texture, actor.profileColor);
            }
            else
            {
                // Use a fallback if picture loading failed
                entryUI.SetDefaultProfilePicture(actor.profileColor);
            }
        });
    }
    
    private void RemovePlayerEntry(int actorNumber)
    {
        if (playerEntries.TryGetValue(actorNumber, out GameObject entryObject))
        {
            Destroy(entryObject);
            playerEntries.Remove(actorNumber);
        }
    }
    
    private void ClearAllEntries()
    {
        foreach (var entry in playerEntries.Values)
        {
            Destroy(entry);
        }
        
        playerEntries.Clear();
    }
}

// Example component for the player entry UI
public class PlayerEntryUI : MonoBehaviour
{
    [SerializeField] private Text playerNameText;
    [SerializeField] private RawImage profilePictureImage;
    [SerializeField] private Image profileBackgroundImage;
    
    public void SetPlayerName(string name)
    {
        playerNameText.text = name;
    }
    
    public void SetProfilePicture(Texture texture, Color backgroundColor)
    {
        profilePictureImage.texture = texture;
        profileBackgroundImage.color = backgroundColor;
    }
    
    public void SetDefaultProfilePicture(Color backgroundColor)
    {
        profilePictureImage.texture = null; // Or use a default texture
        profileBackgroundImage.color = backgroundColor;
    }
}
```

## Best Practices

1. **Always check the succeeded property**: Before accessing the texture, verify that the request was successful to avoid null reference exceptions.

2. **Handle loading failures gracefully**: Provide fallback textures or visuals for cases when profile pictures can't be loaded.

3. **Clean up textures when done**: If you're creating many profile picture requests, consider manually cleaning up textures that are no longer needed to prevent memory leaks.

4. **Use the SetCompletedEvent extension method**: This provides a cleaner syntax than manually subscribing to the completed event.

5. **Combine with actor.profileColor**: The actor's profile color is often used as a background or accent color for the profile picture, creating a consistent visual identity.

6. **Cache profile pictures when appropriate**: If you frequently access the same actor's profile picture, consider caching the texture after the first successful load instead of making multiple requests.

7. **Perform UI updates on the main thread**: Remember that the completed event might fire on a background thread, so ensure UI updates happen on the main thread.

## Common Use Cases

1. **Player profile displays**: Show user profile pictures in UI elements like player cards, leaderboards, or menus.

2. **Chat interfaces**: Display profile pictures alongside chat messages in communication interfaces.

3. **Team/squad member visualization**: Show team members with their profile pictures for easy identification.

4. **Friend lists**: Display profile pictures in friend lists or social features.

5. **Player identification**: Use profile pictures to help players identify each other in multiplayer experiences.

6. **Interactive player directories**: Create interactive directories where players can browse and learn about others in the space.

7. **Customized notification systems**: Include profile pictures in custom notifications or toast messages for a more personalized experience.

## Related Components

- [IActor](./IActor.md): Interface that represents an actor in the Spatial environment and provides the GetProfilePicture method.
- [IActorService](./IActorService.md): Service that manages actors in the Spatial environment.
- [SpatialAsyncOperation](./SpatialAsyncOperation.md): Base class for asynchronous operations in Spatial.