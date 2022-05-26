using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logitech.InputProviders.Args;

namespace Logitech.InputProviders {
    internal class LogitechInputProvider : IDisposable {
        private const int MaxGKeys = 29; // G1 G2 G...
        private const int MaxGStates = 3; // M1 M2 M3

        private volatile bool _isRunning = true;
        private Dictionary<int, bool> _state = new Dictionary<int, bool>(MaxGKeys);

        public event EventHandler OnInput;


        public void Start() {
            new Thread(() => {
                int r = LogitechGKeys.LogiGkeyInitWithoutCallback();
                Console.WriteLine($"Logitech Init result: {r}");

                while (_isRunning) {
                    Thread.Sleep(1);

                    for (int key = 1; key <= MaxGKeys; key++) {
                        for (int state = 1; state <= MaxGStates; state++) {
                            if (LogitechGKeys.LogiGkeyIsKeyboardGkeyPressed(key, state) != 0) {
                                // Limit it to 1 event per click (No support for holding down G keys for the moment)
                                if (!GetLastState(key)) {
                                    Console.WriteLine($"G{key} M{state} {Win32.GetForegroundProcessName()}");

                                    OnInput?.Invoke(this, new InputEventArg {
                                        Key = $"G{key}",
                                        Modifiers = new String[] {$"M{state}"}// TODO: Support for CTRL, Shift, Alt
                                    });
                                }
                            }

                            _state[key] = false;
                        }

                    }
                }

                LogitechGKeys.LogiGkeyShutdown();
            }).Start();
        }

        private bool GetLastState(int key) {
            if (_state.ContainsKey(key))
                return _state[key];

            return false;
        }

        public void Dispose() {
            _isRunning = false;
        }
    }
}
