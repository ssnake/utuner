using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace uTuner
{
    struct MENUITEMINFO
    {
        public int cbSize;
        public uint fMask;
        public uint fType;
        public uint fState;
        public uint wID;
        public IntPtr hSubMenu;
        public IntPtr hbmpChecked;
        public IntPtr hbmpUnchecked;
        public IntPtr dwItemData;
        public IntPtr dwTypeData;
        public uint cch;
        public IntPtr hbmpItem;
    }
    class Win32
    {
        public const uint MIIM_FTYPE = 0x100;
        public const uint MFT_MENUBARBREAK = 0x20;
        public const uint MFT_MENUBREAK = 0x40;
        
        public const uint ES_CONTINUOUS = 0x80000000;
        public const uint ES_DISPLAY_REQUIRED = 0x00000002;


        //gdi
        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        public static extern IntPtr DeleteDC(IntPtr hDc);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        public static extern IntPtr DeleteObject(IntPtr hDc);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        public static extern bool BitBlt(IntPtr hdcDest, int xDest,
            int yDest, int wDest, int hDest, IntPtr hdcSource,
            int xSrc, int ySrc, int RasterOp);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc,
            int nWidth, int nHeight);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        //user
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", EntryPoint = "GetDC")]
        public static extern IntPtr GetDC(IntPtr ptr);

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int abc);

        [DllImport("user32.dll", EntryPoint = "GetWindowDC")]
        public static extern IntPtr GetWindowDC(Int32 ptr);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetMenuItemInfo(IntPtr hMenu, uint uItem, bool fByPosition, ref MENUITEMINFO lpmii);

        [DllImport("user32.dll", EntryPoint = "ShowCursor")]
        public static extern IntPtr ShowCursor(bool show);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint SetThreadExecutionState([In] uint esFlags);
        [DllImport("shell32.dll")]
        public static extern int ShellExecute(int hWnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }
        public static TimeSpan GetLastInput()
        {
            var plii = new LASTINPUTINFO();
            plii.cbSize = (uint)Marshal.SizeOf(plii);

            if (GetLastInputInfo(ref plii))
                return TimeSpan.FromMilliseconds(Environment.TickCount - plii.dwTime);
            else
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}
