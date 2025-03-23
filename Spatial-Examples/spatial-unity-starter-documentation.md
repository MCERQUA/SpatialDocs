# Spatial Unity Starter Template

## Template Overview
The Spatial Unity Starter Template is a basic template that provides the essential foundation for creating Spatial experiences in Unity. It includes the core project structure, essential prefabs, and basic implementation examples for Spatial SDK integration. This template serves as the starting point for developers creating custom Spatial experiences.

## Template Information
- Location: `spatial-unity-starter-template-main` directory
- Purpose: Basic starter template for Spatial SDK integration
- Dependencies: Spatial Unity SDK (Creator Toolkit)

## Directory Structure
```
spatial-unity-starter-template-main/
├── Assets/
│   ├── Scenes/
│   │   └── MainScene.unity
│   ├── Scripts/
│   │   ├── SpatialInitializer.cs
│   │   └── BasicInteraction.cs
│   ├── Prefabs/
│   │   ├── BasicInteractable.prefab
│   │   └── SpawnPoint.prefab
│   └── SpatialSDK/
│       └── [SDK Files]
├── ProjectSettings/
└── Packages/
    └── manifest.json
```

## C# Scripts

### SpatialInitializer.cs
```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class SpatialInitializer : MonoBehaviour
{
    void Start()
    {
        // Subscribe to Spatial events
        SpatialBridge.spaceService.onSpaceLiked += OnSpaceLiked;
        
        // Initial setup
        Debug.Log("Spatial SDK initialized!");
    }

    void OnDestroy()
    {
        // Unsubscribe from Spatial events
        SpatialBridge.spaceService.onSpaceLiked -= OnSpaceLiked;
    }
    
    private void OnSpaceLiked()
    {
        Debug.Log("Space was liked!");
    }
}
```

**Purpose**: Initializes the Spatial SDK and sets up event listeners for core Spatial events.

**Spatial SDK Usage**:
- `SpatialBridge.spaceService` for space-related functionality
- Event subscription: `onSpaceLiked`

**Dependencies**:
- None (core initialization script)

### BasicInteraction.cs
```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class BasicInteraction : MonoBehaviour, IUsable
{
    public string interactionPrompt = "Press F to interact";
    
    public bool OnUse()
    {
        // Show floating text as feedback
        SpatialBridge.vfxService.CreateFloatingText("Interacted!", 
            SpatialBridge.actorService.localActor.avatar.position + Vector3.up, 
            FloatingTextStyle.Normal);
        
        return true;
    }
    
    public string GetPrompt()
    {
        return interactionPrompt;
    }
}
```

**Purpose**: Demonstrates basic interaction with objects using the IUsable interface.

**Spatial SDK Usage**:
- `IUsable` interface implementation
- `SpatialBridge.vfxService.CreateFloatingText` for visual feedback
- `SpatialBridge.actorService.localActor.avatar` to access player avatar

**Dependencies**:
- Requires a collider component for interaction detection

## Unity Components and Prefabs

### BasicInteractable.prefab
- Type: Interactable Object
- Location: `Assets/Prefabs/BasicInteractable.prefab`
- Key Properties:
  - BoxCollider (for interaction trigger)
  - BasicInteraction component
  - Mesh Renderer for visual representation
- Usage: Placed in the scene to provide interactive objects

### SpawnPoint.prefab
- Type: Player Spawn Point
- Location: `Assets/Prefabs/SpawnPoint.prefab`
- Key Properties:
  - Transform defines spawn position and orientation
  - SpatialSpawnPoint component (Spatial SDK component)
- Usage: Defines where players spawn in the space

## Spatial SDK Integration

### Core Services Used
1. **Space Service**
   - Accessing space information and events
   - `SpatialBridge.spaceService`

2. **Actor Service**
   - Accessing avatar information
   - `SpatialBridge.actorService.localActor.avatar`

3. **VFX Service**
   - Creating visual feedback
   - `SpatialBridge.vfxService.CreateFloatingText`

### Implementation Examples
1. **Basic Interaction**
   - Implementing the `IUsable` interface
   - Providing interaction prompts
   - Handling user interactions

2. **Avatar Integration**
   - Accessing the local player's avatar
   - Using avatar position for various operations

3. **Space Events**
   - Subscribing to space-related events
   - Handling space interaction events

## Usage Instructions
1. Create a new Unity project
2. Import the Spatial SDK package
3. Use this template as the foundation
4. Build upon the example scripts and prefabs
5. Use the template scene as a starting point for your space

## Source Verification
- Documentation created on: March 22, 2025
- Template verified against example files from Spatial documentation
- Documentation matches source: ✓
