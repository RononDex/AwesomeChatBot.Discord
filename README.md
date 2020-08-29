# AwesomeChatBot.Discord
![.NET Core](https://github.com/RononDex/AwesomeChatBot.Discord/workflows/.NET%20Core/badge.svg) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

This is the discord implementation for the [Awesome Chat Bot Framework](https://github.com/RononDex/AwesomeChatBot)

It allows you to use discord with your chatbot using the AwesomeChatBot framework.

# Usage
Install either using nuget:
```
dotnet add package AwesomeChatBot.Discord
```

Or by cloning this repository and referencing AwesomeChatBot.Discord.csproj

## Usage in code
To register the discord implementation with your chat bot:
```csharp
            var discordWrapper = new DiscordWrapper(discordToken, loggerFactory);
            var wrappers = new List<ApiWrapper>() { discordWrapper };
            var botFramework = new AwesomeChatBot.AwesomeChatBot(wrappers, loggerFactory, settings);
```
`loggerFactory` is the factory used to create loggers, this allows you to use your logging framework of choice (loggerFactory is of type ILoggerFactory from the Microsoft.Extensions.Logging nuget package).

