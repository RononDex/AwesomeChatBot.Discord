using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeChatBot.ApiWrapper;
using Discord.WebSocket;

namespace AwesomeChatBot.Discord.Objects
{
    /// <summary>
    /// A received discord message
    /// </summary>
    public class DiscordReceivedMessage : ReceivedMessage
    {
        private readonly DiscordChatMessage _chatMessage;

        private readonly bool _isBotMentioned;

        private readonly SocketMessage discordMessage;

        public override bool IsBotMentioned => _isBotMentioned;

        public override string Id => _chatMessage.Id;

        public override User Author => _chatMessage.Author;

        public override DateTime PostedOnUtc => _chatMessage.PostedOnUtc;

        public override Channel Channel => _chatMessage.Channel;

        public override IList<Attachment> Attachments
        {
            get => _chatMessage.Attachments;
            set => throw new InvalidOperationException("The Attachments on existing messages can not be changed");
        }

        public override string Content
        {
            get => _chatMessage.Content;
            set => throw new InvalidOperationException("The message content can not be changed on existing messages");
        }

        public DiscordReceivedMessage(ApiWrapper.ApiWrapper wrapper, SocketMessage discordMessage, bool isBotMentioned = false) : base(wrapper)
        {
            _isBotMentioned = isBotMentioned;
            _chatMessage = new DiscordChatMessage(wrapper, discordMessage);
            this.discordMessage = discordMessage;
        }

        public override async Task PublishAsync()
        {
            if (discordMessage is SocketUserMessage socketUserMessage)
            {
                await socketUserMessage.CrosspostAsync().ConfigureAwait(false);
            }
        }
    }
}
