using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Logitech.Config;

namespace Logitech.LGS {
    /// <summary>
    /// Responsible for adding profiles to Logitech Gaming Software
    /// </summary>
    internal class ProfileUtil {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ProfileUtil));

        public static void RestartLgs() {
            try {
                Process p = Process.GetProcessesByName("LCore").FirstOrDefault();
                if (p == null) return;
                Logger.Info("Restarting Logitech Gaming Software to load configuration changes..");

                var exe = p.MainModule.FileName;
                p.Kill();
                Process.Start(exe, "/minimized");
            } catch (Exception ex) {
                Logger.Warn("Error restarting Logitech Gaming Software. A manual restart is required");
                Logger.Warn(ex.Message, ex);
            }
        }

        public static void Initialize() {
            string resourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources");

            try {
                if (!File.Exists(LogitechPaths.Profile)) {
                    Logger.Info("Could not find Logitech Gaming Software profile installed, installing..");

                    var text = File.ReadAllText(Path.Combine(resourcePath, LogitechPaths.ProfileFilename));
                    text = text.Replace("PLACEHOLDER.EXE", Assembly.GetEntryAssembly().Location);
                    File.WriteAllText(LogitechPaths.Profile, text);

                    RestartLgs();
                }

            } catch (IOException ex) {
                Logger.Warn("Error installing logitech profile");
                Logger.Warn(ex.Message, ex);
            }
        }
    }
}
