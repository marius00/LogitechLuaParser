using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using InputSimulatorStandard.Native;
using KST.InputProviders.Args;

namespace KST.InputProviders {
    /// <summary>
    /// Keyboard hook for global key events
    /// Regular keys only (No support for Logitech G-keys etc)
    /// </summary>
    class InterceptKeys : IDisposable {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104; // Alt https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-syskeydown
        private const int WM_KEYUP = 0x0101;

        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookId = IntPtr.Zero;

        public static event InputEventHandler OnInput;

        public void Start() {
            _hookId = SetHook(_proc);
        }

        public void Dispose() {
            UnhookWindowsHookEx(_hookId);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc) {
            using (Process curProcess = Process.GetCurrentProcess()) {
                using (ProcessModule curModule = curProcess.MainModule) {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern short GetAsyncKeyState(ushort virtualKeyCode);

        private static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)) {
                int vkCode = Marshal.ReadInt32(lParam);

                ushort state = 0;
                if ((GetAsyncKeyState((ushort)VirtualKeyCode.LSHIFT) & 0x8000) != 0) {
                    state += (ushort)InputModifierState.Shift;
                }
                if ((GetAsyncKeyState((ushort)VirtualKeyCode.LCONTROL) & 0x8000) != 0) {
                    state += (ushort)InputModifierState.Ctrl;
                }
                if (wParam == (IntPtr)WM_SYSKEYDOWN) {
                    state += (ushort)InputModifierState.Alt;
                }

                OnInput?.Invoke(null, new InputEventArg(((Keys)vkCode).ToString(), state, InputEventType.Down));
            }
            else if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP) {
                int vkCode = Marshal.ReadInt32(lParam);
                OnInput?.Invoke(null, new InputEventArg(((Keys)vkCode).ToString(), 0, InputEventType.Up));
            } else {
                // Control.ModifierKeys
                // int vkCode = Marshal.ReadInt32(lParam);
                // Console.WriteLine("???" + (Keys)vkCode);
            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}