# IReadOnlyAvatar

Category: Actor Service

Interface: Avatar Interface

`IReadOnlyAvatar` is an interface that provides read-only access to an avatar in the Spatial environment. It contains properties and events related to the avatar's appearance, movement, physics, and state. This interface is implemented by both local and remote avatars, but only provides read access to the avatar's properties.

## Properties/Fields

| Property | Description |
| --- | --- |
| airControl | How much control the player has over the character while in the air. A value of 0 means no control, 1 means full control. |
| bodyMaterials | Returns a collection of all materials used on the avatar's body. This can be used to change the appearance of the avatar. |
| displayName | The display name shown in the nametag above the avatar's head. |
| fallingGravityMultiplier | Multiplier on top of the default gravity settings for the space just for the local avatar while falling. This stacks on top of `gravityMultiplier`. This is useful for making the avatar fall faster than they jump. |
| gravityMultiplier | Multiplier on top of the default gravity settings for the space just for the local avatar. |
| groundFriction | Contribution of how much ground friction to apply to the character. A higher value will give the avatar more grip resulting in higher acceleration. This does not affect the avatar's maximum movement speed. Values should be between 0 and 1. |
| isBodyLoaded | Whether the avatar body is fully loaded. When users join a space, their avatar body is not immediately loaded. This property will be false until the avatar body is loaded. It's also possible for users to switch their avatars on the fly, so this property may change at any time. |
| isGrounded | Whether the avatar is currently grounded (the feet are touching the ground). |
| jumpHeight | The height in meters that the avatar can jump. |
| maxJumpCount | Maximum number of times that the avatar can jump in a row before touching the ground. |
| nametagBarValue | Optional nametag bar value shown in the nametag above the avatar's head. |
| nametagBarVisible | Whether the nametag bar is visible. |
| nametagSubtext | An optional subtext shown in the nametag above the avatar's head. |
| nametagVisible | Whether the nametag should be visible to other users. |
| position | The current position of the avatar's visual representation in the scene. This is the position of the avatar at the feet. |
| ragdollPhysicsActive | Is ragdoll physics currently active for the avatar? |
| ragdollVelocity | The current velocity of the avatar's ragdoll body. |
| rotation | The orientation of the avatar's visual representation in the scene. Currently this is always locked to Y-up. |
| runSpeed | The running speed of the avatar in meters per second. |
| useVariableHeightJump | When enabled, jump is higher depending on how long jump button is held down. Currently variable jump height may result in a slightly higher `jumpHeight`. |
| velocity | The current velocity of the avatar. |
| visibleLocally | Whether the avatar is visible in the scene. |
| visibleRemotely | Whether the avatar is visible in the scene to other remote users. |
| walkSpeed | The walking speed of the avatar in meters per second. |

## Methods

| Method | Description |
| --- | --- |
| GetAvatarBoneTransform(HumanBodyBones) | Get the transform of a bone in the avatar's body. |

## Events

| Event | Description |
| --- | --- |
| onAvatarBeforeUnload | Event that triggers when the avatar is about to be unloaded. If the avatar is owned by an actor, this may be because the actor has changed their avatar, or because the actor is leaving or disconnecting. This can be used to "deconstruct" anything that was created in the `onAvatarLoadComplete` event. `isBodyLoaded` will be set to false after this event is triggered. |
| onAvatarLoadComplete | Event that triggers when the avatar has finished loading. `isBodyLoaded` will be set to true before this event is triggered. |
| onColliderHit | Event that is triggered when the avatar's collider hits another collider. |
| onEmote | Event that is triggered when an emote avatar animation is started. Note that this doesn't trigger immediately when PlayEmote is called, but when the animation is loaded (asynchronously) and started. |
| onIsGroundedChanged | Event that is triggered when the avatar's grounded state changes (`isGrounded`). |
| onJump | Event that is triggered when the avatar starts to jump. |
| onLanded | Event that is triggered when the avatar lands on the ground. |
| onRespawned | Event that triggers when the avatar is respawned. Respawn can happen when: a user's avatar enters a space and is spawned for the first time, an avatar is explicitly respawned with `Respawn`, or an avatar goes out of bounds. For avatars created explicitly through `SpawnAvatar` this event will not trigger. |

## Inherited Members

| Member | Description |
| --- | --- |
| Equals(IAvatar) | Determines if this avatar is equal to another avatar. |
| Equals(IReadOnlyAvatar) | Determines if this avatar is equal to another avatar. |
| isDisposed | Returns true when the component or its parent space object has been destroyed. |
| spaceObject | The space object the component is attached to. |
| spaceObjectID | The id of the spaceObject the component is attached to. |

## Usage Examples

### Avatar Event Listener

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class AvatarEventListener : MonoBehaviour
{
    private void OnEnable()
    {
        // Get the local avatar
        IReadOnlyAvatar avatar = SpatialBridge.actorService.localActor.avatar;
        
        // Only register events if the avatar exists
        if (SpatialBridge.actorService.localActor.hasAvatar)
        {
            RegisterAvatarEvents(avatar);
        }
        
        // Listen for avatar changes
        SpatialBridge.actorService.localActor.onAvatarExistsChanged += OnAvatarExistsChanged;
    }
    
    private void OnDisable()
    {
        // Unregister from events if the avatar exists
        if (SpatialBridge.actorService.localActor.hasAvatar)
        {
            UnregisterAvatarEvents(SpatialBridge.actorService.localActor.avatar);
        }
        
        // Unsubscribe from avatar change events
        SpatialBridge.actorService.localActor.onAvatarExistsChanged -= OnAvatarExistsChanged;
    }
    
    private void OnAvatarExistsChanged(bool exists)
    {
        if (exists)
        {
            RegisterAvatarEvents(SpatialBridge.actorService.localActor.avatar);
        }
        else
        {
            UnregisterAvatarEvents(SpatialBridge.actorService.localActor.avatar);
        }
    }
    
    private void RegisterAvatarEvents(IReadOnlyAvatar avatar)
    {
        // Subscribe to avatar events
        avatar.onJump += OnAvatarJump;
        avatar.onLanded += OnAvatarLanded;
        avatar.onIsGroundedChanged += OnGroundedChanged;
        avatar.onColliderHit += OnColliderHit;
        avatar.onRespawned += OnAvatarRespawned;
        
        // Check if the avatar body is loaded
        if (!avatar.isBodyLoaded)
        {
            avatar.onAvatarLoadComplete += OnAvatarLoaded;
        }
        else
        {
            OnAvatarLoaded();
        }
        
        avatar.onAvatarBeforeUnload += OnAvatarUnloading;
    }
    
    private void UnregisterAvatarEvents(IReadOnlyAvatar avatar)
    {
        // Unsubscribe from all events
        avatar.onJump -= OnAvatarJump;
        avatar.onLanded -= OnAvatarLanded;
        avatar.onIsGroundedChanged -= OnGroundedChanged;
        avatar.onColliderHit -= OnColliderHit;
        avatar.onRespawned -= OnAvatarRespawned;
        avatar.onAvatarLoadComplete -= OnAvatarLoaded;
        avatar.onAvatarBeforeUnload -= OnAvatarUnloading;
    }
    
    private void OnAvatarJump()
    {
        Debug.Log("Avatar jumped");
        // Play jump sound or effect
    }
    
    private void OnAvatarLanded()
    {
        Debug.Log("Avatar landed");
        // Play landing sound or effect
    }
    
    private void OnGroundedChanged(bool isGrounded)
    {
        Debug.Log($"Avatar grounded state changed: {isGrounded}");
    }
    
    private void OnColliderHit(UnityEngine.Collision collision)
    {
        Debug.Log($"Avatar collided with: {collision.gameObject.name}");
    }
    
    private void OnAvatarRespawned(AvatarRespawnEventArgs args)
    {
        Debug.Log($"Avatar respawned. First spawn: {args.isFirstSpawn}");
    }
    
    private void OnAvatarLoaded()
    {
        Debug.Log("Avatar body loaded");
        // Now it's safe to access avatar bones and materials
    }
    
    private void OnAvatarUnloading()
    {
        Debug.Log("Avatar body about to unload");
        // Clean up any avatar-specific resources
    }
}
```

### Avatar Status UI

```csharp
using UnityEngine;
using UnityEngine.UI;
using SpatialSys.UnitySDK;
using System.Collections;

public class AvatarStatusUI : MonoBehaviour
{
    [SerializeField] private Text speedText;
    [SerializeField] private Text positionText;
    [SerializeField] private Text groundedText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Text nameTagText;
    [SerializeField] private float updateInterval = 0.2f;
    
    private IReadOnlyAvatar targetAvatar;
    private Coroutine updateCoroutine;
    
    public void SetTargetAvatar(IReadOnlyAvatar avatar)
    {
        // Clean up previous target if any
        if (targetAvatar != null)
        {
            UnregisterAvatarEvents(targetAvatar);
        }
        
        targetAvatar = avatar;
        
        if (targetAvatar != null)
        {
            // Update UI elements immediately
            UpdateUI();
            
            // Register for events
            RegisterAvatarEvents(targetAvatar);
            
            // Start periodic updates
            if (updateCoroutine != null)
            {
                StopCoroutine(updateCoroutine);
            }
            updateCoroutine = StartCoroutine(UpdateUICoroutine());
        }
    }
    
    private void RegisterAvatarEvents(IReadOnlyAvatar avatar)
    {
        avatar.onIsGroundedChanged += OnGroundedChanged;
    }
    
    private void UnregisterAvatarEvents(IReadOnlyAvatar avatar)
    {
        avatar.onIsGroundedChanged -= OnGroundedChanged;
    }
    
    private void OnGroundedChanged(bool isGrounded)
    {
        // Update grounded status immediately when it changes
        groundedText.text = $"Grounded: {isGrounded}";
        groundedText.color = isGrounded ? Color.green : Color.red;
    }
    
    private IEnumerator UpdateUICoroutine()
    {
        while (targetAvatar != null && !targetAvatar.isDisposed)
        {
            UpdateUI();
            yield return new WaitForSeconds(updateInterval);
        }
        
        // Avatar was disposed, clear UI
        ClearUI();
    }
    
    private void UpdateUI()
    {
        if (targetAvatar == null || targetAvatar.isDisposed)
        {
            ClearUI();
            return;
        }
        
        // Update position display
        Vector3 pos = targetAvatar.position;
        positionText.text = $"Position: ({pos.x:F1}, {pos.y:F1}, {pos.z:F1})";
        
        // Update speed display
        float speed = targetAvatar.velocity.magnitude;
        speedText.text = $"Speed: {speed:F1} m/s";
        
        // Update grounded status
        groundedText.text = $"Grounded: {targetAvatar.isGrounded}";
        groundedText.color = targetAvatar.isGrounded ? Color.green : Color.red;
        
        // Update health bar (assuming we store health in a custom property)
        UpdateHealthBar();
        
        // Update nametag
        nameTagText.text = targetAvatar.displayName;
    }
    
    private void UpdateHealthBar()
    {
        // This is just an example - in a real implementation, you'd get health
        // from a custom property or game state
        
        // For demonstration, we'll just set a random health value
        if (Random.value < 0.05f) // Only change occasionally for demo purposes
        {
            healthBar.value = Random.value;
        }
    }
    
    private void ClearUI()
    {
        speedText.text = "Speed: --";
        positionText.text = "Position: --";
        groundedText.text = "Grounded: --";
        nameTagText.text = "";
        healthBar.value = 0;
    }
    
    private void OnDestroy()
    {
        // Clean up
        if (targetAvatar != null && !targetAvatar.isDisposed)
        {
            UnregisterAvatarEvents(targetAvatar);
        }
        
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
        }
    }
}
```

### Accessing Avatar Bones

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class AvatarAttachmentManager : MonoBehaviour
{
    [System.Serializable]
    public class AttachmentConfig
    {
        public GameObject prefab;
        public HumanBodyBones bone;
        public Vector3 positionOffset;
        public Vector3 rotationOffset;
        public Vector3 scale = Vector3.one;
    }
    
    [SerializeField] private AttachmentConfig[] attachments;
    
    private GameObject[] spawnedAttachments;
    
    private void Start()
    {
        // Initialize array to track spawned attachments
        spawnedAttachments = new GameObject[attachments.Length];
        
        // Check if local actor has an avatar
        if (SpatialBridge.actorService.localActor.hasAvatar)
        {
            IReadOnlyAvatar avatar = SpatialBridge.actorService.localActor.avatar;
            
            // Check if avatar body is loaded
            if (avatar.isBodyLoaded)
            {
                SpawnAttachments(avatar);
            }
            else
            {
                // Wait for avatar to load
                avatar.onAvatarLoadComplete += OnAvatarLoaded;
            }
        }
        
        // Subscribe to avatar changes
        SpatialBridge.actorService.localActor.onAvatarExistsChanged += OnAvatarExistsChanged;
    }
    
    private void OnDestroy()
    {
        // Clean up
        if (SpatialBridge.actorService.localActor.hasAvatar)
        {
            IReadOnlyAvatar avatar = SpatialBridge.actorService.localActor.avatar;
            avatar.onAvatarLoadComplete -= OnAvatarLoaded;
            avatar.onAvatarBeforeUnload -= OnAvatarUnloading;
        }
        
        SpatialBridge.actorService.localActor.onAvatarExistsChanged -= OnAvatarExistsChanged;
        
        DestroyAllAttachments();
    }
    
    private void OnAvatarExistsChanged(bool exists)
    {
        if (exists)
        {
            // Avatar now exists
            IReadOnlyAvatar avatar = SpatialBridge.actorService.localActor.avatar;
            
            // Check if avatar body is loaded
            if (avatar.isBodyLoaded)
            {
                SpawnAttachments(avatar);
            }
            else
            {
                // Wait for avatar to load
                avatar.onAvatarLoadComplete += OnAvatarLoaded;
            }
            
            // Subscribe to unload event
            avatar.onAvatarBeforeUnload += OnAvatarUnloading;
        }
        else
        {
            // Avatar no longer exists
            DestroyAllAttachments();
        }
    }
    
    private void OnAvatarLoaded()
    {
        // Avatar body is now loaded, spawn attachments
        IReadOnlyAvatar avatar = SpatialBridge.actorService.localActor.avatar;
        SpawnAttachments(avatar);
    }
    
    private void OnAvatarUnloading()
    {
        // Avatar is about to be unloaded, destroy attachments
        DestroyAllAttachments();
    }
    
    private void SpawnAttachments(IReadOnlyAvatar avatar)
    {
        // First destroy any existing attachments
        DestroyAllAttachments();
        
        // Spawn each attachment
        for (int i = 0; i < attachments.Length; i++)
        {
            AttachmentConfig config = attachments[i];
            
            // Get the bone transform
            Transform boneTransform = avatar.GetAvatarBoneTransform(config.bone);
            
            if (boneTransform != null)
            {
                // Instantiate the prefab
                GameObject attachment = Instantiate(config.prefab, boneTransform);
                
                // Set position, rotation, and scale
                attachment.transform.localPosition = config.positionOffset;
                attachment.transform.localRotation = Quaternion.Euler(config.rotationOffset);
                attachment.transform.localScale = config.scale;
                
                // Store reference for later cleanup
                spawnedAttachments[i] = attachment;
            }
            else
            {
                Debug.LogWarning($"Bone {config.bone} not found on avatar");
            }
        }
    }
    
    private void DestroyAllAttachments()
    {
        for (int i = 0; i < spawnedAttachments.Length; i++)
        {
            if (spawnedAttachments[i] != null)
            {
                Destroy(spawnedAttachments[i]);
                spawnedAttachments[i] = null;
            }
        }
    }
}
```

## Best Practices

1. **Check avatar existence and loading**: Always check if the avatar exists (using `hasAvatar`) and if the body is loaded (using `isBodyLoaded`) before accessing avatar properties or bones. Use the `onAvatarLoadComplete` event to be notified when the avatar body is fully loaded.

2. **Handle avatar changes**: Remember that avatars can change during a session. Use the actor's `onAvatarExistsChanged` event to track when a user's avatar changes.

3. **Clean up resources**: Always clean up any resources (like attached GameObjects) when an avatar is about to be unloaded. Subscribe to the `onAvatarBeforeUnload` event to be notified when this happens.

4. **Check for disposal**: Always check if the avatar is disposed (using `isDisposed`) before accessing its properties, especially in coroutines or after some time might have passed.

5. **Use appropriate events**: When tracking movement or state changes, use the appropriate events (`onJump`, `onLanded`, `onIsGroundedChanged`) instead of polling properties every frame.

6. **Thread safety**: Remember that event handlers might be called from different threads. Keep UI updates and resource creation on the main thread.

7. **Proper event unsubscription**: Always unsubscribe from events when your components are disabled or destroyed to prevent memory leaks.

## Common Use Cases

1. **Customizing avatar appearance**: Use the `bodyMaterials` collection to modify the avatar's appearance, changing colors or textures.

2. **Attaching items to avatars**: Use `GetAvatarBoneTransform` to attach weapons, accessories, or effects to specific bones on the avatar.

3. **Avatar tracking**: Monitor the avatar's position and velocity for gameplay or analytics purposes.

4. **Custom movement effects**: Create particle effects or sounds that respond to the avatar's movement, using events like `onJump` and `onLanded`.

5. **Health or status visualization**: Use the nametag properties (`nametagSubtext`, `nametagBarVisible`, `nametagBarValue`) to display health, status, or other information above the avatar.

6. **Avatar collision responses**: Use the `onColliderHit` event to create custom collision responses, like damage effects or bounce mechanics.

7. **First-time user experiences**: Use the `onRespawned` event with `isFirstSpawn` to show tutorials or welcome messages to users entering the space for the first time.

## Related Components

- [IAvatar](./IAvatar.md): Interface that extends IReadOnlyAvatar with additional methods for controlling the avatar.
- [IActor](./IActor.md): Interface that represents an actor in the Spatial environment, which owns an avatar.
- [ILocalActor](./ILocalActor.md): Interface that represents the local actor, which provides access to the local avatar.
- [AvatarRespawnEventArgs](./AvatarRespawnEventArgs.md): Event arguments for the onRespawned event.