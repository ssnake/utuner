using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using REMOTESERVICELib;
namespace uTuner
{

    public abstract class RemoteControl: System.Object {
        public enum Key {rckNone, rckChannelUp, rckChannelDown, rckMute, rckVolUp, rckVolDown, rckShutDown, rckFullScreen,
                         rck0, rck1, rck2, rck3,rck4, rck5, rck6, rck7, rck8, rck9,
                         rckReturn};

        public delegate void DataEvent(RemoteControl.Key key);

        public abstract void Deinit();

        public abstract void Init();

        public abstract bool IsSupport();

        protected void OnDataEvent(RemoteControl.Key key)
        {
            if (Data != null)
                Data(key);
        }

        public string Name { get; set; }
        public event DataEvent Data;

    }

    public class AverTVRemoteControl: RemoteControl {
       

        public AverTVRemoteControl() {
            Name = "AverTV Remote Control";

        }

        void remote_OnRemoteData(uint nKeyFun, uint nKey, uint dwKeyCode)
        {
            RemoteControl.Key k= RemoteControl.Key.rckNone;
            if (nKey == 1225)
            {
                switch (nKeyFun)
                {
                    case 29: 
                        k =Key.rckChannelUp;
                        break;
                    case 30: 
                        k = Key.rckChannelDown;
                        break;
                    case 26: 
                        k = Key.rckMute;
                        break;
                    case 28: k = Key.rckVolDown;
                        break;
                    case 27: 
                        k = Key.rckVolUp;
                        break;
                    case 1: k = Key.rckShutDown;
                        break;
                    case 53: k = Key.rckFullScreen;
                        break;
                    case 31: k = Key.rck0;
                        break;
                    case 32: k = Key.rck1;
                        break;
                    case 33: k = Key.rck2;
                        break;
                    case 34: k = Key.rck3;
                        break;
                    case 35: k = Key.rck4;
                        break;
                    case 36: k = Key.rck5;
                        break;
                    case 37: k = Key.rck6;
                        break;
                    case 38: k = Key.rck7;
                        break;
                    case 39: k = Key.rck8;
                        break;
                    case 40: k = Key.rck9;
                        break;
                    case 52: k = Key.rckReturn;
                        break;



                };

                OnDataEvent(k);
            }
        }

        ~AverTVRemoteControl() {
            Deinit();

        }
        private Remote remote;

        public override void Deinit() {
            if (remote != null)
            {
                try
                {
                    remote.UninitialQuick();
                }
                catch
                {
                }

                remote = null;
            }
        }

        public override void Init() {
            remote = new REMOTESERVICELib.Remote();
            remote.InitialQuick();
            uint c;
            remote.GetDeviceNum(out c);
            if (c > 0)
            {
                REMOTESERVICELib.tagHWINFO hwInfo;
                remote.EnumDeviceInfo(0, out hwInfo);
                remote.SetRemoteIsEnable(0, hwInfo.lEnumRemoteID, true);
                //Name = hwInfo.szDeviceName;
                remote.OnRemoteData += new _IRemoteEvents_OnRemoteDataEventHandler(remote_OnRemoteData);
            }
        }

        public override bool IsSupport() {
            try
            {
                var o = Activator.CreateInstance(typeof(REMOTESERVICELib.RemoteClass));
                return o != null;
            }
            catch (COMException e)
            {
                return false;

            }
        }

    }

}
