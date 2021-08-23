using System;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.Brainboxes.Models.Settings.Discovery;
using MultiPlug.Ext.Brainboxes.Models.Components.Discovery;

namespace MultiPlug.Ext.Brainboxes.Controllers.Settings
{
    [Route("Discovery")]
    public class DiscoveryController : SettingsApp
    {
        public Response Get()
        {
            return new Response
            {
                Model = new DiscoveryViewModel
                {
                    DiscoverySubscription = Core.Instance.Discovery.Properties.DiscoverySubscription.Id,
                    StartDeviceDiscoveryEventId = Core.Instance.Discovery.Properties.StartDeviceDiscovery.Id,
                    StartDeviceDiscoveryEventDescription = Core.Instance.Discovery.Properties.StartDeviceDiscovery.Description,
                },
                Template = "BrainboxesDiscoveryView",
                HeadMarkUp = "",
                Events = null,
                Subscriptions = null
            };
        }

        public Response Post(DiscoveryViewModel theModel)
        {
            Core.Instance.Discovery.UpdateProperties(
                new BBDiscoveryProperties
                {
                    DiscoverySubscription = new Base.Exchange.Subscription { Id = theModel.DiscoverySubscription },
                    StartDeviceDiscovery = new Base.Exchange.EventExternal { Id = theModel.StartDeviceDiscoveryEventId, Description = theModel.StartDeviceDiscoveryEventDescription }
                }
            );

            return new Response
            {
                Location = new Uri(Context.Referrer.ToString().Replace("discovery/", "")),
                StatusCode = System.Net.HttpStatusCode.Redirect
            };
        }
    }
}
