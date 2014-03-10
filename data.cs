using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

using DirectShowLib;
using System.Threading;

namespace uTuner
{
    public class MyData
    {

        public MyData() {
            channelList = new ChannelList();

            tuner = new MyTuner();
            scan = new ScanData(tuner);
            OverlayGraphics = new OverlayGraphics(tuner);
            RemoteControls = new List<RemoteControl>();
            InitRemoteControls();
 

        }
        private ChannelList channelList;
        private RemoteControl remoteControl;
        private ScanData scan;
        private Settings settings;
        private MyTuner tuner;

        public void InitRemoteControls() {
            RemoteControls.Add(new AverTVRemoteControl());
        }

        public void Load() {
            
            if (File.Exists("channels.json"))
            {
                var ser = new Serializer<ChannelList>();
                channelList = ser.JSON2Serialize("channels.json", true);
                channelList.Sort();
            }
            if (File.Exists("settings.json"))
            {
                var ser = new Serializer<Settings>();
                settings = ser.JSON2Serialize("settings.json", true);
            }
            else
                settings = new Settings();
        }

        public void Save() {
            
            var ser = new Serializer<ChannelList>();
            //foreach (Channel c in channelList)
              //  c.VideoStandard = AnalogVideoStandard.PAL_D;
            ser.Serialize2JSON(channelList, "channels.json");
            var ser2 = new Serializer<Settings>();
            ser2.Serialize2JSON(settings, "settings.json");
            
        }

        public ChannelList ChannelList {
            get {
                return channelList;
            }
        }

        public OverlayGraphics OverlayGraphics { get; set; }
        public RemoteControl RemoteControl { 
            get {
                return remoteControl;
            }
            set {
                if (remoteControl != value) {
                    if (remoteControl != null)
                        remoteControl.Deinit();
                    remoteControl = value;
                    if (remoteControl != null)
                        remoteControl.Init();
                }
            }
        }

        public List<RemoteControl> RemoteControls { get; set; }

        public ScanData Scan {
            get {
                return scan;
            }
        }

        public Settings Settings {
            get {
                return settings;
            }
        }

        public MyTuner Tuner { 
            get {
                return tuner;
            }
        }

    }

    public class ScanData: System.Object {

        public ScanData(MyTuner tuner) {
            this.tuner = tuner;
        }
        private AnalogVideoStandard analogVideoStandart;
        private ChannelList chanList;
        private int currFreq;
        private int fromFreq;
        private int stepFreq;
        private bool terminated;

        private int toFreq;
        private MyTuner tuner;

        public void Step() {
            
            
            AMTunerSignalStrength strength;
            if (currFreq < toFreq && !terminated) 
            {
                try
                {
                    
                    tuner.SetFreq(currFreq);
                    tuner.Tuner.SignalPresent(out strength);
                    int num;
                    tuner.AnalogVideoDecoder.get_NumberOfLines(out num);
                    num++;

                    if (strength == AMTunerSignalStrength.SignalPresent)
                    {
                        lock(chanList)
                            chanList.Merge(new Channel(currFreq.ToString(), currFreq)
                            {
                                ID = currFreq,
                                VideoStandard = analogVideoStandart
                            });
                    }
                }
                //catch (Exception e)
                finally
                {
                };
                
            };
            currFreq += stepFreq;
        }

        public void Start(int fromFreq, int toFreq, int stepFreq, ChannelList channelList, AnalogVideoStandard analogVideoStandart)
        {
            this.fromFreq = fromFreq;
            this.toFreq = toFreq;
            this.stepFreq = stepFreq;
            currFreq = fromFreq;
            tuner.Mute = true;
            terminated = false;
            tuner.AnalogVideoDecoder.put_TVFormat(analogVideoStandart);
            this.analogVideoStandart = analogVideoStandart;
            this.chanList = channelList;

        }

        public void Stop() {

            terminated = true;
            tuner.Mute = false;

        }

        public ChannelList ChannList {
            get {
                
                    return chanList;
            }
        }

        public bool Active {
            get {

                return currFreq < toFreq && !terminated;
            }
        }

        public int CurrFreq {
            get {
                return currFreq;
            }
        }

    }
    [DataContractAttribute]
    public class Settings: System.Object {

        public Settings() {
            DeinterlaceFilter = None;
            Volume = 100;
        }
        public static string None = "None";

        [DataMember]
        public string DeinterlaceFilter { get; set; }

        [DataMember]
        public string RemoteControl { get; set; }
        [DataMember]
        public string Device { get; set; }
        [DataMember]
        public bool ROT { get; set; }
        [DataMember]
        public int Volume { get; set; } 

        [DataMember]
        public int LastChannelID { get; set; }
        [DataMember]
        public bool Mute { get; set; }
    }

}
