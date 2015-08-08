using SE_chat_bot_app_3.CommonInterfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Net;

public class ModulePotato : ModuleBase, IModule
{
    public new string Name { get { return "Potato"; } }
    public new string Description { get { return "Posts pictures."; } }
    public new string ModuleDir { get; private set; }

    public new bool Enabled { get { return config.enabled; } set { config.enabled = value; } }

    public new List<string> CommandList { get { return commandsList; } }
    private List<string> commandsList = new List<string>() { "post" };

    public new object GetParameter(string parameterName)
    {
        if (gettableParameterFunctionDictionary.ContainsKey(parameterName))
        {
            var function = gettableParameterFunctionDictionary[parameterName];
            var value = function();
            return value;
        }

        Log("[x] Parameter by the name \"" + parameterName + "\" was not found.");
        return null;
    }

    public new void Start(Action<string> debugLogDelegate, string moduleDir)
    {
        try
        {
            this.debugLogMethod = debugLogDelegate;
            this.ModuleDir = moduleDir;
            InitGPFDic();
            Load();
        }
        catch (Exception ex) { Log("[X] " + ex); }
    }
    public new IProcessingResult Enable(IProcessingResult pr)
    {
        Enabled = true;

        pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
        pr.ResponseText = Name + " module enabled.";
        pr.Respond = true;
        pr.Solved = true;

        return pr;
    }
    public new IProcessingResult Disable(IProcessingResult pr)
    {
        Enabled = false;

        pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
        pr.ResponseText = Name + " module disabled.";
        pr.Respond = true;
        pr.Solved = true;

        return pr;
    }
    public new IProcessingResult Update(IBot bot, IProcessingResult pr)
    {
        pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
        pr.Respond = false;
        pr.Solved = true;

        if (!Enabled)
        {
            pr.ResponseText = "module disabled";
            return pr;
        }

        if (unpostedImages.Count < 1)
        {
            if (!linksDepletedAlerted)
            {
                linksDepletedAlerted = true;
                Log("[x] Potato module has run out of image links.");
                if (lowLinksAlert)
                {
                    pr.Respond = true;
                    pr.ResponseText = "*Potato module has run out of links. Or there is a stupid bug somewhere.*";
                    pr.TargetRoomID = bot.MainRoomID;
                    return pr;
                }
            }
            return pr;
        }

        if (unpostedImages.Count < lowLinksNumber)
            if (!lowLinkCountAlerted)
            {
                lowLinkCountAlerted = true;
                Log("[!] Potato module is low on image links (less than " + lowLinksNumber + " left).");
                if (lowLinksAlert)
                {
                    pr.Respond = true;
                    pr.ResponseText = "Potato module is low on links (less than " + lowLinksNumber + " left). Please, refill soon.";
                    return pr;
                }
            }

        if (DateTime.Now.Subtract(dt_LastMessageTime) > ts_PostingInterval) // 15 minutes between autoposts minimum
            if (TimeSinceLastPost > ts_PostingInterval) // a short timeout to prevent multiposts
                if (NumberOfOtherMessagesSinceLastWallpaperPost(bot, bot.MainRoomID) >= MessageInterval) // n messages since last image posted by bot
                    if (bot.LatestReceivedChatMessage != null)
                    {
                        var timeSinceLastMessage =
                          DateTime.UtcNow.Subtract(bot.LatestReceivedChatMessage.PostedAtUTC);
                        if (timeSinceLastMessage >= ts_WaitInterval) // m seconds since last room message
                        {
                            Log("[a] Time post an image automatically.");
                            PostRandomPotato(bot, pr);
                        }
                    }

        return pr;
    }

    public new IProcessingResult Command(IBot bot, IProcessingResult pr, IChatMessage msg, string cmd, string arg, string cmdOriginal, string argOriginal)
    {
        pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
        pr.Respond = true;
        pr.Solved = true;


        var args = arg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string arg1 = null;
        string rest = null;
        if (args.Length > 0)
        {
            arg1 = args[0];
            rest = Helper.TextAfter(arg, arg1).Trim();
        }

        switch (arg1)
        {
            default: return ManualPost(bot, pr, msg, arg);

            case "?":
            case "/?":
            case "help":
            case "/help":
                {
                    pr.ResponseText = ""; // ?
                    return pr;
                }

            case "state":
            case "stats":
            case "status":
            case "info":
                {
                    pr.Respond = true;
                    pr.ResponseText = GetStatusText(bot, cmdOriginal);

                    if (pr.ResponseText.Length > 500)
                        pr.ResponseText += "\n.";

                    return pr;
                };

            case "tag":
            case "tags":
                return Tag(pr, msg, rest);

            case "uploader":
                return LastPostedImageInfo(pr, msg);

            case "start":
            case "enable": return Enable(pr); // these have permission settings and should be managed from outside
            case "stop":
            case "disable": return Disable(pr); // ah fuck it, let's do it here for now

            case "add": return Add(bot, pr, msg, rest);

            case "timeinterval": return SetTimeInterval(pr, msg, rest, cmd);
            case "postinterval": return SetPostInterval(pr, msg, rest, cmd);
            case "waitinterval": return SetWaitInterval(pr, msg, rest, cmd);
        }
    }


    // end of interface


    string ConfigPath { get { return Path.Combine(ModuleDir, Name + ".cfg"); } }
    string PostedSubdir { get { return Path.Combine(ModuleDir, "posted"); } }
    string UnpostedSubdir { get { return Path.Combine(ModuleDir, "unposted"); } }
    string PostedOldZipPath { get { return Path.Combine(PostedSubdir, "posted old.zip"); } }
    string UnpostedOldZipPath { get { return Path.Combine(UnpostedSubdir, "unposted old.zip"); } }
    string TagAliasesPath { get { return Path.Combine(ModuleDir, "tag aliases.json"); } }


    List<TaggedImage> unpostedImages = new List<TaggedImage>();
    List<TaggedImage> postedImages = new List<TaggedImage>();
    Dictionary<string, string> tagAliasesDic = new Dictionary<string, string>();


    bool lowLinkCountAlerted = false, linksDepletedAlerted = false;
    int lowLinksNumber = 100;
    bool lowLinksAlert = true;


    Random r = new Random();
    TaggedImage nextLink { get; set; }
    TaggedImage lastPostedImage { get; set; }

    int MessageInterval { get { return config.postinterval; } set { config.postinterval = value; } }
    DateTime dt_LastMessageTime { get { return config.dt_LastPostTime; } set { config.dt_LastPostTime = value; } }
    TimeSpan TimeSinceLastPost { get { return DateTime.Now.Subtract(dt_LastMessageTime); } }
    public TimeSpan ts_PostingInterval { get { return config.ts_IntervalBetweenAutoPosts; } set { config.ts_IntervalBetweenAutoPosts = value; } }
    public TimeSpan ts_WaitInterval { get { return config.ts_IntervalAfterLastMessage; } set { config.ts_IntervalAfterLastMessage = value; } }


    public new void Load()
    {
        LoadConfig();
        LoadTaggedImages();
        LoadTagAliases();

        if (postedImages.Count > 0)
            lastPostedImage = postedImages[postedImages.Count - 1];

        Log("[L] Loaded config, " + unpostedImages.Count + " unposted, " + postedImages.Count + " posted links.");
    }
    void LoadConfig()
    {
        try
        {
            config = JsonConvert.DeserializeObject<PotatoConfig>(File.ReadAllText(ConfigPath));
            Log("[L] Loaded config.");
        }
        catch (Exception ex) { Log("[X] Failed to load config:\n" + ex); }
    }
    void LoadTaggedImages()
    {
        try
        {
            var fiP = Helper.GetNewestFileByMask(PostedSubdir, "*.txt");
            var fiU = Helper.GetNewestFileByMask(UnpostedSubdir, "*.txt");

            if (fiP == null || fiU == null)
            {
                Log("[x] One of the image link files failed to load.");
                return;
            }

            string fileP = fiP.FullName;
            string fileU = fiU.FullName;

            var p = File.ReadAllText(fileP);
            var u = File.ReadAllText(fileU);

            List<TaggedImage> P = null, U = null;
            bool success = false;
            try
            {
                P = JsonConvert.DeserializeObject<List<TaggedImage>>(p);
                U = JsonConvert.DeserializeObject<List<TaggedImage>>(u);
                if (P == null || U == null)
                {
                    Log("[x] One of the image link files failed to deserialize.");
                    return;
                }
                success = true;
            }
            catch (Exception ex) { Log("[X] " + ex); }

            if (success)
            {
                postedImages = P;
                unpostedImages = U;

                Log("[L] Loaded " + postedImages.Count + " posted and " + unpostedImages.Count + "unposted image links.");
            }
            else
            {
                Log("[x] Failed to deserialize one of the json image lists.");
                postedImages = new List<TaggedImage>();
                unpostedImages = new List<TaggedImage>();
            }
        }
        catch (Exception ex) { Log("[X] " + ex); }
    }
    void LoadTagAliases()
    {
        try
        {
            tagAliasesDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(TagAliasesPath));
            Log("[L] Loaded tag aliases.");
        }
        catch (JsonException jex) { Log("[X] Failed to load tag aliases:\n" + jex); }
    }


    public new void Save()
    {
        SaveConfig();
        SaveTaggedImages();
        SaveTagAliases();

        Log("[S] Saved settings, " + unpostedImages.Count + " unposted, " + postedImages.Count + " posted image links.");
    }
    void SaveConfig()
    {
        try
        {
            var dir = Path.GetDirectoryName(Path.GetDirectoryName(ConfigPath));
            Helper.CreateDirectoryIfItDoesntExist(dir);

            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(config));

            Log("[S] Saved config.");
        }
        catch (Exception ex) { Log("[X] Failed to save config:\n" + ex); }
    }
    void SaveTaggedImages()
    {
        string fileP = Path.Combine(PostedSubdir, Helper.GetDateTimeNowYYYYMMDD_HHMMSSms() + " - links.txt");
        string fileU = Path.Combine(UnpostedSubdir, Helper.GetDateTimeNowYYYYMMDD_HHMMSSms() + " - links.txt");

        try
        {
            File.WriteAllText(fileP, JsonConvert.SerializeObject(postedImages, Formatting.Indented));
            File.WriteAllText(fileU, JsonConvert.SerializeObject(unpostedImages, Formatting.Indented));
            Log("[S] Saved image links.");
        }
        catch (Exception ex) { Log("[x] Failed to save image links:\n" + ex); }

        try
        {
            Helper.ZipAndDeleteOldFiles(PostedOldZipPath, PostedSubdir, "*.txt", 7);
            Helper.ZipAndDeleteOldFiles(UnpostedOldZipPath, UnpostedSubdir, "*.txt", 7);
        }
        catch (Exception ex) { Log("[x] Failed to zip and delete old files:\n" + ex); }
    }
    void SaveTagAliases()
    {
        try
        {
            var dir = Path.GetDirectoryName(Path.GetDirectoryName(TagAliasesPath));
            Helper.CreateDirectoryIfItDoesntExist(dir);

            File.WriteAllText(TagAliasesPath, JsonConvert.SerializeObject(tagAliasesDic));

            Log("[S] Saved tag aliases.");
        }
        catch (Exception ex) { Log("[X] Failed to save tag aliases:\n" + ex); }
    }


    IProcessingResult SetMessageInterval(IProcessingResult pr, IChatMessage msg, string newMessageInterval)
    {
        pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
        pr.Respond = true;
        pr.Solved = true;

        if (newMessageInterval == "")
        {
            pr.ResponseText = "*Pictures are currently set to be posted not more often than in-between " + MessageInterval + " other messages.*";
            return pr;
        }

        int n = -1;

        try
        { n = int.Parse(newMessageInterval); }
        catch (Exception ex)
        {
            pr.ResponseText = "*Could not parse string \"" + newMessageInterval + "\" as integer.*";
            Log("[X] Cloud not parse string \"" + newMessageInterval + "\" as integer:\n" + ex);
            return pr;
        }

        if (n < 1)
        {
            pr.ResponseText = "New interval was less than 1 message. Only positive time intervals are accepted for this setting.";
            return pr;
        }
        if (n > 5000)
        {
            pr.ResponseText = "Can't set potato posting interval larger than five thousand messages. Might as well just disable it altogether.";
            return pr;
        }

        MessageInterval = n;

        pr.ResponseText = "*Potatoes will be posted not more often than every " + n + " non-potato messages.*";

        return pr;
    }



    Dictionary<string, Func<object>> gettableParameterFunctionDictionary;
    void InitGPFDic()
    {
        gettableParameterFunctionDictionary = new Dictionary<string, Func<object>>() 
        { 
          { "last posted image url", GetLastPostedImageUrl },
          { "last posted image short info", GetLastPostedImageInfo }
        };
    }

    string GetLastPostedImageUrl() { return lastPostedImage.url; }


    public IProcessingResult Add(IBot bot, IProcessingResult pr, IChatMessage msg, string arg)
    {
        pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
        pr.Respond = true;
        pr.Solved = true;

        if (arg == "")
        {
            pr.ResponseText = "*Argument cannot be empty.*";
            return pr;
        }

        Log("[…] Processing argument.");

        string link = null;
        string tag = null;

        var words = arg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (words.Length > 0)
            link = words[0].Trim();

        if (words.Length > 1)
            tag = Helper.TextAfter(arg, link).Trim();

        return AddTextFileLinks(bot, pr, msg, link, tag);

        //pr.ResponseText = "You can only add raw text files with image links on each line. Make sure the url ends with `.txt` or contains `pastebin.com/raw.php?i=`.";
        //return pr;
    }
    IProcessingResult AddTextFileLinks(IBot bot, IProcessingResult pr, IChatMessage msg, string arg, string tag)
    {
        pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
        pr.Respond = true;
        pr.Solved = true;

        string link = arg;

        if (!Helper.IsURLValid(link))
        {
            link = Helper.TextBetween(link, @"/… <", @">");

            if (!Helper.IsURLValid(link))
            {
                pr.ResponseText = "Error: The link you provided didn't pass `Uri.TryCreate` test. Or I wasn't able to parse it correctly for some reason.";
                return pr;
            }
        }

        // get the txt file

        string text = "";
        try
        {
            using (var client = new WebClientHeadOnly())
            {
                client.HeadOnly = true;
                string uri = link;
                byte[] body = client.DownloadData(uri); // note: should be 0-length
                string type = client.ResponseHeaders["content-type"];
                if (type.StartsWith(@"text/")) // check for text/html or something...
                {
                    client.HeadOnly = false;
                    text = client.DownloadString(uri);
                    Log("[↓] Downloaded text file with links successfully.");
                }
                else
                {
                    pr.ResponseText = "Error: linked file was not in plan text format. MIME type: \"" + type + "\".";
                    return pr;
                }
            }

            var dir = Path.Combine(ModuleDir, "Added links");
            Helper.CreateDirectoryIfItDoesntExist(dir);

            var username = msg.UserName;
            string invalidPathChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalidPathChars)
                username = username.Replace(c.ToString(), "");

            var path = Path.Combine(dir, msg.MessageID + " " + msg.UserID + " " + username + ".txt");
            var mj = JsonConvert.SerializeObject(msg, Formatting.Indented);

            var contents = mj + Environment.NewLine + text;

            File.WriteAllText(path, contents);
        }
        catch (Exception ex)
        {
            Log("[X] Exception while downloading the file:\n" + ex);
            pr.ResponseText = "Exception occured while downloading the text file.";
            return pr;
        }

        // split text into separate links
        var lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        var allPostedImages = new List<string>();
        foreach (var ti in postedImages)
            allPostedImages.Add(ti.url);

        var allUnpostedImages = new List<string>();
        foreach (var ti in unpostedImages)
            allUnpostedImages.Add(ti.url);

        int postedLinksFound = 0;
        int unpostedLinksFound = 0;
        var valid = new List<TaggedImage>();
        var invalid = new List<TaggedImage>();
        var pstd = new List<string>(); // these are just for data lust
        var unpstd = new List<string>();

        foreach (var line in lines)
        {
            var words = line.Split(' ');

            var url = words[0];
            var tagsonly = line.Replace(url, "").Trim().Split(' ');
            var tags = new List<string>(tagsonly);

            // check if it has been posted before or is in unposted links collection
            if (allPostedImages.Contains(url))
            {
                postedLinksFound++;
                pstd.Add(url);
                continue;
            }
            if (allUnpostedImages.Contains(url))
            {
                unpostedLinksFound++;
                unpstd.Add(url);
                continue;
            }

            var ti = new TaggedImage();
            ti.url = url;
            ti.uploaderUserID = msg.UserID;
            ti.uploaderUserName = msg.UserName;
            ti.uploadDateUTC = DateTime.UtcNow;

            if (tag != null)
                ti.tag = tag;

            if (Helper.IsURLValid(url))
                valid.Add(ti);
            else
                invalid.Add(ti);
        }

        unpostedImages.AddRange(valid);

        Log("[…] Processed " + lines.Length + " lines, found " + invalid.Count + " invalid, " + postedLinksFound + " posted, " + unpostedLinksFound + " unposted and " + valid.Count + " new links.");

        Save();

        // report result
        string reply = "Links processed: " + lines.Length + ", valid: " + valid.Count + ", invalid: " + invalid.Count + ". Posted: " + pstd.Count + ", unposted: " + unpstd.Count + ". New links added: " + valid.Count + ".";

        if (tag != null)
            reply += " Images have been assigned tag \"" + tag + "\".";

        pr.ResponseText = reply;
        return pr;
    }


    public string GetStatusText(IBot bot, string cmdOriginal)
    {
        string ret = Helper.FirstCharToUpper(cmdOriginal) + " module status: ";

        if (Enabled)
            ret += "**Enabled**";
        else
            ret += "**Disabled**";

        ret += ". *" + ts_PostingInterval.TotalMinutes + "* minutes after the previous post, " + Helper.FirstCharToUpper(cmdOriginal) + " module will start checking if *" + MessageInterval + "* messages have been posted since, and then wait *" + ts_WaitInterval.TotalSeconds + "* more seconds after the latest message before posting.";
        ret += " Links unposted: " + unpostedImages.Count + ", links posted: " + postedImages.Count + ", Total: " + (unpostedImages.Count + postedImages.Count) + ". ";


        var imageDic = new Dictionary<string, int>();
        foreach (var ti in unpostedImages)
            if (imageDic.ContainsKey(ti.uploaderUserName))
                imageDic[ti.uploaderUserName]++;
            else
                imageDic[ti.uploaderUserName] = 1;

        if (imageDic.Count > 0)
        {
            string imagesByUser = "Image count by user:";

            foreach (var kvp in imageDic)
                imagesByUser += " " + kvp.Key + ": " + kvp.Value + ";";

            ret += imagesByUser;
        }

        if (unpostedImages.Count > 0)
            ret += " " + GetNextMessageTime(bot, cmdOriginal);
        else
            ret += " No unposted links left in storage. Please, refill with new links.";

        return ret;
    }
    bool waitingForPostedLinkToAppear = false;

    public string GetNextMessageTime(IBot bot, string cmdOriginal)
    {
        if (waitingForPostedLinkToAppear)
            return "Waiting for current " + Helper.FirstCharToUpper(cmdOriginal) + " to appear in parsed messages.";

        var delta = TimeSinceLastPost;

        var num = NumberOfOtherMessagesSinceLastWallpaperPost(bot, bot.MainRoomID);
        if (num >= MessageInterval)
        {
            if (bot.LatestReceivedChatMessage == null)
                return "Last chat message is null. So waiting for *" + config.postinterval + "* messages to be posted before sending a pic.";

            if (DateTime.UtcNow.Subtract(bot.LatestReceivedChatMessage.PostedAtUTC) >= config.ts_IntervalAfterLastMessage)
                return "Next " + Helper.FirstCharToUpper(cmdOriginal) + " post is due *now*.";
            else
                return "Waiting for *" + (int)ts_WaitInterval.Subtract(DateTime.UtcNow.Subtract(bot.LatestReceivedChatMessage.PostedAtUTC)).TotalSeconds + "* seconds to pass after the last message.";
        }
        else
        {
            var n = MessageInterval - num;
            if (n.ToString().EndsWith("1"))
                return "Next " + Helper.FirstCharToUpper(cmdOriginal) + " post is due in " + n + " message.";
            else
                return "Next " + Helper.FirstCharToUpper(cmdOriginal) + " post is due in " + n + " messages.";
        }
    }

    static int lastMessageIDWhenImageWasPosted = -999;
    public static int NumberOfOtherMessagesSinceLastWallpaperPost(IBot bot, int roomid)
    {
        // detect wallpaper posts in chat

        var n = 0;
        var tr = bot.GetTranscript(roomid);
        if (tr == null) return 0;
        var tran = Enumerable.Reverse(tr);

        foreach (var msg in tran)
        {
            if (msg.MessageID == lastMessageIDWhenImageWasPosted)
                //if (msg.UserID == bot.UserID)
                //    if (msg.Text.StartsWith("<div class=\"onebox ob-image\">"))
                break;
            n++;
        }

        return n;
    }

    IProcessingResult ManualPost(IBot bot, IProcessingResult pr, IChatMessage msg, string arg)
    {
        pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
        pr.Respond = true;
        pr.Solved = true;

        if (msg.RoomID != bot.MainRoomID)
        {
            pr.ResponseText = "*This feature is intended to work in the [main room](" + @"http://chat.stackexchange.com/rooms/6697/maid-cafe-" + ") only.*";
            pr.ReplyMessageID = msg.MessageID;
            pr.TargetRoomID = msg.RoomID;

            return pr;
        }

        if (string.IsNullOrEmpty(arg))
            pr = PostRandomPotato(bot, pr);
        else
            pr = PostPotatoTagged(bot, pr, msg, arg);

        return pr;
    }
    IProcessingResult PostRandomPotato(IBot bot, IProcessingResult pr)
    {
        if (unpostedImages.Count < 1)
        {
            Log("[x] No unposted links left to post.");
            pr.Respond = true;
            pr.ResponseText = "No unposted image links left in database.";
            return pr;
        }


        // get list of images for every user
        var imageDic = new Dictionary<int, List<TaggedImage>>();
        foreach (var ti in unpostedImages)
            if (imageDic.ContainsKey(ti.uploaderUserID))
                imageDic[ti.uploaderUserID].Add(ti);
            else
                imageDic[ti.uploaderUserID] = new List<TaggedImage>() { ti };

        var maxWeight = 500;

        int i = 0;
        var countDic = new Dictionary<int, int>(); // just for kvp use
        foreach (var kvp in imageDic)
        {
            var userID = kvp.Key;
            var imageCount = kvp.Value.Count;
            if (imageCount > maxWeight)
                imageCount = maxWeight;

            i += imageCount;

            countDic[userID] = i;
        }

        // still get a random number and multiply by total image count and see on which user's range it landed

        var random = r.NextDouble();
        var magical = (int)Math.Floor(random * i);

        var userIdWhosePicGetsPosted = -1;

        foreach (var kvp in countDic)
            if (magical <= kvp.Value)
            {
                userIdWhosePicGetsPosted = kvp.Key;
                break;
            }

        var pool = imageDic[userIdWhosePicGetsPosted];

        // get a random link to post
        var nextLinkListIndex = r.Next(pool.Count);
        var nextImage = pool[nextLinkListIndex];
        nextLink = nextImage;

        // set it
        pr.Respond = true;
        pr.ResponseText = nextLink.url;
        pr.TargetRoomID = bot.MainRoomID;

        // remove link from queue
        unpostedImages.Remove(nextImage);

        postedImages.Add(nextImage);
        lastPostedImage = nextLink;

        dt_LastMessageTime = DateTime.Now;
        SetLastMessageIDWhenPosted(bot, bot.MainRoomID);

        Save();

        return pr;
    }

    void SetLastMessageIDWhenPosted(IBot bot, int roomid)
    {
        var tr = bot.GetTranscript(roomid);
        if (tr.Count < 1)
            return;

        var msg = tr[tr.Count - 1];
        var id = msg.MessageID;

        lastMessageIDWhenImageWasPosted = id;
    }

    IProcessingResult PostPotatoTagged(IBot bot, IProcessingResult pr, IChatMessage msg, string tag)
    {
        pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
        pr.Respond = true;
        pr.Solved = true;

        // find alias
        string aliasedOriginalTag = null;
        if (tagAliasesDic.ContainsKey(tag))
            aliasedOriginalTag = tagAliasesDic[tag];

        // find all images with that tag

        var list = new List<TaggedImage>();
        foreach (var ti in unpostedImages)
            if (ti.tag != null)
                if (ti.tag == tag || ti.tag == aliasedOriginalTag)
                    list.Add(ti);

        if (list.Count < 1)
            return PostRandomPotato(bot, pr);
        else
        {
            // post random pic from list and do the rest as usual

            // get a random link to post
            var n = r.Next(list.Count);
            var nextImage = list[n];
            nextLink = nextImage;

            // set it
            pr.ResponseText = nextLink.url;
            pr.TargetRoomID = msg.RoomID;

            // move image from one list to another
            unpostedImages.Remove(nextImage);
            postedImages.Add(nextImage);

            lastPostedImage = nextLink;

            dt_LastMessageTime = DateTime.Now;
            SetLastMessageIDWhenPosted(bot, msg.RoomID);

            Save();

            return pr;
        }
    }


    // admins can rename any tags
    // uploaders can rename their own tags (warn about merging with existing tag pool)
    // remove all images uploaded by userid
    // ban userid from uploading (admin rights)

    IProcessingResult Tag(IProcessingResult pr, IChatMessage msg, string arg)
    {
        /// _______ arg1
        // #post tag ghost in the shell = gits
        // #post tag jintai += jinrui wa suitai shimashita

        // #post tag remove naruto shipuuden
        // #post tag -= kuroko basket


        if (arg.Contains(" = "))
        {
            var add1W1 = Helper.TextBefore(arg, " = ").ToLowerInvariant();
            var add1W2 = Helper.TextAfter(arg, " = ").ToLowerInvariant();

            return TagAdd(pr, msg, add1W1, add1W2);
        }

        var arg1 = Helper.FirstWord(arg);
        var rest = Helper.TextAfter(arg, arg1).Trim();
        switch (arg1)
        {
            case "":
            case "*": return TagPostAll(pr, msg);

            case "alias":
            case "aliases": return TagPostAllAliases(pr, msg, rest);

            case "remove": return TagRemove(pr, msg, rest);
        }

        pr.ResponseText = "Unexpected argument. Refer to manual for help.";
        return pr;
    }

    // original = A, alias1: A = B, alias2: b = c → a = c — find original tag and alias to it instead of another alias
    // protect against recursive tag aliases

    IProcessingResult TagAdd(IProcessingResult pr, IChatMessage msg, string originalTag, string newTagAlias)
    {
        pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
        pr.Respond = true;
        pr.Solved = true;
        pr.ReplyMessageID = msg.MessageID;

        // check if it already exists, etc
        if (tagAliasesDic.ContainsKey(newTagAlias))
        {
            pr.ResponseText = "Tag alias \"" + newTagAlias + "\" is already set for tag \"" + tagAliasesDic[newTagAlias] + "\". To set alias for another tag, you must first remove it.";
            return pr;
        }

        tagAliasesDic[newTagAlias] = originalTag;
        SaveTagAliases();

        pr.ResponseText = "Tag \"" + originalTag + "\" was aliased to \"" + newTagAlias + "\".";
        return pr;
    }
    IProcessingResult TagRemove(IProcessingResult pr, IChatMessage msg, string arg)
    {
        pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
        pr.Respond = true;
        pr.Solved = true;
        pr.ReplyMessageID = msg.MessageID;

        var alias = arg.ToLowerInvariant();

        if (tagAliasesDic.ContainsKey(alias))
        {
            var tag = tagAliasesDic[alias];
            tagAliasesDic.Remove(alias);
            SaveTagAliases();

            pr.ResponseText = "Removed alias \"" + alias + "\" from tag \"" + tag + "\".";
            return pr;
        }
        else
        {
            pr.ResponseText = "There are no tags aliased to \"" + alias + "\".";
            return pr;
        }
    }
    IProcessingResult TagPostAllAliases(IProcessingResult pr, IChatMessage msg, string arg)
    {
        pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
        pr.Respond = true;
        pr.Solved = true;
        pr.ReplyMessageID = msg.MessageID;

        if (string.IsNullOrEmpty(arg.Trim()))
        {
            // post all aliases for all tags

            var aliases = new Dictionary<string, List<string>>();

            foreach (var kvp in tagAliasesDic)
            {
                if (aliases.ContainsKey(kvp.Value))
                    aliases[kvp.Value].Add(kvp.Key);
                else
                    aliases[kvp.Value] = new List<string>() { kvp.Key };
            }

            string r = "There are " + aliases.Count + " tags with " + tagAliasesDic.Count + " aliases. ";

            foreach (var kvp in aliases)
                r += kvp.Key + ": " + string.Join(", ", kvp.Value) + "; ";

            r = r.Substring(0, r.Length - 2) + ".";

            pr.ResponseText = r;
        }
        else // just for arg
        {
            var tag = arg.ToLowerInvariant();

            pr.ResponseText = "(test) list aliases for tag: \"" + tag + "\".";
        }
        return pr;
    }
    IProcessingResult TagPostAll(IProcessingResult pr, IChatMessage msg)
    {
        pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
        pr.Respond = true;
        pr.Solved = true;
        pr.ReplyMessageID = msg.MessageID;

        // make a list of all categories
        var tagPoolDic = new Dictionary<string, int>();
        var uncat = 0;

        foreach (var ti in unpostedImages)
        {
            var cat = ti.tag;

            if (cat == null)
                uncat++;
            else
                if (!tagPoolDic.ContainsKey(cat))
                    tagPoolDic[cat] = 1;
                else
                    tagPoolDic[cat]++;
        }

        if (tagPoolDic.Count < 1)
        {
            pr.ResponseText = "There are no images with categories set in unposted images list.";
            return pr;
        }

        var r = "There are " + unpostedImages.Count + " unposted images in " + tagPoolDic.Count + " tagged lists: ";

        if (uncat > 0)
            r += "(untagged − " + uncat + "); ";

        var sorted = GetTagPoolItemCountsDescending(tagPoolDic);

        foreach (var cat in sorted)
            r += cat.Key + " − " + cat.Value + "; ";

        r = r.Substring(0, r.Length - 2) + "."; // replace last ; with .

        pr.ResponseText = r;

        return pr;
    }

    List<KeyValuePair<TKey, TValue>> GetTagPoolItemCountsDescending<TKey, TValue>(Dictionary<TKey, TValue> dic)
    {
        return dic.ToList().OrderByDescending(x => x.Value).ToList();

        //return dic.ToSortedDictionaryByValueAscending().ToArray().Reverse();
    }


    IProcessingResult LastPostedImageInfo(IProcessingResult pr, IChatMessage msg)
    {
        pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
        pr.Respond = true;
        pr.Solved = true;
        pr.ReplyMessageID = msg.MessageID;

        var ti = lastPostedImage;

        pr.ResponseText = GetLastPostedImageInfo();

        return pr;
    }

    string GetLastPostedImageInfo()
    {
        var ti = lastPostedImage;
        var info = "Uploaded (UTC): " + ti.uploadDateUTC +
            " by (" + ti.uploaderUserID + ") " + ti.uploaderUserName +
            ", tag: " + ti.tag;

        return info;
    }


    public IProcessingResult SetTimeInterval(IProcessingResult pr, IChatMessage msg, string newTimeInterval, string cmdOriginal)
    {
        pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
        pr.Respond = true;
        pr.Solved = true;

        if (newTimeInterval == "" || newTimeInterval == null)
        {
            pr.ResponseText = Helper.FirstCharToUpper(cmdOriginal) + "s are currently set to be posted not more often than every *" + ts_PostingInterval.TotalMinutes + "* minutes.";
            return pr;
        }

        int n = -1;

        try
        { n = int.Parse(newTimeInterval); }
        catch (Exception ex)
        {
            pr.ResponseText = "Could not parse string \"" + newTimeInterval + "\" as integer.";
            Log("[X] Cloud not parse string \"" + newTimeInterval + "\" as integer:\n" + ex);
            return pr;
        }

        if (n < 1)
        {
            pr.ResponseText = "New interval was less than 1 second. Only positive time intervals are accepted for this setting.";
            return pr;
        }

        if (n > 9000 - 1)
        {
            pr.ResponseText = "New interval was greater than 9000 seconds. Only time intervals less than or equal to 2.5 hours are accepted for this setting.";
            return pr;
        }

        ts_PostingInterval = new TimeSpan(0, n, 0);

        pr.ResponseText = Helper.FirstCharToUpper(cmdOriginal) + "s will be posted not more often than every *" + n + "* minutes.";
        return pr;
    }
    public IProcessingResult SetPostInterval(IProcessingResult pr, IChatMessage msg, string newPostInterval, string cmdOriginal)
    {
        pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
        pr.Respond = true;
        pr.Solved = true;

        if (newPostInterval == "" || newPostInterval == null)
        {
            pr.ResponseText = Helper.FirstCharToUpper(cmdOriginal) + "s are currently set to be posted not more often than in-between *" + MessageInterval + "* other messages.";
            return pr;
        }

        int n = -1;

        try
        { n = int.Parse(newPostInterval); }
        catch (Exception ex)
        {
            pr.ResponseText = "Could not parse string \"" + newPostInterval + "\" as integer.";
            Log("[X] Cloud not parse string \"" + newPostInterval + "\" as integer:\n" + ex);
            return pr;
        }

        if (n < 1)
        {
            pr.ResponseText = "New interval was less than 1 message. Only positive time intervals are accepted for this setting.";
            return pr;
        }
        if (n > 50000)
        {
            pr.ResponseText = "Can't set " + Helper.FirstCharToUpper(cmdOriginal) + " posting interval larger than fifty thousand messages. Might as well just disable it altogether.";
            return pr;
        }

        MessageInterval = n;

        pr.ResponseText = Helper.FirstCharToUpper(cmdOriginal) + "s will be posted not more often than in-between *" + n + "* other messages.";
        return pr;
    }
    public IProcessingResult SetWaitInterval(IProcessingResult pr, IChatMessage msg, string newWaitInterval, string cmdOriginal)
    {
        pr.ResponseOrigin = Helper.GetExecutingMethodName(this);
        pr.Respond = true;
        pr.Solved = true;

        if (newWaitInterval == "" || newWaitInterval == null)
        {
            pr.ResponseText = Helper.FirstCharToUpper(cmdOriginal) + "s are currently set to be posted not quicker than *" + ts_WaitInterval.TotalSeconds + "* seconds after the latest message.";
            return pr;
        }

        int n = -1;

        try
        { n = int.Parse(newWaitInterval); }
        catch (Exception ex)
        {
            pr.ResponseText = "Could not parse string \"" + newWaitInterval + "\" as integer.";
            Log("[X] Cloud not parse string \"" + newWaitInterval + "\" as integer:\n" + ex);
            return pr;
        }

        if (n < 1)
        {
            pr.ResponseText = "New wait interval was less than 1 second. Only positive time intervals are accepted for this setting.";
            return pr;
        }

        if (n > 300 - 1)
        {
            pr.ResponseText = "New wait interval was greater than 300 seconds. Only time intervals less than or equal to 5 minutes are accepted for this setting.";
            return pr;
        }

        ts_WaitInterval = new TimeSpan(0, 0, n);

        pr.ResponseText = Helper.FirstCharToUpper(cmdOriginal) + "s will be posted not quicker than *" + n + "* seconds after the latest message.";
        return pr;
    }

    PotatoConfig config = new PotatoConfig();

    public class PotatoConfig
    {
        public bool enabled = false;
        public TimeSpan ts_IntervalBetweenAutoPosts = new TimeSpan(0, 15, 0);
        public TimeSpan ts_IntervalAfterLastMessage = new TimeSpan(0, 0, 30);
        public int postinterval = 50;
        public DateTime dt_LastPostTime = DateTime.MinValue;

        public Dictionary<int, int> weightLimitDic = new Dictionary<int, int>();

    }

    public class TaggedImage
    {
        public string url;
        public int uploaderUserID;
        public string uploaderUserName;
        public DateTime uploadDateUTC;

        public string tag;
    }

    public class WebClientHeadOnly : WebClient
    {
        public bool HeadOnly { get; set; }
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (HeadOnly && request.Method == "GET")
                request.Method = "HEAD";
            return request;
        }
    }
}

