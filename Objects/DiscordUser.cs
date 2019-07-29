using System;
using System.Linq;
using System.Collections.Generic;
using AwesomeChatBot.ApiWrapper;
using Discord.WebSocket;

namespace AwesomeChatBot.DiscordWrapper.Objects
{
    public class DiscordUser : ApiWrapper.User
    {
        /// <summary>
        /// The discord user object
        /// </summary>
        public SocketUser SocketUser { get; set; }

        /// <summary>
        /// Creates an instance of of the discord user
        /// </summary>
        /// <param name="wrapper"></param>
        /// <param name="socketUser"></param>
        public DiscordUser(ApiWrapper.ApiWrapper wrapper, SocketUser socketUser) : base(wrapper)
        {
            SocketUser = socketUser;
            if (socketUser is SocketGuildUser socketGuildUser)
            {
                UserDiscordRoles = new List<DiscordUserRole>();
                socketGuildUser.Roles.ToList().ForEach(x => UserDiscordRoles.Add(new DiscordUserRole(this.ApiWrapper, x)));
            }
        }

        /// <summary>
        /// The id of the discord user
        /// </summary>
        public override string UserID
        {
            get => SocketUser.Id.ToString();
        }

        /// <summary>
        /// The name of the user
        /// </summary>
        public override string Name
        {
            get => SocketUser.Username;
        }

        /// <summary>
        /// The unique user name
        /// </summary>
        public override string UniqueUserName
        {
            get => SocketUser.Username + "#" + SocketUser.Discriminator;
        }

        private List<DiscordUserRole> UserDiscordRoles { get; set; }

        /// <summary>
        /// The roles of the user on a server
        /// </summary>
        /// <returns></returns>
        public override IReadOnlyList<UserRole> Roles => UserDiscordRoles;

        /// <summary>
        /// A string that can be used inside a text message to
        /// </summary>
        /// <returns></returns>
        public override string GetMention()
        {
            return SocketUser.Mention;
        }

        /// <summary>
        /// Adds the role with the given name to the user
        /// </summary>
        /// <param name="roleName">the name of the role to add</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public override void AddRole(string roleName)
        {
            if (!(SocketUser is SocketGuildUser socketGuildUser))
            {
                throw new InvalidOperationException("Roles can only be added to users on a server!");
            }

            var discordRole = socketGuildUser.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name, roleName, StringComparison.Ordinal));
            if (discordRole == null)
            {
                throw new ArgumentException(message: $"Can not find any role with name {roleName}");
            }

            socketGuildUser.AddRoleAsync(discordRole).Wait();
        }

        /// <summary>
        /// Removes the role with the given name from the user
        /// </summary>
        /// <param name="roleName">the name of the role to remove</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public override void RemoveRole(string roleName)
        {
            if (!(SocketUser is SocketGuildUser socketGuildUser))
            {
                throw new InvalidOperationException("Roles can only be added to users on a server!");
            }

            var discordRole = socketGuildUser.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name, roleName, StringComparison.Ordinal));
            if (discordRole == null)
            {
                throw new ArgumentException(message: $"Can not find any role with name {roleName}");
            }

            socketGuildUser.RemoveRoleAsync(discordRole).Wait();
        }
    }
}
