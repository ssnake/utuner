using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace uTuner
{
    public class MyController
    {

        public MyController() {
            Data = new MyData();
            View = new MyView(this);
            ScanView = new ScanView(this);
            
        }

        public void Start() {

            Data.Load();
            View.Update();
            View.Start();
            Application.Run(View.Form);
            
        }

        public void Stop() {
            View.Stop();
            Data.Save();
            
        }

        public MyData Data { get; set; }
        public ScanView ScanView { get; set; }

        public MyView View { get; set; }

    }
}
