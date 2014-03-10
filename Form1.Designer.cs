namespace uTuner
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiChannels = new System.Windows.Forms.ToolStripMenuItem();
            this.miFullScreen = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.status1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.filesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.miAction = new System.Windows.Forms.ToolStripMenuItem();
            this.miMute = new System.Windows.Forms.ToolStripMenuItem();
            this.miROT = new System.Windows.Forms.ToolStripMenuItem();
            this.miVideoStandarts = new System.Windows.Forms.ToolStripMenuItem();
            this.miScan = new System.Windows.Forms.ToolStripMenuItem();
            this.miRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.miRemoteControl = new System.Windows.Forms.ToolStripMenuItem();
            this.miAspectRatio = new System.Windows.Forms.ToolStripMenuItem();
            this.mi4x3 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi16x9 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi166 = new System.Windows.Forms.ToolStripMenuItem();
            this.miZoom = new System.Windows.Forms.ToolStripMenuItem();
            this.miZoomIn = new System.Windows.Forms.ToolStripMenuItem();
            this.miZoomOut = new System.Windows.Forms.ToolStripMenuItem();
            this.miReset = new System.Windows.Forms.ToolStripMenuItem();
            this.miDevices = new System.Windows.Forms.ToolStripMenuItem();
            this.miChannels = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.miFilters = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miShowHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.miAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.cmMain.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.ContextMenuStrip = this.cmMain;
            this.panel1.Cursor = System.Windows.Forms.Cursors.Default;
            this.panel1.Location = new System.Drawing.Point(0, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(626, 332);
            this.panel1.TabIndex = 0;
            // 
            // cmMain
            // 
            this.cmMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiChannels,
            this.miFullScreen});
            this.cmMain.Name = "cmMain";
            this.cmMain.Size = new System.Drawing.Size(128, 48);
            this.cmMain.Text = "1111111";
            // 
            // cmiChannels
            // 
            this.cmiChannels.Name = "cmiChannels";
            this.cmiChannels.Size = new System.Drawing.Size(127, 22);
            this.cmiChannels.Text = "Channels";
            // 
            // miFullScreen
            // 
            this.miFullScreen.CheckOnClick = true;
            this.miFullScreen.Name = "miFullScreen";
            this.miFullScreen.Size = new System.Drawing.Size(127, 22);
            this.miFullScreen.Text = "Fullscreen";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 362);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(626, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // status1
            // 
            this.status1.Name = "status1";
            this.status1.Size = new System.Drawing.Size(0, 17);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filesToolStripMenuItem,
            this.miAction,
            this.miDevices,
            this.miChannels,
            this.miFilters,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(626, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // filesToolStripMenuItem
            // 
            this.filesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miExit});
            this.filesToolStripMenuItem.Name = "filesToolStripMenuItem";
            this.filesToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.filesToolStripMenuItem.Text = "Files";
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            this.miExit.Size = new System.Drawing.Size(92, 22);
            this.miExit.Text = "Exit";
            // 
            // miAction
            // 
            this.miAction.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miMute,
            this.miROT,
            this.miVideoStandarts,
            this.miScan,
            this.miRefresh,
            this.miRemoteControl,
            this.miAspectRatio,
            this.miZoom});
            this.miAction.Name = "miAction";
            this.miAction.Size = new System.Drawing.Size(59, 20);
            this.miAction.Text = "Actions";
            // 
            // miMute
            // 
            this.miMute.Name = "miMute";
            this.miMute.Size = new System.Drawing.Size(158, 22);
            this.miMute.Text = "Mute";
            // 
            // miROT
            // 
            this.miROT.Name = "miROT";
            this.miROT.Size = new System.Drawing.Size(158, 22);
            this.miROT.Text = "ROT";
            // 
            // miVideoStandarts
            // 
            this.miVideoStandarts.Name = "miVideoStandarts";
            this.miVideoStandarts.Size = new System.Drawing.Size(158, 22);
            this.miVideoStandarts.Text = "Video Standarts";
            // 
            // miScan
            // 
            this.miScan.Enabled = false;
            this.miScan.Name = "miScan";
            this.miScan.Size = new System.Drawing.Size(158, 22);
            this.miScan.Text = "Scan";
            // 
            // miRefresh
            // 
            this.miRefresh.Name = "miRefresh";
            this.miRefresh.Size = new System.Drawing.Size(158, 22);
            this.miRefresh.Text = "Refresh";
            // 
            // miRemoteControl
            // 
            this.miRemoteControl.Name = "miRemoteControl";
            this.miRemoteControl.Size = new System.Drawing.Size(158, 22);
            this.miRemoteControl.Text = "Remote Control";
            // 
            // miAspectRatio
            // 
            this.miAspectRatio.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi4x3,
            this.mi16x9,
            this.mi166});
            this.miAspectRatio.Name = "miAspectRatio";
            this.miAspectRatio.Size = new System.Drawing.Size(158, 22);
            this.miAspectRatio.Text = "Aspect Ratio";
            // 
            // mi4x3
            // 
            this.mi4x3.Name = "mi4x3";
            this.mi4x3.Size = new System.Drawing.Size(147, 22);
            this.mi4x3.Text = "4x3";
            // 
            // mi16x9
            // 
            this.mi16x9.Name = "mi16x9";
            this.mi16x9.Size = new System.Drawing.Size(147, 22);
            this.mi16x9.Text = "16x9";
            // 
            // mi166
            // 
            this.mi166.Name = "mi166";
            this.mi166.Size = new System.Drawing.Size(147, 22);
            this.mi166.Text = "1.66 Letterbox";
            // 
            // miZoom
            // 
            this.miZoom.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miZoomIn,
            this.miZoomOut,
            this.miReset});
            this.miZoom.Name = "miZoom";
            this.miZoom.Size = new System.Drawing.Size(158, 22);
            this.miZoom.Text = "Zoom";
            // 
            // miZoomIn
            // 
            this.miZoomIn.Name = "miZoomIn";
            this.miZoomIn.Size = new System.Drawing.Size(102, 22);
            this.miZoomIn.Text = "In";
            // 
            // miZoomOut
            // 
            this.miZoomOut.Name = "miZoomOut";
            this.miZoomOut.Size = new System.Drawing.Size(102, 22);
            this.miZoomOut.Text = "Out";
            // 
            // miReset
            // 
            this.miReset.Name = "miReset";
            this.miReset.Size = new System.Drawing.Size(102, 22);
            this.miReset.Text = "Reset";
            // 
            // miDevices
            // 
            this.miDevices.Name = "miDevices";
            this.miDevices.Size = new System.Drawing.Size(59, 20);
            this.miDevices.Text = "Devices";
            // 
            // miChannels
            // 
            this.miChannels.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1});
            this.miChannels.Name = "miChannels";
            this.miChannels.Size = new System.Drawing.Size(68, 20);
            this.miChannels.Text = "Channels";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(57, 6);
            // 
            // miFilters
            // 
            this.miFilters.Name = "miFilters";
            this.miFilters.Size = new System.Drawing.Size(50, 20);
            this.miFilters.Text = "Filters";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miShowHelp,
            this.miAbout});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.aboutToolStripMenuItem.Text = "Help";
            // 
            // miShowHelp
            // 
            this.miShowHelp.Name = "miShowHelp";
            this.miShowHelp.Size = new System.Drawing.Size(131, 22);
            this.miShowHelp.Text = "Show Help";
            // 
            // miAbout
            // 
            this.miAbout.Name = "miAbout";
            this.miAbout.Size = new System.Drawing.Size(131, 22);
            this.miAbout.Text = "About";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 384);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "Form1";
            this.Text = "uTuner";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.cmMain.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        public System.Windows.Forms.ToolStripStatusLabel status1;
        public System.Windows.Forms.ContextMenuStrip cmMain;
        public System.Windows.Forms.ToolStripMenuItem miFullScreen;
        public System.Windows.Forms.ToolStripMenuItem cmiChannels;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem filesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miAction;
        public System.Windows.Forms.ToolStripMenuItem miMute;
        public System.Windows.Forms.ToolStripMenuItem miROT;
        public System.Windows.Forms.ToolStripMenuItem miVideoStandarts;
        public System.Windows.Forms.ToolStripMenuItem miScan;
        public System.Windows.Forms.ToolStripMenuItem miDevices;
        public System.Windows.Forms.ToolStripMenuItem miChannels;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        public System.Windows.Forms.ToolStripMenuItem miFilters;
        public System.Windows.Forms.ToolStripMenuItem miRefresh;
        public System.Windows.Forms.ToolStripMenuItem miExit;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem miShowHelp;
        public System.Windows.Forms.ToolStripMenuItem miAbout;
        public System.Windows.Forms.ToolStripMenuItem miRemoteControl;
        public System.Windows.Forms.ToolStripMenuItem miAspectRatio;
        public System.Windows.Forms.ToolStripMenuItem miZoom;
        public System.Windows.Forms.ToolStripMenuItem miZoomIn;
        public System.Windows.Forms.ToolStripMenuItem miZoomOut;
        public System.Windows.Forms.ToolStripMenuItem miReset;
        public System.Windows.Forms.ToolStripMenuItem mi4x3;
        public System.Windows.Forms.ToolStripMenuItem mi16x9;
        public System.Windows.Forms.ToolStripMenuItem mi166;
    }
}

