using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using TelegramBotSharp.Types;
using RestSharp.Serializers;

namespace TelegramBotSharp
{
    public class TelegramBot
    {
        /// <summary>
        /// The amount of time to delay while polling.
        /// </summary>
        public int PollingTimeout { get; set; } = 5;

        /// <summary>
        /// The authtoken for your bot.
        /// </summary>
        public string AuthToken { get; private set; }
        private RestClient _client;

        /// <summary>
        /// Gets the current API url (including auth token)
        /// </summary>
        public string ApiUrl
        {
            get { return "https://api.telegram.org/bot" + AuthToken; }
        }

        /// <summary>
        /// Sets up a new bot with the given authtoken.
        /// </summary>
        /// <param name="authToken">The authorization token for your bot</param>
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

        private int _lastId;
        /// <summary>
        /// Get messages sent to the bot.
        /// </summary>
        /// <returns>A list of all the messages since the last request.</returns>
        public async Task<List<Message>> GetMessages()
        {
            var request = new RestRequest("getUpdates", Method.POST)
            {
                RootElement = "result"
            };

            request.AddParameter("timeout", PollingTimeout);
            request.AddParameter("offset", _lastId + 1);
            request.Timeout = PollingTimeout * 1000;

            IRestResponse<List<Update>> response = null;

            while (response == null || response.Data == null)
            {
                response = await _client.ExecuteTaskAsync<List<Update>>(request);
            }

            if (response.Data.Any())
            {
                _lastId = response.Data.Last().UpdateId;
                return response.Data.Select(d => d.Message).ToList();
            }

            return new List<Message>();
        }

        private Message SendMessage(int id, string messageText, bool disableLinkPreview = false)
        {
            var request = new RestRequest("sendMessage", Method.POST)
            {
                RootElement = "result"
            };

            request.AddParameter("chat_id", id);
            request.AddParameter("text", messageText);
            request.AddParameter("disable_web_page_preview", disableLinkPreview);

            var result = _client.Execute<Message>(request);
            return result.Data;
        }

        /// <summary>
        /// Sends a message to the given user. 
        /// </summary>
        /// <param name="user">The user to send to.</param>
        /// <param name="messageText">The text of the message.</param>
        /// <param name="disableLinkPreview">Whether to disable link previews on the sent message.</param>
        /// <returns>Returns a Message if successful, otherwise null.</returns>
        public Message SendMessage(User user, string messageText, bool disableLinkPreview = false)
        {
            if (user == null) { throw new ArgumentNullException("user", "User must be valid."); }
            return SendMessage(user.Id, messageText, disableLinkPreview);
        }

        /// <summary>
        /// Sends a message to the given group. 
        /// </summary>
        /// <param name="group">The group to send to.</param>
        /// <param name="messageText">The text of the message.</param>
        /// <param name="disableLinkPreview">Whether to disable link previews on the sent message.</param>
        /// <returns>Returns a Message if successful, otherwise null.</returns>
        public Message SendMessage(GroupChat group, string messageText, bool disableLinkPreview = false)
        {
            if (group == null) { throw new ArgumentNullException("group", "GroupChat must be valid."); }
            return SendMessage(group.Id, messageText, disableLinkPreview);
        }

        private User _me;
        /// <summary>
        /// Gets the current bot as a User
        /// </summary>
        /// <returns>User containing information about the bot</returns>
        public User Me
        {
            get
            {
                if (_me == null)
                {
                    var request = new RestRequest("getMe", Method.POST)
                    {
                        RootElement = "result"
                    };

                    _me = _client.Execute<User>(request).Data;
                }

                return _me;
            }
        }

        
    }
}
