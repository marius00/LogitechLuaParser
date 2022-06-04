using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KST.InputProviders.Args {
    public class InputEventArg : EventArgs {
        public InputEventArg(string key, ushort modifiers, InputEventType type) {
            Key = key;
            Modifiers = modifiers;
            Type = type;
        }
        public string Key { get; private set; }
        public ushort Modifiers { get; private set; }
        public InputEventType Type { get; private set; }
    }
}
