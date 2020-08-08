# RustApi
Rust game server extension to provide **JSON Web API**.
It will made possible to grant access with permissions to particular plugins methods or to execute any hooks via web request.

With this **Web API** anyone can create own external applications (web, mobile, etc.) and use server plugins as a backend or just retrieve any information from server. 

You can use any external services more safety, cause now they haven't full access to server like with RCON connection.

# Status
Testing and improvement.

# How to use extension
1. Download [latest version](https://github.com/NickRimmer/RustApi/releases)
2. Copy `Oxide.Ext.RustApi.dll` file to `\server\RustDedicated_Data\Managed\Oxide.Ext.RustApi.dll`
3. Run game server

After server have been started, you can find new configuration file here:
`\server\oxide\rust-api.config.json`

## Api configuration
Configuration file is serialized to JSON [RustApiOption object](Oxide.Ext.RustApi/Models/RustApiOptions.cs) (you can investigate it, there are summaries for all properties).
```json
{
  "Endpoint": "http://*:28017",
  "LogToFile": false,
  "SkipAuthentication": false,
  "Users": [
    {
      "Name": "admin",
      "Secret": "secret1",
      "Permissions": [
        "admin"
      ]
    },
    {
      "Name": "user1",
      "Secret": "secret2",
      "Permissions": [
        "command1"
      ]
    },
    {
      "Name": "user2",
      "Secret": "secret3",
      "Permissions": [
        "command1",
	    "hooks"
      ]
    }
  ]
}
```

- **Endpoint** - public url for Api listener
- **LogToFile** - if set *true* then logs will be stored in folder `\server\oxide\logs\RustApi`
- **SkipAuthentication** - will skip authentication for Api requests
- **Users** - list of users, who can use your Api

## Api configuration: User
- **Name** - user title _(required)_
- **Secret** - used to build authentication token _(required)_
- **Permissions** - list of assigned permissions _(required)_

## Predefined permissions
- **admin** - grant full access to Api
- **hooks** - can run any hooks on server

## Call the Api commands
You can specify your own permission names for users, like 'app-clans' and configure plugin methods with special attribute:
```c#
[ApiCommand("ListOfClans", "app-clans")]
private void GetClanPlayers(ApiUserInfo user, ApiCommandRequest request)
{
    Puts($"'{user.Name}' requested list of players in clan '{request.Parameters["ClanId"]}'")
}
```

Then you will need to send post request to `http://127.0.0.1:28017/command` (or what endpoint you configured in settings) with JSON in body:
```json
{
    "commandName": "ListOfClans",
    "parameters": {
        "ClanId": "f5f75a8e-451f-4d7b-b65b-4feabdec0004"
    }
}
```
Do not forget about authentication token in request header (in case if you didn't set `SkipAuthentication: true` in configuration)

## Authentication
To protect your server from bad guys, for each request you should provide user name and unique token via request headers:
- **ra_u** - header for user name (e.g. 'admin' or 'app-clans')
- **ra_s** - header for unique token value


It is quite simple to build token, here is example on C#
```C#
/// <summary>
/// Build token for request.
/// </summary>
/// <param name="route">Route name (e.g. 'command' or 'hook').</param>
/// <param name="requestContent">Request content (JSON body value).</param>
/// <param name="userSecret">User secret (specified in api configuration).</param>
/// <returns></returns>
private static string BuildToken(string route, string requestContent, string userSecret)
{
    var str = route + (requestContent?.Trim() ?? string.Empty) + userSecret;
    var bytes = Encoding.UTF8.GetBytes(str);
    var result = Convert.ToBase64String(bytes);

    return result;
}
```

During local development or tests you can disable authentication validation (set `SkipAuthentication` to `true`) and provide only user name in headers.

# Do you have ideas?
Let's write them in issues, and we will think about it together (;
