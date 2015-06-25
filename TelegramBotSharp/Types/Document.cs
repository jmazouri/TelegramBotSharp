namespace TelegramBotSharp.Types
{
    public class Document : AttachmentMessage
    {
        public PhotoSize Thumb { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
    }
}