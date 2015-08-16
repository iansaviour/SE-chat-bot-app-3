using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SE_chat_bot_app_3
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        static void Log(string text) { DebugLogManager.Log(text); }
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                EmptyForm();

                var b = Bot.FromConfig();

                if (b != null)
                {
                    bot = b;
                    Log("[L] Bot loaded from config.");
                }
                else
                {
                    bot = new Bot();
                    l_botConfigNotFound.Visible = true;
                }

                PopulateForm();

                if (bot.CredentialsNotEmpty)
                    StartBot();
                else
                    Log("[x] Bot credentials not valid for automatic start.");

                t_updateRoomTranscript.Tick += t_updateRoomTranscript_Tick;
                t_updateRoomTranscript.Start();
                t_logUpdate.Tick += t_logUpdate_Tick;
                t_logUpdate.Start();
                t_statusStrips.Tick += t_statusStrips_Tick;
                t_statusStrips.Start();
            }
            catch (Exception ex) { Log("[X] Unacceptable!!!\n" + ex); }
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) { SoftShutdown(); }


        public Bot bot { get; set; }


        Timer t_logUpdate = new Timer { Interval = 100 };
        void t_logUpdate_Tick(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedTab.Name != "tab_Log") return;

                var log = new List<string>(DebugLogManager.last);

                //log.Reverse();

                var txt = "";

                foreach (var line in log)
                    txt += line + Environment.NewLine;

                if (tb_log.Text != txt)
                {
                    tb_log.Text = txt;
                    Helper.ScrollToLastLine(tb_log);
                }
            }
            catch (Exception ex) { Log("[X] Unacceptable!!!\n" + ex); }
        }

        Timer t_statusStrips = new Timer { Interval = 100 };
        void t_statusStrips_Tick(object sender, EventArgs e)
        {
            try
            {
                // last chat message
                {
                    var lcm = bot.GetLastChatMessage();

                    var txt = "(last chat message is null)";
                    if (lcm != null)
                        txt = lcm.ToString();

                    if (toolStripStatusLabel1.Text != txt)
                        toolStripStatusLabel1.Text = txt;
                }

                // debug
                {
                    if (DebugLogManager.last.Count > 1)
                    {
                        var last = DebugLogManager.last.Last();
                        var txt = "(last log message is null)";

                        if (last != null)
                            txt = last;

                        if (toolStripStatusLabel2.Text != txt)
                            toolStripStatusLabel2.Text = txt;
                    }
                }

                // dt message
                {
                    var lastMessageTime = bot.GetLastChatMessageArrivalTime();
                    var delta = DateTime.Now - lastMessageTime;

                    var txt = "(time since last message)";
                    if (lastMessageTime != DateTime.MinValue)
                        txt = "Time since last message: " + Helper.GetTimeFormatted(delta);

                    if (toolStripStatusLabel3.Text != txt)
                        toolStripStatusLabel3.Text = txt;
                }

                // dt chat event
                {
                    var lastEventTime = bot.GetLastChatEventArrivalTime();
                    var delta = DateTime.Now - lastEventTime;

                    var txt = "(time since last chat event)";
                    if (lastEventTime != DateTime.MinValue)
                        txt = "Time since last chat event: " + Helper.GetTimeFormatted(delta);

                    if (toolStripStatusLabel4.Text != txt)
                        toolStripStatusLabel4.Text = txt;
                }
            }
            catch (Exception ex) { Log("[X] " + ex); }
        }


        int selectedRoomID;
        Timer t_updateRoomTranscript = new Timer { Interval = 250 };
        void t_updateRoomTranscript_Tick(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedTab.Name != "tab_Chat") return;

                if (bot == null) return;
                if (bot.chatapi == null) return;
                if (bot.chatapi.roomTranscriptDic.Count < 1) return;

                if (selectedRoomID < 1)
                    if (bot.MainRoomID > 0)
                        selectedRoomID = bot.MainRoomID;
                    else
                        if (bot.DebugRoomID > 0)
                            selectedRoomID = bot.DebugRoomID;
                        else
                        {
                            var _ = "It appears that neither main not debug room IDs are greater than zero, which is not cool.";
                            if (tb_chat.Text != _)
                                tb_chat.Text = _;
                        }

                if (!bot.chatapi.roomTranscriptDic.ContainsKey(selectedRoomID))
                {
                    var _ = "Select a room id in the listbox above.";
                    if (tb_chat.Text != _)
                        tb_chat.Text = _;
                    return;
                }

                var t = new List<ChatMessage>(bot.chatapi.roomTranscriptDic[selectedRoomID]);

                var txt = "";

                foreach (var cm in t)
                    txt += cm.ToString() + Environment.NewLine;

                if (tb_chat.Text != txt)
                {
                    tb_chat.Text = txt;
                    Helper.ScrollToLastLine(tb_chat);
                }
            }
            catch (Exception ex) { Log("[X] Unacceptable!!!\n" + ex); }
        }


        void EmptyForm()
        {
            tb_login.Clear();
            tb_pass.Clear();
            tb_trig.Clear();
            tb_mainRoomID.Clear();
            tb_debugRoomID.Clear();

            tb_chat.Clear();
            tb_chat.WordWrap = false;
            tb_input.Clear();

            tb_log.Clear();
            tb_log.WordWrap = false;
        }
        void PopulateForm()
        {
            try
            {
                tb_login.Text = bot.Login;
                tb_pass.PasswordChar = '•';
                tb_pass.Text = bot.Pass;
                tb_trig.Text = bot.TriggerSymbol;
                tb_mainRoomID.Text = bot.MainRoomID + "";
                tb_debugRoomID.Text = bot.DebugRoomID + "";
            }
            catch (Exception ex) { Log("[X] Unacceptable!!!\n" + ex); }
        }


        private void tb_pass_Enter(object sender, EventArgs e)
        {
            try
            {
                tb_pass.PasswordChar = '\0';
            }
            catch (Exception ex) { Log("[X] Unacceptable!!!\n" + ex); }
        }
        private void tb_pass_Leave(object sender, EventArgs e)
        {
            try
            {
                tb_pass.PasswordChar = '•';

                if (tb_pass.ReadOnly) return;

                var txt = tb_pass.Text.Trim();
                var old = bot.Pass;

                if (txt == old) return;

                bot.Pass = txt;

                Log("[~] Changed bot's pass from \"" + old + "\" to \"" + txt + "\".");
            }
            catch (Exception ex) { Log("[X] Unacceptable!!!\n" + ex); }
        }
        private void tb_login_Leave(object sender, EventArgs e)
        {
            try
            {
                if (tb_login.ReadOnly) return;

                var txt = tb_login.Text.Trim();
                var old = bot.Login;

                if (txt == old) return;

                bot.Login = txt;

                Log("[~] Changed bot's login from \"" + old + "\" to \"" + txt + "\".");
            }
            catch (Exception ex) { Log("[X] Unacceptable!!!\n" + ex); }
        }

        private void b_start_Click(object sender, EventArgs e) { SaveBot(); StartBot(); }
        void SaveBot()
        {
            try { bot.Save(); Log("[S] Bot saved."); }
            catch (Exception ex) { Log("[X] Unacceptable!!!\n" + ex); }
        }
        void StartBot()
        {
            try
            {
                if (bot.CredentialsNotEmpty)
                    l_botConfigNotFound.Visible = false;
                else
                {
                    l_botConfigNotFound.Text = "Incorrect bot settings. Please, fix and start again.";
                    Log("[x] Bot credentials are invalid. Cannot start bot.");
                    return;
                }
                // if bot is working, don't start it again // yes, but we don't know if it's working or what

                Log("[↑] Starting bot.");
                bot.Start();
                Log("[↑] Bot started.");

                b_start.Enabled = false;
                b_stop.Enabled = true;

                tb_login.ReadOnly = true;
                tb_pass.ReadOnly = true;
                tb_trig.ReadOnly = true;
                tb_mainRoomID.ReadOnly = true;
                tb_debugRoomID.ReadOnly = true;
            }
            catch (Exception ex) { Log("[X] Unacceptable!!!\n" + ex); StopBot(); }
        }
        private void b_stop_Click(object sender, EventArgs e) { StopBot(); }
        void StopBot()
        {
            try
            {
                // if it's stopped, don't try to stop it again

                bot.Stop();
                Log("[↓] Bot stopped.");
                bot.Save();
                Log("[S] Bot saved.");

                b_start.Enabled = true;
                b_stop.Enabled = false;

                tb_login.ReadOnly = false;
                tb_pass.ReadOnly = false;
                tb_trig.ReadOnly = false;
                tb_mainRoomID.ReadOnly = false;
                tb_debugRoomID.ReadOnly = false;
            }
            catch (Exception ex) { Log("[X] Unacceptable!!!\n" + ex); }
        }
        void SoftShutdown()
        {
            try { bot.Save(); bot.SoftShutdown("Stop button pressed on form or stopped after start because of bad credentials, or an unexpected error."); }
            catch (Exception ex) { Log("[X] Unacceptable!!!\n" + ex); }
        }


        private void b_send_Click(object sender, EventArgs e)
        {
            try
            {
                var txt = tb_input.Text.Trim();

                if (string.IsNullOrEmpty(txt)) return;

                bot.PostMessage(selectedRoomID, txt);
            }
            catch (Exception ex) { Log("[X] Unacceptable!!!\n" + ex); }
        }


        private void cb_wrapChat_CheckedChanged(object sender, EventArgs e)
        {
            tb_chat.WordWrap = cb_wrapChat.Checked;
            tb_chat.ScrollBars = cb_wrapChat.Checked ? ScrollBars.Vertical : ScrollBars.Both;
        }
        private void cb_wrapLog_CheckedChanged(object sender, EventArgs e)
        {
            tb_log.WordWrap = cb_wrapLog.Checked;
            tb_log.ScrollBars = cb_wrapLog.Checked ? ScrollBars.Vertical : ScrollBars.Both;
        }

        private void tb_trig_Leave(object sender, EventArgs e)
        {
            try
            {
                if (tb_trig.ReadOnly) return;

                var txt = tb_trig.Text.Trim();
                var old = bot.TriggerSymbol;

                if (txt == old) return;

                bot.TriggerSymbol = txt;

                Log("[~] Changed bot's trigger symbol from \"" + old + "\" to \"" + txt + "\".");
            }
            catch (Exception ex) { Log("[X] Unacceptable!!!\n" + ex); }
        }

        private void tb_mainRoomID_Leave(object sender, EventArgs e)
        {
            try
            {
                if (tb_mainRoomID.ReadOnly) return;

                var txt = tb_mainRoomID.Text.Trim();
                var old = bot.MainRoomID + "";

                if (txt == old) return;

                bot.MainRoomID = int.Parse(txt);
                tb_mainRoomID.BackColor = SystemColors.Window;

                Log("[~] Changed bot's main room ID from \"" + old + "\" to \"" + txt + "\".");
            }
            catch (Exception ex)
            {
                tb_mainRoomID.BackColor = Color.FromArgb(255, 230, 230);
                Log("[X] Unacceptable!!!\n" + ex);
            }
        }

        private void tb_debugRoomID_Leave(object sender, EventArgs e)
        {
            try
            {
                if (tb_debugRoomID.ReadOnly) return;

                var txt = tb_debugRoomID.Text.Trim();
                var old = bot.DebugRoomID + "";

                if (txt == old) return;

                bot.DebugRoomID = int.Parse(txt);
                tb_debugRoomID.BackColor = SystemColors.Window;

                Log("[~] Changed bot's debug room ID from \"" + old + "\" to \"" + txt + "\".");
            }
            catch (Exception ex)
            {
                tb_debugRoomID.BackColor = Color.FromArgb(255, 230, 230);
                Log("[X] Unacceptable!!!\n" + ex);
            }
        }







    }
}
