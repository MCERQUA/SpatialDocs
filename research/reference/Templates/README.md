# Spatial Templates Documentation Project

## Overview
This directory contains reference documentation for the various templates available in the Spatial Creator Toolkit. Each template is documented in a separate markdown file with comprehensive information about its features, use cases, and implementation details.

## Templates Documentation Status

### Current Status (Updated 2025-03-11)
- Templates identified: 35
- Templates documented: 13
- Overall completion: 37.1%

## Documentation Structure
Each template is documented with the following information:
- Basic description and overview
- Features and functionality
- Included components
- Use cases and when to use
- Integration with SDK components
- Sample code examples
- Best practices and tips
- Links to the original source/GitHub repository

## Categories of Templates

### Starter Templates
- [x] [Spatial Starter Template](./Starter/SpatialStarterTemplate.md) - COMPLETED! (3/10/2025)

### Game Templates
- [x] [Obby (Obstacle Course) Game](./Games/ObbyGame.md) - COMPLETED! (3/10/2025)
- [x] [Top Down Shooter](./Games/TopDownShooter.md) - COMPLETED! (3/11/2025)
- [x] [Gem Collection Game](./Games/GemCollectionGame.md) - COMPLETED! (3/11/2025)
- [x] [Run Master Game](./Games/RunMasterGame.md) - COMPLETED! (3/11/2025)
- [ ] [Basketball Game](./Games/BasketballGame.md)
- [ ] [Hyper Jump Game](./Games/HyperJumpGame.md)

### Camera & Visual Templates
- [x] [Camera Modes](./Camera/CameraModes.md) - COMPLETED! (3/10/2025)
- [ ] [Cinemachine](./Camera/Cinemachine.md)
- [ ] [Lighting Examples](./Visual/LightingExamples.md)
- [ ] [Character Shadows](./Visual/CharacterShadows.md)

### Vehicle Templates
- [x] [Simple Car Controller](./Vehicles/SimpleCarController.md) - COMPLETED! (3/10/2025)
- [x] [Golf Course Driving](./Vehicles/GolfCourseDriving.md) - COMPLETED! (3/11/2025)

### Technical Templates
- [x] [Avatar Input Control](./Technical/AvatarInputControl.md) - COMPLETED! (3/11/2025)
- [x] [GPU Particles](./Technical/GPUParticles.md) - COMPLETED! (3/11/2025)
- [x] [HTTP Request (Web Request) Demo](./Technical/HTTPRequestDemo.md) - COMPLETED! (3/11/2025)
- [ ] [WebGL Instancer](./Technical/WebGLInstancer.md)
- [ ] [Addressables](./Technical/Addressables.md)
- [ ] [Embedded Package](./Technical/EmbeddedPackage.md)

### Multiplayer Templates
- [x] [Matchmaking (Lobby System)](./Multiplayer/Matchmaking.md) - COMPLETED! (3/11/2025)

### User Experience Templates
- [x] [Daily/Weekly Rewards (Streaks)](./UX/DailyRewards.md) - COMPLETED! (3/11/2025)

### Avatar Templates
- [ ] [Space Gun Avatar Attachment](./Avatar/SpaceGunAvatarAttachment.md)

### Environment Templates
- [ ] [Abstract](./Environments/Abstract.md)
- [ ] [Agora](./Environments/Agora.md)
- [ ] [Auditorium](./Environments/Auditorium.md)
- [ ] [Boardroom](./Environments/Boardroom.md)
- [ ] [Classroom](./Environments/Classroom.md)
- [ ] [House Contemporary](./Environments/HouseContemporary.md)
- [ ] [Isle Gallery](./Environments/IsleGallery.md)
- [ ] [Mountain Lounge](./Environments/MountainLounge.md)
- [ ] [Obsidian Gallery](./Environments/ObsidianGallery.md)
- [ ] [Outdoors](./Environments/Outdoors.md)
- [ ] [Spatial Home](./Environments/SpatialHome.md)
- [ ] [Spatial Park](./Environments/SpatialPark.md)
- [ ] [Day + Night Environment (Haven Stage)](./Environments/HavenStage.md)

## Documentation Process
1. Research the template on the Spatial Creator Toolkit website
2. Document the template's purpose, components, and features
3. Create code examples and integration guides
4. Link to related SDK components documented in our main project
5. Update this README with progress information

## Repository Organization
```
research/reference/Templates/
├── README.md                    # This file - project overview
├── Starter/                     # Starter templates
│   ├── SpatialStarterTemplate.md
├── Games/                       # Game templates
│   ├── ObbyGame.md
│   ├── TopDownShooter.md
│   └── ...
├── Camera/                      # Camera-related templates
│   ├── CameraModes.md
│   └── ...
├── Visual/                      # Visual effects templates
│   ├── LightingExamples.md
│   └── ...
├── Vehicles/                    # Vehicle templates
│   ├── SimpleCarController.md
│   └── GolfCourseDriving.md
├── Technical/                   # Technical templates
│   ├── AvatarInputControl.md
│   ├── GPUParticles.md
│   └── ...
├── Multiplayer/                 # Multiplayer templates
│   ├── Matchmaking.md
│   └── ...
├── UX/                          # User experience templates
│   ├── DailyRewards.md
│   └── ...
├── Avatar/                      # Avatar templates
│   ├── SpaceGunAvatarAttachment.md
│   └── ...
└── Environments/                # Environment templates
    ├── Abstract.md
    └── ...
```

## Documentation Completed
1. [Spatial Starter Template](./Starter/SpatialStarterTemplate.md) - 3/10/2025
   - Documented complete project structure with included assets
   - Detailed integration with key SDK components
   - Provided code examples for vehicle and object interaction
   - Added best practices for template customization

2. [Camera Modes](./Camera/CameraModes.md) - 3/10/2025
   - Documented all seven camera perspectives
   - Detailed implementation in both C# and Visual Scripting
   - Provided code examples for camera mode switching
   - Explained integration with Spatial's camera system

3. [Obby (Obstacle Course) Game](./Games/ObbyGame.md) - 3/10/2025
   - Documented modular obstacle system
   - Detailed checkpoint and respawn systems
   - Provided code examples for obstacles and moving platforms
   - Explained integration with Spatial's trigger events

4. [Simple Car Controller](./Vehicles/SimpleCarController.md) - 3/10/2025
   - Documented third-party vehicle integration
   - Detailed vehicle input handling and camera system
   - Provided code examples for vehicle control and entry/exit
   - Explained multiplayer synchronization

5. [Top Down Shooter](./Games/TopDownShooter.md) - 3/11/2025
   - Documented automatic targeting system
   - Detailed enemy AI implementation
   - Provided code examples for weapon system and damage calculations
   - Explained visual feedback system for combat events

6. [Avatar Input Control](./Technical/AvatarInputControl.md) - 3/11/2025
   - Documented custom input handling system
   - Detailed implementation for desktop and mobile controls
   - Provided code examples for input management and avatar control
   - Explained platform-specific considerations and best practices

7. [GPU Particles](./Technical/GPUParticles.md) - 3/11/2025
   - Documented compute shader-based particle system
   - Detailed implementations for high-performance particle rendering
   - Provided code examples for GPU particle simulation and effect triggers
   - Explained performance optimization techniques and WebGL considerations

8. [Daily/Weekly Rewards (Streaks)](./UX/DailyRewards.md) - 3/11/2025
   - Documented comprehensive user engagement streak system
   - Detailed time-based reward mechanics and persistence system
   - Provided code examples for streak tracking and reward distribution
   - Explained server-side validation and anti-cheat measures
   - Created premium reward track implementation
   - Documentation split across multiple files due to size:
     - [Part 1: Main Implementation](./UX/DailyRewards.md)
     - [Part 2: Weekly Calendar View](./UX/DailyRewards-Part2.md)
     - [Part 3: Premium Track & Best Practices](./UX/DailyRewards-Part3.md)

9. [Gem Collection Game](./Games/GemCollectionGame.md) - 3/11/2025
   - Documented complete gem collection game mechanics
   - Detailed gem spawning and collection systems
   - Provided code examples for collectible implementation and scoring
   - Explained persistence options and progress tracking
   - Included time challenge mode implementation
   - Described visual and audio feedback systems

10. [Matchmaking (Lobby System)](./Multiplayer/Matchmaking.md) - 3/11/2025
    - Documented complete multiplayer lobby and matchmaking system
    - Detailed room creation, discovery, and management
    - Provided code examples for lobby manager, room browser, and team management
    - Explained player synchronization and match coordination
    - Included team assignment and balancing systems
    - Described host migration and network state synchronization

11. [HTTP Request (Web Request) Demo](./Technical/HTTPRequestDemo.md) - 3/11/2025
    - Documented comprehensive HTTP request system for API integrations
    - Detailed request management with queueing, caching, and retry mechanisms
    - Provided code examples for request manager, API service layer, and UI display
    - Explained authentication, error handling, and data parsing strategies
    - Included examples for different HTTP methods (GET, POST, PUT, DELETE)
    - Described best practices for security, performance, and error management

12. [Run Master Game](./Games/RunMasterGame.md) - 3/11/2025
    - Documented complete endless runner game implementation 
    - Detailed procedural level generation and object pooling system
    - Provided code examples for runner control, obstacles, and collectibles
    - Explained game management and difficulty progression
    - Included mobile optimization techniques and touch controls
    - Created multi-part documentation due to comprehensive nature:
      - [Part 1: Core Systems](./Games/RunMasterGame.md)
      - [Part 2: Level Generation](./Games/RunMasterGame-Part2.md)
      - [Part 3: Advanced Features](./Games/RunMasterGame-Part3.md)

13. [Golf Course Driving](./Vehicles/GolfCourseDriving.md) - 3/11/2025
    - Documented complete golf cart physics and driving system
    - Detailed golf course environment implementation with terrain variations
    - Provided code examples for vehicle control, cart path following, and time of day system
    - Explained multiplayer synchronization for multiple golf carts
    - Included passenger system implementation with driver/passenger roles
    - Described environmental effects and surface-specific interactions
    - Added integration with golf gameplay mechanics

## Documentation In Progress
- None currently in progress

## Guidelines for Documentation
- Focus on practical applications of each template
- Include notes about integration with Spatial SDK components
- Add code snippets where appropriate
- Explain benefits and limitations
- Document best practices for customization
- Maintain consistent formatting across all template documentation

## Related Resources
- [Spatial Creator Toolkit Templates](https://toolkit.spatial.io/templates)
- [Spatial Creator Toolkit Documentation](https://toolkit.spatial.io/docs)
- [Spatial SDK Component Documentation](../../README.md)
