using System;
using System.Linq;
using System.Collections.Generic;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.Brainboxes.Models.Settings;
using MultiPlug.Ext.Brainboxes.Models.Components;
using MultiPlug.Ext.Brainboxes.Models.Components.Device;
using MultiPlug.Ext.Brainboxes.Models.Settings.Home;

namespace MultiPlug.Ext.Brainboxes.Controllers.Settings.Home
{
    [Route("")]
    public class HomeController : SettingsApp
    {
        public Response Get()
        {


            return new Response
            {
                Model = new Models.Settings.Home.Home
                {
                    Devices = Core.Instance.Devices.Select(Device => new HomeDevice { Guid = Device.Guid, IPAddress = Device.IP, Name = Device.Name, Location = Device.Location, Status = Device.Status.ToString() }).ToArray(),
                    DevicesListEventId = Core.Instance.DevicesListEventId,
                    DiscoverDeviceEventId = Core.Instance.Discovery.Properties.StartDeviceDiscovery.Id
                },
                Template = "BrainboxesSettingsView",
                //HeadMarkUp = "",
                Events = new Base.Exchange.EventExternal[]
                {
                    Core.Instance.Discovery.Properties.StartDeviceDiscovery
                },
                Subscriptions = new Base.Exchange.Subscription[]
                {
                    new Base.Exchange.Subscription { Id = Core.Instance.DevicesListEventId, Guid = Guid.NewGuid().ToString() }
                }
            };
        }

        public Response Post(HomePost theModel)
        {
            List<BBDeviceProperties> UpdatedDevices = new List<BBDeviceProperties>();

            if( theModel != null && theModel.IPAddress != null)
            {

                for(int i = 0; i < theModel.IPAddress.Length; i++)
                {
                    UpdatedDevices.Add(new BBDeviceProperties { Guid = string.Empty, Name = theModel.Name[i], IP = theModel.IPAddress[i] });
                }
            }

            Core.Instance.Update(UpdatedDevices.ToArray());

            return new Response
            {
                Location = Context.Request,
                StatusCode = System.Net.HttpStatusCode.Moved
            };
        }
    }
}
