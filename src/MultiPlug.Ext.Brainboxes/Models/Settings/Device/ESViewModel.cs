using System;
using MultiPlug.Base.Exchange;

namespace MultiPlug.Ext.Brainboxes.Models.Settings.Device
{
    [Serializable]
    public class ESViewModel
    {
        public string IOid { get; set; }
        public string Guid { get; set; }
        public string EventId { get; set; }
        public string Description { get; set; }
        public string RisingEdge { get; set; }
        public string FallingEdge { get; set; }
        public Payload CachedValue { get; internal set; }
    }
}
