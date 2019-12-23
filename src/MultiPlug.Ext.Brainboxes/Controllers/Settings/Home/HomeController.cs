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
    class HomeController : Controller
    {
        public Response Get()
        {
            //var devicesview = new List<Models.DeviceViewModel>();

            //foreach (var Device in Core.Instance.Devices)
            //{
            //    var ddevents = new List<ESViewModel>();
            //    var ddouts = new List<DeviceOutputViewModel>();

            //    Device.IOEvents.ForEach(e => ddevents.Add(new ESViewModel { EventId = e.Id, Guid = e.Guid, Description = e.Description }));
            //    Device.Outputs.ForEach(o => ddouts.Add(new DeviceOutputViewModel { Name = o.Name, Subscriptions = o.Subscriptions.Select(s => new ESViewModel { EventId = s.Id, Guid = s.Guid }).ToList() }));

            //    devicesview.Add(new DeviceViewModel { Guid = Device.Guid, Events = ddevents, Outputs = ddouts, Name = Device.Name, Ip = Device.IP, EventIdConnectCmd = Device.ConnectButtonEvent.Id, Status = (Device.IsConnected) ? "Connected" : "Not Connected" });
            //}

            //var EventsList = new List<Base.Exchange.EventExternal>();

            //EventsList.Add();

            //Core.Instance.Devices.ForEach(d => EventsList.Add(d.ConnectButtonEvent));

            return new Response
            {
                Model = new Models.Settings.Home.Home
                {
                    Devices = Core.Instance.Devices.Select(Device => new HomeDevice { Guid = Device.Guid, IPAddress = Device.IP, Name = Device.Name, Location = Device.Location, Status = Device.Status.ToString() }).ToArray(),
                    //Devices = devicesview,
                    Log = Logging.Instance.ToString(),
                    LogEventId = Logging.Instance.Event.Id,
                    DevicesListEventId = Core.Instance.DevicesListEventId,
                    DiscoverDeviceEventId = Core.Instance.Discovery.Properties.StartDeviceDiscovery.Id,
                    //DiscoverDeviceId = Core.Instance.Discovery.Properties.StartDeviceDiscovery.Id,
                    //DeviceDiscoveredId = Core.Instance.Discovery.Properties.DiscoveryEvent.Id
                },
                Template = "BrainboxesSettingsView",
                //HeadMarkUp = "",
                Events = new Base.Exchange.EventExternal[]
                {
                    Core.Instance.Discovery.Properties.StartDeviceDiscovery
                },
                Subscriptions = new Base.Exchange.Subscription[]
                {
                    new Base.Exchange.Subscription { Id = Logging.Instance.Event.Id, Guid = Guid.NewGuid().ToString() },
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
