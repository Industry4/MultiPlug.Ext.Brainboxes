using MultiPlug.Base;

namespace MultiPlug.Ext.Brainboxes.Models.Settings.Device
{
    public class Subscription : MultiPlugBase
    {
        public string DeviceId { get; set; }
        public string OutputId { get; set; }
        public string SubscriptionGuid { get; set; }
        public string SubscriptionId { get; set; }
        public string HighKey { get; set; }
        public string HighValue { get; set; }
        public string LowKey { get; set; }
        public string LowValue { get; set; }
    }
}
