# Spatial Camera Modes Sample Template

## Template Overview
The Spatial Camera Modes Sample template demonstrates various camera modes and settings available in Spatial. It showcases techniques for camera control including first-person, over-shoulder, side-view, and top-down perspectives, as well as camera effects like wobble, shake, and kick. The template includes both C# and Visual Scripting implementations.

## Template Information
- Location: `E:\1-ECHO-WORKING-FOLDER\Spatial-Templates\spatial-example-camera-modes`
- Repository URL: https://github.com/spatialsys/spatial-example-camera-modes (inferred from directory name)
- Live Space: https://www.spatial.io/s/Camera-Modes-Sample-Scene-65f1c03f27709aee7203421b
- Documentation Reference: https://docs.spatial.io/controlling-the-camera

## Directory Structure
```
E:\1-ECHO-WORKING-FOLDER\Spatial-Templates\spatial-example-camera-modes\
├── .git/
├── .gitignore
├── Assets/
│   ├── Camera Modes Sample/
│   │   ├── Audio/
│   │   ├── Camera Modes Sample Scene C#.unity
│   │   ├── Camera Modes Sample Scene.unity
│   │   ├── Materials/
│   │   ├── Models/
│   │   ├── Prefabs/
│   │   ├── Scripts/
│   │   │   ├── BigBoomScript.cs
│   │   │   ├── CreatorToolkitCustomScripts.asmdef
│   │   │   ├── ForceFirstPersonScript.cs
│   │   │   ├── OverShoulderScript.cs
│   │   │   ├── Rotate.cs
│   │   │   ├── SideViewScript.cs
│   │   │   └── TopDownScript.cs
│   │   ├── Visual Script Graphs/
│   │   │   ├── Big Boom Interactable.asset
│   │   │   ├── First Person View.asset
│   │   │   ├── Over Shoulder Third Person.asset
│   │   │   ├── Rotate.asset
│   │   │   ├── Side View Scrollable.asset
│   │   │   └── Top Down View.asset
│   │   └── Walking Simulator/
│   ├── SampleScene_Thumbnail.png
│   ├── Settings/
│   ├── Spatial/
│   ├── StarterAssets/
│   ├── TextMesh Pro/
│   ├── UnityTechnologies/
│   └── VOiD1 Gaming - Free Hologram Shader for Unity URP/
├── LICENSE
├── Packages/
├── ProjectSettings/
└── README.md
```

## C# Scripts
### BigBoomScript.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;
using TMPro;

namespace CreatorToolkitCustomScripts
{
    [RequireComponent(typeof(SpatialInteractable))]
    public class BigBoomScript : MonoBehaviour
    {
        [SerializeField] private AudioSource countdownAudio;
        [SerializeField] private AudioSource explosionAudio;
        [SerializeField] private ParticleSystem explosionParticles;
        [SerializeField] private GameObject explodingBarrels;
        private SpatialInteractable interactable;
        void Start()
        {
            interactable = GetComponent<SpatialInteractable>();
            interactable.onInteractEvent += OnInteract;
        }
        void OnDestroy()
        {
            interactable.onInteractEvent -= OnInteract;
        }
        public void OnInteract()
        {
            StartCoroutine(ExplosionCoroutine());
        }
        private IEnumerator ExplosionCoroutine()
        {
            countdownAudio.Play();
            yield return new WaitForSeconds(countdownAudio.clip.length);
            explosionParticles.Play();
            explosionAudio.Play();
            explodingBarrels.SetActive(false);
            SpatialBridge.cameraService.shakeAmplitude = 1;
            SpatialBridge.cameraService.Wobble(1);
            SpatialBridge.cameraService.Wobble(new Vector3(5, 5, 5));
            SpatialBridge.cameraService.Kick(new Vector2(20, 20));
            SpatialBridge.cameraService.kickDecay = 200;
            yield return new WaitForSeconds(1.8f);
            explosionParticles.Stop();
            yield return new WaitForSeconds(2);
            explodingBarrels.SetActive(true);
        }
    }
}
```

**Purpose**: Creates an interactive explosion effect that demonstrates camera shake, wobble, and kick effects when triggered.

**Spatial SDK Usage**: 
- Uses `SpatialInteractable` for interaction events
- Uses `SpatialBridge.cameraService` for camera effects:
  - `shakeAmplitude` property
  - `Wobble(float)` and `Wobble(Vector3)` methods
  - `Kick(Vector2)` method
  - `kickDecay` property

**Dependencies**:
- Requires AudioSource components for countdown and explosion sounds
- Requires ParticleSystem for explosion visual effects
- Requires GameObject reference for exploding barrels

### ForceFirstPersonScript.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;

namespace CreatorToolkitCustomScripts
{
    [RequireComponent(typeof(SpatialTriggerEvent))]
    public class ForceFirstPersonScript : MonoBehaviour
    {
        private SpatialTriggerEvent spatialTrigger;

        void Start()
        {
            spatialTrigger = GetComponent<SpatialTriggerEvent>();
            spatialTrigger.onEnterEvent += ActivateFirstPerson;
            spatialTrigger.onExitEvent += DectivateFirstPerson;
        }
        void OnDestroy()
        {
            spatialTrigger.onEnterEvent -= ActivateFirstPerson;
            spatialTrigger.onExitEvent -= DectivateFirstPerson;
        }
        public void ActivateFirstPerson()
        {
            SpatialBridge.cameraService.forceFirstPerson = true;
        }
        public void DectivateFirstPerson()
        {
            SpatialBridge.cameraService.forceFirstPerson = false;
        }
    }
}
```

**Purpose**: Forces the camera into first-person mode when entering a trigger area, and returns to normal when exiting.

**Spatial SDK Usage**: 
- Uses `SpatialTriggerEvent` for trigger enter/exit events
- Uses `SpatialBridge.cameraService.forceFirstPerson` property

**Dependencies**:
- Requires `SpatialTriggerEvent` component on the same GameObject

### OverShoulderScript.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;

namespace CreatorToolkitCustomScripts
{
    [RequireComponent(typeof(SpatialTriggerEvent))]
    public class OverShoulderScript : MonoBehaviour
    {
        [SerializeField] private Vector3 cameraOffset;
        private SpatialTriggerEvent spatialTrigger;

        void Start()
        {
            spatialTrigger = GetComponent<SpatialTriggerEvent>();
            spatialTrigger.onEnterEvent += ActivateThirdPerson;
            spatialTrigger.onExitEvent += DeactivateThirdPerson;
        }
        void OnDestroy()
        {
            spatialTrigger.onEnterEvent -= ActivateThirdPerson;
            spatialTrigger.onExitEvent -= DeactivateThirdPerson;
        }
        void ActivateThirdPerson()
        {
            SpatialBridge.cameraService.zoomDistance = 1;
            SpatialBridge.cameraService.minZoomDistance = 1;
            SpatialBridge.cameraService.maxZoomDistance = 3;
            SpatialBridge.cameraService.thirdPersonFov = 60;
            SpatialBridge.cameraService.thirdPersonOffset = cameraOffset;
        }
        void DeactivateThirdPerson()
        {
            SpatialBridge.cameraService.zoomDistance = 6;
            SpatialBridge.cameraService.minZoomDistance = 0;
            SpatialBridge.cameraService.maxZoomDistance = 10;
            SpatialBridge.cameraService.thirdPersonFov = 70;
            SpatialBridge.cameraService.thirdPersonOffset = Vector3.zero;
        }
    }
}
```

**Purpose**: Configures the camera for an over-shoulder, third-person perspective when entering a trigger area, and resets to default when exiting.

**Spatial SDK Usage**: 
- Uses `SpatialTriggerEvent` for trigger enter/exit events
- Uses several `SpatialBridge.cameraService` properties:
  - `zoomDistance`
  - `minZoomDistance` and `maxZoomDistance`
  - `thirdPersonFov`
  - `thirdPersonOffset`

**Dependencies**:
- Requires `SpatialTriggerEvent` component on the same GameObject
- Takes a serialized `cameraOffset` Vector3 parameter

### Rotate.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CreatorToolkitCustomScripts
{
    public class Rotate : MonoBehaviour
    {
        void Update()
        {
            transform.Rotate(new Vector3(0, 15 * Time.deltaTime));
        }
    }
}
```

**Purpose**: Simple utility script that continuously rotates a GameObject around its Y-axis.

**Spatial SDK Usage**: 
- No direct Spatial SDK usage

**Dependencies**:
- None

### SideViewScript.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;
namespace CreatorToolkitCustomScripts
{
    [RequireComponent(typeof(SpatialTriggerEvent))]
    public class SideViewScript : MonoBehaviour
    {
        [SerializeField] SpatialVirtualCamera vcam;
        private Vector3 playerPos;
        private SpatialTriggerEvent spatialTrigger;
        void Start()
        {
            spatialTrigger = GetComponent<SpatialTriggerEvent>();
            spatialTrigger.onEnterEvent += EnableCam;
            spatialTrigger.onExitEvent += DisableCam;
        }
        void OnDestroy()
        {
            spatialTrigger.onEnterEvent -= EnableCam;
            spatialTrigger.onExitEvent -= DisableCam;
        }
        void EnableCam()
        {
            vcam.enabled = true;
        }
        void DisableCam()
        {
            vcam.enabled = false;
        }
        public void Update()
        {
            playerPos = SpatialBridge.actorService.localActor.avatar.position;
            playerPos.z = 50;
            vcam.transform.position = playerPos;
        }
    }
}
```

**Purpose**: Enables a side-view camera when entering a trigger area, and continuously updates the camera position to track the player on the X and Y axes while maintaining a fixed Z position.

**Spatial SDK Usage**: 
- Uses `SpatialTriggerEvent` for trigger enter/exit events
- Uses `SpatialVirtualCamera` for camera control
- Uses `SpatialBridge.actorService.localActor.avatar.position` to track player position

**Dependencies**:
- Requires `SpatialTriggerEvent` component on the same GameObject
- Requires reference to a `SpatialVirtualCamera`

### TopDownScript.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;

namespace CreatorToolkitCustomScripts
{
    [RequireComponent(typeof(SpatialTriggerEvent))]
    public class TopDownScript : MonoBehaviour
    {
        [SerializeField] private GameObject vcam;
        private SpatialTriggerEvent spatialTrigger;

        void Start()
        {
            spatialTrigger = GetComponent<SpatialTriggerEvent>();
            spatialTrigger.onEnterEvent += ActivateCamera;
            spatialTrigger.onExitEvent += DeactivateCamera;
        }
        void OnDestroy()
        {
            spatialTrigger.onEnterEvent -= ActivateCamera;
            spatialTrigger.onExitEvent -= DeactivateCamera;
        }
        void ActivateCamera()
        {
            vcam.SetActive(true);
        }
        void DeactivateCamera()
        {
            vcam.SetActive(false);
        }
    }
}
```

**Purpose**: Activates a top-down view camera when entering a trigger area, and deactivates it when exiting.

**Spatial SDK Usage**: 
- Uses `SpatialTriggerEvent` for trigger enter/exit events

**Dependencies**:
- Requires `SpatialTriggerEvent` component on the same GameObject
- Requires reference to a GameObject (virtual camera)

## Visual Script Graphs
The template includes Visual Scripting assets (.asset files) that implement the same functionality as the C# scripts:

### Big Boom Interactable.asset
- Location: `Assets\Camera Modes Sample\Visual Script Graphs\Big Boom Interactable.asset`
- Functionality: Visual scripting implementation of the explosion effect with camera shake/wobble/kick

### First Person View.asset
- Location: `Assets\Camera Modes Sample\Visual Script Graphs\First Person View.asset`
- Functionality: Visual scripting implementation of forcing first-person camera mode

### Over Shoulder Third Person.asset
- Location: `Assets\Camera Modes Sample\Visual Script Graphs\Over Shoulder Third Person.asset`
- Functionality: Visual scripting implementation of over-shoulder camera configuration

### Rotate.asset
- Location: `Assets\Camera Modes Sample\Visual Script Graphs\Rotate.asset`
- Functionality: Visual scripting implementation of continuous object rotation

### Side View Scrollable.asset
- Location: `Assets\Camera Modes Sample\Visual Script Graphs\Side View Scrollable.asset`
- Functionality: Visual scripting implementation of side-view camera that follows player

### Top Down View.asset
- Location: `Assets\Camera Modes Sample\Visual Script Graphs\Top Down View.asset`
- Functionality: Visual scripting implementation of top-down camera view

## Unity Scenes
The template includes two scenes that demonstrate the camera modes using different implementation approaches:

### Camera Modes Sample Scene C#.unity
- Uses the C# script implementations

### Camera Modes Sample Scene.unity
- Uses the Visual Scripting implementations

## Key Spatial SDK Camera Service Features Demonstrated
1. **Camera Modes**
   - First Person: `forceFirstPerson = true`
   - Third Person: Configuration via `thirdPersonOffset`, `thirdPersonFov`, etc.
   - Virtual Cameras: Using `SpatialVirtualCamera`

2. **Camera Effects**
   - Shake: `shakeAmplitude` property
   - Wobble: `Wobble(float)` and `Wobble(Vector3)` methods
   - Kick: `Kick(Vector2)` method and `kickDecay` property

3. **Zoom Controls**
   - `zoomDistance` property
   - `minZoomDistance` and `maxZoomDistance` properties

## Source Verification
- Documentation created on: March 22, 2025
- Source files last accessed: March 22, 2025
- Documentation matches source: ✓
