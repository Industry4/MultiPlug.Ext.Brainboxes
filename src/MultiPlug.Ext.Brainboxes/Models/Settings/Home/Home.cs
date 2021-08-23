using System;

namespace MultiPlug.Ext.Brainboxes.Models.Settings.Home
{
    [Serializable]
    public class Home
    {
        public HomeDevice[] Devices { get; set; }
        public string DiscoverDeviceEventId { get; set; }
        public string DevicesListEventId { get;  set; }
    }

}
