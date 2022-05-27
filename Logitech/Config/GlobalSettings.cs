using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitech.Config {
    /// <summary>
    /// Hardcoded paths to settings/logging/etc
    /// </summary>
    internal static class GlobalSettings {
        private static string LocalAppdata {
            get {
                string appdata = System.Environment.GetEnvironmentVariable("LocalAppData");
                if (string.IsNullOrEmpty(appdata))
                    return Path.Combine(System.Environment.GetEnvironmentVariable("AppData"), "..", "local");
                else
                    return appdata;
            }
        }

#if DEBUG
        private const string _settingsFolder = "settings-debug";
#else
        private const string _settingsFolder = "settings";
#endif

        public static string SettingsFile => Path.Combine(SettingsFolder, SettingsFileName);
        public static string SettingsFileName => "settings.json";

        public static string SettingsFolder {
            get {
                string path = Path.Combine(CoreFolder, _settingsFolder);
                Directory.CreateDirectory(path);

                return path;
            }
        }

        private static string CoreFolder {
            get {
                string path = Path.Combine(LocalAppdata, "EvilSoft", "LogiLed");
                Directory.CreateDirectory(path);
                
                return path;
            }
        }
    }
}
