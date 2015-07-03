using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotSharp.Types
{
    public class ChatAction
    {
        public static readonly string Typing = "typing";
        public static readonly string UploadPhoto = "upload_photo";
        public static readonly string RecordVideo = "record_video";
        public static readonly string UploadVideo = "upload_video";
        public static readonly string RecordAudio = "record_audio";
        public static readonly string UploadAudio = "upload_audio";
        public static readonly string UploadDocument = "upload_document";
        public static readonly string FindLocation = "find_location";
    }
}
