# RustApi
Rust game server extension to provide **JSON Web API**.
It will made possible to grant access with permissions to particular plugins methods or to execute any hooks via web request.

With this **Web API** anyone can create own external applications (web, mobile, etc.) and use server plugins to retrieve/send any information from/to game server.

Why not use RCON? Cause it's easier to call and RustApi can provide limited access to server (RCON provide only full access).

# Status
Testing and improvement.

# How to use extension
1. Download [latest version](https://github.com/NickRimmer/RustApi/releases)
2. Copy `Oxide.Ext.RustApi.dll` file to `\server\RustDedicated_Data\Managed\Oxide.Ext.RustApi.dll`
3. Run game server

After server have been started, you can find new configuration file here:
`\server\oxide\rust-api.config.json`

## Extension configuration
Configuration file is serialized to JSON [RustApiOption](Oxide.Ext.RustApi/Models/RustApiOptions.cs) and [ApiUserInfo](Oxide.Ext.RustApi/Models/ApiUserInfo.cs) objects (you can investigate it, there are summaries for all properties).

[Read more](Configuration.md) about configuration file.

## Call the Api commands
You can specify your custom permission name, like 'app-clans' and configure plugin method for 'ClanPlayer' api command:
```c#
[ApiCommand("ClanPlayers", "app-clans")]
private void GetClanPlayers(ApiUserInfo user, ApiCommandRequest request)
{
    Puts($"'{user.Name}' requested list of players in clan '{request.Parameters["ClanId"]}'")
}
```

Then you will need to send post request to `http://127.0.0.1:28017/command` (or what endpoint you configured in settings) with JSON in body:
```json
{
    "commandName": "ClanPlayers",
    "parameters": {
        "ClanId": "f5f75a8e-451f-4d7b-b65b-4feabdec0004"
    }
}
```

## Authentication
To identify user, for each request you should provide user name and secret string via request headers:
- **ra_u** - header for user name (e.g. 'admin' or 'app-clans')
- **ra_s** - header for secret value

# Do you have ideas?
Let's write them in issues, and we will think about it together (;
