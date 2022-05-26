using System;
using System.Collections.Generic;
using log4net;

namespace Logitech.Led {
    internal class LogitechLedProvider : IDisposable {
        // TODO: Gotta ensure that the logitech LED, the keyboard events and the SendKey all uses the same mapping.
        // See https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.sendkeys?view=windowsdesktop-6.0 for sendkeys
        private readonly Dictionary<string, LogitechKeyMapping> _keyMapping = new Dictionary<string, LogitechKeyMapping>() {
            { "ESC", LogitechKeyMapping.ESC },
            { "F1", LogitechKeyMapping.F1 },
            { "F2", LogitechKeyMapping.F2 },
            { "F3", LogitechKeyMapping.F3 },
            { "F4", LogitechKeyMapping.F4 },
            { "F5", LogitechKeyMapping.F5 },
            { "F6", LogitechKeyMapping.F6 },
            { "F7", LogitechKeyMapping.F7 },
            { "F8", LogitechKeyMapping.F8 },
            { "F9", LogitechKeyMapping.F9 },
            { "F10", LogitechKeyMapping.F10 },
            { "F11", LogitechKeyMapping.F11 },
            { "F12", LogitechKeyMapping.F12 },
            { "PRINTSCREEN", LogitechKeyMapping.PRINT_SCREEN },
            { "SCROLL", LogitechKeyMapping.SCROLL_LOCK },
            { "PAUSE,.-", LogitechKeyMapping.PAUSE_BREAK },
            { "TILDE", LogitechKeyMapping.TILDE },
            { "D1", LogitechKeyMapping.ONE },
            { "D2", LogitechKeyMapping.TWO },
            { "D3", LogitechKeyMapping.THREE },
            { "D4", LogitechKeyMapping.FOUR },
            { "D5", LogitechKeyMapping.FIVE },
            { "D6", LogitechKeyMapping.SIX },
            { "D7", LogitechKeyMapping.SEVEN },
            { "D8", LogitechKeyMapping.EIGHT },
            { "D9", LogitechKeyMapping.NINE },
            { "D0", LogitechKeyMapping.ZERO },
            { "1", LogitechKeyMapping.ONE },
            { "2", LogitechKeyMapping.TWO },
            { "3", LogitechKeyMapping.THREE },
            { "4", LogitechKeyMapping.FOUR },
            { "5", LogitechKeyMapping.FIVE },
            { "6", LogitechKeyMapping.SIX },
            { "7", LogitechKeyMapping.SEVEN },
            { "8", LogitechKeyMapping.EIGHT },
            { "9", LogitechKeyMapping.NINE },
            { "0", LogitechKeyMapping.ZERO },
            { "MINUS", LogitechKeyMapping.MINUS },
            { "EQUALS", LogitechKeyMapping.EQUALS },
            { "BACKSPACE", LogitechKeyMapping.BACKSPACE },
            { "INSERT", LogitechKeyMapping.INSERT },
            { "HOME", LogitechKeyMapping.HOME },
            { "PAGE_UP", LogitechKeyMapping.PAGE_UP },
            { "NUMLOCK", LogitechKeyMapping.NUM_LOCK },
            { "DIVIDE", LogitechKeyMapping.NUM_SLASH },
            { "MULTIPLY", LogitechKeyMapping.NUM_ASTERISK },
            { "SUBTRACT", LogitechKeyMapping.NUM_MINUS },
            { "TAB", LogitechKeyMapping.TAB },
            { "Q", LogitechKeyMapping.Q },
            { "W", LogitechKeyMapping.W },
            { "E", LogitechKeyMapping.E },
            { "R", LogitechKeyMapping.R },
            { "T", LogitechKeyMapping.T },
            { "Y", LogitechKeyMapping.Y },
            { "U", LogitechKeyMapping.U },
            { "I", LogitechKeyMapping.I },
            { "O", LogitechKeyMapping.O },
            { "P", LogitechKeyMapping.P },
            { "OPEN_BRACKET", LogitechKeyMapping.OPEN_BRACKET },
            { "CLOSE_BRACKET", LogitechKeyMapping.CLOSE_BRACKET },
            { "BACKSLASH", LogitechKeyMapping.BACKSLASH },
            { "KEYBOARD_DELETE", LogitechKeyMapping.KEYBOARD_DELETE },
            { "END", LogitechKeyMapping.END },
            { "PAGE_DOWN", LogitechKeyMapping.PAGE_DOWN },
            { "NUMPAD7", LogitechKeyMapping.NUM_SEVEN },
            { "NUMPAD8", LogitechKeyMapping.NUM_EIGHT },
            { "NUMPAD9", LogitechKeyMapping.NUM_NINE },
            { "NUMPADPLUS", LogitechKeyMapping.NUM_PLUS },
            { "CAPS_LOCK", LogitechKeyMapping.CAPS_LOCK },
            { "A", LogitechKeyMapping.A },
            { "S", LogitechKeyMapping.S },
            { "D", LogitechKeyMapping.D },
            { "F", LogitechKeyMapping.F },
            { "G", LogitechKeyMapping.G },
            { "H", LogitechKeyMapping.H },
            { "J", LogitechKeyMapping.J },
            { "K", LogitechKeyMapping.K },
            { "L", LogitechKeyMapping.L },
            { "SEMICOLON", LogitechKeyMapping.SEMICOLON },
            { "APOSTROPHE", LogitechKeyMapping.APOSTROPHE },
            { "ENTER", LogitechKeyMapping.ENTER },
            { "NUMPAD4", LogitechKeyMapping.NUM_FOUR },
            { "NUMPAD5", LogitechKeyMapping.NUM_FIVE },
            { "NUMPAD6", LogitechKeyMapping.NUM_SIX },
            { "LEFT_SHIFT", LogitechKeyMapping.LEFT_SHIFT },
            { "Z", LogitechKeyMapping.Z },
            { "X", LogitechKeyMapping.X },
            { "C", LogitechKeyMapping.C },
            { "V", LogitechKeyMapping.V },
            { "B", LogitechKeyMapping.B },
            { "N", LogitechKeyMapping.N },
            { "M", LogitechKeyMapping.M },
            { "COMMA", LogitechKeyMapping.COMMA },
            { "PERIOD", LogitechKeyMapping.PERIOD },
            { "FORWARD_SLASH", LogitechKeyMapping.FORWARD_SLASH },
            { "RIGHT_SHIFT", LogitechKeyMapping.RIGHT_SHIFT },
            { "ARROW_UP", LogitechKeyMapping.ARROW_UP },
            { "NUMPAD1", LogitechKeyMapping.NUM_ONE },
            { "NUMPAD2", LogitechKeyMapping.NUM_TWO },
            { "NUMPAD3", LogitechKeyMapping.NUM_THREE },
            { "NUMPADENTER", LogitechKeyMapping.NUM_ENTER },
            { "LEFT_CONTROL", LogitechKeyMapping.LEFT_CONTROL },
            { "LEFT_WINDOWS", LogitechKeyMapping.LEFT_WINDOWS },
            { "LEFT_ALT", LogitechKeyMapping.LEFT_ALT },
            { "SPACE", LogitechKeyMapping.SPACE },
            { "RIGHT_ALT", LogitechKeyMapping.RIGHT_ALT },
            { "RIGHT_WINDOWS", LogitechKeyMapping.RIGHT_WINDOWS },
            { "APPLICATION_SELECT", LogitechKeyMapping.APPLICATION_SELECT },
            { "RIGHT_CONTROL", LogitechKeyMapping.RIGHT_CONTROL },
            { "ARROW_LEFT", LogitechKeyMapping.ARROW_LEFT },
            { "ARROW_DOWN", LogitechKeyMapping.ARROW_DOWN },
            { "ARROW_RIGHT", LogitechKeyMapping.ARROW_RIGHT },
            { "NUM_ZERO", LogitechKeyMapping.NUM_ZERO },
            { "NUM_PERIOD", LogitechKeyMapping.NUM_PERIOD },
            { "G1", LogitechKeyMapping.G_1 },
            { "G2", LogitechKeyMapping.G_2 },
            { "G3", LogitechKeyMapping.G_3 },
            { "G4", LogitechKeyMapping.G_4 },
            { "G5", LogitechKeyMapping.G_5 },
            { "G6", LogitechKeyMapping.G_6 },
            { "G7", LogitechKeyMapping.G_7 },
            { "G8", LogitechKeyMapping.G_8 },
            { "G9", LogitechKeyMapping.G_9 },
            { "G_LOGO", LogitechKeyMapping.G_LOGO },
            { "G_BADGE", LogitechKeyMapping.G_BADGE },
        };
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LogitechLedProvider));

        private bool _isInitialized;


        public void Start() {
            _isInitialized = LogitechGSDK.LogiLedInitWithName("LogitechLua");
            if (_isInitialized) {
                LogitechGSDK.LogiLedSetTargetDevice(LogitechGSDK.LOGI_DEVICETYPE_ALL);
            }
            else {
                Logger.Warn("Error initializing logitech LED driver");
            }
        }

        public void SetColor(string key, int r, int g, int b) {
            if (r < 0 || r > 100) {
                Logger.Warn($"Argument red \"{r}\" is outside range [0, 100]");
            } else if (g < 0 || g > 100) {
                Logger.Warn($"Argument green \"{g}\" is outside range [0, 100]");
            }
            else if (b < 0 || b > 100) {
                Logger.Warn($"Argument blue \"{b}\" is outside range [0, 100]");
            }
            else if (!_keyMapping.ContainsKey(key)) {
                Logger.Warn("Invalid key \"{key}\"");
            } else if (!_isInitialized) {
                Logger.Warn("Attempting to set keyboard colors, but LED api is not initialized");
            }
            else {
                Logger.Debug($"Setting color for {key} to ({r}, {g}, {b})");
                LogitechGSDK.LogiLedSetLightingForKeyWithKeyName(_keyMapping[key], r, g, b);
            }
        }

        public void Dispose() {
            if (_isInitialized) {
                LogitechGSDK.LogiLedShutdown();
            }
        }
    }
}
