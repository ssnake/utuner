using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using DirectShowLib;
namespace uTuner
{

    public class MyTuner
    {
        public const int ZoomStep = 20;
        public MyTuner() {
            VideoDeviceList = new List<string>();
            AudioDeviceList = new List<string>();
           
            Init();
            

        }

        ~MyTuner() {
            try
            {
                DerenderDevice();
            }
            catch { };
            try
            {
                DeInit();
            }
            catch
            {
            }
             
        }

        private IAMStreamConfig  AMStreamConfig;
        private IAMAnalogVideoDecoder analogVideoDecoder;
        private IAMCrossbar crossbar;
        private IBaseFilter videoDevice;
        private IBaseFilter audioDevice;
        private ICaptureGraphBuilder2 captureGraphBuilder;
        private bool mute;
        private IAMTVTuner tuner;
        private bool useVMR9 = true;
        private IBaseFilter videoRendererFilter;
        private IVideoWindow videoWindow;
        private IGraphBuilder graphBuilder = null;
        IMediaControl mediaControl = null;
        IMediaEvent mediaEvent = null;
        public DsROTEntry rOT;
        private float aspectRatio;
        private DeInterlaceLayout deInterlaceLayout;
        private bool useROT;
        private int volume;
        private int zoom;
        internal static readonly Guid PROPSETID_TUNER = new Guid(0x6a2e0605, 0x28e4, 0x11d0, 0xa1, 0x8c, 0x00, 0xa0, 0xc9, 0x11, 0x89, 0x56);
        //A (modified) definition of OleCreatePropertyFrame found here: http://groups.google.no/group/microsoft.public.dotnet.languages.csharp/browse_thread/thread/db794e9779144a46/55dbed2bab4cd772?lnk=st&q=[DllImport(%22olepro32.dll%22)]&rnum=1&hl=no#55dbed2bab4cd772
        [DllImport("oleaut32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int OleCreatePropertyFrame(
            IntPtr hwndOwner,
            int x,
            int y,
            [MarshalAs(UnmanagedType.LPWStr)] string lpszCaption,
            int cObjects,
            [MarshalAs(UnmanagedType.Interface, ArraySubType = UnmanagedType.IUnknown)] 
            ref object ppUnk,
            int cPages,
            IntPtr lpPageClsID,
            int lcid,
            int dwReserved,
            IntPtr lpvReserved);
        public void RefreshDevices() {
            VideoDeviceList.Clear();
            AudioDeviceList.Clear();

            foreach (DsDevice ds in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
            {
                VideoDeviceList.Add(ds.Name);
            }
            foreach (DsDevice ds in DsDevice.GetDevicesOfCat(FilterCategory.AMKSTVAudio))
            {
                AudioDeviceList.Add(ds.Name);
            }
        }

        public void SetVideoDeviceByName(string deviceName) {
            var dev = CreateFilter(FilterCategory.VideoInputDevice, deviceName);
            initDevice(dev);
        }

        public void SetAudioDeviceByName(string deviceName) {
            var dev = CreateFilter(FilterCategory.AMKSTVAudio, deviceName);
          //  initDevice(dev);
        }

        public void SetFreq(int freq) {
            if (tuner == null)
                return;
            //var PROPSETID_TUNER = new Guid("6a2e0605-28e4-11d0-a18c-00a0c9118956");
            IKsPropertySet prop = (IKsPropertySet)Tuner;

            KSPropertySupport ps;
  

            // Check either the Property is supported or not by the Tuner drivers 
            var hr = prop.QuerySupported(PROPSETID_TUNER, KSPROPERTY_TUNER.TUNER_FREQUENCY, out ps);
            if (hr >= 0 && (ps & KSPropertySupport.Get) == KSPropertySupport.Get && (ps & KSPropertySupport.Set) == KSPropertySupport.Set)
            {
                //prepeare data
                //KSPROPERTY_TUNER_MODE_CAPS_S ModeCaps = new KSPROPERTY_TUNER_MODE_CAPS_S();
                KSPROPERTY_TUNER_FREQUENCY_S Frequency = new KSPROPERTY_TUNER_FREQUENCY_S();
                IntPtr freqData = Marshal.AllocCoTaskMem(Marshal.SizeOf(Frequency));
                IntPtr instData = Marshal.AllocCoTaskMem(Marshal.SizeOf(Frequency.Instance));
                Marshal.StructureToPtr(Frequency, freqData, true);
                Marshal.StructureToPtr(Frequency.Instance, instData, true);


                int bytes = 0;
                hr = prop.Get(
                  PROPSETID_TUNER,
                  (int)KSPROPERTY_TUNER.TUNER_FREQUENCY,
                  instData,
                  Marshal.SizeOf(Frequency.Instance),
                  freqData,
                  Marshal.SizeOf(Frequency),
                  out bytes);
                if (hr == 0)
                {
                    // Specify the TV broadcast frequency and tuning flag
                    Frequency.Instance.Frequency = freq;
                    Frequency.Instance.TuningFlags =
                        (int)KS_TUNER_TUNING_FLAGS.TUNING_EXACT;

                    // Convert the data
                    Marshal.StructureToPtr(Frequency, freqData, true);
                    Marshal.StructureToPtr(Frequency.Instance, instData, true);

                    // Now change the broadcast frequency
                    hr = prop.Set(
                        PROPSETID_TUNER,
                        (int)KSPROPERTY_TUNER.TUNER_FREQUENCY,
                        instData,
                        Marshal.SizeOf(Frequency.Instance),
                        freqData,
                        Marshal.SizeOf(Frequency));
                    DsError.ThrowExceptionForHR(hr);
                };
                Resize();
                DsError.ThrowExceptionForHR(hr);
            }
        }

        public void Start() {
            if (mediaControl != null)
            {

                mediaControl.Run();

                return;
               

            }
        }

        public void Stop() {
            if (mediaControl != null)
            mediaControl.Stop();
        }

        /// <summary>
        /// Enumerates all filters of the selected category and returns the IBaseFilter for the 
        /// filter described in friendlyname
        /// </summary>
        /// <param name="category">Category of the filter</param>
        /// <param name="friendlyname">Friendly name of the filter</param>
        /// <returns>IBaseFilter for the videoDevice</returns>
        private IBaseFilter CreateFilter(Guid category, string friendlyname)
        {
            object source = null;
            Guid iid = typeof(IBaseFilter).GUID;
            foreach (DsDevice videoDevice in DsDevice.GetDevicesOfCat(category))
            {
                if (videoDevice.Name.CompareTo(friendlyname) == 0)
                {
                    videoDevice.Mon.BindToObject(null, null, ref iid, out source);
                    break;
                }
            }

            return (IBaseFilter)source;
        }

        /// <summary>
        /// Displays a property page for a filter
        /// </summary>
        /// <param name="dev">The filter for which to display a property page</param>
        public void DisplayPropertyPage(IBaseFilter dev)
        {
            //Get the ISpecifyPropertyPages for the filter
            ISpecifyPropertyPages pProp = dev as ISpecifyPropertyPages;
            int hr = 0;

            if (pProp == null)
            {
                //If the filter doesn't implement ISpecifyPropertyPages, try displaying IAMVfwCompressDialogs instead!
                IAMVfwCompressDialogs compressDialog = dev as IAMVfwCompressDialogs;
                if (compressDialog != null)
                {

                    hr = compressDialog.ShowDialog(VfwCompressDialogs.Config, IntPtr.Zero);
                    DsError.ThrowExceptionForHR(hr);
                }
                else
                {
                    MessageBox.Show("Item has no property page", "No Property Page", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                return;
            }

            //Get the name of the filter from the FilterInfo struct
            FilterInfo filterInfo;
            hr = dev.QueryFilterInfo(out filterInfo); 
            DsError.ThrowExceptionForHR(hr);

            // Get the propertypages from the property bag
            DsCAUUID caGUID;
            hr = pProp.GetPages(out caGUID);
            DsError.ThrowExceptionForHR(hr);

            //Create and display the OlePropertyFrame
            object oDevice = (object)dev;
            hr = OleCreatePropertyFrame(Render.Handle, 0, 0, filterInfo.achName, 1, ref oDevice, caGUID.cElems, caGUID.pElems, 0, 0, IntPtr.Zero);
            DsError.ThrowExceptionForHR(hr);

            Marshal.ReleaseComObject(oDevice);

            if (filterInfo.pGraph != null)
            {
                Marshal.ReleaseComObject(filterInfo.pGraph);
            }

            // Release COM objects
            Marshal.FreeCoTaskMem(caGUID.pElems);
        }

        public Size GetCaptureScreenSize() {
            AMMediaType mt;
            var hr = AMStreamConfig.GetFormat(out mt);
            
            DsError.ThrowExceptionForHR(hr);
            // copy out the videoinfoheader
            VideoInfoHeader v = new VideoInfoHeader();
            Marshal.PtrToStructure(mt.formatPtr, v);
            
            return new Size(v.BmiHeader.Width, v.BmiHeader.Height);

        }

        public KSPROPERTY_TUNER_STATUS_S GetStatus() {
            var status = new KSPROPERTY_TUNER_STATUS_S();
            IKsPropertySet prop = (IKsPropertySet)Tuner;
            KSPropertySupport ps;


            // Check either the Property is supported or not by the Tuner drivers 
            var hr = prop.QuerySupported(PROPSETID_TUNER, KSPROPERTY_TUNER.TUNER_STATUS, out ps);
            if (hr >= 0 && (ps & KSPropertySupport.Get) == KSPropertySupport.Get)
            {
                //prepeare data

                IntPtr statusPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(status));
                IntPtr statusInstPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(status.Instance));
                Marshal.StructureToPtr(status, statusPtr, true);
                Marshal.StructureToPtr(status.Instance, statusInstPtr, true);


                int bytes = 0;
                hr = prop.Get(
                  PROPSETID_TUNER,
                  (int)KSPROPERTY_TUNER.TUNER_STATUS,
                  statusInstPtr,
                  Marshal.SizeOf(status.Instance),
                  statusPtr,
                  Marshal.SizeOf(status),
                  out bytes);
                DsError.ThrowExceptionForHR(hr);
            }

            return status;
        }

        public void GetModeCaps() {
            //var PROPSETID_TUNER = new Guid("6a2e0605-28e4-11d0-a18c-00a0c9118956");
            IKsPropertySet prop = (IKsPropertySet)Tuner;

            KSPropertySupport ps;
  

            // Check either the Property is supported or not by the Tuner drivers 
            var hr = prop.QuerySupported(PROPSETID_TUNER, KSPROPERTY_TUNER.TUNER_MODE_CAPS, out ps);
            if (hr >= 0 && (ps & KSPropertySupport.Get) == KSPropertySupport.Get)
            {
                //prepeare data
                KSPROPERTY_TUNER_MODE_CAPS_S modeCaps = new KSPROPERTY_TUNER_MODE_CAPS_S();

                IntPtr modeCapsPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(modeCaps));
                IntPtr modeCapsInstPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(modeCaps.Instance));
                modeCaps.Instance.Mode = (int)AMTunerModeType.TV;
                Marshal.StructureToPtr(modeCaps, modeCapsPtr, true);
                Marshal.StructureToPtr(modeCaps.Instance, modeCapsInstPtr, true);


                int bytes = 0;
                hr = prop.Get(
                  PROPSETID_TUNER,
                  (int)KSPROPERTY_TUNER.TUNER_MODE_CAPS,
                  modeCapsInstPtr,
                  Marshal.SizeOf(modeCaps.Instance),
                  modeCapsPtr,
                  Marshal.SizeOf(modeCaps),
                  out bytes);
                DsError.ThrowExceptionForHR(hr);
            }
        }

        public void SetChannel(Channel aChannel) {
            if (tuner == null)
                return;

            AnalogVideoDecoder.put_TVFormat(aChannel.VideoStandard);
            SetFreq(aChannel.Freq);
            Zoom = aChannel.Zoom;
            if (aChannel.AspectRatio != 0)
                AspectRatio = aChannel.AspectRatio;
            else
                AspectRatio = 4 / 3.0f;

            
           
        }

        private bool AddDeInterlaceFilter()
        {/*
           // FilterCategory.LegacyAmFilterCategory
            if(this.DeInterlace != null)
            {
                if(this.deInterlaceFilter != null)
                {
                    this.graphBuilder.RemoveFilter(this.deInterlaceFilter);
                    Marshal.ReleaseComObject(this.deInterlaceFilter);
                    this.deInterlaceFilter = null;
                }
                this.deInterlaceFilter = (IBaseFilter) Marshal.BindToMoniker(this.DeInterlace.MonikerString);
                int hr = this.graphBuilder.AddFilter(this.deInterlaceFilter, "De-Interlace filter");
                if(hr >= 0)
                {
                    return true;
                }
            }*/
            return false;
        }

        private void DeInit() {
            DeinterlaceLayoutList = null;
            rOT = null;
            mediaEvent = null;
            mediaControl = null;

            if (graphBuilder != null)
                Marshal.ReleaseComObject(graphBuilder); 
            graphBuilder = null;
            
            if (captureGraphBuilder != null)
                Marshal.ReleaseComObject(captureGraphBuilder); 
            captureGraphBuilder = null;
          //  GC.Collect();
        }

        private void DerenderDevice() {
            Stop();
           
            DisableVideoWindow();
            if (videoDevice != null)
                removeDownstream(videoDevice, true);
            if (audioDevice != null)
                removeDownstream(audioDevice, true);
            videoRendererFilter = null;
            
            DeinitDeInterlace();

            GC.Collect();
            

        }

        private void DisableVideoWindow() {
            // Free the preview window (ignore errors)
            if (videoWindow != null)
            {

                videoWindow.put_Visible(OABool.False);
                videoWindow.put_Owner(IntPtr.Zero);
                videoWindow = null;
            }

            // Remove the Resize event handler
            if (Render != null)
                Render.Resize -= new EventHandler(onResize);
        }

                /// <summary>
                ///  Removes all filters downstream from a filter from the graph.
                ///  This is called only by derenderGraph() to remove everything
                ///  from the graph except the devices and compressors. The parameter
                ///  "removeFirstFilter" is used to keep a compressor (that should
                ///  be immediately downstream of the device) if one is begin used.
                /// </summary>
                private void removeDownstream( IBaseFilter filter, bool removeFirstFilter )
                {
                    if (filter == null)
                        return;

                    // Get a pin enumerator off the filter
                    IEnumPins pinEnum;
                    int hr = filter.EnumPins( out pinEnum );
                    pinEnum.Reset();
                    if( (hr == 0) && (pinEnum != null) )
                    {
                        // Loop through each pin
                        IPin[] pins = new IPin[1];

                        IntPtr f = IntPtr.Zero;

                        do
                        {
                            // Get the next pin


                            hr = pinEnum.Next(1, pins, f);

                            if( (hr == 0) && (pins[0] != null) )
                            {
                                
                                // Get the pin it is connected to
                                IPin pinTo = null;
                                pins[0].ConnectedTo( out pinTo );
                                if ( pinTo != null )
                                {
                                    // Is this an input pin?
                                    PinInfo info = new PinInfo();
                                    hr = pinTo.QueryPinInfo( out info );
                                    if( (hr == 0) && (info.dir == (PinDirection.Input)) )
                                    {
                                        // Recurse down this branch
                                        removeDownstream( info.filter, true );

                                        // Disconnect 
                                        graphBuilder.Disconnect( pinTo );
                                        graphBuilder.Disconnect( pins[0] );

                                        // Remove this filter
                                        // but don't remove the video or audio compressors
                                     //   if ( ( info.filter != videoCompressorFilter ) &&
                                     //        ( info.filter != audioCompressorFilter ) )
                                            graphBuilder.RemoveFilter( info.filter );
                                    }
                                    Marshal.ReleaseComObject( info.filter );
                                    Marshal.ReleaseComObject( pinTo );
                                }
                                Marshal.ReleaseComObject( pins[0] );
                            }
                        }
                        while( hr == 0 );

                        Marshal.ReleaseComObject( pinEnum ); pinEnum = null;
                    }
                }

        private void InitAMStreamConfig(ICaptureGraphBuilder2 captureGraphBuilder2, IBaseFilter aDev) {
            Object o;
            if (AMStreamConfig != null)
            {
               // IBaseFilter bf = (IBaseFilter)AMStreamConfig;
              //  RemoveFilter(ref bf);
            }
            var hr = captureGraphBuilder2.FindInterface(PinCategory.Capture, MediaType.Video, aDev, typeof(IAMStreamConfig).GUID, out o);
            DsError.ThrowExceptionForHR(hr);
            AMStreamConfig = o as IAMStreamConfig;
            
            if (AMStreamConfig == null)
            {
                throw new Exception("Failed to get IAMStreamConfig");
            }
            
 
        }

        private void Init() {
            
            graphBuilder = (IGraphBuilder)new FilterGraph();
            
            
            
            //Create the media control for controlling the graph
            mediaControl = (IMediaControl)graphBuilder;

            mediaEvent = (IMediaEvent)graphBuilder;
            volume = 100;
            captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            // initialize the Capture Graph Builder
            int hr = captureGraphBuilder.SetFiltergraph(this.graphBuilder);
            DsError.ThrowExceptionForHR(hr);

            
            DeinterlaceLayoutList = new DeinterlaceList();
            DeinterlaceLayoutList.Add(new DeInterlaceAlparyLayout(this));
            DeinterlaceLayoutList.Add(new DeInterlaceDscalerLayout(this));
            DeinterlaceLayoutList.Add(new DeInterlaceFFDShowLayout(this));
            aspectRatio = 4.0f / 3.0f;

        }

        private void InitCrossbar() {
 

                if (crossbar.CanRoute(1, 3) == 0)//True => can be routed
                    crossbar.Route(1, 3);
                else
                {
                    if (crossbar.CanRoute(1, 7) == 0)
                        crossbar.Route(1, 7);
                }
        
        } 

        private void InitDeInterlace() {
          

           
            
            //deInterlaceLayout = new DeInterlaceAlparyLayout(this);
            if (deInterlaceLayout != null)
                deInterlaceLayout.Init();
        }

        private void DeinitDeInterlace() {
            if (deInterlaceLayout != null)
            {
                deInterlaceLayout.Clear();
                //deInterlaceLayout = null;
                
            }
            
        }
        


        private void InitVideoWindow() {
            if (Render == null)
                return;
            
            
            videoWindow = (IVideoWindow)graphBuilder;
            
            //Set the owener of the videoWindow to an IntPtr of some sort (the Handle of any control - could be a form / button etc.)
            var hr = videoWindow.put_Owner(Render.Handle);
            DsError.ThrowExceptionForHR(hr);
            //Set the style of the video window
            hr = videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren | WindowStyle.ClipSiblings);
            DsError.ThrowExceptionForHR(hr);
           
            // Position video window in client rect of main application window
            //hr = videoWindow.SetWindowPosition(0, 0, Render.Width, Render.Height);
            Resize();
            DsError.ThrowExceptionForHR(hr);
            videoWindow.put_Visible(OABool.True);
            Render.SizeChanged -= new EventHandler(onResize);
            Render.SizeChanged += new EventHandler(onResize);
           
        }

        private void InitTuner(ICaptureGraphBuilder2 captureGraphBuilder) {
            
            Object o;
            var hr = captureGraphBuilder.FindInterface(null, null, videoDevice, typeof(IAMTVTuner).GUID, out o);
            if (hr >= 0)
            {

                tuner = (IAMTVTuner)o;
                //tuner.put_Mode(AMTunerModeType.TV);
                o = null;
                //find crossbar
                var list = findCrossbars(captureGraphBuilder, (IBaseFilter)tuner);
                /*hr = captureGraphBuilder.FindInterface(null, null, (IBaseFilter)Tuner, typeof(IAMCrossbar).GUID, out o);
                if (hr >= 0)
                {
                    crossbar = (IAMCrossbar)o;
                    InitCrossbar();
                }
                else
                    crossbar = null;
                */
                if (list.Count > 0)
                {
                    crossbar = (IAMCrossbar)list[0];
                    InitCrossbar();
                    
                }
                o = null;
                // find amtvaudio
                hr = captureGraphBuilder.FindInterface(null, null, videoDevice, typeof(IAMTVAudio).GUID, out o);
                if (hr >= 0)
                {
                    TVAudio = (IAMTVAudio)o;

                }
                o = null;
                // find IAMAnalogVideoDecoder
                hr = captureGraphBuilder.FindInterface(null, null, videoDevice, typeof(IAMAnalogVideoDecoder).GUID, out o);
               
                if (hr >= 0)
                {
                    analogVideoDecoder = (o as IAMAnalogVideoDecoder);
                    AnalogVideoStandard avs;
                    analogVideoDecoder.get_TVFormat(out avs);
                   
                   
                }
                
                o = null;
            }
            else
                tuner = null;
        }

        private bool InitVideoRenderer()
        {
            RemoveFilter(ref videoRendererFilter);
            if(this.useVMR9)
            {
                videoRendererFilter = (IBaseFilter)new VideoMixingRenderer9();
                //videoRendererFilter = (IBaseFilter)new EnhanchedVideoRender();
               // IVMRMixerBitmap9 bitmap = VideoRendererFilter as IVMRMixerBitmap9;
                //VMR9AlphaBitmap bit;
                
                
                //bitmap.SetAlphaBitmap(
            }
            else
            {
                videoRendererFilter = (IBaseFilter)new VideoRenderer();
            }

            if (this.VideoRendererFilter != null)
            {
                this.graphBuilder.AddFilter(videoRendererFilter, "Video Renderer");

            }
            //Render any preview pin of the videoDevice
            DsGuid cat = PinCategory.Preview;
            DsGuid med = MediaType.Video;
            var hr = captureGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Video, videoDevice, null, this.VideoRendererFilter);
            DsError.ThrowExceptionForHR(hr);
            return false;
        }

        public void RemoveFilter(ref IBaseFilter filter) {
            if (filter != null)
            {
                
                graphBuilder.RemoveFilter(filter);

                Marshal.ReleaseComObject(filter);
                filter = null;
                
            }
        }

        public void SetFullScreen() {
            videoWindow.put_FullScreenMode(OABool.True);
        }

        public void CancelFullScreen() {
            videoWindow.put_FullScreenMode(OABool.False);
        }

        public bool IsScanningSupported() {
            
            if (tuner == null)
                return false;
           
            IKsPropertySet prop = (IKsPropertySet)Tuner;

            KSPropertySupport ps;            
            var hr = prop.QuerySupported(PROPSETID_TUNER, KSPROPERTY_TUNER.TUNER_FREQUENCY, out ps);
            return (hr >= 0 && (ps & KSPropertySupport.Get) == KSPropertySupport.Get && (ps & KSPropertySupport.Set) == KSPropertySupport.Set);
            
        }

        public void Refresh() {
            DerenderDevice();
            RenderDevice();
            Start();
        }

                /// <summary>
                ///	 Retrieve a list of crossbar filters in the graph.
                ///  Most hardware devices should have a maximum of 2 crossbars, 
                ///  one for video and another for audio.
                /// </summary>
                protected ArrayList findCrossbars(ICaptureGraphBuilder2 graphBuilder, IBaseFilter deviceFilter)
                {
                    ArrayList crossbars = new ArrayList();
                    
                    DsGuid category = FindDirection.DownstreamOnly;
                    DsGuid type = new DsGuid();
                    DsGuid riid = typeof(IAMCrossbar).GUID;
                    int hr;

                    object comObj = null;
                    object comObjNext = null;

                    // Find the first interface, look upstream from the selected device

                    hr = graphBuilder.FindInterface(category, type, deviceFilter, riid, out comObj);

                    while ( (hr == 0) && (comObj != null) )
                    {
                        // If found, add to the list
                        if ( comObj is IAMCrossbar )
                        {
                            crossbars.Add( comObj as IAMCrossbar );

                            // Find the second interface, look upstream from the next found crossbar

                            hr = graphBuilder.FindInterface(category, type, comObj as IBaseFilter, riid, out comObjNext);

                            comObj = comObjNext;
                        }
                        else
                            comObj = null;
                    }

                    return( crossbars );
                }

        private void RenderDevice() {
            InitDeInterlace();            
            
            InitVideoRenderer();
           
            
            //try to set default audio render 
            var hr = captureGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Audio, videoDevice, null, null);
            // DsError.ThrowExceptionForHR(hr);  


            InitAMStreamConfig(captureGraphBuilder, videoDevice);
            InitTuner(captureGraphBuilder);
            InitVideoWindow();

            if (mute)
                SetVolume(0);
            else
                SetVolume((uint) volume);
        }

        private void Resize() {
           // var videoWindow = (IVideoWindow)graphBuilder;
            int top =0, left=0, width=0, height = 0;

            if (this.AMStreamConfig == null)
            {
                width = Render.Width;
                height = Render.Height;
            }
            else
            {

                var s = GetCaptureScreenSize();
                
                height = (int)(Render.Height);
                height += (int)(zoom * ZoomStep * (Render.Height * 1.0f / s.Height));
                width = (int)(height * aspectRatio);
                
                left = (Render.Width - width) / 2;
                top = (Render.Height - height) / 2;

            };
            //height = (int)(width / aspectRatio);
            // Position video window in client rect of main application window
            var hr = videoWindow.SetWindowPosition(left, top, width, height);
            DsError.ThrowExceptionForHR(hr);
        }

        private void SetVolume(uint volume) {
            if (mute && volume != 0)
                return;
            IBasicAudio basicAudio = graphBuilder as IBasicAudio;
            int hr = basicAudio.put_Volume((int)(volume- 100)*100);
            //DsError.ThrowExceptionForHR(hr);
        }

        private void initDevice(IBaseFilter newDevice) {
            DerenderDevice();

            if (videoDevice != null)
            {
                RemoveFilter(ref videoDevice);
                videoDevice = null;
            }             
            videoDevice = newDevice;

            
            //Add the Video input videoDevice to the graph
            var hr = graphBuilder.AddFilter(videoDevice, "source filter");
            DsError.ThrowExceptionForHR(hr);

            RenderDevice();


        }
        private void onResize(object sender, EventArgs e)
        {
            Resize();

        }

        public IAMAnalogVideoDecoder AnalogVideoDecoder {
            get {
                return analogVideoDecoder;
            }
        }

        public float AspectRatio {
            get {
                return aspectRatio;
            }
            set {
                if (aspectRatio != value) {
                    aspectRatio = value;
                    Resize();
                }
            }
        }

        public VideoInfoHeader VideoInfoHeader { get; set; }
        public List<string> VideoDeviceList { get; set; }
        public Control Render { get; set; } 
        public IAMTVTuner Tuner { 
            get {
                return tuner;
            }
        }

        public List<string> AudioDeviceList { get; set; }

        public IAMCrossbar Crossbar {
            get {
                return crossbar;
            }
        }

        public DeInterlaceLayout DeInterlaceLayout {
            get {
                return deInterlaceLayout;
            }
            set {
                if (videoDevice != null)
                    DerenderDevice();
                deInterlaceLayout = value;
                if (videoDevice != null)
                {
                    RenderDevice();
                    Start();
                }
            }
        }

        public DeinterlaceList DeinterlaceLayoutList { get; set; }

        public IGraphBuilder GraphBuilder {
            get {
                return graphBuilder;
            }
        }

        public bool Mute {
            get {
                return mute;
            }
            set {
                if (mute != value) {

                    if (value)
                    {
                        SetVolume(0);
                        mute = value;
                    }
                    else
                    {
                        mute = value;
                        SetVolume((uint)volume);
                    }
                    
                }
            }
        }

        public bool ROT {
            get {
                return useROT;
            }
            set {
                if (videoDevice != null)
                    DerenderDevice();
                
                useROT = value;
                try{
                    if (rOT !=null)
                        rOT.Dispose();
                } catch{};
                rOT = null;
                if (useROT)                
                    rOT = new DsROTEntry(graphBuilder);
                if (videoDevice != null)
                {
                    RenderDevice();
                    Start();
                }
            }
        }

        public IAMTVAudio  TVAudio { get; set; }

        public IBaseFilter VideoDevice {
            get {
                return videoDevice;
            }
        }
        public IBaseFilter VideoRendererFilter { 
            get {
                return videoRendererFilter;
            }
        }

        public IVideoWindow VideoWindow {
            get {
                return videoWindow;
            }
        }

        public int Volume {
            get {
                return volume;
            }
            set {
                if (volume != value) {
                    if (value > 100)
                        value = 100;
                    if (value < 0)
                        value = 0;
                    volume = value;
                    SetVolume((uint) volume);
                }
            }
        }

        public int Zoom {
            get {
                return zoom;
            }
            set {
                if (zoom != value) {
                    zoom = value;
                    Resize();
                }
            }
        }

    }

    public class OverlayGraphics: System.Object {

        public OverlayGraphics(MyTuner myTunner) {
            this.myTunner = myTunner;
        }
        private Bitmap bitmap;

        private MyTuner myTunner;

        public void DrawText(string text) {
            DrawText(text, Color.White);



        }

        public void DrawText(string text, Color color) {
            var bmp = GetBitmap();
            var graphics = Graphics.FromImage(bmp);
            Font font = new System.Drawing.Font("Arial", 22.0f);
            SolidBrush brush = new System.Drawing.SolidBrush(color);

            int offsetY = (int)(myTunner.Zoom * MyTuner.ZoomStep / 2.0f * (myTunner.Render.Height * 1.0f / bmp.Height));
            
            //convert into capture screen coordinates
            int h;
            int w;
            myTunner.VideoWindow.get_Height(out h);
            offsetY = (int)(offsetY * bmp.Height / h);
            
            myTunner.VideoWindow.get_Width(out w);
            int offsetX = (int)(offsetY * 1 / myTunner.AspectRatio);
           // offsetX = (int)(offsetX );
            if (offsetX < 0)
                offsetX = 0;
            if (offsetY < 0)
                offsetY = 0;

            // put it on overlay
            graphics.DrawString(text, font, brush, offsetX, offsetY);
           
            PutOnOverlay(bmp);
            GC.Collect();

        }



        private Bitmap GetBitmap() {
            var w = myTunner.Render.Width;
            var h = myTunner.Render.Height;
            var s = myTunner.GetCaptureScreenSize();
            w = s.Width;
            h = s.Height;
            if (bitmap == null || (bitmap != null &&(bitmap.Height != h || bitmap.Width != w)))
            {
                bitmap = new Bitmap(w, h);
                
            }
            var bitmapGraphics = Graphics.FromImage(bitmap as Image);
            var brush = new System.Drawing.SolidBrush(Color.Black);
            bitmapGraphics.FillRectangle(brush, 0, 0, w, h);
            return bitmap;
        }

        private void PutOnOverlay(Bitmap bmp) {
            if (myTunner.VideoRendererFilter is IVMRMixerBitmap9)
            {
                
                var mixerBitmap = myTunner.VideoRendererFilter as IVMRMixerBitmap9;
                var param = new VMR9AlphaBitmap();
                var hr = mixerBitmap.GetAlphaBitmapParameters(out param);
                DsError.ThrowExceptionForHR(hr);

                var hdc = Graphics.FromHwnd(myTunner.Render.Handle).GetHdc();
                var memDC = Win32.CreateCompatibleDC(hdc);
                var hBitmap = bmp.GetHbitmap();
                try
                {
                    Win32.SelectObject(memDC, hBitmap);

                    var bw = bmp.Width;
                    var bh = bmp.Height;

                    var rw = myTunner.Render.Width;
                    var rh = myTunner.Render.Height;
                    var size = myTunner.GetCaptureScreenSize();
                    bw = rw;
                    bh = rh;

                    rw = size.Width;
                    rh = size.Height;



                    param.dwFlags = VMR9AlphaBitmapFlags.hDC | VMR9AlphaBitmapFlags.SrcColorKey | VMR9AlphaBitmapFlags.FilterMode;
                    param.hdc = memDC;
                    param.rSrc = new DsRect(0, 0, bw, bh);
                    param.rDest = new NormalizedRect(0f, 0f, (float)bw / rw, (float)bh / rh);
                    param.fAlpha = 1.0f;
                    //param.dwFilterMode = VMRMixerPrefs.BiLinearFiltering;
                    //param.clrSrcKey = ColorTranslator.ToWin32(Color.FromArgb(255, 0, 0, 0));//Color.White.ToArgb();
                    param.clrSrcKey = ColorTranslator.ToWin32(bmp.GetPixel(0, 0));
                    hr = mixerBitmap.SetAlphaBitmap(ref param);
                    DsError.ThrowExceptionForHR(hr);
                }
                finally
                {
                    Win32.DeleteDC(memDC);
                    Win32.ReleaseDC(myTunner.Render.Handle, hdc);
                    Win32.DeleteObject(hBitmap);
                }
                
            }
        }




    }

}

[ComImport, Guid("FA10746C-9B63-4B6C-BC49-FC300EA5F256")]
public class EnhanchedVideoRender {

}
