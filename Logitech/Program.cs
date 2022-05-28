using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
using InputSimulatorStandard;
using log4net;
using Logitech.Config;
using Logitech.InputProviders;
using Logitech.InputProviders.Args;
using Logitech.Led;
using Logitech.LGS;
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

            int x = 8 + 4 + 2;
            int y = x & 16;

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
                        if (settingsService.IsProcessRelevant(Win32.GetForegroundProcessName())) {
                            eventQueue.Enqueue(arg);
                        }
                        else {
                            Logger.Debug($"Ignoring key event, process: {Win32.GetForegroundProcessName()}");
                        }
                    }

                    logitechInputProvider.OnInput += HandleInputEvent;
                    InterceptKeys.OnInput += HandleInputEvent;

                    var lastProcessRelevant = false;
                    var lastProcess = "";
                    while (isRunning) {
                        Thread.Sleep(1);
                        var processName = Win32.GetForegroundProcessName();
                        if (settingsService.IsProcessRelevant(processName)) {
                            if (!lastProcessRelevant) {
                                // Notify current process that it just gained focus
                                settingsService.OnEvent(processName, LuaEventType.Focus, "true", 0);

                            }

                            while (eventQueue.TryDequeue(out var arg)) {
                                settingsService.OnEvent(processName, LuaEventType.Input, arg.Key, arg.Modifiers);
                            }

                            settingsService.OnEvent(processName, LuaEventType.Tick, null, 0);
                            lastProcessRelevant = true;
                        }
                        else {
                            if (lastProcessRelevant) {
                                // Notify the last relevant process that it lost focus
                                settingsService.OnEvent(lastProcess, LuaEventType.Focus, "false", 0);
                            }
                            lastProcessRelevant = false;
                        }

                        lastProcess = processName;
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
            string appResFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources");
            
            if (!File.Exists(AppPaths.SettingsFile)) {
                Logger.Info($"First run detected, copying example files to {AppPaths.SettingsFolder}");

                foreach (string filename in Directory.GetFiles(appResFolder, "*.*", SearchOption.TopDirectoryOnly)) {
                    if (Path.GetExtension(filename) == ".json" || Path.GetExtension(filename) == ".lua") {
                        File.Copy(filename, filename.Replace(appResFolder, AppPaths.SettingsFolder), false);
                    }
                }
            }

            ProfileUtil.Install();
        }
        /*
        

        Profiles:
        %USERPROFILE%\AppData\Local\Logitech\Logitech Gaming Software\profiles
        Can iterate these, and if the target EXE name matches the one in a LUA/json config, add it automagically.


        Documentation?
        
        /*
         Desired functionality:
         * G-Macro support?
         * Mouseclick events (?)
         * Auto install globally
         * Anon stats
         * UI?
         *
         * Ability to run with "no application" ?
         * Auto add a profile for running the tool (Can auto detect when .exe files are missing from the .xml and add them..)
         *
         */
    }
}