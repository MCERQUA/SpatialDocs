# Spatial Daily Rewards Sample Template

## Template Overview
The Spatial Daily Rewards Sample template demonstrates how to implement a comprehensive daily rewards and missions system using Spatial's data persistence capabilities. The template includes implementations for day streak rewards, daily missions, and weekly missions, along with a UI system to display and claim rewards.

## Template Information
- Location: `E:\1-ECHO-WORKING-FOLDER\Spatial-Templates\spatial-example-daily-rewards`
- Repository Structure: The template is organized as a Unity project with all necessary scripts and assets

## Directory Structure
```
E:\1-ECHO-WORKING-FOLDER\Spatial-Templates\spatial-example-daily-rewards\
└── spatial-example-daily-rewards-unity\
    └── Assets\
        └── DailyRewards\
            ├── Animation\
            ├── Materials\
            ├── Prefabs\
            ├── Scenes\
            ├── Scripts\
            │   ├── DataStoreManager.cs
            │   ├── DateTimeUtilities.cs
            │   ├── GameManager.cs
            │   ├── RewardDataManager.cs
            │   ├── Debug\
            │   ├── RewardData\
            │   │   ├── Data_DailyMission\
            │   │   ├── Data_DailyStreak\
            │   │   ├── Data_WeeklyMission\
            │   │   ├── RewardDataGroupObject.cs
            │   │   ├── RewardDataObject.cs
            │   │   ├── RewardType.cs
            │   │   └── RequirementType.cs
            │   ├── UI\
            │   │   ├── RewardButtonView.cs
            │   │   ├── RewardUnclaimedView.cs
            │   │   ├── UIManager.cs
            │   │   └── UIReward.cs
            │   └── Utilities\
            ├── Shaders\
            └── Thumbnails\
```

## C# Scripts
### Core System Scripts

#### GameManager.cs
```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DailyRewards
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public const int DATE_TIME_OFFSET = 19; // Change day at 7 PM
        public const DayOfWeek DATE_RESET_WEEKLY = DayOfWeek.Friday; // Reset weekly at Friday
        private const int SECOND_TO_NEXT_DAY = 86400; // 24 * 60 * 60
        private const int SECOND_TO_NEXT_WEEK = 604800; // 7 * 24 * 60 * 60

        [Header("Game Data")]
        public static int score = 0;
        public static int dayStreak = 1;
        public static int totalClicksDay = 0;
        public static int totalClicksWeek = 0;
        public static DateTime dateTimeLastLogin;

        public static Action onDataStoreLoaded;

        [Header("References")]
        [SerializeField] private DataStoreManager _dataStoreManager;
        [SerializeField] private RewardDataManager _rewardDataManager;
        [SerializeField] private UIManager _uiManager;

        // Game initialization, data loading, and core game loop
        // ...

        // Reset daily and weekly data when needed
        // ...

        // Handle reward claiming
        public static void ClaimReward(RewardType rewardType, RewardDataObject reward)
        {
            _instance.ClaimRewardInternal(rewardType, reward);
        }
        
        // Other methods omitted for brevity
    }
}
```

**Purpose**: Central manager for the daily rewards system. Handles time tracking, reward status, and data persistence.

**Spatial SDK Usage**: 
- Indirectly uses `SpatialBridge.userWorldDataStoreService` through DataStoreManager

**Key Features**:
- Tracks user login times to calculate day streaks
- Manages daily and weekly reset intervals
- Handles reward claiming and progress tracking
- Sets up automatic time-based resets using coroutines

#### DataStoreManager.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;
using System;

namespace DailyRewards
{
    // Save/Load data from Spatial UserWorldDataStoreService
    // https://docs.spatial.io/data-store#block-c5a2364413f24c0ea59e34aa78dd2488
    public class DataStoreManager : MonoBehaviour
    {
        private const string DATA_KEY_SCORE = "rwd/score"; // int
        private const string DATA_KEY_TOTAL_CLICKS_DAY = "rwd/total_clicks_day"; // int
        private const string DATA_KEY_TOTAL_CLICKS_WEEK = "rwd/total_clicks_week"; // int
        private const string DATA_KEY_DATE_TIME_LAST_LOGIN = "rwd/date_time_last_login"; // datetime
        private const string DATA_KEY_DAY_STREAK = "rwd/day_streak"; // int

        private const string DATA_KEY_DAY_STREAK_CLAIMED = "rwd/day_streak_claimed"; // List<bool>
        private const string DATA_KEY_DAILY_MISSION_CLAIMED = "rwd/daily_mission_claimed"; // List<bool>
        private const string DATA_KEY_WEEKLY_MISSION_CLAIMED = "rwd/weekly_mission_claimed"; // List<bool>

        public enum DataName
        {
            Score,
            TotalClicksDay,
            TotalClicksWeek,
            DateTimeLastLogin,
            DayStreak,
            DayStreakClaimed,
            DailyMissionClaimed,
            WeeklyMissionClaimed,
        }
        
        // Methods to save and load data from Spatial's data store
        // ...
    }
}
```

**Purpose**: Handles saving and loading data to/from Spatial's user data store service.

**Spatial SDK Usage**: 
- Direct use of `SpatialBridge.userWorldDataStoreService.SetVariable` for saving data
- Direct use of `SpatialBridge.userWorldDataStoreService.GetVariable` for loading data
- Direct use of `SpatialBridge.userWorldDataStoreService.HasVariable` for checking if data exists

**Key Features**:
- Persistent storage for player scores, day streaks, and mission progress
- Robust error handling and retry mechanism for failed data loads
- Organization of data keys with prefixes for namespacing

#### DateTimeUtilities.cs
```csharp
using System;

namespace DailyRewards
{
    public class DateTimeUtilities
    {
        public static int GetMinutePassed(DateTime dateTime)
        {
            return (int)(DateTime.Now - dateTime).TotalMinutes;
        }

        // Apply offsetHour so that the day starts at the specified hour
        public static int GetDayPassed(DateTime dateTime, int offsetHour)
        {
            return GetDayPassed(DateTime.Now, dateTime, offsetHour);
        }
        
        // Additional utility methods for time calculations
        // ...
    }
}
```

**Purpose**: Provides utility functions for datetime calculations relevant to daily rewards systems.

**Spatial SDK Usage**: None (pure utility class)

**Key Features**:
- Calculate days passed between dates with custom day boundaries
- Determine when the next day or week starts
- Check if a user has crossed into a new week
- Support for custom week start days

### Reward Data Scripts

#### RewardDataManager.cs
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DailyRewards
{
    public class RewardDataManager : MonoBehaviour
    {
        private static RewardDataManager _instance;

        [SerializeField] private RewardDataGroupObject _rewardGroupDayStreak;
        [SerializeField] private RewardDataGroupObject _rewardGroupDaily;
        [SerializeField] private RewardDataGroupObject _rewardGroupWeekly;

        private static Dictionary<RewardType, RewardDataGroupObject> _rewardDataGroupByType;

        // Methods to manage reward data and status
        // ...
        
        // Update each data status: Claimed / Achieved / Available
        // By checking the progress of each reward in IsRequirementMet()
        public void UpdateRewardDataStatus(RewardType rewardType)
        {
            RewardDataGroupObject rewardDataGroupObject = GetRewardDataGroupByType(rewardType);
            RewardDataObject[] rewards = rewardDataGroupObject.rewards;
            for (int i = 0; i < rewards.Length; i++)
            {
                RewardDataObject reward = rewards[i];
                if (rewardDataGroupObject.GetRewardClaimedSafe(reward))
                {
                    reward.status = RewardDataObject.Status.Claimed;
                }
                else
                {
                    if (IsRequirementMet(reward))
                    {
                        reward.status = RewardDataObject.Status.Achieved;
                    }
                    else
                    {
                        reward.status = RewardDataObject.Status.Available;
                    }
                }
            }
        }
        
        // Additional methods omitted for brevity
    }
}
```

**Purpose**: Manages the different types of rewards and their status.

**Spatial SDK Usage**: None (uses GameManager data)

**Key Features**:
- Tracks three reward types: Day Streaks, Daily Missions, and Weekly Missions
- Updates reward status based on player progress
- Handles reward claiming logic
- Provides access to reward data

#### RewardDataObject.cs
```csharp
using UnityEngine;

namespace DailyRewards
{
    [CreateAssetMenu(fileName = "RewardData_", menuName = "DailyRewards/RewardData", order = 1)]
    public class RewardDataObject : ScriptableObject
    {
        public enum Status
        {
            None,
            Available,
            Achieved,
            Claimed,
        }
        public string id;
        public string title;
        public RequirementType requirementType;
        public int requirement;
        public int rewardAmount;
        public Status status;
    }
}
```

**Purpose**: ScriptableObject that defines individual rewards.

**Spatial SDK Usage**: None

**Key Features**:
- Defines reward properties like title, requirement type, and amount
- Tracks reward status (Available, Achieved, Claimed)
- Used to create reward definitions in the Unity Editor

#### RewardType.cs
```csharp
namespace DailyRewards
{
    public enum RewardType
    {
        None,
        DayStreak,
        DailyMissions,
        WeeklyMissions,
    }
}
```

**Purpose**: Defines the types of rewards available in the system.

#### RequirementType.cs
```csharp
namespace DailyRewards
{
    public enum RequirementType
    {
        None,
        DayStreak,
        TotalClicksDay,
        TotalClicksWeek,
    }
}
```

**Purpose**: Defines the types of requirements for rewards.

### UI Scripts

#### UIReward.cs
```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DailyRewards
{
    public class UIReward : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private GameObject _sideButtonRoot;
        [SerializeField] private GameObject _loadingImage;

        [Header("Side Buttons")]
        [SerializeField] private Button _buttonDayStreak; // Day-Streak
        [SerializeField] private Button _buttonDaily; // Daily Reward
        [SerializeField] private Button _buttonWeekly; // Weekly Reward
        [SerializeField] private RewardUnclaimedView _rewardUnclaimedViewDayStreak;
        [SerializeField] private RewardUnclaimedView _rewardUnclaimedViewDaily;
        [SerializeField] private RewardUnclaimedView _rewardUnclaimedViewWeekly;

        [Header("Reward List Window")]
        [SerializeField] private GameObject _rootListWindow;
        [SerializeField] private Transform _content;
        [SerializeField] private GameObject _buttonViewReference;
        
        // Methods for UI management
        // ...
    }
}
```

**Purpose**: Manages the reward UI system, including buttons and windows for displaying rewards.

**Spatial SDK Usage**: None (interfaces with GameManager and RewardDataManager)

**Key Features**:
- Displays daily, weekly, and streak rewards in UI
- Shows unclaimed reward counts
- Handles reward claiming through UI interaction
- Uses object pooling for reward buttons

## Key System Features

### 1. Day Streak Tracking
- Tracks consecutive days the player logs in
- Can be configured to change "day" at a specific hour
- Rewards players for consecutive logins
- Resets streak if player misses a day

### 2. Daily and Weekly Missions
- Daily missions reset every 24 hours
- Weekly missions reset on a specific day of the week
- Missions track player activity (e.g., total clicks)
- Rewards can be claimed when mission requirements are met

### 3. Spatial Data Persistence
- Uses Spatial's UserWorldDataStoreService for persistent storage
- Saves player progress across sessions
- Includes error handling and retry mechanisms
- Supports complex data structures (dictionaries, DateTime objects)

### 4. Time-Based Systems
- Custom day boundaries (e.g., day changes at 7 PM)
- Custom week boundaries (e.g., week resets on Friday)
- Automatically resets missions at appropriate times
- Gracefully handles time zone differences

### 5. Reward System
- Three reward categories: Day Streaks, Daily Missions, Weekly Missions
- Configurable reward requirements and amounts
- Visual indicators for available, achieved, and claimed rewards
- Uses ScriptableObjects for easy reward configuration

## Integration with Spatial SDK
The template primarily uses Spatial's data persistence system:

1. **Data Storage**
   - Uses `SpatialBridge.userWorldDataStoreService.SetVariable` to save player data
   - Uses `SpatialBridge.userWorldDataStoreService.GetVariable` to load player data
   - Uses `SpatialBridge.userWorldDataStoreService.HasVariable` to check data existence

2. **Data Types Supported**
   - Integers (for scores, streak counts)
   - DateTime objects (for timestamp tracking)
   - Dictionary objects (for reward claim status)

## How to Use This Template
1. The core system manages three reward types:
   - Day Streak rewards for consecutive daily logins
   - Daily Missions for in-game activities reset each day
   - Weekly Missions for in-game activities reset each week

2. Configuration options include:
   - Custom time for day changeover (`DATE_TIME_OFFSET` in GameManager)
   - Custom day for week reset (`DATE_RESET_WEEKLY` in GameManager)
   - Configurable rewards using ScriptableObjects

3. The template's click mechanic can be replaced with any game-specific action by modifying:
   - `OnClickerButtonClicked` method in GameManager
   - Mission requirements in reward ScriptableObjects

## Source Verification
- Documentation created on: March 22, 2025
- Source files last accessed: March 22, 2025
- Documentation matches source: ✓
