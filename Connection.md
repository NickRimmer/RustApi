# How to connect to RustApi
You just need to send HTTP POST request with credentials in headers (=


## Authentication
To identify user, for each request you should provide user name and secret string via request headers:
- **ra_u** - header for user name (e.g. 'admin', 'app-clans' or 'playerSteamId')
- **ra_s** - header for secret value

# Call hooks
**Endpoint**: http://your-server.com/hook  
**Request body**: [ApiHookRequest](Oxide.Ext.RustApi/Primitives/Models/ApiHookRequest.cs)  
**Response**: Hook method return data
```json
{
    "hookName": "TestHook",
    "parameters": {
        "name": "Some name",
        "value": 100
    }
}
```

If hook returns something, you will received serlized to JSON object in response.


# Execute commands
Commands work pretty same as hooks.

**Endpoint**: http://your-server.com/command  
**Request body**: [ApiCommandRequest](Oxide.Ext.RustApi/Primitives/Models/ApiCommandRequest.cs)  
**Response**: List of objects 

```json
{
    "commandName": "test_arguments_3",
    "parameters": {
        "t1": 1
    }
}
```
Because in plugins you can specify a few commands with same names, reponse will be an array of objects.

# Ping endpoint
Just for testing.

**Endpoint**: http://your-server.com/system/ping  
**Response**: Something :P