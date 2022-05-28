using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitech.InputProviders.Args {
    internal enum InputModifierState {
        Shift = 2,
        Ctrl = 4,
        Alt = 8,
        M1 = 16,
        M2 = 32,
        M3 = 64,
    }
}
