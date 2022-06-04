using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InputSimulatorStandard;
using KST.Led;
using log4net;

namespace KST.LuaIntegration {
    internal class LuaIntegration {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LuaIntegration));
        private readonly LogitechLedProvider _ledProvider;
        private readonly InputSimulator _simulator = new InputSimulator();
        private readonly List<string> _targets;

        public LuaIntegration(LogitechLedProvider ledProvider, List<string> targets) {
            _ledProvider = ledProvider;
            _targets = targets;
        }


        private bool HasFocus => _targets.Any(m => m.Equals(Win32.GetForegroundProcessName(), StringComparison.CurrentCultureIgnoreCase));

        public void SetColor(string key, int r, int g, int b) {
            if (HasFocus)
                _ledProvider.SetColor(key, r, g, b);
        }

        public void MouseMove(int x, int y) {
            _simulator.Mouse.MoveMouseBy(x, y);
        }

        public void MouseDown(string key) {
            if (key == "LMB") {
                _simulator.Mouse.LeftButtonDown();
            } else if (key == "RMB") {
                _simulator.Mouse.RightButtonDown();
            } else if (key == "MMB") {
                _simulator.Mouse.MiddleButtonDown();
            } else {
                Logger.Warn($"Unknown mouse key \"{key}\", expected LMB, RMB or MMB");
            }
        }

        public void MouseUp(string key) {
            if (key == "LMB") {
                _simulator.Mouse.LeftButtonUp();
            } else if (key == "RMB") {
                _simulator.Mouse.RightButtonUp();
            } else if (key == "MMB") {
                _simulator.Mouse.MiddleButtonUp();
            } else {
                Logger.Warn($"Unknown mouse key \"{key}\", expected LMB, RMB or MMB");
            }
        }

        public void MouseClick(string key) {
            if (key == "LMB") {
                _simulator.Mouse.LeftButtonClick();
            } else if (key == "RMB") {
                _simulator.Mouse.RightButtonClick();
            } else if (key == "MMB") {
                _simulator.Mouse.MiddleButtonClick();
            } else {
                Logger.Warn($"Unknown mouse key \"{key}\", expected LMB, RMB or MMB");
            }
        }

        public void MouseDoubleClick(string key) {
            if (key == "LMB") {
                _simulator.Mouse.LeftButtonDoubleClick();
            } else if (key == "RMB") {
                _simulator.Mouse.RightButtonDoubleClick();
            } else if (key == "MMB") {
                _simulator.Mouse.MiddleButtonDoubleClick();
            } else {
                Logger.Warn($"Unknown mouse key \"{key}\", expected LMB, RMB or MMB");
            }


        }



        // https://github.com/GregsStack/InputSimulatorStandard
        public void KeyDown(string key) {
            if (KeyMapper.IsValidKeyCode(key)) {
                _simulator.Keyboard.KeyDown(KeyMapper.TranslateToKeyCode(key));
                if (HasFocus) {
                    _simulator.Keyboard.KeyDown(KeyMapper.TranslateToKeyCode(key));
                }
            } else {
                Logger.Warn($"Invalid key \"{key}\"");
            }
        }

        public void KeyUp(string key) {
            if (KeyMapper.IsValidKeyCode(key)) {
                if (HasFocus) {
                    _simulator.Keyboard.KeyUp(KeyMapper.TranslateToKeyCode(key));
                }
            } else {
                Logger.Warn($"Invalid key \"{key}\"");
            }
        }

        public void KeyPress(string key) {
            if (KeyMapper.IsValidKeyCode(key)) {
                if (HasFocus) {
                    _simulator.Keyboard.KeyPress(KeyMapper.TranslateToKeyCode(key));
                }
            } else {
                Logger.Warn($"Invalid key \"{key}\"");
            }
        }

        public void Sleep(int milliseconds) {
            Thread.Sleep(milliseconds);
        }

        public long Time() {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public void OutputLogMessage(string message, params object[] args) {
            try {
                Logger.Debug(string.Format(message, args));
            } catch (FormatException ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(message + "[" + string.Join(", ", args.Select(arg => arg.ToString())) + "]");

            }
        }

        public void OutputLogMessage(string message) {
            Logger.Debug(message);
        }

    }
}
