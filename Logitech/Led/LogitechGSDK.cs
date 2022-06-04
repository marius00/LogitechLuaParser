using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Logitech.Led {


    public enum DeviceType {
        Keyboard = 0x0,
        Mouse = 0x3,
        Mousemat = 0x4,
        Headset = 0x8,
        Speaker = 0xe
    }

    internal class LogitechGSDK {

        //LED SDK
        private const int LOGI_DEVICETYPE_MONOCHROME_ORD = 0;
        private const int LOGI_DEVICETYPE_RGB_ORD = 1;
        private const int LOGI_DEVICETYPE_PERKEY_RGB_ORD = 2;

        public const int LOGI_DEVICETYPE_MONOCHROME = (1 << LOGI_DEVICETYPE_MONOCHROME_ORD);
        public const int LOGI_DEVICETYPE_RGB = (1 << LOGI_DEVICETYPE_RGB_ORD);
        public const int LOGI_DEVICETYPE_PERKEY_RGB = (1 << LOGI_DEVICETYPE_PERKEY_RGB_ORD);
        public const int LOGI_DEVICETYPE_ALL = (LOGI_DEVICETYPE_MONOCHROME | LOGI_DEVICETYPE_RGB | LOGI_DEVICETYPE_PERKEY_RGB);

        public const int LOGI_LED_BITMAP_WIDTH = 21;
        public const int LOGI_LED_BITMAP_HEIGHT = 6;
        public const int LOGI_LED_BITMAP_BYTES_PER_KEY = 4;

        public const int LOGI_LED_BITMAP_SIZE = LOGI_LED_BITMAP_WIDTH * LOGI_LED_BITMAP_HEIGHT * LOGI_LED_BITMAP_BYTES_PER_KEY;
        public const int LOGI_LED_DURATION_INFINITE = 0;

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedInit();


        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool DllRegisterServer();

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool DllUnregisterServer();

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedInitWithName(String name);

        //Config option functions
        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedGetConfigOptionNumber([MarshalAs(UnmanagedType.LPWStr)] String configPath, ref double defaultNumber);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedGetConfigOptionBool([MarshalAs(UnmanagedType.LPWStr)] String configPath, ref bool defaultRed);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedGetConfigOptionColor([MarshalAs(UnmanagedType.LPWStr)] String configPath, ref int defaultRed, ref int defaultGreen, ref int defaultBlue);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedGetConfigOptionKeyInput([MarshalAs(UnmanagedType.LPWStr)] String configPath, StringBuilder buffer, int bufsize);
        /////////////////////

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedSetTargetDevice(int targetDevice);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedGetSdkVersion(ref int majorNum, ref int minorNum, ref int buildNum);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedSaveCurrentLighting();

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedSetLighting(int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedRestoreLighting();

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedFlashLighting(int redPercentage, int greenPercentage, int bluePercentage, int milliSecondsDuration, int milliSecondsInterval);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedPulseLighting(int redPercentage, int greenPercentage, int bluePercentage, int milliSecondsDuration, int milliSecondsInterval);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedStopEffects();

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedExcludeKeysFromBitmap(LogitechKeyMapping[] keyList, int listCount);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedSetLightingFromBitmap(byte[] bitmap);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedSetLightingForKeyWithScanCode(int keyCode, int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedSetLightingForKeyWithHidCode(int keyCode, int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedSetLightingForKeyWithQuartzCode(int keyCode, int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedSetLightingForKeyWithKeyName(LogitechKeyMapping keyCode, int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedSaveLightingForKey(LogitechKeyMapping keyName);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedRestoreLightingForKey(LogitechKeyMapping keyName);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedFlashSingleKey(LogitechKeyMapping keyName, int redPercentage, int greenPercentage, int bluePercentage, int msDuration, int msInterval);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedPulseSingleKey(LogitechKeyMapping keyName, int startRedPercentage, int startGreenPercentage, int startBluePercentage, int finishRedPercentage, int finishGreenPercentage, int finishBluePercentage, int msDuration, bool isInfinite);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedStopEffectsOnKey(LogitechKeyMapping keyName);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLedSetLightingForTargetZone(DeviceType deviceType, int zone, int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("LogitechLed ", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LogiLedShutdown();
    }
}
