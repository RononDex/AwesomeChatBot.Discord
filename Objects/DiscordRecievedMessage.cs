using System;
using System.Collections.Generic;
using System.Text;
using AwesomeChatBot.ApiWrapper;
using Discord.WebSocket;

namespace AwesomeChatBot.DiscordWrapper.Objects
{
    /// <summary>
    /// A recieved discord message
    /// </summary>
    public class DiscordRecievedMessage : ApiWrapper.RecievedMessage
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
        /// List of attachements
        /// </summary>
        public override List<Attachement> Attacehemnts { get; set; } = new List<Attachement>();

        /// <summary>
        /// The formatted content of the message
        /// </summary>
        public override string Content { get => DiscordMessage.Content; set => throw new NotSupportedException(); }


        /// <summary>
        /// Internal storage of the channel
        /// </summary>
        private Channel _channel;
        
        /// <summary>
        /// The channel in which this message was recieved
        /// </summary>
        /// <value></value>
        public override Channel Channel { get {return _channel;} }

        private bool _isBotMentioned;

        public override bool IsBotMentioned => _isBotMentioned;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="wrapper"></param>
        public DiscordRecievedMessage(ApiWrapper.ApiWrapper wrapper, SocketMessage discordMessage, bool isBotMentioned = false) : base(wrapper)
        {
            #region PRECONDITIONS

            if (discordMessage == null)
                throw new ArgumentNullException("DiscordMessage can not be null!");

            #endregion

            this.DiscordMessage = discordMessage;
            this._isBotMentioned = isBotMentioned;

            this._author = new DiscordUser(wrapper, DiscordMessage.Author);
            
            if (discordMessage.Channel is SocketDMChannel)            
                this._channel = new DiscordChannel(wrapper, discordMessage.Channel as SocketDMChannel);
            else
                this._channel = new DiscordChannel(wrapper, discordMessage.Channel as SocketGuildChannel);
            

            // Load attachements
            if (discordMessage.Attachments != null && discordMessage.Attachments.Count > 0)
            {
                foreach (var attachement in discordMessage.Attachments)
                {
                    this.Attacehemnts.Add(new DiscordAttachement(wrapper, attachement));
                }
            }
        }
    }
}
