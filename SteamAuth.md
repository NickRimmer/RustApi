# Steam players authorization

With **RustApi** you can build full featured `REST API` server for you external solution, like mobile application or web site in your `Plugins`.

But you also will be need to authorize players somehow, so **RestApi** provides this functions to identify and verify users via `Steam Login page`.

## How to login (for mobile application example)
- Open your mobile application
- Open page in-app browser: `http://your-configured-endpoint/auth/login`
- Player will be redirected to regular `Steam login page`
- Player should login on steam platform (only steam will see player credentials)
- After finishing browser will be redirected to `callback url`
- Handle `callback url` in your in-app browser to extract RustApi credentials

## Steam login page
You should open `/auth/login` page, then RustApi will build correct url for Steam Login page and redirect to it.

## Callback url
By default, when player logged, he will be redirected to `/auth/steamId?name={name}&secret={secret}`. In mobile application you can handle this url and extract `name` and `secret` values from url.

Then you can use this credentials for regular calls of RustApi.  
[Read more](Connection.md) how to connect with credentials.

## Custom callback url
To specify custom callback url, set `callback` parameter on login (e.g. `http://your-configured-endpoint/auth/login?callback=http://my-service.com/verify`).

When player logged, he will be redirected to `http://my-service.com/verify?name={name}&secret={secret}`