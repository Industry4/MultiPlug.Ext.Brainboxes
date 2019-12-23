
using System.Xml;

namespace MultiPlug.Ext.Brainboxes.Models.Components.Device
{
    public class DeviceInformation
    {
        public string Firmware { get; internal set; }
        public string Location { get; internal set; }
        public string MACAddress { get; internal set; }
        public string Name { get; internal set; }
        public string ProductModel { get; internal set; }
    }
}
