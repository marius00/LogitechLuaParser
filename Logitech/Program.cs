using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Web;
using log4net;
using Logitech.Config;
using Logitech.InputProviders;
using Logitech.InputProviders.Args;
using Logitech.Led;
using Logitech.LGS;
using Logitech.LuaIntegration;
using Logitech.Settings;
using Logitech.UI;

namespace Logitech {
    internal class Program {
        private static volatile bool _isRunning = true;
        private static volatile bool _isLuaThreadRunning = false;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));



        [STAThread]
        static void Main(string[] args) {
            Logger.Info("Starting LogiLed..");
            CopyInitialFiles();

            List<IDisposable> disposables = new List<IDisposable>();

            InterceptKeys nativeKeyboardHook = new InterceptKeys();
            disposables.Add(nativeKeyboardHook);

            LogitechLedProvider ledProvider = new LogitechLedProvider();
            disposables.Add(ledProvider);

            SettingsService settingsService = new SettingsService(ledProvider);
            disposables.Add(settingsService);

            LogitechInputProvider logitechInputProvider = new LogitechInputProvider();
            disposables.Add(logitechInputProvider);

            MouseInputProvider mouseInputProvider = new MouseInputProvider();
            disposables.Add(mouseInputProvider);

            try {
                nativeKeyboardHook.Start();
                ledProvider.Start();
                logitechInputProvider.Start();
                mouseInputProvider.Start();


                new Thread(() => {
                    _isLuaThreadRunning = true;

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
                    mouseInputProvider.OnInput += HandleInputEvent;


                    var lastProcessRelevant = false;
                    var lastProcess = "";
                    while (_isRunning) {
                        Thread.Sleep(1);
                        var processName = Win32.GetForegroundProcessName();
                        if (settingsService.IsProcessRelevant(processName)) {
                            if (!lastProcessRelevant) {
                                // Notify current process that it just gained focus
                                settingsService.OnEvent(processName, LuaEventType.Focus, "true", 0);

                            }

                            while (eventQueue.TryDequeue(out var arg)) {
                                var type = arg.Type == InputEventType.Down ? LuaEventType.KeyDown : LuaEventType.KeyUp;
                                settingsService.OnEvent(processName, type, arg.Key, arg.Modifiers);
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

                    _isLuaThreadRunning = false;
                }).Start();



                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainContainer());

                _isRunning = false;
            }
            finally {
                while (_isLuaThreadRunning) {
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

            LgsProfileUtil.Install();
            GHubProfileUtil.Install();
        }
        /*
        
        /*
         Desired functionality:
         * G-Macro support?
         * Mouseclick events (?)
         * Anon stats

         * Delay "on change" events for .lua files.
         *
         * KeyUp events, test if this can be used to detect clicking W with autorun on
         * Expand settings to keep "isminimized" and "isfirstrun"?
         * Toast informing of where it minimized?
         *
         */
    }
}