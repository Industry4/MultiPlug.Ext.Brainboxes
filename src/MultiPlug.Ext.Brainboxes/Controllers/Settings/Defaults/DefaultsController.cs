using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.Brainboxes.Models.Components.Defaults;
using MultiPlug.Ext.Brainboxes.Models.Settings.Defaults;

namespace MultiPlug.Ext.Brainboxes.Controllers.Settings.Defaults
{
    [Route("defaults")]
    class DefaultsController : Controller
    {
        public Response Get()
        {
            return new Response
            {
                Model = new DefaultsValues
                {
                    Key = Core.Instance.Defaults.Key,
                    FallingEdge = Core.Instance.Defaults.FallingEdge,
                    RisingEdge = Core.Instance.Defaults.RisingEdge
                },
                Template = "BrainboxesDeviceDefaults",
            };
        }

        public Response Post(DefaultsValues theModel)
        {
            Core.Instance.Update(new BBDefaultProperties
            {
                Key = theModel.Key,
                FallingEdge = theModel.FallingEdge,
                RisingEdge = theModel.RisingEdge
            });

            return new Response
            {
                Location = Context.Request,
                StatusCode = System.Net.HttpStatusCode.Moved
            };
        }
    }
}
