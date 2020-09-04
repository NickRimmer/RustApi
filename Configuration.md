# API configuration

Basic configuration file example:
```json
{
  "Endpoint": "http://*:28017",
  "LogToFile": false,
  "LogLevel": "Error"
}
```

- `Endpoint` - public url for Api listener
- `LogToFile` - if set *true* then logs will be stored in folder `\server\oxide\logs\RustApi`
- `Users` - list of users, who can use your Api
- `LogLevel` - to filter log messages and do not spam to console and file

## Log level
You can specify next levels:
- `Disable` - Disable (no logs)
- `Error` - Errors only (configured by default)
- `Warning` - Warnings and previous
- `Information` - Info and previous
- `Debug` - Debug and previous (all logs)

# Users configuration
```json
[
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
    "Name": "76561198051734570",
    "Secret": "06e62911cf8140f4b2c45bda5d9a14d4",
    "Permissions": [
      "player",
      "tester"
    ]
  }
]
```

Here is just an array of users.

- `Name` - user title _(required)_
- `Secret` - used to build authentication token _(required)_
- `Permissions` - list of assigned permissions _(required)_

## Predefined permissions
- `admin` - grant full access to Api
- `hooks` - can run any hooks on server
- `player` - steam authorized users (name will be steam ID)
