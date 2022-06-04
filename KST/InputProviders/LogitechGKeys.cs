using System;
using System.Runtime.InteropServices;

namespace KST.InputProviders {
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


        [DllImport("LogitechGkeyEnginesWrapper.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int LogiGkeyIsKeyboardGkeyPressed(int gKeyNumber, int modeNumber);



        [DllImport("LogitechGkeyEnginesWrapper.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void LogiGkeyShutdown();


        [DllImport("LogitechGkeyEnginesWrapper.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int LogiGkeyInitWithoutCallback();


        [DllImport("LogitechGkeyEnginesWrapper.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr LogiGkeyGetKeyboardGkeyString(int gKeyNumber, int modeNumber);


    
    }
}