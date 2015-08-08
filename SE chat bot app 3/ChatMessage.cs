using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SE_chat_bot_app_3.CommonInterfaces;

namespace SE_chat_bot_app_3
{
    public class ChatMessage : IChatMessage
    {
        public string Text { get; set; }

        public int UserID { get; set; }
        public string UserName { get; set; }
        public int MessageID { get; set; }
        public int ReplyMessageID { get; set; }

        public int RoomID { get; set; }

        public DateTime PostedAtUTC { get; set; }
        public TimeSpan TimeLeftForEditing { get { return DateTime.Now.Subtract(PostedAtUTC); } }

        public int Edits { get; set; }

        public bool IsReply { get { if (ReplyMessageID > 0) return true; return false; } }

        public override string ToString()
        {
            if (IsReply)
                return Helper.GetDateTimeNowHHMMSS(PostedAtUTC) + " " + MessageID + " (@" + ReplyMessageID + ") " + UserName + ": " + Text;
            else
                return Helper.GetDateTimeNowHHMMSS(PostedAtUTC) + " " + MessageID + " " + UserName + ": " + Text;
        }

        public string GetMaxDescription()
        {
            if (IsReply)
                return Helper.GetDateTimeNowHHMMSS(PostedAtUTC) + " " + MessageID + " (u:" + UserID + ") " + UserName + ": (@" + ReplyMessageID + ")" + Text;
            else
                return Helper.GetDateTimeNowHHMMSS(PostedAtUTC) + " " + MessageID + " (u:" + UserID + ") " + UserName + ": " + Text;
        }
    }
}
