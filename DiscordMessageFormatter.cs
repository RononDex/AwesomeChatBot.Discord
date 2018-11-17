namespace AwesomeChatBot.DiscordWrapper
{
    /// <summary>
    /// The discord message formatter
    /// </summary>
    public class DiscordMessageFormatter : global::AwesomeChatBot.ApiWrapper.MessageFormatter
    {
        public override string Bold(string message)
        {
            return $"*{message}*";
        }

        public override string Italic(string message)
        {
            return $"**{message}**";
        }

        public override string Quote(string message)
        {
            return $"```\r\n{message}\r\n```";
        }
    }
}