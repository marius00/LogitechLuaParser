using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;

namespace Logitech.UI {
    public partial class BrowseProcess : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BrowseProcess));

        public string ProcessName => tbProcess.Text;

        public BrowseProcess() {
            InitializeComponent();
        }


        private void BrowseProcess_Load(object sender, EventArgs e) {
            listView1.BeginUpdate();
            listView1.Items.Clear();

            foreach (var process in Process.GetProcesses()) {
                try {
                    if (process.MainWindowHandle == IntPtr.Zero)
                        continue;

                    ListViewItem lvi = new ListViewItem(process.ProcessName + ": " +process.MainWindowTitle);
                    lvi.SubItems.Add(process.MainModule.FileName);
                    lvi.Tag = process.ProcessName;
                    listView1.Items.Add(lvi);
                }
                catch (Exception ex) {
                    Logger.Warn(ex.Message, ex);
                }
            }
            listView1.EndUpdate();

            listView1.ItemSelectionChanged +=ListView1_ItemSelectionChanged;
            listView1.DoubleClick +=ListView1_DoubleClick;
        }

        private void ListView1_DoubleClick(object sender, EventArgs e) {
            foreach (var item in listView1.SelectedItems) {
                var lvi = (ListViewItem)item;
                tbProcess.Text = lvi.Tag.ToString();
                btnOk_Click(this, e);
            }
        }

        private void ListView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
            tbProcess.Text = e?.Item?.Tag?.ToString() ?? string.Empty;
        }

        private void btnOk_Click(object sender, EventArgs e) {
            this.DialogResult = tbProcess.Text.Length > 0 ? DialogResult.OK : DialogResult.Abort;
            this.Close();
        }
    }
}
