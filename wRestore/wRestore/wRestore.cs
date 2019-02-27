using System;
using System.Diagnostics;
using System.Windows.Forms;
using wRestore.Definitions;

namespace wRestore
{
    public partial class wRestore : Form
    {
        Restore Restore = new Restore();

        public wRestore()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Restore.BrowseFile("Browse for IPSW", Definitions.Type.IPSW);
            ipswTxtBox.Text = Restore.TargetVersion;
        }

        private void BasebandBtn_Click(object sender, EventArgs e)
        {
            Restore.BrowseFile("Browse for Baseband", Definitions.Type.Baseband);
            basebandTxtBox.Text = Restore.TargetBaseband;
        }

        private void buildmanifestBtn_Click(object sender, EventArgs e)
        {
            Restore.BrowseFile("Browse for Buildmanifest", Definitions.Type.Buildmanifest);
            buildmanifestTxtBox.Text = Restore.TargetBuildmanifest;
        }

        private void sepBtn_Click(object sender, EventArgs e)
        {
            Restore.BrowseFile("Browse for Sep", Definitions.Type.Sep);
            sepTxtBox.Text = Restore.TargetSep;
        }

        private void StartRestoreBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Make sure to set your boot nonce if not already, if needed. Click OK when you're ready.", "Warning incoming", MessageBoxButtons.OKCancel) == DialogResult.OK)
                Restore.Start();
        }

        private void shshBtn_Click(object sender, EventArgs e)
        {
            Restore.BrowseFile("Browse for shsh blob", Definitions.Type.Blob);
            shshTxtBox.Text = Restore.TargetBlob;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/s0uthwest/futurerestore/releases");
        }

        private void exitRecoveryBtn_Click(object sender, EventArgs e)
        {
            Restore.Start(true);
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string msg = "To ensure a successful restore, make sure you have the following pre-requisits:\n\n" +
                "* Shsh blob - for the target version you want to restore to\n" +
                "* Target ipsw - target version ipsw. Should the the referred version from the blob\n" +
                "* Latest baseband - latest baseband from current signed firmware ipsw. Very important to get the proper baseband or your device may brick!\n" +
                "* Latest buildmanifest - latest buildmanifest from current signed firmware ipsw\n" +
                "* Latest sep - latest sep from current signed firmware ipsw. Very important to get the proper sep or your device may brick!";
            MessageBox.Show(msg, "Requirements", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.theiphonewiki.com/wiki/Firmware/iPad");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.theiphonewiki.com/wiki/Firmware/iPhone");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Eperty123");
        }
    }
}
