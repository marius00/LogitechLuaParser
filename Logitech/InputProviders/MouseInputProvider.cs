using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using log4net;
using Logitech.InputProviders.Args;

namespace Logitech.InputProviders {
    internal class MouseInputProvider : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MouseInputProvider));
        private volatile bool _isRunning = true;
        public event InputEventHandler OnInput;

        public void Start() {
            new Thread(() => {
                var hookProc = new HookProc(HookFunction);
                IntPtr hook;
                using (Process curProcess = Process.GetCurrentProcess()) {
                    using (ProcessModule curModule = curProcess.MainModule) {
                        hook = SetWindowsHookEx(HookType.WH_MOUSE_LL, hookProc, GetModuleHandle(curModule.ModuleName), 0); // (uint)AppDomain.GetCurrentThreadId());


                        while (_isRunning) {
                            var foundMessage = PeekMessage(out MSG msg, IntPtr.Zero, 0, 0, 0x0001);
                            if (foundMessage) {
                                TranslateMessage(ref msg);
                                DispatchMessage(ref msg);
                            }
                            else {
                                Thread.Sleep(1);
                            }

                            Thread.Sleep(0);
                        }
                    }
                }

                UnhookWindowsHookEx(hook);
            }).Start();
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct MSG {
            IntPtr hwnd;
            uint message;
            UIntPtr wParam;
            IntPtr lParam;
            int time;
            POINT pt;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

        [DllImport("user32.dll")]
        private static extern bool TranslateMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll")]
        private static extern IntPtr DispatchMessage([In] ref MSG lpmsg);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] in string lpModuleName);

        private IntPtr HookFunction(int code, IntPtr wParam, [In] MSLLHOOKSTRUCT lParam) {
            if (code < 0) {
                //you need to call CallNextHookEx without further processing
                //and return the value returned by CallNextHookEx
                return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }


            // https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-xbuttondown
            var wmMouse = (MouseMessage)wParam;
            switch (wmMouse) {
                case MouseMessage.WM_LBUTTONDOWN:
                    OnInput?.Invoke(this, new InputEventArg("LMB", 0, InputEventType.Down));
                    break;
                case MouseMessage.WM_LBUTTONUP:
                    OnInput?.Invoke(this, new InputEventArg("LMB", 0, InputEventType.Up));
                    break;
                case MouseMessage.WM_RBUTTONDOWN:
                    OnInput?.Invoke(this, new InputEventArg("RMB", 0, InputEventType.Down));
                    break;
                case MouseMessage.WM_RBUTTONUP:
                    OnInput?.Invoke(this, new InputEventArg("RMB", 0, InputEventType.Up));
                    break;
                case MouseMessage.WM_XBUTTONDOWN when (lParam.mouseData & 0x00010000) != 0:
                    OnInput?.Invoke(this, new InputEventArg("XMB1", 0, InputEventType.Down));
                    break;
                case MouseMessage.WM_XBUTTONDOWN when (lParam.mouseData & 0x00020000) != 0: {
                    OnInput?.Invoke(this, new InputEventArg("XMB2", 0, InputEventType.Down));
                    break;
                }
                case MouseMessage.WM_XBUTTONUP when (lParam.mouseData & 0x00010000) != 0:
                    OnInput?.Invoke(this, new InputEventArg("XMB1", 0, InputEventType.Up));
                    break;
                case MouseMessage.WM_XBUTTONUP when (lParam.mouseData & 0x00020000) != 0: {
                    OnInput?.Invoke(this, new InputEventArg("XMB2", 0, InputEventType.Up));
                    break;
                }
            }

            //return the value returned by CallNextHookEx
            return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }


        internal enum MouseMessage {
            WM_MOUSEMOVE = 0x0200,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_RBUTTONDBLCLK = 0x0206,
            WM_MBUTTONDOWN = 0x0207,
            WM_MBUTTONUP = 0x0208,
            WM_MBUTTONDBLCLK = 0x0209,

            WM_MOUSEWHEEL = 0x020A,
            WM_MOUSEHWHEEL = 0x020E,

            WM_NCMOUSEMOVE = 0x00A0,
            WM_NCLBUTTONDOWN = 0x00A1,
            WM_NCLBUTTONUP = 0x00A2,
            WM_NCLBUTTONDBLCLK = 0x00A3,
            WM_NCRBUTTONDOWN = 0x00A4,
            WM_NCRBUTTONUP = 0x00A5,
            WM_NCRBUTTONDBLCLK = 0x00A6,
            WM_NCMBUTTONDOWN = 0x00A7,
            WM_NCMBUTTONUP = 0x00A8,
            WM_NCMBUTTONDBLCLK = 0x00A9,


            WM_XBUTTONDOWN = 0x020B,
            WM_XBUTTONUP = 0x020C
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT {
            public POINT pt;
            public int mouseData; // be careful, this must be ints, not uints (was wrong before I changed it...). regards, cmew.
            public int flags;
            public int time;
            public UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT {
            public int X;
            public int Y;

            public POINT(int x, int y) {
                this.X = x;
                this.Y = y;
            }

            public static implicit operator System.Drawing.Point(POINT p) {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p) {
                return new POINT(p.X, p.Y);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);


        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, [In] MSLLHOOKSTRUCT lParam);

        delegate IntPtr HookProc(int code, IntPtr wParam, [In] MSLLHOOKSTRUCT lParam);

        public enum HookType : int {
            // For a complete list see https://www.pinvoke.net/default.aspx/Enums/HookType.html
            WH_KEYBOARD = 2,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        public void Dispose() {
            _isRunning = false;
        }
    }
}