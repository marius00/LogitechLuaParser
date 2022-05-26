using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitech {
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
    }
}
