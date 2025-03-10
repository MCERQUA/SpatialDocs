# SpatialSFX

Category: Scriptable Objects

Class

SpatialSFX is a scriptable object that defines a sound effect or collection of sound effects, along with playback settings. It provides methods for playing sounds at specific positions or through AudioSource components.

## Properties

### Audio Clips and Mixer
| Property | Type | Description |
| --- | --- | --- |
| clips | AudioClip[] | Array of audio clips. When played, a random clip from this array will be selected. |
| mixerGroup | AudioMixerGroup | Optional audio mixer group for routing and processing. |

### Volume and Pitch Settings
| Property | Type | Description |
| --- | --- | --- |
| volume | Vector2 | Volume range (x = min, y = max). A random value in this range will be used. |
| pitch | Vector2 | Pitch range (x = min, y = max). A random value in this range will be used. |

### Spatial Audio Settings
| Property | Type | Description |
| --- | --- | --- |
| spatialBlend | float | Blend between 2D (0) and 3D (1) sound. Values between create a mix of both. |
| rolloff | AudioRolloffMode | Distance attenuation curve type (Logarithmic, Linear, or Custom). |
| rollOffMin | float | Minimum distance before volume starts to attenuate. |
| rollOffMax | float | Maximum distance at which the sound becomes completely inaudible. |
| reverbMix | float | How much the sound is affected by reverb zones. |

### Inherited from SpatialScriptableObjectBase
| Property | Type | Description |
| --- | --- | --- |
| documentationURL | string | URL to external documentation or resources for this scriptable object. |
| isExperimental | bool | Indicates whether this scriptable object is experimental and may be subject to changes. |
| prettyName | string | A user-friendly display name for the scriptable object. |
| tooltip | string | Descriptive text that appears when hovering over the scriptable object in the editor. |

## Methods

| Method | Description |
| --- | --- |
| Play(Vector3 position) | Plays the sound effect at the specified world position. |
| Play(Vector3 position, float extraVolume, float extraPitch) | Plays the sound effect at the specified world position with additional volume and pitch adjustments. |
| Play(AudioSource source) | Plays the sound effect through the specified AudioSource component. |
| Play(AudioSource source, float extraVolume, float extraPitch) | Plays the sound effect through the specified AudioSource component with additional volume and pitch adjustments. |
| GetRandomClip() | Returns a random AudioClip from the clips array. |
| GetRandomVolume() | Returns a random volume value within the configured range. |
| GetRandomPitch() | Returns a random pitch value within the configured range. |

## Usage Examples

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private SpatialSFX footstepSound;
    [SerializeField] private SpatialSFX jumpSound;
    [SerializeField] private SpatialSFX ambientSound;
    [SerializeField] private AudioSource audioSource;
    
    private void Start()
    {
        // Play ambient sound through the AudioSource
        if (ambientSound != null)
        {
            ambientSound.Play(audioSource);
        }
    }
    
    public void PlayFootstep(Vector3 position)
    {
        // Play footstep sound at the specified position
        if (footstepSound != null)
        {
            footstepSound.Play(position);
        }
    }
    
    public void PlayJump(Vector3 position, float jumpForce)
    {
        // Play jump sound with pitch variation based on jump force
        if (jumpSound != null)
        {
            // Extra pitch increases with jump force (normalized to a reasonable range)
            float extraPitch = (jumpForce / 10f) - 0.5f; 
            jumpSound.Play(position, 0f, extraPitch);
        }
    }
    
    // Example of creating a custom sound player with variation
    public void PlayCustomSound(SpatialSFX sound, Vector3 position, float intensity)
    {
        if (sound != null)
        {
            // Volume increases with intensity
            float extraVolume = Mathf.Clamp(intensity - 1f, -0.5f, 0.5f);
            
            // Pitch decreases with intensity
            float extraPitch = Mathf.Clamp(1f - intensity, -0.3f, 0.3f);
            
            sound.Play(position, extraVolume, extraPitch);
        }
    }
}
```

## Best Practices

1. Include multiple similar audio clips in the clips array to add natural variation.
2. Set appropriate volume and pitch ranges to create dynamic sound effects.
3. Configure spatial audio settings (spatialBlend, rollOffMin, rollOffMax) based on the expected use of the sound.
4. Use audioMixerGroup to organize sounds into categories (e.g., SFX, Music, UI).
5. For frequently played sounds, consider performance optimizations like object pooling.
6. Keep volume levels consistent across different SpatialSFX assets to maintain audio balance.
7. Test sounds in different environments to ensure they sound good with various reverb settings.

## Common Use Cases

1. Character movement sounds (footsteps, jumping, landing)
2. Environmental effects (wind, water, fire)
3. Interactive object sounds (buttons, doors, mechanisms)
4. Feedback for player actions (collecting items, using abilities)
5. Ambient sound effects for creating atmosphere
6. UI sound effects with consistent styling
7. Creating layered sound systems with multiple SpatialSFX assets playing together