# SpatialSeatHotspot

Category: Core Components

Interface/Class/Enum: Class

The SpatialSeatHotspot component defines a location where avatars can sit in a Spatial environment. This component allows developers to create seating areas such as chairs, benches, stools, or other objects that users can interact with to make their avatars sit down. When a user interacts with a seat hotspot, their avatar is positioned at the hotspot's location and enters a sitting pose.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| forceAvatarOrientation | bool | When true, forces the avatar to face in the same direction as the seat hotspot. |
| priority | int | Determines the priority of this seat hotspot when multiple seats are available. Higher values indicate higher priority. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |
| documentationURL | string | URL to documentation for this component. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System.Collections.Generic;

public class SeatingManager : MonoBehaviour
{
    // Collection of seat hotspots in the scene
    [SerializeField] private List<SpatialSeatHotspot> seats = new List<SpatialSeatHotspot>();
    
    // Optional UI elements
    [SerializeField] private GameObject seatOccupiedIndicator;
    [SerializeField] private Transform seatHighlightEffect;
    
    // Tracking for occupied seats
    private Dictionary<SpatialSeatHotspot, bool> seatOccupationStatus = new Dictionary<SpatialSeatHotspot, bool>();
    
    // Track currently highlighted seat
    private SpatialSeatHotspot highlightedSeat;
    
    private void Start()
    {
        // Initialize seating manager
        InitializeSeats();
    }
    
    private void Update()
    {
        // Example: Highlight nearest available seat when user presses 'H'
        if (Input.GetKeyDown(KeyCode.H))
        {
            HighlightNearestAvailableSeat();
        }
    }
    
    // Initialize seats in the scene
    private void InitializeSeats()
    {
        // Find all seats in the scene if none were specified
        if (seats.Count == 0)
        {
            SpatialSeatHotspot[] foundSeats = FindObjectsOfType<SpatialSeatHotspot>();
            seats.AddRange(foundSeats);
        }
        
        // Initialize tracking dictionary
        foreach (var seat in seats)
        {
            if (seat != null)
            {
                seatOccupationStatus[seat] = false; // Initially, all seats are unoccupied
                
                // Add interactable component if needed for interaction
                AddInteractionToSeat(seat);
            }
        }
        
        Debug.Log($"Initialized {seats.Count} seats");
    }
    
    // Add interactable component to allow interaction with seat
    private void AddInteractionToSeat(SpatialSeatHotspot seat)
    {
        // Check if seat already has an interactable component
        SpatialInteractable interactable = seat.GetComponent<SpatialInteractable>();
        if (interactable == null)
        {
            // Add interactable component to the seat
            interactable = seat.gameObject.AddComponent<SpatialInteractable>();
            interactable.title = "Sit";
            interactable.description = "Sit on this seat";
        }
        
        // Add interaction event to handle sitting
        interactable.onInteract.AddListener(() => OnSeatInteraction(seat));
    }
    
    // Handle seat interaction
    private void OnSeatInteraction(SpatialSeatHotspot seat)
    {
        // In a real implementation, the Spatial system would handle the actual sitting
        // This is a simulation for demonstration purposes
        
        // Check if seat is already occupied
        if (seatOccupationStatus.ContainsKey(seat) && seatOccupationStatus[seat])
        {
            // Seat is already occupied
            Debug.Log($"Seat {seat.name} is already occupied!");
            return;
        }
        
        // Mark seat as occupied
        seatOccupationStatus[seat] = true;
        
        // Show occupied indicator if available
        UpdateSeatVisuals(seat);
        
        Debug.Log($"Local player sat on {seat.name}");
        
        // In a real implementation, this would be handled by the Spatial SDK
        // Avatar would be moved to the seat position and the sitting animation would be played
    }
    
    // Update visuals to show seat occupation status
    private void UpdateSeatVisuals(SpatialSeatHotspot seat)
    {
        if (seatOccupiedIndicator != null)
        {
            // Create an instance of the indicator at the seat position
            GameObject indicator = Instantiate(seatOccupiedIndicator, seat.transform.position + Vector3.up * 0.5f, Quaternion.identity);
            indicator.transform.parent = seat.transform;
        }
    }
    
    // Highlight the nearest available seat to the player
    private void HighlightNearestAvailableSeat()
    {
        // Get player position (in a real implementation, this would be the local avatar position)
        Vector3 playerPosition = Camera.main.transform.position;
        
        // Find nearest available seat
        SpatialSeatHotspot nearestSeat = null;
        float nearestDistance = float.MaxValue;
        
        foreach (var seat in seats)
        {
            if (seat != null && (!seatOccupationStatus.ContainsKey(seat) || !seatOccupationStatus[seat]))
            {
                float distance = Vector3.Distance(playerPosition, seat.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestSeat = seat;
                }
            }
        }
        
        // Highlight the nearest seat if found
        if (nearestSeat != null)
        {
            HighlightSeat(nearestSeat);
        }
        else
        {
            Debug.Log("No available seats found!");
        }
    }
    
    // Highlight a specific seat
    private void HighlightSeat(SpatialSeatHotspot seat)
    {
        // Remove previous highlight
        if (highlightedSeat != null && seatHighlightEffect != null)
        {
            // Remove any existing highlight effect
            foreach (Transform child in highlightedSeat.transform)
            {
                if (child.name == "SeatHighlight")
                {
                    Destroy(child.gameObject);
                }
            }
        }
        
        // Set new highlighted seat
        highlightedSeat = seat;
        
        // Add highlight effect
        if (seatHighlightEffect != null)
        {
            Transform highlight = Instantiate(seatHighlightEffect, seat.transform.position, seat.transform.rotation);
            highlight.name = "SeatHighlight";
            highlight.parent = seat.transform;
            
            Debug.Log($"Highlighted seat: {seat.name}");
        }
    }
    
    // Get all available seats
    public List<SpatialSeatHotspot> GetAvailableSeats()
    {
        List<SpatialSeatHotspot> availableSeats = new List<SpatialSeatHotspot>();
        
        foreach (var seat in seats)
        {
            if (seat != null && (!seatOccupationStatus.ContainsKey(seat) || !seatOccupationStatus[seat]))
            {
                availableSeats.Add(seat);
            }
        }
        
        // Sort by priority
        availableSeats.Sort((a, b) => b.priority.CompareTo(a.priority));
        
        return availableSeats;
    }
    
    // Create a seat programmatically
    public SpatialSeatHotspot CreateSeat(Vector3 position, Quaternion orientation, int priority = 0)
    {
        // Create a new GameObject for the seat
        GameObject seatObject = new GameObject("Seat_" + seats.Count);
        seatObject.transform.position = position;
        seatObject.transform.rotation = orientation;
        
        // Add a SpatialSeatHotspot component
        SpatialSeatHotspot seat = seatObject.AddComponent<SpatialSeatHotspot>();
        seat.priority = priority;
        seat.forceAvatarOrientation = true;
        
        // Add collider for interaction
        BoxCollider collider = seatObject.AddComponent<BoxCollider>();
        collider.size = new Vector3(0.5f, 0.5f, 0.5f);
        collider.center = new Vector3(0, 0.25f, 0);
        
        // Add visual representation
        GameObject seatMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
        seatMesh.transform.parent = seatObject.transform;
        seatMesh.transform.localPosition = new Vector3(0, 0.25f, 0);
        seatMesh.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        
        // Add interaction component
        AddInteractionToSeat(seat);
        
        // Add to tracking lists
        seats.Add(seat);
        seatOccupationStatus[seat] = false;
        
        Debug.Log($"Created new seat at position {position}");
        return seat;
    }
    
    // Create a row of seats
    public void CreateSeatRow(Vector3 startPosition, Vector3 direction, float spacing, int count, int basePriority = 0)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 position = startPosition + direction * spacing * i;
            
            // Create seat with declining priority
            int priority = basePriority - i;
            CreateSeat(position, Quaternion.LookRotation(-direction), priority);
        }
        
        Debug.Log($"Created row of {count} seats starting at {startPosition}");
    }
    
    // Reset occupation status (e.g. when scene resets)
    public void ResetAllSeats()
    {
        foreach (var seat in seats)
        {
            if (seat != null)
            {
                seatOccupationStatus[seat] = false;
                
                // Remove any occupation indicators
                foreach (Transform child in seat.transform)
                {
                    if (child.name.Contains("Indicator") || child.name == "SeatHighlight")
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }
        
        Debug.Log("Reset all seats to unoccupied state");
    }
}
```

## Best Practices

1. Position seat hotspots at the exact location where you want avatars to sit, with the forward direction aligned to the direction the seated avatar should face.
2. Use the forceAvatarOrientation property to ensure avatars face the correct direction when seated, especially for benches or chairs with a clear front.
3. Set appropriate priorities for seats to control which seats are preferred when multiple options are available.
4. For rows of seats (like in theaters or classrooms), assign higher priorities to seats that provide better views or experiences.
5. Include visual indicators like seat cushions or chair meshes to make it clear to users where they can sit.
6. Add SpatialInteractable components to seat hotspots to provide a clear interaction point for users.
7. Add appropriate colliders to ensure users can easily select and interact with seats.
8. For special seating areas (like VIP sections), consider using distinct visual designs and higher priorities.
9. Test your seating arrangements with multiple avatars to ensure proper spacing and avoid clipping issues.
10. For seating areas like couches where multiple users might sit, place multiple seat hotspots with appropriate spacing.

## Common Use Cases

1. Chairs and benches for resting areas or social spaces.
2. Theater or auditorium seating for presentations or performances.
3. Classroom seating arrangements for educational spaces.
4. Vehicle seating for cars, buses, or other transportation.
5. Dining areas with tables and chairs.
6. Waiting room or lobby seating.
7. Park benches or outdoor seating areas.
8. VIP or special seating areas for events.
9. Gaming setups with chairs or couches.
10. Stadium or arena seating for sporting events or concerts.

## Completed: March 10, 2025