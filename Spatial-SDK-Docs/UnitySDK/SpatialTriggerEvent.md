# SpatialTriggerEvent

Category: Core Components

Interface/Class/Enum: Class

The SpatialTriggerEvent component allows developers to trigger events when objects enter or exit a collider in a Spatial environment. This component is commonly used for creating interactive zones, triggers, and spatial awareness in experiences.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| LATEST_VERSION | int | Static constant representing the latest version of the SpatialTriggerEvent component. |
| listenFor | ListenFor | Determines what kind of object will invoke the event when it enters or exits the trigger. |
| onEnterEvent | SpatialEvent | Event triggered when a valid object enters the trigger collider. |
| onExitEvent | SpatialEvent | Event triggered when a valid object exits the trigger collider. |
| deprecated_onEnter | SpatialEvent | Deprecated version of the enter event (use onEnterEvent instead). |
| deprecated_onExit | SpatialEvent | Deprecated version of the exit event (use onExitEvent instead). |
| isExperimental | bool | Indicates if this component uses experimental features. |
| version | int | The version of this component instance. |
| documentationURL | string | URL to documentation for this component. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using UnityEngine.Events;

public class TriggerZoneManager : MonoBehaviour
{
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material activeMaterial;
    
    // References to trigger zones in scene
    [SerializeField] private GameObject[] triggerZones;
    
    // Track which zones are currently active
    private bool[] zoneActive;
    
    // Store renderers for visual feedback
    private Renderer[] zoneRenderers;
    
    private void Start()
    {
        InitializeTriggerZones();
    }
    
    private void InitializeTriggerZones()
    {
        if (triggerZones == null || triggerZones.Length == 0)
            return;
            
        // Initialize tracking arrays
        zoneActive = new bool[triggerZones.Length];
        zoneRenderers = new Renderer[triggerZones.Length];
        
        // Set up each trigger zone
        for (int i = 0; i < triggerZones.Length; i++)
        {
            GameObject zone = triggerZones[i];
            
            // Ensure the zone has a collider
            Collider zoneCollider = zone.GetComponent<Collider>();
            if (zoneCollider == null)
            {
                zoneCollider = zone.AddComponent<BoxCollider>();
            }
            
            // Make sure the collider is a trigger
            zoneCollider.isTrigger = true;
            
            // Add/get trigger event component
            SpatialTriggerEvent triggerEvent = zone.GetComponent<SpatialTriggerEvent>();
            if (triggerEvent == null)
            {
                triggerEvent = zone.AddComponent<SpatialTriggerEvent>();
            }
            
            // Configure the trigger to listen for the local avatar
            triggerEvent.listenFor = SpatialTriggerEvent.ListenFor.LocalAvatar;
            
            // Get or add renderer for visual feedback
            zoneRenderers[i] = zone.GetComponent<Renderer>();
            if (zoneRenderers[i] == null && zone.transform.childCount > 0)
            {
                zoneRenderers[i] = zone.transform.GetChild(0).GetComponent<Renderer>();
            }
            
            // Set default material
            if (zoneRenderers[i] != null && defaultMaterial != null)
            {
                zoneRenderers[i].material = defaultMaterial;
            }
            
            // Store the zone index for event handlers
            int zoneIndex = i;
            
            // Set up event listeners
            // Use a lambda to capture the zone index
            triggerEvent.onEnterEvent.AddListener(() => OnZoneEnter(zoneIndex));
            triggerEvent.onExitEvent.AddListener(() => OnZoneExit(zoneIndex));
            
            Debug.Log($"Initialized trigger zone {zone.name} at index {i}");
        }
    }
    
    // Event handlers
    private void OnZoneEnter(int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= zoneActive.Length)
            return;
            
        Debug.Log($"Player entered zone {triggerZones[zoneIndex].name}");
        
        // Update zone state
        zoneActive[zoneIndex] = true;
        
        // Visual feedback
        if (zoneRenderers[zoneIndex] != null && activeMaterial != null)
        {
            zoneRenderers[zoneIndex].material = activeMaterial;
        }
        
        // Check if this activation completes a sequence or puzzle
        CheckZoneCompletion();
        
        // Provide feedback to the player
        SpatialBridge.coreGUIService.DisplayToastMessage($"Entered zone {zoneIndex + 1}");
    }
    
    private void OnZoneExit(int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= zoneActive.Length)
            return;
            
        Debug.Log($"Player exited zone {triggerZones[zoneIndex].name}");
        
        // Update zone state
        zoneActive[zoneIndex] = false;
        
        // Visual feedback
        if (zoneRenderers[zoneIndex] != null && defaultMaterial != null)
        {
            zoneRenderers[zoneIndex].material = defaultMaterial;
        }
        
        // Provide feedback to the player
        SpatialBridge.coreGUIService.DisplayToastMessage($"Exited zone {zoneIndex + 1}");
    }
    
    // Check if all zones are active (for puzzle completion, etc.)
    private void CheckZoneCompletion()
    {
        bool allZonesActive = true;
        foreach (bool active in zoneActive)
        {
            if (!active)
            {
                allZonesActive = false;
                break;
            }
        }
        
        if (allZonesActive)
        {
            Debug.Log("All zones activated!");
            SpatialBridge.coreGUIService.DisplayToastMessage("All zones activated! Puzzle complete!");
            
            // Trigger reward, next stage, etc.
            TriggerCompletion();
        }
    }
    
    // Handle completion of all zones
    private void TriggerCompletion()
    {
        // Example: Spawn a reward object
        GameObject reward = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        reward.transform.position = transform.position + Vector3.up * 2f;
        reward.transform.localScale = Vector3.one * 0.5f;
        
        // Add a material for visibility
        Renderer rewardRenderer = reward.GetComponent<Renderer>();
        if (rewardRenderer != null)
        {
            rewardRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            rewardRenderer.material.color = Color.yellow;
        }
        
        // Optional: Make the reward interactable
        SpatialInteractable interactable = reward.AddComponent<SpatialInteractable>();
        interactable.interactText = "Collect Reward";
        
        // Optional: Add collection behavior
        interactable.onInteractEvent.AddListener(() => {
            Debug.Log("Reward collected!");
            SpatialBridge.coreGUIService.DisplayToastMessage("Reward collected!");
            Destroy(reward);
        });
    }
    
    // Create a trigger zone programmatically
    public GameObject CreateTriggerZone(string zoneName, Vector3 position, Vector3 size, UnityAction onEnterAction, UnityAction onExitAction)
    {
        // Create the trigger object
        GameObject triggerObject = new GameObject(zoneName);
        triggerObject.transform.position = position;
        
        // Add a box collider configured as a trigger
        BoxCollider collider = triggerObject.AddComponent<BoxCollider>();
        collider.size = size;
        collider.isTrigger = true;
        
        // Add a visual representation (slightly transparent for visibility)
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.transform.SetParent(triggerObject.transform, false);
        visual.transform.localScale = size;
        
        // Make the visual semi-transparent
        Renderer renderer = visual.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            renderer.material.color = new Color(0f, 1f, 1f, 0.3f);
        }
        
        // Remove the collider from the visual since the parent has the trigger collider
        Destroy(visual.GetComponent<Collider>());
        
        // Add the SpatialTriggerEvent component
        SpatialTriggerEvent triggerEvent = triggerObject.AddComponent<SpatialTriggerEvent>();
        triggerEvent.listenFor = SpatialTriggerEvent.ListenFor.LocalAvatar;
        
        // Set up the event handlers
        if (onEnterAction != null)
        {
            triggerEvent.onEnterEvent.AddListener(onEnterAction);
        }
        
        if (onExitAction != null)
        {
            triggerEvent.onExitEvent.AddListener(onExitAction);
        }
        
        Debug.Log($"Created trigger zone '{zoneName}' at position {position} with size {size}");
        return triggerObject;
    }
}
```

## Best Practices

1. Always ensure the GameObject has a Collider component with isTrigger enabled.
2. Choose the appropriate ListenFor value based on what you want to detect.
3. Provide visual feedback when triggers are activated to improve user experience.
4. Use appropriately sized colliders - they should be large enough to detect but not so large they trigger unexpectedly.
5. Consider the player's movement speed when placing triggers - faster movement may require larger trigger zones.
6. Add visual indicators (subtle particles, glowing effects, etc.) to help users identify interactive zones.
7. For puzzle or sequence-based triggers, provide clear feedback about progress.
8. Consider using a combination of enter and exit events to create more complex behaviors.
9. Test trigger zones with different avatar sizes and movement speeds to ensure reliable detection.
10. Use layer-based filtering if you need finer control over what can trigger the event.

## Common Use Cases

1. Creating checkpoint systems in racing or obstacle course experiences.
2. Designing puzzle mechanics that require players to stand in specific locations.
3. Building interactive tutorials that detect when users enter training areas.
4. Constructing ambient zones that change music, lighting, or environmental effects.
5. Creating automatic doors that open when approached.
6. Building security or restricted areas that respond to player presence.
7. Developing dynamic spawn points that activate when players enter specific regions.
8. Implementing save points or respawn locations.
9. Creating invisible boundaries that trigger warnings when crossed.
10. Building sequential activation puzzles where players must touch multiple triggers in order.

## Completed: March 10, 2025