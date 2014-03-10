using System;
using System.Runtime.InteropServices;
namespace uTuner
{  //dword = uInt32
    class DsConstants
    {
          public static int AMTUNER_SUBCHAN_NO_TUNE = -2;
          public static int AMTUNER_SUBCHAN_DEFAULT = -1;

          public static Int32 AMTUNER_HASNOSIGNALSTRENGTH = -1;
          public static Int32 AMTUNER_NOSIGNAL = 0;
          public static Int32 AMTUNER_SIGNALPRESENT = 1;

          public static UInt32 AMTUNER_MODE_DEFAULT    = 0;
          public static UInt32 AMTUNER_MODE_TV = 0x1;
          public static UInt32 AMTUNER_MODE_FM_RADIO = 0x2;
          public static UInt32 AMTUNER_MODE_AM_RADIO = 0x4;
          public static UInt32 AMTUNER_MODE_DSS = 0x8;
    }
    class KSPROPERTY_TUNER
    {
        public static int TUNER_CAPS = 0;              // R  -overall device capabilities
        public static int TUNER_MODE_CAPS = 1;         // R  -capabilities in this mode
        public static int TUNER_MODE = 2;              // RW -set a mode (TV, FM, AM, DSS)
        public static int TUNER_STANDARD = 3;          // R  -get TV standard (only if TV mode)
        public static int TUNER_FREQUENCY = 4;         // RW -set/get frequency
        public static int TUNER_INPUT = 5;             // RW -select an input
        public static int TUNER_STATUS = 6;            // R  -tuning status
        public static int TUNER_IF_MEDIUM = 7;          // R O-Medium for IF or Transport Pin
    }
    class KS_TUNER_TUNING_FLAGS 
    {
        public static int TUNING_Invalid = 0;        
        public static int TUNING_EXACT = 1;         // No fine tuning
        public static int TUNING_FINE = 2;              // Fine grained search
        public static int TUNING_COARSE = 3;          // Coarse search
      
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x10)]
    struct GUID {
        UInt32 Data1;
        UInt32  Data2;
        UInt32  Data3;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Data4;
    }
    public struct KSPROPERTY
    {
        GUID Set;
        int Id;
        int Flags;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct KSPROPERTY_TUNER_MODE_CAPS
    {
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 Mode;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 StandardsSupported;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 MinFrequency;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 MaxFrequency;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 TuningGranularity;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 NumberOfInputs;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SettlingTime;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 Strategy;
       // [MarshalAs(UnmanagedType.U4)]
       // public int Dummy;
        

    }
    [StructLayout(LayoutKind.Sequential)]
    public struct KSPROPERTY_TUNER_MODE_CAPS_S
    {
        public KSPROPERTY Property;
        public KSPROPERTY_TUNER_MODE_CAPS Instance;

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KSPROPERTY_TUNERFREQUENCY
    {
        /// <summary /> Hz </summary />
        [MarshalAs(UnmanagedType.U4)]
        public int Frequency;
        /// <summary /> Hz (last known good) </summary />
        [MarshalAs(UnmanagedType.U4)]
        public int LastFrequency;
        /// <summary /> KS_TUNER_TUNING_FLAGS </summary />
        [MarshalAs(UnmanagedType.U4)]
        public int TuningFlags;
        /// <summary /> DSS </summary />
        [MarshalAs(UnmanagedType.U4)]
        public int VideoSubChannel;
        /// <summary /> DSS </summary />
        [MarshalAs(UnmanagedType.U4)]
        public int AudioSubChannel;
        /// <summary /> Channel number </summary />
        [MarshalAs(UnmanagedType.U4)]
        public int Channel;
        /// <summary /> Country number </summary />
        [MarshalAs(UnmanagedType.U4)]
        public int Country;
        /// <summary /> Undocumented or error ... </summary />
        [MarshalAs(UnmanagedType.U4)]
        public int Dummy;
        // Dummy added to get a successful return of the Get, Set 
        // function
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct KSPROPERTY_TUNER_FREQUENCY_S
    {
        /// <summary /> Property Guid </summary />
        public KSPROPERTY Property;
        /// <summary /> Tuner frequency data structure 
        /// </summary />
        public KSPROPERTY_TUNERFREQUENCY Instance;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KSPROPERTY_TUNER_STATUS
    {
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 CurrentFrequency;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 PLLOffset;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SignalStrength;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 Busy;
    //    [MarshalAs(UnmanagedType.U4)]
    //    public int Dummy;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct KSPROPERTY_TUNER_STATUS_S
    {
        /// <summary /> Property Guid </summary />
        public KSPROPERTY Property;
        /// <summary /> Tuner frequency data structure 
        /// </summary />
        public KSPROPERTY_TUNER_STATUS Instance;
    }
}