using MultiPlug.Ext.Brainboxes.Models.Settings.Device;
using System;
using System.Collections.Generic;

namespace MultiPlug.Ext.Brainboxes.Models
{
  //  [Serializable]
    public class DeviceViewModel : MarshalByRefObject
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public List<DeviceOutputViewModel> Outputs { get; set; }
        public List<ESViewModel> Events { get; set; }
        public string EventIdConnectCmd { get; set; }
        public string Status { get; set; }
    }
}
