using MultiPlug.Base.Http;
using MultiPlug.Extension.Core.Attribute;

namespace MultiPlug.Ext.Brainboxes.Controllers.Settings
{
    [Name("Brainboxes")]
    [HttpEndpointType(HttpEndpointType.Settings)]
    [ViewAs(ViewAs.Partial)]
    public class SettingsApp : Controller
    {
    }
}
