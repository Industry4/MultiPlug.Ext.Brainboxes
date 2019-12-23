using System;

namespace MultiPlug.Ext.Brainboxes.Models.Settings.Discovery
{
    [Serializable]
    public class DiscoveryViewModel
    {
        public string DiscoverySubscription { get; set; }
        public string StartDeviceDiscoveryEventId { get; set; }
        public string StartDeviceDiscoveryEventDescription { get; set; }
    }
}
