using System.Collections.Generic;
using MultiPlug.Base.Http;
using MultiPlug.Extension.Core.Http;
using MultiPlug.Extension.Core.Attribute;
using MultiPlug.Ext.Brainboxes.Controllers.Assets.Js;
using MultiPlug.Ext.Brainboxes.Controllers.Assets.Images;
using MultiPlug.Ext.Brainboxes.Controllers.Assets.Css;

namespace MultiPlug.Ext.Brainboxes.Controllers.Assets
{
    [HttpEndpointType(HttpEndpointType.Assets)]
    class AssetsEndpoint : HttpEndpoint
    {
        private Controller[] m_Controllers = new Controller[]
        {
            new ImageController(),
            new JavaScriptsController(),
            new CssController()
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
