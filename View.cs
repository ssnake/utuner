using System;

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using DirectShowLib;
using System.Runtime.InteropServices;

namespace uTuner

{


    public class MyView
    {


        public MyView(MyController controller) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            this.controller = controller;

            Init();
           
        }

        void channelNumTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            channelNumTimer.Enabled = false;
            if (channelNumber != -1)
            {
                
                var chan = controller.Data.ChannelList.GetChannelByPos(channelNumber-1);
                if (chan != null)
                {
                    Form.Invoke((MethodInvoker)delegate
                    {
                        var mi = FindChannelMI(chan);
                        if (mi != null)
                            mi.PerformClick();
                    });
                }
                channelNumber = -1;
            }
        }
        private System.Timers.Timer channelNumTimer;
        private int channelNumber = -1;
        private MyController controller;
        private Channel currentChannel;
        private ToolStripMenuItem currentChannelMI;
        private bool doInitTuner = true;
        private System.Timers.Timer screenTimer;
        private System.Timers.Timer cursorTimer;
        private bool fullScreen = false;
        private bool cursorHidding = false;
        private int prevChannelID;

        private void CancelFullScreen() {
            Form.FormBorderStyle = FormBorderStyle.Sizable;
            Form.WindowState = FormWindowState.Normal;
            //Form.mainMenu.Visible = true;
            Form.status1.Visible = true;
            Form.panel1.Location = new Point(0, 27);
            Form.panel1.Size = new Size(Form.Width, Form.Height-90);
            fullScreen = false;
        }

        private void SetFullScreen() {

                //Form.mainMenu.Visible = false;
                Form.status1.Visible = false;
                Form.panel1.Refresh();
                Form.FormBorderStyle = FormBorderStyle.None;
                Form.WindowState = FormWindowState.Maximized;
                Form.panel1.Location = new Point(0, 0);
                Form.panel1.Size = new Size(Form.Width, Form.Height);
                fullScreen = true;
 
        }

        private void LoadSettings() {
            var set = controller.Data.Settings;
            controller.Data.Tuner.Volume = 0;
            
            Form.miROT.Checked = false;
            if (set.ROT)
                Form.miROT.PerformClick();
            //deinterlace filter
            if (set.DeinterlaceFilter == "")
                set.DeinterlaceFilter = Settings.None;
            foreach (var i in Form.miFilters.DropDownItems)
                if ((i is ToolStripMenuItem) && (i as ToolStripMenuItem).Text.CompareTo(set.DeinterlaceFilter) == 0)
                    (i as ToolStripMenuItem).PerformClick();
            //device
            foreach (var i in Form.miDevices.DropDownItems)
                if ((i is ToolStripMenuItem) && (i as ToolStripMenuItem).Text.CompareTo(set.Device) == 0)
                    (i as ToolStripMenuItem).PerformClick();
            
           // channels
            prevChannelID = set.LastChannelID;
            foreach(var i in Form.miChannels.DropDownItems)
                if (i is ToolStripMenuItem)
                {
                    var mi = i as ToolStripMenuItem;
                     int chanID = GetID("channel_", mi.Name);
                     if (chanID == set.LastChannelID)
                        mi.PerformClick();
                     
                }
            //Mute
            Form.miMute.Checked = false;
            if (set.Mute)
                Form.miMute.PerformClick();
            
            controller.Data.Tuner.Volume = set.Volume;
            //remote controls
            foreach (var i in Form.miRemoteControl.DropDownItems)
                if ((i is ToolStripMenuItem) && (i as ToolStripMenuItem).Text.CompareTo(set.RemoteControl) == 0)
                    (i as ToolStripMenuItem).PerformClick(); 
            
        }

        private void UpdateDevices() {
            controller.Data.Tuner.RefreshDevices();
           

            Form.miDevices.DropDownItems.Clear();
                foreach (String dev in controller.Data.Tuner.VideoDeviceList)
                {
                    ToolStripMenuItem item = (ToolStripMenuItem)Form.miDevices.DropDownItems.Add(dev);
                   
                   
                    item.Name = "video_" + dev;
                    item.Click += new EventHandler(OnVideoItemClick);
                   


                }

                var miModes =(ToolStripMenuItem) Form.miDevices.DropDownItems.Add("TV Tuner Modes");
                foreach (var mode in Enum.GetValues(typeof(AMTunerModeType)))
                {
                    var item = (ToolStripMenuItem)miModes.DropDownItems.Add(mode.ToString());
                    item.Click += new EventHandler(OnTVModeClick);
                    item.CheckOnClick = true;
                }
          
        }

        void OnTVModeClick(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            var m = (AMTunerModeType)Enum.Parse(typeof(AMTunerModeType), item.Text);
            controller.Data.Tuner.Tuner.put_Mode(m);
        }

        public void Start() {
            LoadSettings();
            Win32.SetThreadExecutionState(Win32.ES_CONTINUOUS | Win32.ES_DISPLAY_REQUIRED);
            cursorTimer.Start();


        }

        public void Stop() {
            Win32.SetThreadExecutionState(Win32.ES_CONTINUOUS);

        }

        public void Update() {
            UpdateDevices();
            UpdateChannels();
            UpdateVideoStandart();
        }

        private void BindUI() {
            Form.KeyDown += new KeyEventHandler(Form_KeyDown);

            Form.FormClosing += new FormClosingEventHandler(Form_FormClosing);
            Form.miMute.Click += new EventHandler(miMute_CheckedChanged);
            Form.miROT.Click += new EventHandler(miROT_CheckedChanged);
            Form.miFullScreen.CheckedChanged += new EventHandler(miFullScreen_CheckedChanged);
            Form.panel1.MouseWheel += new MouseEventHandler(OnMouseWheel);
            Form.MouseWheel += new MouseEventHandler(OnMouseWheel);

          //  Form.MouseMove += new MouseEventHandler(Form_MouseMove);
            Form.panel1.MouseMove += new MouseEventHandler(Form_MouseMove);

            channelNumTimer.Elapsed += new ElapsedEventHandler(channelNumTimer_Elapsed);
            screenTimer.Elapsed += new ElapsedEventHandler(screenTimer_Elapsed);
            cursorTimer.Elapsed += new ElapsedEventHandler(cursorTimer_Elapsed);
            
            Form.miScan.Click += new EventHandler(OnScanClick);
            Form.miRefresh.Click += new EventHandler(miRefresh_Click);
            Form.miExit.Click += new EventHandler(miExit_Click);
            Form.miAbout.Click += new EventHandler(miAbout_Click);
            Form.miShowHelp.Click += new EventHandler(miShowHelp_Click);
        }

        void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form.Invoke((MethodInvoker)delegate
            {
                cursorTimer.Stop();
                screenTimer.Stop();
            });
            Form = null;            
        }

        void Form_MouseMove(object sender, MouseEventArgs e)
        {
         
        }

        void cursorTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Form == null)
                return;
            Form.Invoke((MethodInvoker)delegate           {
               var actThreshold = TimeSpan.FromSeconds(3);
               cursorTimer.Enabled = false;
               if (fullScreen) 
               {
                   if (Win32.GetLastInput() > actThreshold)
                   {
                       if (!cursorHidding)
                       {
                           // Cursor.Hide();
                           Win32.ShowCursor(false);
                           cursorHidding = true;
                       }
                   }
                   else
                       if (cursorHidding)
                       {
                           Win32.ShowCursor(true);
                           cursorHidding = false;
                       }
               }
               cursorTimer.Enabled = true;
           });
        }

        private ToolStripMenuItem FindChannelMI(Channel channel) {
            var menu = Form.miChannels;
            foreach (var i in Form.miChannels.DropDownItems)
            {
                if ((i is ToolStripMenuItem) && (i as ToolStripMenuItem).Name.Contains("channel_"))
                {
                    var mii = (i as ToolStripMenuItem);
                    int chanID = GetID("channel_", mii.Name);
                    if (channel.ID == chanID)
                    {
                        return mii;
                    }

                }

            }
            return null;
        }

        void miShowHelp_Click(object sender, EventArgs e)
        {

            var s = "\t\tHelp \n\n\tM - Mute\n\tEnter - Full Screen\n\tH - Help\n\tMouse Wheel - Volume\n\tArrows - channels switching\n\t PgUp/PgDown - Zoom In/Out";
            ShowMsg(s);

        }

        void miAbout_Click(object sender, EventArgs e)
        {
            var aboutDlg = new MyAboutBox();
            aboutDlg.ShowDialog();
        }

        void miExit_Click(object sender, EventArgs e)
        {
            Form.Close();
        }

        private int GetID(string prefix, string value) {
            if (prefix.Length < value.Length)
            {
                var idStr = value.Substring(prefix.Length);
                return  Convert.ToInt32(idStr);
            } else
                return -1;
        }

        private void button1_Click(object sender, EventArgs e)
        {

          //  controller.Data.Tuner.SetFullScreen();
        }

        private void Init() {
           
            
            Form = new Form1();
            screenTimer = new System.Timers.Timer(3000);
            screenTimer.AutoReset = false;
            cursorTimer = new System.Timers.Timer(100);
            cursorTimer.AutoReset = false;
            
            channelNumTimer = new System.Timers.Timer(3000);
            channelNumTimer.AutoReset = false;
           
            controller.Data.Tuner.Render = Form.panel1;
            UpdateDeinterlace();
            UpdateRemoteCtrls();
            UpdateAspectRatio();
            UpdateZoom();
            BindUI();
          
        }

        void screenTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Form == null)
                return;
            Form.Invoke((MethodInvoker)delegate
            {
                screenTimer.Enabled = false;

                var emptyMsg = "";

                if (controller.Data.Tuner.Mute)
                    emptyMsg = "Mute";
                controller.Data.OverlayGraphics.DrawText(emptyMsg, Color.Red);
            });
        }

        void Form_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.KeyCode == Keys.M) 
                Form.miMute.PerformClick();

            if (e.KeyCode == Keys.Enter)
                Form.miFullScreen.PerformClick();
            if (e.KeyCode == Keys.F1 || e.KeyCode == Keys.H)
                miShowHelp_Click(null, null);

            if (e.KeyCode == Keys.Add)
            {
                controller.Data.Tuner.Volume += 5;
                controller.Data.Settings.Volume = controller.Data.Tuner.Volume;
                ShowVolume();     
            }
            if (e.KeyCode == Keys.Subtract)
            {
                controller.Data.Tuner.Volume -= 5;
                controller.Data.Settings.Volume = controller.Data.Tuner.Volume;
                ShowVolume();
            }
            if (currentChannelMI != null)
            {
                if (e.KeyCode == Keys.R || e.KeyCode == Keys.Back)
                {
                    var chan = controller.Data.ChannelList.GetChannelByID(prevChannelID);
                    if (chan != null)
                    {
                        var mi = FindChannelMI(chan);
                        if (mi != null)
                            mi.PerformClick();
                    }
                }
            }
            if (currentChannelMI != null)
            {
                var channels = Form.miChannels.DropDownItems;
                if (e.KeyCode == Keys.Up)
                {
                    for (int i = 0; i < channels.Count; i++)
                    {
                        if (currentChannelMI == channels[i] && i != channels.Count - 1)
                        {
                            channels[i + 1].PerformClick();
                            return;
                        }
                        if (currentChannelMI == channels[i] && i == channels.Count - 1)
                        {

                            channels[0].PerformClick();
                            return;
                        }
                    }


                }
                if (e.KeyCode == Keys.Down)
                {

                    for (int i = 0; i < channels.Count; i++)
                    {
                        if (currentChannelMI == channels[i] && i != 0)
                        {
                            channels[i - 1].PerformClick();
                            return;
                        }
                        if (currentChannelMI == channels[i] && i == 0)
                        {
                            channels[channels.Count - 1].PerformClick();
                            return; 
                        }
                    }
                }
                if (e.KeyCode == Keys.PageUp)
                    Form.miZoomIn.PerformClick();

                if (e.KeyCode == Keys.PageDown)
                    Form.miZoomOut.PerformClick();
   
               
            }
            switch (e.KeyCode)
            {
                case Keys.D1:
                case Keys.NumPad1:
                    OnChannelNumberPressed(1);
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    OnChannelNumberPressed(2);
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                    OnChannelNumberPressed(3);
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                    OnChannelNumberPressed(4);
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                    OnChannelNumberPressed(5);
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                    OnChannelNumberPressed(6);
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                    OnChannelNumberPressed(7);
                    break;
                case Keys.D8:
                case Keys.NumPad8:
                    OnChannelNumberPressed(8);
                    break;
                case Keys.D9:
                case Keys.NumPad9:
                    OnChannelNumberPressed(9);
                    break;
                case Keys.D0:
                case Keys.NumPad0:
                    OnChannelNumberPressed(0);
                    break;
            }

        }

        void miRefresh_Click(object sender, EventArgs e)
        {
            controller.Data.Tuner.Refresh();
            //controller.Data.Tuner.Render = null;
            //controller.Data.Tuner.Render = Form.panel1;
        }

        void OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (Form.miMute.Checked)
                Form.miMute.PerformClick();
            int delta = e.Delta/ 100;
            controller.Data.Tuner.Volume +=  delta;
            controller.Data.Settings.Volume = controller.Data.Tuner.Volume;
            ShowVolume();

        }

        void miFullScreen_CheckedChanged(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            if (item.Checked)
                SetFullScreen();
            else
                CancelFullScreen();

        }

        void miROT_CheckedChanged(object sender, EventArgs e)
        {
            var item = (sender as ToolStripMenuItem);
            item.Checked = !item.Checked;
            controller.Data.Tuner.ROT = item.Checked;
            Settings.ROT = controller.Data.Tuner.ROT;
            
        }

        void miMute_CheckedChanged(object sender, EventArgs e)
        {
            var item = (sender as ToolStripMenuItem);
            item.Checked = !item.Checked;
           
            controller.Data.Tuner.Mute = item.Checked;
            
            Settings.Mute = item.Checked;
            if (item.Checked)
                ShowMsg("Mute");
            else
                ShowVolume();

        }

        private void OnVideoItemClick(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            
            foreach (ToolStripMenuItem i in Form.miDevices.DropDownItems)
            {
                if (i != item && i.Name.Contains("video_"))
                {
                    i.Checked = false;
                    i.Enabled = true;
                }
            }
            item.Checked = true;
            controller.Data.Tuner.Stop();
            controller.Data.Tuner.SetVideoDeviceByName(item.Text);
            if (doInitTuner)
            {
          
                var tuner = controller.Data.Tuner.Tuner;
                if (tuner != null)
                {
                    //init if tuner was turned off
                    int chMin, chMax;
                    tuner.ChannelMinMax(out chMin, out chMax);
                    tuner.put_Channel(chMin, AMTunerSubChannel.Default, AMTunerSubChannel.Default);
                    doInitTuner = false;
                }
            }
            controller.Data.Tuner.Start();
            Settings.Device = item.Text;
            item.Enabled = false;
            Form.miScan.Enabled = (controller.Data.Tuner.Tuner != null);
           
        }

        private void OnAudioItemClick(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            foreach (ToolStripMenuItem i in Form.miDevices.DropDownItems)
            {
                if (i != item && i.Name.Contains("audio_"))
                    i.Checked = false;
            }
            controller.Data.Tuner.Stop();
            controller.Data.Tuner.SetAudioDeviceByName(item.Text);
            

           
        }

        private void button2_Click(object sender, EventArgs e)
        {
           /* var ch = 0;
            DirectShowLib.AMTunerSubChannel v;
            DirectShowLib.AMTunerSubChannel s;
            DirectShowLib.AMTunerSignalStrength ss;
           
            controller.Data.Tuner.SignalPresent(out ss);
            controller.Data.Tuner.get_VideoFrequency(out ch);
            controller.Data.Tuner.get_Channel(out ch, out v, out s);*/
            //controller.Data.Tunner.DisplayPropertyPage(controller.Data.Tunner.Crossbar as IBaseFilter);
            /*controller.Data.Tunner.GetStatus();
            controller.Data.Tunner.GetModeCaps();
            
            int freq;
            controller.Data.Tunner.Tuner.get_VideoFrequency(out freq);
            AMTunerSignalStrength s;
            controller.Data.Tunner.Tuner.SignalPresent(out s);*/

            controller.Data.OverlayGraphics.DrawText("sfkjsksss");
        }
        private void OnChannelClick(object sender, EventArgs e)
        {
            if (controller.Data.Tuner.Tuner == null)
                return;

            var mi = (ToolStripMenuItem)sender;

            foreach (var i in Form.miChannels.DropDownItems)
            {
                if (i != mi && (i is ToolStripMenuItem) && (i as ToolStripMenuItem).Name.Contains("channel_"))
                {
                    var mii = (i as ToolStripMenuItem);

                    mii.Checked = false;
                    mii.Font = new Font(mii.Font, FontStyle.Regular);
                    //mii.Font.Style -= (FontStyle.Bold); 
                }
            }
            mi.Checked = true;
            mi.Font = new Font(mi.Font, FontStyle.Bold);
            var name = mi.Name;
            int chanID = GetID("channel_", name);
            var c = controller.Data.ChannelList.GetChannelByID(chanID);
            if (c != null)
            {
               
                
                currentChannelMI = mi;
                prevChannelID = controller.Data.Settings.LastChannelID;
                controller.Data.Settings.LastChannelID = c.ID;
                controller.Data.Tuner.SetChannel(c);
                catchUpChanSettings(c);
                currentChannel = c;
                ShowMsg("Channel " + (c.Position + 1) + ": " + c.Name);
                
            }
            else
                throw new Exception("Unable to find channel");
        }        

        private void OnChannelNumberPressed(int number) {
            var prevNum = channelNumber;
            if (channelNumber == -1)
                channelNumber = 0;
            channelNumber = channelNumber*10  + number;
            number = channelNumber;
           // if (channelNumber < 10 && prevNum != 0)
          //      number = channelNumber * 10;
            
            var s = "" + String.Format("{0:D2}", number);


            ShowMsg(s);
            if (channelNumber > 9 || prevNum == 0)
                channelNumTimer.Interval = 500;
            else
                channelNumTimer.Interval = 3000;

            channelNumTimer.Start();
                

            

        }

        private void OnFilterClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem)sender;

            foreach (var i in Form.miFilters.DropDownItems)
            {
                if (i != mi && (i is ToolStripMenuItem) && (i as ToolStripMenuItem).Name.Contains("deinterlace_"))
                {
                    (i as ToolStripMenuItem).Checked = false;
                    (i as ToolStripMenuItem).Enabled = true;
                }
            }
            mi.Checked = true;
            var name = mi.Name;            
            int index = GetID("deinterlace_", name);
            if (index >= 0 && index < controller.Data.Tuner.DeinterlaceLayoutList.Count)
            {
                var c = controller.Data.Tuner.DeinterlaceLayoutList[index];
                controller.Data.Tuner.DeInterlaceLayout = c;
                Settings.DeinterlaceFilter = c.Name;
                



            }
            else
            {
                controller.Data.Tuner.DeInterlaceLayout = null;
                Settings.DeinterlaceFilter = Settings.None;
            }
            mi.Enabled = false;
        }        

        private void OnRemoteCtrlClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem)sender;

            foreach (var i in Form.miRemoteControl.DropDownItems)
            {
                if (i != mi && (i is ToolStripMenuItem) && (i as ToolStripMenuItem).Name.Contains("rc_"))
                {
                    (i as ToolStripMenuItem).Checked = false;
                    (i as ToolStripMenuItem).Enabled = true;
                }
            }
            mi.Checked = true;
            var name = mi.Name;            
            int index = GetID("rc_", name);
            if (index >= 0)
            {
                var rc = controller.Data.RemoteControls[index];
                controller.Data.RemoteControl = rc;
                Settings.RemoteControl = rc.Name;
                rc.Data += new RemoteControl.DataEvent(rc_Data);
                



            }
            else
            {
                controller.Data.RemoteControl = null;
                Settings.RemoteControl = Settings.None;
            }
            mi.Enabled = false;
        }

        void rc_Data(RemoteControl.Key key)
        {
            Keys k = Keys.None;
            Form.Invoke((MethodInvoker)delegate
            {
                switch (key)
                {
                    case RemoteControl.Key.rckChannelDown:
                        k = Keys.Down;
                        break;
                    case RemoteControl.Key.rckChannelUp:
                        k = Keys.Up;
                        break;
                    case RemoteControl.Key.rckVolUp:
                        k = Keys.Add;
                        break;
                    case RemoteControl.Key.rckVolDown:
                        k = Keys.Subtract;
                        break;
                    case RemoteControl.Key.rckMute:
                        k = Keys.M;
                        break;
                    case RemoteControl.Key.rckShutDown:
                        Form.Close();
                        return;
                    case RemoteControl.Key.rckFullScreen:
                        k = Keys.Enter;
                        break;
                    case RemoteControl.Key.rck0:
                        k = Keys.D0;
                        break;
                    case RemoteControl.Key.rck1:
                        k = Keys.D1;
                        break;
                    case RemoteControl.Key.rck2:
                        k = Keys.D2;
                        break;
                    case RemoteControl.Key.rck3:
                        k = Keys.D3;
                        break;
                    case RemoteControl.Key.rck4:
                        k = Keys.D4;
                        break;
                    case RemoteControl.Key.rck5:
                        k = Keys.D5;
                        break;
                    case RemoteControl.Key.rck6:
                        k = Keys.D6;
                        break;
                    case RemoteControl.Key.rck7:
                        k = Keys.D7;
                        break;
                    case RemoteControl.Key.rck8:
                        k = Keys.D8;
                        break;
                    case RemoteControl.Key.rck9:
                        k = Keys.D9;                       
                        break;
                    case RemoteControl.Key.rckReturn:
                        k = Keys.R;
                        break;
                }


                Form_KeyDown(this, new KeyEventArgs(k));
            });
        }        

        private void OnScanClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem)sender;
            if (controller.ScanView.Show() == DialogResult.OK)
            {
                UpdateChannels();
                UpdateCurrentChanMI();
            }



        }
        private void OnFilterPropertyClick(object sender, EventArgs e)
        {
            var tuner = controller.Data.Tuner;
            if (tuner.DeInterlaceLayout != null)
                tuner.DisplayPropertyPage(tuner.DeInterlaceLayout.Filter);


        }

        private void ShowMsg(string msg) {
            controller.Data.OverlayGraphics.DrawText(msg);
            screenTimer.Stop();
            screenTimer.Start();
            
        }

        private void ShowVolume() {
            var vol = controller.Data.Tuner.Volume;
            ShowMsg("Volume: " + vol);

        }

        private void UpdateChannels() {
            var channels = Form.miChannels;
            var cchannels = Form.cmiChannels;
            ToolStripDropDown menu = new ToolStripDropDown();


            channels.DropDownItems.Clear();
            
            cchannels.DropDownItems.Clear();
            

            
            foreach (Channel c in controller.Data.ChannelList)
            {
                var i = new ToolStripMenuItem((c.Position + 1)+". " + c.Name);
                menu.Items.Add(i);
                
                i.Name = "channel_" + c.ID;
                i.Click += new EventHandler(OnChannelClick);
                i.CheckOnClick = true;
                
               

            }
 
            menu.LayoutStyle = ToolStripLayoutStyle.Table;

           
            ((TableLayoutSettings)menu.LayoutSettings).ColumnCount = 1 + menu.Items.Count / 30;
           
            channels.DropDown = menu;
            cchannels.DropDown = menu;


        }

        private void UpdateCurrentChanMI() {
            var channels = Form.miChannels.DropDownItems;
            foreach (var mi in channels)
            {
                if (mi is ToolStripMenuItem)
                {
                    var id = GetID("channel_", (mi as ToolStripMenuItem).Name);
                    if (id == controller.Data.Settings.LastChannelID)
                        currentChannelMI = (mi as ToolStripMenuItem);
                }
            }

        }

        private void UpdateDeinterlace() {
            var menu = Form.miFilters;
            menu.DropDownItems.Clear();
            menu.CheckOnClick = true;
            var deinterList = controller.Data.Tuner.DeinterlaceLayoutList;
            int ind = 0;
            foreach (DeInterlaceLayout d in deinterList)
            {
                if (d.IsSupported())
                {
                    var i = (ToolStripMenuItem)menu.DropDownItems.Add(d.Name);
                    i.Name = "deinterlace_" + ind;
                    i.Click += new EventHandler(OnFilterClick);
                }
                ind++;
            }
            var mi = (ToolStripMenuItem)menu.DropDownItems.Add(Settings.None);
            mi.Name = "deinterlace_" + ind++; 
            mi.Click += new EventHandler(OnFilterClick);

            menu.DropDownItems.Add("-");
            var prop = (ToolStripMenuItem) menu.DropDownItems.Add("Property");
            prop.Click +=new EventHandler(OnFilterPropertyClick);

        }

        private void UpdateRemoteCtrls() {
            var menu = Form.miRemoteControl;
            menu.DropDownItems.Clear();
            var rcList = controller.Data.RemoteControls;
            int ind = 0;
            foreach (RemoteControl rc in rcList)
            {
                if (!rc.IsSupport())
                    continue;
                try
                {
                    
                    var i = (ToolStripMenuItem)menu.DropDownItems.Add(rc.Name);
                    i.Name = "rc_" + ind++;
                    i.Click += new EventHandler(OnRemoteCtrlClick);
                }
                catch
                {
                }
            }
            menu.Enabled = rcList.Count > 0;

        }

        private void UpdateVideoStandart() {
            var menu = Form.miVideoStandarts;
            menu.DropDownItems.Clear();

            foreach (AnalogVideoStandard vs in Enum.GetValues(typeof(AnalogVideoStandard)))
            {
                var item = (ToolStripMenuItem)menu.DropDownItems.Add(vs.ToString());
                item.Click += new EventHandler(OnVideoStandartsClick);
            }
        }

        private void UpdateAspectRatio() {
            var menu = Form.miAspectRatio;
            

            foreach (var i in Form.miAspectRatio.DropDownItems)
                if (i is ToolStripMenuItem)
                    (i as ToolStripMenuItem).Click += new EventHandler(OnAspectRatioClick);
        }

        private void UpdateZoom() {
            var menu = Form.miZoom;
            

            foreach (var i in menu.DropDownItems)
                if (i is ToolStripMenuItem)
                    (i as ToolStripMenuItem).Click += new EventHandler(OnZoomClick);
        }

        private void catchUpChanSettings(Channel channel) {
           //aspect ratio
           if (channel.AspectRatio == 4 / 3.0f || channel.AspectRatio == 0.0f)
               Form.mi4x3.PerformClick();
           if (channel.AspectRatio == 16 / 9.0f)
               Form.mi16x9.PerformClick();
           if (channel.AspectRatio == 1.66f)
               Form.mi166.PerformClick();
           
           // video standarts
           var menu = Form.miVideoStandarts;
           foreach (var mi in menu.DropDownItems)
           {
               if ((mi is ToolStripMenuItem) && (mi as ToolStripMenuItem).Text.CompareTo(channel.VideoStandard.ToString()) == 0)
                   (mi as ToolStripMenuItem).PerformClick();

           }

           

        }

        void OnZoomClick(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            if (item.Text.CompareTo("In") == 0)
            {
                controller.Data.Tuner.Zoom += 1;
                ShowMsg("Zoom in");
            }
            else
                if (item.Text.CompareTo("Out") == 0)
                {
                    controller.Data.Tuner.Zoom -= 1;
                    ShowMsg("Zoom out");
                }
                else
                {
                    controller.Data.Tuner.Zoom = 0;
                    ShowMsg("Zoom's reseted");
                }

            if (currentChannel != null)
                currentChannel.Zoom = controller.Data.Tuner.Zoom;
            
        }

        void OnAspectRatioClick(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            foreach (var i in Form.miAspectRatio.DropDownItems)
            {
                if (i != item && (i is ToolStripMenuItem))
                {
                    (i as ToolStripMenuItem).Checked = false;
                    (i as ToolStripMenuItem).Enabled = true;
                }
            }
            item.Checked = true;
            item.Enabled = false;
            if (item.Text.CompareTo("4x3") == 0)
            {
                
                controller.Data.Tuner.AspectRatio = 4 / 3.0f;
                ShowMsg("Aspect Ratio: 4x3");

            }
            if (item.Text.CompareTo("16x9") == 0)
            {
                
                controller.Data.Tuner.AspectRatio = 16 / 9.0f;
                ShowMsg("Aspect Ratio: 16x9");
            }
            if (item.Text.CompareTo("1.66 Letterbox") == 0)
            {
                
                controller.Data.Tuner.AspectRatio = 1.66f;
                ShowMsg("Aspect Ratio: 1.66 Letterbox");

            }
            if (currentChannel != null)
                currentChannel.AspectRatio = controller.Data.Tuner.AspectRatio;
            
        }

        void OnVideoStandartsClick(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            foreach (var mi in Form.miVideoStandarts.DropDownItems)
            {
                if ((mi != item) && (mi is ToolStripMenuItem))
                {
                    (mi as ToolStripMenuItem).Enabled = true;
                    (mi as ToolStripMenuItem).Checked = false;

                }
            }
            item.Enabled = false;
            item.Checked = true;
            var avs = (AnalogVideoStandard) Enum.Parse(typeof(AnalogVideoStandard), item.Text);
            controller.Data.Tuner.AnalogVideoDecoder.put_TVFormat(avs);
            if (currentChannel != null)
                currentChannel.VideoStandard = avs;
        }

        public Form1 Form { get; set; } 

        public Settings Settings {
            get {
                return controller.Data.Settings;
            }
        }

    }

    public class ScanView: System.Object {

        public ScanView(MyController controller)
        {
            form = new Scan();
            this.controller = controller;
            timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Interval = 1000;

            channelList = new ChannelList();
            fromFreq = 92250000;
            toFreq = 814250000;
            //stepFreq = 62500;
            stepFreq = 1000000;
            form.cbVideoStandarts.Items.Clear();
            form.cbChanVideoStandart.Items.Clear();
            foreach (AnalogVideoStandard vs in Enum.GetValues(typeof(AnalogVideoStandard)))
            {
                form.cbVideoStandarts.Items.Add(vs.ToString());
                form.cbChanVideoStandart.Items.Add(vs.ToString());
            }
            form.cbVideoStandarts.Text = AnalogVideoStandard.PAL_D.ToString();
            form.cbChanVideoStandart.SelectedValueChanged += new EventHandler(cbChanVideoStandart_SelectedValueChanged);
            BindUI();
        }

        void cbChanVideoStandart_SelectedValueChanged(object sender, EventArgs e)
        {
            if (currentChannel != null  )
            {
                var s = form.cbChanVideoStandart.SelectedItem.ToString();
                var avs = (AnalogVideoStandard)Enum.Parse(typeof(AnalogVideoStandard), s);
                currentChannel.VideoStandard = avs;
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //if (controller.Data.Scan.IsAlive)
            {
                channelList = controller.Data.Scan.ChannList;
                RefreshGUI();
            }
        }
        private ChannelList channelList;
        private MyController controller;
        private Channel currentChannel;
        private TreeNode currentNode;
        private int fromFreq;
        private Scan form;
        private int stepFreq;
        private System.Timers.Timer timer;
        private int toFreq;
        private Rectangle dragBoxFromMouseDown;

        public void FillChanProp(Channel channel) {
            form.lblID.Text = channel.ID.ToString();
            form.edtName.Text = channel.Name;
           // form.edtFreq.Value =(decimal) (channel.Freq / 1000.0f);
            form.edtFreq.Value = channel.Freq;
            var i = form.cbChanVideoStandart.FindString(channel.VideoStandard.ToString());
            form.cbChanVideoStandart.Text = channel.VideoStandard.ToString();
            
        }

        public DialogResult Show() {
            if (!controller.Data.Tuner.IsScanningSupported())
            {
                MessageBox.Show("Unfortunetly you card doesn't support scanning :(", "uTuner");
                return DialogResult.Abort;
            }
            channelList = controller.Data.ChannelList.Fork();

            RefreshGUI();
            var res = form.ShowDialog();
            if (res == DialogResult.OK)
            {
                SetPositions();
                controller.Data.ChannelList.Merge(channelList, true);
                controller.Data.ChannelList.Sort();
                controller.Data.Save();

            }
            return res;
        }

        private void BindUI() {
            form.edtFrom.Text = fromFreq.ToString();
            form.edtTo.Text = toFreq.ToString();
            form.edtStep.Text = stepFreq.ToString();


            form.edtFreq.ValueChanged +=new EventHandler(edtFreq_ValueChanged);
            form.btnScan.Click += new EventHandler(btnScan_Click);
            form.edtName.TextChanged +=new EventHandler(edtName_TextChanged);
            form.miDelete.Click += new EventHandler(miDelete_Click);
            form.channelsTreeView.DragEnter += new DragEventHandler(channelsTreeView_DragEnter);
            form.channelsTreeView.DragDrop += new DragEventHandler(channelsTreeView_DragDrop);
            form.channelsTreeView.DragOver += new DragEventHandler(channelsTreeView_DragOver);
            form.channelsTreeView.MouseDown += new MouseEventHandler(channelsTreeView_MouseDown);
            form.channelsTreeView.MouseMove += new MouseEventHandler(channelsTreeView_MouseMove);
            form.channelsTreeView.MouseUp += new MouseEventHandler(channelsTreeView_MouseUp);

        }

        void channelsTreeView_MouseUp(object sender, MouseEventArgs e)
        {

            dragBoxFromMouseDown = Rectangle.Empty;
        }

        void channelsTreeView_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {

                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty &&
                    !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    TreeView tree = (TreeView)sender;
                    tree.DoDragDrop(currentNode, DragDropEffects.Copy);

                }
            }
        }

        void channelsTreeView_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the Dataformat of the data can be accepted
            // (we only accept file drops from Explorer, etc.)
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy; // Okay
            else
                e.Effect = DragDropEffects.None; // Unknown data, ignore it
        }

        void channelsTreeView_DragOver(object sender, DragEventArgs e)
        {
            // Get the tree.
            TreeView tree = (TreeView)sender;

            // Drag and drop denied by default.
            e.Effect = DragDropEffects.None;
            var pt = new Point(e.X, e.Y);
            pt = tree.PointToClient(pt);
            
            var node = tree.GetNodeAt(pt);
            if (node != null)
            {
                e.Effect = DragDropEffects.Copy;
                tree.SelectedNode = node;
            }

            
        }

        void channelsTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            TreeView tree = (TreeView)sender;
            var node = tree.GetNodeAt(e.X, e.Y);
            dragBoxFromMouseDown = Rectangle.Empty;
            if (node != null)
            {
                SelectNode(node);

                Size dragSize = SystemInformation.DragSize;
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                                                              e.Y - (dragSize.Height / 2)), dragSize);
            }
        }

 

        void channelsTreeView_DragDrop(object sender, DragEventArgs e)
        {
            // Get the tree.
            TreeView tree = (TreeView)sender;
            // Get the screen point.
            Point pt = new Point(e.X, e.Y);

            // Convert to a point in the TreeView's coordinate system.
            pt = tree.PointToClient(pt);

            // Get the node underneath the mouse.
            TreeNode node = tree.GetNodeAt(pt);
            TreeNode dragNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
            var id1 = Convert.ToInt32(node.Name.Substring("node_".Length));
            var c1 = channelList.GetChannelByID(id1);
            var id2 = Convert.ToInt32(dragNode.Name.Substring("node_".Length));
            var c2 = channelList.GetChannelByID(id2);

            int bufPosition = c1.Position;
            string bufName = node.Name;
            string bufText = node.Text;
            c1.Position = c2.Position;
            c2.Position = bufPosition;
            node.Text = dragNode.Text;
            node.Name = dragNode.Name;
            dragNode.Text = bufText;
            dragNode.Name = bufName;


        }

        void miDelete_Click(object sender, EventArgs e)
        {
            if (currentNode != null)
            {
                var id = Convert.ToInt32(currentNode.Name.Substring("node_".Length));

                channelList.Delete(id);
                form.channelsTreeView.Nodes.Remove(currentNode);
            }
            
        }

        void btnScan_Click(object sender, EventArgs e)
        {
            var scanData = controller.Data.Scan;
            fromForm();
            if (form.btnScan.Text == "Scan")
            {
                form.grpChannelProp.Enabled = false;
                form.channelsTreeView.Enabled = false;
                form.btnScan.Text = "Stop";
                var s = form.cbVideoStandarts.SelectedItem.ToString();
                var avs = (AnalogVideoStandard)Enum.Parse(typeof(AnalogVideoStandard), s);
                try
                {
                    scanData.Start(fromFreq, toFreq, stepFreq, channelList, avs);
                    while (scanData.Active)
                    {
                        scanData.Step();
                        form.edtCurrFreq.Text = scanData.CurrFreq.ToString();

                        RefreshGUI();
                        Application.DoEvents();
                    }
                }
                finally
                {
                    form.btnScan.Text = "Scan";
                    form.grpChannelProp.Enabled = true;
                    form.channelsTreeView.Enabled = true;
                    scanData.Stop();
                }
            }
            else
            {
                form.btnScan.Text = "Scan";
                form.grpChannelProp.Enabled = true;
                form.channelsTreeView.Enabled = true;
                scanData.Stop();

            }
           
        }

        private void RefreshGUI() {
            var tv = form.channelsTreeView;
            tv.Nodes.Clear();
            currentChannel = null;
            currentNode = null;
            foreach (Channel c in channelList)
            {
                var node = tv.Nodes.Add(c.Name);
                node.Name = "node_" + c.ID;
               
              

            }
            if (channelList.Count > 0)
                SelectNode(tv.Nodes[0]);
            

            
        }

        private void SelectNode(TreeNode node) {
            if (controller.Data.Scan.Active)
                return;
            var id = Convert.ToInt32(node.Name.Substring("node_".Length));
            var c = channelList.GetChannelByID(id);
            currentChannel = c;
            controller.Data.Settings.LastChannelID = c.ID;
            currentNode = node;
            form.channelsTreeView.SelectedNode = currentNode;

            controller.Data.Tuner.SetChannel(c);
            FillChanProp(c);
        }

        private void SetPositions() {
            var tv = form.channelsTreeView;
            int i=0;
            foreach (TreeNode node in tv.Nodes)
            {
                var id = Convert.ToInt32(node.Name.Substring("node_".Length));
                var ch = channelList.GetChannelByID(id);
                if (ch != null)
                {
                    ch.Position = i;
                }
                i++;
            }
        }

        private void edtFreq_ValueChanged(object sender, EventArgs e)
        {
            if (currentChannel == null)
                return;

            currentChannel.Freq = (int) form.edtFreq.Value;

        }

        private void edtName_TextChanged(object sender, EventArgs e)
        {
            if (currentChannel == null)
                return;
            TextBox t = (TextBox)sender;
            currentChannel.Name = t.Text;            
            currentNode.Text = t.Text;

        }

        private void fromForm() {
            fromFreq = Convert.ToInt32(form.edtFrom.Text);
            stepFreq = Convert.ToInt32(form.edtStep.Text);
            toFreq = Convert.ToInt32(form.edtTo.Text);
        }

    }

}
