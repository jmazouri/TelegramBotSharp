using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotSharp;
using TelegramBotSharp.Types;

namespace TelegramBotSharp.ConsoleExample
{
    class Program
    {
        public static TelegramBot bot;

        static void Main(string[] args)
        {
            Console.WriteLine("Initializing Bot...");
            bot = new TelegramBot(System.IO.File.ReadAllText("apikey.txt"));

            Console.WriteLine("Bot initialized.");
            Console.WriteLine("Hi, i'm {0}! ID: {1}", bot.Me.FirstName, bot.Me.Id);

            new Task(() => PollMessages()).Start();

            Console.ReadLine();
        }

        static async void PollMessages()
        {
            while (true)
            {
                var result = await bot.GetMessages();
                foreach (Message m in result)
                {
                    if (m.Chat.Title != null)
                    {
                        Console.WriteLine("[{0}] {1}: {2}", m.Chat.Title, m.From.Username, m.Text);
                    }
                    else
                    {
                        Console.WriteLine("{0}: {1}", m.From.Username, m.Text);
                    }

                    HandleMessage(m);
                }
            }
        }

        static void HandleMessage(Message m)
        {
            if (m.Text != null)
            {
                MessageTarget target = (m.Chat.Title == null ? (MessageTarget)m.From : m.Chat);

                if (m.Text.ToLower() == "bots are dumb")
                {
                    string messageToSend = "You're dumb, " + m.From.Username + "!";

                    bot.SendMessage(target, messageToSend);
                }

                if (m.Text.ToLower() == "hurt me plenty")
                {
                    bot.SendPhoto(target, new FileStream("doomimage.png", FileMode.Open), "RIP AND TEAR", "doomimage.png");
                }
            }
        }
    }
}
