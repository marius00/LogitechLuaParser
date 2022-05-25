using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace Logitech {
    internal class Program {
        private static volatile bool isRunning = true;
        static void Main(string[] args) {
            try {
                InterceptKeys.Start();
                int r = LogitechGKeys.LogiGkeyInitWithoutCallback();
                Console.WriteLine($"Logitech Init result: {r}");

                
                Thread t = new Thread(() => {
                    while (isRunning) {
                        Thread.Sleep(1);

                        int lastKey = -1;
                        int lastState = -1;
                        for (int key = 1; key <= 29; key++) {
                            for (int state = 1; state <= 3; state++) {
                                // Store the state of G-keys, so that we can filter duplicate clicks.
                                // Also store the timestamp.. if the key is being held down, we might want to trigger multiples? or do we?
                                if (LogitechGKeys.LogiGkeyIsKeyboardGkeyPressed(key, state) != 0) {
                                    Console.WriteLine($"G{key} M{state}");
                                    lastKey = key;
                                    lastState = state;
                                }
                                else {
                                    lastKey = -1;
                                    lastState = -1;

                                }
                            }

                        }
                    }
                });

                t.Start();
                Application.Run();

                isRunning = false;
            }
            finally {
                LogitechGKeys.LogiGkeyShutdown();
                InterceptKeys.Finalize();
            }
        }
    }
}
