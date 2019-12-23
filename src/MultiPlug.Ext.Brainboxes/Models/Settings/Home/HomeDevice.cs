using System;

namespace MultiPlug.Ext.Brainboxes.Models.Settings.Home
{
    [Serializable]
    public class HomeDevice
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
    }
}
