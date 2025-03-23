## Overview
SpatialAsyncOperationExtensions provides extension methods for the SpatialAsyncOperation class and its derived types. These extensions enhance the usability of async operations in Spatial by allowing for more fluent coding patterns and simplified callback registration.

## Methods

| Method | Description |
|--------|-------------|
| SetCompletedEvent<T>(T operation, Action<T> callback) | Sets the completion event for an async operation and returns the operation itself, enabling method chaining. This provides a more fluent API than using the completed event directly. |

## Usage Example

### Basic Usage

```csharp
public void SaveGameData(int score)
{
    // Without extension method
    var request = SpatialBridge.userWorldDataStoreService.SetVariable("score", score);
    request.completed += (op) => {
        Debug.Log("Score saved!");
    };
    
    // With extension method - more concise and enables method chaining
    SpatialBridge.userWorldDataStoreService.SetVariable("score", score)
        .SetCompletedEvent((op) => {
            Debug.Log("Score saved!");
        });
}
```

### Method Chaining with Strong Typing

```csharp
public void LoadPlayerProfilePicture(int actorNumber)
{
    SpatialBridge.actorService.actors[actorNumber].GetProfilePicture()
        .SetCompletedEvent((request) => {
            // The request parameter is strongly typed as ActorProfilePictureRequest
            if (request.succeeded)
            {
                // Access the texture directly because we know the type
                profileImage.texture = request.texture;
            }
            else
            {
                Debug.LogError($"Failed to load profile picture for actor {actorNumber}");
            }
        });
}
```

### Multiple Operations in Sequence

```csharp
public void ProcessGameResults(int score, string playerName)
{
    // First operation - save score
    SpatialBridge.userWorldDataStoreService.SetVariable("score", score)
        .SetCompletedEvent((scoreOp) => {
            Debug.Log("Score saved!");
            
            // Second operation - save player name after score is saved
            SpatialBridge.userWorldDataStoreService.SetVariable("playerName", playerName)
                .SetCompletedEvent((nameOp) => {
                    Debug.Log("Player name saved!");
                    
                    // Third operation - award currency after both operations complete
                    SpatialBridge.economyService.AwardWorldCurrency(100)
                        .SetCompletedEvent((currencyOp) => {
                            Debug.Log("Currency awarded!");
                        });
                });
        });
}
```

## Best Practices

- Use SetCompletedEvent instead of directly subscribing to the completed event when you want a more fluent API.
- Take advantage of the method chaining capability to create more readable code.
- Remember that the generic type parameter T preserves the specific operation type, giving you strong typing in your callbacks.
- Be careful with deeply nested callbacks as they can lead to "callback hell" - consider using coroutines for more complex sequences.
- For independent operations that don't need to be sequenced, start them in parallel rather than nesting them.

## Common Use Cases

- Registering callbacks for asynchronous operations
- Creating chains of dependent operations
- Writing more concise and readable code
- Preserving type information in callback handlers
- Handling the results of specific operation types like profile picture requests or data store operations

## Example Comparison with Coroutines

While SetCompletedEvent is excellent for simple callbacks, complex sequences might be more readable with coroutines:

```csharp
// Using nested callbacks with SetCompletedEvent
public void ComplexOperationWithCallbacks()
{
    Operation1().SetCompletedEvent((op1) => {
        Operation2().SetCompletedEvent((op2) => {
            Operation3().SetCompletedEvent((op3) => {
                // Deeply nested and can become harder to read
            });
        });
    });
}

// Equivalent code using coroutines
public IEnumerator ComplexOperationWithCoroutine()
{
    // More linear and readable for complex sequences
    yield return Operation1();
    yield return Operation2();
    yield return Operation3();
    // Logic after all operations complete
}
```

Choose the approach that makes your code most readable and maintainable for your specific scenario.

## Completed: March 10, 2025
