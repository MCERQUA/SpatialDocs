## Overview
The MinMaxAttribute is used to create a dual-slider control in the Unity Inspector that lets users set minimum and maximum values within a specified range. This attribute is particularly useful for defining ranges of values, such as random spawn times, distance thresholds, or scaling limits.

## Properties
- **min**: The lower bound of the slider range
- **max**: The upper bound of the slider range
- **minDefaultVal**: The default value for the minimum slider
- **maxDefaultVal**: The default value for the maximum slider

## Usage Example
```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class EnemySpawner : MonoBehaviour
{
    // Creates a dual slider with values from 0 to 10
    // Default min value is 2, default max value is 7
    [MinMax(2.0f, 7.0f, 0.0f, 10.0f)]
    public Vector2 spawnTimeRange;
    
    // Creates a dual slider with values from 0 to 100
    // Default min value is 5, default max value is 50
    [MinMax(5.0f, 50.0f, 0.0f, 100.0f)]
    public Vector2 distanceRange;
    
    private void Start()
    {
        // Get a random spawn time between the min and max values
        float randomSpawnTime = Random.Range(spawnTimeRange.x, spawnTimeRange.y);
        
        // Start spawning enemies at random intervals
        InvokeRepeating("SpawnEnemy", randomSpawnTime, randomSpawnTime);
    }
    
    private void SpawnEnemy()
    {
        // Get a random distance within the range
        float spawnDistance = Random.Range(distanceRange.x, distanceRange.y);
        
        // Spawn logic using the random distance
        Vector3 spawnPosition = transform.position + Random.insideUnitSphere.normalized * spawnDistance;
        // ... (spawn implementation)
    }
}
```

## Best Practices
- Use MinMaxAttribute when you need to define a range of values that will be used together
- Store the result in a Vector2 where x represents the minimum value and y represents the maximum value
- Make sure the minimum value is always less than or equal to the maximum value in your code
- Choose appropriate default values that make sense for your use case
- Set a reasonable min/max range to prevent users from setting extreme values

## Common Use Cases
- Defining minimum and maximum random spawn times
- Setting distance thresholds for triggering events (min activation distance, max detection distance)
- Creating random size variations (min scale, max scale)
- Setting audio parameters like volume or pitch ranges
- Configuring time ranges for animations or effects
- Defining difficulty settings with ranges (enemy count, speed variations)
- Setting up procedural generation parameters (feature density, size variation)

## Completed: March 10, 2025