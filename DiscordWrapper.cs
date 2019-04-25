using Discord.WebSocket;
using AwesomeChatBot.ApiWrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AwesomeChatBot.DiscordWrapper.Objects;
using AwesomeChatBot.Config;

namespace AwesomeChatBot.DiscordWrapper
{
    /// <summary>
    /// Discord API Wrapper for the "AwesomeChatBot" Framework
    /// </summary>
    public class DiscordWrapper : ApiWrapper.ApiWrapper
    {
        /// <summary>
        /// The token used to authenticate against discord API
        /// </summary>
        private string DiscordToken { get; set; }

        /// <summary>
        /// Internal reference to the discord client
        /// </summary>
        /// <value></value>
        private DiscordSocketClient DiscordClient { get; set; }

        /// <summary>
        /// The logging factory used to create new loggers
        /// </summary>
        public ILoggerFactory LoggerFactory { get; set; }

        /// <summary>
        /// Logger instance
        /// </summary>
        /// <value></value>
        private ILogger Logger { get; set; }

        /// <summary>
        /// Name of the wrapper
        /// </summary>
        public override string Name => "DiscordWrapper";

        /// <summary>
        /// Discord formatter instance
        /// </summary>
        /// <returns></returns>
        private DiscordMessageFormatter _messageFormatter = new DiscordMessageFormatter();

        /// <summary>
        /// The formatter used to format discord messages
        /// </summary>
        public override MessageFormatter MessageFormatter => this._messageFormatter;

        /// <summary>
        /// Creates an instance of the DiscordWrapper
        /// </summary>
        /// <param name="token">The token to authenticate with the discord API</param>
        public DiscordWrapper(string token, ILoggerFactory loggingFactory)
        {
            #region  PRECONDITIONS

            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException("Parameter \"token\" can not be null!");
            if (loggingFactory == null)
                throw new ArgumentNullException("Parameter \"loggingFactory\" can not be null!");

            #endregion

            this.DiscordToken = token;
            this.LoggerFactory = loggingFactory;
            this.Logger = this.LoggerFactory.CreateLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Initializes the
        /// </summary>
        public override void Initialize(ConfigStore configStore)
        {
            base.Initialize(configStore);

            // Setup the discord client
            this.DiscordClient = new DiscordSocketClient(new DiscordSocketConfig()
            {
                MessageCacheSize = 50,
            });

            this.Logger.LogInformation("Loging into discord");

            try
            {
                // Login into discord
                this.DiscordClient.LoginAsync(global::Discord.TokenType.Bot, this.DiscordToken).Wait();
            }
            catch (Exception)
            {
                this.Logger.LogCritical("Login failed, check if discord token is valid!");
                throw;
            }

            this.Logger.LogInformation("Login successfull");

            this.DiscordClient.StartAsync().Wait();

            // Setup the events
            this.DiscordClient.MessageReceived += OnMessageReceived;
            this.DiscordClient.GuildAvailable += OnServerAvailable;
            this.DiscordClient.GuildUnavailable += OnServerUnavailable;
            this.DiscordClient.UserJoined += OnUserJoined;
        }

        #region API Events

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
                   var botUserMention = this.DiscordClient.CurrentUser.Mention.Replace("!", "");
                   var isMentioned = message.Content.Contains(botUserMention)
                           || message.Content.Contains(this.DiscordClient.CurrentUser.Mention);

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

        #endregion

        /// <summary>
        /// Get a list of all available servers
        /// </summary>
        /// <returns></returns>
        public override List<Server> GetAvailableServers()
        {
            var result = new List<Server>();

            foreach (var guild in this.DiscordClient.Guilds)
            {
                result.Add(new DiscordGuild(this, guild));
            }

            return result;
        }
    }
}
