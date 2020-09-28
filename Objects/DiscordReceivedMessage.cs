using System;
using System.Collections.Generic;
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

        public override bool IsBotMentioned => _isBotMentioned;

        public override string Id => _chatMessage.Id;

        public override User Author => _chatMessage.Author;

        public override DateTime PostedOnUtc => _chatMessage.PostedOnUtc;

        public override Channel Channel => _chatMessage.Channel;

        public override List<Attachment> Attachments 
        {
            get => _chatMessage.Attachments;
            set => _chatMessage.Attachments = value;
        }

        public override string Content 
        {
            get => _chatMessage.Content;
            set => _chatMessage.Content = value;
        }

        public DiscordReceivedMessage(ApiWrapper.ApiWrapper wrapper, SocketMessage discordMessage, bool isBotMentioned = false) : base(wrapper)
        {
            _isBotMentioned = isBotMentioned;
            _chatMessage = new DiscordChatMessage(wrapper, discordMessage);
        }
    }
}
