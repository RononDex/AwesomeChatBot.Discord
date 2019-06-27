using System;
using System.IO;
using System.Threading.Tasks;
using AwesomeChatBot.ApiWrapper;
using Discord;
using Discord.WebSocket;

namespace AwesomeChatBot.DiscordWrapper.Objects
{
    public class DiscordChannel : ApiWrapper.Channel
    {
        /// <summary>
        /// A reference to the discord guild channel
        /// </summary>
        public SocketGuildChannel GuildChannel { get; private set; }

        /// <summary>
        /// A reference to the discord DM channel
        /// </summary>
        public SocketDMChannel DMChannel { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="wrapper">A reference to the api wrapper</param>
        /// <param name="channel">A reference to the discord channel object</param>
        public DiscordChannel(ApiWrapper.ApiWrapper wrapper, SocketGuildChannel channel) : base(wrapper)
        {
            GuildChannel = channel ?? throw new ArgumentNullException("Parameter channel can not be null!");
            _server = new DiscordGuild(wrapper, channel.Guild);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="wrapper">A reference to the api wrapper</param>
        /// <param name="channel">A reference to the discord dm channel object</param>
        public DiscordChannel(ApiWrapper.ApiWrapper wrapper, SocketDMChannel channel) : base(wrapper)
        {
            #region PRECONDITION

            if (channel == null)
                throw new ArgumentNullException("Parameter channel can not be null!");


            #endregion

            this.DMChannel = channel;
        }

        /// <summary>
        /// True if this is a DM channel
        /// </summary>
        public override bool IsDirectMessageChannel => this.GuildChannel == null;

        /// <summary>
        /// The name of the channel
        /// </summary>
        public override string Name => this.GuildChannel != null ?
            this.GuildChannel.Name :
            DMChannel.Recipient.Username;

        /// <summary>
        /// A unique ID for the channel
        /// </summary>
        public override string ChannelId => this.GuildChannel != null ?
            Convert.ToString(this.GuildChannel.Id) :
            Convert.ToString(this.DMChannel.Id);

        /// <summary>
        /// Internal reference to the discord server
        /// </summary>
        private readonly DiscordGuild _server;

        /// <summary>
        /// A reference to the parent server that this channel belongs to
        /// </summary>
        /// <returns></returns>
        public override Server ParentServer => this._server;

        /// <summary>
        /// Gets a string that can be used to mention this channel in chat messages
        /// </summary>
        /// <returns></returns>
        public override string GetMention()
        {
            if (this.GuildChannel != null)
                return ((ITextChannel)this.GuildChannel).Mention;
            else
                return $"DM channel {this.Name}";
        }

        /// <summary>
        /// Sends a message in the channel (asynchronously)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override Task SendMessageAsync(SendMessage message)
        {
            // If direct message, we have to send the message in that channel
            if (IsDirectMessageChannel)
            {
                var task = Task.Factory.StartNew(() =>
                {
                    if (!string.IsNullOrEmpty(message.Content))
                        DMChannel.SendMessageAsync(message.Content).Wait();
                });

                if (message.Attachments?.Count > 0)
                {
                    foreach (var attachment in message.Attachments)
                    {
                        task = task.ContinueWith((prevTask) =>
                            {
                                prevTask.Wait();
                                return DMChannel.SendFileAsync(new MemoryStream(attachment.Content), attachment.Name, null);
                            }
                        );
                    }
                }

                return task;
            }
            else // else use the discord guild channel
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    if (!string.IsNullOrEmpty(message.Content))
                        ((ITextChannel)GuildChannel).SendMessageAsync(message.Content).Wait();
                });

                if (message.Attachments?.Count > 0)
                {
                    foreach (var attachment in message.Attachments)
                    {
                        task = task.ContinueWith((prevTask) =>
                            {
                                prevTask.Wait();
                                using (var memoryStream = new MemoryStream(attachment.Content))
                                {
                                    task = ((ITextChannel)GuildChannel).SendFileAsync(memoryStream, attachment.Name, null);
                                }
                            }
                        );
                    }
                }

                return task;
            }
        }
    }
}