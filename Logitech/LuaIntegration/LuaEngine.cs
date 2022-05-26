using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
                    ";

        private readonly Lua _lua;
        private LuaFunction _onEvent;
        private readonly LogitechLedProvider _ledProvider;
        private readonly string _target;

        public LuaEngine(LogitechLedProvider ledProvider, string target, string script) {
            _lua = new Lua();
            _lua.State.Encoding = Encoding.UTF8;
            _lua.LoadCLRPackage();
            _lua["provider"] = this;

            _ledProvider = ledProvider;
            _target = target;

            SetScript(script);
        }

        public void SetColor(string key, int r, int g, int b) {
            if (Win32.GetForegroundProcessName().Equals(_target, StringComparison.CurrentCultureIgnoreCase))
                _ledProvider.SetColor(key, r, g, b);
        }

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

        private void SetScript(string script) {
            try {
                _lua.DoString(Hardcoded + script);
                _onEvent = _lua["OnEvent"] as LuaFunction;
            } catch (NLua.Exceptions.LuaScriptException ex) {
                Logger.Error("Error parsing script");
                Logger.Error(ex.Message, ex);
            }

        }

        public void OnEvent(LuaEventType eventType, string arg, string[] modifiers) {
            try {
                _onEvent.Call(eventType, arg, modifiers);
            } catch (NLua.Exceptions.LuaScriptException ex) {
                Logger.Error("Error executing script");
                Logger.Error(ex.Message, ex);
            } catch (NLua.Exceptions.LuaException ex) {
                Logger.Error("Error executing script");
                Logger.Error(ex.Message, ex);
            }
        }

        public void Dispose() {
            _lua.Dispose();
        }
    }
}
