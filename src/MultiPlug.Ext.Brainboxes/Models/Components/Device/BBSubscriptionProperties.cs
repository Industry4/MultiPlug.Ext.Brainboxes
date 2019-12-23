using System.Runtime.Serialization;
using Brainboxes.IO;
using MultiPlug.Base.Exchange;

namespace MultiPlug.Ext.Brainboxes.Models.Components.Device
{
    public class BBSubscriptionProperties : Subscription
    {
        public IOLine IOLine { get; set; }
        [DataMember]
        public string HighKey { get; set; }
        [DataMember]
        public string HighValue { get; set; }
        [DataMember]
        public string LowKey { get; set; }
        [DataMember]
        public string LowValue { get; set; }
    }
}
