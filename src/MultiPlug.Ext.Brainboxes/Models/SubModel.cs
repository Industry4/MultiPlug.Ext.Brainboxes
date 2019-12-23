using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlug.Ext.Brainboxes.Models
{
    [Serializable]
    public class SubModel
    {
        public string DeviceGuid { get; set; }
        public string SubGuid { get; set; }
        public string OutputName { get; set; }
        public string EventId { get; set; }

    }
}
