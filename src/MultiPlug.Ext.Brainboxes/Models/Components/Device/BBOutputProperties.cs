using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MultiPlug.Ext.Brainboxes.Models.Components.Device
{
    public class BBOutputProperties
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public List<BBSubscription> Subscriptions { get; set; } = new List<BBSubscription>();
    }
}
