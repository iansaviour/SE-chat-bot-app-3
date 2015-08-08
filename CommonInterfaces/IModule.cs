using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE_chat_bot_app_3.CommonInterfaces
{
    public interface IModule
    {
        string Name { get; }
        string Description { get; }
        string ModuleDir { get; }

        DateTime BuildDate { get; }

        Dictionary<string, List<string>> CommandAliases { get; set; } // string is the original command name, List<string> are aliases

        object GetParameter(string parameterName);
        void SetParameter(string parameterName, object value);
        List<string> CommandList { get; }


        void Start(Action<string> debugLogMethod, string moduleDir);
        void Stop();
        void Load();
        void Save();

        bool Enabled { get; }
        IProcessingResult Enable(IProcessingResult pr);
        IProcessingResult Disable(IProcessingResult pr);
        IProcessingResult Update(IBot bot, IProcessingResult pr);
        IProcessingResult Command(IBot bot, IProcessingResult pr, IChatMessage msg, string cmd, string arg, string cmdOriginal, string argOriginal);


    }
}
