using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace SE_chat_bot_app_3
{
    public static class DebugLogManager
    {
        public static string logDir = Path.Combine(Program.rootdir, "logs");
        public static int lastLen = 100;
        public static List<string> last = new List<string>(lastLen);
        private static List<string> fullLog = new List<string>();

        public static void Log(string text)
        {
            try
            {
                StackTrace stackTrace = new StackTrace();

                // get calling method name
                var v = stackTrace.GetFrame(2);
                var b = v.GetMethod().Name;
                var m = v.GetMethod().DeclaringType.Name;
                var n = " " + m + "." + b + ":";
                if (text.StartsWith("["))
                    text = Helper.InsertAfterFirstOccurrence(text, "]", n);

                string s = Helper.GetDateTimeNowYYYYMMDD_HHMMSSms() + " " + text;
                fullLog.Add(s);

                while (last.Count >= lastLen)
                    last.RemoveAt(0);

                last.Add(s);
            }
            catch { } // unfortunately, we cannot log errors about adding log messages :/
        }

        public static bool Save()
        {
            // only write what hasn't been written yet
            bool success = false;
            try
            {
                var path = Path.Combine(logDir, Helper.GetDateTimeNowYYYYMMDD_HHMMSSms() + ".log");
                Helper.CreateDirectoryIfItDoesntExist(logDir);
                Log("[S] Saving log to \"" + path + "\"…");
                File.WriteAllLines(path, fullLog);
                success = true;
            }
            catch (Exception ex) { Log("Exception occurred while trying to save log file:\n" + ex); }

            if (success)
            {
                fullLog.Clear();
                Log("[S] Log saved.");
            }

            return true;
        }

    }
}
