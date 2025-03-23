# Spatial Car Controller (SCC) Template

## Template Overview
The Spatial Car Controller (SCC) template provides an implementation for integrating the Simple Car Controller Unity asset store package with Spatial's platform. It enables vehicle controls, camera management, and avatar interaction within Spatial environments.

## Template Information
- Location: `E:\1-ECHO-WORKING-FOLDER\Spatial-Templates\spatial-car-controller-scc`
- Repository URL: https://github.com/spatialsys/spatial-car-controller-scc (inferred from directory name)
- Dependencies: 
  - Requires the Unity Asset Store package "Simple Car Controller" (as stated in README.md)
  - Note: When importing the asset package, certain scripts should NOT be included (SCC_Demo.cs, SCC_InputActions.cs, SCC_InputManager.cs, SCC_InputProcessor.cs, or SCC_Singleton.cs)

## Directory Structure
```
E:\1-ECHO-WORKING-FOLDER\Spatial-Templates\spatial-car-controller-scc\
├── .git/
├── .gitignore
├── README.md
└── SpatialSCCUnity/
    ├── Assets/
    │   ├── Materials/
    │   ├── Prefabs/
    │   ├── Scenes/
    │   ├── Scripts/
    │   │   ├── CreatorToolkitCustomScripts.asmdef
    │   │   ├── SCC_InputProcessor.cs
    │   │   ├── Spatial_SCC.cs
    │   │   └── SpawnCarButton.cs
    │   ├── Spatial/
    │   ├── TextMesh Pro/
    │   ├── Terrain.asset
    │   └── thumbnail.png
    ├── Packages/
    └── ProjectSettings/
```

## C# Scripts
### SCC_InputProcessor.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCC_InputProcessor : MonoBehaviour{

    public SCC_Inputs inputs = new SCC_Inputs();        //  Target inputs.
    public bool smoothInputs = true;        //  Smoothly lerp the inputs?
    public float smoothingFactor = 5f;      //  Smoothing factor.

    public void OverrideInputs(SCC_Inputs newInputs) {
        if (!smoothInputs) {
            inputs = newInputs;
        } else {
            inputs.throttleInput = Mathf.MoveTowards(inputs.throttleInput, newInputs.throttleInput, Time.deltaTime * smoothingFactor);
            inputs.steerInput = Mathf.MoveTowards(inputs.steerInput, newInputs.steerInput, Time.deltaTime * smoothingFactor);
            inputs.brakeInput = Mathf.MoveTowards(inputs.brakeInput, newInputs.brakeInput, Time.deltaTime * smoothingFactor);
            inputs.handbrakeInput = Mathf.MoveTowards(inputs.handbrakeInput, newInputs.handbrakeInput, Time.deltaTime * smoothingFactor);
        }
    }
}
```

**Purpose**: Processes vehicle input controls with optional smoothing for the Simple Car Controller system. It allows for overriding current input values with new ones, either immediately or with gradual interpolation.

**Spatial SDK Usage**: 
- No direct Spatial SDK usage in this script
- Note: This script replaces the original SCC_InputProcessor.cs from the Simple Car Controller package

**Dependencies**:
- Requires the SCC_Inputs class (not shown in the provided scripts, likely part of the Simple Car Controller package)

### Spatial_SCC.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;

public class Spatial_SCC : MonoBehaviour, IVehicleInputActionsListener
{
    public SCC_InputProcessor inputProcessor;
    public Transform seat;
    public Transform cameraTarget;

    private SpatialSyncedObject _syncedObject;
    private SCC_Inputs _inputs = new SCC_Inputs();
    private bool _isDriving = false;

    private const VehicleInputFlags INPUT_FLAGS = VehicleInputFlags.Steer1D | VehicleInputFlags.Throttle | VehicleInputFlags.Reverse | VehicleInputFlags.PrimaryAction;

    private void Awake()
    {
        _syncedObject = GetComponent<SpatialSyncedObject>();

        UpdateShouldBeDriving();
        _syncedObject.onOwnerChanged += HandleOwnerChanged;
        SpatialBridge.spaceContentService.onSceneInitialized += HandleSceneInitialized;
    }

    private void OnDestroy()
    {
        SpatialBridge.inputService.ReleaseInputCapture(this);
        if (_isDriving)
            StoppedDriving();
    }

    private void Update()
    {
        if (_syncedObject.isLocallyOwned)
        {
            inputProcessor.OverrideInputs(_inputs);
        }
    }

    public void StartDriving()
    {
        SpatialBridge.inputService.StartVehicleInputCapture(INPUT_FLAGS, null, null, this);
    }

    public void StopDriving()
    {
        SpatialBridge.inputService.ReleaseInputCapture(this);
        Destroy(gameObject);
    }

    private void HandleOwnerChanged(int newOwner)
    {
        UpdateShouldBeDriving();
    }

    private void HandleSceneInitialized()
    {
        UpdateShouldBeDriving();
    }

    private void UpdateShouldBeDriving()
    {
        if (_syncedObject.isLocallyOwned)
        {
            if (!_isDriving)
            {
                StartDriving();
            }
        }
        else
        {
            if (_isDriving)
            {
                StopDriving();
            }
        }
    }

    private void StartedDriving()
    {
        SpatialBridge.cameraService.SetTargetOverride(cameraTarget, SpatialCameraMode.Vehicle);
        SpatialBridge.actorService.localActor.avatar.Sit(seat);
        _inputs.steerInput = 0;
        _inputs.handbrakeInput = 0;
        _inputs.throttleInput = 0;
        _inputs.brakeInput = 0;
        _isDriving = true;
    }

    private void StoppedDriving()
    {
        SpatialBridge.cameraService.ClearTargetOverride();
        SpatialBridge.actorService.localActor.avatar.Stand();
        _inputs.handbrakeInput = 1;
        _inputs.throttleInput = 0;
        _inputs.brakeInput = 0;
        inputProcessor.OverrideInputs(_inputs);
        _isDriving = false;
    }

#region IVehicleInputActionsListener

    public void OnInputCaptureStarted(InputCaptureType type)
    {
        StartedDriving();
    }

    public void OnInputCaptureStopped(InputCaptureType type)
    {
        StoppedDriving();
    }

    public void OnVehicleSteerInput(InputPhase inputPhase, Vector2 inputSteer)
    {
        _inputs.steerInput = inputSteer.x;
    }

    public void OnVehicleThrottleInput(InputPhase inputPhase, float inputThrottle)
    {
        _inputs.throttleInput = inputThrottle;
    }

    public void OnVehicleReverseInput(InputPhase inputPhase, float inputReverse)
    {
        _inputs.brakeInput = inputReverse;
    }

    public void OnVehiclePrimaryActionInput(InputPhase inputPhase)
    {
        _inputs.handbrakeInput = inputPhase != InputPhase.OnReleased ? 1 : 0;
    }

    public void OnVehicleSecondaryActionInput(InputPhase inputPhase)
    {
    }

    public void OnVehicleExitInput()
    {
        StopDriving();
    }

#endregion
}
```

**Purpose**: Main script for Spatial vehicle integration. Handles vehicle input through Spatial's input system, manages ownership and synchronization, controls avatar sitting/standing, and manages camera behavior for the vehicle.

**Spatial SDK Usage**: 
- Implements `IVehicleInputActionsListener` interface for vehicle input handling
- Uses `SpatialSyncedObject` for ownership and networking
- Uses multiple SpatialBridge services:
  - `spaceContentService` for scene initialization events
  - `inputService` for vehicle input capture
  - `cameraService` for camera targeting
  - `actorService` for avatar control
- Uses `SpatialCameraMode.Vehicle` for camera behavior
- Uses `InputPhase` and `InputCaptureType` enums
- Uses `VehicleInputFlags` for input configuration

**Dependencies**:
- `SCC_InputProcessor` for handling inputs
- `SCC_Inputs` class (from Simple Car Controller)
- Requires Transform references for `seat` and `cameraTarget`
- Requires `SpatialSyncedObject` component on the same GameObject

### SpawnCarButton.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;
using TMPro;

public class SpawnCarButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public Spatial_SCC prefab;
    private Spatial_SCC _currentCar;

    public void ToggleSpawn()
    {
        if (_currentCar != null)
        {
            _currentCar.StopDriving();
            _currentCar = null;
            buttonText.text = "Drive";
        }
        else
        {
            Vector3 pos = SpatialBridge.actorService.localActor.avatar.position;
            Quaternion rot = SpatialBridge.actorService.localActor.avatar.rotation;
            pos += rot * new Vector3(0, 0, 3);
            _currentCar = Instantiate(prefab.gameObject, pos, rot).GetComponent<Spatial_SCC>();
            buttonText.text = "Stop";
        }
    }
}
```

**Purpose**: Provides a UI button functionality to spawn or despawn a car. When activated, it instantiates a car in front of the player's avatar or removes the existing car.

**Spatial SDK Usage**: 
- Uses `SpatialBridge.actorService.localActor.avatar` to get the player's position and rotation

**Dependencies**:
- Requires `TextMeshProUGUI` component for button text
- References the `Spatial_SCC` prefab for instantiation
- Uses TextMesh Pro for UI

## Relationships and Dependencies
- `SpawnCarButton` → Instantiates → `Spatial_SCC` prefab
- `Spatial_SCC` → Uses → `SCC_InputProcessor` for input handling
- `Spatial_SCC` → Implements → `IVehicleInputActionsListener` for Spatial vehicle input
- All scripts rely on the Simple Car Controller package from Unity Asset Store
- System requires proper setup of the car prefab with seat and camera target transforms

## Source Verification
- Documentation created on: March 22, 2025
- Source files last accessed: March 22, 2025
- Documentation matches source: ✓
