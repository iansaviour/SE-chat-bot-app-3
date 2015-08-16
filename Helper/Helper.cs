using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

public static class Helper
{
    static public void ScrollToLastLine(TextBox tb)
    {
        var pos = tb.Text.LastIndexOf(Environment.NewLine) + 2;
        if (pos > -1)
            tb.SelectionStart = pos;
        tb.ScrollToCaret();
    }

    public static string UnescapeChatMessages(string chatmessage)
    {
        string str = chatmessage;

        //unescape normal stuff
        str = System.Web.HttpUtility.HtmlDecode(str);
        //str = System.Text.RegularExpressions.Regex.Unescape(chatmessage);


        // images: "<div class=\"onebox ob-image\"><a rel=\"nofollow\"href=\"https://googledrive.com/host/0B_8okaqRyPERVlM0OVVCS29TVzQ/576-24039-2362.png\"><img src=\"https://googledrive.com/host/0B_8okaqRyPERVlM0OVVCS29TVzQ/576-24039-2362.png\" class=\"user-image\" alt=\"user image\" /></a></div>"
        // start:     <div class=\"onebox ob-image\"><a rel=\"nofollow\"href=\"
        // end:       \"><img src=\"

        return str;
    }

    static Regex linkRegex = new Regex(@"(<a href="".*?"" rel=""nofollow"">.*?</a>)", RegexOptions.Compiled);
    static Regex urlRegex = new Regex(@"<a href=""(.*?)"" rel=""nofollow"">", RegexOptions.Compiled);
    static Regex texRegex = new Regex(@""" rel=""nofollow"">(.*?)<\/a>", RegexOptions.Compiled);

    public static string ReplaceChatTags(string chatmessage)
    {
        string str = chatmessage;

        str = str.Replace(@"<i>", @"*");
        str = str.Replace(@"</i>", @"*");

        str = str.Replace(@"<b>", @"**");
        str = str.Replace(@"</b>", @"**");

        str = str.Replace(@"<code>", @"`");
        str = str.Replace(@"</code>", @"`");



        return str;
    }
    static Random r = new Random();
    public static string RandomItem(List<string> list)
    {
        int len = list.Count;
        var i = r.Next(len);
        string item = list[i];
        return item;
    }
    public static int MessageContainsBotNameOrBeginsWithTriggerSymbol(string botName, string message, string triggerSymbol)
    {
        string text = message;

        int bestResult = 0; // 0 = no name found, 1 = indirect name, 2 = direct address, 3 = triggerSymbol

        if (text.StartsWith(triggerSymbol)) return 3;

        if (text.Contains("@" + botName + " ")) return 2;
        if (text.Contains(botName)) bestResult = 1;

        return bestResult;
    }
    public static string RemoveBotNamesAndTriggerSymbol(string botName, string text, string triggerSymbol)
    {
        // remove bot name from string without changing other words to lowercase (detect index or word in lowercase and apply to input text)

        string t = text.Trim();

        if (t.StartsWith(triggerSymbol))
        {
            while (t.StartsWith(triggerSymbol))
                t = t.Substring(triggerSymbol.Length);
            return t;
        }
        else
        {
            if (t.StartsWith("@"))
            //foreach (var name in botCurrentNames)
            {
                var address = "@" + botName;
                if (t.StartsWith(address))
                    t = t.Replace(address, "").TrimStart(',').Trim();
            }
            if (t.StartsWith(":"))
            {
                t = new Regex(@"^\:\d.*? (.*$)").Match(t).Groups[1].Value;
            }
        }

        return t;
    }

    // text

    /// <summary> Gets substring between first occurence of From and last occurence of To. </summary>
    static public string TextBetween(string input, string from, string to)
    {
        var f = input.IndexOf(from);
        var t = input.LastIndexOf(to);

        try
        {
            var r = input.Substring(f + from.Length, t - f - from.Length);
            return r;
        }
        catch { }

        return input;
    }
    static public string TextBefore(string input, string before)
    {
        var r = input;

        if (r.Contains(before))
            r = input.Substring(0, input.IndexOf(before));

        return r;
    }
    static public string TextAfter(string input, string after)
    {
        if (input == null || after == null) return null;

        var r = input;

        if (r.Contains(after))
            r = input.Substring(input.IndexOf(after) + after.Length);

        return r;
    }

    static public int WordCount(string input)
    {
        int n = -1;

        // trim of spaces before and after
        var npu = input.Trim();

        // replace consequent spaces to 1 space char
        var nu = System.Text.RegularExpressions.Regex.Replace(input, @"\s+", " ");

        var v = nu.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        n = v.Length;

        return n;
    }
    static public string FirstWord(string input)
    {
        if (input == null) return null;

        var r = input.Trim();

        if (r.Contains(" "))
            r = r.Substring(0, r.IndexOf(" "));

        return r;
    }
    static public string LastWord(string input)
    {
        var r = input.TrimStart();

        if (r.Contains(" "))
            r = r.Substring(r.LastIndexOf(" ") + 1);

        return r;
    }
    static public string LastNSymbols(string input, int length)
    {
        var r = input;

        if (length <= r.Length)
            r = input.Substring(input.Length - length);

        return r;
    }
    static public string WordNumberN(string input, int n)
    {
        string w = input.Trim();

        if (input.Contains(" "))
        {
            var words = w.Split(' ');
            if (n <= words.Length)
            {
                return words[n - 1]; // this sounds risky
            }
            else
            {
                return null;
            }
        }

        return input;
    }

    static public string InsertAfterFirstOccurrence(string original, string afterWhat, string insert)
    {
        string ret = original;

        if (ret.Contains(afterWhat))
        {
            var indexofoccurrence = original.IndexOf(afterWhat);
            var indexofWheretoInsert = indexofoccurrence + afterWhat.Length;

            var before = original.Substring(0, indexofWheretoInsert);
            var after = original.Substring(indexofWheretoInsert);

            var result = before + insert + after;
            ret = result;
        }
        return ret;
    }
    static public string FirstCharToUpper(string input)
    {
        if (String.IsNullOrEmpty(input))
            return input;
        return input.First().ToString().ToUpper() + String.Join("", input.Skip(1));
    }



    // time
    static public string GetDateTimeNowYYYYMMDD_HHMMSSms()
    {
        var dt = DateTime.Now;
        var str = dt.Year.ToString("D4") + "-" + dt.Month.ToString("D2") + "-" + dt.Day.ToString("D2") + " " + dt.Hour.ToString("D2") + "-" + dt.Minute.ToString("D2") + "-" + dt.Second.ToString("D2") + "-" + dt.Millisecond.ToString("D3");
        return str;
    }
    static public string GetDateTimeNowHHMMSS(DateTime dateTime = default(DateTime))
    {
        var dt = DateTime.Now;
        if (dateTime != default(DateTime))
            dt = dateTime;

        var str = dt.Hour.ToString("D2") + "-" + dt.Minute.ToString("D2") + "-" + dt.Second.ToString("D2");
        return str;
    }
    static public string GetTimeFormatted(TimeSpan ts)
    {
        var str = ts.Milliseconds.ToString("D3") + "";
        if (ts.TotalSeconds > 0) str = ts.Seconds.ToString("D2") + "." + str;
        if (ts.TotalMinutes > 0) str = ts.Minutes.ToString("D2") + ":" + str;
        if (ts.TotalHours > 0) str = ts.Hours.ToString("D2") + ":" + str;
        if (ts.TotalDays > 0) str = ts.Days + "." + str;
        return str;
    }


    // url
    static public bool IsURLValid(string suspiciousUrl)
    {
        Uri u;
        bool result = Uri.TryCreate(suspiciousUrl, UriKind.Absolute, out u) && (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps);
        return result;
    }


    // io
    /// <summary> If directory doesn't exist, attempts to create it. Returns true on success, false on failure. </summary>
    static public bool CreateDirectoryIfItDoesntExist(string path, bool pathIsFile = false)
    {
        var dir = path;

        if (pathIsFile)
            dir = Path.GetDirectoryName(path);

        try
        {
            // is dir really a directory?
            /// You could use File.Exists(url) and Directory.Exists(url)

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                return true;
            }
            return false;
        }
        catch { return false; }
    }

    static public FileInfo GetNewestFileByMask(string dir, string mask)
    {
        var q = new DirectoryInfo(dir);
        if (!q.Exists) return null;
        var w = q.GetFiles(mask);
        if (w.Length < 1) return null;

        var e = (from f in w orderby f.LastWriteTime descending select f).First();

        return e;
    }

    static public void ZipAndDeleteOldFiles(string zipPath, string dir, string pattern, int daysOld)
    {
        if (!Directory.Exists(dir)) return;
        CreateDirectoryIfItDoesntExist(zipPath, true);

        var files = Directory.GetFiles(dir, pattern).ToList();

        var old = GetFilesOlderThanNDays(files, daysOld);

        AddFilesToZip(zipPath, old);

        foreach (var file in old)
            File.Delete(file);
    }
    static public List<string> GetFilesOlderThanNDays(List<string> allFiles, int days)
    {
        var list = new List<string>();

        var now = DateTime.Now;

        foreach (var file in allFiles)
        {
            var info = new FileInfo(file);
            var lastwrite = info.LastWriteTime;
            var diff = now - lastwrite;
            if (diff.TotalDays > days)
                list.Add(file);
        }

        return list;
    }
    static public void AddFilesToZip(string zipPath, List<string> files)
    {
        var v = new Ionic.Zip.ZipFile(zipPath);

        v.AddFiles(files);
        v.Save();
    }

    // info
    static public Int64 GetBotAppMemoryUsage()
    {
        var p = Process.GetCurrentProcess();
        var ws = p.WorkingSet64;
        return ws;
    }

    // logging
    static public string GetExecutingMethodName(object caller, Exception ex = null)
    {
        string result = "Unknown";
        StackTrace trace;
        if (ex != null)
            trace = new StackTrace(ex);
        else
            trace = new StackTrace();

        // next frame after combine

        bool combine = false;

        for (int index = 0; index < trace.FrameCount; ++index)
        {
            StackFrame frame = trace.GetFrame(index);
            MethodBase method = frame.GetMethod();

            if (method.Name == "GetExecutingMethodName")
            {
                combine = true;
                continue;
            }
            if (combine)
            {
                result = string.Concat(method.DeclaringType.Name, ".", method.Name);
                break;
            }
        }

        return result;
    }


}
