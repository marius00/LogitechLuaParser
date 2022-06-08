using System;
using System.Text;
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
    return (modifier & 2) > 0
end

function IsCtrl(modifier)
    return (modifier & 4) > 0
end

function IsAlt(modifier)
    return (modifier & 8) > 0
end

function IsM1(modifier)
    return (modifier & 16) > 0
end

function IsM2(modifier)
    return (modifier & 32) > 0
end

function IsM3(modifier)
    return (modifier & 64) > 0
end

function SetOutputPrefix(prefix)
    provider:SetOutputPrefix(prefix)
end


TickEvent = 1
KeyDownEvent = 2
KeyUpEvent = 3
FocusEvent = 4
";

        private readonly Lua _lua;
        private LuaFunction _onEvent;
        private string _newScriptQueued;
        private readonly LuaIntegration _integration;

        public LuaEngine(LogitechLedProvider ledProvider, string script) {
            _integration = new LuaIntegration(ledProvider);

            _lua = new Lua();
            _lua.State.Encoding = Encoding.UTF8;
            _lua["provider"] = _integration;
            _newScriptQueued = script;
        }

        /// <summary>
        /// Performs queued actions such as changing the active script
        /// Is called before events to ensure thread-safe behavior (no changing script mid-execution)
        /// </summary>
        public void ExecuteQueuedActions() {
            if (!string.IsNullOrEmpty(_newScriptQueued)) {

                try {
                    _onEvent = null;
                    _lua.DoString(Hardcoded + _newScriptQueued);
                    _onEvent = _lua["OnEvent"] as LuaFunction;
                }
                catch (NLua.Exceptions.LuaScriptException ex) {
                    Logger.Error("Error parsing script");
                    Logger.Error(ex.Message, ex);
                }

                _newScriptQueued = string.Empty;
            }


        }

        /// <summary>
        /// Queues a new script change
        /// </summary>
        /// <param name="script"></param>
        public void QueueScriptChange(string script) {
            _newScriptQueued = script;
        }

        /// <summary>
        /// Executes OnEvent in the lua script
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="arg"></param>
        /// <param name="modifiers"></param>
        public void OnEvent(LuaEventType eventType, string arg, ushort modifiers) {
            try {
                ExecuteQueuedActions();
            }
            catch (Exception ex) {
                // Should not happen.. some bug if it does..
                Logger.Error(ex.Message, ex);
            }

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