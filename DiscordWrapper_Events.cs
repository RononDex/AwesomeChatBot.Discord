using System;
using System.Threading.Tasks;
using AwesomeChatBot.DiscordWrapper.Objects;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

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
        /// <param name="message"></param>
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

        /// <summary>
        /// When the bot joins a new server
        /// </summary>
        /// <param name="server">The server that was joined</param>
        /// <returns></returns>
        protected Task OnJoinedNewServer(SocketGuild server)
        {
            return Task.Factory.StartNew(() =>
            {
                // Materialize the objects
                var serverObj = new DiscordGuild(this, server);

                base.OnJoinedNewServer(serverObj);
            });
        }

        /// <summary>
        /// When the wrapper has connected to the discord API
        /// </summary>
        protected Task OnConnected()
        {
            return Task.Factory.StartNew(() => base.OnConnected(this));
        }

        /// <summary>
        /// When the wrapper has disconnected form the discord API
        /// </summary>
        /// <param name="ex">The exception that caused the disconnect</param>
        protected Task OnDisconnected(Exception ex)
        {
            Logger.Log(LogLevel.Error, $"Lost connection to the discord API: {ex}");
            return Task.Factory.StartNew(() => base.OnDisconnected(this));
        }
    }
}