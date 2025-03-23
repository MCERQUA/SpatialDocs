# IAvatar

Avatar Interface

Interface representing an avatar in a Spatial space. This interface provides comprehensive control over avatar appearance, movement, physics, and attachments.

## Properties

### Movement
| Property | Description |
| --- | --- |
| position | Avatar position |
| rotation | Avatar orientation |
| velocity | Current velocity |
| isGrounded | Ground contact state |

### Movement Settings
| Property | Description |
| --- | --- |
| walkSpeed | Walking velocity |
| runSpeed | Running velocity |
| jumpHeight | Jump height |
| maxJumpCount | Multi-jump limit |
| useVariableHeightJump | Variable jump control |

### Physics
| Property | Description |
| --- | --- |
| airControl | Air movement control |
| gravityMultiplier | Gravity modifier |
| fallingGravityMultiplier | Fall gravity boost |
| groundFriction | Ground grip factor |
| ragdollVelocity | Ragdoll velocity |

### Appearance
| Property | Description |
| --- | --- |
| visibleLocally | Local visibility |
| visibleRemotely | Remote visibility |
| bodyMaterials | Avatar materials |
| isBodyLoaded | Body load status |

### Nametag
| Property | Description |
| --- | --- |
| nametagVisible | Show nametag |
| nametagSubtext | Nametag subtitle |
| nametagBarVisible | Show status bar |
| nametagBarValue | Status bar value |
| displayName | Display name |

## Methods

### Movement Control
| Method | Description |
| --- | --- |
| Move(Vector2, bool) | Input movement |
| Jump() | Perform jump |
| SetDestination(Vector3, bool) | AI navigation |
| SetPositionRotation(Vector3, Quaternion) | Teleport avatar |

### Physics Control
| Method | Description |
| --- | --- |
| AddForce(Vector3) | Apply force |
| AddRagdollForce(Vector3, bool) | Ragdoll force |
| SetRagdollPhysicsActive(bool, Vector3) | Toggle ragdoll |

### Attachments
| Method | Description |
| --- | --- |
| EquipAttachment(...) | Add attachment |
| ClearAttachments() | Remove all |
| ClearAttachmentSlot(Slot) | Clear slot |
| ClearAttachmentsByTag(string) | Clear by tag |
| IsAttachmentEquipped(string) | Check equipped |

### Animation
| Method | Description |
| --- | --- |
| PlayEmote(...) | Play animation |
| StopEmote() | Stop animation |

### Avatar Control
| Method | Description |
| --- | --- |
| SetAvatarBody(...) | Change body |
| ResetAvatarBody() | Restore body |
| Respawn() | Reset position |
| Sit(Transform) | Sit at target |
| Stand() | Stand up |

### Bone Access
| Method | Description |
| --- | --- |
| GetAvatarBoneTransform(HumanBodyBones) | Get bone transform |

## Events

### State Changes
| Event | Description |
| --- | --- |
| onAvatarLoadComplete | Load finished |
| onAvatarBeforeUnload | Pre-unload |
| onAttachmentEquippedChanged | Equipment change |
| onEmote | Animation start |

### Movement Events
| Event | Description |
| --- | --- |
| onJump | Jump start |
| onLanded | Ground contact |
| onIsGroundedChanged | Ground state |
| onRespawned | Position reset |
| onColliderHit | Collision detect |

## Usage Examples

```csharp
// Example: Avatar Controller
public class AvatarController : MonoBehaviour
{
    private IAvatar avatar;
    private Dictionary<string, AvatarState> avatarStates;
    private bool isInitialized;

    private class AvatarState
    {
        public bool isActive;
        public float lastUpdateTime;
        public Dictionary<string, object> settings;
        public Dictionary<string, object> metrics;
    }

    void Start()
    {
        avatar = GetComponent<IAvatar>();
        avatarStates = new Dictionary<string, AvatarState>();
        InitializeController();
        SubscribeToEvents();
    }

    private void InitializeController()
    {
        InitializeAvatarState("movement", new Dictionary<string, object>
        {
            { "walkSpeed", 5.0f },
            { "runSpeed", 10.0f },
            { "jumpHeight", 2.0f },
            { "airControl", 0.5f }
        });

        InitializeAvatarState("physics", new Dictionary<string, object>
        {
            { "gravityMultiplier", 1.0f },
            { "groundFriction", 0.8f },
            { "useRagdoll", false }
        });
    }

    private void SubscribeToEvents()
    {
        avatar.onJump += HandleJump;
        avatar.onLanded += HandleLanded;
        avatar.onColliderHit += HandleCollision;
        avatar.onAvatarLoadComplete += HandleAvatarLoaded;
    }

    public void UpdateMovement(Vector2 input, bool isRunning)
    {
        try
        {
            if (!CanMove())
                return;

            avatar.Move(input, isRunning);

            UpdateAvatarMetrics("movement", new Dictionary<string, object>
            {
                { "lastInput", input },
                { "isRunning", isRunning },
                { "velocity", avatar.velocity }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating movement: {e.Message}");
        }
    }

    public void PerformJump()
    {
        try
        {
            if (!CanJump())
                return;

            avatar.Jump();

            UpdateAvatarMetrics("movement", new Dictionary<string, object>
            {
                { "lastJump", DateTime.UtcNow },
                { "jumpCount", GetJumpCount() + 1 }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error performing jump: {e.Message}");
        }
    }

    public void EquipItem(
        string itemId,
        string slot,
        bool replaceExisting = true
    )
    {
        try
        {
            if (!CanEquip(itemId))
                return;

            avatar.EquipAttachment(
                AssetType.AvatarAttachment,
                itemId,
                replaceExisting,
                true,
                slot
            );

            UpdateAvatarMetrics("equipment", new Dictionary<string, object>
            {
                { "lastEquip", DateTime.UtcNow },
                { "itemId", itemId },
                { "slot", slot }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Error equipping item: {e.Message}");
        }
    }

    private void InitializeAvatarState(
        string stateId,
        Dictionary<string, object> settings
    )
    {
        var state = new AvatarState
        {
            isActive = true,
            lastUpdateTime = Time.time,
            settings = settings,
            metrics = new Dictionary<string, object>()
        };

        avatarStates[stateId] = state;
    }

    private bool CanMove()
    {
        return avatar != null &&
               !avatar.isDisposed &&
               avatarStates["movement"].isActive;
    }

    private bool CanJump()
    {
        return CanMove() &&
               avatar.isGrounded &&
               GetJumpCount() < avatar.maxJumpCount;
    }

    private bool CanEquip(string itemId)
    {
        return avatar != null &&
               !avatar.isDisposed &&
               !string.IsNullOrEmpty(itemId) &&
               !avatar.IsAttachmentEquipped(itemId);
    }

    private int GetJumpCount()
    {
        if (!avatarStates["movement"].metrics.ContainsKey("jumpCount"))
            return 0;
        return (int)avatarStates["movement"].metrics["jumpCount"];
    }

    private void HandleJump()
    {
        UpdateAvatarMetrics("movement", new Dictionary<string, object>
        {
            { "lastJumpStart", DateTime.UtcNow }
        });
    }

    private void HandleLanded()
    {
        UpdateAvatarMetrics("movement", new Dictionary<string, object>
        {
            { "lastLanding", DateTime.UtcNow },
            { "jumpCount", 0 }
        });
    }

    private void HandleCollision(Collision collision)
    {
        UpdateAvatarMetrics("physics", new Dictionary<string, object>
        {
            { "lastCollision", DateTime.UtcNow },
            { "collisionForce", collision.impulse.magnitude }
        });
    }

    private void HandleAvatarLoaded()
    {
        UpdateAvatarMetrics("state", new Dictionary<string, object>
        {
            { "loadTime", DateTime.UtcNow },
            { "materials", avatar.bodyMaterials.Count }
        });
    }

    private void UpdateAvatarMetrics(
        string stateId,
        Dictionary<string, object> metrics
    )
    {
        if (!avatarStates.TryGetValue(stateId, out var state))
            return;

        foreach (var kvp in metrics)
        {
            state.metrics[kvp.Key] = kvp.Value;
        }

        state.lastUpdateTime = Time.time;
    }

    void OnDestroy()
    {
        if (avatar != null)
        {
            avatar.onJump -= HandleJump;
            avatar.onLanded -= HandleLanded;
            avatar.onColliderHit -= HandleCollision;
            avatar.onAvatarLoadComplete -= HandleAvatarLoaded;
        }
    }
}

// Example: Avatar Monitor
public class AvatarMonitor : MonoBehaviour
{
    private AvatarController avatarController;
    private Dictionary<string, MonitorState> monitorStates;
    private bool isInitialized;

    private class MonitorState
    {
        public bool isActive;
        public float lastUpdateTime;
        public Dictionary<string, object> settings;
    }

    void Start()
    {
        avatarController = GetComponent<AvatarController>();
        monitorStates = new Dictionary<string, MonitorState>();
        InitializeMonitors();
    }

    private void InitializeMonitors()
    {
        InitializeMonitor("performance", new Dictionary<string, object>
        {
            { "updateInterval", 1.0f },
            { "velocityThreshold", 20.0f }
        });

        InitializeMonitor("physics", new Dictionary<string, object>
        {
            { "updateInterval", 0.5f },
            { "collisionThreshold", 10.0f }
        });
    }

    private void InitializeMonitor(
        string monitorId,
        Dictionary<string, object> settings
    )
    {
        var state = new MonitorState
        {
            isActive = true,
            lastUpdateTime = Time.time,
            settings = settings
        };

        monitorStates[monitorId] = state;
    }

    private void UpdateMonitors()
    {
        foreach (var state in monitorStates.Values)
        {
            if (!state.isActive)
                continue;

            var interval = (float)state.settings["updateInterval"];
            if (Time.time - state.lastUpdateTime >= interval)
            {
                UpdateMonitorMetrics(state);
                state.lastUpdateTime = Time.time;
            }
        }
    }

    private void UpdateMonitorMetrics(MonitorState state)
    {
        // Implementation for monitor metric updates
    }

    void Update()
    {
        UpdateMonitors();
    }
}
```

## Best Practices

1. Avatar Control
   - Handle movement
   - Track states
   - Manage physics
   - Cache data

2. Movement Control
   - Smooth input
   - Handle transitions
   - Track metrics
   - Monitor collisions

3. Performance
   - Batch updates
   - Cache states
   - Handle timing
   - Optimize checks

4. Error Handling
   - Validate state
   - Handle failures
   - Recover data
   - Log issues

## Common Use Cases

1. Avatar States
   - Movement control
   - Physics simulation
   - Animation handling
   - Equipment management

2. Avatar Features
   - Custom movement
   - Physics interactions
   - Equipment systems
   - Animation control

3. Avatar Systems
   - Movement mechanics
   - Physics behaviors
   - Equipment loadouts
   - Animation sequences

4. Avatar Processing
   - State validation
   - Update handling
   - Event processing
   - Metric tracking
