using System.Collections.Generic;
using MultiPlug.Base;

namespace MultiPlug.Ext.Brainboxes.Models
{
    public class SettingsViewModel : MultiPlugBase
    {
        public List<DeviceViewModel> Devices { get; set; }
        public string Log { get; set; }
        public string LogEventId { get; set; }
        public string DiscoverDeviceId { get; set; }
        public string DeviceDiscoveredId { get; set; }
    }
}
