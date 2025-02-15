﻿using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using KST.Settings.Dto;
using Newtonsoft.Json;

namespace KST.Settings {
    /// <summary>
    /// Reads settings.json and parses the result
    /// </summary>
    internal static class SettingsReader {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SettingsReader));

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
        };

        public static void Persist(string filename, SettingsRootNode data) {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented, Settings);
            try {
                File.WriteAllText(filename, json);
            }
            catch (Exception ex) {
                Logger.Warn(ex.Message, ex);
            }
        }

        public static SettingsRootNode Load(string filename) {
            if (File.Exists(filename)) {
                try {
                    string json = File.ReadAllText(filename);
                    var template = JsonConvert.DeserializeObject<SettingsRootNode>(json, Settings);
                    return template;
                }
                catch (IOException ex) {
                    Logger.Error($"Error reading settings from {filename}, discarding settings.", ex);
                }
                catch (JsonReaderException ex) {
                    Logger.Error($"Error parsing settings from {filename}, discarding settings.", ex);
                }
            }

            Logger.Warn("Could not find settings JSON, defaulting to no settings.");
            return new SettingsRootNode {
                Entries = new List<LuaScriptEntry>(),
                FirstRun = true,
                MinimizeToTray = true,
                StartMinimized = false
            };
        }
    }
}