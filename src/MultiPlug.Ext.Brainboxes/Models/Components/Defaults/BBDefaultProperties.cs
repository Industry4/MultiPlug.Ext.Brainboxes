
using System.Runtime.Serialization;

namespace MultiPlug.Ext.Brainboxes.Models.Components.Defaults
{
    public class BBDefaultProperties
    {
        [DataMember]
        public string Key { get; set; } = "value";
        [DataMember]
        public string RisingEdge { get; set; } = "1";
        [DataMember]
        public string FallingEdge { get; set; } = "0";
    }
}
