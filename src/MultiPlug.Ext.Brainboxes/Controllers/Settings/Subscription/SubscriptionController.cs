using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.Brainboxes.Models.Components.Device;

namespace MultiPlug.Ext.Brainboxes.Controllers.Settings.Subscription
{
    [Route("subscription")]
    public class SubscriptionController : SettingsApp
    {
        public Response Get( string device, string output, string id )
        {
            if (string.IsNullOrEmpty( device ) || string.IsNullOrEmpty( output )  || string.IsNullOrEmpty(id))
            {
                return null;
            }

            bool CreateNew = string.Equals(id, "new", System.StringComparison.OrdinalIgnoreCase);

            BBSubscription Subscription = null;

            var DeviceSearch = Core.Instance.Devices.FirstOrDefault(d => d.Guid == device);

            if( DeviceSearch != null)
            {
                var OutputSearch = DeviceSearch.Outputs.FirstOrDefault(o => o.Name == output);

                if(OutputSearch != null)
                {
                    if( !CreateNew)
                    {
                        Subscription = OutputSearch.Subscriptions.FirstOrDefault(s => s.Guid == id);
                    }
                }
            }

            if(CreateNew)
            {
                return new Response
                {
                    Model = new Models.Settings.Device.Subscription
                    {
                        DeviceId = device,
                        OutputId = output,
                        SubscriptionGuid = "new",
                        SubscriptionId = "",
                        HighKey = "value",
                        HighValue = "1",
                        LowKey = "value",
                        LowValue = "0"
                    },
                    Template = "BrainboxesSubscription"
                };
            }



            if(Subscription == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }

            return new Response
            {
                Model = new Models.Settings.Device.Subscription
                {
                    DeviceId = device,
                    OutputId = output,
                    SubscriptionGuid = id,
                    SubscriptionId = Subscription.Id,
                    HighKey = Subscription.HighSubject,
                    HighValue = Subscription.HighValue,
                    LowKey = Subscription.LowSubject,
                    LowValue = Subscription.LowValue
                },
                Template = "BrainboxesSubscription"
            };

        }

        public Response Post(Models.Settings.Device.Subscription theModel)
        {

            Core.Instance.Update(theModel);

            var HomePage = Context.Referrer.AbsoluteUri;
            HomePage = HomePage.Remove(HomePage.LastIndexOf("subscription/"));
            HomePage = HomePage + "device/?id=" + theModel.DeviceId;

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = new System.Uri(HomePage)
            };
        }
    }
}
