using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using log4net;
using Logitech.InputProviders;
using Logitech.InputProviders.Args;
using Logitech.LuaIntegration;
using NLua;

namespace Logitech {
    internal class Program {
        private static volatile bool isRunning = true;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));


        static void Main(string[] args) {
            Logger.Info("Starting LogiLed..");
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
                    var script = @"

                            function OnEvent(event, arg, modifiers)
                                if event == LuaEventType.Input then
                                    OutputLogMessage('testytest: {0}, {1}, {2}, {3}', 5, event, arg, modifiers)
                                end
                            end
                        ";

                    using (LuaEngine engine = new LuaEngine(script)) {
                        ConcurrentQueue<InputEventArg> eventQueue = new ConcurrentQueue<InputEventArg>();
                        
                        logitechInputProvider.OnInput += (_, e) => {
                            var arg = e as InputEventArg;
                            eventQueue.Enqueue(arg);
                        };

                        InterceptKeys.OnInput += (_, e) => {
                            var arg = e as InputEventArg;
                            eventQueue.Enqueue(arg);
                        };


                        while (isRunning) {
                            Thread.Sleep(1);
                            if (eventQueue.TryDequeue(out var arg)) {
                                engine.OnEvent(LuaEventType.Input, arg.Key, arg.Modifiers);
                            }
                            engine.OnEvent(LuaEventType.Tick, null, null);

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
