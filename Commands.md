# Api commands
To add Rust API commands, specify attribute `ApiCommand` for your plugin methods.
```C#
[ApiCommand ("test_public")]
private void SomeMethod1 () {
    Puts("public method handled");
}

[ApiCommand ("test_secure", "admin")]
public void SomeMethod2 () {
    Puts("secure method handled");
}
```

## Command settings 
```C#
[ApiCommand("command_name", "required_permissions_1", "required_permissions_2", ...)]
```

- `command_name` - not empty command name. Shouldn't be unique, you can specify multiply methods with same command name
- `"required_permissions_1", "required_permissions_2", ...` - list of required permissions, only users with one of these permissions will have access to command

## Command arguments
```C#
[ApiCommand ("test_arguments_1")]
public void SomeMethod2 (ApiCommandAttribute attribute) {
    Puts($"Command name: {attribute.CommandName}");
}

[ApiCommand ("test_arguments_2", "admin")]
public void SomeMethod2 (ApiCommandAttribute attribute, ApiUserInfo user) {
    Puts($"User name: {user.Name}");
}

[ApiCommand ("test_arguments_3", "admin")]
public void SomeMethod2 (ApiCommandAttribute attribute, ApiUserInfo user, ApiCommandRequest request) {
    Puts($"Request params: {string.Join(", ", request.Parameters.Keys)}");
}
```

- `ApiCommandRequest` - request data
- `ApiCommandAttribute` - attribute configuration object
- `ApiUserInfo` - user information

## Api request
Use you configured endpoint in options, and send post requests to
`{RustApiOptions.Endpoint}/command`.

Request body example:
```json
{
    "commandName": "test_arguments_3",
    "parameters": {
        "Param1": "f5f75a8e-451f-4d7b-b65b-4feabdec0004",
        "Param2": 19,
        "Param3": true
    }
}
```