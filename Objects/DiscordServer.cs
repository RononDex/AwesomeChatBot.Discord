using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeChatBot.ApiWrapper;
using Discord.WebSocket;
using System;

namespace AwesomeChatBot.Discord.Objects
{
    public class DiscordServer : Server
    {
        /// <summary>
        /// An internal reference to the discord guild object
        /// </summary>
        /// <value></value>
        protected SocketGuild DiscordGuild { get; set; }

        public DiscordServer(SocketGuild guild, ApiWrapper.ApiWrapper wrapper) : base(wrapper)
        {
            #region PRECONDITIONS

            if (guild == null)
                throw new ArgumentNullException("Parameter \"guild\" can not be null!");

            #endregion

            this.DiscordGuild = guild;
        }

        public override string ServerName => this.DiscordGuild.Name;

        public override string ServerID => this.DiscordGuild.Id.ToString();

        public override string Description => string.Empty;

        public override Task<List<Channel>> GetAllChannelsAsync()
        {
            throw new System.NotImplementedException();
        }

        public override Task<Channel> ResolveChannelAsync(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}