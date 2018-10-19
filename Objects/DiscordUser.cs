using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

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
            this.SocketUser = socketUser;
        }

        /// <summary>
        /// The id of the discord user
        /// </summary>
        public override string UserID {
            get => SocketUser.Id.ToString();
        }

        /// <summary>
        /// The name of the user
        /// </summary>
        public override string Name {
            get => SocketUser.Username;
        }

        /// <summary>
        /// The unique user name
        /// </summary>
        public override string UniqueUserName {
            get => SocketUser.Username + "#" + SocketUser.Discriminator;
        }

        /// <summary>
        /// A string that can be used inside a text message to 
        /// </summary>
        /// <returns></returns>
        public override string GetMention()
        {
            return SocketUser.Mention;
        }
    }
}
