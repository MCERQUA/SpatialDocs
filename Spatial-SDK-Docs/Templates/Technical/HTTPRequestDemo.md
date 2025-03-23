# HTTP Request (Web Request) Demo Template

## Overview
The HTTP Request Demo template provides a complete implementation for making web requests from Spatial experiences to external APIs and services. It demonstrates how to fetch, process, and display data from external sources using both Unity's standard HTTP functionality and Spatial-specific implementations. The template showcases API calls, data parsing, error handling, and asynchronous programming patterns optimized for web environments.

## Features
- **RESTful API Integration**: Complete system for interacting with external web services
- **Multiple Request Types**: Support for GET, POST, PUT, and DELETE HTTP methods
- **JSON Parsing**: Tools for serializing and deserializing JSON data
- **Request Authentication**: Examples of API key, token, and OAuth implementation
- **Response Handling**: Comprehensive error and exception management
- **Asynchronous Workflow**: Non-blocking network requests using coroutines and async/await
- **Rate Limiting**: Built-in throttling to prevent API abuse
- **UI Integration**: Display mechanisms for fetched data
- **Caching System**: Optional local storage of responses to reduce API calls
- **Cross-Origin Support**: Handling of CORS restrictions in web builds

## Included Components

### 1. HTTP Request Manager
Core system for handling web requests:
- Centralized request management
- Request queue with priority handling
- Timeout and retry functionality
- Response caching options
- Comprehensive error handling
- Automatic header management
- Request authentication

### 2. API Service Layer
High-level implementation for specific API integrations:
- Separation of API-specific logic from generic HTTP handling
- Service interfaces for different external APIs
- Endpoint configuration management
- Response models and type conversion
- API version handling
- Rate limit tracking

### 3. Data Models
Structured data representations for API interactions:
- Request payload models
- Response parsing models
- Serialization/deserialization helpers
- Data validation
- Type conversion utilities

### 4. UI Display Components
Components for visualizing fetched data:
- Data binding to UI elements
- Loading state indicators
- Error message display
- Data refresh controls
- Pagination controls
- Sorting and filtering options

## Integration with SDK Components
The template integrates with these key SDK components:

| SDK Component | Usage in Template |
|---------------|-------------------|
| IUIService | Displaying fetched data and request status |
| MonoBehaviour | Base for request management and coroutines |
| SpatialLogger | Logging request details and errors |
| ISpaceContentService | Integrating external content into the experience |
| SpatialBridge | Core connectivity to Spatial's systems |

## When to Use
Use this template when:
- Integrating external APIs into your Spatial experience
- Creating experiences that need live data (weather, news, stocks, etc.)
- Building admin dashboards that connect to backend services
- Implementing user authentication with external services
- Creating content that needs to be updated from a CMS
- Building experiences that interact with databases
- Implementing leaderboards with external storage
- Creating experiences that require server-side processing

## Implementation Details

### HTTP Request Manager Implementation
The core system for making HTTP requests:

```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPRequestManager : MonoBehaviour
{
    [SerializeField] private float defaultTimeoutSeconds = 10f;
    [SerializeField] private int maxConcurrentRequests = 5;
    [SerializeField] private int maxRetries = 3;
    [SerializeField] private bool enableCaching = true;
    
    private Dictionary<string, string> cachedResponses = new Dictionary<string, string>();
    private Queue<RequestData> requestQueue = new Queue<RequestData>();
    private int activeRequests = 0;
    
    private static HTTPRequestManager _instance;
    public static HTTPRequestManager Instance
    {
        get 
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("HTTPRequestManager");
                _instance = obj.AddComponent<HTTPRequestManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void Update()
    {
        // Process queue if there's capacity
        while (requestQueue.Count > 0 && activeRequests < maxConcurrentRequests)
        {
            RequestData nextRequest = requestQueue.Dequeue();
            StartCoroutine(ProcessRequest(nextRequest));
            activeRequests++;
        }
    }
    
    /// <summary>
    /// Makes a GET request to the specified URL
    /// </summary>
    public void Get(string url, Action<string> onSuccess, Action<string> onError = null, 
                    Dictionary<string, string> headers = null, bool bypassCache = false)
    {
        RequestData request = new RequestData
        {
            Url = url,
            Method = UnityWebRequest.kHttpVerbGET,
            Headers = headers,
            OnSuccess = onSuccess,
            OnError = onError,
            BypassCache = bypassCache
        };
        
        QueueRequest(request);
    }
    
    /// <summary>
    /// Makes a POST request to the specified URL with the provided body
    /// </summary>
    public void Post(string url, string body, Action<string> onSuccess, Action<string> onError = null, 
                     Dictionary<string, string> headers = null)
    {
        RequestData request = new RequestData
        {
            Url = url,
            Method = UnityWebRequest.kHttpVerbPOST,
            Body = body,
            Headers = headers,
            OnSuccess = onSuccess,
            OnError = onError,
            BypassCache = true // POST requests are not cached
        };
        
        QueueRequest(request);
    }
    
    /// <summary>
    /// Makes a PUT request to the specified URL with the provided body
    /// </summary>
    public void Put(string url, string body, Action<string> onSuccess, Action<string> onError = null, 
                    Dictionary<string, string> headers = null)
    {
        RequestData request = new RequestData
        {
            Url = url,
            Method = UnityWebRequest.kHttpVerbPUT,
            Body = body,
            Headers = headers,
            OnSuccess = onSuccess,
            OnError = onError,
            BypassCache = true // PUT requests are not cached
        };
        
        QueueRequest(request);
    }
    
    /// <summary>
    /// Makes a DELETE request to the specified URL
    /// </summary>
    public void Delete(string url, Action<string> onSuccess, Action<string> onError = null, 
                       Dictionary<string, string> headers = null)
    {
        RequestData request = new RequestData
        {
            Url = url,
            Method = UnityWebRequest.kHttpVerbDELETE,
            Headers = headers,
            OnSuccess = onSuccess,
            OnError = onError,
            BypassCache = true // DELETE requests are not cached
        };
        
        QueueRequest(request);
    }
    
    private void QueueRequest(RequestData request)
    {
        // Check cache for GET requests
        if (enableCaching && !request.BypassCache && request.Method == UnityWebRequest.kHttpVerbGET)
        {
            if (cachedResponses.TryGetValue(request.Url, out string cachedResponse))
            {
                request.OnSuccess?.Invoke(cachedResponse);
                return;
            }
        }
        
        requestQueue.Enqueue(request);
    }
    
    private IEnumerator ProcessRequest(RequestData request, int retryCount = 0)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(request.Url, request.Method))
        {
            // Set up the request
            if (!string.IsNullOrEmpty(request.Body))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(request.Body);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }
            
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.timeout = Mathf.RoundToInt(defaultTimeoutSeconds);
            
            // Add headers
            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    webRequest.SetRequestHeader(header.Key, header.Value);
                }
            }
            
            // Common headers
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Accept", "application/json");
            
            // Send the request
            yield return webRequest.SendWebRequest();
            
            // Handle response
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                
                // Cache successful GET responses
                if (enableCaching && request.Method == UnityWebRequest.kHttpVerbGET)
                {
                    cachedResponses[request.Url] = response;
                }
                
                request.OnSuccess?.Invoke(response);
            }
            else
            {
                // Check if we should retry
                if (retryCount < maxRetries && IsRetryableError(webRequest))
                {
                    yield return new WaitForSeconds(GetRetryDelaySeconds(retryCount));
                    StartCoroutine(ProcessRequest(request, retryCount + 1));
                }
                else
                {
                    string errorMessage = $"Error: {webRequest.error}";
                    if (!string.IsNullOrEmpty(webRequest.downloadHandler.text))
                    {
                        errorMessage += $" - {webRequest.downloadHandler.text}";
                    }
                    
                    request.OnError?.Invoke(errorMessage);
                }
            }
        }
        
        activeRequests--;
    }
    
    private bool IsRetryableError(UnityWebRequest request)
    {
        // Retry server errors (5xx) and specific client errors
        return request.responseCode >= 500 || 
               request.responseCode == 429 || // Too many requests
               request.result == UnityWebRequest.Result.ConnectionError;
    }
    
    private float GetRetryDelaySeconds(int retryCount)
    {
        // Exponential backoff: 1s, 2s, 4s, etc.
        return Mathf.Pow(2, retryCount);
    }
    
    public void ClearCache()
    {
        cachedResponses.Clear();
    }
    
    public void ClearCache(string url)
    {
        if (cachedResponses.ContainsKey(url))
        {
            cachedResponses.Remove(url);
        }
    }
}

/// <summary>
/// Data structure for request information
/// </summary>
public class RequestData
{
    public string Url { get; set; }
    public string Method { get; set; }
    public string Body { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public Action<string> OnSuccess { get; set; }
    public Action<string> OnError { get; set; }
    public bool BypassCache { get; set; }
}
```

### API Service Layer Implementation
An example service for a weather API integration:

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeatherResponse
{
    public float temperature;
    public float feelsLike;
    public float humidity;
    public string description;
    public string iconCode;
    public float windSpeed;
    public string cityName;
}

public class WeatherAPIService : MonoBehaviour
{
    [SerializeField] private string apiKey = "your_api_key_here";
    [SerializeField] private string baseUrl = "https://api.example-weather.com/v1";
    
    [SerializeField] private WeatherUIDisplay uiDisplay;
    
    private const string API_KEY_HEADER = "X-API-Key";
    
    public void GetCurrentWeather(string city, Action<WeatherResponse> onSuccess, Action<string> onError = null)
    {
        string url = $"{baseUrl}/weather?city={Uri.EscapeDataString(city)}";
        
        Dictionary<string, string> headers = new Dictionary<string, string>
        {
            { API_KEY_HEADER, apiKey }
        };
        
        HTTPRequestManager.Instance.Get(
            url, 
            (response) => {
                try
                {
                    // Parse JSON response to WeatherResponse object
                    WeatherResponse weatherData = JsonUtility.FromJson<WeatherResponse>(response);
                    onSuccess?.Invoke(weatherData);
                }
                catch (Exception ex)
                {
                    onError?.Invoke($"Failed to parse weather data: {ex.Message}");
                }
            },
            onError,
            headers
        );
    }
    
    public void GetForecast(string city, int days, Action<List<WeatherResponse>> onSuccess, Action<string> onError = null)
    {
        string url = $"{baseUrl}/forecast?city={Uri.EscapeDataString(city)}&days={days}";
        
        Dictionary<string, string> headers = new Dictionary<string, string>
        {
            { API_KEY_HEADER, apiKey }
        };
        
        HTTPRequestManager.Instance.Get(
            url, 
            (response) => {
                try
                {
                    // Need to wrap the JSON response for JsonUtility to parse as a list
                    string wrappedJson = "{ \"forecasts\": " + response + "}";
                    ForecastListWrapper wrapper = JsonUtility.FromJson<ForecastListWrapper>(wrappedJson);
                    onSuccess?.Invoke(wrapper.forecasts);
                }
                catch (Exception ex)
                {
                    onError?.Invoke($"Failed to parse forecast data: {ex.Message}");
                }
            },
            onError,
            headers
        );
    }
    
    // UI integration examples
    
    public void FetchWeatherForUI(string city)
    {
        if (uiDisplay == null)
        {
            Debug.LogError("UI Display reference not set!");
            return;
        }
        
        uiDisplay.ShowLoading(true);
        
        GetCurrentWeather(
            city,
            (weatherData) => {
                uiDisplay.ShowLoading(false);
                uiDisplay.DisplayWeatherData(weatherData);
            },
            (error) => {
                uiDisplay.ShowLoading(false);
                uiDisplay.DisplayError(error);
            }
        );
    }
    
    [Serializable]
    private class ForecastListWrapper
    {
        public List<WeatherResponse> forecasts;
    }
}
```

### UI Display Implementation
Example of displaying API data in the UI:

```csharp
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeatherUIDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField cityInput;
    [SerializeField] private Button searchButton;
    [SerializeField] private TextMeshProUGUI temperatureText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI humidityText;
    [SerializeField] private TextMeshProUGUI windSpeedText;
    [SerializeField] private Image weatherIcon;
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TextMeshProUGUI errorText;
    
    [Header("Icon References")]
    [SerializeField] private Sprite sunnyIcon;
    [SerializeField] private Sprite cloudyIcon;
    [SerializeField] private Sprite rainyIcon;
    [SerializeField] private Sprite snowyIcon;
    
    private WeatherAPIService weatherService;
    
    private void Awake()
    {
        weatherService = GetComponent<WeatherAPIService>();
        
        if (weatherService == null)
        {
            Debug.LogError("WeatherAPIService component not found!");
            return;
        }
        
        // Set up button click handler
        searchButton.onClick.AddListener(OnSearchButtonClicked);
    }
    
    private void OnSearchButtonClicked()
    {
        string city = cityInput.text.Trim();
        
        if (string.IsNullOrEmpty(city))
        {
            DisplayError("Please enter a city name");
            return;
        }
        
        weatherService.FetchWeatherForUI(city);
    }
    
    public void DisplayWeatherData(WeatherResponse weatherData)
    {
        // Display weather information in UI
        temperatureText.text = $"{weatherData.temperature}°C";
        descriptionText.text = weatherData.description;
        humidityText.text = $"Humidity: {weatherData.humidity}%";
        windSpeedText.text = $"Wind: {weatherData.windSpeed} km/h";
        
        // Set weather icon based on iconCode
        weatherIcon.sprite = GetWeatherIconForCode(weatherData.iconCode);
        
        // Hide error panel if visible
        errorPanel.SetActive(false);
    }
    
    public void DisplayError(string errorMessage)
    {
        errorPanel.SetActive(true);
        errorText.text = errorMessage;
    }
    
    public void ShowLoading(bool isLoading)
    {
        loadingIndicator.SetActive(isLoading);
    }
    
    private Sprite GetWeatherIconForCode(string iconCode)
    {
        // Map icon codes to actual sprite references
        switch (iconCode)
        {
            case "01d":
            case "01n":
                return sunnyIcon;
            case "02d":
            case "02n":
            case "03d":
            case "03n":
            case "04d":
            case "04n":
                return cloudyIcon;
            case "09d":
            case "09n":
            case "10d":
            case "10n":
                return rainyIcon;
            case "13d":
            case "13n":
                return snowyIcon;
            default:
                return cloudyIcon;
        }
    }
}
```

## Best Practices
- **Authentication Security**: Never hardcode API keys in scripts; use encrypted PlayerPrefs or a secure configuration service
- **Error Handling**: Implement comprehensive error handling for all network requests
- **Retry Logic**: Use exponential backoff for retrying failed requests
- **Separation of Concerns**: Keep HTTP request logic separate from business logic
- **Rate Limiting**: Implement proper throttling to prevent API rate limit issues
- **Caching Strategy**: Cache responses where appropriate to reduce API calls
- **Timeout Management**: Set appropriate timeouts to prevent hanging requests
- **Parsing Robustness**: Handle different response formats and error conditions gracefully
- **API Versioning**: Design with API versioning in mind to handle service changes
- **CORS Handling**: Be aware of cross-origin restrictions in web builds
- **Loading States**: Always provide feedback to users during API requests
- **Cancellation**: Implement request cancellation for long-running operations
- **Async Patterns**: Use asynchronous programming patterns to avoid blocking the main thread
- **Batch Requests**: Combine related requests when possible to reduce network overhead
- **Response Validation**: Always validate API responses before processing

## Related Templates
- [Matchmaking (Lobby System)](../Multiplayer/Matchmaking.md) - For more advanced server communication
- [Daily/Weekly Rewards (Streaks)](../UX/DailyRewards.md) - For integrating HTTP requests with user rewards
- [GPU Particles](./GPUParticles.md) - For visualizing data from APIs in interesting ways

## Additional Resources
- [GitHub Repository](https://github.com/spatialsys/spatial-http-request-demo)
- [Unity Web Request Documentation](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html)
- [JSON Parsing in Unity](https://docs.unity3d.com/ScriptReference/JsonUtility.html)
- [Spatial Bridge Documentation](../../SpatialSys/UnitySDK/SpatialBridge.md)
