# Spatial Cinemachine Sample Template

## Template Overview
The Spatial Cinemachine Sample template demonstrates how to integrate Unity's Cinemachine system with Spatial for advanced camera control. It showcases three different Cinemachine virtual camera setups: Dolly track, Look-at, and Follow cameras, along with UI controls to switch between them and adjust camera priorities.

## Template Information
- Location: `E:\1-ECHO-WORKING-FOLDER\Spatial-Templates\spatial-example-cinemachine`
- Repository URL: https://github.com/spatialsys/spatial-example-cinemachine
- Live Space: https://www.spatial.io/s/Cinemachine-Sample-65ef8246e491514b195b4b03
- Documentation Reference: https://docs.spatial.io/controlling-the-camera

## Directory Structure
```
E:\1-ECHO-WORKING-FOLDER\Spatial-Templates\spatial-example-cinemachine\
├── .git/
├── .gitattributes
├── .gitignore
├── Assets/
│   ├── Examples/
│   │   └── Cinemachine_Sample/
│   │       ├── Scenes/
│   │       │   ├── Environment.unity
│   │       │   └── Lighting Settings.lighting
│   │       └── Scripts/
│   │           ├── CameraManager.cs
│   │           ├── CreatorToolkitCustomScripts.asmdef
│   │           ├── DollyCam.cs
│   │           ├── FollowCam.cs
│   │           ├── LookAtCam.cs
│   │           └── PrioritySlider.cs
│   ├── Spatial/
│   └── TextMesh Pro/
├── LICENSE
├── Packages/
├── ProjectSettings/
└── README.md
```

## C# Scripts
### CameraManager.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace CreatorToolkitCustomScripts
{
    public class CameraManager : MonoBehaviour
    {
        enum VirtualCameraType { Dolly, LookAt, Follow, None }
        VirtualCameraType activeCam = VirtualCameraType.LookAt;
        public CinemachineVirtualCamera dollyCam;
        public CinemachineVirtualCamera lookAtCam;
        public CinemachineVirtualCamera followCam;

        public Button dollyButton;
        public Button lookAtButton;
        public Button followButton;
        public Button disableAll;
        void Start()
        {
            dollyButton.onClick.AddListener(SwitchToDolly);
            lookAtButton.onClick.AddListener(SwitchToLookAt);
            followButton.onClick.AddListener(SwitchToFollow);
            disableAll.onClick.AddListener(DisableAll);
        }
        void SwitchToDolly()
        {
            activeCam = VirtualCameraType.Dolly;
            UpdateCameras();

        }

        void SwitchToLookAt()
        {
            activeCam = VirtualCameraType.LookAt;
            UpdateCameras();

        }

        void SwitchToFollow()
        {
            activeCam = VirtualCameraType.Follow;
            UpdateCameras();

        }

        void DisableAll()
        {
            activeCam = VirtualCameraType.None;
            UpdateCameras();
        }

        void UpdateCameras()
        {
            dollyCam.enabled = activeCam == VirtualCameraType.Dolly;
            lookAtCam.enabled = activeCam == VirtualCameraType.LookAt;
            followCam.enabled = activeCam == VirtualCameraType.Follow;
        }
    }
}
```

**Purpose**: Manages the activation and deactivation of different Cinemachine virtual cameras based on UI button input.

**Spatial SDK Usage**: 
- No direct Spatial SDK usage in this script

**Dependencies**:
- Requires references to three `CinemachineVirtualCamera` components
- Requires references to four UI `Button` components
- Uses Unity's UI system for button events

### DollyCam.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace CreatorToolkitCustomScripts
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class DollyCam : MonoBehaviour
    {
        CinemachineVirtualCamera vcam;
        CinemachineTrackedDolly dolly;
        float speed = 0.2f;

        void Awake()
        {
            vcam = GetComponent<CinemachineVirtualCamera>();
            dolly = vcam.GetCinemachineComponent<CinemachineTrackedDolly>();
        }

        void Update()
        {
            float increment = speed * Time.deltaTime;
            if (dolly.m_PathPosition + increment < dolly.m_Path.MinPos || dolly.m_PathPosition + increment > dolly.m_Path.MaxPos)
            {
                speed *= -1;
            }
            dolly.m_PathPosition += speed * Time.deltaTime;
        }
    }
}
```

**Purpose**: Controls a Cinemachine virtual camera with a dolly track, automatically moving it back and forth along the path.

**Spatial SDK Usage**: 
- No direct Spatial SDK usage in this script

**Dependencies**:
- Requires a `CinemachineVirtualCamera` component on the same GameObject
- Requires the virtual camera to have a `CinemachineTrackedDolly` component

### FollowCam.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using SpatialSys.UnitySDK;
using UnityEngine.PlayerLoop;

namespace CreatorToolkitCustomScripts
{
    public class FollowCam : MonoBehaviour
    {
        CinemachineVirtualCamera vcam;
        void Awake()
        {
            vcam = GetComponent<CinemachineVirtualCamera>();

            // Check to make sure the avatar is loaded before setting Follow target
            if (SpatialBridge.actorService.localActor.avatar.isBodyLoaded)
            {
                SetCameraFollow();
            }

            // Spatial allows users to change their avatars, which can cause the initial Follow target
            // to be destroyed. We have to set it again once new avatar is loaded
            SpatialBridge.actorService.localActor.avatar.onAvatarLoadComplete += SetCameraFollow;
        }

        void OnDestroy()
        {
            SpatialBridge.actorService.localActor.avatar.onAvatarLoadComplete -= SetCameraFollow;
        }

        void SetCameraFollow()
        {
            // Set the Follow target to the chest of the player's avatar
            vcam.Follow = SpatialBridge.actorService.localActor.avatar.GetAvatarBoneTransform(HumanBodyBones.Chest);
        }
    }
}
```

**Purpose**: Sets up a Cinemachine virtual camera to follow the player's avatar, specifically targeting the chest bone.

**Spatial SDK Usage**: 
- Uses `SpatialBridge.actorService.localActor.avatar` to access the player's avatar
- Uses `avatar.isBodyLoaded` to check avatar loading status
- Uses `avatar.onAvatarLoadComplete` event to handle avatar changes
- Uses `avatar.GetAvatarBoneTransform(HumanBodyBones.Chest)` to target a specific avatar bone

**Dependencies**:
- Requires a `CinemachineVirtualCamera` component on the same GameObject

### LookAtCam.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;
using Cinemachine;
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class LookAtCam : MonoBehaviour
{
    CinemachineVirtualCamera vcam;
    void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();

        // Check to make sure the avatar is loaded before setting LookAt target
        if (SpatialBridge.actorService.localActor.avatar.isBodyLoaded)
        {
            SetCameraLookAt();
        }

        // Spatial allows users to change their avatars, which can cause the initial LookAt target
        // to be destroyed. We have to set it again once new avatar is loaded
        SpatialBridge.actorService.localActor.avatar.onAvatarLoadComplete += SetCameraLookAt;
    }

    void OnDestroy()
    {
        SpatialBridge.actorService.localActor.avatar.onAvatarLoadComplete -= SetCameraLookAt;
    }

    void SetCameraLookAt()
    {
        // Set the LookAt target to the chest of the player's avatar
        vcam.LookAt = SpatialBridge.actorService.localActor.avatar.GetAvatarBoneTransform(HumanBodyBones.Chest);
    }
}
```

**Purpose**: Sets up a Cinemachine virtual camera to always look at the player's avatar, specifically targeting the chest bone.

**Spatial SDK Usage**: 
- Uses `SpatialBridge.actorService.localActor.avatar` to access the player's avatar
- Uses `avatar.isBodyLoaded` to check avatar loading status
- Uses `avatar.onAvatarLoadComplete` event to handle avatar changes
- Uses `avatar.GetAvatarBoneTransform(HumanBodyBones.Chest)` to target a specific avatar bone

**Dependencies**:
- Requires a `CinemachineVirtualCamera` component on the same GameObject

### PrioritySlider.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

namespace CreatorToolkitCustomScripts
{
    [RequireComponent(typeof(Slider))]
    public class PrioritySlider : MonoBehaviour
    {
        public CinemachineVirtualCamera lookAtCam;
        public CinemachineVirtualCamera dollyCam;
        public CinemachineVirtualCamera followCam;
        public TextMeshProUGUI priorityText;
        Slider slider;
        void Start()
        {
            slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(HandleOnValueChanged);
        }

        void HandleOnValueChanged(float value)
        {
            lookAtCam.Priority = (int)value;
            dollyCam.Priority = (int)value;
            followCam.Priority = (int)value;
            priorityText.text = $"{(int)value}";
        }
    }
}
```

**Purpose**: Controls a UI slider that adjusts the priority of Cinemachine virtual cameras, demonstrating how camera priority affects blending between cameras.

**Spatial SDK Usage**: 
- No direct Spatial SDK usage in this script

**Dependencies**:
- Requires a `Slider` component on the same GameObject
- Requires references to three `CinemachineVirtualCamera` components
- Requires a `TextMeshProUGUI` component for displaying the current priority value

## Unity Scene
### Environment.unity
This scene contains the complete setup of the Cinemachine example, including:
- Three Cinemachine virtual cameras (Dolly, LookAt, Follow)
- UI controls for switching between cameras and adjusting priority
- A physical environment for demonstration
- Cinemachine path for the dolly camera to follow

## Key Cinemachine Features Demonstrated
1. **Camera Types**
   - Dolly Camera: Camera that moves along a predefined path
   - Look-At Camera: Static camera that always looks at the player
   - Follow Camera: Camera that follows the player's position

2. **Camera Targeting**
   - Using avatar bones as targets (`HumanBodyBones.Chest`)
   - Handling avatar loading and changes

3. **Cinemachine Controls**
   - Priority-based camera selection
   - Enable/disable cameras via script
   - Path-based camera movement

## Integration with Spatial SDK
1. **Avatar Integration**
   - Access to avatar bones through `SpatialBridge.actorService.localActor.avatar.GetAvatarBoneTransform`
   - Handling avatar loading events with `onAvatarLoadComplete`
   - Checking avatar loading status with `isBodyLoaded`

2. **Event Handling**
   - Proper cleanup in `OnDestroy` to prevent memory leaks
   - Setting up event listeners for avatar loading

## Source Verification
- Documentation created on: March 22, 2025
- Source files last accessed: March 22, 2025
- Documentation matches source: ✓
