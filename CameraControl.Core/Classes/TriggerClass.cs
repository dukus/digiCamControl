#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CameraControl.Devices;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

#endregion

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
            try
            {
                if (ServiceProvider.Settings.UseWebserver)
                {
                    WebServer.Start(ServiceProvider.Settings.WebserverPort);
                }
            }
            catch (Exception)
            {
                Log.Error("Unable to start webserver");
            }
        }

        public void Stop()
        {
            if (_hookID != IntPtr.Zero)
                UnhookWindowsHookEx(_hookID);
            WebServer.Stop();
        }

        public static void KeyDown(KeyEventArgs e)
        {
            foreach (var item in ServiceProvider.Settings.Actions)
            {
                if (item.Alt == _altpressed && item.Ctrl == _ctrlpressed && item.KeyEnum == e.Key)
                {
                    ServiceProvider.WindowsManager.ExecuteCommand(item.Name);
                    e.Handled = true;
                }
            }
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
            if (nCode >= 0 && (wParam == (IntPtr) WM_KEYDOWN || wParam == (IntPtr) WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (((Keys) vkCode) == Keys.Alt || ((Keys) vkCode) == Keys.RMenu || ((Keys) vkCode) == Keys.LMenu)
                    _altpressed = true;
                if (((Keys)vkCode) == Keys.Control || ((Keys)vkCode) == Keys.LControlKey || ((Keys)vkCode) == Keys.RControlKey || ((Keys)vkCode) == Keys.ControlKey)
                    _ctrlpressed = true;
                if (((Keys) vkCode) == Keys.RShiftKey || ((Keys) vkCode) == Keys.LShiftKey)
                    _shiftpressed = true;

                foreach (var item in ServiceProvider.Settings.Actions)
                {
                    if (!item.Global)
                        continue;
                    Key inputKey = KeyInterop.KeyFromVirtualKey(vkCode);
                    if (item.Alt == _altpressed && item.Ctrl == _ctrlpressed && item.KeyEnum == inputKey)
                         ServiceProvider.WindowsManager.ExecuteCommand(item.Name);
                }
            }
            if (nCode >= 0 && (wParam == (IntPtr) WM_KEYUP || wParam == (IntPtr) WM_SYSKEYUP))
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (((Keys) vkCode) == Keys.Alt || ((Keys) vkCode) == Keys.RMenu || ((Keys) vkCode) == Keys.LMenu)
                    _altpressed = false;
                if (((Keys)vkCode) == Keys.Control || ((Keys)vkCode) == Keys.LControlKey || ((Keys)vkCode) == Keys.RControlKey || ((Keys)vkCode) == Keys.ControlKey)
                    _ctrlpressed = false;
                if (((Keys) vkCode) == Keys.RShiftKey || ((Keys) vkCode) == Keys.LShiftKey)
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