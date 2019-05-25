using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeChatBot.ApiWrapper;
using Discord.WebSocket;

namespace AwesomeChatBot.DiscordWrapper.Objects
{
    public class DiscordGuild : ApiWrapper.Server
    {
        /// <summary>
        /// The underlying discord guild object
        /// </summary>
        public SocketGuild Guild { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="wrapper"></param>
        /// <param name="discordGuild"></param>
        public DiscordGuild(ApiWrapper.ApiWrapper wrapper, SocketGuild discordGuild) : base(wrapper)
        {
            #region PRECONDITIONS

            if (wrapper == null)
                throw new ArgumentNullException("Parameter wrapper can not be null!");
            if (discordGuild == null)
                throw new ArgumentNullException("Parameter discordGuild can not be null!");

            #endregion

            this.Guild = discordGuild;
        }

        /// <summary>
        /// Name of the server
        /// </summary>
        public override string ServerName => this.Guild.Name;

        /// <summary>
        /// Unique ID of the server
        /// </summary>
        public override string ServerID => Convert.ToString(this.Guild.Id);

        /// <summary>
        /// Servers don't have a description on discord
        /// </summary>
        public override string Description => null;

        /// <summary>
        /// Tries to find a channel with the given name, if not found returns NULL
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override Task<Channel> ResolveChannelAsync(string name)
        {
            return Task.Run(() =>
            {
                var channelOrNull = Guild.Channels.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
                return channelOrNull != null
                    ? new DiscordChannel(ApiWrapper, channelOrNull) as Channel
                    : null;
            });
        }

        /// <summary>
        /// Gets a list of all
        /// </summary>
        /// <returns></returns>
        public override Task<List<Channel>> GetAllChannelsAsync()
        {
            return Task.Run(() => Guild.Channels.Select(x => new DiscordChannel(ApiWrapper, x) as Channel).ToList());
        }
    }
}
