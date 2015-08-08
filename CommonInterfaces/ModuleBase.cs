using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE_chat_bot_app_3.CommonInterfaces
{
    public class ModuleBase : IModule
    {
        public string Name { get { return "(Default module implementation)"; } }
        public string Description { get { return "(Default module implementation)"; } }
        public string ModuleDir { get; private set; }

        public DateTime BuildDate { get { return new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTimeUtc; } }
        public Dictionary<string, List<string>> CommandAliases { get; set; }

        public object GetParameter(string parameterName) { return null; }
        public void SetParameter(string parameterName, object value) { }

        public List<string> CommandList { get { return new List<string>(); } }

        protected Action<string> debugLogMethod;
        protected void Log(string text) { if (debugLogMethod != null) debugLogMethod(text); }

        public void Start(Action<string> debugLogMethod, string moduleDir)
        {
            this.debugLogMethod = debugLogMethod;
            this.ModuleDir = moduleDir;
            Load();
        }
        public void Stop() { throw new NotImplementedException(); }
        public void Load() { Log("[!] Executed default module implementation method."); }
        public void Save() { Log("[!] Executed default module implementation method."); }

        public bool Enabled { get; private set; }
        public IProcessingResult Enable(IProcessingResult pr)
        {
            Enabled = true;
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseText = "*Enabled module " + Name + "*";
            return pr;
        }
        public IProcessingResult Disable(IProcessingResult pr)
        {
            Enabled = false;
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseText = "*Disabled module " + Name + "*";
            return pr;
        }
        public IProcessingResult Update(IBot bot, IProcessingResult pr)
        {
            pr.Solved = true;
            return pr;
        }
        public IProcessingResult Command(IBot bot, IProcessingResult pr, IChatMessage msg, string cmd, string arg, string cmdOriginal, string argOriginal)
        {
            pr.Solved = true;
            return pr;
        }
       
    }
}
