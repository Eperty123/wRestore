using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace wRestore.Definitions
{
    public class Restore
    {
        // Definitions
        public string TargetBlob { get; set; } = "";
        public string TargetBuildmanifest { get; set; } = "";
        public string TargetBaseband { get; set; } = "";
        public string TargetSep { get; set; } = "";
        public string TargetVersion { get; set; } = "";

        // Checkboxes
        public CheckBox LatestBasebandCheckbox { get; set; }
        public CheckBox LatestSepCheckBox { get; set; }

        // Bools
        // Unused for now...
        public bool UseLatestBaseband { get; set; }
        public bool UseLatestSep { get; set; }

        // Other
        private string BBCommand { get; set; } = "";
        private string SepCommand { get; set; } = "";
        private OpenFileDialog FileBrowser { get; set; } = new OpenFileDialog();

        // Shsh blob, sep, baseband, buildmanifest and lastly ipsw.
        // -d is just for debugging purposes, ignore.
        public string RestoreCommand { get; set; } = "-t \"{0}\" -s \"{1}\" -b \"{2}\" -p \"{3}\" -m \"{4}\" {5} -d";
        public string RecoveryCommand { get; set; } = "--exit-recovery";
        private string CreatedBat { get; set; } = "";


        /// <summary>
        /// Creates a bat which will be executed. Essential!
        /// </summary>
        /// <param name="restore"></param>
        private void CreateBat(bool recovery = false)
        {
            string file = Path.Combine(Directory.GetCurrentDirectory(), (!recovery ? "restore.bat" : "recovery.bat"));
            string cmd = "cmd /K futurerestore " + (!recovery ? RestoreCommand : RecoveryCommand);
            File.WriteAllText(file, cmd);
            CreatedBat = file;
        }

        /// <summary>
        /// Parse the corresponding commands.
        /// </summary>
        private void ParseCommand()
        {
            // Get checkbox states.
            //GetCheckboxState();

            if (TargetBaseband != "" && TargetBuildmanifest != "" && TargetSep != "" && TargetVersion != "")
            {
                RestoreCommand = string.Format("-t \"{0}\" -s \"{1}\" -b \"{2}\" -p \"{3}\" -m \"{4}\" \"{5}\" -d", TargetBlob, TargetSep, TargetBaseband, TargetBuildmanifest, TargetBuildmanifest, TargetVersion);
                CreateBat();
                StartRestore();
            }
            else
            {
                MessageBox.Show("Please add baseband, buildmanifest, sep and ipsw!", "Not so fast!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// Start the restore/exit recovery process.
        /// </summary>
        private void StartRestore()
        {
            Process p = new Process();
            p.StartInfo.FileName = CreatedBat;
            //p.StartInfo.UseShellExecute = false;
            p.StartInfo.WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory());
            //p.StartInfo.RedirectStandardError = true;
            p.Start();

            p.WaitForExit();
        }

        /// <summary>
        /// Get check state of the checkboxes. (Unused for now...)
        /// </summary>
        private void GetCheckboxState()
        {
            UseLatestBaseband = LatestBasebandCheckbox.Checked;
            UseLatestSep = LatestSepCheckBox.Checked;
        }

        /// <summary>
        /// Exit recovery mode.
        /// </summary>
        private void KickRecoveryMode()
        {
            CreateBat(true); // True means recovery mode and false means restore.
            StartRestore();
        }

        /// <summary>
        /// Start restoring.
        /// </summary>
        /// <param name="recoverymode"></param>
        public void Start(bool recoverymode = false)
        {
            if (!recoverymode) ParseCommand();
            else KickRecoveryMode();
        }

        /// <summary>
        /// Opens up a file browser.
        /// </summary>
        /// <param name="title">Title of the file browser.</param>
        /// <param name="type">Type to browse.</param>
        public void BrowseFile(string title, Type type)
        {
            FileBrowser = new OpenFileDialog();
            FileBrowser.Title = title;
            string ChosenFile = "";

            switch (type)
            {
                case Type.Blob:
                    FileBrowser.Filter = "Shsh blob v2|*.shsh2|Shsh blob|*.shsh";
                    break;

                case Type.Baseband:
                    FileBrowser.Filter = "Baseband|*.bbfw";
                    break;

                case Type.Buildmanifest:
                    FileBrowser.Filter = "Buildmanifest|*.plist";
                    break;

                case Type.IPSW:
                    FileBrowser.Filter = "IPSW|*.ipsw";
                    break;

                case Type.Sep:
                    FileBrowser.Filter = "Sep|*.im4p";
                    break;
            }

            if (FileBrowser.ShowDialog() == DialogResult.OK)
            {
                ChosenFile = FileBrowser.FileName;

                switch (type)
                {
                    case Type.Blob:
                        FileBrowser.Filter = "Shsh blob v2|*.shsh2|Shsh blob|*.shsh";
                        TargetBlob = ChosenFile;
                        break;

                    case Type.Baseband:
                        FileBrowser.Filter = "Baseband|*.bbfw";
                        TargetBaseband = ChosenFile;
                        break;

                    case Type.Buildmanifest:
                        FileBrowser.Filter = "Buildmanifest|*.plist";
                        TargetBuildmanifest = ChosenFile;
                        break;

                    case Type.IPSW:
                        FileBrowser.Filter = "IPSW|*.ipsw";
                        TargetVersion = ChosenFile;
                        break;

                    case Type.Sep:
                        FileBrowser.Filter = "Sep|*.im4p";
                        TargetSep = ChosenFile;
                        break;
                }
            }
        }

        /// <summary>
        /// Assign checkboxes. (Unused for now...)
        /// </summary>
        /// <param name="baseband">Baseband checkbox.</param>
        /// <param name="sep">Sep checkbox.</param>
        public void SetCheckboxes(CheckBox baseband, CheckBox sep)
        {
            UseLatestBaseband = baseband.Checked;
            UseLatestSep = sep.Checked;
        }
    }
}
