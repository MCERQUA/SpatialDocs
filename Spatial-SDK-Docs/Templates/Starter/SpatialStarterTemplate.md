# Spatial Starter Template

## Overview
The Spatial Starter Template is the official starter package for creating projects with the Spatial Creator Toolkit. It contains everything you need to get started, including multiple scenes, prefabs, assets, and examples of key Spatial features.

## Features
- **Complete Project Structure**: Includes properly configured scenes, folders, and project settings
- **Sample Assets**: Contains multiple ready-to-use assets and prefabs
- **Example Implementations**: Shows how to use Spatial's core features
- **Cross-Platform Compatibility**: Configured to work across all platforms Spatial supports
- **Documentation**: Includes detailed setup and usage instructions

## Included Components

The Starter Template includes these key components:

### 1. Spatial Island Space
A ready-to-use environment demonstrating proper space setup with:
- Optimized lighting and environment settings
- Properly configured entrance points
- Example interactive elements
- Performance-optimized assets

### 2. Spatian Astronaut Avatar
A complete avatar implementation that shows:
- Proper avatar setup and configuration
- Avatar animation implementation
- Avatar attachments and customization

### 3. 3D Spatial Coin
A collectible item demonstrating:
- Interactive object implementation
- Proper collision detection
- Visual and audio feedback
- Networked object synchronization

### 4. Golf Cart Vehicle
A functional vehicle showing:
- Vehicle controls implementation
- Camera setup for vehicles
- Player entry/exit system
- Multiplayer synchronization

### 5. 1980s Boombox
An interactive object demonstrating:
- Audio playback controls
- User interaction handling
- Visual feedback for interactions
- Network synchronization

### 6. Arguing Animation
A sample avatar animation showing:
- Animation implementation
- Animation triggering
- Blend tree setup
- Avatar compatibility

## Integration with SDK Components
The template integrates with these key SDK components:

| SDK Component | Usage in Template |
|---------------|-------------------|
| SpatialBridge | Central access point for all services |
| IActorService | Used for player management and avatar handling |
| IAvatar | Avatar setup and animation |
| ICameraService | Camera configuration and transitions |
| IInputService | Input handling for interactions |
| ISpaceContentService | Managing space objects and prefabs |
| SpatialNetworkObject | Networked object synchronization |
| SpatialAvatarAttachment | Avatar attachments implementation |

## When to Use
Use this template when:
- Starting a new Spatial project from scratch
- Learning how to implement Spatial features
- Needing a reference for proper implementation patterns
- Creating demos or prototypes quickly

## Getting Started

### Prerequisites
- Unity 2022.3 LTS or later
- Spatial Creator Toolkit installed

### Installation Steps
1. Download the template from [GitHub](https://github.com/spatialsys/spatial-unity-starter-template)
2. Create a new Unity project or use an existing one
3. Import the template package
4. Open the main scene in `/Scenes/SpatialIsland.unity`
5. Configure your Spatial settings in the Project Settings window
6. Update the package manifest as needed for your project type

## Best Practices
- **Study the structure**: Take time to understand how the template is organized
- **Use the provided prefabs**: Leverage the included prefabs as starting points
- **Follow the patterns**: Use the template's implementation patterns for consistency
- **Start small**: Begin with simple modifications before significant changes
- **Backup regularly**: Create backups as you modify the template

## Code Examples

### Accessing the Golf Cart Vehicle
```csharp
// Example of finding and entering the golf cart
public class GolfCartInteraction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to a local player
        if (other.gameObject.TryGetComponent<SpatialAvatar>(out var avatar) && 
            avatar.actorId == SpatialBridge.actorService.localActor.actorNumber)
        {
            // Enter the vehicle
            GetComponent<SpatialVehicle>().EnterVehicle();
        }
    }
}
```

### Playing the Boombox
```csharp
// Example of interacting with the boombox
public class BoomboxController : MonoBehaviour, IVariablesChanged
{
    [SpatialNetworkVariable]
    public NetworkVariable<bool> isPlaying = new NetworkVariable<bool>(false);
    
    private AudioSource audioSource;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    public void TogglePlay()
    {
        if (SpatialBridge.spaceContentService.TryClaimOwnership(this))
        {
            isPlaying.Value = !isPlaying.Value;
        }
    }
    
    public void OnVariablesChanged(NetworkObjectVariablesChangedEventArgs args)
    {
        if (args.changedVariables.Contains("isPlaying"))
        {
            if (isPlaying.Value)
                audioSource.Play();
            else
                audioSource.Pause();
        }
    }
}
```

## Related Templates
- [Camera Modes](../Camera/CameraModes.md) - For advanced camera implementation
- [Avatar Input Control](../Technical/AvatarInputControl.md) - For custom avatar control
- [Simple Car Controller](../Vehicles/SimpleCarController.md) - For more advanced vehicle implementation

## Additional Resources
- [GitHub Repository](https://github.com/spatialsys/spatial-unity-starter-template)
- [Spatial Creator Toolkit Documentation](https://toolkit.spatial.io/docs)
- [Spatial Template Gallery](https://toolkit.spatial.io/templates)
