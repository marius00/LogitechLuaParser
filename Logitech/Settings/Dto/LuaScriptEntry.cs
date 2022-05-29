namespace Logitech.Settings.Dto {
    /// <summary>
    /// JSON Schema definition for "settings.json"
    /// Settings.json is a LuaScriptEntry[]
    /// </summary>
    public class LuaScriptEntry {
        public string Path { get; set; }
        public string Process { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
    }
}
