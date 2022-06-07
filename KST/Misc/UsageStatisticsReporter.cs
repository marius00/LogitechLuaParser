using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;
using Timer = System.Timers.Timer;

namespace KST.Misc {
    internal class UsageStatisticsReporter : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UsageStatisticsReporter));
        private readonly Timer _timer;
        public static string Uuid { protected get; set; }

        public static string UrlStats {
            get;
            set;
        }

        public UsageStatisticsReporter() {

            int min = 1000 * 60;
            int hour = 60 * min;
            _timer = new Timer();
            _timer.Start();
            _timer.Elapsed += (a1, a2) => {
                if (Thread.CurrentThread.Name == null) {
                    Thread.CurrentThread.Name = "ReportUsageThread";
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                }

                ReportUsageAsync();
            };

            _timer.Interval = 12 * hour;
            _timer.AutoReset = true;
            _timer.Start();
        }

        public static void ReportUsageAsync() {
            ThreadPool.QueueUserWorkItem(m => ReportUsage());
        }

        private static void ReportUsage() {
            try {
                string postData = string.Format("version={0}&uuid={1}", Uri.EscapeDataString(VersionString), Uuid);
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(UrlStats);
                Encoding encoding = new UTF8Encoding();
                byte[] data = encoding.GetBytes(postData);

                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/x-www-form-urlencoded";
                httpWReq.ContentLength = data.Length;

                using (Stream stream = httpWReq.GetRequestStream()) {
                    stream.Write(data, 0, data.Length);
                }

                // threshold
                using (HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse()) {
                    if (response.StatusCode != HttpStatusCode.OK) {
                        Logger.Warn("Failed to send anonymous usage statistics");
                        return;
                    }

                    string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    Logger.Info("Sent anonymous usage statistics");
                }
            } catch (Exception ex) {
                Logger.Fatal(ex.Message);
                Logger.Fatal(ex.StackTrace);
            }
        }


        private static string VersionString {
            get {
                try {
                    var version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
                    return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
                } catch (Exception ex) {
                    Logger.Warn("Error getting assembly version, automatic updates may not function correctly.");
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);

                    var version = Assembly.GetExecutingAssembly().GetName().Version;
                    return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
                }
            }
        }

        public void Dispose() {
            _timer?.Stop();
        }
    }
}
