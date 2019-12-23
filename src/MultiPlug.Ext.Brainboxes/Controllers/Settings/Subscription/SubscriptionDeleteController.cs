using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;

namespace MultiPlug.Ext.Brainboxes.Controllers.Settings.Subscription
{
    [Route("subscription/delete")]
    class SubscriptionDeleteController : Controller
    {
        public Response Get(string device, string output, string id)
        {
            if (string.IsNullOrEmpty(device) || string.IsNullOrEmpty(output) || string.IsNullOrEmpty(id))
            {
                return null;
            }

            Core.Instance.Delete(new Models.Settings.Device.Subscription
            {
                DeviceId = device,
                OutputId = output,
                SubscriptionGuid = id
            });

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Redirect,
                Location = Context.Referrer
            };
        }
    }
}
