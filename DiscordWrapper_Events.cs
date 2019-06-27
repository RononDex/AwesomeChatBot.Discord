using System.Threading.Tasks;
using AwesomeChatBot.DiscordWrapper.Objects;
using Discord.WebSocket;

namespace AwesomeChatBot.DiscordWrapper
{
    public partial class DiscordWrapper
    {
        /// <summary>
        /// Raised when a new server becomes available (connected)
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        protected Task OnServerAvailable(SocketGuild server)
        {
            return Task.Factory.StartNew(() =>
            {
                // Create the server object
                var serverObj = new DiscordGuild(this, server);

                // Propagate event to framework
                base.OnServerAvailable(serverObj);
            });
        }


        /// <summary>
        /// When a new message is received
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected Task OnMessageReceived(SocketMessage message)
        {
            return Task.Factory.StartNew(() =>
               {
                   var botUserMention = DiscordClient.CurrentUser.Mention.Replace("!", "");
                   var isMentioned = message.Content.Contains(botUserMention)
                           || message.Content.Contains(DiscordClient.CurrentUser.Mention);

                   // Create the message object
                   var messageObj = new DiscordReceivedMessage(this, message, isMentioned);

                   // Propagate the event
                   base.OnMessageReceived(messageObj);
               });
        }


        /// <summary>
        /// When a server becomes unavailable (disconnected)
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        protected Task OnServerUnavailable(SocketGuild server)
        {
            return Task.Factory.StartNew(() =>
            {
                // Create the server object
                var serverObj = new DiscordGuild(this, server);

                // Raise the event
                base.OnServerUnavailable(serverObj);
            });
        }

        /// <summary>
        /// When a new user joins on a server
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        protected Task OnUserJoined(SocketGuildUser user)
        {
            return Task.Factory.StartNew(() =>
            {
                // Materialize the objects
                var userObj = new DiscordUser(this, user);
                var serverObj = new DiscordGuild(this, user.Guild);

                base.OnNewUserJoinedServer(userObj, serverObj);
            });
        }
    }
}