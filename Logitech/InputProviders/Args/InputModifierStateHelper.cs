using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitech.InputProviders.Args {
    static class InputModifierStateHelper {
        public static ulong GetValue(this InputModifierState s1) {
            return (ulong)s1;
        }
    }
}
