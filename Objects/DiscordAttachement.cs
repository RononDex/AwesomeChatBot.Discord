using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace AwesomeChatBot.DiscordWrapper.Objects
{
    /// <summary>
    /// Represents an attachements inside a discord message
    /// </summary>
    public class DiscordAttachement : ApiWrapper.Attachement
    {

        /// <summary>
        /// The original discord Attachement (if any)
        /// </summary>
        public Discord.Attachment Attachement { get; set; }

        /// <summary>
        /// Used by the discord wrapper when a message is recieved
        /// </summary>
        /// <param name="wrapper">ApiWrapper</param>
        /// <param name="attechement">The recieved discord attachement</param>
        public DiscordAttachement(ApiWrapper.ApiWrapper wrapper, Discord.Attachment attechement) : base(wrapper)
        {
            this.Attachement = attechement;

            this._name = attechement.Filename;
        }

        /// <summary>
        /// Constructor to use to create a new attachement
        /// </summary>
        /// <param name="wrapper"></param>
        public DiscordAttachement() : base(null)
        {
            // Nothing to do here
        }


        private string _name;

        /// <summary>
        /// The name of the attachement
        /// </summary>
        public override string Name
        {
            get;
            set;
        }


        private byte[] _content;

        /// <summary>
        /// The content of the attachement
        /// </summary>
        public override byte[] Content
        {
            get
            {
                // Lazy downloading of attachement content
                if (this.Attachement != null && _content == null)
                {
                    var wc = new WebClient();
                    using (var memstream = new MemoryStream(wc.DownloadData(this.Attachement.Url)))
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
