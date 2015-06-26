using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using TelegramBotSharp;
using TelegramBotSharp.Types;

namespace TelegramBotSharp.Tests
{
    public class TelegramBotTests
    {
        private TelegramBot _bot;
        private TelegramBot Bot
        {
            get
            {
                if (_bot == null)
                {
                    _bot = new TelegramBot(System.IO.File.ReadAllText("apikey.txt"));
                }

                return _bot;
            }
        }

        [Fact]
        public void MeTest()
        {
            Assert.NotNull(Bot.Me);
            Assert.True(Bot.Me.Id > 0);
        }
    }
}
