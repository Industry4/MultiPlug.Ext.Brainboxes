using System;

namespace MultiPlug.Ext.Brainboxes.Models.Settings.Device
{
    [Serializable]
    public class DeviceOutputViewModel
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public string UIToggleEventID { get; set; }
        public ESViewModel[] Subscriptions { get; set; }

    }
}
