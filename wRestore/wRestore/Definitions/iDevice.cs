using iMobileDevice;
using iMobileDevice.iDevice;
using iMobileDevice.Lockdown;
using iMobileDevice.Plist;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace wRestore.Definitions
{
    public class iDevice
    {
        /* Public variables */
        public string Name { get; set; }
        public string Version { get; set; }
        public string DeviceIdentifier { get; set; }

        private DeviceType Type { get; set; }
        private bool DeviceConnected { get; set; }

        private iDeviceHandle deviceHandle;
        private LockdownClientHandle lockdownHandle;
        private iDeviceApi DeviceState { get; set; }
        private LockdownApi LockDownState { get; set; }
        private PlistApi PlistReader { get; set; }
        private PlistHandle PlistHandle { get; set; }
        private ReadOnlyCollection<string> pluggedDevices { get; set; }

        #region PRIVATE METHODS

        #region Device Management
        private bool HookDevice()
        {
            // Don't do anything if a device already connected.
            //if (DeviceConnected) return;

            // Load native imobiledevice shit..
            NativeLibraries.Load();

            bool result = false;

            ReadOnlyCollection<string> _pluggedDevices;
            int pluggedDevicesCount = 0;

            try
            {
                DeviceState = (iDeviceApi)LibiMobileDevice.Instance.iDevice;
                LockDownState = (LockdownApi)LibiMobileDevice.Instance.Lockdown;

                // Try to connect to any connected iDevice...
                DeviceState.idevice_get_device_list(out _pluggedDevices, ref pluggedDevicesCount);


                // Populate the list with found iDevices and then put
                // the said devices to our global list.
                pluggedDevices = _pluggedDevices;

                // Check if any device is present, if no do nothing but display message.
                if (pluggedDevices.Count <= 0)
                {
                    // Not actually an error in our case.
                    MessageBox.Show("No iDevice found!", "No iDevice connected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    ReleaseDevice();
                    result = false;
                }
                // Since we only plan (and only supports) 1 device at a time, check.
                else if (pluggedDevices.Count > 1)
                {
                    MessageBox.Show("Please only connect one iDevice at a time!", "Multiple iDevices detected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    result = false;
                }

                // Etasblish connection to found device.
                if (pluggedDevices.Count > 0)
                {
                    // Connect to the first found device...
                    var thisDevice = pluggedDevices[0];

                    DeviceState.idevice_new(out deviceHandle, thisDevice).ThrowOnError();
                    LockDownState.lockdownd_client_new_with_handshake(deviceHandle, out lockdownHandle, "Quamotion").ThrowOnError();
                    PlistReader = new PlistApi(LibiMobileDevice.Instance);
                    DeviceConnected = (LockDownState != null ? true : false);
                    if (DeviceConnected)
                    {
                        ParseDevice();
                        result = true;
                    }
                }
            }
            catch (LockdownException)
            {
                ReleaseDevice();
                throw new LockdownException("Couldn't connect to iDevice. Please make sure your device has trusted this pc.");
            }
            return result;
        }
        private void ReleaseDevice()
        {
            if (!DeviceConnected) return;

            deviceHandle.Dispose();
            lockdownHandle.Dispose();

            Name = "";
            DeviceIdentifier = "";
            Version = "";
            DeviceConnected = false;
        }
        private bool IsDeviceConnected()
        {
            // Don't do anything if a device already connected.
            //if (DeviceConnected) return;

            // Load native imobiledevice shit..
            NativeLibraries.Load();

            bool result = false;

            ReadOnlyCollection<string> _pluggedDevices;
            int pluggedDevicesCount = 0;

            try
            {
                DeviceState = (iDeviceApi)LibiMobileDevice.Instance.iDevice;
                LockDownState = (LockdownApi)LibiMobileDevice.Instance.Lockdown;

                // Try to connect to any connected iDevice...
                DeviceState.idevice_get_device_list(out _pluggedDevices, ref pluggedDevicesCount);


                // Populate the list with found iDevices and then put
                // the said devices to our global list.
                pluggedDevices = _pluggedDevices;

                // Etasblish connection to found device.
                if (pluggedDevices.Count > 0)
                {
                    // Connect to the first found device...
                    var thisDevice = pluggedDevices[0];

                    DeviceState.idevice_new(out deviceHandle, thisDevice).ThrowOnError();
                    LockDownState.lockdownd_client_new_with_handshake(deviceHandle, out lockdownHandle, "Quamotion").ThrowOnError();
                    DeviceConnected = (LockDownState != null ? true : false);
                    if (DeviceConnected)
                    {
                        //ParseDevice();
                        result = true;
                    }
                }
            }
            catch (LockdownException)
            {
                ReleaseDevice();
                //throw new LockdownException("Couldn't connect to iDevice. Please make sure your device has trusted this pc.");
            }
            return result;
        }
        #endregion

        #region Data Management
        private void WriteDataToFile(string data, string path)
        {
            if (data == null || data == "")
            {
                MessageBox.Show("Data is null! Cannot export something that doesn't exist!");
                return;
            }

            if (path == null || path == "")
            {
                MessageBox.Show("Path is empty! Please specify a save location.");
                return;
            }

            // Continue if no errors found...
            File.WriteAllText(path, data);
        }
        private string ConvertPlistToXml(PlistHandle plist)
        {
            string result = "";
            uint buffer = 1024;

            if (PlistReader != null)
                PlistReader.plist_to_xml(plist, out result, ref buffer);
            return result;
        }
        private string GetFieldValue(PlistHandle phandle)
        {
            string result = "";
            string data = ConvertPlistToXml(phandle);
            Match match = Regex.Match(data, "<string>(.*)</string>");
            if (match.Success) result = match.Groups[1].Value;
            else result = "";
            return result;
        }
        #endregion

        #region Parser Management
        private void ParseDevice(string identifier = "")
        {
            try
            {
                string _identifier = (identifier == "" || identifier == null ? GetInfo(null, "ProductType") : identifier);
                Match match = Regex.Match(_identifier, @"iPhone([0-9]+),([0-9]+)|iPad([0-9]+),([0-9]+)|iPod([0-9]+),([0-9]+)");
                string _deviceIdentifer = (match.Success ? match.Value : "");
                DeviceIdentifier = (GetModel(_deviceIdentifer) != "" ? GetModel(_deviceIdentifer) : "Unknown");

                Name = GetName();
                Version = GetInfo(null, "ProductVersion");
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "Error");
            }
        }

        private string GetModel(string pattern)
        {
            string result = "";
            if (TypeParser.DeviceTypes.Count > 0)
            {
                foreach (string device in TypeParser.DeviceTypes)
                {
                    var split = device.Split(':');
                    var identifier = split[0];
                    var model = split[1];
                    if (Regex.Match(identifier, pattern).Success)
                        result = model;
                }
            }
            return result;
        }
        #endregion

        #endregion

        #region PUBLIC METHODS

        #region Device Management
        public bool CheckforDevices()
        {
            return IsDeviceConnected();
        }
        public bool ConnectToDevice()
        {
            return HookDevice();
        }
        public void DiconnectDevice()
        {
            ReleaseDevice();
        }
        public string GetInfo(string domain = null, string name = null)
        {
            string result = "";
            PlistHandle _plistData = PlistHandle;
            if (lockdownHandle != null)
                LockDownState.lockdownd_get_value(lockdownHandle, domain, name, out _plistData);
            // Show plist data.
            //result = ConvertPlistToXml(_plistData);

            // Get actual value.
            result = GetFieldValue(_plistData);
            result = (result == "" ? result = "Unknown" : result);
            return result;

        }
        public string GetName()
        {
            string name = "";
            if (lockdownHandle != null)
                LockDownState.lockdownd_get_device_name(lockdownHandle, out name);
            return name;
        }
        #endregion

        #region Data Management
        public void WriteData(string data, string path)
        {
            WriteDataToFile(data, path);
        }
        #endregion

        #endregion
    }
}
