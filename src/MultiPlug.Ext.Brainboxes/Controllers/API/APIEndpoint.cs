using System.Collections.Generic;
using MultiPlug.Base.Http;
using MultiPlug.Ext.Brainboxes.Controllers.API.Control;
using MultiPlug.Extension.Core.Attribute;
using MultiPlug.Extension.Core.Http;
using MultiPlug.Ext.Brainboxes.Controllers.API.Devices;

namespace MultiPlug.Ext.Brainboxes.Controllers.API
{
    [HttpEndpointType(HttpEndpointType.Api)]
    class APIEndpoint : HttpEndpoint
    {
        private Controller[] m_Controllers = new Controller[]
        {
            new DevicesController(),
            new ControlController()
        };

        public override IEnumerable<Controller> Controllers
        {
            get
            {
                return m_Controllers;
            }
        }
    }
}
