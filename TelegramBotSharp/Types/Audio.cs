namespace TelegramBotSharp.Types
{
    public class Audio : AttachmentMessage
    {
        public int Duration { get; set; }
        public string MimeType { get; set; }
    }
}