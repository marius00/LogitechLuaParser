using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NLua;

namespace Logitech.LuaIntegration {
    /// <summary>
    /// One per game/script
    /// </summary>
    internal class LuaEngine : IDisposable {
        const string hardcoded = @"
                               import ('Logitech', 'Logitech.LuaIntegration')
                               OutputLogMessage = LuaInterface.OutputLogMessage
                    ";

        private readonly Lua _lua;
        private LuaFunction _onEvent;

        public LuaEngine(string script) {
            _lua = new Lua();
            _lua.State.Encoding = Encoding.UTF8;
            _lua.LoadCLRPackage();

            SetScript(script);
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
