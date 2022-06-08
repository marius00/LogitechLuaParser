using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace KST.Misc {
    internal class IdleSleeper {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(IdleSleeper));
        private DateTimeOffset _lastActiveTime = DateTimeOffset.UtcNow.AddMinutes(-2);
        private bool _isActive;
        public void SetStatus(bool isActive) {
            if (isActive) {
            }

            // Not active now, was active before. Store last active time.
            else if (_isActive) {
                _lastActiveTime = DateTimeOffset.UtcNow;
            } 

            // Not been active for at least 2 runs..
            else if (!_isActive) {
                var minSinceLastActive = (DateTimeOffset.UtcNow - _lastActiveTime).Minutes;
                if (minSinceLastActive > 15) {
                    Thread.Sleep(3000);
                }
                else if (minSinceLastActive > 5) {
                    Thread.Sleep(1500);
                }
                else if(minSinceLastActive >= 1) {
                    Thread.Sleep(500);
                }
            }

            _isActive = isActive;
        }
    }
}
