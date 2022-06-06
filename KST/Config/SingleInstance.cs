using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace KST.Config {


    /// <summary>This class ensures there is only one instance of this program running at any given time</summary>
    public class SingleInstance : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        private Mutex _mutex = null;
        private readonly Boolean _ownsMutex = false;
        private Guid _identifier = Guid.Empty;

        /// <summary>
        /// Enforces single instance for an application.
        /// </summary>
        /// <param name="identifier">An identifier unique to this application.</param>
        public SingleInstance(Guid identifier) {
            this._identifier = identifier;
            _mutex = new Mutex(true, identifier.ToString(), out _ownsMutex);
        }

        /// <summary>
        /// Indicates whether this is the first instance of this application.
        /// </summary>
        public Boolean IsFirstInstance { get { return _ownsMutex; } }


        #region IDisposable
        private Boolean disposed = false;

        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                if (_mutex != null && _ownsMutex)
                    try {
                        _mutex.ReleaseMutex();
                        _mutex = null;
                    } catch (System.ApplicationException) {
                        // Were shutting down, so just eat it and move on.
                    } catch (Exception ex) {
                        Logger.Warn(ex.Message);
                    }
                disposed = true;
            }
        }

        ~SingleInstance() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
