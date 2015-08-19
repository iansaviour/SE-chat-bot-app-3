using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE_chat_bot_app_3.CommonInterfaces
{
    public interface IProcessingResult
    {
        bool SolvedSet { get; }
        bool Solved { get; set; }
        bool Respond { get; set; }
        bool RespondIfUnrecognized { get; set; }

        int CommandMessageID { get; set; }
        bool CommandMessageWasEdited { get; set; }

        int OwnResponseMessageIDToEdit { get; set; }

        int TargetRoomID { get; set; }
        string ResponseText { get; set; }
        string ResponseOrigin { get; set; }

        int ReplyMessageID { get; set; }
    }
}
