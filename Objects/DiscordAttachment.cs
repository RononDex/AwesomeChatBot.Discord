using System.IO;
using System.Net;

namespace AwesomeChatBot.DiscordWrapper.Objects
{
    /// <summary>
    /// Represents an attachments inside a discord message
    /// </summary>
    public class DiscordAttachment : ApiWrapper.Attachment
    {

        /// <summary>
        /// The original discord Attachment (if any)
        /// </summary>
        public global::Discord.Attachment Attachment { get; set; }

        /// <summary>
        /// Used by the discord wrapper when a message is received
        /// </summary>
        /// <param name="wrapper">ApiWrapper</param>
        /// <param name="attachment">The received discord attachment</param>
        public DiscordAttachment(ApiWrapper.ApiWrapper wrapper, global::Discord.Attachment attachment) : base(wrapper)
        {
            this.Attachment = attachment;

            this._name = attachment.Filename;
        }

        /// <summary>
        /// Constructor to use to create a new attachment
        /// </summary>
        /// <param name="wrapper"></param>
        public DiscordAttachment() : base(null)
        {
            // Nothing to do here
        }


        private string _name;

        /// <summary>
        /// The name of the attachement
        /// </summary>
        public override string Name
        {
            get => _name1;
            set => _name1 = value;
        }


        private byte[] _content;
        private string _name1;

        /// <summary>
        /// The content of the attachment
        /// </summary>
        public override byte[] Content
        {
            get
            {
                // Lazy downloading of attachment content
                if (this.Attachment != null && _content == null)
                {
                    var wc = new WebClient();
                    using (var memstream = new MemoryStream(wc.DownloadData(this.Attachment.Url)))
                    {
                        _content = memstream.ToArray();
                    }
                }

                return _content;
            }
            set { _content = value; }
        }
    }
}
