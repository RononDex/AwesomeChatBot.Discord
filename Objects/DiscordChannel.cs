using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        public SocketDMChannel DMChannel { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wrapper">A reference to the api wrapper</param>
        /// <param name="channel">A reference to the discord channel object</param>
        public DiscordChannel(ApiWrapper.ApiWrapper wrapper, SocketGuildChannel channel) : base(wrapper)
        {
            #region PRECONDITIONS

            if (channel == null)
                throw new ArgumentNullException("Parameter channel can not be null!");

            #endregion

            this.GuildChannel = channel;
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
        public override string ID => this.GuildChannel != null ?
            Convert.ToString(this.GuildChannel.Id) :
            Convert.ToString(this.DMChannel.Id);

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
        /// Sends a message in the channel (asynchroniously)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override Task SendMessageAsync(SendMessage message)
        {
            // If direct message, we have to send the message in that channel
            if (this.IsDirectMessageChannel)
            {
                Task task = DMChannel.SendMessageAsync(message.Content);

                if (message.Attacehemnts != null && message.Attacehemnts.Count > 0)
                {
                    foreach (var attachement in message.Attacehemnts)
                    {
                        task = task.ContinueWith((prevTask) =>
                            {
                                prevTask.Wait();
                                return DMChannel.SendFileAsync(new MemoryStream(attachement.Content), attachement.Name, null);
                            }
                        );
                    }
                }

                return task;
            }
            else // else use the discord guild channel
            {
                Task task = ((ITextChannel)GuildChannel).SendMessageAsync(message.Content);

                if (message.Attacehemnts != null && message.Attacehemnts.Count > 0)
                {
                    foreach (var attachement in message.Attacehemnts)
                    {
                        task = task.ContinueWith((prevTask) =>
                            {
                                prevTask.Wait();
                                task = ((ITextChannel)GuildChannel).SendFileAsync(new MemoryStream(attachement.Content), attachement.Name, null);
                            }
                        );
                    }
                }

                return task;
            }
        }
    }
}
