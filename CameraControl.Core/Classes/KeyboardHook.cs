﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

// source https://github.com/factormystic/FMUtils.KeyboardHook
namespace CameraControl.Core.Classes
{
    public class KeyboardHook
    {
        #region PInvoke
        private enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        public struct KBDLLHOOKSTRUCT
        {
            public UInt32 vkCode;
            public UInt32 scanCode;
            public UInt32 flags;
            public UInt32 time;
            public IntPtr extraInfo;
        }

        private int WM_KEYDOWN = 0x100;
        private int WM_KEYUP = 0x101;

        private int WM_SYSKEYDOWN = 0x0104;
        private int WM_SYSKEYUP = 0x105;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        /// Installs an application-defined hook procedure into a hook chain.
        /// </summary>
        /// <param name="idHook">The type of hook procedure to be installed.</param>
        /// <param name="lpfn">Reference to the hook callback method.</param>
        /// <param name="hMod">A handle to the DLL containing the hook procedure pointed to by the lpfn parameter. The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread created by the current process and if the hook procedure is within the code associated with the current process.</param>
        /// <param name="dwThreadId">The identifier of the thread with which the hook procedure is to be associated. If this parameter is zero, the hook procedure is associated with all existing threads running in the same desktop as the calling thread.</param>
        /// <returns>If the function succeeds, the return value is the handle to the hook procedure. If the function fails, the return value is NULL. To get extended error information, call GetLastError.</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(HookType idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        /// <summary>
        /// Removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
        /// </summary>
        /// <param name="hhk">A handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to SetWindowsHookEx.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr hhk);

        /// <summary>
        /// Passes the hook information to the next hook procedure in the current hook chain. A hook procedure can call this function either before or after processing the hook information.
        /// </summary>
        /// <param name="hhk">This parameter is ignored.</param>
        /// <param name="nCode">The hook code passed to the current hook procedure. The next hook procedure uses this code to determine how to process the hook information.</param>
        /// <param name="wParam">The wParam value passed to the current hook procedure. The meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
        /// <param name="lParam">The lParam value passed to the current hook procedure. The meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
        /// <returns>This value is returned by the next hook procedure in the chain. The current hook procedure must also return this value. The meaning of the return value depends on the hook type. For more information, see the descriptions of the individual hook procedures.</returns>
        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        /// <summary>
        /// An application-defined or library-defined callback function used with the SetWindowsHookEx function. The system calls this function whenever an application calls the GetMessage or PeekMessage function and there is a keyboard message (WM_KEYUP or WM_KEYDOWN) to be processed.
        /// </summary>
        /// <param name="code">A code the hook procedure uses to determine how to process the message. If code is less than zero, the hook procedure must pass the message to the CallNextHookEx function without further processing and should return the value returned by CallNextHookEx.</param>
        /// <param name="wParam">The virtual-key code of the key that generated the keystroke message.</param>
        /// <param name="lParam">The repeat count, scan code, extended-key flag, context code, previous key-state flag, and transition-state flag. For more information about the lParam parameter, see Keystroke Message Flags.</param>
        /// <returns>If code is less than zero, the hook procedure must return the value returned by CallNextHookEx. If code is greater than or equal to zero, and the hook procedure did not process the message, it is highly recommended that you call CallNextHookEx and return the value it returns; otherwise bad stuff.</returns>
        private delegate int HookProc(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);
        #endregion

        public string Name { get; private set; }

        /// <summary>
        /// When true, suspends firing of the hook notification events
        /// </summary>
        public bool isPaused
        {
            get
            {
                return _ispaused;
            }
            set
            {
                if (value != _ispaused && value == true)
                    StopHook();

                if (value != _ispaused && value == false)
                    StartHook();

                _ispaused = value;
            }
        }
        bool _ispaused = false;

        public delegate void KeyDownEventDelegate(KeyboardHookEventArgs e);
        public KeyDownEventDelegate KeyDownEvent = delegate { };

        public delegate void KeyUpEventDelegate(KeyboardHookEventArgs e);
        public KeyUpEventDelegate KeyUpEvent = delegate { };

        HookProc _hookproc;
        IntPtr _hhook;


        public KeyboardHook(string name)
        {
            Name = name;
            StartHook();
        }

        private void StartHook()
        {
            Trace.WriteLine(string.Format("Starting hook '{0}'...", Name), string.Format("Hook.StartHook [{0}]", Thread.CurrentThread.Name));

            _hookproc = new HookProc(HookCallback);
            _hhook = SetWindowsHookEx(HookType.WH_KEYBOARD_LL, _hookproc, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
            if (_hhook == null || _hhook == IntPtr.Zero)
            {
                Win32Exception LastError = new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private void StopHook()
        {
            Trace.WriteLine(string.Format("Stopping hook '{0}'...", Name), string.Format("Hook.StartHook [{0}]", Thread.CurrentThread.Name));

            UnhookWindowsHookEx(_hhook);
        }

        private int HookCallback(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            int result = 0;

            try
            {
                if (!isPaused && code >= 0)
                {
                    if (wParam.ToInt32() == WM_SYSKEYDOWN || wParam.ToInt32() == WM_KEYDOWN)
                        KeyDownEvent(new KeyboardHookEventArgs(lParam));

                    if (wParam.ToInt32() == WM_SYSKEYUP || wParam.ToInt32() == WM_KEYUP)
                        KeyUpEvent(new KeyboardHookEventArgs(lParam));
                }
            }
            finally
            {
                result = CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);
            }

            return result;
        }

        ~KeyboardHook()
        {
            StopHook();
        }
    }

    public class KeyboardHookEventArgs
    {
        #region PInvoke
        [DllImport("user32.dll")]
        static extern short GetKeyState(VirtualKeyStates nVirtKey);

        private enum VirtualKeyStates : int
        {
            VK_LWIN = 0x5B,
            VK_RWIN = 0x5C,
            VK_LSHIFT = 0xA0,
            VK_RSHIFT = 0xA1,
            VK_LCONTROL = 0xA2,
            VK_RCONTROL = 0xA3,
            VK_LALT = 0xA4, //aka VK_LMENU
            VK_RALT = 0xA5, //aka VK_RMENU
        }

        private const int KEY_PRESSED = 0x8000;
        #endregion

        public Keys Key { get; private set; }

        public bool isAltPressed { get { return isLAltPressed || isRAltPressed; } }
        public bool isLAltPressed { get; private set; }
        public bool isRAltPressed { get; private set; }

        public bool isCtrlPressed { get { return isLCtrlPressed || isRCtrlPressed; } }
        public bool isLCtrlPressed { get; private set; }
        public bool isRCtrlPressed { get; private set; }

        public bool isShiftPressed { get { return isLShiftPressed || isRShiftPressed; } }
        public bool isLShiftPressed { get; private set; }
        public bool isRShiftPressed { get; private set; }

        public bool isWinPressed { get { return isLWinPressed || isRWinPressed; } }
        public bool isLWinPressed { get; private set; }
        public bool isRWinPressed { get; private set; }

        internal KeyboardHookEventArgs(KeyboardHook.KBDLLHOOKSTRUCT lParam)
        {
            this.Key = (Keys)lParam.vkCode;

            //Control.ModifierKeys doesn't capture alt/win, and doesn't have r/l granularity
            this.isLAltPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LALT) & KEY_PRESSED) || this.Key == Keys.LMenu;
            this.isRAltPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_RALT) & KEY_PRESSED) || this.Key == Keys.RMenu;

            this.isLCtrlPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LCONTROL) & KEY_PRESSED) || this.Key == Keys.LControlKey;
            this.isRCtrlPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_RCONTROL) & KEY_PRESSED) || this.Key == Keys.RControlKey;

            this.isLShiftPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LSHIFT) & KEY_PRESSED) || this.Key == Keys.LShiftKey;
            this.isRShiftPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_RSHIFT) & KEY_PRESSED) || this.Key == Keys.RShiftKey;

            this.isLWinPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LWIN) & KEY_PRESSED) || this.Key == Keys.LWin;
            this.isRWinPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_RWIN) & KEY_PRESSED) || this.Key == Keys.RWin;

            if (new[] { Keys.LMenu, Keys.RMenu, Keys.LControlKey, Keys.RControlKey, Keys.LShiftKey, Keys.RShiftKey, Keys.LWin, Keys.RWin }.Contains(this.Key))
                this.Key = Keys.None;
        }

        public override string ToString()
        {
            return string.Format("Key={0}; Win={1}; Alt={2}; Ctrl={3}; Shift={4}", new object[] { Key, isWinPressed, isAltPressed, isCtrlPressed, isShiftPressed });
        }
    }
}
