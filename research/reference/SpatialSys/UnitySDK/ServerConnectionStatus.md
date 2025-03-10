# ServerConnectionStatus

Category: Networking Service Related

Structure: Enum

Represents the connection status to the current server in the Spatial networking system. This enum is used to determine the current state of the connection to a server instance.

## Properties

| Value | Description |
| --- | --- |
| Connected | The client is fully connected to a server and can send/receive messages. |
| Connecting | The client is currently in the process of connecting to a server. |
| Disconnected | The client is not connected to any server. |
| Disconnecting | The client is currently in the process of disconnecting from a server. |

## Usage Examples

```csharp
using UnityEngine;
using SpatialSys.UnitySDK;

public class NetworkStatusManager : MonoBehaviour
{
    [SerializeField] private GameObject connectingIndicator;
    [SerializeField] private GameObject connectedIndicator;
    [SerializeField] private GameObject disconnectedIndicator;
    [SerializeField] private GameObject disconnectingIndicator;
    
    private void OnEnable()
    {
        // Subscribe to connection status changes
        SpatialBridge.networkingService.onConnectionStatusChanged += HandleConnectionStatusChanged;
        
        // Initialize with current status
        UpdateConnectionUI(SpatialBridge.networkingService.connectionStatus);
    }
    
    private void OnDisable()
    {
        // Unsubscribe when disabled
        SpatialBridge.networkingService.onConnectionStatusChanged -= HandleConnectionStatusChanged;
    }
    
    private void HandleConnectionStatusChanged(ServerConnectionStatus status)
    {
        Debug.Log($"Server connection status changed: {status}");
        
        // Update UI based on new status
        UpdateConnectionUI(status);
        
        // Perform actions based on connection state
        switch (status)
        {
            case ServerConnectionStatus.Connected:
                OnConnected();
                break;
                
            case ServerConnectionStatus.Connecting:
                OnConnecting();
                break;
                
            case ServerConnectionStatus.Disconnected:
                OnDisconnected();
                break;
                
            case ServerConnectionStatus.Disconnecting:
                OnDisconnecting();
                break;
        }
    }
    
    private void UpdateConnectionUI(ServerConnectionStatus status)
    {
        // Hide all indicators
        connectingIndicator.SetActive(false);
        connectedIndicator.SetActive(false);
        disconnectedIndicator.SetActive(false);
        disconnectingIndicator.SetActive(false);
        
        // Show the appropriate indicator
        switch (status)
        {
            case ServerConnectionStatus.Connected:
                connectedIndicator.SetActive(true);
                break;
                
            case ServerConnectionStatus.Connecting:
                connectingIndicator.SetActive(true);
                break;
                
            case ServerConnectionStatus.Disconnected:
                disconnectedIndicator.SetActive(true);
                break;
                
            case ServerConnectionStatus.Disconnecting:
                disconnectingIndicator.SetActive(true);
                break;
        }
    }
    
    private void OnConnected()
    {
        // Perform actions when connected
        Debug.Log("Successfully connected to server!");
        
        // Example: Initialize game state
        InitializeGameState();
        
        // Example: Send a join notification
        SendJoinNotification();
    }
    
    private void OnConnecting()
    {
        // Perform actions when connecting
        Debug.Log("Connecting to server...");
        
        // Example: Show loading progress or animation
        ShowConnectingProgress();
    }
    
    private void OnDisconnected()
    {
        // Perform actions when disconnected
        Debug.Log("Disconnected from server");
        
        // Example: Show reconnect button
        ShowReconnectOption();
        
        // Example: Clean up any server-dependent resources
        CleanupNetworkResources();
    }
    
    private void OnDisconnecting()
    {
        // Perform actions when disconnecting
        Debug.Log("Disconnecting from server...");
        
        // Example: Show disconnecting message
        ShowDisconnectingMessage();
    }
    
    // Example helper methods
    private void InitializeGameState() { /* Implementation */ }
    private void SendJoinNotification() { /* Implementation */ }
    private void ShowConnectingProgress() { /* Implementation */ }
    private void ShowReconnectOption() { /* Implementation */ }
    private void CleanupNetworkResources() { /* Implementation */ }
    private void ShowDisconnectingMessage() { /* Implementation */ }
}
```

## Best Practices

1. Always check the connection status before performing network operations
2. Use the `isConnected` property on `INetworkingService` as a shortcut for `connectionStatus == ServerConnectionStatus.Connected`
3. When teleporting between servers, monitor the connection status to determine when the operation is complete
4. Provide clear UI feedback to users about the current connection state
5. Design your system to handle transient connection issues gracefully
6. Implement appropriate behaviors for each connection state
7. Be aware of the restrictions in each state (e.g., most network operations require Connected status)
8. Subscribe to `onConnectionStatusChanged` to react to status changes promptly

## Common Use Cases

1. Displaying connection status indicators in the UI
2. Disabling network-dependent features when not connected
3. Showing connecting animation or progress during the Connecting state
4. Implementing reconnection logic when in the Disconnected state
5. Handling teleport operations between different server instances
6. Managing resource cleanup during the Disconnecting state
7. Creating graceful fallbacks for offline operation
8. Debugging network-related issues by monitoring status changes
9. Implementing connection-aware game logic
10. Synchronizing game start with successful connection establishment

## Completed: March 9, 2025
