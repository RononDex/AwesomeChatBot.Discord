using System.Net.Http;
using System.Threading.Tasks;
using Discord;

namespace AwesomeChatBot.Discord.Objects
{
    /// <summary>
    /// Represents an attachments inside a discord message
    /// </summary>
    public class DiscordAttachment : ApiWrapper.Attachment
    {

        /// <summary>
        /// The original discord Attachment (if any)
        /// </summary>
        public Attachment Attachment { get; set; }

        /// <summary>
        /// Used by the discord wrapper when a message is received
        /// </summary>
        /// <param name="wrapper">ApiWrapper</param>
        /// <param name="attachment">The received discord attachment</param>
        public DiscordAttachment(ApiWrapper.ApiWrapper wrapper, Attachment attachment) : base(wrapper)
        {
            Attachment = attachment;

            _name = attachment.Filename;
        }

        /// <summary>
        /// Constructor to use to create a new attachment
        /// </summary>
        public DiscordAttachment() : base(null)
        {
            // Nothing to do here
        }


        private string _name;

        /// <summary>
        /// The name of the attachment
        /// </summary>
        public override string Name
        {
            get => _name;
            set => _name = value;
        }


        private byte[] _content;

        private static readonly HttpClient HttpClient = new HttpClient();

        /// <summary>
        /// The content of the attachment
        /// </summary>
        public override byte[] Content
        {
            // Lazy downloading of attachment content, blocks, prefer GetContentAsync
            get => GetContentAsync().GetAwaiter().GetResult();
            set { _content = value; }
        }

        /// <summary>
        /// Downloads the attachment content (once) without blocking
        /// </summary>
        public override async Task<byte[]> GetContentAsync()
        {
            if (Attachment != null && _content == null)
            {
                _content = await HttpClient.GetByteArrayAsync(Attachment.Url).ConfigureAwait(false);
            }
            return _content;
        }
    }
}
