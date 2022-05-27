using System;
using System.Collections.Generic;
using log4net;

namespace Logitech.Led {
    internal class LogitechLedProvider : IDisposable {
        // TODO: Gotta ensure that the logitech LED, the keyboard events and the SendKey all uses the same mapping.
        // See https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.sendkeys?view=windowsdesktop-6.0 for sendkeys
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
            else if (!KeyMapper.IsValidLogitechMapping(key)) {
                Logger.Warn("Invalid key \"{key}\"");
            } else if (!_isInitialized) {
                Logger.Warn("Attempting to set keyboard colors, but LED api is not initialized");
            }
            else {
                Logger.Debug($"Setting color for {key} to ({r}, {g}, {b})");
                LogitechGSDK.LogiLedSetLightingForKeyWithKeyName(KeyMapper.TranslateToLogitechMapping(key), r, g, b);
            }
        }

        public void Dispose() {
            if (_isInitialized) {
                LogitechGSDK.LogiLedShutdown();
            }
        }
    }
}
