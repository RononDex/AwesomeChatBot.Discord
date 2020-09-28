using System;
using System.Collections.Generic;
using AwesomeChatBot.ApiWrapper;
using Discord.WebSocket;

namespace AwesomeChatBot.Discord.Objects
{
    public class DiscordChatMessage : ChatMessage
    {
        /// <summary>
        /// The underlying discord message object
        /// </summary>
        public SocketMessage DiscordMessage { get; }

        private readonly DiscordUser _author;

        /// <summary>
        /// The author of the message
        /// </summary>
        public override User Author
        {
            get => _author;
        }

        /// <summary>
        /// DateTime (UTC) of when the message was posted
        /// </summary>
        public override DateTime PostedOnUtc
        {
            get => DiscordMessage.CreatedAt.UtcDateTime;
        }

        /// <summary>
        /// List of attachments
        /// </summary>
        public override List<Attachment> Attachments { get; set; } = new List<Attachment>();

        /// <summary>
        /// The formatted content of the message
        /// </summary>
        public override string Content { get => DiscordMessage.Content; set => throw new NotSupportedException(); }

        /// <summary>
        /// Internal storage of the channel
        /// </summary>
        private readonly Channel _channel;

        /// <summary>
        /// The channel in which this message was received
        /// </summary>
        /// <value></value>
        public override Channel Channel { get { return _channel; } }

        /// <summary>
        /// The unique Id of the message
        /// </summary>
        public override string Id => DiscordMessage.Id.ToString();

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="wrapper"></param>
        public DiscordChatMessage(ApiWrapper.ApiWrapper wrapper, SocketMessage discordMessage) : base(wrapper)
        {
            DiscordMessage = discordMessage ?? throw new ArgumentNullException(nameof(discordMessage));

            _author = new DiscordUser(ApiWrapper, DiscordMessage.Author);

            _channel = discordMessage.Channel is SocketDMChannel
                ? new DiscordChannel(ApiWrapper, discordMessage.Channel as SocketDMChannel)
                : new DiscordChannel(ApiWrapper, discordMessage.Channel as SocketGuildChannel);

            // Load attachments
            if (discordMessage.Attachments?.Count > 0)
            {
                foreach (var attachment in discordMessage.Attachments)
                {
                    Attachments.Add(new DiscordAttachment(ApiWrapper, attachment));
                }
            }
        }
    }
}

