using MultiPlug.Ext.Brainboxes.Models.Components.Discovery;
using System.Runtime.Serialization;

namespace MultiPlug.Ext.Brainboxes.Models.Load
{
    public class Discovery
    {
        [DataMember]
        public BBDiscoveryProperties Properties { get; set; }
    }
}
