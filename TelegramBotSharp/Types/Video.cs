namespace TelegramBotSharp.Types
{
    public class Video : AttachmentMessage
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Duration { get; set; }
        public PhotoSize Thumb { get; set; }
        public string MimeType { get; set; }
        public string Caption { get; set; }
    }
}