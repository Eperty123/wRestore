using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wRestore.Definitions
{
    public enum DeviceType
    {
        Blob,
        Baseband,
        Buildmanifest,
        Sep,
        IPSW
    }

    public static class TypeParser
    {
        public static List<string> DeviceTypes { get; set; }

        public static void DownloadDeviceTypes()
        {
            string info_url = "https://gist.githubusercontent.com/adamawolf/3048717/raw/371db8e0c83b8254d03c54712044a9a560099dbc/Apple_mobile_device_types.txt";
            string[] devices_unformatted = new[] { "" };
            DeviceTypes = new List<string>();
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += (sender, e) =>
            {
                devices_unformatted = e.Result.Split('\n');
                foreach (string device in devices_unformatted)
                {
                    var split = device.Split(':');
                    var deviceIdentier = split[0];
                    var deviceModel = (split.Length > 1 ? split[1] : "");
                    DeviceTypes.Add(string.Format("{0}:{1}", deviceIdentier, deviceModel));
                }
            };
            wc.DownloadStringAsync(new Uri(info_url));
        }
    }
}
