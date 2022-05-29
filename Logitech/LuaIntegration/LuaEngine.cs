using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using InputSimulatorStandard;
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
import ('Logitech.InputProviders.Args')
OutputLogMessage = LuaInterface.OutputLogMessage

function SetBacklightColor(k, r, g, b)
    provider:SetColor(k, r, g, b)
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

function MouseMove(x, y)
    provider:MouseMove(x, y)
end

function Sleep(ms)
    provider:Sleep(ms)
end

function time()
    return provider:Time()
end

function IsShift(modifier)
    return (modifier & InputModifierStateHelper.GetValue(InputModifierState.Shift)) > 0
end

function IsCtrl(modifier)
    return (modifier & InputModifierStateHelper.GetValue(InputModifierState.Ctrl)) > 0
end

function IsAlt(modifier)
    return (modifier & InputModifierStateHelper.GetValue(InputModifierState.Alt)) > 0
end

function IsM1(modifier)
    return (modifier & InputModifierStateHelper.GetValue(InputModifierState.M1)) > 0
end

function IsM2(modifier)
    return (modifier & InputModifierStateHelper.GetValue(InputModifierState.M2)) > 0
end

function IsM3(modifier)
    return (modifier & InputModifierStateHelper.GetValue(InputModifierState.M3)) > 0
end


TickEvent = LuaEventType.Tick
KeyDownEvent = LuaEventType.KeyDown
KeyUpEvent = LuaEventType.KeyUp
FocusEvent = LuaEventType.Focus


                    ";

        private Lua _lua;
        private LuaFunction _onEvent;
        private readonly LogitechLedProvider _ledProvider;
        private readonly List<string> _targets = new List<string>();
        private readonly InputSimulator _simulator = new InputSimulator();

        public LuaEngine(LogitechLedProvider ledProvider, string script) {
            _ledProvider = ledProvider;
            
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

        public void AddTarget(string target) {
            _targets.Add(target);
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
            }
            else {
                Logger.Warn($"Invalid key \"{key}\"");
            }
        }

        public void KeyPress(string key) {
            if (KeyMapper.IsValidKeyCode(key)) {
                if (HasFocus) {
                    _simulator.Keyboard.KeyPress(KeyMapper.TranslateToKeyCode(key));
                }
            }
            else {
                Logger.Warn($"Invalid key \"{key}\"");
            }
        }

        public void Sleep(int milliseconds) {
            Thread.Sleep(milliseconds);
        }

        public long Time() {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public bool SetScript(string script) {
            try {
                _onEvent = null;
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


        public void OnEvent(LuaEventType eventType, string arg, ushort modifiers) {
            try {
                _onEvent?.Call(eventType, arg, modifiers);
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