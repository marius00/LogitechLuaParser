using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitech.InputProviders.Args {
    internal class InputEventArg : EventArgs {
        public string Key { get; set; }
        public string[] Modifiers { get; set; } = Array.Empty<string>();
    }
}
