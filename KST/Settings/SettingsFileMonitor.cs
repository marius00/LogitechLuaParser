using System;
using System.IO;
using System.Security.Permissions;
using log4net;
using KST.Config;

namespace KST.Settings {
    /// <summary>
    /// Monitors the settings folder for changes
    /// Will notify about changes to files, to reload LUA and JSON files
    /// </summary>
    internal class SettingsFileMonitor : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SettingsFileMonitor));
        private FileSystemWatcher _watcher = new FileSystemWatcher();
        public event FileSystemEventHandler OnModified;


        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public bool Start() {
            _watcher = new FileSystemWatcher();
            _watcher.Path = AppPaths.SettingsFolder;
            _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            _watcher.Filter = "*.*";
            _watcher.IncludeSubdirectories = false;
            _watcher.Changed +=_watcher_Changed;
            _watcher.EnableRaisingEvents = true;

            Logger.Info($"Monitoring \"{_watcher.Path}\" for file changes");
            return true;
        }

        private void _watcher_Changed(object sender, FileSystemEventArgs e) {
            Logger.Debug($"Changes detected to \"{e.FullPath}\"");
            OnModified?.Invoke(sender, e);
        }

        public void Dispose() {
            if (_watcher != null) {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
            }
        }
    }
}
