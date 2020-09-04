![Validation build](https://github.com/NickRimmer/RustApi/workflows/Validation%20build/badge.svg?branch=master) ![Latest release](https://img.shields.io/github/v/release/NickRimmer/RustApi)

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

After server have been started, you can find new configuration files here:
`\server\oxide\rust-api.config.json` and `\server\oxide\rust-api.users.json`

## Configuration
Configuration file is serialized to JSON [RustApiOption](Oxide.Ext.RustApi/Primitives/Models/RustApiOptions.cs) and [ApiUserInfo](Oxide.Ext.RustApi/Models/ApiUserInfo.cs) objects (you can investigate it, there are summaries for all properties).

[Read more](Configuration.md) about configuration file.

# Connection libraries
- [RustApi.ClientNet](https://github.com/NickRimmer/RustApi.ClientNet) - .NET Standard 2.0

It is quite easy to connect, so you can implement your own solution. [Read more](Connection.md) about how to do it.

# API commands
Just add `[ApiCommand(name, permission, ...)]` attribute to your plugin methods.  

[Read more](Commands.md) about api commands.

# Console commands
- `api.help` - list of available console api commands
- `api.reload` - Reload extenstion configuration from file
- `api.reload_users` - Reload extenstion users from file
- `api.version` - Installed version of RustApi extension
- `api.commands` - Cached API commands

# Players steam authorization
API service provide authorization of users via `Steam Login page`.  
New players will be registerd as regular user with `Steam ID` as `Name` and random `Secret` value.  
[Read more](SteamAuth.md) information about Steam authorization.

TODO: not described yet -_-

# Do you have any ideas?
Feel free to write them in issues, and we will think about it together (;
