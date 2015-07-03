using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

            new Task(PollMessages).Start();

            Console.ReadLine();
        }

        static async void PollMessages()
        {
            while (true)
            {
                var result = await bot.GetMessages();
                foreach (Message m in result)
                {
                    if (m.Chat != null)
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
            if (m.Text == null) return;

            MessageTarget target = ((MessageTarget) m.Chat ?? m.From);

            if (m.Text.ToLower() == "bots are dumb")
            {
                string messageToSend = "You're dumb, " + m.From.Username + "!";

                bot.SendMessage(target, messageToSend);
            }

            if (m.Text.ToLower() == "who are you talking to?")
            {
                bot.SendMessage(target, "You, dummy!", false, m);
            }

            if (m.Text.ToLower() == "hey")
            {
                bot.SendMessage(target, "Say that to my face, @"+m.From.Username, false, m, new ForceReplyOptions(true));
            }

            if (m.Text.Contains("/cc"))
            {
                bot.ForwardMessage(m, m.From);
            }

            if (m.Text.ToLower() == "hold on a second")
            {
                bot.SendChatAction(target, ChatAction.Typing);
                Thread.Sleep(1500);
                bot.SendMessage(target, "...okay, now what?");
            }

            if (m.Text.ToLower() == "hurt me plenty")
            {
                bot.SendPhoto(target, new FileStream("doomimage.png", FileMode.Open), "RIP AND TEAR", "doomimage.png");
            }
        }
    }
}
