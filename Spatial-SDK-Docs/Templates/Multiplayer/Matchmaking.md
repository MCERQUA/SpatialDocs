# Matchmaking (Lobby System) Template

## Overview
The Matchmaking (Lobby System) template provides a complete implementation of a multiplayer matchmaking and lobby system for Spatial experiences. It enables users to create, join, and manage game rooms with customizable game settings, player limits, and team assignments. The template handles all aspects of multiplayer session management, from room discovery to player synchronization, allowing developers to focus on their core game mechanics rather than network infrastructure.

## Features
- **Room Management**: Create, join, and leave multiplayer game rooms
- **Room Discovery**: Browse available public rooms with filtering options
- **Room Settings**: Customize game modes, maps, time limits, and other parameters
- **Player Management**: Track player status, readiness, and team assignments
- **Team System**: Assign and balance players across multiple teams
- **Host Migration**: Handle host disconnection gracefully with automatic host transfer
- **Match Starting**: Coordinate synchronized game start when all players are ready
- **Private Rooms**: Password protection for private lobbies
- **Player Limits**: Configurable minimum and maximum player counts
- **UI System**: Complete lobby interface with room creation, browser, and player management

## Included Components

### 1. Lobby Manager
Central system for coordinating all matchmaking functionality:
- Room creation and configuration interface
- Room discovery and filtering
- Joining and leaving room management
- Player status synchronization
- Match start coordination
- Network message handling
- Session persistence

### 2. Room Browser
Complete system for discovering and joining existing game rooms:
- List view of available rooms with details
- Filtering options (game mode, player count, etc.)
- Room sorting by various criteria
- Refresh functionality to update available rooms
- Direct join via room code
- Status indicators for rooms in progress

### 3. Team Management
Tools for organizing players into balanced teams:
- Team assignment interface
- Auto-balance functionality
- Team switching permissions
- Team-based player listing
- Team readiness tracking
- Team customization options

### 4. UI Framework
Comprehensive UI elements for the entire lobby experience:
- Main menu for entry points
- Room creation panel with game settings
- Room browser with search and filters
- Lobby room with player management
- Team assignment interface
- Ready state controls
- Match countdown and transition

## Integration with SDK Components
The template integrates with these key SDK components:

| SDK Component | Usage in Template |
|---------------|-------------------|
| INetworkingService | Core networking for room management and player synchronization |
| IActorService | Player tracking and identification |
| ISpaceContentService | Scene loading for different game modes |
| NetworkObject | Synchronized objects for lobby state |
| NetworkVariable | State synchronization across clients |
| RPC | Function calls between clients |
| IUserWorldDataStoreService | Optional match history and player statistics |

## When to Use
Use this template when:
- Creating multiplayer games that require organized matchmaking
- Implementing team-based multiplayer experiences
- Building competitive games with lobby configuration
- Developing games where players need to coordinate before starting
- Creating experiences with different game modes or maps to select
- Implementing tournaments or bracketed competitions
- Building experiences where players need to see available rooms

## Implementation Details

### Lobby Manager Implementation
The core lobby management system:

```csharp
public class MatchmakingLobbyManager : NetworkBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private int minPlayers = 2;
    [SerializeField] private int maxPlayers = 8;
    [SerializeField] private float lobbyCountdownTime = 5f;
    
    [Header("UI References")]
    [SerializeField] private LobbyUIController uiController;
    
    // Network variables for synchronization
    public NetworkVariable<MatchState> currentMatchState = new NetworkVariable<MatchState>(MatchState.WaitingForPlayers);
    public NetworkVariable<float> startCountdown = new NetworkVariable<float>(0f);
    public NetworkVariable<string> roomName = new NetworkVariable<string>("New Game");
    public NetworkVariable<string> gameMode = new NetworkVariable<string>("Default");
    public NetworkVariable<string> mapName = new NetworkVariable<string>("Default Map");
    public NetworkVariable<int> maxPlayersInRoom = new NetworkVariable<int>(8);
    public NetworkVariable<bool> isPrivate = new NetworkVariable<bool>(false);
    
    // Local player data
    private bool isLocalPlayerReady = false;
    private int localPlayerTeam = 0;
    
    // Player tracking
    private Dictionary<int, PlayerInfo> connectedPlayers = new Dictionary<int, PlayerInfo>();
    
    void Start()
    {
        // Initialize manager
        if (SpatialBridge.networkingService.IsServer)
        {
            // Register with room directory when hosting
            RegisterRoom();
        }
        
        // Register local player
        RegisterLocalPlayer();
        
        // Set up UI
        if (uiController != null)
        {
            uiController.Initialize(this);
        }
    }
    
    void Update()
    {
        // Handle countdown if we're the server
        if (SpatialBridge.networkingService.IsServer && currentMatchState.Value == MatchState.Countdown)
        {
            startCountdown.Value -= Time.deltaTime;
            
            if (startCountdown.Value <= 0)
            {
                StartMatch();
            }
        }
    }
    
    private void RegisterRoom()
    {
        // In a real implementation, this would register the room
        // with Spatial's matchmaking service
    }
    
    [Rpc(RpcTarget.Server)]
    public void SetPlayerReady(bool isReady)
    {
        int senderId = RpcSender.ActorId;
        
        if (connectedPlayers.TryGetValue(senderId, out PlayerInfo playerInfo))
        {
            playerInfo.isReady = isReady;
            connectedPlayers[senderId] = playerInfo;
            
            // Notify all clients of the player state change
            UpdatePlayerState(senderId, playerInfo);
            
            // Check if all players are ready to start
            CheckAllPlayersReady();
        }
    }
    
    [Rpc(RpcTarget.All)]
    private void UpdatePlayerState(int playerId, PlayerInfo playerInfo)
    {
        // Update local player tracking
        connectedPlayers[playerId] = playerInfo;
        
        // Update UI
        if (uiController != null)
        {
            uiController.UpdatePlayerList(connectedPlayers);
        }
    }
    
    private void CheckAllPlayersReady()
    {
        // Only the server should check if all players are ready
        if (!SpatialBridge.networkingService.IsServer)
            return;
            
        if (connectedPlayers.Count < minPlayers)
            return;
            
        bool allReady = true;
        
        foreach (var player in connectedPlayers.Values)
        {
            if (!player.isReady)
            {
                allReady = false;
                break;
            }
        }
        
        if (allReady)
        {
            // All players are ready, start countdown
            currentMatchState.Value = MatchState.Countdown;
            startCountdown.Value = lobbyCountdownTime;
        }
    }
    
    [Rpc(RpcTarget.All)]
    private void StartMatch()
    {
        currentMatchState.Value = MatchState.InProgress;
        
        // Load the appropriate game scene based on settings
        string sceneName = GetSceneNameFromGameMode();
        
        // In a real implementation, this would use Spatial's scene loading
        // system to transition to the actual game
    }
    
    private string GetSceneNameFromGameMode()
    {
        // Map game mode to scene name
        switch (gameMode.Value)
        {
            case "Deathmatch":
                return "DeathmatchScene";
            case "Capture the Flag":
                return "CTFScene";
            case "King of the Hill":
                return "KOTHScene";
            default:
                return "DefaultGameScene";
        }
    }
    
    [Rpc(RpcTarget.Server)]
    public void RequestTeamChange(int teamId)
    {
        int senderId = RpcSender.ActorId;
        
        if (connectedPlayers.TryGetValue(senderId, out PlayerInfo playerInfo))
        {
            // Check if team change is valid
            if (IsTeamChangeValid(senderId, playerInfo.teamId, teamId))
            {
                playerInfo.teamId = teamId;
                connectedPlayers[senderId] = playerInfo;
                
                // Notify all clients of the team change
                UpdatePlayerState(senderId, playerInfo);
            }
        }
    }
    
    private bool IsTeamChangeValid(int playerId, int currentTeam, int newTeam)
    {
        // Implement team balancing logic
        // For example, prevent team changes that would make teams too unbalanced
        
        // Simple implementation: always allow changes
        return true;
    }
    
    [Rpc(RpcTarget.Server)]
    public void UpdateGameSettings(string newGameMode, string newMapName, int newMaxPlayers, bool newIsPrivate)
    {
        // Only the host should be able to change game settings
        if (RpcSender.ActorId != GetHostId())
            return;
            
        gameMode.Value = newGameMode;
        mapName.Value = newMapName;
        maxPlayersInRoom.Value = Mathf.Clamp(newMaxPlayers, minPlayers, maxPlayers);
        isPrivate.Value = newIsPrivate;
    }
    
    private int GetHostId()
    {
        // In a real implementation, this would return the actor ID of the host
        // Spatial's networking system would provide this information
        return 0;
    }
    
    private void RegisterLocalPlayer()
    {
        var localActor = SpatialBridge.actorService.localActor;
        
        PlayerInfo localPlayerInfo = new PlayerInfo
        {
            actorId = localActor.actorNumber,
            displayName = localActor.displayName,
            teamId = 0, // Default team
            isReady = false,
            isHost = SpatialBridge.networkingService.IsServer
        };
        
        // Register with server
        RegisterPlayerWithServer(localPlayerInfo);
    }
    
    [Rpc(RpcTarget.Server)]
    private void RegisterPlayerWithServer(PlayerInfo playerInfo)
    {
        int senderId = RpcSender.ActorId;
        
        // Override actor ID with the sender's ID to prevent spoofing
        playerInfo.actorId = senderId;
        
        // Add player to tracking
        connectedPlayers[senderId] = playerInfo;
        
        // Notify all clients of the new player
        NotifyPlayerJoined(senderId, playerInfo);
        
        // If this is a new player, send them the current player list
        SynchronizePlayerList(senderId);
    }
    
    [Rpc(RpcTarget.All)]
    private void NotifyPlayerJoined(int playerId, PlayerInfo playerInfo)
    {
        // Add player to local tracking
        connectedPlayers[playerId] = playerInfo;
        
        // Update UI
        if (uiController != null)
        {
            uiController.UpdatePlayerList(connectedPlayers);
        }
    }
    
    [Rpc(RpcTarget.Target)]
    private void SynchronizePlayerList(int targetPlayerId)
    {
        // Send the entire player list to a specific player
        foreach (var playerEntry in connectedPlayers)
        {
            UpdatePlayerState(playerEntry.Key, playerEntry.Value);
        }
    }
}

// Data structure for tracking player information
[Serializable]
public struct PlayerInfo
{
    public int actorId;
    public string displayName;
    public int teamId;
    public bool isReady;
    public bool isHost;
}

// Match states
public enum MatchState
{
    WaitingForPlayers,
    Countdown,
    InProgress,
    Completed
}
```

### Room Browser Implementation
The system for discovering and joining existing game rooms:

```csharp
public class RoomBrowserManager : MonoBehaviour
{
    [SerializeField] private RoomBrowserUI uiController;
    [SerializeField] private float refreshInterval = 10f;
    
    private List<RoomInfo> availableRooms = new List<RoomInfo>();
    private float refreshTimer;
    
    void Start()
    {
        refreshTimer = refreshInterval;
        
        // Initial refresh
        RefreshRoomList();
    }
    
    void Update()
    {
        refreshTimer -= Time.deltaTime;
        
        if (refreshTimer <= 0)
        {
            RefreshRoomList();
            refreshTimer = refreshInterval;
        }
    }
    
    public void RefreshRoomList()
    {
        // In a real implementation, this would query Spatial's room directory
        // for the current list of available rooms
        
        // For now, simulate room discovery
        FetchAvailableRooms();
    }
    
    private async void FetchAvailableRooms()
    {
        // Simulated room discovery
        // In a real implementation, this would use SpatialBridge.networkingService
        // to get actual room information
        
        // Update UI with the refreshed room list
        if (uiController != null)
        {
            uiController.UpdateRoomList(availableRooms);
        }
    }
    
    public void JoinRoom(string roomId)
    {
        // In a real implementation, this would use SpatialBridge.networkingService
        // to join the specified room
        
        // For private rooms, this would check passwords
        
        // After successful join, transition to the lobby scene
    }
    
    public void CreateRoom()
    {
        // In a real implementation, this would use SpatialBridge.networkingService
        // to create a new room with the specified settings
        
        // After successful creation, transition to the lobby scene as host
    }
    
    public void FilterRooms(string gameMode, int minPlayers, int maxPlayers, bool hideFullRooms, bool hideInProgress)
    {
        // Apply filters to the room list
        List<RoomInfo> filteredRooms = availableRooms.Where(room => 
        {
            // Apply all filter criteria
            if (!string.IsNullOrEmpty(gameMode) && room.gameMode != gameMode)
                return false;
                
            if (room.playerCount < minPlayers)
                return false;
                
            if (hideFullRooms && room.playerCount >= room.maxPlayers)
                return false;
                
            if (hideInProgress && room.state == MatchState.InProgress)
                return false;
                
            return true;
        }).ToList();
        
        // Update UI with filtered list
        if (uiController != null)
        {
            uiController.UpdateRoomList(filteredRooms);
        }
    }
}

// Data structure for room information
[Serializable]
public struct RoomInfo
{
    public string roomId;
    public string roomName;
    public string gameMode;
    public string mapName;
    public int playerCount;
    public int maxPlayers;
    public bool isPrivate;
    public MatchState state;
}
```

### Team Management Implementation
System for handling team assignments and balancing:

```csharp
public class TeamManager : NetworkBehaviour
{
    [SerializeField] private int teamCount = 2;
    [SerializeField] private int maxPlayersPerTeam = 4;
    
    private Dictionary<int, List<int>> teamPlayerCounts = new Dictionary<int, List<int>>();
    
    void Start()
    {
        // Initialize team structures
        for (int i = 0; i < teamCount; i++)
        {
            teamPlayerCounts[i] = new List<int>();
        }
    }
    
    public bool AssignPlayerToTeam(int actorId, int teamId)
    {
        // Validate team ID
        if (teamId < 0 || teamId >= teamCount)
            return false;
            
        // Check if team is full
        if (teamPlayerCounts[teamId].Count >= maxPlayersPerTeam)
            return false;
            
        // Remove player from current team if any
        RemovePlayerFromAllTeams(actorId);
        
        // Add to new team
        teamPlayerCounts[teamId].Add(actorId);
        
        return true;
    }
    
    private void RemovePlayerFromAllTeams(int actorId)
    {
        foreach (var team in teamPlayerCounts)
        {
            team.Value.Remove(actorId);
        }
    }
    
    public int GetTeamForPlayer(int actorId)
    {
        foreach (var team in teamPlayerCounts)
        {
            if (team.Value.Contains(actorId))
            {
                return team.Key;
            }
        }
        
        return -1; // Not assigned to any team
    }
    
    public bool AreTeamsBalanced()
    {
        int minPlayers = int.MaxValue;
        int maxPlayers = 0;
        
        foreach (var team in teamPlayerCounts)
        {
            int count = team.Value.Count;
            minPlayers = Mathf.Min(minPlayers, count);
            maxPlayers = Mathf.Max(maxPlayers, count);
        }
        
        // Teams are balanced if the difference between the largest and smallest team is at most 1
        return maxPlayers - minPlayers <= 1;
    }
    
    public void AutoBalanceTeams()
    {
        // Get all players
        List<int> allPlayers = new List<int>();
        foreach (var team in teamPlayerCounts)
        {
            allPlayers.AddRange(team.Value);
        }
        
        // Clear teams
        foreach (var team in teamPlayerCounts)
        {
            team.Value.Clear();
        }
        
        // Shuffle players
        allPlayers = allPlayers.OrderBy(x => UnityEngine.Random.value).ToList();
        
        // Redistribute
        for (int i = 0; i < allPlayers.Count; i++)
        {
            int teamId = i % teamCount;
            teamPlayerCounts[teamId].Add(allPlayers[i]);
        }
    }
    
    public int GetTeamPlayerCount(int teamId)
    {
        if (teamId < 0 || teamId >= teamCount)
            return 0;
            
        return teamPlayerCounts[teamId].Count;
    }
    
    public int GetOptimalTeamForNewPlayer()
    {
        // Find the team with the fewest players
        int minTeamId = 0;
        int minCount = int.MaxValue;
        
        foreach (var team in teamPlayerCounts)
        {
            if (team.Value.Count < minCount)
            {
                minCount = team.Value.Count;
                minTeamId = team.Key;
            }
        }
        
        return minTeamId;
    }
}
```

## Best Practices
- **Room Discoverability**: Use clear, descriptive room names that convey gameplay style
- **Room Settings**: Include enough customization options without overwhelming users
- **UI Clarity**: Ensure the matchmaking UI is intuitive with clear status indicators
- **Team Balance**: Implement automatic team balancing or restrictions to prevent uneven teams
- **Player Communication**: Consider adding pre-game chat or player status indicators
- **Connection Quality**: Display connection quality warnings for potential high-latency players
- **Error Handling**: Gracefully handle disconnections and rejoin attempts
- **Host Migration**: Always implement host migration to prevent session loss if the host leaves
- **Private Rooms**: Offer private room options for friends to play together
- **Performance**: Limit network synchronization to essential lobby data
- **Timeouts**: Implement idle timeouts to prevent inactive lobbies from persisting
- **Ready Flow**: Make the ready/unready process clear and responsive
- **Status Updates**: Provide clear visual feedback for room status changes

## Related Templates
- [Top Down Shooter](../Games/TopDownShooter.md) - For implementing team-based shooter gameplay
- [Gem Collection Game](../Games/GemCollectionGame.md) - For creating multiplayer collection challenges
- [HTTP Request Demo](../Technical/HTTPRequestDemo.md) - For implementing external API integration with matchmaking servers

## Additional Resources
- [GitHub Repository](https://github.com/spatialsys/spatial-matchmaking-template)
- [Spatial Networking Service Documentation](../../SpatialSys/UnitySDK/INetworkingService.md)
- [Network Variables Documentation](../../SpatialSys/UnitySDK/NetworkVariable.md)
- [RPC Documentation](../../SpatialSys/UnitySDK/Rpc.md)
