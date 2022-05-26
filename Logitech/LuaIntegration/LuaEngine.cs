using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Logitech.Led;
using NLua;

namespace Logitech.LuaIntegration {
    /// <summary>
    /// One per game/script
    /// </summary>
    internal class LuaEngine : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LuaEngine));
        const string hardcoded = @"
                               import ('Logitech', 'Logitech.LuaIntegration')
                               OutputLogMessage = LuaInterface.OutputLogMessage
                               function SetBacklightColor(k, r, g, b)
                                    provider:SetColor(k, r, g, b)
                               end
                    ";

        private readonly Lua _lua;
        private LuaFunction _onEvent;
        private readonly LogitechLedProvider _ledProvider;

        public LuaEngine(LogitechLedProvider ledProvider, string script) {
            _lua = new Lua();
            _lua.State.Encoding = Encoding.UTF8;
            _lua.LoadCLRPackage();
            _lua["provider"] = this;

            _ledProvider = ledProvider;

            SetScript(script);
        }

        public void SetColor(string key, int r, int g, int b) {
            Logger.Info("blabla it works");
            _ledProvider.SetColor(key, r, g, b);
        }

        private void SetScript(string script) {
            try {
                _lua.DoString(hardcoded + script);
                _onEvent = _lua["OnEvent"] as LuaFunction;
            } catch (NLua.Exceptions.LuaScriptException ex) {
                Console.WriteLine("Error parsing script");
                Console.WriteLine(ex.Message);
            }

        }

        public void OnEvent(LuaEventType eventType, string arg, string[] modifiers) {
            try {
                _onEvent.Call(eventType, arg, modifiers);
            } catch (NLua.Exceptions.LuaScriptException ex) {
                Console.WriteLine("Error executing script");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            } catch (NLua.Exceptions.LuaException ex) {
                Console.WriteLine("Error executing script");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void Dispose() {
            _lua.Dispose();
        }
    }
}
