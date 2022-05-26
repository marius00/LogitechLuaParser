using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Logitech.InputProviders;
using NLua;

namespace Logitech {
    internal class Program {
        private static volatile bool isRunning = true;


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        static void Main(string[] args) {
            InterceptKeys nativeKeyboardHook = new InterceptKeys();
            try {

                nativeKeyboardHook.Start();
                LogitechInputProvider logitechInputProvider = new LogitechInputProvider();
                logitechInputProvider.Start();

                /*
                Process p = Process.GetProcessesByName("mIRC").FirstOrDefault();
                if (p != null) {
                    IntPtr h = p.MainWindowHandle;
                    SetForegroundWindow(h);
                    SendKeys.SendWait("kkkkk");
                }*/




                new Thread(() => {
                    const string Hardcoded = @"
                               import ('Logitech')
                               OutputLogMessage = LuaInterface.OutputLogMessage";
                    using (Lua lua = new Lua()) {
                        lua.State.Encoding = Encoding.UTF8;
                        lua.LoadCLRPackage();
                        lua.DoString(Hardcoded + @"

                            function OnEvent(event, arg)
                                OutputLogMessage('testytest: {0}', 5)
                            end
                        ");

                        var onEvent = lua["OnEvent"] as LuaFunction;
                        
                        while (isRunning) {
                            Thread.Sleep(1);
                           // onEvent.Call(null, null);

                        }
                    }
                }).Start();

                Application.Run();

                isRunning = false;
                logitechInputProvider.Dispose();
            } finally {
                nativeKeyboardHook.Dispose();
            }
        }
    }
    /*
     Send keys to another application:
     https://stackoverflow.com/questions/15292175/how-to-send-a-key-to-another-application#15292428

    Give focus to another application:

    Detect application lost focus:
        [to stop sending keys, stop all scripts]

    Application has focus?
        [to only trigger G-keys inside application]

    Profiles:
    %USERPROFILE%\AppData\Local\Logitech\Logitech Gaming Software\profiles
    Can iterate these, and if the target EXE name matches the one in a LUA/json config, add it automagically.
    
     *
     */


    /*
     Desired functionality:
     * Cancel script on alt+tab / tab out of game
     * Reset/restart script (LUA)
     * Detect G-keys
     * Detect regular keys
     * Detect modifiers (Alt, Shift, Ctrl)
     * Able to set colors on G910
     * Able to HOLD keys
     * Able to SPAM keys
     * G-Macro support?
     *
     * v2:
     * Able to hold LMB/RMB
     * Able to spam LMB/RMB
     *
     */
}
