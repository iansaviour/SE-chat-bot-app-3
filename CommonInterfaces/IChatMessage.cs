using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE_chat_bot_app_3.CommonInterfaces
{
    public interface IChatMessage
    {
        string Text { get; set; }

        int UserID { get; set; }
        string UserName { get; set; }
        int MessageID { get; set; }
        int ReplyMessageID { get; set; }

        int RoomID { get; set; }

        DateTime PostedAtUTC { get; }
        TimeSpan TimeLeftForEditing { get; }
        int Edits { get; }

        bool IsReply { get; }

        string GetMaxDescription();
    }
}
