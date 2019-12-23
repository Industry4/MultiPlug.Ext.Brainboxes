using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlug.Ext.Brainboxes.Models
{
    [Serializable]
    public class EventModel
    {
        public string DeviceGuid { get; set; }
        public string EventGuid { get; set; }
        public string OutputName { get; set; }
        public string EventId { get; set; }
        public string Description { get; set; }
        public string OnChangeValue { get; set; }
    }
}
