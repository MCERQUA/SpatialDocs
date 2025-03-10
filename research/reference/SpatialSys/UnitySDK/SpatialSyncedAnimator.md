# SpatialSyncedAnimator

Category: Core Components

Interface/Class/Enum: Class

The SpatialSyncedAnimator component enables synchronized animation across all clients in a Spatial environment. It works with Unity's Animator component to ensure that animations play consistently for all users, regardless of when they joined the space. This component is essential for creating interactive objects with animations that need to maintain the same state for everyone in the environment.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| animator | Animator | Reference to the Unity Animator component that will be synchronized. |
| id | string | Unique identifier for this synced animator. |
| isExperimental | bool | Indicates whether this component is still in experimental status and may be subject to changes. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |
| documentationURL | string | URL to documentation for this component. |

## Methods

| Method | Description |
| --- | --- |
| SetParameter(string name, object value) | Sets a parameter on the animator. This will be synchronized with all connected users. The value type should match the parameter type in the Animator (bool, int, float, etc.). |
| SetTrigger(string name) | Sets a trigger on the animator. This will be synchronized with all connected users. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections;

public class SynchronizedAnimationController : MonoBehaviour
{
    // Reference to the SpatialSyncedAnimator component
    [SerializeField] private SpatialSyncedAnimator syncedAnimator;
    
    // Reference to the SpatialInteractable for user interaction
    [SerializeField] private SpatialInteractable interactable;
    
    // References to animated object
    [SerializeField] private GameObject animatedObject;
    
    // Animation state tracking
    private bool isDoorOpen = false;
    private bool isAnimating = false;
    
    private void Start()
    {
        // Verify references
        if (syncedAnimator == null)
        {
            syncedAnimator = GetComponent<SpatialSyncedAnimator>();
            
            if (syncedAnimator == null)
            {
                Debug.LogError("No SpatialSyncedAnimator component found!");
                return;
            }
        }
        
        if (syncedAnimator.animator == null)
        {
            Debug.LogError("Animator reference is missing in SpatialSyncedAnimator!");
            return;
        }
        
        // Set up interaction events if an interactable is available
        if (interactable != null)
        {
            interactable.onInteract.AddListener(OnObjectInteraction);
        }
        
        // Initialize animation state
        // For example, ensure the door starts in a closed state
        syncedAnimator.SetParameter("IsOpen", false);
    }
    
    // Handle user interaction with the object
    public void OnObjectInteraction()
    {
        if (isAnimating)
            return; // Don't allow interaction while already animating
            
        // Toggle the door state
        ToggleDoorState();
    }
    
    // Toggle the door between open and closed states
    private void ToggleDoorState()
    {
        isDoorOpen = !isDoorOpen;
        
        // Set the animator parameter to trigger the appropriate animation
        syncedAnimator.SetParameter("IsOpen", isDoorOpen);
        
        // Trigger the transition animation
        syncedAnimator.SetTrigger("Toggle");
        
        // Start tracking animation duration
        StartCoroutine(TrackAnimationState());
        
        Debug.Log($"Door is now {(isDoorOpen ? "opening" : "closing")}");
    }
    
    // Track animation state to prevent interaction during animation
    private IEnumerator TrackAnimationState()
    {
        isAnimating = true;
        
        // Wait for animation to complete (adjust time as needed)
        yield return new WaitForSeconds(1.5f);
        
        isAnimating = false;
    }
    
    // Example of controlling animation speed
    public void SetAnimationSpeed(float speed)
    {
        if (syncedAnimator != null)
        {
            syncedAnimator.SetParameter("Speed", speed);
            Debug.Log($"Animation speed set to {speed}");
        }
    }
    
    // Example of playing a one-shot animation
    public void PlaySpecialAnimation(string animationName)
    {
        if (syncedAnimator != null)
        {
            // Set the animation name parameter
            syncedAnimator.SetParameter("AnimationName", animationName);
            
            // Trigger the animation
            syncedAnimator.SetTrigger("PlaySpecial");
            
            Debug.Log($"Playing special animation: {animationName}");
        }
    }
    
    // Example of controlling a blend tree parameter
    public void SetBlendParameter(float blendValue)
    {
        if (syncedAnimator != null)
        {
            syncedAnimator.SetParameter("BlendValue", Mathf.Clamp01(blendValue));
            Debug.Log($"Blend value set to {blendValue}");
        }
    }
    
    // Example of controlling multiple synchronized objects
    public static void SynchronizeAnimatorGroup(SpatialSyncedAnimator[] animators, string parameterName, object value)
    {
        foreach (var animator in animators)
        {
            if (animator != null)
            {
                animator.SetParameter(parameterName, value);
            }
        }
    }
    
    // Example of sequencing animations across multiple objects
    public IEnumerator PlayAnimationSequence(SpatialSyncedAnimator[] animators, float delayBetweenAnimations)
    {
        if (animators == null || animators.Length == 0)
            yield break;
            
        for (int i = 0; i < animators.Length; i++)
        {
            if (animators[i] != null)
            {
                // Trigger animation on this animator
                animators[i].SetTrigger("Activate");
                
                // Wait before triggering the next animation
                yield return new WaitForSeconds(delayBetweenAnimations);
            }
        }
        
        Debug.Log("Animation sequence completed");
    }
    
    // Create a synchronized animation controller programmatically
    public static SpatialSyncedAnimator CreateSynchronizedAnimator(GameObject targetObject, RuntimeAnimatorController animatorController)
    {
        if (targetObject == null)
            return null;
            
        // Add an Animator component if it doesn't exist
        Animator animator = targetObject.GetComponent<Animator>();
        if (animator == null)
        {
            animator = targetObject.AddComponent<Animator>();
        }
        
        // Set the animator controller
        animator.runtimeAnimatorController = animatorController;
        
        // Add the SpatialSyncedAnimator component
        SpatialSyncedAnimator syncedAnimator = targetObject.AddComponent<SpatialSyncedAnimator>();
        syncedAnimator.animator = animator;
        syncedAnimator.id = System.Guid.NewGuid().ToString();
        
        Debug.Log($"Created synchronized animator for {targetObject.name}");
        return syncedAnimator;
    }
}
```

## Best Practices

1. Always ensure the SpatialSyncedAnimator has a reference to a valid Animator component.
2. Use unique, descriptive IDs for each SpatialSyncedAnimator to avoid conflicts and make debugging easier.
3. Structure your Animator controllers with network synchronization in mind, using parameters that are easy to control through the SetParameter method.
4. Avoid excessive parameter changes to reduce network traffic; only synchronize parameters that truly need to be synchronized.
5. For complex animations, consider using a state machine with transitions controlled by a small number of synchronized parameters rather than directly triggering many animations.
6. When designing interactive animated objects, implement a state system to prevent unwanted interactions during animations (as shown in the example with isAnimating).
7. Test your synchronized animations with multiple clients to ensure they behave consistently across different network conditions.
8. For blend trees, remember that only the blend parameters are synchronized, not the internal state of the blend tree.
9. Use the SetTrigger method sparingly, as triggers are one-shot signals and might not properly synchronize for users joining mid-animation.
10. Consider the performance impact of complex animation synchronization, especially in scenes with many animated objects.

## Common Use Cases

1. Doors, gates, drawbridges, or other interactive architecture that opens and closes.
2. Elevators, platforms, or other moving transportation elements.
3. Interactive machinery or mechanical devices that players can operate.
4. Animated puzzles with synchronized states across all players.
5. Cinematic sequences or scripted events that need to play identically for all users.
6. Interactive furniture like drawers, cabinets, or adjustable chairs.
7. Animated creatures or characters that respond to player interaction.
8. Weather or environmental effects that change gradually and need to stay synchronized.
9. Game mechanics like levers, buttons, or switches that trigger visible changes.
10. Coordinated animations across multiple objects, such as a sequence of moving platforms.

## Completed: March 10, 2025