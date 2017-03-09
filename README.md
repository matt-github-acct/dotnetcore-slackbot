## Dotnet core slackbot

A simple .Net core slackbot built to listen for and respond to messages.

## Usage

```
var bot = new Slackbot("bot-token", "bot-username");

bot.OnMessage += (sender, message) =>
{
    if (message.MentionedUsers.Any(user => user == "bot-username"))
    {
        bot.SendMessage(message.Channel, "hi there, thanks for mentioning my name!");
    }
};
```

## License

This code is provided under the under the [MIT license](LICENSE)
