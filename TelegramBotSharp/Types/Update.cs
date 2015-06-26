using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotSharp.Types
{
    public class Update
    {
        public int UpdateId { get; set; }
        public Message Message { get; set; }
    }
}
