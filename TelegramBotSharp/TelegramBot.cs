using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using TelegramBotSharp.Types;

namespace TelegramBotSharp
{
    public class TelegramBot
    {
        public string AuthToken { get; private set; }
        private RestClient _client;

        public string ApiUrl
        {
            get { return "https://api.telegram.org/bot" + AuthToken; }
        }

        public TelegramBot(string authToken)
        {
            if (String.IsNullOrWhiteSpace(authToken))
            {
                throw new ArgumentNullException("authToken", "Your authtoken must be valid. Message @Botfather on Telegram to get one.");
            }

            AuthToken = authToken;
            _client = new RestClient(ApiUrl);
            _client.AddDefaultHeader("Content-Type", "application/x-www-form-urlencoded ; charset=UTF-8");
        }

        public User Me()
        {
            var request = new RestRequest("getMe", Method.POST)
            {
                RootElement = "result"
            };

            return _client.Execute<User>(request).Data;
        }
    }
}
