# NetworkingRemoteEventArgs

Category: Networking Service Related

Structure: Struct

Arguments for the `onEvent` callback in [`INetworkingRemoteEventsService`](./INetworkingRemoteEventsService.md). Contains information about a received remote networking event.

## Properties

| Property | Description |
| --- | --- |
| eventID | The event ID (byte) that identifies the type of event being received. |
| eventArgs | The event arguments sent with the event. These are the parameters passed to the RaiseEvent method. |
| senderActor | The actor number of the sender who raised this event. |

## Usage Examples

```csharp
// Define event IDs as constants for clarity
private const byte EVENT_PLAYER_ACTION = 1;
private const byte EVENT_GAME_STATE = 2;
private const byte EVENT_CHAT_MESSAGE = 3;

private void OnEnable()
{
    // Subscribe to network events
    SpatialBridge.networkingService.remoteEvents.onEvent += HandleNetworkEvent;
}

private void OnDisable()
{
    // Always unsubscribe when disabled
    SpatialBridge.networkingService.remoteEvents.onEvent -= HandleNetworkEvent;
}

private void HandleNetworkEvent(NetworkingRemoteEventArgs args)
{
    // Determine which type of event was received by checking the eventID
    switch (args.eventID)
    {
        case EVENT_PLAYER_ACTION:
            HandlePlayerAction(args);
            break;
            
        case EVENT_GAME_STATE:
            HandleGameState(args);
            break;
            
        case EVENT_CHAT_MESSAGE:
            HandleChatMessage(args);
            break;
            
        default:
            Debug.LogWarning($"Received unknown event ID: {args.eventID}");
            break;
    }
}

private void HandlePlayerAction(NetworkingRemoteEventArgs args)
{
    // Extract arguments from the event
    string actionType = (string)args.eventArgs[0];
    Vector3 position = (Vector3)args.eventArgs[1];
    
    // Get the actor who sent the event
    IActor sender = SpatialBridge.actorService.actors[args.senderActor];
    
    Debug.Log($"Player {sender.displayName} performed action '{actionType}' at position {position}");
    
    // Perform the appropriate response to the player action
    // ...
}

private void HandleGameState(NetworkingRemoteEventArgs args)
{
    // Extract game state information
    int stateID = (int)args.eventArgs[0];
    float stateTime = (float)args.eventArgs[1];
    
    // Update the local game state
    UpdateGameState(stateID, stateTime);
}

private void HandleChatMessage(NetworkingRemoteEventArgs args)
{
    // Extract message information
    string message = (string)args.eventArgs[0];
    bool isTeamChat = (bool)args.eventArgs[1];
    
    // Get the sender information
    IActor sender = SpatialBridge.actorService.actors[args.senderActor];
    
    // Display the chat message in the appropriate chat interface
    DisplayChatMessage(sender.displayName, message, isTeamChat);
}

// Example of sending events that others would receive
private void SendPlayerAction(string actionType, Vector3 position)
{
    // Send a player action event to all other players
    SpatialBridge.networkingService.remoteEvents.RaiseEventOthers(EVENT_PLAYER_ACTION, actionType, position);
}

private void BroadcastGameState(int stateID, float stateTime)
{
    // Send the game state to all players (including self)
    SpatialBridge.networkingService.remoteEvents.RaiseEventAll(EVENT_GAME_STATE, stateID, stateTime);
}

private void SendChatMessage(string message, bool isTeamChat)
{
    // Send a chat message to everyone
    SpatialBridge.networkingService.remoteEvents.RaiseEventAll(EVENT_CHAT_MESSAGE, message, isTeamChat);
}
```

## Best Practices

1. Use unique byte constants for event IDs to avoid conflicts and improve readability
2. Group related events together with similar ID values (e.g., all player actions from 1-10)
3. Always check the event ID before processing event arguments
4. Type cast event arguments carefully, as unexpected types will cause runtime exceptions
5. Keep event payloads small to minimize network traffic
6. Use a switch statement or event map to handle different event types cleanly
7. Remember that RaiseEventAll will trigger the event locally too, while RaiseEventOthers will not
8. Consider creating a dedicated event handler class for complex applications with many network events
9. Document event IDs and their expected parameter types for team collaboration

## Common Use Cases

1. Multiplayer game synchronization (player actions, game state, scores)
2. Chat systems for communication between users
3. Collaborative tools where changes need to be communicated to other users
4. Real-time notifications about events in the virtual space
5. Synchronized interactive elements like buttons, levers, or doors
6. Multi-user creation/editing tools
7. Voting systems or polls
8. Synchronized timers or countdowns
9. Player emotes or gestures
10. Triggering effects or animations visible to multiple users

## Completed: March 9, 2025
