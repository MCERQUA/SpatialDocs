# Spatial Templates Documentation Project

## Overview
This directory contains reference documentation for the various templates available in the Spatial Creator Toolkit. Each template is documented in a separate markdown file with comprehensive information about its features, use cases, and implementation details.

## Templates Documentation Status

### Current Status (Updated 2025-03-11)
- Templates identified: 35
- Templates documented: 5
- Overall completion: 14.3%

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
- [ ] [Gem Collection Game](./Games/GemCollectionGame.md)
- [ ] [Run Master Game](./Games/RunMasterGame.md)
- [ ] [Basketball Game](./Games/BasketballGame.md)
- [ ] [Hyper Jump Game](./Games/HyperJumpGame.md)

### Camera & Visual Templates
- [x] [Camera Modes](./Camera/CameraModes.md) - COMPLETED! (3/10/2025)
- [ ] [Cinemachine](./Camera/Cinemachine.md)
- [ ] [Lighting Examples](./Visual/LightingExamples.md)
- [ ] [Character Shadows](./Visual/CharacterShadows.md)

### Vehicle Templates
- [x] [Simple Car Controller](./Vehicles/SimpleCarController.md) - COMPLETED! (3/10/2025)
- [ ] [Golf Course Driving](./Vehicles/GolfCourseDriving.md)

### Technical Templates
- [ ] [Avatar Input Control](./Technical/AvatarInputControl.md)
- [ ] [WebGL Instancer](./Technical/WebGLInstancer.md)
- [ ] [GPU Particles](./Technical/GPUParticles.md)
- [ ] [HTTP Request (Web Request) Demo](./Technical/HTTPRequestDemo.md)
- [ ] [Addressables](./Technical/Addressables.md)
- [ ] [Embedded Package](./Technical/EmbeddedPackage.md)

### Multiplayer Templates
- [ ] [Matchmaking (Lobby System)](./Multiplayer/Matchmaking.md)

### User Experience Templates
- [ ] [Daily/Weekly Rewards (Streaks)](./UX/DailyRewards.md)

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
│   └── ...
├── Technical/                   # Technical templates
│   ├── AvatarInputControl.md
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

## Documentation In Progress
- Avatar Input Control (Technical) - Started 3/11/2025

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
