using System;
using System.Linq;
using log4net;

namespace Logitech.LuaIntegration {
    public class LuaInterface {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LuaInterface));
        public static void OutputLogMessage(string message, params object[] args) {
            try {
                Logger.Debug(string.Format(message, args));
            } catch (FormatException ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(message + "[" + string.Join(", ", args.Select(arg => arg.ToString())) + "]");

            }
        }

        public static void OutputLogMessage(string message) {
            Logger.Debug(message);
        }

        public void PressKey(string key) {
            // TODO:
        }

        public void SetBacklightColor() {
            // TODO:
        }
    }
}
