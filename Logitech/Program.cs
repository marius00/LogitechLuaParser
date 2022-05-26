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
using Logitech.Led;
using Logitech.LuaIntegration;
using NLua;

namespace Logitech {
    internal class Program {
        private static volatile bool isRunning = true;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));


        static void Main(string[] args) {
            Logger.Info("Starting LogiLed..");
            InterceptKeys nativeKeyboardHook = new InterceptKeys();
            LogitechLedProvider ledProvider = new LogitechLedProvider();
            try {

                nativeKeyboardHook.Start();
                LogitechInputProvider logitechInputProvider = new LogitechInputProvider();
                logitechInputProvider.Start();
                ledProvider.Start();


                new Thread(() => {
                    var script = @"
                            SetBacklightColor('W', 0, 100, 0)
                            SetBacklightColor('S', 0, 100, 0)

                            spam = false
                            function OnEvent(event, arg, modifiers)
                                if event == LuaEventType.Tick then
                                    if spam then
                                        SendKeys('spammyspam')
                                    end
                                end
                                if event == LuaEventType.Input then
                                    if arg == 'G9' then
                                        OutputLogMessage('Yep its G9')
                                        SetBacklightColor(arg, 100, 0, 0)
                                        SendKeys('Haha!')
                                    end
                                    if arg == 'G8' then
                                        spam = not spam
                                        OutputLogMessage('Spam mode is: {0}', spam)
                                        if spam then
                                            SetBacklightColor(arg, 0, 100, 0)
                                        else
                                            SetBacklightColor(arg, 0, 0, 0)
                                        end
                                    end
                                end
                            end
                        ";

                    const string desiredProcess = "Notepad";

                    using (LuaEngine engine = new LuaEngine(ledProvider, desiredProcess, script)) {
                        ConcurrentQueue<InputEventArg> eventQueue = new ConcurrentQueue<InputEventArg>();

                        
                        logitechInputProvider.OnInput += (_, e) => {
                            var arg = e as InputEventArg;
                            if (Win32.GetForegroundProcessName().Equals(desiredProcess, StringComparison.CurrentCultureIgnoreCase))
                                eventQueue.Enqueue(arg);
                        };

                        InterceptKeys.OnInput += (_, e) => {
                            var arg = e as InputEventArg;
                            if (Win32.GetForegroundProcessName().Equals(desiredProcess, StringComparison.CurrentCultureIgnoreCase))
                                eventQueue.Enqueue(arg);
                        };


                        while (isRunning) {
                            Thread.Sleep(1);
                            if (Win32.GetForegroundProcessName().Equals(desiredProcess, StringComparison.CurrentCultureIgnoreCase)) {
                                while (eventQueue.TryDequeue(out var arg)) {
                                    engine.OnEvent(LuaEventType.Input, arg.Key, arg.Modifiers);
                                }

                                engine.OnEvent(LuaEventType.Tick, null, null);
                            }

                        }
                    }
                }).Start();

                Application.Run();

                isRunning = false;
                logitechInputProvider.Dispose();
            } finally {
                nativeKeyboardHook.Dispose();
                ledProvider.Dispose();
            }

            Logger.Info("LogiLed terminated");
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
