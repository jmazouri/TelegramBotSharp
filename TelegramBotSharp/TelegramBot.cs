using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using TelegramBotSharp.Types;
using RestSharp.Serializers;
using System.IO;

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

        /// <summary>
        /// Sends a message to the given target (user or group chat)
        /// </summary>
        /// <param name="target">A User or GroupChat</param>
        /// <param name="messageText">The text of the message</param>
        /// <param name="disableLinkPreview">Whether or not to disable link previews</param>
        /// <returns>The message that was sent.</returns>
        public Message SendMessage(MessageTarget target, string messageText, bool disableLinkPreview = false)
        {
            var request = new RestRequest("sendMessage", Method.POST)
            {
                RootElement = "result"
            };

            request.AddParameter("chat_id", target.Id);
            request.AddParameter("text", messageText);
            request.AddParameter("disable_web_page_preview", disableLinkPreview);

            var result = _client.Execute<Message>(request);
            return result.Data;
        }

        /// <summary>
        /// Sends a photo to the given target (user or group chat)
        /// </summary>
        /// <param name="target">A User or GroupChat</param>
        /// <param name="imageStream">A stream containing the data for the image</param>
        /// <param name="caption">A caption for the image</param>
        /// <param name="filename">The filename to report to Telegram</param>
        /// <returns>The message that was sent.</returns>
        public Message SendPhoto(MessageTarget target, Stream imageStream, string caption = null, string filename = "picture.jpg")
        {
            if (filename == null) { throw new ArgumentNullException("filename", "Lectern requires a filename with proper extension."); }
            if (!filename.Contains(".")) { throw new ArgumentException("Lectern requires a filename with proper extension.", "filename"); }

            var request = new RestRequest("sendPhoto", Method.POST)
            {
                RootElement = "result"
            };

            request.AddParameter("chat_id", target.Id);
            request.AddFile("photo", imageStream.CopyTo, filename);
            request.AddParameter("caption", caption);

            request.AddHeader("Content-Type", "multipart/form-data");

            var result = _client.Execute<Message>(request);

            imageStream.Close();
            return result.Data;
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
