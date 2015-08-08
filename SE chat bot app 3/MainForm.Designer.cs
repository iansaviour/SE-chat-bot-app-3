namespace SE_chat_bot_app_3
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tab_Main = new System.Windows.Forms.TabPage();
            this.l_botConfigNotFound = new System.Windows.Forms.Label();
            this.b_stop = new System.Windows.Forms.Button();
            this.b_start = new System.Windows.Forms.Button();
            this.tb_pass = new System.Windows.Forms.TextBox();
            this.tb_debugRoomID = new System.Windows.Forms.TextBox();
            this.tb_mainRoomID = new System.Windows.Forms.TextBox();
            this.tb_trig = new System.Windows.Forms.TextBox();
            this.tb_login = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tab_Chat = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tb_chat = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cb_wrapChat = new System.Windows.Forms.CheckBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.tb_input = new System.Windows.Forms.TextBox();
            this.b_send = new System.Windows.Forms.Button();
            this.tab_Modules = new System.Windows.Forms.TabPage();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.b_update = new System.Windows.Forms.Button();
            this.tab_Log = new System.Windows.Forms.TabPage();
            this.tb_log = new System.Windows.Forms.TextBox();
            this.cb_wrapLog = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip2 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1.SuspendLayout();
            this.tab_Main.SuspendLayout();
            this.tab_Chat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tab_Modules.SuspendLayout();
            this.tab_Log.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.statusStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tab_Main);
            this.tabControl1.Controls.Add(this.tab_Chat);
            this.tabControl1.Controls.Add(this.tab_Modules);
            this.tabControl1.Controls.Add(this.tab_Log);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(694, 424);
            this.tabControl1.TabIndex = 1;
            // 
            // tab_Main
            // 
            this.tab_Main.Controls.Add(this.l_botConfigNotFound);
            this.tab_Main.Controls.Add(this.b_stop);
            this.tab_Main.Controls.Add(this.b_start);
            this.tab_Main.Controls.Add(this.tb_pass);
            this.tab_Main.Controls.Add(this.tb_debugRoomID);
            this.tab_Main.Controls.Add(this.tb_mainRoomID);
            this.tab_Main.Controls.Add(this.tb_trig);
            this.tab_Main.Controls.Add(this.tb_login);
            this.tab_Main.Controls.Add(this.label5);
            this.tab_Main.Controls.Add(this.label2);
            this.tab_Main.Controls.Add(this.label4);
            this.tab_Main.Controls.Add(this.label3);
            this.tab_Main.Controls.Add(this.label1);
            this.tab_Main.Location = new System.Drawing.Point(4, 25);
            this.tab_Main.Name = "tab_Main";
            this.tab_Main.Padding = new System.Windows.Forms.Padding(3);
            this.tab_Main.Size = new System.Drawing.Size(686, 395);
            this.tab_Main.TabIndex = 0;
            this.tab_Main.Text = "Main";
            this.tab_Main.UseVisualStyleBackColor = true;
            // 
            // l_botConfigNotFound
            // 
            this.l_botConfigNotFound.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.l_botConfigNotFound.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.l_botConfigNotFound.Location = new System.Drawing.Point(3, 3);
            this.l_botConfigNotFound.Name = "l_botConfigNotFound";
            this.l_botConfigNotFound.Size = new System.Drawing.Size(680, 71);
            this.l_botConfigNotFound.TabIndex = 4;
            this.l_botConfigNotFound.Text = "Bot configuration not found.\r\nPlease, enter bot account credentials.";
            this.l_botConfigNotFound.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.l_botConfigNotFound.Visible = false;
            // 
            // b_stop
            // 
            this.b_stop.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.b_stop.Enabled = false;
            this.b_stop.Location = new System.Drawing.Point(366, 285);
            this.b_stop.Name = "b_stop";
            this.b_stop.Size = new System.Drawing.Size(51, 26);
            this.b_stop.TabIndex = 7;
            this.b_stop.Text = "Stop";
            this.b_stop.UseVisualStyleBackColor = true;
            this.b_stop.Click += new System.EventHandler(this.b_stop_Click);
            // 
            // b_start
            // 
            this.b_start.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.b_start.Location = new System.Drawing.Point(272, 285);
            this.b_start.Name = "b_start";
            this.b_start.Size = new System.Drawing.Size(51, 26);
            this.b_start.TabIndex = 6;
            this.b_start.Text = "Start";
            this.b_start.UseVisualStyleBackColor = true;
            this.b_start.Click += new System.EventHandler(this.b_start_Click);
            // 
            // tb_pass
            // 
            this.tb_pass.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tb_pass.Location = new System.Drawing.Point(255, 147);
            this.tb_pass.Name = "tb_pass";
            this.tb_pass.Size = new System.Drawing.Size(179, 20);
            this.tb_pass.TabIndex = 2;
            this.tb_pass.Text = "tb_pass";
            this.tb_pass.Enter += new System.EventHandler(this.tb_pass_Enter);
            this.tb_pass.Leave += new System.EventHandler(this.tb_pass_Leave);
            // 
            // tb_debugRoomID
            // 
            this.tb_debugRoomID.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tb_debugRoomID.Location = new System.Drawing.Point(366, 236);
            this.tb_debugRoomID.Name = "tb_debugRoomID";
            this.tb_debugRoomID.Size = new System.Drawing.Size(51, 20);
            this.tb_debugRoomID.TabIndex = 5;
            this.tb_debugRoomID.Text = "tb_debugRoomID";
            this.tb_debugRoomID.Leave += new System.EventHandler(this.tb_debugRoomID_Leave);
            // 
            // tb_mainRoomID
            // 
            this.tb_mainRoomID.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tb_mainRoomID.Location = new System.Drawing.Point(366, 210);
            this.tb_mainRoomID.Name = "tb_mainRoomID";
            this.tb_mainRoomID.Size = new System.Drawing.Size(51, 20);
            this.tb_mainRoomID.TabIndex = 4;
            this.tb_mainRoomID.Text = "tb_mainRoomID";
            this.tb_mainRoomID.Leave += new System.EventHandler(this.tb_mainRoomID_Leave);
            // 
            // tb_trig
            // 
            this.tb_trig.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tb_trig.Location = new System.Drawing.Point(366, 183);
            this.tb_trig.Name = "tb_trig";
            this.tb_trig.Size = new System.Drawing.Size(51, 20);
            this.tb_trig.TabIndex = 3;
            this.tb_trig.Text = "tb_trig";
            this.tb_trig.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tb_trig.Leave += new System.EventHandler(this.tb_trig_Leave);
            // 
            // tb_login
            // 
            this.tb_login.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tb_login.Location = new System.Drawing.Point(255, 99);
            this.tb_login.Name = "tb_login";
            this.tb_login.Size = new System.Drawing.Size(179, 20);
            this.tb_login.TabIndex = 1;
            this.tb_login.Text = "tb_login";
            this.tb_login.Leave += new System.EventHandler(this.tb_login_Leave);
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(269, 239);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Debug room ID:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(252, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Password";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(269, 213);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Main room ID:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(269, 186);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Trigger symbol:";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(252, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Login";
            // 
            // tab_Chat
            // 
            this.tab_Chat.Controls.Add(this.splitContainer1);
            this.tab_Chat.Location = new System.Drawing.Point(4, 25);
            this.tab_Chat.Name = "tab_Chat";
            this.tab_Chat.Padding = new System.Windows.Forms.Padding(3);
            this.tab_Chat.Size = new System.Drawing.Size(686, 395);
            this.tab_Chat.TabIndex = 1;
            this.tab_Chat.Text = "Chat";
            this.tab_Chat.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tb_chat);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tb_input);
            this.splitContainer1.Panel2.Controls.Add(this.b_send);
            this.splitContainer1.Size = new System.Drawing.Size(680, 389);
            this.splitContainer1.SplitterDistance = 328;
            this.splitContainer1.TabIndex = 2;
            // 
            // tb_chat
            // 
            this.tb_chat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_chat.Location = new System.Drawing.Point(0, 31);
            this.tb_chat.Multiline = true;
            this.tb_chat.Name = "tb_chat";
            this.tb_chat.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tb_chat.Size = new System.Drawing.Size(680, 297);
            this.tb_chat.TabIndex = 0;
            this.tb_chat.Text = "tb_chat";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cb_wrapChat);
            this.panel1.Controls.Add(this.radioButton1);
            this.panel1.Controls.Add(this.radioButton2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(680, 31);
            this.panel1.TabIndex = 5;
            // 
            // cb_wrapChat
            // 
            this.cb_wrapChat.AutoSize = true;
            this.cb_wrapChat.Location = new System.Drawing.Point(167, 7);
            this.cb_wrapChat.Name = "cb_wrapChat";
            this.cb_wrapChat.Size = new System.Drawing.Size(78, 17);
            this.cb_wrapChat.TabIndex = 5;
            this.cb_wrapChat.Text = "Word wrap";
            this.cb_wrapChat.UseVisualStyleBackColor = true;
            this.cb_wrapChat.CheckedChanged += new System.EventHandler(this.cb_wrapChat_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.radioButton1.Location = new System.Drawing.Point(3, 3);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(66, 23);
            this.radioButton1.TabIndex = 4;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Main room";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton2.AutoSize = true;
            this.radioButton2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.radioButton2.Location = new System.Drawing.Point(77, 3);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(75, 23);
            this.radioButton2.TabIndex = 4;
            this.radioButton2.Text = "Debug room";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // tb_input
            // 
            this.tb_input.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_input.Location = new System.Drawing.Point(0, 0);
            this.tb_input.Multiline = true;
            this.tb_input.Name = "tb_input";
            this.tb_input.Size = new System.Drawing.Size(605, 57);
            this.tb_input.TabIndex = 1;
            this.tb_input.Text = "tb_input";
            // 
            // b_send
            // 
            this.b_send.Dock = System.Windows.Forms.DockStyle.Right;
            this.b_send.Location = new System.Drawing.Point(605, 0);
            this.b_send.Name = "b_send";
            this.b_send.Size = new System.Drawing.Size(75, 57);
            this.b_send.TabIndex = 2;
            this.b_send.Text = "Send";
            this.b_send.UseVisualStyleBackColor = true;
            this.b_send.Click += new System.EventHandler(this.b_send_Click);
            // 
            // tab_Modules
            // 
            this.tab_Modules.Controls.Add(this.listBox1);
            this.tab_Modules.Controls.Add(this.b_update);
            this.tab_Modules.Location = new System.Drawing.Point(4, 25);
            this.tab_Modules.Name = "tab_Modules";
            this.tab_Modules.Size = new System.Drawing.Size(686, 395);
            this.tab_Modules.TabIndex = 3;
            this.tab_Modules.Text = "Modules";
            this.tab_Modules.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(177, 104);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(332, 147);
            this.listBox1.TabIndex = 4;
            // 
            // b_update
            // 
            this.b_update.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.b_update.Location = new System.Drawing.Point(300, 261);
            this.b_update.Name = "b_update";
            this.b_update.Size = new System.Drawing.Size(87, 29);
            this.b_update.TabIndex = 3;
            this.b_update.Text = "Update";
            this.b_update.UseVisualStyleBackColor = true;
            // 
            // tab_Log
            // 
            this.tab_Log.Controls.Add(this.tb_log);
            this.tab_Log.Controls.Add(this.cb_wrapLog);
            this.tab_Log.Location = new System.Drawing.Point(4, 25);
            this.tab_Log.Name = "tab_Log";
            this.tab_Log.Size = new System.Drawing.Size(686, 395);
            this.tab_Log.TabIndex = 2;
            this.tab_Log.Text = "Log";
            this.tab_Log.UseVisualStyleBackColor = true;
            // 
            // tb_log
            // 
            this.tb_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_log.Location = new System.Drawing.Point(0, 17);
            this.tb_log.Multiline = true;
            this.tb_log.Name = "tb_log";
            this.tb_log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tb_log.Size = new System.Drawing.Size(686, 378);
            this.tb_log.TabIndex = 0;
            this.tb_log.Text = "tb_log";
            // 
            // cb_wrapLog
            // 
            this.cb_wrapLog.AutoSize = true;
            this.cb_wrapLog.Dock = System.Windows.Forms.DockStyle.Top;
            this.cb_wrapLog.Location = new System.Drawing.Point(0, 0);
            this.cb_wrapLog.Name = "cb_wrapLog";
            this.cb_wrapLog.Size = new System.Drawing.Size(686, 17);
            this.cb_wrapLog.TabIndex = 1;
            this.cb_wrapLog.Text = "Word wrap";
            this.cb_wrapLog.UseVisualStyleBackColor = true;
            this.cb_wrapLog.CheckedChanged += new System.EventHandler(this.cb_wrapLog_CheckedChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel3});
            this.statusStrip1.Location = new System.Drawing.Point(0, 424);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(694, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(561, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel3.Text = "toolStripStatusLabel3";
            // 
            // statusStrip2
            // 
            this.statusStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel4});
            this.statusStrip2.Location = new System.Drawing.Point(0, 446);
            this.statusStrip2.Name = "statusStrip2";
            this.statusStrip2.Size = new System.Drawing.Size(694, 22);
            this.statusStrip2.TabIndex = 4;
            this.statusStrip2.Text = "statusStrip2";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(561, 17);
            this.toolStripStatusLabel2.Spring = true;
            this.toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel4.Text = "toolStripStatusLabel4";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 468);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.statusStrip2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "SE chat bot app 3";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tab_Main.ResumeLayout(false);
            this.tab_Main.PerformLayout();
            this.tab_Chat.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tab_Modules.ResumeLayout(false);
            this.tab_Log.ResumeLayout(false);
            this.tab_Log.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.statusStrip2.ResumeLayout(false);
            this.statusStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tab_Main;
        private System.Windows.Forms.Button b_stop;
        private System.Windows.Forms.Button b_start;
        private System.Windows.Forms.TextBox tb_pass;
        private System.Windows.Forms.TextBox tb_trig;
        private System.Windows.Forms.TextBox tb_login;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tab_Chat;
        private System.Windows.Forms.TabPage tab_Log;
        private System.Windows.Forms.TabPage tab_Modules;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button b_update;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox tb_chat;
        private System.Windows.Forms.TextBox tb_input;
        private System.Windows.Forms.Button b_send;
        private System.Windows.Forms.TextBox tb_log;
        private System.Windows.Forms.CheckBox cb_wrapLog;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.TextBox tb_debugRoomID;
        private System.Windows.Forms.TextBox tb_mainRoomID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox cb_wrapChat;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.StatusStrip statusStrip2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.Label l_botConfigNotFound;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
    }
}

