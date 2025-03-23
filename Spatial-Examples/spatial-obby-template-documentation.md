# Spatial Obby Template

## Template Overview
The Spatial Obby Template provides a complete framework for creating "obby" (obstacle course) games within Spatial. It features a node-based level design system, various platform types with different effects, moving platforms, checkpoints, and progress tracking. The template includes save/load functionality, a camera system, and tools for creating diverse obstacle challenges.

## Template Information
- Location: `E:\1-ECHO-WORKING-FOLDER\Spatial-Templates\spatial-obby-template`
- Repository URL: Inferred from folder name: https://github.com/spatialsys/spatial-obby-template
- Key Features:
  - Node-based obstacle course design
  - Multiple platform types with different effects (kill, force, trampoline, speed)
  - Waypoint carousel system for moving platforms
  - Progress tracking and save/load functionality
  - Checkpoint system with respawn functionality
  - Multiple courses with optional course hopping

## Directory Structure
```
E:\1-ECHO-WORKING-FOLDER\Spatial-Templates\spatial-obby-template\
├── README.md
└── ObbyUnityProject\
    ├── Assets\
    │   ├── Art\
    │   ├── ArtAssets\
    │   ├── Editor\
    │   ├── Examples\
    │   ├── ObbyEngine\
    │   │   ├── GONotation.cs
    │   │   ├── ObbyCourse.cs
    │   │   ├── ObbyGameManager.cs
    │   │   ├── ObbyNode.cs
    │   │   ├── ObbyNodeTarget.cs
    │   │   ├── ObbyPlatform.cs
    │   │   ├── ObbyPlatformCollisionManager.cs
    │   │   ├── ObbyProgressUI.cs
    │   │   ├── ObbySettings.cs
    │   │   ├── ObbySmoothCamera.cs
    │   │   ├── ObbyStartCourseInteractable.cs
    │   │   ├── ObbyWaypointCarousel.cs
    │   │   ├── ObbyZone.cs
    │   │   ├── Rotate.cs
    │   │   └── SpatialObby.asmdef
    │   ├── ObbyTemplate\
    │   ├── Spatial\
    │   └── TextMesh Pro\
    ├── Exports\
    ├── Packages\
    └── ProjectSettings\
```

## C# Scripts
### Core System Scripts

#### ObbyGameManager.cs
```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SpatialSys.UnitySDK;
using UnityEngine;

public class ObbyGameManager : MonoBehaviour
{
    private const string SAVED_PROGRESS_KEY = "OBBY_GAME_SAVED_PROGRESS";
    public static ObbyGameManager instance;

    //* Inspector
    public ObbyCourse defaultCourse;
    [Tooltip("If a player enters a different course, should we treat this new course as the new active course? If false, the player will be teleported back to the last node they were on in the previous course.")]
    public bool allowCourseHopping = true;
    [Tooltip("When the user joins, should we teleport them to a node? If they have played before, they will be teleported to the last node they were on.")]
    public bool teleportPlayerToNodeOnStart = true;
    public ParticleSystem newNodeParticles;

    //* Properties
    public ObbyCourse currentCourse { get; private set; } = null;
    public int currentCourseNodeIndex { get; private set; } = -1;

    public Dictionary<string, int> saveFile { get; private set; } = new Dictionary<string, int>();

    // Implementation details omitted for brevity
}
```

**Purpose**: Central manager for the obby game system. Handles course tracking, checkpoints, saving/loading progress, and player death/respawn.

**Spatial SDK Usage**: 
- Uses `SpatialBridge.userWorldDataStoreService` for saving/loading progress
- Uses `SpatialBridge.actorService.localActor.avatar` for teleporting and manipulating the player
- Uses `SpatialBridge.inputService` for handling input during player death
- Uses `SpatialBridge.cameraService` for camera effects like shake and wobble
- Uses `SpatialBridge.coreGUIService` for displaying messages

**Key Features**:
- Manages current course and node tracking
- Loads and saves player progress using UserWorldDataStoreService
- Handles teleporting player to appropriate nodes
- Manages player death and respawn
- Supports multiple courses and optional course hopping

#### ObbyPlatform.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SpatialSys.UnitySDK;

public enum ObbyPlatformActorEffect
{
    None,
    Kill,
    Force,
    Trampoline,
}

public enum ForceSpace
{
    Local,
    World,
}

[RequireComponent(typeof(Collider))]
[DefaultExecutionOrder(50)]
public class ObbyPlatform : MonoBehaviour
{
    private static int PLAYER_LAYER = 30;

    // Settings
    public SpatialMovementMaterial movementMaterial;

    //Affectors
    public ObbyPlatformActorEffect actorEffect = ObbyPlatformActorEffect.None;
    public ForceSpace forceSpace = ForceSpace.Local;
    public Vector3 force = Vector3.zero;
    public float trampolineHeight = 0;

    public UnityEvent OnPlayerEnter;
    public UnityEvent OnPlayerExit;
    public UnityEvent OnPlayerStay;

    // Implementation details omitted for brevity
}
```

**Purpose**: Defines a platform that players can interact with. Platforms can have different effects like killing the player, applying force, or functioning as a trampoline.

**Spatial SDK Usage**: 
- Uses `SpatialMovementMaterial` for platform movement properties
- Uses `SpatialMovementMaterialSurface` for surface properties
- Uses `SpatialBridge.actorService.localActor.avatar` for applying effects to the player

**Key Features**:
- Multiple platform effects (None, Kill, Force, Trampoline)
- Events for player enter, exit, and stay
- Support for different force spaces (local or world)
- Automatic application of movement material to colliders

#### ObbyZone.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SpatialSys.UnitySDK;
using System.Security.Cryptography;

public enum ObbyZoneActorEffect
{
    None,
    Kill,
    Force,
    SpeedMultiplier,
}

[RequireComponent(typeof(Collider))]
[DefaultExecutionOrder(50)]
public class ObbyZone : MonoBehaviour
{
    public const float MIN_SPEED_MULTIPLIER = 0.1f;
    private static int PLAYER_LAYER = 30;

    //Affectors
    public ObbyZoneActorEffect actorEffect = ObbyZoneActorEffect.None;
    public ForceSpace forceSpace = ForceSpace.Local;
    public Vector3 force = Vector3.zero;
    public float speedMultiplier;
    public UnityEvent OnPlayerEnter;
    public UnityEvent OnPlayerExit;
    public UnityEvent OnPlayerStay;

    // Implementation details omitted for brevity
}
```

**Purpose**: Defines a trigger zone that applies effects to players when they enter. Unlike platforms, zones don't provide collision.

**Spatial SDK Usage**: 
- Uses `SpatialBridge.actorService.localActor.avatar` for applying effects to the player

**Key Features**:
- Multiple zone effects (None, Kill, Force, SpeedMultiplier)
- Events for player enter, exit, and stay
- Support for different force spaces (local or world)
- Speed multiplier effect that can speed up or slow down player movement

### Course and Node Scripts

#### ObbyCourse.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages an obby course
/// </summary>
public class ObbyCourse : MonoBehaviour
{
    public string courseID;

    [SerializeField] private ObbyNode[] _nodes = new ObbyNode[0];
    public ObbyNode[] nodes {
        get {
            _nodes = _nodes.Where(n => n != null).ToArray();
            return _nodes;
        }
        set { _nodes = value; }
    }

    // Implementation details omitted for brevity
}
```

**Purpose**: Defines a complete obstacle course made up of a series of nodes.

**Key Features**:
- Manages an array of ObbyNodes that make up the course
- Provides methods to find nodes by index or get indices of nodes

#### ObbyNode.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SpatialSys.UnitySDK;
using UnityEngine;

public class ObbyNode : MonoBehaviour
{
    public ObbyPlatform nodePlatform;
    public ObbyNodeTarget target;

    // Implementation details omitted for brevity
}
```

**Purpose**: Represents a single checkpoint in a course. Contains a platform and a target that determines the position and orientation of the next node.

**Key Features**:
- Links to an ObbyPlatform for checkpoint functionality
- Contains a target object that determines the position of the next node
- Notifies the game manager when the player reaches this node

### Platform Movement Scripts

#### ObbyWaypointCarousel.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum WaypointLoopType {
    PingPong,
    Loop
}

public enum WaypointSpacingType {
    Max,
    Custom
}

public class ObbyWaypointCarousel : MonoBehaviour
{
    public List<Transform> _waypoints;
    public List<Transform> _platforms;
    public float speed;
    public WaypointLoopType loopType;
    public bool pauseAtEnds;
    public float pauseTime;
    public WaypointSpacingType spacingType;
    public float customSpacing;

    // Implementation details omitted for brevity
}
```

**Purpose**: Moves platforms along a path defined by waypoints. Supports looping or ping-pong movement patterns.

**Key Features**:
- Moves any number of platforms along a waypoint path
- Supports loop or ping-pong movement patterns
- Optional pause at the ends of the path
- Configurable speed and spacing between platforms
- Handles both maximum and custom spacing options

## Key System Features

### 1. Node-Based Course Design
- Courses are composed of a series of nodes
- Each node has a platform and a target position for the next node
- Nodes serve as checkpoints for progress tracking
- Multiple courses can exist in a single space

### 2. Platform Effects System
- Platforms can have different effects:
  - **None**: Standard platform with no special effects
  - **Kill**: Kills the player on contact, causing them to respawn
  - **Force**: Applies a configurable force to the player
  - **Trampoline**: Bounces the player into the air

### 3. Trigger Zone System
- Trigger zones apply effects without providing collision:
  - **None**: Trigger with no special effects
  - **Kill**: Kills the player when they enter the zone
  - **Force**: Applies a configurable force to the player
  - **SpeedMultiplier**: Changes player movement speed while in the zone

### 4. Moving Platform System
- Waypoint Carousel system for creating moving platforms
- Platforms can follow a looping or ping-pong path
- Configurable speed, spacing, and pause options
- Multiple platforms can move on the same path

### 5. Progress Tracking and Save System
- Automatically saves player progress using Spatial's UserWorldDataStoreService
- Players can continue from their last checkpoint when rejoining
- Support for multiple courses with separate progress tracking
- Optional course hopping restriction

### 6. Camera and Visual Feedback
- Smooth camera system that follows the player
- Camera effects for player death (freeze, shake, wobble)
- Particle effects for reaching new nodes and player death
- UI for showing current progress

## Integration with Spatial SDK
The template demonstrates several Spatial SDK features:

1. **Avatar Manipulation**
   - Teleporting the player with `SetPositionRotation`
   - Adding forces to the avatar
   - Controlling avatar jumping and movement

2. **Data Persistence**
   - Using `UserWorldDataStoreService` to save and load player progress
   - Identifying players across sessions

3. **Input System**
   - Temporarily disabling player input during death
   - Implementing custom input listeners

4. **Camera Controls**
   - Camera shaking and wobbling effects
   - Custom camera targeting and following

5. **Movement Materials**
   - Using `SpatialMovementMaterial` for platform surfaces
   - Configuring how players interact with different surfaces

## How to Use This Template

### Creating a New Course
1. Create a new empty GameObject and add the `ObbyCourse` component
2. Set a unique `courseID` for the course
3. Add the course to the scene by dragging it into the scene hierarchy
4. Create nodes by duplicating the `EmptyObbyNode` prefab
5. Add nodes to the course by dragging them into the `Nodes` list of the `ObbyCourse`
6. Position the `Target` GameObject of each node to determine where the next node should start

### Creating Custom Platforms
1. Add the `ObbyPlatform` component to any GameObject with a Collider
2. Choose an actor effect (None, Kill, Force, Trampoline)
3. Configure effect parameters as needed
4. Optionally add a movement material for custom friction/bounce properties

### Creating Trigger Zones
1. Create a GameObject with a Collider component
2. Add the `ObbyZone` component
3. The collider will automatically be set to trigger mode
4. Configure the zone effect (None, Kill, Force, SpeedMultiplier)
5. Set up the effect parameters as needed

### Creating Moving Platforms
1. Create a GameObject and add the `ObbyWaypointCarousel` component
2. Add transforms to the `Waypoints` list to define the path
3. Add platforms to the `Platforms` list
4. Configure loop type, speed, and spacing options
5. Adjust pause settings if using ping-pong movement

## Source Verification
- Documentation created on: March 22, 2025
- Source files last accessed: March 22, 2025
- Documentation matches source: ✓
