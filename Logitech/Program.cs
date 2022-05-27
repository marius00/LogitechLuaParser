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

            var xxx = new WindowsInputDeviceStateAdapter();


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

                            if (KeyMapper.IsValidKeyCode(arg.Key)) {
                                Logger.Debug($"IsRealKey: {xxx.IsHardwareKeyDown(KeyMapper.TranslateToKeyCode(arg.Key))}");
                            }

                            eventQueue.Enqueue(arg);
                        }
                        else {
                            Logger.Debug($"Ignoring key event, process: {Win32.GetForegroundProcessName()}");
                        }
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
            string appResFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources");
            var xxxx = Path.GetExtension(@"C:\Users\Marius\AppData\Local\Logitech\Logitech Gaming Software\profiles\{F752A6C0-5D8A-4EB1-A25A-3CC275A060C1}.xml");
            if (!File.Exists(GlobalSettings.SettingsFile)) {
                Logger.Info($"First run detected, copying example files to {GlobalSettings.SettingsFolder}");

                foreach (string filename in Directory.GetFiles(appResFolder, "*.*", SearchOption.TopDirectoryOnly)) {
                    if (Path.GetExtension(filename) == ".json" || Path.GetExtension(filename) == ".lua") {
                        File.Copy(filename, filename.Replace(appResFolder, GlobalSettings.SettingsFolder), false);
                    }
                }
            }

            try {
                var logiFolder = Path.Combine(GlobalSettings.LocalAppdata, "Logitech", "Logitech Gaming Software", "profiles");
                if (Directory.Exists(logiFolder)) {
                    if (!File.Exists(Path.Combine(logiFolder, "{F752A6C0-5D8A-4EB1-A25A-3CC275A060C1}.xml"))) {
                        Logger.Info("Could not find Logitech Gaming Software profile installed, installing..");

                        var text = File.ReadAllText(Path.Combine(appResFolder, "{F752A6C0-5D8A-4EB1-A25A-3CC275A060C1}.xml"));
                        text = text.Replace("PLACEHOLDER.EXE", Assembly.GetEntryAssembly().Location);
                        File.WriteAllText(Path.Combine(logiFolder, "{F752A6C0-5D8A-4EB1-A25A-3CC275A060C1}.xml"), text);


                        try {
                            Logger.Info("Restarting Logitech Gaming Software to load configuration changes..");
                            Process p = Process.GetProcessesByName("LCore").FirstOrDefault();
                            if (p != null) {
                                var exe = p.MainModule.FileName;
                                p.Kill();
                                Process.Start(exe, "/minimized");
                            }
                        }
                        catch (Exception ex) {
                            Logger.Warn("Error restarting Logitech Gaming Software. A manual restart is required");
                            Logger.Warn(ex.Message, ex);
                        }
                    }
                }
                else {
                    Logger.Warn("Could not find the Logitech Gaming Software configuration directory");
                }
            }
            catch (IOException ex) {
                Logger.Warn("Error installing logitech profile");
                Logger.Warn(ex.Message, ex);
            }

            {
                Process p = Process.GetProcessesByName("LCore").FirstOrDefault();
                if (p != null) {
                    var exe = p.MainModule.FileName;
                    p.Kill();
                    Process.Start(exe, "/minimized");
                }
            }


        }
        /*
        
        Detect application lost focus:
            [to stop sending keys, stop all scripts]

        Application has focus?
            [to only trigger G-keys inside application]

        Profiles:
        %USERPROFILE%\AppData\Local\Logitech\Logitech Gaming Software\profiles
        Can iterate these, and if the target EXE name matches the one in a LUA/json config, add it automagically.


        Documentation?
        
        /*
         Desired functionality:
         * G-Macro support?
         * Detect modifiers (Alt, Shift, Ctrl)
         * Cancel script on alt+tab / tab out of game (- Gain/Lost focus events to LUA)
         *
         * Ability to run with "no application" ?
         * Auto add a profile for running the tool (Can auto detect when .exe files are missing from the .xml and add them..)
         *
         */
    }
}