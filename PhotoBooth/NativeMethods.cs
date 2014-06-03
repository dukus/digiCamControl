using System;
using System.Runtime.InteropServices;

namespace PhotoBooth
{
    internal static class NativeMethods
    {
        [DllImportAttribute("user32")]
        public static extern void keybd_event(byte bVirtualKey, byte bScanCode, int dwFlags, IntPtr dwExtraInfo);
    }
}
