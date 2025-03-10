# SpatialAvatarTeleporter

Category: Core Components

Interface/Class/Enum: Class

The SpatialAvatarTeleporter component allows avatars to be teleported to a specified target location in the Spatial environment. This component creates an interactive area that, when triggered, will instantly move the user's avatar to the designated destination.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| targetLocation | Transform | Specifies the destination transform where the avatar will be teleported. |
| documentationURL | string | URL to documentation for this component. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    [SerializeField] private Transform[] teleportDestinations;
    
    private void Awake()
    {
        // Set up teleporters programmatically
        SetupTeleporters();
    }
    
    private void SetupTeleporters()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform teleporterTransform = transform.GetChild(i);
            
            // Create teleporter for each child object
            if (!teleporterTransform.TryGetComponent<SpatialAvatarTeleporter>(out var teleporter))
            {
                teleporter = teleporterTransform.gameObject.AddComponent<SpatialAvatarTeleporter>();
            }
            
            // Assign destination (cyclic through available destinations)
            int destinationIndex = i % teleportDestinations.Length;
            teleporter.targetLocation = teleportDestinations[destinationIndex];
            
            Debug.Log($"Teleporter {teleporterTransform.name} configured to teleport to {teleportDestinations[destinationIndex].name}");
        }
    }
}
```

## Best Practices

1. Always ensure the targetLocation is properly set to a valid transform, otherwise teleportation may not work as expected.
2. Place the target location in an open area where the avatar won't collide with objects or get stuck inside geometry.
3. Consider adding visual effects or audio cues to telegraph teleportation points to users.
4. Use teleporters strategically to help users navigate large spaces or reach otherwise inaccessible areas.
5. The teleporter should be placed on the ground or in a location that's accessible for avatars to interact with.
6. For networked experiences, remember that teleportation instantly moves the user's avatar, which may affect gameplay or user experience for other participants.

## Common Use Cases

1. Fast travel systems between distant parts of a large environment.
2. Level transitions between different scenes or areas.
3. Shortcuts to frequently visited locations.
4. Access points to elevated or hard-to-reach areas.
5. Escape mechanisms from trap areas or confined spaces.
6. Entrance and exit points for special activities or mini-games.

## Completed: March 09, 2025