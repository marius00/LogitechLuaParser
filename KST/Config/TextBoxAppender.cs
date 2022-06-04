using System;
using System.Collections.Generic;
using System.Windows.Forms;
using log4net.Appender;
using log4net.Core;

namespace KST.Config {

    // https://stackoverflow.com/questions/14114614/configuring-log4net-textboxappender-custom-appender-via-xml-file
    public class TextBoxAppender : AppenderSkeleton {
        private readonly object _lockObj = new object();
        private TextBox _textBox;
        public string FormName { get; set; }
        public string TextBoxName { get; set; }
        private List<string> backlog = new List<string>();

        protected override void Append(LoggingEvent loggingEvent) {
            var msg = "[" + loggingEvent.TimeStamp.ToString("HH:mm:ss") + "] " + loggingEvent.RenderedMessage + "\r\n";

            if (_textBox == null) {
                backlog.Add(msg);
                if (backlog.Count > 200)
                    backlog.Clear();

                if (string.IsNullOrEmpty(FormName) || String.IsNullOrEmpty(TextBoxName))
                    return;

                Form form = Application.OpenForms[FormName];
                if (form == null)
                    return;

                _textBox = form.Controls[TextBoxName] as TextBox;
                if (_textBox == null)
                    return;

                form.FormClosing += (s, e) => {
                    lock (_lockObj) {
                        _textBox = null;
                    }
                };
            }


            if (backlog.Count > 0) {
                msg = string.Join("", backlog) + msg;
                backlog.Clear();
            }

            lock (_lockObj) {
                var del = new Action<string>(s => {
                    if (_textBox.TextLength > _textBox.MaxLength * 0.8) {
                        _textBox.Text = _textBox.Text.Substring(_textBox.TextLength / 2);
                    }
                    _textBox.AppendText(s);
                });
                try {
                    _textBox.BeginInvoke(del, msg);
                }
                catch (System.InvalidOperationException) {
                    /* Probably logging in the middle of a shutdown. Window just got destroyed */
                }
            }
        }
    }
}
