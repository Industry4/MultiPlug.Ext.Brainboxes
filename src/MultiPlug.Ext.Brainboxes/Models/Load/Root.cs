using MultiPlug.Ext.Brainboxes.Models.Components.Defaults;
using MultiPlug.Ext.Brainboxes.Models.Components.Device;
using System.Runtime.Serialization;

namespace MultiPlug.Ext.Brainboxes.Models.Load
{
    public class Root
    {
        [DataMember]
        public BBDeviceProperties[] Devices { get; set; }
        [DataMember]
        public Discovery Discovery { get; set; }
        [DataMember]
        public BBDefaultProperties Defaults { get; set; }
    }
}
