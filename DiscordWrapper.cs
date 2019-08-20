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
        private string DiscordToken { get; }

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
        private ILogger Logger { get; }

        /// <summary>
        /// Name of the wrapper
        /// </summary>
        public override string Name => nameof(DiscordWrapper);

        /// <summary>
        /// Discord formatter instance
        /// </summary>
        /// <returns></returns>
        private readonly DiscordMessageFormatter _messageFormatter = new DiscordMessageFormatter();

        /// <summary>
        /// The formatter used to format discord messages
        /// </summary>
        public override MessageFormatter MessageFormatter => _messageFormatter;

        /// <summary>
        /// Creates an instance of the DiscordWrapper
        /// </summary>
        /// <param name="token">The token to authenticate with the discord API</param>
        /// <param name="loggingFactory"></param>
        public DiscordWrapper(string token, ILoggerFactory loggingFactory)
        {
            #region  PRECONDITIONS

            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException("Parameter \"token\" can not be null!");
            if (loggingFactory == null)
                throw new ArgumentNullException("Parameter \"loggingFactory\" can not be null!");

            #endregion PRECONDITIONS

            DiscordToken = token;
            LoggerFactory = loggingFactory;
            Logger = LoggerFactory.CreateLogger(GetType().FullName);
        }

        /// <summary>
        /// Initializes the discord wrapper
        /// </summary>
        /// <param name="configStore">Instance of the config store used to access configuration</param>
        public override void Initialize(ConfigStore configStore)
        {
            base.Initialize(configStore);

            // Setup the discord client
            DiscordClient = new DiscordSocketClient(new DiscordSocketConfig { MessageCacheSize = 50 });

            Logger.LogInformation("Loging into discord");

            try
            {
                // Login into discord
                DiscordClient.LoginAsync(global::Discord.TokenType.Bot, DiscordToken).Wait();
            }
            catch (Exception)
            {
                Logger.LogCritical("Login failed, check if discord token is valid!");
                throw;
            }

            Logger.LogInformation("Login successfull");

            DiscordClient.StartAsync().Wait();

            // Setup the events
            DiscordClient.MessageReceived += OnMessageReceived;
            DiscordClient.GuildAvailable += OnServerAvailable;
            DiscordClient.GuildUnavailable += OnServerUnavailable;
            DiscordClient.JoinedGuild += OnJoinedNewServer;
            DiscordClient.UserJoined += OnUserJoined;
            DiscordClient.Connected += OnConnected;
            DiscordClient.Disconnected += OnDisconnected;
        }

        /// <summary>
        /// Get a list of all available servers
        /// </summary>
        /// <returns></returns>
        public override IList<Server> GetAvailableServers()
        {
            var result = new List<Server>();

            foreach (var guild in DiscordClient.Guilds)
            {
                result.Add(new DiscordGuild(this, guild));
            }

            return result;
        }
    }
}
