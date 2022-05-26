using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Logitech.InputProviders.Args;
using Logitech.LuaIntegration;

namespace Logitech.InputProviders {
    internal class LogitechInputProvider : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LogitechInputProvider));
        private const int MaxGKeys = 29; // G1 G2 G...
        private const int MaxGStates = 3; // M1 M2 M3

        private volatile bool _isRunning = true;
        private readonly Dictionary<string, bool> _state = new Dictionary<string, bool>(MaxGKeys);

        public event EventHandler OnInput;


        public void Start() {
            new Thread(() => {
                if (LogitechGKeys.LogiGkeyInitWithoutCallback() == 0) {
                    Logger.Error("Could not initialize logitech input provider, G-keys are disabled");
                    return;
                }

                while (_isRunning) {
                    Thread.Sleep(1);

                    for (int key = 1; key <= MaxGKeys; key++) {
                        for (int state = 1; state <= MaxGStates; state++) {
                            var uniqueKey = $"G{key} M{state}";
                            if (LogitechGKeys.LogiGkeyIsKeyboardGkeyPressed(key, state) != 0) {

                                // Limit it to 1 event per click (No support for holding down G keys for the moment)
                                if (!GetLastState(uniqueKey)) {
                                    // Console.WriteLine($"G{key} M{state} {Win32.GetForegroundProcessName()}");

                                    OnInput?.Invoke(this, new InputEventArg {
                                        Key = $"G{key}",
                                        Modifiers = new String[] {$"M{state}"}// TODO: Support for CTRL, Shift, Alt
                                    });
                                }

                                _state[uniqueKey] = true;
                            }
                            else {
                                _state[uniqueKey] = false;
                            }
                        }

                    }
                }

                LogitechGKeys.LogiGkeyShutdown();
            }).Start();
        }

        private bool GetLastState(string key) {
            if (_state.ContainsKey(key))
                return _state[key];

            return false;
        }

        public void Dispose() {
            _isRunning = false;
        }
    }
}
