# APINotAuthorizedException

Category: Other Classes

Class: Exception

## Overview
The `APINotAuthorizedException` class is an exception that is thrown when a request is made to an API that requires authorization, but the user does not have the necessary permissions or credentials to access it. This exception extends the standard System.Exception class and is used throughout the Spatial SDK to indicate authorization failures.

## Constructors

| Constructor | Description |
| --- | --- |
| APINotAuthorizedException(string) | Creates a new instance of the APINotAuthorizedException with the specified error message. |

## Inherited Members
This class inherits from System.Exception and includes all standard exception properties and methods:

| Member | Description |
| --- | --- |
| Data | Gets a collection of key/value pairs that provide additional user-defined information about the exception. |
| GetBaseException() | Returns the exception that is the root cause of one or more subsequent exceptions. |
| GetType() | Gets the runtime type of the current instance. |
| HResult | Gets or sets HRESULT, a coded numerical value that is assigned to a specific exception. |
| HelpLink | Gets or sets a link to the help file associated with this exception. |
| InnerException | Gets the Exception instance that caused the current exception. |
| Message | Gets a message that describes the current exception. |
| Source | Gets or sets the name of the application or the object that causes the error. |
| StackTrace | Gets a string representation of the immediate frames on the call stack. |
| TargetSite | Gets the method that throws the current exception. |
| ToString() | Creates and returns a string representation of the current exception. |

## Usage Example

```csharp
using SpatialSys.UnitySDK;
using UnityEngine;
using System;

public class AuthorizationExample : MonoBehaviour
{
    public void PerformSecureOperation()
    {
        try
        {
            // Attempt to perform an operation that requires authorization
            AccessSecureResource();
        }
        catch (APINotAuthorizedException ex)
        {
            // Handle the authorization exception
            Debug.LogError($"Authorization failed: {ex.Message}");
            
            // Prompt the user to log in or request proper permissions
            RequestUserAuthentication();
        }
        catch (Exception ex)
        {
            // Handle other types of exceptions
            Debug.LogError($"An error occurred: {ex.Message}");
        }
    }
    
    private void AccessSecureResource()
    {
        // Example of code that might throw an APINotAuthorizedException
        if (!SpatialBridge.actorService.localActor.isSpaceAdministrator)
        {
            throw new APINotAuthorizedException("Administrator privileges required for this operation.");
        }
        
        // Proceed with the secure operation if authorized
        Debug.Log("Secure operation completed successfully!");
    }
    
    private void RequestUserAuthentication()
    {
        // Show UI prompting the user to authenticate or request permissions
        // This could open a login dialog or permission request panel
        Debug.Log("Please log in or request administrator permissions to proceed.");
    }
}
```

## Best Practices

1. Always catch APINotAuthorizedException specifically to handle authorization failures gracefully
2. Provide clear error messages when throwing this exception that explain what permission is missing
3. Include appropriate recovery options when catching this exception, such as prompting for login
4. Check permissions proactively before attempting operations that require authorization
5. Use try/catch blocks around operations that might throw this exception to prevent application crashes

## Common Use Cases

1. Handling unauthorized access to API endpoints that require specific user roles
2. Managing permission-based feature access in Spatial experiences
3. Gracefully handling authentication timeouts or token expiration
4. Implementing role-based access control for administrative functions
5. Providing user-friendly error messages for permission-related issues

## Completed: March 10, 2025
