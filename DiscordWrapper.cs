using Discord.WebSocket;
using AwesomeChatBot.ApiWrapper;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using AwesomeChatBot.DiscordWrapper.Objects;
using AwesomeChatBot.Config;

namespace AwesomeChatBot.DiscordWrapper
{
    /// <summary>
    /// Discord API Wrapper for the "AwesomeChatBot" Framework
    /// </summary>
    public partial class DiscordWrapper : ApiWrapper.ApiWrapper
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
