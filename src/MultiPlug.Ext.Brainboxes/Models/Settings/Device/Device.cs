using MultiPlug.Base;
using System.Runtime.Serialization;

namespace MultiPlug.Ext.Brainboxes.Models.Settings.Device
{
    public class Device : MultiPlugBase
    {
        [DataMember]
        public string Guid { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Status { get; set; }
     //   public string Log { get; set; }
        [DataMember]
        public string IPAddress { get; set; }
        [DataMember]
        public string MACAddress { get; set; }
        [DataMember]
        public string Location { get; set; }
        [DataMember]
        public string ProductModel { get; set; }
        [DataMember]
        public string Firmware { get; set; }
        [DataMember]
        public string ConnectButtonText { get; set; }
        public string ConnectButtonEventId { get; set; }
        [DataMember]
        public ESViewModel[] Events { get; set; }
        [DataMember]
        public DeviceOutputViewModel[] Outputs { get; set; }
        public string LogEventId { get; internal set; }
        public string RestartButtonEventId { get; internal set; }
        public string DeviceStatusEventId { get; internal set; }
    }
}
