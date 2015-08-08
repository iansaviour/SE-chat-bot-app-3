using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE_chat_bot_app_3.CommonInterfaces
{
    public interface IBot
    {
        string TriggerSymbol { get; set; }
        int UserID { get; }
        string Name { get; set; }
        int MainRoomID { get; set; }
        int DebugRoomID { get; set; }
        string LogDirectoryPath { get; }
        Dictionary<string, string> CommandAliases { get; set; }
        IChatMessage LatestReceivedChatMessage { get; }

        List<int> TrustedUserIDs { get; set; }
        List<int> BannedUserIDs { get; set; }

        DateTime StartTimeUTC { get; }

        void Start();
        void Save();
        void SoftShutdown(string arg);
        void UpdateModules();
        List<IModule> GetLoadedModules();
        List<IChatMessage> GetTranscript(int roomid);
        List<IChatMessage> GetStillEditableMessages();
        IModule GetModuleByName(string moduleName);

        void DeleteMessage(int roomid, int messageid);
    }
}
