using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InputSimulatorStandard;
using InputSimulatorStandard.Native;
using log4net;
using Logitech.Led;
using NLua;

namespace Logitech.LuaIntegration {
    /// <summary>
    /// One per game/script
    /// </summary>
    internal class LuaEngine : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LuaEngine));

        const string Hardcoded = @"
import ('Logitech', 'Logitech.LuaIntegration')
OutputLogMessage = LuaInterface.OutputLogMessage

function SetBacklightColor(k, r, g, b)
    provider:SetColor(k, r, g, b)
end

function SendKeys(keys)
    provider:SendInput(keys)
end

function KeyUp(key)
    provider:KeyUp(key)
end

function KeyDown(key)
    provider:KeyDown(key)
end

function KeyPress(key)
    provider:KeyPress(key)
end

function ResetScript()
    provider:ResetState()
end

function MouseDown(key)
    provider:MouseDown(key)
end

function MouseUp(key)
    provider:MouseUp(key)
end

function MouseClick(key)
    provider:MouseClick(key)
end

function MouseDoubleClick(key)
    provider:MouseDoubleClick(key)
end



TickEvent = LuaEventType.Tick
InputEvent = LuaEventType.Input
FocusEvent = LuaEventType.Focus

                    ";

        private Lua _lua;
        private LuaFunction _onEvent;
        private readonly LogitechLedProvider _ledProvider;
        private readonly string _target;
        private readonly InputSimulator _simulator = new InputSimulator();

        public LuaEngine(LogitechLedProvider ledProvider, string target, string script) {
            _ledProvider = ledProvider;
            _target = target;

            ResetState();
            SetScript(script);
        }

        public void ResetState() {
            _lua?.Dispose();
            _lua = new Lua();
            _lua.State.Encoding = Encoding.UTF8;
            _lua.LoadCLRPackage();
            _lua["provider"] = this;
        }

        public void SetColor(string key, int r, int g, int b) {
            if (Win32.GetForegroundProcessName().Equals(_target, StringComparison.CurrentCultureIgnoreCase))
                _ledProvider.SetColor(key, r, g, b);
        }

        // TODO: Ideally I'd like to wrap these somehow.. so they're public to LAU but not to C#..
        public void SendInput(string keys) {
            if (Win32.GetForegroundProcessName().Equals(_target, StringComparison.CurrentCultureIgnoreCase)) {
                Process p = Process.GetProcessesByName(_target).FirstOrDefault();
                if (p != null) {
                    IntPtr h = p.MainWindowHandle;
                    Win32.SetForegroundWindow(h);
                    SendKeys.SendWait(keys);
                }
            }
        }

        // https://github.com/GregsStack/InputSimulatorStandard
        public void KeyDown(string key) {
            if (KeyMapper.IsValidKeyCode(key)) {
                if (Win32.GetForegroundProcessName().Equals(_target, StringComparison.CurrentCultureIgnoreCase)) {
                    _simulator.Keyboard.KeyDown(KeyMapper.TranslateToKeyCode(key));
                }
            }
            else {
                Logger.Warn($"Invalid key \"{key}\"");
            }
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


        public void KeyUp(string key) {
            if (KeyMapper.IsValidKeyCode(key)) {
                if (Win32.GetForegroundProcessName().Equals(_target, StringComparison.CurrentCultureIgnoreCase)) {
                    _simulator.Keyboard.KeyUp(KeyMapper.TranslateToKeyCode(key));
                }
            }
            else {
                Logger.Warn($"Invalid key \"{key}\"");
            }
        }

        public void KeyPress(string key) {
            if (KeyMapper.IsValidKeyCode(key)) {
                if (Win32.GetForegroundProcessName().Equals(_target, StringComparison.CurrentCultureIgnoreCase)) {
                    _simulator.Keyboard.KeyPress(KeyMapper.TranslateToKeyCode(key));
                }
            }
            else {
                Logger.Warn($"Invalid key \"{key}\"");
            }
        }

        public bool SetScript(string script) {
            try {
                _lua.DoString(Hardcoded + script);
                _onEvent = _lua["OnEvent"] as LuaFunction;
                return true;
            }
            catch (NLua.Exceptions.LuaScriptException ex) {
                Logger.Error("Error parsing script");
                Logger.Error(ex.Message, ex);
            }

            return false;
        }


        public void OnEvent(LuaEventType eventType, string arg, string[] modifiers) {
            try {
                _onEvent.Call(eventType, arg, modifiers);
            }
            catch (NLua.Exceptions.LuaScriptException ex) {
                Logger.Error("Error executing script");
                Logger.Error(ex.Message, ex);
            }
            catch (NLua.Exceptions.LuaException ex) {
                Logger.Error("Error executing script");
                Logger.Error(ex.Message, ex);
            }
        }

        public void Dispose() {
            _lua.Dispose();
        }
    }
}