using System;
using System.Collections.Generic;
using System.Text;
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
        /// <param name="disocrdGuild"></param>
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
        /// Servers dont have a description on discord
        /// </summary>
        public override string Description => null;

        public override Task<Channel> ResolveChannelAsync(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of all 
        /// </summary>
        /// <returns></returns>
        public override Task<List<Channel>> GetAllChannelsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
