using System;
using System.Collections.Generic;
using AwesomeChatBot.ApiWrapper;
using Discord.WebSocket;

namespace AwesomeChatBot.DiscordWrapper.Objects
{
    /// <summary>
    /// A received discord message
    /// </summary>
    public class DiscordReceivedMessage : ApiWrapper.ReceivedMessage
    {
        /// <summary>
        /// The underlying discord message object
        /// </summary>
        public SocketMessage DiscordMessage { get; private set; }


        private DiscordUser _author;

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
        private Channel _channel;

        /// <summary>
        /// The channel in which this message was received
        /// </summary>
        /// <value></value>
        public override Channel Channel { get { return _channel; } }

        private bool _isBotMentioned;

        public override bool IsBotMentioned => _isBotMentioned;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="wrapper"></param>
        public DiscordReceivedMessage(ApiWrapper.ApiWrapper wrapper, SocketMessage discordMessage, bool isBotMentioned = false) : base(wrapper)
        {
            #region PRECONDITIONS

            if (discordMessage == null)
                throw new ArgumentNullException("DiscordMessage can not be null!");

            #endregion

            DiscordMessage = discordMessage;
            _isBotMentioned = isBotMentioned;

            _author = new DiscordUser(ApiWrapper, DiscordMessage.Author);

            if (discordMessage.Channel is SocketDMChannel)
                _channel = new DiscordChannel(ApiWrapper, discordMessage.Channel as SocketDMChannel);
            else
                _channel = new DiscordChannel(ApiWrapper, discordMessage.Channel as SocketGuildChannel);


            // Load attachments
            if (discordMessage.Attachments != null && discordMessage.Attachments.Count > 0)
            {
                foreach (var attachment in discordMessage.Attachments)
                {
                    Attachments.Add(new DiscordAttachment(ApiWrapper, attachment));
                }
            }
        }
    }
}
