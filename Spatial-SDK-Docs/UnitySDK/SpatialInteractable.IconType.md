## Overview
The SpatialInteractable.IconType enum defines the different icon types that can be displayed when a user interacts with a SpatialInteractable component. These icons provide visual cues to users about the type of interaction that will occur when they engage with an object in the Spatial environment.

## Properties
- **None**: No icon will be displayed.
- **Charge**: Displays a charging/energy icon, typically used for power-related interactables.
- **Couch**: Displays a couch/seating icon, typically used for sitting interactions.
- **CrisisAlert**: Displays an alert/warning icon, typically used for hazardous or important interactions.
- **DoorFront**: Displays a door icon, typically used for portals, entrances, or exits.
- **FitnessCenter**: Displays a fitness/activity icon, typically used for exercise or activity interactions.
- **Flood**: Displays a water/flooding icon, typically used for water-related interactions.
- **Hail**: Displays a hail/weather icon, typically used for weather-related interactions.
- **LunchDining**: Displays a food/dining icon, typically used for eating or food-related interactions.
- **MusicNote**: Displays a musical note icon, typically used for audio or music interactions.
- **RamenDining**: Displays a noodle/food icon, alternative to LunchDining for food interactions.
- **Soap**: Displays a soap/cleaning icon, typically used for cleanliness or hygiene interactions.
- **Timer**: Displays a timer/clock icon, typically used for time-related interactions.
- **Weapon**: Displays a weapon icon, typically used for combat or tool interactions.

## Usage Example
```csharp
using SpatialSys.UnitySDK;
using UnityEngine;

public class CustomInteractable : MonoBehaviour
{
    private SpatialInteractable interactable;
    
    void Start()
    {
        // Get or add the SpatialInteractable component
        interactable = GetComponent<SpatialInteractable>() ?? gameObject.AddComponent<SpatialInteractable>();
        
        // Set the interactable properties
        interactable.iconType = SpatialInteractable.IconType.MusicNote;
        interactable.text = "Play Music";
        
        // Subscribe to interaction events
        interactable.onInteract.AddListener(OnInteract);
    }
    
    private void OnInteract()
    {
        Debug.Log("User interacted with the music object!");
        // Play music or perform related action
    }
    
    // Dynamically change the icon based on state
    public void SetAsDoorIcon()
    {
        interactable.iconType = SpatialInteractable.IconType.DoorFront;
        interactable.text = "Open Door";
    }
    
    public void SetAsTimerIcon()
    {
        interactable.iconType = SpatialInteractable.IconType.Timer;
        interactable.text = "Start Timer";
    }
}
```

## Best Practices
- Choose icons that best represent the action that will occur when the user interacts with the object
- Use consistent icons across your space for similar types of interactions to build user familiarity
- Consider combining the icon with appropriate text to clarify the interaction
- For objects with multiple possible interactions, change the icon type to reflect the current primary interaction
- Avoid using icons that might mislead users about the outcome of an interaction
- Use **None** when you want to have an interactable without a visual icon (text-only or custom UI)
- Consider accessibility when choosing icons - not all users may understand the meaning of every icon

## Common Use Cases
- Creating interactive objects that players can engage with in Spatial environments
- Providing visual cues for different types of interactions in a space
- Building user interfaces for interactive elements like doors, seats, or music players
- Creating gameplay mechanics that require user interaction
- Designing educational or training simulations with guided interactions
- Building accessible experiences with clear visual feedback about interactable elements
- Implementing consistent interaction systems across different spaces or experiences

## Completed: March 10, 2025
