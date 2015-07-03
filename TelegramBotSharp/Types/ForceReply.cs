using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotSharp.Types
{
    public class ForceReplyOptions
    {
        public bool ForceReply { get { return true; } }

        /// <summary>
        /// Use this parameter if you want to force reply from specific users only; either users that are @mentioned in the text of the message, or if replying to a message, the sender of the original message.
        /// </summary>
        public bool Selective { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selective">Use this parameter if you want to force reply from specific users only; either users that are @mentioned in the text of the message, or if replying to a message, the sender of the original message.</param>
        public ForceReplyOptions(bool selective)
        {
            Selective = selective;
        }
    }
}
