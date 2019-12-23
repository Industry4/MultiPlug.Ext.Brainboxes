using System;

namespace MultiPlug.Ext.Brainboxes.Models.Settings.Defaults
{
    [Serializable]
    public class DefaultsValues
    {
        public string Key { get; set; }
        public string RisingEdge { get; set; }
        public string FallingEdge { get; set; }
    }
}
