using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using log4net;
using Logitech.Config;
using Logitech.InputProviders;
using Logitech.InputProviders.Args;
using Logitech.Led;
using Logitech.LuaIntegration;
using Logitech.Settings;

namespace Logitech {
    internal class Program {
        private static volatile bool isRunning = true;
        private static volatile bool isLuaThreadRunning = false;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args) {
            Logger.Info("Starting LogiLed..");
            CopyInitialFiles();


            // KeyMapper.Validate();
            // return;

            List<IDisposable> disposables = new List<IDisposable>();

            InterceptKeys nativeKeyboardHook = new InterceptKeys();
            disposables.Add(nativeKeyboardHook);

            LogitechLedProvider ledProvider = new LogitechLedProvider();
            disposables.Add(ledProvider);

            SettingsService settingsService = new SettingsService(ledProvider);
            disposables.Add(settingsService);

            LogitechInputProvider logitechInputProvider = new LogitechInputProvider();
            disposables.Add(logitechInputProvider);

            try {
                nativeKeyboardHook.Start();
                ledProvider.Start();
                logitechInputProvider.Start();


                new Thread(() => {
                    isLuaThreadRunning = true;

                    ConcurrentQueue<InputEventArg> eventQueue = new ConcurrentQueue<InputEventArg>();

                    void HandleInputEvent(object _, InputEventArg arg) {
                        if (settingsService.IsProcessRelevant(Win32.GetForegroundProcessName()))
                            eventQueue.Enqueue(arg);
                    }

                    logitechInputProvider.OnInput += HandleInputEvent;
                    InterceptKeys.OnInput += HandleInputEvent;


                    while (isRunning) {
                        Thread.Sleep(1);
                        var processName = Win32.GetForegroundProcessName();
                        if (settingsService.IsProcessRelevant(processName)) {
                            while (eventQueue.TryDequeue(out var arg)) {
                                settingsService.OnEvent(processName, LuaEventType.Input, arg.Key, arg.Modifiers);
                            }

                            settingsService.OnEvent(processName, LuaEventType.Tick, null, null);
                        }
                    }

                    isLuaThreadRunning = false;
                }).Start();

                Application.Run();

                isRunning = false;
            }
            finally {
                while (isLuaThreadRunning) {
                    /* Waiting for lua thread to terminate before cleaning up dependencies */
                }

                foreach (var item in disposables) {
                    item.Dispose();
                }
            }

            Logger.Info("LogiLed terminated");
        }

        /// <summary>
        /// Copy the example files to appdata if no settings exists yet
        /// </summary>
        private static void CopyInitialFiles() {
            if (!File.Exists(GlobalSettings.SettingsFile)) {
                Logger.Info($"First run detected, copying example files to {GlobalSettings.SettingsFolder}");

                string appResFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources");
                foreach (string filename in Directory.GetFiles(appResFolder, "*.*", SearchOption.TopDirectoryOnly)) {
                    File.Copy(filename, filename.Replace(appResFolder, GlobalSettings.SettingsFolder), false);
                }
            }
        }
        /*
        }

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


        Documentation?
        - Gain/Lost focus events to LUA
        - Init method? No point?
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
}