using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Core;
using Logitech.Config;
using Logitech.Led;
using Logitech.LuaIntegration;

namespace Logitech.Settings {
    /// <summary>
    /// Responsible for emitting events with up-to-date script info
    /// Will read settings.json upon start, and continue to monitor the settings file and lua files for changes.
    /// </summary>
    internal class SettingsService : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SettingsService));
        private readonly SettingsFileMonitor _fileMonitor = new SettingsFileMonitor();
        private readonly Dictionary<string, LuaEngine> _luaScriptFromFilename = new Dictionary<string, LuaEngine>();
        private readonly Dictionary<string, LuaEngine> _luaScriptFromProcess = new Dictionary<string, LuaEngine>();
        private readonly LogitechLedProvider _ledProvider;

        public SettingsService(LogitechLedProvider ledProvider) {
            this._ledProvider = ledProvider;
            _fileMonitor.Start();
            _fileMonitor.OnModified += _fileMonitor_OnModified;

            ParseSettingsJson(GlobalSettings.SettingsFile);
        }

        /// <summary>
        /// Parse the settings.json and create LUA engine entries for each process
        /// </summary>
        /// <param name="filename"></param>
        private void ParseSettingsJson(string filename) {
            var settings = SettingsReader.Load(filename);

            // Add any new entries
            foreach (var entry in settings) {
                if (!_luaScriptFromProcess.ContainsKey(entry.Process)) {
                    try {
                        string script = File.ReadAllText(Path.Combine(GlobalSettings.SettingsFolder, entry.Path));

                        // May have multiple references to a single script
                        if (!_luaScriptFromFilename.ContainsKey(entry.Path.ToLower())) {
                            var engine = new LuaEngine(_ledProvider, entry.Process, script);
                            _luaScriptFromFilename[entry.Path.ToLower()] = engine;
                        }
                        _luaScriptFromProcess[entry.Process] = _luaScriptFromFilename[entry.Path.ToLower()];
                        Logger.Debug($"Configured \"{entry.Path}\" for \"{entry.Process}\"");
                    }
                    catch (IOException ex) {
                        Logger.Warn(ex.Message, ex);
                        Logger.Warn($"Unable to read lua file {entry.Path}, script for {entry.Process} not loaded.");
                    }
                }
            }

            // Remove any entries that has been removed from settings.json
            var processes = _luaScriptFromProcess.Keys.ToArray(); // Immuteable list
            foreach (var process in processes) {
                if (!settings.Any(m => m.Process == process)) {
                    _luaScriptFromProcess.Remove(process);
                    Logger.Debug($"Removed lua script for {process}");
                    // TODO: Remove it from _luaScriptFromFilename (Make sure it's not referenced by another process first)
                    // TODO: Dispose of the LUA engine
                }
            }
        }

        /// <summary>
        /// Listens for file changes and re-parses LUA and the settings.json when needed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _fileMonitor_OnModified(object sender, System.IO.FileSystemEventArgs e) {
            if (e.Name == GlobalSettings.SettingsFileName) {
                ParseSettingsJson(e.FullPath);
            }
            else if (e.Name.EndsWith(".lua", StringComparison.CurrentCultureIgnoreCase)) {
                if (_luaScriptFromFilename.ContainsKey(e.Name.ToLower())) {
                    try {
                        string script = File.ReadAllText(e.FullPath);
                        if (_luaScriptFromFilename[e.Name.ToLower()].SetScript(script)) {
                            Logger.Info($"Updated LUA script {e.Name}");
                        }
                        else {
                            Logger.Warn($"Failed updating LUA script {e.Name}");
                        }
                    }
                    catch (IOException ex) {
                        Logger.Warn($"Error updating internal LUA for {e.Name}, {ex.Message}", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if there are any scripts configured for the given process name
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public bool IsProcessRelevant(string processName) {
            return _luaScriptFromProcess.Keys.Any(key => key.Equals(processName, StringComparison.CurrentCultureIgnoreCase));
        }

        public void OnEvent(string processName, LuaEventType eventType, string arg, string[] modifiers) {
            if (!_luaScriptFromProcess.ContainsKey(processName)) {
                Logger.Error($"Attempting to dispatch event to {processName}, which does not exist.");
                return;
            }

            _luaScriptFromProcess[processName].OnEvent(eventType, arg, modifiers);
        }

        public void Dispose() {
            _fileMonitor.Dispose();

            foreach (var script in _luaScriptFromFilename.Values) {
                script.Dispose();
            }

            _luaScriptFromFilename.Clear();
            _luaScriptFromProcess.Clear();
        }
    }
}