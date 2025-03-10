## Overview
The SpatialTriggerEvent.ListenFor enum defines the types of entities that a SpatialTriggerEvent component can detect when they enter, stay within, or exit the trigger area. This enum allows developers to specify which types of actors should trigger events in a Spatial space.

## Properties
- **None**: No entities will trigger the event.
- **LocalAvatar**: Only the local user's avatar will trigger the event.
- **LocalNPC**: Only locally controlled NPCs will trigger the event.

## Usage Example
```csharp
using SpatialSys.UnitySDK;
using UnityEngine;

public class TriggerAreaExample : MonoBehaviour
{
    private SpatialTriggerEvent triggerEvent;
    
    void Start()
    {
        // Get or add the SpatialTriggerEvent component
        triggerEvent = GetComponent<SpatialTriggerEvent>() ?? gameObject.AddComponent<SpatialTriggerEvent>();
        
        // Configure to listen for avatar entry/exit
        triggerEvent.listenFor = SpatialTriggerEvent.ListenFor.LocalAvatar;
        
        // Set up event handlers
        triggerEvent.onTriggerEnter.AddListener(OnPlayerEnter);
        triggerEvent.onTriggerExit.AddListener(OnPlayerExit);
        triggerEvent.onTriggerStay.AddListener(OnPlayerStay);
    }
    
    private void OnPlayerEnter()
    {
        Debug.Log("Local avatar entered the trigger area");
        
        // Perform actions when player enters the area
        ShowWelcomeMessage();
    }
    
    private void OnPlayerExit()
    {
        Debug.Log("Local avatar exited the trigger area");
        
        // Perform actions when player leaves the area
        HideWelcomeMessage();
    }
    
    private void OnPlayerStay()
    {
        // This event fires continuously while the avatar remains in the trigger area
        // Use sparingly to avoid performance issues
    }
    
    private void ShowWelcomeMessage()
    {
        SpatialBridge.coreGUIService.DisplayToastMessage("Welcome to this area!");
    }
    
    private void HideWelcomeMessage()
    {
        // Clear any UI elements or messages
    }
    
    // Example of changing the trigger setting at runtime
    public void SetListenForNPC()
    {
        triggerEvent.listenFor = SpatialTriggerEvent.ListenFor.LocalNPC;
        Debug.Log("Now listening for NPC triggers");
    }
    
    public void SetListenForNothing()
    {
        triggerEvent.listenFor = SpatialTriggerEvent.ListenFor.None;
        Debug.Log("Trigger events disabled");
    }
}
```

## Best Practices
- Use **LocalAvatar** for player-specific triggers, such as area entry notifications or player-activated mechanisms
- Use **LocalNPC** when you want to detect AI-controlled characters entering an area
- Use **None** to temporarily disable trigger functionality without removing the component
- Remember that SpatialTriggerEvent works similarly to Unity's standard trigger system but is optimized for Spatial's networking architecture
- For complex scenarios, you might need multiple trigger objects with different ListenFor settings
- Consider performance implications when using onTriggerStay, as it's called every frame while an entity remains in the trigger
- When working with multiplayer experiences, remember that LocalAvatar only detects the local user, not other players' avatars

## Common Use Cases
- Creating area entry/exit notifications for players
- Triggering ambient sound or visual effects when the player enters a region
- Creating invisible boundaries that perform actions when crossed
- Building tutorial sequences that progress as the player moves through designated areas
- Implementing checkpoints or save points in games
- Creating automatic doors or elevators that activate when the player approaches
- Defining zones for gameplay mechanics like capture points or safe areas
- Triggering cutscenes or narrative events when the player reaches specific locations
- Implementing trigger-based puzzles or interactive elements

## Completed: March 10, 2025
