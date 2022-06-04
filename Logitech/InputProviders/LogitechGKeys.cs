using System;
using System.Runtime.InteropServices;

namespace Logitech.InputProviders {
    /// <summary>
    /// Keyboard hook for local Logitech key events
    /// Logitech keys only (G-keys etc)
    /// </summary>
    class LogitechGKeys {
        /*
            LogiGkeyGetKeyboardGkeyString
            LogiGkeyGetMouseButtonString
            LogiGkeyInit
            LogiGkeyInitWithoutCallback
            LogiGkeyInitWithoutContext
            LogiGkeyIsKeyboardGkeyPressed
            LogiGkeyIsMouseButtonPressed
            LogiGkeyShutdown
        */


        [DllImport("LogitechGkey.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int LogiGkeyIsKeyboardGkeyPressed(int gKeyNumber, int modeNumber);



        [DllImport("LogitechGkey.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void LogiGkeyShutdown();


        [DllImport("LogitechGkey.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int LogiGkeyInitWithoutCallback();


        [DllImport("LogitechGkey.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr LogiGkeyGetKeyboardGkeyString(int gKeyNumber, int modeNumber);


    
    }
}