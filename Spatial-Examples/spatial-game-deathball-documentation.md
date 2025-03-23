# Spatial Game Deathball Sample Template

## Template Overview
The Spatial Game Deathball Sample template demonstrates a multiplayer ball game where a bouncing ball targets players and bots. Players can block the ball by pressing a key, and bots have a configurable chance to block. The template showcases Spatial's multiplayer features, including synced objects, actor properties, and networking.

## Template Information
- Location: `E:\1-ECHO-WORKING-FOLDER\Spatial-Templates\spatial-game-deathball`
- Repository URL: Inferred from folder name: https://github.com/spatialsys/spatial-game-deathball
- Key Features:
  - Multiplayer ball game where a ball bounces between players/bots
  - Players can block the ball using a key press
  - Bots are controlled by a simple AI
  - Networking implemented using Spatial's SyncedObject system
  - Supports multiple players and players joining/leaving

## Directory Structure
```
E:\1-ECHO-WORKING-FOLDER\Spatial-Templates\spatial-game-deathball\
├── README.md
└── spatial-deathball-unity\
    ├── Assets\
    │   ├── Art\
    │   ├── New Physic Material.physicMaterial
    │   ├── Prefabs\
    │   ├── Scripts\
    │   │   ├── ActorBlockStateDebug.cs
    │   │   ├── ActorListDebug.cs
    │   │   ├── BallControl.cs
    │   │   ├── BallSyncVariableControl.asset
    │   │   ├── BallVariableSync.cs
    │   │   ├── Bot.cs
    │   │   ├── BotManager.cs
    │   │   ├── LocalPlayerControls.cs
    │   │   ├── MyAssembly.asmdef
    │   │   ├── NetworkLobbyManager.cs
    │   │   ├── PlayerTrigger.cs
    │   │   ├── RPCManager.cs
    │   │   └── SceneINIT.cs
    │   ├── Spatial\
    │   ├── TextMesh Pro\
    │   ├── _DeathBall\
    │   ├── _DeathBall.unity
    │   └── thumb.png
    ├── Packages\
    └── ProjectSettings\
```

## C# Scripts
### Core Game Scripts

#### BallControl.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SpatialSys.UnitySDK;
using UnityEngine;

/// <summary>
/// Keeping it dead simple and not worrying about quality of netcode at all. No owner swapping.
/// 
/// Server host client controls ball. Host is in charge of checking for hits/blocks.
/// </summary>
[RequireComponent(typeof(BallVariableSync)), RequireComponent(typeof(Rigidbody))]
public class BallControl : MonoBehaviour
{
    public static BallControl instance;

    private const int PLAYER_TRIGGER_LAYER = 7;
    private const int BOT_PLAYER_LAYER = 6;

    public float powerPerSecond = .1f;// power resets on HIT
    public float maxPower = 10f;
    public float forceByPower = 10f;// how much force we add to the ball per power
    public float rotationByPower = 1f;// how much we rotate the ball's velocity per power

    public BallVariableSync ballVariables { get; private set; }

    private Rigidbody rb;
    private SpatialSyncedObject syncedObject;

    private Transform previousTarget;
    private Transform target;

    // Implementation details omitted for brevity
}
```

**Purpose**: Controls the ball's movement, targeting, and collision detection. The ball will target a player or bot and move toward them, with increasing power over time.

**Spatial SDK Usage**: 
- Uses `SpatialBridge.actorService.actors` to access all players
- Uses `SpatialBridge.vfxService.CreateFloatingText` for visual feedback
- Uses `SpatialSyncedObject` for networking
- Accesses player avatars through `SpatialBridge.actorService.actors[id].avatar`
- Accesses custom properties through `SpatialBridge.actorService.actors[id].customProperties`

**Key Features**:
- The ball targets players and bots, moving towards them with increasing power
- Checks for collisions with players and bots
- Handles block/hit logic when colliding with targets
- Uses floating text for visual feedback
- Selects new targets after a hit or block

#### Bot.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;
using UnityEngine.AI;

/// <summary>
/// Controls a bot. Should only be actively used by host client.
/// </summary>
[RequireComponent(typeof(SpatialSyncedObject)), RequireComponent(typeof(NavMeshAgent))]
public class Bot : MonoBehaviour
{
    public SpatialSyncedObject syncedObject { get; private set; }
    private NavMeshAgent navMeshAgent;

    public float newPositionEvery = 2f; // how often to move to a new position
    public float blockChance = 0.5f; // how likely out of 1 is the bot to block the ball

    private float newPosTimer;

    // Implementation details omitted for brevity
}
```

**Purpose**: Controls the AI bots that participate in the game alongside players. Bots move around the environment and can block the ball.

**Spatial SDK Usage**: 
- Uses `SpatialSyncedObject` for networking
- Uses `SpatialBridge.vfxService.CreateFloatingText` for visual feedback

**Key Features**:
- NavMesh-based movement for realistic navigation
- Random movement to new positions at configurable intervals
- Configurable chance to block the ball
- Uses floating text for visual feedback when hit

#### BotManager.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    public static BotManager instance;
    public static List<Bot> bots = new List<Bot>();
    public static Dictionary<string,Bot> botDict = new Dictionary<string, Bot>();

    // Implementation details omitted for brevity
}
```

**Purpose**: Manages all the bots in the scene, providing a centralized registry for bots.

**Key Features**:
- Maintains a list of all active bots
- Provides dictionary lookup by instance ID
- Handles bot registration and deregistration

#### LocalPlayerControls.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using SpatialSys.UnitySDK;
using UnityEngine;

public class LocalPlayerControls : MonoBehaviour
{
    void Update()
    {
        //super basic implementation of how ACTORS will block. TLDR they set a custom property bool. 
        //These get auto synced to all clients so host can read them.
        if (Input.GetKeyDown(KeyCode.F))
        {
            SpatialBridge.actorService.localActor.SetCustomProperty("isBlocking", true);
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            SpatialBridge.actorService.localActor.SetCustomProperty("isBlocking", false);
        }
    }
}
```

**Purpose**: Handles player input for blocking the ball.

**Spatial SDK Usage**: 
- Uses `SpatialBridge.actorService.localActor.SetCustomProperty` to set blocking state

**Key Features**:
- Players press and hold 'F' key to block
- Blocking state is synced across the network using custom properties

### Networking Scripts

#### BallVariableSync.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;
using System;

/// <summary>
/// As of writing we don't have proper synced object C# support.
/// They where originally built for Visual Scripting and the Variables component.
/// This script handles passing variable events and changes to the ballControl
/// </summary>
public class BallVariableSync : MonoBehaviour
{
    public delegate void EventListenerDelegate(string eventName, object[] args);
    private EventListenerDelegate _eventListener;

    // Event handlers and implementation details omitted for brevity
}
```

**Purpose**: Handles the synchronization of ball state across the network, bridging between C# and Visual Scripting.

**Spatial SDK Usage**: 
- Uses `VisualScriptingUtility` to interact with Visual Scripting system
- Bridges C# code to Visual Scripting events

**Key Features**:
- Synchronizes ball state including target type, target ID, and power
- Provides events for when values change
- Connects C# scripts to Visual Scripting variables

## Key System Features

### 1. Multiplayer Gameplay
- Ball targets different players and bots
- Players can block the ball by pressing a key
- Bots have a configurable chance to block
- Visual feedback for hits and blocks
- Supports multiple players joining and leaving

### 2. Networking Implementation
- Uses Spatial's SyncedObject system for networking
- Host client controls the ball (authoritative server model)
- Player blocking state is synchronized using actor custom properties
- Bot state is synchronized using SyncedObject
- Visual Scripting integration for synchronizing variables

### 3. Bot AI System
- Bots move randomly around the environment using NavMesh
- Bots have a configurable chance to block the ball
- Bot manager keeps track of all bots in the scene
- Bots are implemented as SyncedObjects with local ownership

### 4. Visual Feedback
- Floating text for hits, blocks, and other game events
- Visual effects for game events
- Debug tools for monitoring actor states

## Integration with Spatial SDK
The template demonstrates several Spatial SDK features:

1. **Multiplayer and Networking**
   - `SpatialSyncedObject` for networked objects
   - Actor custom properties for player state
   - Actor service for player management

2. **Avatar Integration**
   - Targeting avatars using avatar bone transforms
   - Detecting collisions with avatars using trigger colliders

3. **Visual Effects**
   - `SpatialBridge.vfxService.CreateFloatingText` for game feedback

4. **Visual Scripting Integration**
   - Bridging C# code to Visual Scripting variables
   - Using Visual Scripting for networking

## Known Issues
- As noted in the README, there's an issue with actor custom properties not properly syncing, resulting in non-host clients being unable to block the ball. This is expected to be fixed in a future Spatial.io patch.

## How to Use This Template

### Key Components
1. **_DeathBall.unity**: Main scene containing the complete setup
2. **BallControl.cs**: Controls the ball behavior
3. **Bot.cs**: Controls the AI bots
4. **LocalPlayerControls.cs**: Handles player input

### Customization Options
1. **Ball Behavior**:
   - Adjust power settings in BallControl.cs
   - Modify target selection logic
   - Change collision handling

2. **Bot Behavior**:
   - Adjust movement frequency and range
   - Change block chance
   - Implement more complex AI

3. **Player Controls**:
   - Modify input keys for blocking
   - Add additional player actions

## Source Verification
- Documentation created on: March 22, 2025
- Source files last accessed: March 22, 2025
- Documentation matches source: ✓
