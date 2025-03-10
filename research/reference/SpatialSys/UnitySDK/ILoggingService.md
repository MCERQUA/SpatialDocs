# ILoggingService

Logging Service Interface

Service for managing error logging and debugging in Spatial spaces. This service provides functionality for logging errors with additional context and formatting for better debugging and troubleshooting.

## Methods

### Error Logging
| Method | Description |
| --- | --- |
| LogError(Exception, Dictionary<string, object>) | Log error with context |
| LogError(string, Exception, Dictionary<string, object>) | Log error with message |

## Usage Examples

```csharp
// Example: Logging Manager
public class LoggingManager : MonoBehaviour
{
    private ILoggingService loggingService;
    private Dictionary<string, LogState> logStates;
    private bool isInitialized;

    private class LogState
    {
        public bool isActive;
        public float lastLogTime;
        public Dictionary<string, object> metadata;
        public List<LogEntry> entries;
    }

    private class LogEntry
    {
        public string message;
        public LogType type;
        public float timestamp;
        public Dictionary<string, object> context;
    }

    void Start()
    {
        loggingService = SpatialBridge.loggingService;
        logStates = new Dictionary<string, LogState>();
        InitializeManager();
    }

    private void InitializeManager()
    {
        InitializeLogState("system", new Dictionary<string, object>
        {
            { "category", "system" },
            { "level", "error" },
            { "maxEntries", 100 }
        });

        InitializeLogState("network", new Dictionary<string, object>
        {
            { "category", "network" },
            { "level", "error" },
            { "maxEntries", 50 }
        });
    }

    public void LogSystemError(
        Exception exception,
        string component = null,
        Dictionary<string, object> additionalContext = null
    )
    {
        try
        {
            var context = new Dictionary<string, object>
            {
                { "timestamp", DateTime.UtcNow },
                { "component", component ?? "unknown" }
            };

            if (additionalContext != null)
            {
                foreach (var kvp in additionalContext)
                {
                    context[kvp.Key] = kvp.Value;
                }
            }

            loggingService.LogError(exception, context);
            AddLogEntry("system", null, exception, context);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error logging system error: {e.Message}");
        }
    }

    public void LogNetworkError(
        string message,
        Exception exception = null,
        Dictionary<string, object> context = null
    )
    {
        try
        {
            var logContext = new Dictionary<string, object>
            {
                { "timestamp", DateTime.UtcNow },
                { "category", "network" }
            };

            if (context != null)
            {
                foreach (var kvp in context)
                {
                    logContext[kvp.Key] = kvp.Value;
                }
            }

            loggingService.LogError(message, exception, logContext);
            AddLogEntry("network", message, exception, logContext);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error logging network error: {e.Message}");
        }
    }

    private void InitializeLogState(
        string category,
        Dictionary<string, object> metadata
    )
    {
        var state = new LogState
        {
            isActive = true,
            lastLogTime = Time.time,
            metadata = metadata,
            entries = new List<LogEntry>()
        };

        logStates[category] = state;
    }

    private void AddLogEntry(
        string category,
        string message,
        Exception exception,
        Dictionary<string, object> context
    )
    {
        if (!logStates.TryGetValue(category, out var state))
            return;

        var entry = new LogEntry
        {
            message = message ?? exception?.Message,
            type = LogType.Error,
            timestamp = Time.time,
            context = new Dictionary<string, object>(context)
        };

        state.entries.Add(entry);
        state.lastLogTime = Time.time;

        var maxEntries = (int)state.metadata["maxEntries"];
        while (state.entries.Count > maxEntries)
        {
            state.entries.RemoveAt(0);
        }
    }

    public List<LogEntry> GetLogEntries(
        string category,
        float? since = null
    )
    {
        if (!logStates.TryGetValue(category, out var state))
            return new List<LogEntry>();

        if (!since.HasValue)
            return new List<LogEntry>(state.entries);

        return state.entries
            .Where(e => e.timestamp >= since.Value)
            .ToList();
    }

    public void ClearLogs(string category)
    {
        if (!logStates.TryGetValue(category, out var state))
            return;

        state.entries.Clear();
        state.lastLogTime = Time.time;
    }
}

// Example: Error Handler
public class ErrorHandler : MonoBehaviour
{
    private LoggingManager loggingManager;
    private Dictionary<string, ErrorState> errorStates;
    private bool isInitialized;

    private class ErrorState
    {
        public bool isActive;
        public float lastErrorTime;
        public Dictionary<string, object> settings;
    }

    void Start()
    {
        loggingManager = GetComponent<LoggingManager>();
        errorStates = new Dictionary<string, ErrorState>();
        InitializeHandler();
    }

    private void InitializeHandler()
    {
        InitializeErrorState("network", new Dictionary<string, object>
        {
            { "maxRetries", 3 },
            { "retryDelay", 1.0f },
            { "timeout", 10.0f }
        });

        InitializeErrorState("system", new Dictionary<string, object>
        {
            { "maxErrors", 10 },
            { "resetInterval", 60.0f },
            { "criticalThreshold", 5 }
        });
    }

    private void InitializeErrorState(
        string errorType,
        Dictionary<string, object> settings
    )
    {
        var state = new ErrorState
        {
            isActive = true,
            lastErrorTime = 0f,
            settings = settings
        };

        errorStates[errorType] = state;
    }

    public void HandleNetworkError(
        string operation,
        Exception exception,
        Dictionary<string, object> context = null
    )
    {
        if (!errorStates.TryGetValue("network", out var state))
            return;

        var errorContext = new Dictionary<string, object>
        {
            { "operation", operation },
            { "timestamp", DateTime.UtcNow }
        };

        if (context != null)
        {
            foreach (var kvp in context)
            {
                errorContext[kvp.Key] = kvp.Value;
            }
        }

        loggingManager.LogNetworkError(
            $"Network error during {operation}",
            exception,
            errorContext
        );

        state.lastErrorTime = Time.time;
    }

    public void HandleSystemError(
        Exception exception,
        string component,
        Dictionary<string, object> context = null
    )
    {
        if (!errorStates.TryGetValue("system", out var state))
            return;

        loggingManager.LogSystemError(
            exception,
            component,
            context
        );

        state.lastErrorTime = Time.time;
    }
}
```

## Best Practices

1. Error Management
   - Categorize errors
   - Add context
   - Track timing
   - Set limits

2. Log Organization
   - Group by type
   - Include metadata
   - Manage storage
   - Clean old logs

3. Performance
   - Batch updates
   - Cache states
   - Handle timing
   - Limit entries

4. Error Handling
   - Validate input
   - Handle failures
   - Recover states
   - Log issues

## Common Use Cases

1. Error Types
   - System errors
   - Network errors
   - Runtime errors
   - User errors

2. Logging Features
   - Error tracking
   - Context capture
   - State monitoring
   - History management

3. Debug Systems
   - Error reporting
   - Log analysis
   - State tracking
   - Performance monitoring

4. Error Processing
   - Error filtering
   - Context enrichment
   - State management
   - Alert generation
