using Discord.WebSocket;

namespace AwesomeChatBot.DiscordWrapper.Objects
{
    public class DiscordUserRole : ApiWrapper.UserRole
    {
        private readonly SocketRole DiscordRole;

        public DiscordUserRole(ApiWrapper.ApiWrapper wrapper, SocketRole userRole) : base(wrapper)
        {
            this.DiscordRole = userRole;
        }

        public override string RoleId => DiscordRole.Id.ToString();

        public override string Name => DiscordRole.Name;

        public override bool IsAdmin => DiscordRole.Permissions.Administrator;
    }
}