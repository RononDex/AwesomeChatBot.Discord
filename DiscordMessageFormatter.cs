namespace AwesomeChatBot.Discord
{
    /// <summary>
    /// The discord message formatter
    /// </summary>
    public class DiscordMessageFormatter : ApiWrapper.MessageFormatter
    {
        public override string Bold(string message)
        {
            return $"**{message}**";
        }

        public override string CodeBlock(string message, string language = null)
        {
            return $"```{language}\r\n{message}\r\n```";
        }

        public override string Italic(string message)
        {
            return $"*{message}*";
        }

        public override string Quote(string message)
        {
            return $"```\r\n{message}\r\n```";
        }

        public override string Underline(string message)
        {
            return $"__{message}__";
        }
    }
}
