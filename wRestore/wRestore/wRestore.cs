using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using wRestore.Definitions;
using static wRestore.Definitions.Restore;

namespace wRestore
{
    public partial class wRestore : Form
    {
        Restore Restore = new Restore();
        iDevice ConnectedDevice = new iDevice();
        ThreadScheduler DeviceCheckerThread = new ThreadScheduler();
        ThreadScheduler RestoreThread = new ThreadScheduler();
        ThreadScheduler DeviceTypesDownloadThread = new ThreadScheduler();

        string NoDeviceName = "";
        string NoModel = "";
        string NoVersion = "";

        bool KeepCheckingiDevices = true;
        bool DeviceConnected = false;

        public wRestore()
        {
            InitializeComponent();
            FormClosing += wRestore_Closing;

            NoDeviceName = deviceNameLbl.Text;
            NoModel = deviceModelLbl.Text;
            NoVersion = deviceVersionLbl.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Restore.BrowseFile("Browse for IPSW", FileType.IPSW);
            ipswTxtBox.Text = Restore.TargetVersion;
        }

        private void BasebandBtn_Click(object sender, EventArgs e)
        {
            Restore.BrowseFile("Browse for Baseband", FileType.Baseband);
            basebandTxtBox.Text = Restore.TargetBaseband;
        }

        private void buildmanifestBtn_Click(object sender, EventArgs e)
        {
            Restore.BrowseFile("Browse for Buildmanifest", FileType.Buildmanifest);
            buildmanifestTxtBox.Text = Restore.TargetBuildmanifest;
        }

        private void sepBtn_Click(object sender, EventArgs e)
        {
            Restore.BrowseFile("Browse for Sep", FileType.Sep);
            sepTxtBox.Text = Restore.TargetSep;
        }

        private void StartRestoreBtn_Click(object sender, EventArgs e)
        {
            if (!DeviceConnected)
            {
                MessageBox.Show("No iDevice is connected yet.", "No iDevice found!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (RestoreThread.IsRunning()) return;
            if (MessageBox.Show("Make sure to set your boot nonce if not already, if needed. Click OK when you're ready.", "Warning incoming", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                RestoreDevice();
            }
        }

        private void shshBtn_Click(object sender, EventArgs e)
        {
            Restore.BrowseFile("Browse for shsh blob", FileType.Blob);
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

        private void SetLabelText(Label lbl, string txt)
        {
            lbl.Text = txt;
        }

        private void GetiDevice()
        {
            if (DeviceCheckerThread == null)
            {
                DeviceCheckerThread = new ThreadScheduler();
                DeviceCheckerThread.Name = "DeviceCheckerThread";
                DeviceCheckerThread.IsBackground = true;
            }
            DeviceCheckerThread.Start(new Action(() =>
            {
                while (KeepCheckingiDevices)
                {
                    DeviceConnected = ConnectedDevice.CheckforDevices();
                    if (DeviceConnected)
                        ConnectedDevice.ConnectToDevice();
                    else ConnectedDevice.DiconnectDevice();

                    if (deviceNameLbl.InvokeRequired)
                        deviceNameLbl.BeginInvoke((MethodInvoker)delegate ()
                        {
                            SetLabelText(deviceNameLbl, NoDeviceName + ConnectedDevice.Name);
                        });

                    if (deviceModelLbl.InvokeRequired)
                        deviceModelLbl.BeginInvoke((MethodInvoker)delegate ()
                        {
                            SetLabelText(deviceModelLbl, NoModel + ConnectedDevice.DeviceIdentifier);
                        });

                    if (deviceVersionLbl.InvokeRequired)
                        deviceVersionLbl.BeginInvoke((MethodInvoker)delegate ()
                        {
                            SetLabelText(deviceVersionLbl, NoVersion + ConnectedDevice.Version);
                        });
                    Thread.Sleep(1000);
                }
            }));
        }
        private void RestoreDevice()
        {
            if (RestoreThread == null)
            {
                RestoreThread = new ThreadScheduler();
                RestoreThread.Name = "RestoreThread";
                RestoreThread.IsBackground = false;
            }
            RestoreThread.Start(new Action(() =>
            {
                Restore.Start();
            }));
        }

        private void GetDeviceTypes()
        {
            //if (DeviceTypesDownloadThread == null)
            //{
            //    DeviceTypesDownloadThread = new ThreadScheduler();
            //    DeviceTypesDownloadThread.Name = "DeviceTypesDownloadThread";
            //    DeviceTypesDownloadThread.IsBackground = false;
            //}
            //DeviceTypesDownloadThread.Start(new Action(() =>
            //{
            TypeParser.DownloadDeviceTypes();
            //}));
        }

        private void wRestore_Load(object sender, EventArgs e)
        {
            GetDeviceTypes();
            GetiDevice();
        }
        private void wRestore_Closing(object sender, FormClosingEventArgs e)
        {
            KeepCheckingiDevices = false;
            DeviceCheckerThread.Stop();
            RestoreThread.Stop();
        }
    }
}
