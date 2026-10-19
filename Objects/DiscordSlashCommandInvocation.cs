using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AwesomeChatBot.ApiWrapper;
using AwesomeChatBot.Commands.Handlers;
using Discord;
using Discord.WebSocket;

namespace AwesomeChatBot.Discord.Objects
{
    /// <summary>
    /// A slash command invoked by a discord user
    /// </summary>
    public class DiscordSlashCommandInvocation : SlashCommandInvocation
    {
        /// <summary>
        /// The underlying discord interaction
        /// </summary>
        public SocketSlashCommand SlashCommand { get; }

        public override string CommandName => SlashCommand.Data.Name;

        public override User Author { get; }

        public override Channel Channel { get; }

        public override IReadOnlyDictionary<string, object> Options { get; }

        public DiscordSlashCommandInvocation(ApiWrapper.ApiWrapper wrapper, SocketSlashCommand slashCommand)
        {
            SlashCommand = slashCommand;
            Author = new DiscordUser(wrapper, slashCommand.User);

            if (slashCommand.Channel is SocketGuildChannel guildChannel)
                Channel = new DiscordChannel(wrapper, guildChannel);
            else if (slashCommand.Channel is SocketDMChannel dmChannel)
                Channel = new DiscordChannel(wrapper, dmChannel);

            Options = slashCommand.Data.Options.ToDictionary(
                option => option.Name,
                option => option.Value switch
                {
                    global::Discord.Attachment attachment => new DiscordAttachment(wrapper, attachment),
                    SocketUser user => new DiscordUser(wrapper, user),
                    _ => option.Value
                });
        }

        /// <summary>
        /// Sends a follow up message (the interaction is deferred when received)
        /// </summary>
        public override async Task RespondAsync(SendMessage message)
        {
            var embed = message.EmbeddedMessage != null ? DiscordChannel.BuildEmbed(message.EmbeddedMessage) : null;

            if (!(message.Attachments?.Count > 0))
            {
                await SlashCommand.FollowupAsync(message.Content, embed: embed).ConfigureAwait(false);
                return;
            }

            var files = message.Attachments.Select(a => new FileAttachment(new MemoryStream(a.Content), a.Name)).ToList();
            try
            {
                await SlashCommand.FollowupWithFilesAsync(files, message.Content, embed: embed).ConfigureAwait(false);
            }
            finally
            {
                files.ForEach(f => f.Dispose());
            }
        }
    }
}
