## Overview
The SpatialPlatform enum defines the different platforms from which users can access and interact with Spatial spaces. This enum allows developers to identify the platform a user is connecting from, enabling platform-specific optimizations, features, or behaviors.

## Properties
- **Unknown**: Represents an unidentified or unsupported platform.
- **Web**: Represents users accessing Spatial through a web browser.
- **Mobile**: Represents users accessing Spatial through a mobile device (iOS or Android).
- **MetaQuest**: Represents users accessing Spatial through a Meta Quest VR headset.

## Usage Example
```csharp
using SpatialSys.UnitySDK;
using UnityEngine;

public class PlatformSpecificBehavior : MonoBehaviour
{
    // Check the platform of the local user
    void Start()
    {
        SpatialPlatform userPlatform = SpatialBridge.actorService.localActor.platform;
        
        switch (userPlatform)
        {
            case SpatialPlatform.Web:
                ConfigureForWebPlatform();
                break;
                
            case SpatialPlatform.Mobile:
                ConfigureForMobilePlatform();
                break;
                
            case SpatialPlatform.MetaQuest:
                ConfigureForVRPlatform();
                break;
                
            case SpatialPlatform.Unknown:
            default:
                ConfigureForDefaultPlatform();
                break;
        }
    }
    
    // Check the platform of other users in the space
    void DisplayUserPlatforms()
    {
        foreach (IActor actor in SpatialBridge.actorService.actors)
        {
            Debug.Log($"User {actor.displayName} is on platform: {actor.platform}");
        }
    }
    
    // Configure UI elements based on platform
    private void ConfigureForMobilePlatform()
    {
        Debug.Log("Configuring for mobile: larger buttons, simplified UI");
        // Adjust UI elements for touch controls
    }
    
    private void ConfigureForWebPlatform()
    {
        Debug.Log("Configuring for web: standard controls");
        // Set up keyboard and mouse interactions
    }
    
    private void ConfigureForVRPlatform()
    {
        Debug.Log("Configuring for VR: gesture controls, spatial UI");
        // Enable VR-specific interaction methods
    }
    
    private void ConfigureForDefaultPlatform()
    {
        Debug.Log("Using default configuration");
        // Fallback settings that work across platforms
    }
}
```

## Best Practices
- Use platform checks to optimize user experience for each device type
- Provide appropriate control schemes based on the user's platform
- Adjust UI elements and interactions to match platform capabilities (touch for mobile, pointer for web, etc.)
- Consider performance optimizations for resource-constrained platforms like mobile
- Implement fallback behaviors for the Unknown platform type
- When designing multiplayer experiences, remember that users may be connecting from different platforms
- Test your space on all supported platforms to ensure a consistent experience

## Common Use Cases
- Adapting control schemes based on the platform (keyboard/mouse for web, touch for mobile, controller for VR)
- Adjusting UI scale and placement for different screen sizes and input methods
- Implementing platform-specific features or optimizations
- Creating analytics to track user platform distribution
- Providing helpful instructions based on the user's platform
- Enabling or disabling features based on platform capabilities
- Debugging platform-specific issues by identifying the user's platform

## Completed: March 10, 2025
