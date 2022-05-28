﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Logitech.Config;
using Logitech.Settings;

namespace Logitech.UI {
    public partial class ConfigurationView : Form {
        public ConfigurationView() {
            InitializeComponent();
        }

        private void ConfigurationView_Load(object sender, EventArgs e) {
            this.Dock = DockStyle.Fill;
            UpdateListView();

            listView1.DoubleClick +=ListView1_DoubleClick;
            listView1.ItemSelectionChanged +=ListView1_ItemSelectionChanged;
            btnModify.Enabled = listView1.SelectedItems.Count > 0;
            btnDelete.Enabled = listView1.SelectedItems.Count > 0;
        }

        private void ListView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
            btnModify.Enabled = listView1.SelectedItems.Count > 0;
            btnDelete.Enabled = listView1.SelectedItems.Count > 0;
        }

        private void ListView1_DoubleClick(object sender, EventArgs e) {
            btnModify_Click(sender, e);
        }

        private void UpdateListView() {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            var sr = SettingsReader.Load(AppPaths.SettingsFile);
            foreach (var entry in sr) {
                ListViewItem lvi = new ListViewItem(entry.Description ?? entry.Process);
                lvi.SubItems.Add(entry.Process);
                lvi.SubItems.Add(entry.Path);
                lvi.Tag = entry.Id;
                listView1.Items.Add(lvi);
            }

            listView1.EndUpdate();
        }

        private void btnAdd_Click(object sender, EventArgs e) {
            var dialog = new AddModifyEntry();
            if (dialog.ShowDialog() == DialogResult.OK) {
                var entries = SettingsReader.Load(AppPaths.SettingsFile).ToList();
                if (entries.Any(m => m.Process == dialog.Process)) {
                    MessageBox.Show("A script already exists for this process", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                entries.Add(new SettingsJsonEntry {
                    Process = dialog.Process,
                    Description = dialog.Description,
                    Path = dialog.Script,
                    Id = Guid.NewGuid().ToString()
                });



                SettingsReader.Persist(AppPaths.SettingsFile, entries.ToArray());
                UpdateListView();
            }
        }

        private void btnModify_Click(object sender, EventArgs e) {
            foreach (var entry in listView1.SelectedItems) {
                ListViewItem lvi = (ListViewItem)entry;
                var dialog = new AddModifyEntry() {
                    Description = lvi.Text,
                    Process = lvi.SubItems[1].Text,
                    Script = lvi.SubItems[2].Text
                };

                if (dialog.ShowDialog() == DialogResult.OK) {
                    var tag = lvi.Tag.ToString();

                    var entries = SettingsReader.Load(AppPaths.SettingsFile).Where(m => m.Id != tag).ToList();
                    if (entries.Any(m => m.Process == dialog.Process)) {
                        MessageBox.Show("A script already exists for this process", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    entries.Add(new SettingsJsonEntry {
                        Process = dialog.Process,
                        Description = dialog.Description,
                        Path = dialog.Script,
                        Id = tag
                    });
                    
                    SettingsReader.Persist(AppPaths.SettingsFile, entries.ToArray());

                    UpdateListView();
                }
                return;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Are you sure?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;

            foreach (var entry in listView1.SelectedItems) {
                ListViewItem lvi = (ListViewItem)entry;
                var tag = lvi.Tag.ToString();

                var entries = SettingsReader.Load(AppPaths.SettingsFile).Where(m => m.Id != tag).ToArray();
                SettingsReader.Persist(AppPaths.SettingsFile, entries);
                UpdateListView();
                return;
            }

        }

        private void btnViewFolder_Click(object sender, EventArgs e) {
            Process.Start(AppPaths.SettingsFolder);
        }

        private void linkViewLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(Path.Combine(AppPaths.CoreFolder, "log.txt"));
        }
    }
}
