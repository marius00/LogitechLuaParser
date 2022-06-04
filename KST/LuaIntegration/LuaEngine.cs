using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using InputSimulatorStandard;
using log4net;
using KST.Led;
using NLua;

namespace KST.LuaIntegration {
    /// <summary>
    /// One per game/script
    /// </summary>
    internal class LuaEngine : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LuaEngine));

        const string Hardcoded = @"
function OutputLogMessage(...)
    provider:OutputLogMessage(...)
end

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


TickEvent = 1
KeyDownEvent = 2
KeyUpEvent = 3
FocusEvent = 4
";

        private Lua _lua;
        private LuaFunction _onEvent;
        private readonly List<string> _targets = new List<string>();
        private bool _resetStateQueued = false;
        private string _newScriptQueued = string.Empty;
        private readonly LuaIntegration _integration;

        public LuaEngine(LogitechLedProvider ledProvider, string script) {
            _integration = new LuaIntegration(ledProvider, _targets);

            _lua = new Lua();
            _lua.State.Encoding = Encoding.UTF8;
            _lua["provider"] = _integration;
            _resetStateQueued = false;

            SetScript(script);
        }

        public void ResetState() {
            _resetStateQueued = true;
        }

        public void ExecuteQueuedActions() {
            if (_resetStateQueued) {
                _lua?.Dispose();
                _lua = new Lua();
                _lua.State.Encoding = Encoding.UTF8;
                _lua["provider"] = _integration;
                _resetStateQueued = false;
            }


        }

        public void AddTarget(string target) {
            _targets.Add(target);
        }


        // TODO: Queue this to run on the Lua thread, to prevent race conditions
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
                _onEvent?.Call((int)eventType, arg, modifiers);
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