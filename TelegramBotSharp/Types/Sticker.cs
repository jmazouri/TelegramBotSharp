namespace TelegramBotSharp.Types
{
    public class Sticker : AttachmentMessage
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public PhotoSize Thumb { get; set; }
    }
}