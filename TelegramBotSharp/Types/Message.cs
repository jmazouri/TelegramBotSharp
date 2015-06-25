using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotSharp.Types
{
    public class Message
    {
        public int MessageId { get; set; }
        public User From { get; set; }
        public DateTime Date { get; set; }
    }
}
