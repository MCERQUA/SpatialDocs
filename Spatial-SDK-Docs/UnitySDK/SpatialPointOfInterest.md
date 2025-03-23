# SpatialPointOfInterest

Category: Core Components

Interface/Class/Enum: Class

The SpatialPointOfInterest component allows developers to mark locations in a Spatial environment as points of interest. It displays informational markers and text descriptions to users when they come within specified distances of the point. This component is useful for creating guided tours, providing contextual information, highlighting features, or creating in-world documentation.

## Properties/Fields

| Property | Type | Description |
| --- | --- | --- |
| title | string | The title or name of the point of interest. |
| description | string | Detailed description text that appears when a user is close enough to the point. |
| sprite | Sprite | Optional custom sprite/icon to display as the marker. |
| spriteSize | float | Size of the sprite marker. |
| spriteRectSize | Vector2 | Rectangle size for the sprite marker. |
| markerDisplayRadius | float | Distance at which the marker becomes visible to users. |
| textDisplayRadius | float | Distance at which the title and description become visible to users. |
| backgroundColor | Color | Background color for the information panel. |
| foregroundColor | Color | Text color for the information panel. |
| titleFontOverride | TMP_FontAsset | Optional custom font for the title text. |
| descriptionFontOverride | TMP_FontAsset | Optional custom font for the description text. |
| onTextDisplayedEvent | UnityEvent | Event triggered when the text information is displayed to a user. |
| prettyName | string | Display name shown in the Unity Inspector. |
| tooltip | string | Descriptive text shown when hovering over the component in the Unity Inspector. |
| documentationURL | string | URL to documentation for this component. |

## Usage Examples

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TourManager : MonoBehaviour
{
    // Prefab with SpatialPointOfInterest component
    [SerializeField] private GameObject pointOfInterestPrefab;
    
    // Visual assets for markers
    [SerializeField] private Sprite defaultMarkerSprite;
    [SerializeField] private Sprite historicalMarkerSprite;
    [SerializeField] private Sprite technicalMarkerSprite;
    [SerializeField] private Sprite scenicMarkerSprite;
    
    // Font assets
    [SerializeField] private TMP_FontAsset defaultFont;
    [SerializeField] private TMP_FontAsset headingFont;
    
    // Colors
    [SerializeField] private Color defaultBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    [SerializeField] private Color defaultForegroundColor = Color.white;
    
    // Serializable class for defining tour points
    [System.Serializable]
    public class TourPoint
    {
        public string title;
        public string description;
        public Vector3 position;
        public MarkerType markerType;
        public float importance = 1.0f; // Affects display radii
    }
    
    public enum MarkerType
    {
        Default,
        Historical,
        Technical,
        Scenic
    }
    
    // List of tour points defined in the Inspector
    [SerializeField] private List<TourPoint> tourPoints = new List<TourPoint>();
    
    // Optional: If we want to track the created POIs
    private List<SpatialPointOfInterest> createdPOIs = new List<SpatialPointOfInterest>();
    
    private void Start()
    {
        // Create points of interest for all defined tour points
        CreateTourPoints();
    }
    
    // Create all defined tour points
    public void CreateTourPoints()
    {
        if (pointOfInterestPrefab == null)
        {
            Debug.LogError("Point of Interest prefab is missing!");
            return;
        }
        
        // Clear any existing POIs
        foreach (var poi in createdPOIs)
        {
            if (poi != null)
            {
                Destroy(poi.gameObject);
            }
        }
        createdPOIs.Clear();
        
        // Create new POIs for each tour point
        foreach (var tourPoint in tourPoints)
        {
            CreatePointOfInterest(tourPoint);
        }
        
        Debug.Log($"Created {createdPOIs.Count} points of interest.");
    }
    
    // Create a single point of interest
    private SpatialPointOfInterest CreatePointOfInterest(TourPoint tourPoint)
    {
        // Instantiate the prefab
        GameObject poiObject = Instantiate(pointOfInterestPrefab, tourPoint.position, Quaternion.identity);
        poiObject.name = $"POI - {tourPoint.title}";
        
        // Get the SpatialPointOfInterest component
        SpatialPointOfInterest poi = poiObject.GetComponent<SpatialPointOfInterest>();
        if (poi == null)
        {
            Debug.LogError("Prefab does not have a SpatialPointOfInterest component!");
            Destroy(poiObject);
            return null;
        }
        
        // Configure the point of interest
        poi.title = tourPoint.title;
        poi.description = tourPoint.description;
        
        // Set marker type based on the tour point type
        ConfigureMarkerType(poi, tourPoint.markerType);
        
        // Set display radii based on importance
        float baseMarkerRadius = 30f;
        float baseTextRadius = 10f;
        poi.markerDisplayRadius = baseMarkerRadius * tourPoint.importance;
        poi.textDisplayRadius = baseTextRadius * tourPoint.importance;
        
        // Set default colors
        poi.backgroundColor = defaultBackgroundColor;
        poi.foregroundColor = defaultForegroundColor;
        
        // Set default fonts
        poi.titleFontOverride = headingFont;
        poi.descriptionFontOverride = defaultFont;
        
        // Subscribe to the displayed event
        poi.onTextDisplayedEvent.AddListener(() => OnPointOfInterestDisplayed(poi));
        
        // Add to our tracking list
        createdPOIs.Add(poi);
        
        return poi;
    }
    
    // Configure marker appearance based on type
    private void ConfigureMarkerType(SpatialPointOfInterest poi, MarkerType markerType)
    {
        switch (markerType)
        {
            case MarkerType.Historical:
                poi.sprite = historicalMarkerSprite;
                poi.backgroundColor = new Color(0.5f, 0.3f, 0.1f, 0.8f); // Brown
                break;
                
            case MarkerType.Technical:
                poi.sprite = technicalMarkerSprite;
                poi.backgroundColor = new Color(0.1f, 0.3f, 0.5f, 0.8f); // Blue
                break;
                
            case MarkerType.Scenic:
                poi.sprite = scenicMarkerSprite;
                poi.backgroundColor = new Color(0.1f, 0.5f, 0.3f, 0.8f); // Green
                break;
                
            case MarkerType.Default:
            default:
                poi.sprite = defaultMarkerSprite;
                poi.backgroundColor = defaultBackgroundColor;
                break;
        }
        
        // Set appropriate sprite size
        poi.spriteSize = 1.5f;
        poi.spriteRectSize = new Vector2(1f, 1f);
    }
    
    // Event handler for when a POI is displayed to a user
    private void OnPointOfInterestDisplayed(SpatialPointOfInterest poi)
    {
        Debug.Log($"User is viewing point of interest: {poi.title}");
        
        // You could trigger additional events here, such as:
        // - Playing audio narration
        // - Spawning related objects
        // - Tracking tour progress
        // - Unlocking achievements
    }
    
    // Create a programmatic tour path
    public void CreateCircularTourPath(Vector3 center, float radius, int pointCount, string theme)
    {
        // Clear existing tour points
        tourPoints.Clear();
        
        // Create points in a circle
        for (int i = 0; i < pointCount; i++)
        {
            float angle = i * (360f / pointCount);
            float radian = angle * Mathf.Deg2Rad;
            
            Vector3 position = center + new Vector3(
                Mathf.Sin(radian) * radius,
                0,
                Mathf.Cos(radian) * radius
            );
            
            // Create a new tour point
            TourPoint point = new TourPoint
            {
                title = $"{theme} Point {i + 1}",
                description = $"This is point {i + 1} on the {theme} tour path.",
                position = position,
                markerType = (MarkerType)(i % 4), // Cycle through marker types
                importance = 1.0f + (i % 3) * 0.25f // Vary importance
            };
            
            tourPoints.Add(point);
        }
        
        // Create the points of interest
        CreateTourPoints();
    }
    
    // Method to highlight a specific POI (make it more noticeable)
    public void HighlightPointOfInterest(int index)
    {
        if (index < 0 || index >= createdPOIs.Count)
        {
            Debug.LogWarning($"Invalid POI index: {index}");
            return;
        }
        
        SpatialPointOfInterest poi = createdPOIs[index];
        
        // Make the marker larger
        poi.spriteSize = 2.5f;
        
        // Change the background color to make it stand out
        Color highlightColor = Color.yellow;
        highlightColor.a = 0.8f;
        poi.backgroundColor = highlightColor;
        
        // Increase the visibility radius
        poi.markerDisplayRadius *= 1.5f;
        poi.textDisplayRadius *= 1.5f;
        
        Debug.Log($"Highlighted POI: {poi.title}");
    }
    
    // Reset a highlighted POI back to normal
    public void ResetPointOfInterestHighlight(int index)
    {
        if (index < 0 || index >= createdPOIs.Count)
        {
            Debug.LogWarning($"Invalid POI index: {index}");
            return;
        }
        
        // Reconfigure the POI based on its original tour point
        TourPoint tourPoint = tourPoints[index];
        SpatialPointOfInterest poi = createdPOIs[index];
        
        // Reset sprite size
        poi.spriteSize = 1.5f;
        
        // Reset display radii
        float baseMarkerRadius = 30f;
        float baseTextRadius = 10f;
        poi.markerDisplayRadius = baseMarkerRadius * tourPoint.importance;
        poi.textDisplayRadius = baseTextRadius * tourPoint.importance;
        
        // Reset marker type configuration
        ConfigureMarkerType(poi, tourPoint.markerType);
        
        Debug.Log($"Reset POI highlight: {poi.title}");
    }
}
```

## Best Practices

1. Write clear and concise titles that quickly communicate the point of interest's significance.
2. Keep descriptions informative but brief, focusing on the most important details.
3. Use appropriate marker sprites that visually communicate the type of point (historical, scenic, technical, etc.).
4. Set appropriate visibility radii - larger for important landmarks, smaller for minor points.
5. Choose colors that provide good contrast and readability while matching your overall design theme.
6. Group related points of interest into coherent tours or categories.
7. Consider accessibility by using readable fonts and sufficient color contrast.
8. Use the onTextDisplayedEvent to trigger additional content like audio narration or animations.
9. Place POIs at appropriate heights - usually at eye level or slightly above for best visibility.
10. Avoid cluttering an area with too many points of interest, as this can overwhelm users.

## Common Use Cases

1. Creating guided tours of environments with informational stops.
2. Providing contextual information about features, artwork, or landmarks.
3. Explaining the function of interactive elements in educational spaces.
4. Highlighting historical or significant areas within a virtual environment.
5. Creating scavenger hunts or exploration activities.
6. Providing instructions at key locations for training simulations.
7. Marking viewpoints or photo opportunities in scenic environments.
8. Adding developer commentary to showcase design decisions or features.
9. Creating interactive museum-like exhibits with information cards.
10. Providing navigation assistance by marking important locations like entrances, exits, or meeting points.

## Completed: March 10, 2025