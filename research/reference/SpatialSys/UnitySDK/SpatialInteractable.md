# SpatialInteractable

Category: Core Components

Interface/Class/Enum: Class

The SpatialInteractable component enables objects to be interacted with by users in a Spatial environment. It provides functionality for displaying interaction prompts and triggering events when users interact with the object.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| interactText | string | The text prompt displayed to users when they can interact with this object. |
| interactiveRadius | float | The distance from the object at which users can interact with it. |
| visibilityRadius | float | The distance at which the interaction prompt becomes visible to users. |
| icon | Sprite | Custom icon to display alongside the interaction prompt. |
| iconType | SpatialInteractable.IconType | Type of icon to display (system icon or custom). |
| textFontOverride | TMP_FontAsset | Optional font override for the interaction text. |
| onInteractEvent | SpatialEvent | Event triggered when a user interacts with the object. |
| onEnterEvent | SpatialEvent | Event triggered when a user enters the interaction radius. |
| onExitEvent | SpatialEvent | Event triggered when a user exits the interaction radius. |
| documentationURL | string | URL to documentation for this component. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |

## Methods

| Method | Description |
| --- | --- |
| UpdateRadius(float, float) | Updates the interactive and visibility radius values. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObjectManager : MonoBehaviour
{
    [System.Serializable]
    public class InteractableConfig
    {
        public string interactText = "Interact";
        public float interactiveRadius = 3f;
        public float visibilityRadius = 5f;
        public UnityEvent onInteract;
    }
    
    [SerializeField] private InteractableConfig[] interactableConfigs;
    [SerializeField] private GameObject[] targetObjects;
    
    private void Start()
    {
        // Setup interactable objects with configurations
        ConfigureInteractables();
    }
    
    private void ConfigureInteractables()
    {
        // Ensure we have valid arrays
        if (interactableConfigs == null || targetObjects == null || 
            interactableConfigs.Length == 0 || targetObjects.Length == 0)
        {
            Debug.LogWarning("Missing interactable configurations or target objects!");
            return;
        }
        
        // Apply configurations to objects (cycling through configs if needed)
        for (int i = 0; i < targetObjects.Length; i++)
        {
            GameObject targetObject = targetObjects[i];
            InteractableConfig config = interactableConfigs[i % interactableConfigs.Length];
            
            // Add SpatialInteractable component if not already present
            SpatialInteractable interactable = targetObject.GetComponent<SpatialInteractable>();
            if (interactable == null)
            {
                interactable = targetObject.AddComponent<SpatialInteractable>();
            }
            
            // Configure interactable properties
            interactable.interactText = config.interactText;
            interactable.UpdateRadius(config.interactiveRadius, config.visibilityRadius);
            
            // Setup the interact event
            // Note: This is a conceptual example - the actual implementation
            // might differ based on how SpatialEvent is implemented
            if (config.onInteract != null)
            {
                // Add listener for the interact event (implementation depends on SpatialEvent)
                // This might look different in actual implementation
                // interactable.onInteractEvent.AddListener(config.onInteract.Invoke);
                
                Debug.Log($"Set up interaction for {targetObject.name} with text '{config.interactText}'");
            }
        }
    }
    
    // Example of a method to create an interactable object programmatically
    public SpatialInteractable CreateInteractableObject(string name, Vector3 position, string promptText, float interactRadius, float visibilityRadius, UnityAction onInteractAction)
    {
        // Create new GameObject
        GameObject newObject = new GameObject(name);
        newObject.transform.position = position;
        
        // Add visual representation
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visual.transform.parent = newObject.transform;
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = Vector3.one * 0.5f;
        
        // Add SpatialInteractable component
        SpatialInteractable interactable = newObject.AddComponent<SpatialInteractable>();
        interactable.interactText = promptText;
        interactable.UpdateRadius(interactRadius, visibilityRadius);
        
        // Setup interact event (implementation depends on SpatialEvent)
        // This might look different in actual implementation
        // if (onInteractAction != null)
        // {
        //     interactable.onInteractEvent.AddListener(onInteractAction);
        // }
        
        Debug.Log($"Created interactable object '{name}' with prompt '{promptText}'");
        return interactable;
    }
    
    // Example handler methods for interactable events
    public void HandleChestOpen()
    {
        Debug.Log("Player opened a chest!");
        // Add chest opening animation or logic here
    }
    
    public void HandleDoorInteraction()
    {
        Debug.Log("Player interacted with a door!");
        // Add door opening/closing logic here
    }
    
    public void HandleButtonPress()
    {
        Debug.Log("Player pressed a button!");
        // Add button activation logic here
    }
}
```

## Best Practices

1. Use clear and concise text for the `interactText` property to communicate what will happen when the user interacts.
2. Set appropriate radius values for `interactiveRadius` and `visibilityRadius` to ensure good user experience.
3. Choose appropriate icons that clearly communicate the type of interaction.
4. Consider accessibility when designing interaction prompts (font size, contrast, etc.).
5. Use event handlers to implement complex interaction behaviors separate from the SpatialInteractable component.
6. For networked experiences, consider how interaction events should be synchronized across clients.
7. Test interactions on different device types to ensure they're accessible to all users.
8. Group related interactable objects logically in your scene hierarchy for easier management.
9. Avoid placing too many interactable objects close together to prevent confusion.

## Common Use Cases

1. Interactive buttons, levers, and switches for controlling mechanisms.
2. Doors that can be opened or closed through interaction.
3. Information displays that show details when interacted with.
4. Collectible items that can be picked up.
5. NPC characters that can be conversed with.
6. Puzzles with interactive elements.
7. Customization stations where users can modify their avatars.
8. Commerce points where users can purchase items.
9. Teleportation points activated through interaction.
10. Hidden secrets or easter eggs revealed through interaction.

## Completed: March 09, 2025