﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KST.Settings.Dto {
    public class SettingsRootNode {
        public List<LuaScriptEntry> Entries { get; set; }
        public bool MinimizeToTray { get; set; }
        public bool StartMinimized { get; set; }
        public bool FirstRun { get; set; }
        public bool NoAnonymousUsageStats { get; set; }
        public string Uuid { get; set; }
    }
}
