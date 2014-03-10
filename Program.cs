using System;
using System.Collections.Generic;
using System.Linq;


namespace uTuner
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var c = new MyController();
            
            try
            {
                c.Start();
            }
            finally
            {
                c.Stop();
            }
                
            
            
        }
    }
}
