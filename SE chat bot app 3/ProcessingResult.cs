using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SE_chat_bot_app_3.CommonInterfaces;

namespace SE_chat_bot_app_3
{
    public class ProcessingResult : IProcessingResult
    {
        private bool solvedSet = false;
        public bool SolvedSet { get { return solvedSet; } }

        private bool solved = false;
        public bool Solved { get { return solved; } set { solved = value; solvedSet = true; } }

        private bool respond = false;
        public bool Respond { get { return respond; } set { respond = value; } }

        private int commandMessageID;
        public int CommandMessageID { get { return commandMessageID; } set { commandMessageID = value; } }

        private bool commandMessageWasEdited;
        public bool CommandMessageWasEdited { get { return commandMessageWasEdited; } set { commandMessageWasEdited = value; } }

        private int ownResponseMessageIDToEdit;
        public int OwnResponseMessageIDToEdit { get { return ownResponseMessageIDToEdit; } set { ownResponseMessageIDToEdit = value; } }

        private int targetRoomID;
        public int TargetRoomID { get { return targetRoomID; } set { targetRoomID = value; } }

        private string responseText = "";
        public string ResponseText { get { return responseText; } set { responseText = value; } }

        private string responseOrigin = "";
        public string ResponseOrigin { get { return responseOrigin; } set { responseOrigin = value; } }

        private int replyMessageID;
        public int ReplyMessageID { get { return replyMessageID; } set { replyMessageID = value; } }
    }
}
