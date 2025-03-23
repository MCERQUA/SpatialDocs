# SpatialMovementMaterial

Category: Scriptable Objects

Class

SpatialMovementMaterial is a scriptable object that defines surface material properties for avatar movement, including physics properties and associated sound and visual effects. This asset allows creators to customize how avatars interact with different surfaces in a Spatial experience.

## Properties

### Physics Properties
| Property | Type | Description |
| --- | --- | --- |
| usePhysicsMaterial | bool | When true, the material will use physics-based friction values. |
| staticFriction | float | The friction used when an avatar is not moving (when usePhysicsMaterial is true). |
| dynamicFriction | float | The friction used when an avatar is moving (when usePhysicsMaterial is true). |
| frictionCombine | PhysicMaterialCombine | Determines how friction is combined between multiple physics materials. |

### Sound Effects
| Property | Type | Description |
| --- | --- | --- |
| syncSFX | bool | Whether sound effects should be synchronized across the network. |
| syncVolume | float | Volume of synchronized sound effects. |
| stepSound | SpatialSFX | Sound played when an avatar takes a step on this material. |
| runStepSound | SpatialSFX | Sound played when an avatar is running on this material. |
| walkStepSound | SpatialSFX | Sound played when an avatar is walking on this material. |
| stopSound | SpatialSFX | Sound played when an avatar stops moving on this material. |
| jumpSound | SpatialSFX | Sound played when an avatar jumps from this material. |
| landSound | SpatialSFX | Sound played when an avatar lands on this material. |
| takeoffSound | SpatialSFX | Sound played when an avatar takes off from this material. |

### Visual Effects
| Property | Type | Description |
| --- | --- | --- |
| syncVFX | bool | Whether visual effects should be synchronized across the network. |
| stepVFX | GameObject | Visual effect prefab spawned when an avatar takes a step on this material. |
| runStepVFX | GameObject | Visual effect prefab spawned when an avatar is running on this material. |
| walkStepVFX | GameObject | Visual effect prefab spawned when an avatar is walking on this material. |
| stopVFX | GameObject | Visual effect prefab spawned when an avatar stops moving on this material. |
| jumpVFX | GameObject | Visual effect prefab spawned when an avatar jumps from this material. |
| landVFX | GameObject | Visual effect prefab spawned when an avatar lands on this material. |
| takeoffVFX | GameObject | Visual effect prefab spawned when an avatar takes off from this material. |

### Configuration
| Property | Type | Description |
| --- | --- | --- |
| splitRunAndWalk | bool | When true, uses separate effects for running and walking. When false, uses the same effects for both. |
| limitSyncDistance | bool | When true, limits how far sound and visual effects can be seen from. |
| maxSyncDistance | float | Maximum distance that synchronized effects can be seen from when limitSyncDistance is true. |

### Inherited from SpatialScriptableObjectBase
| Property | Type | Description |
| --- | --- | --- |
| documentationURL | string | URL to external documentation or resources for this scriptable object. |
| isExperimental | bool | Indicates whether this scriptable object is experimental and may be subject to changes. |
| prettyName | string | A user-friendly display name for the scriptable object. |
| tooltip | string | Descriptive text that appears when hovering over the scriptable object in the editor. |

## Usage Examples

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class MaterialSurfaceManager : MonoBehaviour 
{
    [SerializeField] private SpatialMovementMaterial concreteMaterial;
    [SerializeField] private SpatialMovementMaterial grassMaterial;
    [SerializeField] private SpatialMovementMaterial iceMaterial;
    
    // Use this to assign materials to SpatialMovementMaterialSurface components
    public void SetSurfaceMaterial(SpatialMovementMaterialSurface surface, SurfaceType type)
    {
        switch(type)
        {
            case SurfaceType.Concrete:
                surface.movementMaterial = concreteMaterial;
                break;
            case SurfaceType.Grass:
                surface.movementMaterial = grassMaterial;
                break;
            case SurfaceType.Ice:
                surface.movementMaterial = iceMaterial;
                break;
        }
    }
    
    // Check material properties for gameplay mechanics
    public bool IsSurfaceSlippery(SpatialMovementMaterialSurface surface)
    {
        if (surface.movementMaterial != null)
        {
            return surface.movementMaterial.dynamicFriction < 0.2f;
        }
        return false;
    }
}

public enum SurfaceType
{
    Concrete,
    Grass,
    Ice
}
```

## Best Practices

1. Create a library of movement materials for different surface types in your experience (concrete, grass, wood, metal, etc.).
2. Adjust friction values to create realistic movement differences between surfaces.
3. Use appropriate sound effects that match the physical material being represented.
4. Balance visual effects to enhance gameplay without overwhelming the scene.
5. Consider performance implications when using complex VFX or many different materials.
6. When using splitRunAndWalk, ensure both sets of effects are consistent in style.
7. Test materials with different avatar movement speeds to ensure effects work well in all cases.

## Common Use Cases

1. Creating distinct surface types for different areas of your world (grass, sand, wood, metal, etc.)
2. Enhancing immersion with appropriate footstep sounds and visual effects
3. Creating gameplay mechanics based on surface properties (slippery ice, sticky mud)
4. Building platforming challenges with surfaces that have different movement characteristics
5. Creating environmental storytelling through surface feedback (creaky floorboards, echoing metal walkways)