using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;

namespace MultiPlug.Ext.Brainboxes.Controllers.Settings.Device
{
    [Route("device/delete")]
    public class DeviceDeleteController : SettingsApp
    {
        public Response Post(string id)
        {
            if( ! Core.Instance.Delete(id) )
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = Context.Referrer
            };
        }
    }
}
