using AwesomeChatBot.ApiWrapper;
using Discord.WebSocket;

namespace AwesomeChatBot.Discord.Objects
{
    public class DiscordReaction : Reaction
    {
        public SocketReaction SocketReaction { get; private set; }

        public DiscordReaction(
            ApiWrapper.ApiWrapper wrapper,
            SocketReaction socketReaction) : base(wrapper)
        {
            SocketReaction = socketReaction;
        }

        /// <summary>
        /// The message that was reacted to
        /// </summary>
        public override ChatMessage Message =>
            new DiscordChatMessage(ApiWrapper, SocketReaction.Message.GetValueOrDefault());

        /// <summary>
        /// The user who reacted
        /// </summary>
        public override User User =>
            new DiscordUser(ApiWrapper, (SocketUser)SocketReaction.User.GetValueOrDefault());

        /// <summary>
        /// The content of the reaction (emote)
        /// </summary>
        public override string Content => SocketReaction.Emote.Name;
    }
}
