using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using KST.Config;
using KST.InputProviders;
using KST.InputProviders.Args;
using KST.Led;
using KST.LGS;
using KST.LuaIntegration;
using KST.Settings;
using KST.UI;
using log4net;

namespace KST {
    internal class Program {
        private static volatile bool _isRunning = true;
        private static volatile bool _isLuaThreadRunning = false;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));



        [STAThread]
        static void Main(string[] args) {
            Logger.Info("Starting KST..");
            Initialize();

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
            if (!SettingsReader.Load(AppPaths.SettingsFile).NoAnonymousUsageStats) {
                disposables.Add(new UsageStatisticsReporter());
            }

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

            Logger.Info("KST terminated");
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

        private static void Initialize() {
            Logger.Info("Source code available at https://github.com/marius00/KeyboardScriptingTool");

            CopyInitialFiles();
            InitializeUuid();

            var settings = SettingsReader.Load(AppPaths.SettingsFile);
            if (!settings.NoAnonymousUsageStats) {
                UsageStatisticsReporter.Uuid = settings.Uuid;
                UsageStatisticsReporter.UrlStats = "https://webstats.evilsoft.net/report/kst";
                UsageStatisticsReporter.ReportUsageAsync();
            }
        }

        private static void InitializeUuid() {
            var settings = SettingsReader.Load(AppPaths.SettingsFile);

            if (string.IsNullOrEmpty(settings.Uuid)) { 
                settings.Uuid = Guid.NewGuid().ToString();
                SettingsReader.Persist(AppPaths.SettingsFile, settings);
            }
        }
    }
}