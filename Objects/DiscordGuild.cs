using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeChatBot.ApiWrapper;
using Discord.WebSocket;
using Discord;

namespace AwesomeChatBot.Discord.Objects
{
    public class DiscordGuild : Server
    {
        /// <summary>
        /// The underlying discord guild object
        /// </summary>
        public SocketGuild Guild { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="wrapper"></param>
        /// <param name="discordGuild"></param>
        public DiscordGuild(ApiWrapper.ApiWrapper wrapper, SocketGuild discordGuild) : base(wrapper)
        {
            #region PRECONDITIONS

            if (wrapper == null)
                throw new ArgumentNullException(nameof(wrapper));

            #endregion PRECONDITIONS

            Guild = discordGuild ?? throw new ArgumentNullException(nameof(discordGuild));
        }

        /// <summary>
        /// Name of the server
        /// </summary>
        public override string ServerName => Guild.Name;

        /// <summary>
        /// Unique ID of the server
        /// </summary>
        public override string ServerID => Convert.ToString(Guild.Id);

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
                var channelOrNull = Guild.Channels.FirstOrDefault(x =>
                    string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase)
                    && x is ITextChannel);

                return channelOrNull != null
                    ? new DiscordChannel(ApiWrapper, channelOrNull) as Channel
                    : null;
            });
        }

        /// <summary>
        /// Gets a list of all
        /// </summary>
        /// <returns></returns>
        public override async Task<IList<Channel>> GetAllChannelsAsync()
        {
            return Guild.Channels.Select(x => new DiscordChannel(ApiWrapper, x) as Channel).ToList();
        }

        public override async Task<IList<UserRole>> GetAvailableUserRolesAsync()
        {
            return Guild.Roles.Select(role => new DiscordUserRole(ApiWrapper, role) as UserRole).ToList();
        }

        public override async Task<IList<User>> GetUserseOnServer()
        {
            return Guild.Users.Select(u => new DiscordUser(ApiWrapper, u) as User).ToList();
        }
    }
}
