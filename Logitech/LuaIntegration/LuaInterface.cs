using System;
using System.Linq;

namespace Logitech.LuaIntegration {
    public class LuaInterface {
        public static void OutputLogMessage(string message, params object[] args) {
            try {
                Console.WriteLine(message, args);
            } catch (FormatException ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine(message + "[" + string.Join(", ", args.Select(arg => arg.ToString())) + "]");

            }
        }

        public static void OutputLogMessage(string message) {
            Console.WriteLine(message);
        }

        public void PressKey(string key) {
            // TODO:
        }

        public void SetBacklightColor() {
            // TODO:
        }
    }
}
