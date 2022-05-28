using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitech.Settings {
    /// <summary>
    /// JSON Schema definition for "settings.json"
    /// Settings.json is a SettingsJsonEntry[]
    /// </summary>
    public class SettingsJsonEntry {
        public string Path { get; set; }
        public string Process { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
    }
}
