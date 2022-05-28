using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Logitech.UI {
    public partial class MainContainer : Form {
        public MainContainer() {
            InitializeComponent();
        }

        private void MainContainer_Load(object sender, EventArgs e) {
            ConfigurationView f = new ConfigurationView();
            f.TopLevel = false;
            /*p.Controls.Add(f);
            p.Width = p.Parent.Width;
            p.Height = p.Parent.Height;*/
            Controls.Add(f);
            f.Show();
        }
    }
}
