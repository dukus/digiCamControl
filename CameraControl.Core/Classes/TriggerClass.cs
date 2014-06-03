using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;

namespace CameraControl.Core.Classes
{
    public class TriggerClass
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYUP = 0x0105;

        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private static bool _altpressed = false;
        private static bool _ctrlpressed = false;
        private static bool _shiftpressed = false;

        public WebServer WebServer { get; set; }



        public TriggerClass()
        {
            WebServer = new WebServer();
        }

        public void Start()
        {
            _hookID = SetHook(_proc);
            if (ServiceProvider.Settings.UseWebserver)
            {
                WebServer.Start(ServiceProvider.Settings.WebserverPort);
            }
        }

        public void Stop()
        {
            if (_hookID != IntPtr.Zero)
                UnhookWindowsHookEx(_hookID);
            WebServer.Stop();
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                                        GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(
          int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(
          int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (((Keys)vkCode) == Keys.Alt || ((Keys)vkCode) == Keys.RMenu || ((Keys)vkCode) == Keys.LMenu)
                    _altpressed = true;
                if (((Keys)vkCode) == Keys.Control)
                    _ctrlpressed = true;
                if (((Keys)vkCode) == Keys.RShiftKey || ((Keys)vkCode) == Keys.LShiftKey)
                    _shiftpressed = true;

                foreach (var item in ServiceProvider.Settings.Actions)
                {
                    Key inputKey = KeyInterop.KeyFromVirtualKey(vkCode);
                     if (item.Alt == _altpressed && item.Ctrl == _ctrlpressed && item.KeyEnum == inputKey)
                        ServiceProvider.WindowsManager.ExecuteCommand(item.Name);
                }
            }
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP))
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (((Keys)vkCode) == Keys.Alt || ((Keys)vkCode) == Keys.RMenu || ((Keys)vkCode) == Keys.LMenu)
                    _altpressed = false;
                if (((Keys)vkCode) == Keys.Control)
                    _ctrlpressed = false;
                if (((Keys)vkCode) == Keys.RShiftKey || ((Keys)vkCode) == Keys.LShiftKey)
                    _shiftpressed = false;
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
                                                      LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
                                                    IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

    }
}
