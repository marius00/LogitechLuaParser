using System;
using System.Text;
using log4net;
using KST.Led;
using NLua;
using System.IO;
using KST.Config;

namespace KST.LuaIntegration {
    /// <summary>
    /// One per game/script
    /// </summary>
    internal class LuaEngine : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LuaEngine));

        private readonly Lua _lua;
        private LuaFunction _onEvent;
        private string _newScriptQueued;
        private readonly LuaIntegration _integration;

        public LuaEngine(LogitechLedProvider ledProvider, string script) {
            _integration = new LuaIntegration(ledProvider);

            _lua = new Lua();
            
            _lua.State.Encoding = Encoding.UTF8;

            Directory.SetCurrentDirectory(AppPaths.SettingsFolder);
            
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
                    _lua.DoString(_newScriptQueued);
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