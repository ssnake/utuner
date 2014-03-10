using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using System.Windows.Forms;
using System.Drawing;
using DirectShowLib;

namespace uTuner
{

    [ComImport, Guid("45FA9781-E904-11D6-A2FE-0080AD0B2EA7")]
    //[ComImport, Guid("51b4abf3-748f-4e3b-a276-c828330e926a")]
    public class DeinterlaceAlparysoftFilter: System.Object {

    }

    [ComImport, Guid("463D645D-48F7-11D4-8464-0008C782A257")]
    //[ComImport, Guid("463D645D-48F7-11D4-8464-0008C782A257")]
    
    public class DeinterlaceDScalerFilter: System.Object {

    }

    public class DeInterlaceLayout: System.Object {

        public DeInterlaceLayout(MyTuner tuner) {
            this.tuner = tuner;

        }

        protected MyTuner tuner;
        protected IBaseFilter filter;

        public void Clear() {
            if (filter != null)
            {

                tuner.RemoveFilter(ref filter);
                filter = null;
            }
        }

        public virtual void Init() {
        }

        public virtual bool IsSupported() {
            return false;
        }

        public IBaseFilter Filter { 
            get {
                return filter;
            }
        }
        public string Name { get; set; }

    }

    public class DeInterlaceDscalerLayout: DeInterlaceLayout {

        public DeInterlaceDscalerLayout(MyTuner tuner):base(tuner) {
            
            Name = "Dscaler Deinterlace Filter";
           
            
        }
        public override void Init() {
            filter = (IBaseFilter)new DeinterlaceDScalerFilter();
            tuner.GraphBuilder.AddFilter(this.Filter, Name);
            
        }

        public override bool IsSupported() {
            try
            {
                var o = Activator.CreateInstance(typeof(DeinterlaceDScalerFilter));
                return o != null;
            }
            catch
            {
                return false;

            }
        }

    }

    public class DeInterlaceVMR9Layout: DeInterlaceLayout {

        public DeInterlaceVMR9Layout(MyTuner tuner):base(tuner) {
            Name = "DeInterlaceVMR9";
            Init();
        }



        private IVMRDeinterlaceControl9 deinterlaceControl = null;

        private Guid[] deinterlaceModes;
        private VMR9VideoDesc videoDesc;

        private void GetNumberOfDeinterlaceModes()
        {
          int hr = 0;
          int numModes = 0;

          videoDesc = GetVideoDesc9();

          hr = deinterlaceControl.GetNumberOfDeinterlaceModes(ref videoDesc, ref numModes, null);
          DsError.ThrowExceptionForHR(hr);

          deinterlaceModes = new Guid[numModes];

          hr = deinterlaceControl.GetNumberOfDeinterlaceModes(ref videoDesc, ref numModes, deinterlaceModes);
          DsError.ThrowExceptionForHR(hr);

          
        }
        public override void Init() {
            deinterlaceControl = (IVMRDeinterlaceControl9) tuner.VideoRendererFilter;
            GetNumberOfDeinterlaceModes();
            // Try to activate the second best mode (assuming at least 2 are supported)
            var hr = deinterlaceControl.SetDeinterlaceMode(0, deinterlaceModes[1]);
            DsError.ThrowExceptionForHR(hr);
        }

        private VMR9SampleFormat ConvertInterlaceFlags(AMInterlace dwInterlaceFlags)
        {
          if ((dwInterlaceFlags & AMInterlace.IsInterlaced) != 0)
          {
            if ((dwInterlaceFlags & AMInterlace.OneFieldPerSample) != 0)
            {
              if ((dwInterlaceFlags & AMInterlace.Field1First) != 0)
                return VMR9SampleFormat.FieldSingleEven;
              else
                return VMR9SampleFormat.FieldSingleOdd;
            }
            else
            {
              if ((dwInterlaceFlags & AMInterlace.Field1First) != 0)
                return VMR9SampleFormat.FieldInterleavedEvenFirst;
              else
                return VMR9SampleFormat.FieldInterleavedOddFirst;
            }
          }
          else
            return VMR9SampleFormat.ProgressiveFrame;
        }

        // this method is an implementation of the procedure describe in this page :
        // http://msdn.microsoft.com/library/en-us/directshow/htm/settingdeinterlacepreferences.asp?frame=true
        private VMR9VideoDesc GetVideoDesc9()
        {
          int hr = 0;
          AMMediaType mediaType = new AMMediaType();
          VMR9VideoDesc vDesc = new VMR9VideoDesc();
          vDesc.dwSize = Marshal.SizeOf(typeof(VMR9VideoDesc));

          IPin pinIn = DsFindPin.ByDirection(tuner.VideoRendererFilter, PinDirection.Input, 0);

          hr = pinIn.ConnectionMediaType(mediaType);
          DsError.ThrowExceptionForHR(hr);

          Marshal.ReleaseComObject(pinIn);

          if (mediaType.formatType == FormatType.VideoInfo2)
          {
              VideoInfoHeader2 videoHeader = (VideoInfoHeader2)Marshal.PtrToStructure(mediaType.formatPtr, typeof(VideoInfoHeader2));
              if ((videoHeader.InterlaceFlags & AMInterlace.IsInterlaced) != 0)
              {
                  vDesc.dwSampleWidth = videoHeader.BmiHeader.Width;
                  vDesc.dwSampleHeight = videoHeader.BmiHeader.Height;
                  vDesc.SampleFormat = ConvertInterlaceFlags(videoHeader.InterlaceFlags);
                  vDesc.dwFourCC = videoHeader.BmiHeader.Compression;

                  switch (videoHeader.AvgTimePerFrame)
                  {
                      case 166833:
                          {
                              vDesc.InputSampleFreq.dwNumerator = 60000;
                              vDesc.InputSampleFreq.dwDenominator = 1001;
                              break;
                          }
                      case 333667:
                          {
                              vDesc.InputSampleFreq.dwNumerator = 30000;
                              vDesc.InputSampleFreq.dwDenominator = 1001;
                              break;
                          }
                      case 333666: // this value is not define in the paper but is returned by testme.iso
                          {
                              vDesc.InputSampleFreq.dwNumerator = 30000;
                              vDesc.InputSampleFreq.dwDenominator = 1001;
                              break;
                          }
                      case 417188:
                          {
                              vDesc.InputSampleFreq.dwNumerator = 24000;
                              vDesc.InputSampleFreq.dwDenominator = 1001;
                              break;
                          }
                      case 200000:
                          {
                              vDesc.InputSampleFreq.dwNumerator = 50;
                              vDesc.InputSampleFreq.dwDenominator = 1;
                              break;
                          }
                      case 400000:
                          {
                              vDesc.InputSampleFreq.dwNumerator = 25;
                              vDesc.InputSampleFreq.dwDenominator = 1;
                              break;
                          }
                      case 416667:
                          {
                              vDesc.InputSampleFreq.dwNumerator = 24;
                              vDesc.InputSampleFreq.dwDenominator = 1;
                              break;
                          }
                      default:
                          {
                              throw new ApplicationException("Unknown AvgTimePerFrame : " + videoHeader.AvgTimePerFrame);
                          }
                  }

                  // Video is interleaved
                  vDesc.OutputFrameFreq.dwNumerator = vDesc.InputSampleFreq.dwNumerator * 2;
                  vDesc.OutputFrameFreq.dwDenominator = vDesc.InputSampleFreq.dwDenominator;
              }
              else
                  throw new ApplicationException("Only interlaced formats");
          }
          else
          {
              var s = "";
              if (mediaType.formatType == FormatType.VideoInfo)
                  s = "VideInfo";
              if (mediaType.formatType == FormatType.AnalogVideo)
                  s = "AnalogVideo)";
              throw new ApplicationException("Only VIDEOINFOHEADER2 formats can be interlaced. Now it has "+s);
          }
          DsUtils.FreeAMMediaType(mediaType);
          return vDesc;
        }

    }

    public class DeInterlaceAlparyLayout: DeInterlaceLayout {

        public DeInterlaceAlparyLayout(MyTuner tuner):base(tuner) {
            
            Name = "Alpary Deinterlace Filter";
            
            
        }

        public override void Init() {
            filter = (IBaseFilter)new DeinterlaceAlparysoftFilter();
            tuner.GraphBuilder.AddFilter(this.Filter, Name);
        }

        public override bool IsSupported() {
            try
            {
                var o = Activator.CreateInstance(typeof(DeinterlaceAlparysoftFilter));
                return o != null;
            }
            catch (COMException e)
            {
                return false;

            }
        }

    }

    public class DeinterlaceList: List<DeInterlaceLayout> {

        public DeInterlaceLayout Find(string name) {
            foreach (DeInterlaceLayout d in this)
            {
                if (d.Name.CompareTo(name) == 0)
                    return d;

                
            }
            return null;

        }

    }

    [ComImport, Guid("0B390488-D80F-4A68-8408-48DC199F0E97")]
    public class DeinterlaceFFDShowFilter: System.Object {

    }

    public class DeInterlaceFFDShowLayout: DeInterlaceLayout {

        public DeInterlaceFFDShowLayout(MyTuner tuner):base(tuner) {
            
            Name = "FFDShow Raw Video Filter";
            
            
        }

        public override void Init() {
            filter = (IBaseFilter)new DeinterlaceFFDShowFilter();
            tuner.GraphBuilder.AddFilter(this.Filter, Name);
        }

        public override bool IsSupported() {
            try
            {
                var o = Activator.CreateInstance(typeof(DeinterlaceFFDShowFilter));
                return o != null;
            }
            catch (COMException e)
            {
                return false;

            }
        }

    }

}
