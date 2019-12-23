using System;
using System.Linq;
using System.Collections.Generic;

using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.Brainboxes.Models;
using MultiPlug.Ext.Brainboxes.Models.Settings.Device;
using MultiPlug.Ext.Brainboxes.Models.Components.Device;
using MultiPlug.Base.Exchange;

namespace MultiPlug.Ext.Brainboxes.Controllers.Settings.Device
{
    [Route("device")]
    class DeviceController : Controller
    {
        public Response Get(string id)
        {
            var DeviceSearch = Core.Instance.Devices.FirstOrDefault(Device => Device.Guid == id);

            if( DeviceSearch == null)
            {
                return new Response
                {
                    Model = new NotFound(),
                    Template = "BrainboxesDeviceNotFound"
                };
            }

            var EventsList = new List<Base.Exchange.EventExternal>();

            EventsList.Add(Core.Instance.Discovery.Properties.StartDeviceDiscovery);

            Array.ForEach(Core.Instance.Devices, d => EventsList.Add(d.ConnectButtonEvent));

            string ConnectButtonText = "Disconnect";

            switch(DeviceSearch.Status.Status)
            {
                case Components.Device.StatusEnum.Disconnected:
                    ConnectButtonText = "Connect";
                    break;
                case Components.Device.StatusEnum.Errored:
                    ConnectButtonText = "Connect";
                    break;
            }

            List<EventExternal> EventExternals = DeviceSearch.Outputs.Select(o => o.UIToggleEvent).ToList();
            EventExternals.Add(DeviceSearch.ConnectButtonEvent);
            EventExternals.Add(DeviceSearch.RestartButtonEvent);

            List<Base.Exchange.Subscription> Subs = DeviceSearch.IOEvents.Select(e => new Base.Exchange.Subscription { Id = e.Id, Guid = Guid.NewGuid().ToString() }).ToList();

            Subs.Add(new Base.Exchange.Subscription { Id = DeviceSearch.Logging.Event.Id, Guid = Guid.NewGuid().ToString() });
            Subs.Add(new Base.Exchange.Subscription { Id = DeviceSearch.Connection.Status.Event.Id, Guid = Guid.NewGuid().ToString() });


            return new Response
            {
                Model = new Models.Settings.Device.Device
                {
                    Guid = id,
                    Name = DeviceSearch.Name,
                    IPAddress = DeviceSearch.IP,
                    MACAddress = DeviceSearch.MACAddress,
                    Location = DeviceSearch.Location,
                    Firmware = DeviceSearch.Firmware,
                    ProductModel = DeviceSearch.ProductModel,
                    Status = DeviceSearch.Status.ToString(),
                    Log = DeviceSearch.Logging.LogRead(),
                    LogEventId = DeviceSearch.Logging.Event.Id,
                    ConnectButtonText = ConnectButtonText,
                    ConnectButtonEventId = DeviceSearch.ConnectButtonEvent.Id,
                    RestartButtonEventId = DeviceSearch.RestartButtonEvent.Id,
                    DeviceStatusEventId = DeviceSearch.Connection.Status.Event.Id,
                    Events = DeviceSearch.IOEvents.Select(Event => new ESViewModel { IOid = Event.IONumber.ToString() ,EventId = Event.Id, Guid = Event.Guid, Description = Event.Description, RisingEdge = Event.RisingEdgeValue, FallingEdge = Event.FallingEdgeValue, CachedValue = Event.Object.CachedValue() }).ToArray(),
                    Outputs = DeviceSearch.Outputs.Select(Output => new DeviceOutputViewModel { Name = Output.Name, Value = Output.Value, UIToggleEventID = Output.UIToggleEvent.Id, Subscriptions = Output.Subscriptions.Select(s => new ESViewModel { EventId = s.Id, Guid = s.Guid }).ToArray() }).ToArray()
                },
                Template = "BrainboxesDeviceView",
                HeadMarkUp = "<link rel=\"stylesheet\" href=\"assets/brainboxes/css/toggleswitch.css\"> <link rel=\"stylesheet\" href=\"assets/brainboxes/css/IOStateColours.css\">",
                Events = EventExternals.ToArray(),
                Subscriptions = Subs.ToArray()
            };
        }

        public Response Post(DevicePost theModel)
        {
            List<BBDeviceProperties> UpdatedDevices = new List<BBDeviceProperties>();
            List<BBDeviceEvent> Events = new List<BBDeviceEvent>();
            if(theModel.EventId != null)
            {
                for (int i = 0; i < theModel.EventId.Length; i++)
                {
                    int io = 0;
                    if (Int32.TryParse(theModel.IONumber[i], out io))
                    {
                        Events.Add( new BBDeviceEvent
                        {
                            IONumber = io,
                            Id = theModel.EventId[i],
                            Description = theModel.EventDescription[i],
                            RisingEdgeValue = theModel.High[i],
                            FallingEdgeValue = theModel.Low[i]
                        });
                    }
                }
            }

            if (theModel != null && theModel.Guid != null && theModel.Name != null)
            {
                UpdatedDevices.Add(new BBDeviceProperties { Guid = theModel.Guid, IP = theModel.IPAddress, Name = theModel.Name, IOEvents = Events.ToArray() });
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
