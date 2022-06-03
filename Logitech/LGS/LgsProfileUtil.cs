using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using Logitech.Config;
using Newtonsoft.Json.Linq;

namespace Logitech.LGS {
    /// <summary>
    /// Responsible for adding profiles to Logitech Gaming Software
    /// </summary>
    internal class LgsProfileUtil {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LgsProfileUtil));

        public static void RestartLgs() {
            try {
                Process p = Process.GetProcessesByName("LCore").FirstOrDefault();
                if (p == null) return;
                Logger.Info("Restarting Logitech Gaming Software to load configuration changes..");

                var exe = p.MainModule.FileName;
                p.Kill();
                Process.Start(exe, "/minimized");
            }
            catch (Exception ex) {
                Logger.Warn("Error restarting Logitech Gaming Software. A manual restart is required");
                Logger.Warn(ex.Message, ex);
            }
        }

        public static void Install() {
            if (!File.Exists(LogitechPaths.DefaultProfile)) {
                Logger.Warn("Could not find Logitech Gaming Software default profile installed..");
                return;
            }

            try {
                var assemblyPath = Assembly.GetEntryAssembly().Location.Replace("\\\\", "\\").ToUpperInvariant();
                var text = File.ReadAllText(LogitechPaths.DefaultProfile);
                if (!text.Contains(assemblyPath)) {
                    Logger.Info("Installing LogiLed into the Logitech Gaming Software default profile");
                    text = text.Replace("</description>", "</description>\n    " + $"<target path=\"{assemblyPath}\"/>");

                    // Backup existing config and write in the software location
                    File.Copy(LogitechPaths.DefaultProfile, Path.Combine(AppPaths.CoreFolder, LogitechPaths.DefaultProfileFilename));
                    File.WriteAllText(LogitechPaths.DefaultProfile, text);
                    RestartLgs();
                }
            }
            catch (IOException ex) {
                Logger.Warn("Error installing into logitech default profile");
                Logger.Warn(ex.Message, ex);
            }
        }


        /**
         * Parse the G-hub JSON format and add the exe path to all profiles
         */
        private static bool AddExeToJson(string json, string exe, out string newJson) {
            int numChanges = 0;
            var exeArray = new string[] { exe };

            JToken token = JToken.Parse(json);
            var applications = token.SelectToken("applications").SelectTokens("applications");
            foreach (var app in applications.Children()) {
                var userPath = app.SelectToken("userPaths");

                if (userPath == null) {
                    // "userPaths" does not exist
                    Logger.Warn($"Mapping missing for entry {app.SelectToken("name")}, auto adding..");
                    app.Last.AddAfterSelf(new JProperty("userPaths", exeArray));
                    numChanges++;
                }
                else {
                    // "userPaths" exists, but does not contain this exe
                    if (userPath.Children().All(m => !m.ToString().Equals(exe, StringComparison.InvariantCultureIgnoreCase))) {
                        Logger.Warn($"Mapping missing for entry {app.SelectToken("name")}, auto adding..");
                        JArray arr = userPath as JArray;
                        arr.Add(exe);
                        numChanges++;
                    }
                }
            }

            newJson = token.ToString();
            return numChanges > 0;
        }
    }
}