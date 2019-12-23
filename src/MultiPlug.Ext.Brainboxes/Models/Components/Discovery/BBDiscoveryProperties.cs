using MultiPlug.Base.Exchange;
using System.Runtime.Serialization;

namespace MultiPlug.Ext.Brainboxes.Models.Components.Discovery
{
    public class BBDiscoveryProperties
    {
        //[DataMember]
        //public Event DiscoveryEvent { get; set; }
        [DataMember]
        public Subscription DiscoverySubscription { get; set; }
        [DataMember]
        public EventExternal StartDeviceDiscovery { get; set; }
    }
}
