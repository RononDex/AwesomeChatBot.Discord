using System;
using System.Threading.Tasks;
using AwesomeChatBot.Discord.Objects;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace AwesomeChatBot.Discord
{
    public partial class DiscordWrapper
    {
        /// <summary>
        /// Raised when a new server becomes available (connected)
        /// </summary>
        /// <param name="server"></param>
        protected Task OnServerAvailableAsync(SocketGuild server)
        {
            return Task.Run(async () =>
            {
                // Create the server object
                var serverObj = new DiscordGuild(this, server);

                // Propagate event to framework
                await OnServerAvailableAsync(serverObj).ConfigureAwait(false);
            });
        }

        /// <summary>
        /// When a new message is received
        /// </summary>
        /// <param name="message"></param>
        protected Task OnMessageReceivedAsync(SocketMessage message)
        {
            return Task.Run(async () =>
            {
                var botUserMention = DiscordClient.CurrentUser.Mention.Replace("!", "");
                var isMentioned = message.Content.Contains(botUserMention)
                        || message.Content.Contains(DiscordClient.CurrentUser.Mention);

                // Create the message object
                var messageObj = new DiscordReceivedMessage(this, message, isMentioned);

                // Propagate the event
                await OnMessageReceivedAsync(messageObj).ConfigureAwait(false);
            });
        }

        /// <summary>
        /// When a new message is deleted
        /// </summary>
        /// <param name="cacheable"></param>
        /// <param name="channel"></param>
        protected Task OnMessageDeletedAsync(Cacheable<IMessage, ulong> cacheable, ISocketMessageChannel channel)
        {
            return Task.Run(async () =>
               {
                   if (cacheable.Value is SocketMessage socketMessage)
                   {
                       var botUserMention = DiscordClient.CurrentUser.Mention.Replace("!", "");
                       var isMentioned = socketMessage.Content.Contains(botUserMention)
                               || socketMessage.Content.Contains(DiscordClient.CurrentUser.Mention);

                       // Create the message object
                       var messageObj = new DiscordReceivedMessage(this, socketMessage, isMentioned);

                       // Propagate the event
                       await OnMessageDeletedAsync(messageObj).ConfigureAwait(false);
                   }
               });
        }

        /// <summary>
        /// When a server becomes unavailable (disconnected)
        /// </summary>
        /// <param name="server"></param>
        protected Task OnServerUnavailableAsync(SocketGuild server)
        {
            return Task.Run(async () =>
            {
                // Create the server object
                var serverObj = new DiscordGuild(this, server);

                // Raise the event
                await OnServerUnavailableAsync(serverObj).ConfigureAwait(false);
            });
        }

        /// <summary>
        /// When a new user joins on a server
        /// </summary>
        /// <param name="user"></param>
        protected Task OnUserJoinedAsync(SocketGuildUser user)
        {
            return Task.Run(async () =>
            {
                // Materialize the objects
                var userObj = new DiscordUser(this, user);
                var serverObj = new DiscordGuild(this, user.Guild);

                await OnNewUserJoinedServerAsync(userObj, serverObj).ConfigureAwait(false);
            });
        }

        /// <summary>
        /// When the bot joins a new server
        /// </summary>
        /// <param name="server">The server that was joined</param>
        protected Task OnJoinedNewServerAsync(SocketGuild server)
        {
            return Task.Run(async () =>
            {
                // Materialize the objects
                var serverObj = new DiscordGuild(this, server);

                await OnJoinedNewServerAsync(serverObj).ConfigureAwait(false);
            });
        }

        /// <summary>
        /// When the wrapper has connected to the discord API
        /// </summary>
        protected Task OnWrapperConnectedAsync()
        {
            return OnConnectedAsync();
        }

        /// <summary>
        /// When the wrapper has disconnected form the discord API
        /// </summary>
        /// <param name="ex">The exception that caused the disconnect</param>
        protected Task OnDisconnectedAsync(Exception ex)
        {
            return Task.Run(async () =>
            {
                Logger.Log(LogLevel.Error, $"Lost connection to the discord API: {ex}");
                await base.OnDisconnectedAsync().ConfigureAwait(false);
            });
        }

        /// <summary>
        /// When the wrapper receives a reaction added
        /// </summary>
        /// <param name="cachable"></param>
        /// <param name="channel"></param>
        /// <param name="reaction"></param>
        protected Task OnReactionAddedAsync(Cacheable<IUserMessage, ulong> cachable, ISocketMessageChannel channel, SocketReaction reaction)
        {
            return Task.Run(async () =>
            {
                var reactionObj = new DiscordReaction(this, reaction);
                await OnReactionAddedAsync(reactionObj).ConfigureAwait(false);
            });
        }
    }
}
