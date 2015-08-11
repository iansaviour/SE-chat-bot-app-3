using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SE_chat_bot_app_3.CommonInterfaces;
using System.Threading;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace SE_chat_bot_app_3
{
    public class Bot : IBot
    {

        public static string serializedBotFileName = "bot config.json";

        public static Bot FromConfig()
        {
            var serializedBotConfigPath = Path.Combine(Program.rootdir, serializedBotFileName);
            if (Helper.CreateDirectoryIfItDoesntExist(Program.rootdir) ||
                !File.Exists(serializedBotConfigPath))
            {
                Log("[x] Bot config not found. Bot not loaded. Either find the config or create a new bot.");
                return null;
            }

            Bot bot = null;

            try
            {
                bot = new Bot() { config = JsonConvert.DeserializeObject<BotConfigurationClass>(File.ReadAllText(serializedBotConfigPath)) };
            }
            catch (JsonException jex)
            {
                string error = "[X] Bot.FromBotDir: Failed to deserialize bot config file at \"" + serializedBotConfigPath + "\": \n" + jex;
                Log(error);
                throw new Exception(error);
            }

            return bot;
        }

        public Bot() { config = new BotConfigurationClass(); }

        BotConfigurationClass config { get; set; }

        public bool CredentialsNotEmpty { get { return !string.IsNullOrEmpty(config.login) && !string.IsNullOrEmpty(config.pass) && (MainRoomID > 0 || DebugRoomID > 0); } }
        public string Login { get { return config.login; } set { config.login = value; } }
        public string Pass { get { return config.pass; } set { config.pass = value; } }
        public string TriggerSymbol { get { return config.triggerSymbol; } set { config.triggerSymbol = value; } }
        public int UserID { get { return config.userid; } set { config.userid = value; } }
        public string Name { get { return config.name; } set { config.name = value; } }
        public int MainRoomID { get { return config.mainRoomID; } set { config.mainRoomID = value; } }
        public int DebugRoomID { get { return config.debugRoomID; } set { config.debugRoomID = value; } }
        public string LogDirectoryPath { get { return DebugLogManager.logDir; } }
        public Dictionary<string, string> CommandAliases { get { return config.commandAliases; } set { config.commandAliases = value; } }
        public IChatMessage LatestReceivedChatMessage { get { return chatapi.lastReceivedMessage; } }

        public List<int> TrustedUserIDs { get { return config.trustedUserIDs; } set { config.trustedUserIDs = value; } }
        public List<int> BannedUserIDs { get { return config.bannedUserIDs; } set { config.bannedUserIDs = value; } }

        int MessageProcessingInterval { get { return config.messageProcessingInterval; } set { config.messageProcessingInterval = value; } }



        public ChatApi chatapi;
        public DateTime StartTimeUTC { get { return chatapi.startTime; } }


        // bot update loop task
        CancellationTokenSource taskCancellationTokenSource;
        void StartTask()
        {
            taskCancellationTokenSource = new CancellationTokenSource();
            var token = taskCancellationTokenSource.Token;
            task = Task.Factory.StartNew(() => task_DoWork(token), token);
        }
        void StopTask()
        {
            taskCancellationTokenSource.Cancel();
        }
        Task task;
        void task_DoWork(CancellationToken token)
        {
            while (true)
            {
                var t = Thread.CurrentThread;
                using (token.Register(t.Abort))
                {
                    task_cycle();
                    Thread.Sleep(MessageProcessingInterval);
                }
            }
        }
        void task_cycle()
        {
            CheckIfReconnectIsNecessary();

            var responses = new List<OutgoingMessage>();
            var messages = new List<ChatMessage>(chatapi.unprocessedMessages); // this is required to prevent collectionmodified bullshit, yet it could still happen

            foreach (var msg in messages)
            {
                var _ = new ProcessingResult() { TargetRoomID = msg.RoomID };
                var pr = ProcessMessage(this, _, msg);

                if (!pr.Respond)
                    continue;

                var om = new OutgoingMessage
                {
                    Text = pr.ResponseText,
                    RoomID = pr.TargetRoomID,
                    ReplyMessageID = pr.ReplyMessageID
                };
                responses.Add(om);

                if (pr.CommandMessageWasEdited)
                {
                    // don't edit if it was Potato, or the image will be wasted
                    if (!pr.ResponseOrigin.StartsWith("ModulePotato."))
                        if (chatapi.commandResponseMessageIDsDic.ContainsKey(pr.CommandMessageID))
                            om.MessageIDToEdit = chatapi.commandResponseMessageIDsDic[pr.CommandMessageID]; // edit own message with id matching the original command's pair value
                        else
                            Log("[x] Command-response dictionary didn't have a pair for the edited command. Perhaps the corresponding previous reply didn't appear in chat. I don't know.");
                }
            }
            chatapi.unprocessedMessages.Clear();

            var loadedModules = GetLoadedModules();
            foreach (var m in loadedModules)
            {
                var pr = m.Update(this, new ProcessingResult());

                if (!pr.Respond)
                    continue;

                var om = new OutgoingMessage
                {
                    Text = pr.ResponseText,
                    RoomID = pr.TargetRoomID,
                    ReplyMessageID = pr.ReplyMessageID
                };
                responses.Add(om);
            }


            foreach (var om in responses)
            {
                var room = chatapi.GetRoomByID(om.RoomID);

                if (om.MessageIDToEdit > 0)
                {
                    Log("[<] Editing message with id " + om.MessageIDToEdit + ": \"" + om.Text + "\".");
                    var r = room.EditMessage(om.MessageIDToEdit, om.Text);
                    Log("[«] Edit response: \"" + JsonConvert.SerializeObject(r) + "\".");
                }
                else

                    if (om.ReplyMessageID == 0)
                    {
                        Log("[<] Posting message: \"" + om.Text + "\".");
                        var r = room.PostMessage(om.Text);
                        Log("[«] Post response: \"" + JsonConvert.SerializeObject(r) + "\".");
                    }
                    else
                    {
                        Log("[<] Posting reply to messageid " + om.ReplyMessageID + ": \"" + om.Text + "\".");
                        var r = room.PostReply(om.ReplyMessageID, om.Text);
                        Log("[«] Reply response: \"" + JsonConvert.SerializeObject(r) + "\".");
                    }
            }
            responses.Clear();
        }
        void CheckIfReconnectIsNecessary()
        {
            if (DateTime.Now - chatapi.LastChatApiInitializationAttempt > chatapi.ChatApiReinitializationInterval &&
                DateTime.Now - chatapi.lastEventArrivalTime > chatapi.LastEventArrivalMaximumDelta ||
                chatapi.exceptionOccurred)
            {
                Log("[R] Restarting chat api.");
                chatapi.Stop();
                chatapi.Start();
            }
        }


        public IProcessingResult ProcessMessage(IBot bot, IProcessingResult pr, IChatMessage msg)
        {
            if (!CheckIfMessageShouldBeProcessed(bot, msg))
            {
                pr.Solved = true;
                return pr;
            }

            var txt = Helper.RemoveBotNamesAndTriggerSymbol(bot.Name, msg.Text, bot.TriggerSymbol);

            var w1 = Helper.FirstWord(txt);
            var cmd = w1.ToLowerInvariant();
            var arg = Helper.TextAfter(txt, w1).Trim();

            pr = InvokeExplicitCommand(bot, pr, msg, cmd, arg);

            if (pr.SolvedSet && pr.Solved)
                return pr;

            pr = InvokeImplicitCommand(bot, pr, msg, cmd, arg);

            if (pr.SolvedSet && pr.Solved)
                return pr;

            pr = CheckIfIProcessingResultIsOkay(bot, pr, msg);

            return pr;
        }
        private bool CheckIfMessageShouldBeProcessed(IBot bot, IChatMessage msg)
        {
            if (msg.UserID == bot.UserID)
                return false;

            if (bot.BannedUserIDs.Contains(msg.UserID))
                return false;


            bool b = false;
            var n = Helper.MessageContainsBotNameOrBeginsWithTriggerSymbol(bot.Name, msg.Text, bot.TriggerSymbol);
            switch (n)
            {
                case 0:
                    {
                        Log("[−] " + msg.MessageID + " Message didn't address bot.");
                        b = false;
                        break;
                    }
                case 1:
                    {
                        Log("[!] " + msg.MessageID + " Message didn't address bot, but contained bot's name.");
                        b = false;
                        break;
                    }
                case 2:
                    {
                        Log("[@] " + msg.MessageID + " Message directly addresses bot: \"" + msg.GetMaxDescription() + "\"");
                        b = false;
                        break;
                    }
                case 3:
                    {
                        Log("[" + bot.TriggerSymbol + "] " + msg.MessageID + " Message starts with trigger symbol " + bot.TriggerSymbol + ": \"" + msg.GetMaxDescription() + "\"");
                        b = true;
                        break;
                    }
            }
            return b;
        }
        private IProcessingResult InvokeExplicitCommand(IBot bot, IProcessingResult pr, IChatMessage msg, string cmd, string arg)
        {
            string cmdToExecute = cmd;

            if (bot.CommandAliases.ContainsKey(cmd))
                cmdToExecute = bot.CommandAliases[cmd];

            if (Command_ModuleDic.ContainsKey(cmd))
                return Command_ModuleDic[cmd].Command(bot, pr, msg, cmdToExecute, arg, cmd, arg);

            return pr;
        }
        private IProcessingResult InvokeImplicitCommand(IBot bot, IProcessingResult pr, IChatMessage msg, string cmd, string arg)
        {
            var txt = Helper.RemoveBotNamesAndTriggerSymbol(bot.Name, msg.Text, bot.TriggerSymbol);
            var low = txt.ToLowerInvariant();


            // get uploader name and image tag
            {
                if (low == "uploader" ||
                    low == "info" ||
                    low == "tag")
                {
                    var v = bot.GetModuleByName("Potato");
                    if (v != null)
                    {
                        var resp = (string)v.GetParameter("last posted image short info");
                        pr.ResponseText = resp;
                        pr.Solved = true;
                        pr.Respond = true;

                        return pr;
                    }
                }
            }


            // who − last image identification
            {
                var words = low.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (words.Contains("who") ||
                    words.Contains("who's") ||
                    words.Contains("what") ||
                    words.Contains("wut") ||
                    words.Contains("wat") ||
                    words.Contains("where") && words.Contains("from"))
                {
                    var v = bot.GetModuleByName("CoreCommands");

                    if (v != null)
                    {
                        pr = v.Command(bot, pr, msg, "last", arg, cmd, arg);

                        pr.Solved = true;
                        pr.Respond = true;

                        return pr;
                    }
                }
            }


            // loli − lolis are the best…
            {
                if (low.Contains("loli"))
                {
                    var v = bot.GetModuleByName("CoreCommands");

                    if (v != null)
                    {
                        pr = v.Command(bot, pr, msg, "say", "Lolis are the best!", cmd, arg);

                        pr.Solved = true;
                        pr.Respond = true;

                        return pr;
                    }
                }
            }


            return pr;
        }
        private IProcessingResult CheckIfIProcessingResultIsOkay(IBot bot, IProcessingResult pr, IChatMessage msg)
        {
            // if it hasn't been changed, it's probably unknown
            if (pr.Respond == false &&
                pr.ResponseText == "")
            {
                Log("[?] Don't know what to do with this message (processing result has not been changed by any processing methods since instantiation).");

                pr.Solved = true; // not really solved, but yeah
                pr.ReplyMessageID = msg.MessageID;

                pr.Respond = true;

                var replies = new string[] {
                        "I don't know how to deal with this message.", "I don't understand your request.", "I don't know that command.",
                        "Looks like there is no such command.", 
                        
                        "I think your syntax might be wrong.", "Perhaph you mistyped something.", "You probably made a typo in that command.",

                        "That's not a command I understand.", 
                        "I don't get it.", "I don't understand what you mean by that.",  "I'm not sure what that means.",
                        "It doesn't make perfect sense.",  "This doesn't make sense.", "It's not making a lot of sense.", "Something about sense not being made.",

                        "You gotta post a correct command.", "You must type a proper request.", "That command is not correct.", 
                        "I'm not completely sure what you meant by that, and I am not allowed to guess.",

                        "This is not how you request pictures.", "That is not a correct way to ask for images.", "It's not how you order more pictures.",

                        "If you want to bind that to a " + bot.TriggerSymbol +"post command, you have to edit-post that again.",
                        
                        "Did you mean \"!!lolis\"?", "Could you rephrase your request?", "Perhaps you want me to `" + bot.TriggerSymbol + "post` a picture?",
                    };

                pr.ResponseText = Helper.RandomItem(replies.ToList());

                return pr;
            }
            else
            {
                if (!pr.SolvedSet)
                {
                    pr.Solved = true;
                    pr.Respond = true;
                    pr.ResponseText = "*error: processing result's `solved` attribute was unset*";
                    return pr;
                }
            }
            return pr;
        }



        public void Start()
        {
            chatapi = new ChatApi(Login, Pass, Name, UserID, MainRoomID, DebugRoomID);
            chatapi.Start();
            this.Name = chatapi.botname;
            this.UserID = chatapi.userID;

            LoadModulesAtRuntime();
            StartTask();
        }
        public void Save()
        {
            File.WriteAllText(Path.Combine(Program.rootdir, serializedBotFileName), JsonConvert.SerializeObject(config));

            var modules = GetLoadedModules();

            foreach (var module in modules)
                module.Save();
        }
        public void Stop()
        {
            chatapi.Stop();
            DebugLogManager.Save();
        }
        public void SoftShutdown(string arg)
        {
            // write reason and exit
            try { System.IO.File.WriteAllText("shutdown " + DateTime.Now.ToString().Replace(":", "-") + ".txt", arg); }
            catch (Exception ex) { Log("[X] Failed to save shutdown reason file \"" + this.Name + "\":\n" + ex); }

            Log("[.] Stopped bot.");

            DebugLogManager.Save();

            System.Windows.Forms.Application.Exit();
        }
        public void UpdateModules()
        {
            var moduleRootFolder = @"P:\msvs projects\SE chat bot app 3\";

            var potentialNewModules = Directory.GetFiles(moduleRootFolder, "Module*.dll", SearchOption.AllDirectories);

            var currentModules = GetLoadedModules();

            foreach (var pm in potentialNewModules)
            {
                var moduleName = Path.GetFileNameWithoutExtension(pm).Substring(6);
                if (ModuleName_ModuleDic.ContainsKey(moduleName))
                {
                    var cm = ModuleName_ModuleDic[moduleName];
                    var cmDate = cm.BuildDate;
                    var pmDate = File.GetLastWriteTimeUtc(pm);

                    if (pmDate > cmDate)
                        LoadModuleAtRuntime(pm);

                    //var newPath = Path.Combine(workingDir, Path.GetFileName(pm));

                    //File.Copy(pm, newPath, true); // wait this won't work before the module is stopped, right?
                    //LoadModuleAtRuntime(newPath); // or will it??
                }
                else
                    LoadModuleAtRuntime(pm);
            }
        }


        public List<IModule> GetLoadedModules() { return ModuleName_ModuleDic.Values.ToList(); }

        public Dictionary<string, IModule> ModuleName_ModuleDic = new Dictionary<string, IModule>();
        public Dictionary<string, IModule> Command_ModuleDic = new Dictionary<string, IModule>();

        public string ModulesDir { get { return Path.Combine(Program.rootdir, "modules"); } }

        private void LoadModulesAtRuntime()
        {
            if (Helper.CreateDirectoryIfItDoesntExist(ModulesDir)) return;

            var dlls = Directory.GetFiles(ModulesDir, "*.dll");

            ModuleName_ModuleDic.Clear();

            foreach (var dll in dlls)
                LoadModuleAtRuntime(dll);
        }
        private void LoadModuleAtRuntime(string pathToDll)
        {
            Assembly assembly = Assembly.LoadFrom(pathToDll);

            var types = assembly.DefinedTypes;
            foreach (var typ in types)
                try
                {
                    IModule module = (IModule)Activator.CreateInstance(typ);
                    var name = module.GetType().Name;
                    Log("[→] Cast type \"" + name + "\" as IModule.");

                    if (ModuleName_ModuleDic.ContainsKey(module.Name))
                    {
                        Log("[x] Module with name \"" + module.Name + "\" is already loaded. Replacing.");
                        var oldModule = ModuleName_ModuleDic[module.Name];
                        oldModule.Stop();
                        oldModule.Save();
                        ModuleName_ModuleDic.Remove(module.Name);
                        foreach (var cmd in oldModule.CommandList)
                            Command_ModuleDic.Remove(cmd);
                    }

                    ModuleName_ModuleDic.Add(module.Name, module);
                    foreach (var cmd in module.CommandList)
                        Command_ModuleDic.Add(cmd, module);

                    var moduleDir = Path.Combine(Path.GetDirectoryName(pathToDll), module.Name);
                    var action = (Action<string>)((txt) => DebugLogManager.Log(txt));
                    module.Start(action, moduleDir);

                    Log("[L] Loaded and started module \"" + name + "\".");
                }
                catch (Exception ex) { Log("[X] Could not load type \"" + typ + "\" as IModule: \n" + ex); }
        }

        public ChatMessage GetLastChatMessage()
        {
            if (chatapi == null) return null;
            return chatapi.lastReceivedMessage;
        }
        public DateTime GetLastChatMessageArrivalTime()
        {
            if (chatapi == null) return DateTime.MinValue;
            return chatapi.lastMessageArrivalTime;
        }
        public DateTime GetLastChatEventArrivalTime()
        {
            if (chatapi == null) return DateTime.MinValue;
            return chatapi.lastEventArrivalTime;
        }

        public List<IChatMessage> GetTranscript(int roomid)
        {
            List<IChatMessage> list = null;

            try
            {
                if (!chatapi.roomTranscriptDic.ContainsKey(roomid))
                    return list;

                var v = chatapi.roomTranscriptDic[roomid];
                if (list == null)
                    list = new List<IChatMessage>();
                list.AddRange(v);  // invalid operation exception, collection was mudified
            }
            catch (Exception ex) { Log("[X] " + ex); }

            return list;
        }
        private List<ChatMessage> GetAllTranscriptMessages()
        {
            var list = new List<ChatMessage>();

            foreach (var transcript in chatapi.roomTranscriptDic)
                list.AddRange(transcript.Value);

            return list;
        }
        private List<ChatMessage> GetOwnMessagesFromTranscript()
        {
            var list = new List<ChatMessage>();

            foreach (var cm in GetAllTranscriptMessages())
                if (cm.UserID == this.UserID)
                    list.Add(cm);

            return list;
        }
        public List<IChatMessage> GetStillEditableMessages()
        {
            var list = new List<IChatMessage>();

            // keep getting posted messages until they're older than 2 minutes

            foreach (var cm in Enumerable.Reverse(GetOwnMessagesFromTranscript()))
                if (DateTime.UtcNow.Subtract(cm.PostedAtUTC) < new TimeSpan(0, 2, 0))
                    list.Add(cm);

            return list;
        }

        public IModule GetModuleByName(string moduleName)
        {
            IModule imod = null;

            var loaded = GetLoadedModules();

            foreach (var mo in loaded)
                if (mo.Name == moduleName)
                    return mo;

            return imod;
        }

        public void PostMessage(int roomid, string text)
        {
            var room = chatapi.GetRoomByID(roomid);
            room.PostMessage(text);
        }
        public void DeleteMessage(int roomid, int messageid)
        {
            chatapi.DeleteMessage(roomid, messageid);


            // remove kvp where value == messageid
            var newdic = new Dictionary<int, int>();
            foreach (var kvp in chatapi.commandResponseMessageIDsDic)
                if (kvp.Value != messageid)
                    newdic.Add(kvp.Key, kvp.Value);

            foreach (var transcript in chatapi.roomTranscriptDic.Values)
            {
                var list = new List<ChatMessage>();
                foreach (var cm in transcript)
                    if (cm.MessageID == messageid)
                        list.Add(cm);
                foreach (var cm in list)
                    transcript.Remove(cm);
            }
        }

        static void Log(string text) { DebugLogManager.Log(text); }
    }

    public class BotConfigurationClass
    {
        public int messageProcessingInterval = 250;
        public string login;
        public string pass;
        public string name;
        public string triggerSymbol;
        public int userid;
        public int mainRoomID;
        public int debugRoomID;
        public Dictionary<string, string> commandAliases = new Dictionary<string, string>();
        public List<int> trustedUserIDs = new List<int>();
        public List<int> bannedUserIDs = new List<int>();


    }

}
