using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using TelegramBotSharp.Types;
using RestSharp.Serializers;
using System.IO;
using TelegramBotSharp.Serialization;

namespace TelegramBotSharp
{
    public class TelegramBot
    {
        /// <summary>
        /// The amount of time to delay while polling.
        /// </summary>
        public int PollingTimeout { get; set; }

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
            PollingTimeout = 5;
            _client = new RestClient(ApiUrl);
            _client.AddDefaultHeader("Content-Type", "application/x-www-form-urlencoded ; charset=UTF-8");
            SimpleJson.CurrentJsonSerializerStrategy = new TelegramSerializationStrategy();
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

            if (!response.Data.Any()) return new List<Message>();

            _lastId = response.Data.Last().UpdateId;
            var rawData = response.Data.Select(d => d.Message);

            return rawData.Select(d => (d.Chat.Title == null ? d.AsUserMessage() : d)).ToList();
        }

        /// <summary>
        /// Sends a message to the given target (user or group chat)
        /// </summary>
        /// <param name="target">A User or GroupChat</param>
        /// <param name="messageText">The text of the message</param>
        /// <param name="disableLinkPreview">Whether or not to disable link previews</param>
        /// <param name="replyTarget">The message to reply to</param>
        /// <param name="forceReplyOptions">Specifies that the message must be replied to, and what users must reply</param>
        /// <returns>The message that was sent.</returns>
        public Message SendMessage(MessageTarget target, string messageText, bool disableLinkPreview = false, Message replyTarget = null, ForceReplyOptions forceReplyOptions = null)
        {
            var request = new RestRequest("sendMessage", Method.POST)
            {
                RootElement = "result"
            };

            request.AddParameter("chat_id", target.Id);
            request.AddParameter("text", messageText);
            request.AddParameter("disable_web_page_preview", disableLinkPreview);

            if (replyTarget != null)
            {
                request.AddParameter("reply_to_message_id", replyTarget.MessageId);
            }

            if (forceReplyOptions != null)
            {
                request.AddParameter("reply_markup", SimpleJson.SerializeObject(forceReplyOptions));
            }

            var result = _client.Execute<Message>(request);
            return result.Data;
        }

        /// <summary>
        /// Indicates that the bot is doing a specified action
        /// </summary>
        /// <param name="target">The target to indicate towards</param>
        /// <param name="action">The action the bot is doing (from the ChatAction class)</param>
        public void SendChatAction(MessageTarget target, string action)
        {
            var request = new RestRequest("sendChatAction", Method.POST);

            request.AddParameter("chat_id", target.Id);
            request.AddParameter("action", action);

            _client.Execute(request);
        }

        /// <summary>
        /// Forward a message from one chat to another.
        /// </summary>
        /// <param name="message">The message to forward.</param>
        /// <param name="target">The user/group to send to.</param>
        /// <returns>The message that was forwarded</returns>
        public Message ForwardMessage(Message message, MessageTarget target)
        {
            var request = new RestRequest("forwardMessage", Method.POST)
            {
                RootElement = "result"
            };

            request.AddParameter("chat_id", target.Id);
            request.AddParameter("from_chat_id", (message.Chat == null ? message.From.Id : message.Chat.Id));
            request.AddParameter("message_id", message.MessageId);

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
