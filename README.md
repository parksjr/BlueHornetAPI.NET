# EMS Library

A .NET Library that effortlessly integrates with BlueHornet API, built for Only Natural Pet.

## Requirements

* .NET 2.0 or greater

## Installation

> Download the latest binary release in the [Downloads](https://bitbucket.org/onp/ems-library/downloads) section of this repository, or
> download the source code and compile in Visual Studio 2010 or greater.  
> Include downloaded or compiled binary file in your solution

## Basic Usage
> You can view all details on Blue Hornet's API at their online reference guide: http://resources.bluehornet.com/api/guide

> API keys and secrets can be obtained at your company's Blue Hornet dashboard by navigation Administration -> API Settings

### Example API Request

```csharp
// Configure static properties
string _apiSecret = "yourCompany.apiKey";
string _apiSecret = "yourCompany.sharedSecret";
bool _noHalt = true; // instructs the system to continue processing all method calls in a single POST in case of error
string _apiUrl = "https://echo.bluehornet.com/api/xmlrpc/index.php";

// Instantiate Authentication Object with api key, shared secret and noHalt true/false option
ApiAuthentication apiAuth = ApiAuthentication.CreateAuthenticationObject(_apiKey, _apiSecret, _noHalt);

// Instantiate Request object
ApiRequest apiRequest = ApiRequest.CreateRequest(_apiUrl, apiAuth);

// Build Method Call object and attach to request
ApiMethodCall apiMethod = ApiMethodCall.CreateMethodCall("legacy.retrieve_active");
apiMethod.AddArg("email", "example@email.com");
apiRequest.AddMethodCall(apiMethod);

// send the API request, you can pass an optional output string argument which will give you the entire response as XML
if (!apiRequest.SendRequest())
{
    // request failed
    Console.WriteLine(apiRequest.LastEventMessage);
}
```