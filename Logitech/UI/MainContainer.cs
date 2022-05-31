using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Logitech.Config;
using Logitech.Settings;

namespace Logitech.UI {
    public partial class MainContainer : Form {
        MinimizeToTrayHandler _minimizeToTrayHandler;
        public MainContainer() {
            InitializeComponent();

            var settings = SettingsReader.Load(AppPaths.SettingsFile);
            _minimizeToTrayHandler = new MinimizeToTrayHandler(this, notifyIcon1, settings.StartMinimized, settings.MinimizeToTray);

            if (settings.FirstRun) {
                settings.StartMinimized = true;
                settings.MinimizeToTray = true;
                settings.FirstRun = false;
                SettingsReader.Persist(AppPaths.SettingsFile, settings);
            }
        }

        private void MainContainer_Load(object sender, EventArgs e) {
            ConfigurationView f = new ConfigurationView();
            f.TopLevel = false;
            Controls.Add(f);
            f.Show();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {
            _minimizeToTrayHandler.notifyIcon_MouseDoubleClick(sender, e);
        }
    }
}
