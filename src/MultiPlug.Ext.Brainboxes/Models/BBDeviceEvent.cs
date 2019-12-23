using System.Runtime.Serialization;
using MultiPlug.Base.Exchange;

namespace MultiPlug.Ext.Brainboxes.Models
{
    public class BBDeviceEvent : Event
    {
        [DataMember]
        public int IONumber { get; set; }
        [DataMember]
        public string RisingEdgeValue { get; set; }
        [DataMember]
        public string FallingEdgeValue { get; set; }
    }
}
