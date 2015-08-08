using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SE_chat_bot_app_3.CommonInterfaces;
using System.Reflection;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.IO;

namespace ModuleCoreCommands
{
    public class ModuleCoreCommands : ModuleBase, IModule
    {
        public new string Name { get { return "CoreCommands"; } }
        public new string Description { get { return "This is the second most important module. Don't lose it."; } }

        public new IProcessingResult Command(IBot bot, IProcessingResult pr, IChatMessage msg, string cmd, string arg, string cmdOriginal, string argOriginal)
        {
            switch (cmd)
            {
                default: return pr;

                // core commands

                case "about": return About(bot, pr, arg);
                case "help": return Help(bot, pr, arg);
                case "commands": return GetCommandList(bot, pr, arg);
                case "uptime": return Uptime(bot, pr);
                case "modules": return Modules(bot, pr);

                case "edit": return Edit(pr, bot, msg, arg);
                case "unonebox":
                case "unbox": return Unonebox(pr, bot, msg, arg);
                case "undo": return Undo(bot, pr, arg);

                case "status": return Status(bot, pr);

                case "trust": return TrustedUsers(bot, pr, arg);
                case "ignore": return BannedUsers(bot, pr, arg);

                case "say": return Say(pr, arg);
                case "tell": return Tell(pr, arg);

                case "alias": return Alias(pr, bot, msg, arg);

                case "save": return Save(bot, pr, msg.UserID);
                case "shutdown": return Shutdown(bot, pr, arg, msg.UserID);


                // hoihoi-san borrowed

                case "google": return Google(pr, msg, arg);
                case "wiki": return Wiki(pr, msg, arg);
                case "urban": return Urban(pr, msg, arg);
                case "youtube": return Youtube(pr, msg, arg);
                case "weather": return Weather(pr, msg, arg);


                // advanfaged

                case "id": return ID(bot, pr, msg, arg);
                case "isch": return Isch(bot, pr, msg, arg);

                case "whois":
                case "what":
                case "last": return LastPostedImageID(bot, pr, msg, arg);


                // bonus

                case "hats": return Hats(pr);
                case "meme": return Meme(bot, pr, msg, arg);

            }
        }

        public new List<string> CommandList { get { return commandsList; } }
        private List<string> commandsList = new List<string>() { 
            "about", "help", "commands", "alias", "modules",
            "edit", 
            "unonebox", "unbox",
            "undo",
            "uptime", "status", "save", "shutdown",
            "google", "wiki", "urban", "youtube", "weather", 
            "say", "tell",

            "meme",
            "hats",

            "id", "isch", "last"};

        // end of interface



        // core commands

        public IProcessingResult About(IBot bot, IProcessingResult pr, string arg)
        {
            if (arg.Trim() != "")
                return pr;

            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
            pr.ResponseText = "My name is " + bot.Name + " and I'm a chat bot. " +
                "I can't do much right now, but I'm learning new things all the time. If you know something fun I could learn to do, let me know." + Environment.NewLine +
                "You can view the list of all commands by saying `" + bot.TriggerSymbol + "commands`. If anything is unclear, say `" + bot.TriggerSymbol + "help`.";

            return pr;
        }
        public IProcessingResult Help(IBot bot, IProcessingResult pr, string arg)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
            pr.ResponseText = "Check [this meta post on Anime&Manga.SE](http://meta.anime.stackexchange.com/q/1166/) or ask around at Maid Café!";
            return pr;
        }
        public IProcessingResult GetCommandList(IBot bot, IProcessingResult pr, string arg)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);

            var modules = bot.GetLoadedModules();
            var commandsByModule = new Dictionary<string, List<string>>();

            foreach (var module in modules)
            {
                commandsByModule[module.Name] = new List<string>();

                foreach (var command in module.CommandList)
                    commandsByModule[module.Name].Add(command);
            }

            if (modules.Count > 0)
            {
                string reply = "";

                foreach (var module in commandsByModule)
                    reply += "[" + module.Key + "] " + string.Join(", ", module.Value) + Environment.NewLine;

                pr.ResponseText = reply;
            }
            else
                pr.ResponseText = "No modules are currently loaded.";

            return pr;
        }
        public IProcessingResult Alias(IProcessingResult pr, IBot bot, IChatMessage msg, string arg)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);

            // w1      // set         // list      // remove
            // w2      // post       // post    // asd, 
            // w3      // tomato   //           // fasd,

            var wc = Helper.WordCount(arg);
            var w1 = Helper.FirstWord(arg);
            var args = Helper.TextAfter(arg, w1).Trim();

            var ts = bot.TriggerSymbol;
            var ca = bot.CommandAliases;

            switch (w1)
            {
                //case "*":
                //case "list":
                case "all":
                case "list":
                    return Alias_case_List(pr, bot, msg, args);

                case "learn":
                case "add":
                case "set":
                    return Alias_case_Add(pr, bot, msg, args);


                case "forget":
                case "remove":
                case "delete":
                case "unset":
                    return Alias_case_Delete(pr, bot, msg, args);

                default:
                    return Alias_case_Default(pr, bot, msg, args);
            }
        }
        IProcessingResult Alias_case_Default(IProcessingResult pr, IBot bot, IChatMessage msg, string args)
        {
            pr.ReplyMessageID = msg.MessageID;
            pr.ResponseText = @"Command `alias` requires a parameter: *set*, *remove* or *list*. Refer to [this meta post](http://meta.anime.stackexchange.com/q/1166/) for more info.";
            return pr;
        }

        IProcessingResult Alias_case_List(IProcessingResult pr, IBot bot, IChatMessage msg, string args)
        {
            var ca = bot.CommandAliases;
            var wc = Helper.WordCount(args);
            var w2 = Helper.WordNumberN(args, 2);

            if (ca.Count < 1)
            {
                pr.ResponseText = "There are no aliases currently set.";
                return pr;
            }

            if (wc > 2)
            {
                pr.ResponseText = "You typed too many words in that request.";
                return pr;
            }

            if (w2 == "*")
                return AliasListAll(pr, bot, msg);

            return AliasList(pr, bot, msg, args);
        }
        IProcessingResult AliasListAll(IProcessingResult pr, IBot bot, IChatMessage msg)
        {
            var ca = bot.CommandAliases;

            var oad = new Dictionary<string, List<string>>();

            foreach (var kvp in ca)
            {
                if (!oad.ContainsKey(kvp.Value))
                    oad[kvp.Value] = new List<string>();
                oad[kvp.Value].Add(kvp.Key);
            }

            string reply = "List of all " + ca.Count + " command aliases:" + Environment.NewLine;

            foreach (var list in oad)
                reply += "[" + list.Key + "] " + string.Join(", ", list.Value) + Environment.NewLine;

            reply = reply.Substring(0, reply.Length - 2);
            reply += ".";

            pr.ReplyMessageID = msg.MessageID;
            pr.ResponseText = reply;
            return pr;
        }
        IProcessingResult AliasList(IProcessingResult pr, IBot bot, IChatMessage msg, string args)
        {
            var ca = bot.CommandAliases;
            var w2 = Helper.WordNumberN(args, 2);

            var aliases = new List<string>();
            foreach (var a in ca)
                if (a.Value == w2)
                    aliases.Add(a.Key);

            if (aliases.Count == 0)
            {
                pr.ResponseText = "Command *" + w2 + "* does not have any aliases.";
                return pr;
            }

            var txt1 = string.Join(", ", aliases);
            var reply = "Command *" + w2 + "* has ";
            if (txt1 != "")
                reply += aliases.Count + " aliases: " + txt1;
            reply += ".";

            pr.ReplyMessageID = msg.MessageID;
            pr.ResponseText = reply;
            return pr;
        }

        IProcessingResult Alias_case_Add(IProcessingResult pr, IBot bot, IChatMessage msg, string args)
        {
            var wc = Helper.WordCount(args);

            var a1 = Helper.WordNumberN(args, 1);
            var a2 = Helper.WordNumberN(args, 2);
            var wrest = Helper.TextAfter(args, a2);

            var ts = bot.TriggerSymbol;
            var ca = bot.CommandAliases;

            // currently only accepting 1 at a time

            if (wc < 2)
            {
                pr.ResponseText = "You must provide both target command name and at least one alias.";
                return pr;
            }

            if (wc > 2) // multiple aliases
            {
                bool o = false;
                if (args.EndsWith(" -overwrite") || args.EndsWith(" -o"))
                    o = true;

                return AliasAdd(pr, bot, msg, a1, a2, wrest, o);
            }

            // one alias
            if (wc == 2 || (wc == 3 && (args.EndsWith("-o") || args.EndsWith("-overwrite"))))
            {
                bool o = false;
                if (args.EndsWith(" -overwrite") || args.EndsWith(" -o"))
                    o = true;

                return AliasAdd(pr, bot, msg, a1, a2, o);
            }

            Log("[!] Unexpected!");
            pr.ResponseText = "(this was not supposed to happen)";
            return pr;
        }
        IProcessingResult AliasAdd(IProcessingResult pr, IBot bot, IChatMessage msg, string w2, string w3, string wrest, bool overwrite)
        {
            var ca = bot.CommandAliases;
            var arr = wrest.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var before = ca.Count;

            foreach (var alias in arr)
            {
                var alia = alias.TrimEnd(',');
                AliasAdd(pr, bot, msg, w2, alia, overwrite);
            }

            var delta = ca.Count - before;

            pr.ReplyMessageID = msg.MessageID;
            pr.ResponseText = "Set " + delta + " aliases out of " + arr.Length + ".";
            return pr;
        }
        IProcessingResult AliasAdd(IProcessingResult pr, IBot bot, IChatMessage msg, string command, string alias, bool overwrite)
        {
            var ca = bot.CommandAliases;
            var c = command;
            var a = alias;

            if (!overwrite)
                if (ca.ContainsKey(a))
                {
                    var cmdname = ca[a];

                    pr.ResponseText = "Command *" + cmdname + "* was already aliased to *" + a + "*. If you want to overwrite it, add \"-overwrite\" or \"-o\" to your request.";
                    return pr;
                }

            if (c.Contains("alias") || a.Contains("alias") || c == a || commandsList.Contains(a)) // sanity check
            {
                pr.ResponseText = "You can't do that.";
                return pr;
            }

            // just set
            ca[a] = c;
            Log("[+] Added new alias: \"" + alias + "\" for command \"" + command + "\".");

            pr.ReplyMessageID = msg.MessageID;
            pr.ResponseText = "Command *" + c + "* was aliased to *" + a + "*";
            return pr;
        }

        IProcessingResult Alias_case_Delete(IProcessingResult pr, IBot bot, IChatMessage msg, string args)
        {
            var wc = Helper.WordCount(args);
            var w2 = Helper.WordNumberN(args, 2);
            var ca = bot.CommandAliases;

            if (wc < 1)
            {
                pr.ResponseText = "Specify which alias you want to remove.";
                return pr;
            }
            if (wc > 1)
                return AliasesDelete(pr, bot, msg, args);
            else
            {
                if (ca.ContainsKey(w2)) /// uhh wut?
                    return AliasDelete(pr, bot, msg, w2);
                else
                {
                    pr.ResponseText = "There was no such alias defined.";
                    return pr;
                }
            }
        }
        IProcessingResult AliasesDelete(IProcessingResult pr, IBot bot, IChatMessage msg, string aliases)
        {
            var arr = aliases.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var before = bot.CommandAliases.Count;

            foreach (var alias in arr)
                AliasDelete(pr, bot, msg, alias);

            var delta = before - bot.CommandAliases.Count;

            pr.ReplyMessageID = msg.MessageID;
            pr.ResponseText = "Removed " + delta + " aliases out of " + arr.Length + ".";
            return pr;
        }
        IProcessingResult AliasDelete(IProcessingResult pr, IBot bot, IChatMessage msg, string alias)
        {
            var dic = bot.CommandAliases;

            var alia = alias.TrimEnd(',');
            if (dic.ContainsKey(alia))
            {
                dic.Remove(alia);
                Log("[−] Deleted alias by key \"" + alia + "\".");
            }
            else
                Log("[x] Could not delete alias by key \"" + alia + "\" because it was not present in alias dictionary.");

            pr.ReplyMessageID = msg.MessageID;
            pr.ResponseText = "Removed alias *" + alias + "*.";
            return pr;
        }

        public IProcessingResult Modules(IBot bot, IProcessingResult pr)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);


            // get all module files and their build date

            var modules = bot.GetLoadedModules();

            if (modules.Count < 1)
            {
                pr.ResponseText = "No modules loaded. This is very bad.";
                return pr;
            }

            var ret = "List of " + modules.Count + " loaded modules (name − build date):" + Environment.NewLine;
            foreach (var module in modules)
                ret += module.Name + " − " + module.BuildDate + Environment.NewLine;



            pr.ResponseText = ret;
            return pr;
        }

        public IProcessingResult Edit(IProcessingResult pr, IBot bot, IChatMessage msg, string arg)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);

            var editablePosts = bot.GetStillEditableMessages();

            if (editablePosts.Count < 1)
            {
                pr.ResponseText = "2 minutes have passed since last message was posted, apparently. Can't edit or delete it myself now.";
                pr.ReplyMessageID = msg.MessageID;
            }

            var wc = Helper.WordCount(arg);

            if (wc < 2)
            {
                pr.ResponseText = "You must specify a messageid and the new message text with a space between them.";
                return pr;
            }

            var a1 = Helper.FirstWord(arg).Trim().TrimStart(':');
            var wrest = Helper.TextAfter(arg, a1);

            if (a1.ToLowerInvariant() == "last")
            {
                pr.OwnResponseMessageIDToEdit = editablePosts[0].MessageID;
                pr.ResponseText = wrest;
            }
            else
            {
                int mid = 0;
                if (int.TryParse(a1, out mid))
                {

                    pr.OwnResponseMessageIDToEdit = mid;
                    pr.ResponseText = wrest;
                }
                else
                {
                    pr.ResponseText = "Could not parse messageid.";
                    return pr;
                }
            }

            return pr;
        }
        public IProcessingResult Unonebox(IProcessingResult pr, IBot bot, IChatMessage msg, string arg)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);

            { }
            // basically add "…" to the last message

            //pr = Edit(pr,bot,msg,);

            var editablePosts = bot.GetStillEditableMessages();

            if (editablePosts.Count < 1)
            {
                pr.ResponseText = "2 minutes have passed since last message was posted, apparently. Can't edit or delete it myself now.";
                pr.ReplyMessageID = msg.MessageID;
            }

            IChatMessage lastbotpost = null;
            try { lastbotpost = editablePosts[0]; }
            catch
            {
                pr.ResponseText = "Exception occurred while getting last editable post. Perhaps it's too late to edit the last message.";
                return pr;
            }

            // check if it's an image
            var regex = new System.Text.RegularExpressions.Regex(@"<div class=\""onebox ob-image\""><a rel=\""nofollow\""href=(.*)><img src=\"".*"" class=\""user-image\"" alt=\""user image\"" /></a>");
            var matches = regex.Matches(lastbotpost.Text);
            if (matches.Count == 1)
            {
                var b = matches[0].Groups[1].Value.Trim('"');
                pr.OwnResponseMessageIDToEdit = lastbotpost.MessageID;
                pr.ResponseText = b + " …";

                return pr;
            }

            var text = lastbotpost.Text;
            var untagged = Helper.ReplaceChatTags(text);

            pr.OwnResponseMessageIDToEdit = lastbotpost.MessageID;
            pr.ResponseText = untagged + " …";

            return pr;
        }
        public IProcessingResult Undo(IBot bot, IProcessingResult pr, string arg)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);

            var editablePosts = bot.GetStillEditableMessages();

            var wc = Helper.WordCount(arg);
            var a1 = Helper.FirstWord(arg).Trim().TrimStart(':');

            if (wc == 0)
            {
                if (editablePosts.Count > 0)
                {
                    var last = editablePosts[0];
                    bot.DeleteMessage(last.RoomID, last.MessageID);
                    pr.Respond = false;
                    return pr;
                }
                else
                {
                    pr.ResponseText = "No messages less than 2 minutes old.";
                    return pr;
                }
            }

            if (wc == 1)
            {
                if (a1 == "last")
                {
                    if (editablePosts.Count > 0)
                    {
                        var last = editablePosts[0];
                        bot.DeleteMessage(last.RoomID, last.MessageID);
                        pr.Respond = false;
                        return pr;
                    }
                    else
                    {
                        pr.ResponseText = "No messages less than 2 minutes old.";
                        return pr;
                    }
                }
                else
                {
                    int i = 0;
                    var b = int.TryParse(a1, out i);
                    if (!b)
                    {
                        pr.ResponseText = "Could not parse the number of messages to delete or messageid, whatever it was.";
                        return pr;
                    }
                    else
                    {
                        if (i < 100)
                        {
                            if (editablePosts.Count > 0)
                            {
                                if (editablePosts.Count >= i)
                                {
                                    // get i first posts to delete
                                    var todel = new List<IChatMessage>();
                                    for (int n = 0; n < i; n++)
                                        todel.Add(editablePosts[n]);

                                    foreach (var tod in todel)
                                        bot.DeleteMessage(tod.RoomID, tod.MessageID);

                                    pr.Respond = false;
                                    return pr;
                                }
                                else
                                {
                                    foreach (var tod in editablePosts)
                                        bot.DeleteMessage(tod.RoomID, tod.MessageID);

                                    pr.Respond = false;
                                    return pr;
                                }
                            }
                            else
                            {
                                pr.ResponseText = "No messages less than 2 minutes old.";
                                return pr;
                            }
                        }
                        else // delete by messageid
                        {
                            // try to get the messageid
                            foreach (var v in editablePosts)
                                if (v.MessageID == i)
                                {
                                    bot.DeleteMessage(v.RoomID, v.MessageID);

                                    pr.Respond = false;
                                    return pr;
                                }
                        }
                    }
                }
            }

            var wrest = Helper.TextAfter(arg, a1);

            if (a1.ToLowerInvariant() == "last")
            {
                var v = editablePosts[0].MessageID;

                var last = editablePosts[0];
                bot.DeleteMessage(last.RoomID, last.MessageID);

                pr.Respond = false;
                return pr;
            }
            else
            {
                int mid = 0;
                if (int.TryParse(a1, out mid))
                {

                    pr.OwnResponseMessageIDToEdit = mid;
                    pr.ResponseText = wrest;
                }
                else
                {
                    pr.ResponseText = "Could not parse messageid.";
                    return pr;
                }
            }


            return pr;
        }

        public IProcessingResult Uptime(IBot bot, IProcessingResult pr)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);

            var now = DateTime.UtcNow;
            var then = bot.StartTimeUTC;
            var diff = now - then;

            pr.ResponseText = "Bot app was launched at " + then + " (UTC), and has been running for " + diff + ".";

            return pr;
        }
        public IProcessingResult Status(IBot bot, IProcessingResult pr)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
            //pr.ResponseContent = "status message";
            //ResponseText = "Uptime: " + Helper.GetUptime(Bot.initTime) + ". Firefox memory use: " 
            var uptime = DateTime.UtcNow - bot.StartTimeUTC;
            var memusage = Helper.GetBotAppMemoryUsage();



            var logDir = bot.LogDirectoryPath;

            var logDirInfo = new DirectoryInfo(logDir);

            var logFileInfos = logDirInfo.GetFiles("*.log", SearchOption.AllDirectories);
            var logZipInfo = logDirInfo.GetFiles("*.zip", SearchOption.AllDirectories);

            long logFileSize = 0;
            foreach (var info in logFileInfos)
                logFileSize += info.Length;

            long logZipSize = 0;
            foreach (var info in logZipInfo)
                logZipSize += info.Length;

            var logText = logFileInfos.Length + " log files, size: " + logFileSize + "; " +
                                  logZipInfo.Length + " archives, size: " + logZipSize + ".";


            var potatoDir = bot.GetModuleByName("Potato").ModuleDir;
            var potDirInfo = new DirectoryInfo(potatoDir);

            var potFileInfos = potDirInfo.GetFiles("*.txt", SearchOption.AllDirectories);
            var potZipInfo = potDirInfo.GetFiles("*.zip", SearchOption.AllDirectories);

            long potSize = 0;
            long potZipSize = 0;

            foreach (var info in potFileInfos)
                potSize += info.Length;

            foreach (var info in potZipInfo)
                potZipSize += info.Length;

            var potText = potFileInfos.Length + " list files, size: " + potSize + "; " +
                                  potZipInfo.Length + " archives, size: " + potZipSize + ".";


            pr.ResponseText = "Bot app memory use: " + memusage + " bytes." +
                " Uptime: " + uptime + "." +
                " " + logText +
                " " + potText;

            return pr;
        }
        public IProcessingResult Save(IBot bot, IProcessingResult pr, int userid)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
            pr.ResponseText = "This command is limited to trusted users. Let's hope they won't abuse it.";

            if (bot.TrustedUserIDs != null &&
                bot.TrustedUserIDs.Contains(userid) ||
                userid == int.MinValue)
            {
                Log("[S] Save initiated by userid " + userid + ". Saving bot.");
                //DebugLogManager.Save();
                bot.Save();
                return pr; // it won't really be posted
            }

            return pr;
        }
        public IProcessingResult Shutdown(IBot bot, IProcessingResult pr, string arg, int userid)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
            pr.ResponseText = "This command is limited to trusted users. Let's hope they won't abuse it.";

            if (bot.TrustedUserIDs != null && bot.TrustedUserIDs.Contains(userid) ||
                userid == int.MinValue)
            {
                Log("[X] Shutdown initiated by userid " + userid + " with reason " + arg + ". Shutting down.");
                //DebugLogManager.Save();
                bot.SoftShutdown(arg);
                return pr; // it won't really be posted
            }

            return pr;
        }


        // ext

        public IProcessingResult Meme(IBot bot, IProcessingResult pr, IChatMessage msg, string arg)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);

            var w1 = Helper.WordNumberN(arg, 1);
            var rest = Helper.TextAfter(arg, w1);
            var w2 = "";
            var w3 = "";

            var reg = new Regex("\".*?\"");
            var matches = reg.Matches(rest);

            if (matches.Count < 1)
            {
                pr.ResponseText = "You must provide some text, or else the image will return unmodified.";
                return pr;
            }

            w2 = matches[0].Value.Trim('"');

            if (matches.Count == 2) // could be only top text
                w3 = matches[1].Value.Trim('"');


            var response = Thing(w1, w2, w3);

            pr.ResponseText = response.Result.ToString();

            return pr;
        }

        async Task<string> Thing(string img, string topText, string bottomText)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string> { { "uri", img }, { "top", topText }, { "bottom", bottomText }, { "nowebsocket", "true" } };

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync("http://caption.madara.ninja/", content);

                    var responseString = await response.Content.ReadAsStringAsync();

                    return responseString;
                }
            }
            catch (Exception ex) { return "[X] " + ex; }
        }


        // bonus
        public IProcessingResult Hats(IProcessingResult pr)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
            pr.ResponseText = "Burn the hats.";

            return pr;
        }


        // HoiHoi-san's borrowed commands

        public IProcessingResult Google(IProcessingResult pr, IChatMessage msg, string arg)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
            pr.ResponseText = "!!tell :" + msg.MessageID + " google " + arg;

            return pr;
        }
        public IProcessingResult Wiki(IProcessingResult pr, IChatMessage msg, string arg)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
            pr.ResponseText = "!!tell :" + msg.MessageID + " wiki " + arg;

            return pr;
        }
        public IProcessingResult Urban(IProcessingResult pr, IChatMessage msg, string arg)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
            pr.ResponseText = "!!tell :" + msg.MessageID + " urban " + arg;

            return pr;
        }
        public IProcessingResult Youtube(IProcessingResult pr, IChatMessage msg, string arg)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
            pr.ResponseText = "!!tell :" + msg.MessageID + " youtube " + arg;

            return pr;
        }
        public IProcessingResult Weather(IProcessingResult pr, IChatMessage msg, string arg)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
            pr.ResponseText = "!!tell :" + msg.MessageID + " weather " + arg;

            return pr;
        }

        public IProcessingResult Say(IProcessingResult pr, string arg) // looks fine
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
            //ResponseType = MessageType.Success,
            //ResponseContent = "listcommands"

            pr.ResponseText = Helper.ReplaceChatTags(Helper.UnescapeChatMessages(arg));

            return pr;
        }
        public IProcessingResult ID(IBot bot, IProcessingResult pr, IChatMessage msg, string arg) // related to "Other" method above
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);

            string gris = @"https://www.google.com/searchbyimage?image_url=";
            string snao = @"http://saucenao.com/search.php?url=";
            string iqdb = @"http://iqdb.org/?url=";
            string tin = @"http://tineye.com/search?url=";
            string unparsedimagelink = msg.Text;
            string unescapedimagelink = System.Web.HttpUtility.HtmlDecode(unparsedimagelink);
            string uimagelink = Helper.WordNumberN(unescapedimagelink, 2);
            string imagelink = System.Web.HttpUtility.UrlEncode(uimagelink);
            string grislink = gris + imagelink;
            string snaolink = snao + imagelink;
            string iqdblink = iqdb + imagelink;
            string tinlink = tin + imagelink;
            string ugrislink = gris + uimagelink;
            string usnaolink = snao + uimagelink;
            string uiqdblink = iqdb + uimagelink;
            string utinlink = tin + uimagelink;

            string niceresponse = "[Google Reverse Image Search](" + grislink + "), [TinEye](" + tinlink + "), [SauceNAO](" + snaolink + "), [iqdb](" + iqdblink + ").";
            string shortresponse = ugrislink + " | " + utinlink + " | " + usnaolink + " | " + uiqdblink;

            pr.ResponseText = niceresponse.Length < 500 ? niceresponse : shortresponse;


            if (string.IsNullOrEmpty(arg))
                pr.ResponseText = "Which image do you want to identify? Say `" + bot.TriggerSymbol + "id hxxp://example.com/image.jpg` to get the id links.";

            return pr;
        }
        public IProcessingResult Isch(IBot bot, IProcessingResult pr, IChatMessage msg, string arg)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);

            var ts = bot.TriggerSymbol;

            string link = @"https://www.google.com/search?tbm=isch&q=";
            string unparsedtxt = msg.Text;
            string firstw = Helper.FirstWord(unparsedtxt);
            string args = unparsedtxt.Replace(firstw, "");
            args = System.Web.HttpUtility.UrlEncode(args);
            var words = args.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string slink = link + string.Join("+", words);

            pr.ResponseText = "[Google Image Search](" + slink + ").";

            if (string.IsNullOrEmpty(arg))
                pr.ResponseText = "Which images do you want to find? Say `" + ts + "isch anime vampire loli` to get the relevant image search results.";

            return pr;
        }
        public IProcessingResult LastPostedImageID(IBot bot, IProcessingResult pr, IChatMessage msg, string arg)
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);

            var v = bot.GetModuleByName("Potato");

            if (v != null)
            {
                var last = (string)v.GetParameter("last posted image url");

                string gris = @"https://www.google.com/searchbyimage?image_url=";
                string snao = @"http://saucenao.com/search.php?url=";
                string iqdb = @"http://iqdb.org/?url=";
                string tin = @"http://tineye.com/search?url=";
                string imagelink = last;
                string grislink = gris + imagelink;
                string snaolink = snao + imagelink;
                string iqdblink = iqdb + imagelink;
                string tinlink = tin + imagelink;

                string niceresponse = "[Google Reverse Image Search](" + grislink + "), [TinEye](" + tinlink + "), [SauceNAO](" + snaolink + "), [iqdb](" + iqdblink + ").";
                string shortresponse = grislink + " | " + tinlink + " | " + snaolink + " | " + iqdblink;

                pr.ResponseText = niceresponse.Length < 500 ? niceresponse : shortresponse;
            }

            return pr;
        }


        //public IProcessingResult Invite(IProcessingResult pr, string arg) // under construction
        //{
        //    pr.Solved = true;
        //    pr.Respond = false;
        //    pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
        //    //pr.ResponseContent = "status message";
        //    pr.Result = IProcessingResultType.Default;
        //    //ResponseText = "Uptime: " + Helper.GetUptime(Bot.initTime) + ". Firefox memory use: " 
        //    //pr.ResponseText = "(not implemented yet)";


        //    //{

        //    //}

        //    return pr;
        //}

        public IProcessingResult TrustedUsers(IBot bot, IProcessingResult pr, string arg) // ugh…
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);

            pr.ResponseText = "*Not implemented*";

            //ResponseType = MessageType.Success,
            //ResponseContent = "listcommands"

            //var ts = bot.TriggerSymbol;

            //var w1 = Helper.FirstWord(arg);
            //switch (w1)
            //{
            //    case "*":
            //    case "list":
            //    case "all":
            //        {
            //            pr.ResponseText = "Trusted user IDs: " + string.Join(", ", bot.TrustedUserIDs) + ".";
            //            return pr;
            //        }

            //    case "add":
            //        {
            //            pr.ResponseText = "This isn't implemented yet.";
            //            return pr;
            //        }
            //    case "remove":
            //        {
            //            pr.ResponseText = "This isn't implemented yet.";
            //            return pr;
            //        }
            //    case "change": // change trust level (specific commands?)
            //        {
            //            pr.ResponseText = "This isn't implemented yet.";
            //            return pr;
            //        }
            //    case "find":
            //        {
            //            pr.ResponseText = "This isn't implemented yet.";
            //            return pr;
            //        }

            //    default:
            //        {
            //            pr.ResponseText = "Command `trust` requires one of the following parameters: list, add, remove, change, find. Say `" + ts + "help trust` for more info.";
            //            return pr;
            //        }
            //}

            return pr;
        }
        public IProcessingResult BannedUsers(IBot bot, IProcessingResult pr, string arg) // not implemented
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);

            pr.ResponseText = "*Not implemented*";


            //ResponseType = MessageType.Success,
            //ResponseContent = "listcommands"

            //pr.ResponseText = "Can't ignore anyone at this time. It's better to talk to everyone now for testing purposes.";


            return pr;
        }
        public IProcessingResult Tell(IProcessingResult pr, string arg) // not implemented
        {
            pr.Solved = true;
            pr.Respond = true;
            pr.ResponseOrigin = Helper.GetExecutingMethodName(this);

            var w1 = Helper.FirstWord(arg);
            var wrest = Helper.TextAfter(arg, w1).Trim();

            pr.ResponseText = "@" + w1 + " " + wrest;

            return pr;
        }
        

    }
}
