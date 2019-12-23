using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.Brainboxes.Models;
using MultiPlug.Ext.Brainboxes.Models.Settings;
using MultiPlug.Ext.Brainboxes.Models.Settings.Device;

namespace MultiPlug.Ext.Brainboxes.Controllers.Settings.Home
{
    [Route("oldhome")]
    class OLDHomeController : Controller
    {
        public Response Get()
        {
            var devicesview = new List<Models.DeviceViewModel>();

            foreach (var Device in Core.Instance.Devices)
            {
                var ddevents = new List<ESViewModel>();
                var ddouts = new List<DeviceOutputViewModel>();

   //             Device.IOEvents.ForEach(e => ddevents.Add(new ESViewModel { EventId = e.Id, Guid = e.Guid, Description = e.Description }));
    //            Device.Outputs.ForEach(o => ddouts.Add(new DeviceOutputViewModel { Name = o.Name, Subscriptions = o.Subscriptions.Select(s => new ESViewModel { EventId = s.Id, Guid = s.Guid }).ToArray() }));

          //      devicesview.Add(new DeviceViewModel { Guid = Device.Guid, Events = ddevents, Outputs = ddouts, Name = Device.Name, Ip = Device.IP, EventIdConnectCmd = Device.ConnectButtonEvent.Id, Status = (Device.Status) ? "Connected" : "Not Connected" });
            }

            var EventsList = new List<Base.Exchange.EventExternal>();

            EventsList.Add(Core.Instance.Discovery.Properties.StartDeviceDiscovery);

        //    Core.Instance.Devices.ForEach(d => EventsList.Add(d.ConnectButtonEvent));

            return new Response
            {
                Model = new SettingsViewModel
                {
                    Devices = devicesview,
                    Log = Logging.Instance.ToString(),
                    LogEventId = Logging.Instance.Event.Id,
                    DiscoverDeviceId = Core.Instance.Discovery.Properties.StartDeviceDiscovery.Id,
                  //  DeviceDiscoveredId = Core.Instance.Discovery.Properties.DiscoveryEvent.Id
                },
                Template = "BrainboxesOldSettingsView",
                HeadMarkUp = "",
                Events = EventsList,
                Subscriptions = new Base.Exchange.Subscription[]
                {
            //        new Base.Exchange.Subscription { Id = Core.Instance.Discovery.Properties.DiscoveryEvent.Id, Guid = Guid.NewGuid().ToString() },
                    new Base.Exchange.Subscription { Id = Logging.Instance.Event.Id, Guid = Guid.NewGuid().ToString() }
                }
            };
        }

        public Response Post()
        {
            var UpdatedDevices = new List<Models.DeviceModel>();
            var UpdatedSubscriptions = new List<Models.SubModel>();
            var UpdatedEvents = new List<Models.EventModel>();

            var form = Context.FormData;

            var SquareBracketsPattern = @"\[(.*?)\]";
            var SquareBracketsMatchCollection = new List<KeyValuePair<System.Text.RegularExpressions.MatchCollection, string>>();

            foreach (var item in form)
            {
                var matches = Regex.Matches(item.Key, SquareBracketsPattern);

                if (matches.Count > 0)
                {
                    SquareBracketsMatchCollection.Add(new KeyValuePair<MatchCollection, string>(matches, item.Value));
                }
            }

            var Devices = SquareBracketsMatchCollection.FindAll(m => m.Key[1].Groups[1].Value == "deviceip");

            if (Devices != null)
            {
                Devices.ForEach(d => {

                    string DeviceName = "";

                    var FindDeviceName = SquareBracketsMatchCollection.Find(m => m.Key.Count > 0 &&
                                                            m.Key[0].Groups[1].Value == d.Key[0].Groups[1].Value &&
                                                            m.Key[1].Groups[1].Value == "devicename");

                    if (!FindDeviceName.Equals(default(KeyValuePair<System.Text.RegularExpressions.MatchCollection, string>)))
                    {
                        DeviceName = FindDeviceName.Value;
                    }

                    if (!(d.Key[0].Groups[1].Value.StartsWith("new-") && d.Value == "-delete-"))
                    {
                        UpdatedDevices.Add(new Models.DeviceModel { Guid = d.Key[0].Groups[1].Value, Name = DeviceName, Ip = d.Value });
                    }

                });
            }

            var Subs = SquareBracketsMatchCollection.FindAll(m => m.Key[2].Groups[1].Value == "subid");

            if (Subs != null)
            {
                Subs.ForEach(d => {
                    if (d.Key[3].Groups[1].Value.StartsWith("new-"))
                    {
                        if (d.Value != "-delete-")
                        {
                            UpdatedSubscriptions.Add
                            (new SubModel
                            {
                                SubGuid = Guid.NewGuid().ToString(),
                                DeviceGuid = d.Key[0].Groups[1].Value,
                                OutputName = d.Key[1].Groups[1].Value,
                                EventId = d.Value
                            });
                        }
                    }
                    else
                    {
                        UpdatedSubscriptions.Add
                        (new Models.SubModel
                        {
                            SubGuid = d.Key[3].Groups[1].Value,
                            DeviceGuid = d.Key[0].Groups[1].Value,
                            OutputName = d.Key[1].Groups[1].Value,
                            EventId = d.Value
                        });
                    }
                });
            }

            var Events = SquareBracketsMatchCollection.FindAll(m => m.Key[2].Groups[1].Value == "eventid");

            if (Events != null)
            {
                Events.ForEach(e =>
                {
                    var FindDescription = SquareBracketsMatchCollection.Find(m => m.Key.Count > 0 &&
                                                                                m.Key[0].Groups[1].Value == e.Key[0].Groups[1].Value &&
                                                                                m.Key[1].Groups[1].Value == e.Key[1].Groups[1].Value &&
                                                                                m.Key[2].Groups[1].Value == "description");

                    if (!FindDescription.Equals(default(KeyValuePair<System.Text.RegularExpressions.MatchCollection, string>)))
                    {
                        UpdatedEvents.Add(new Models.EventModel
                        {
                            EventGuid = e.Key[1].Groups[1].Value,
                            DeviceGuid = e.Key[0].Groups[1].Value,
                            OutputName = e.Key[1].Groups[1].Value,
                            EventId = e.Value,
                            Description = FindDescription.Value
                        });
                    }
                });
            }

//            Core.Instance.Update(UpdatedSubscriptions);
 //           Core.Instance.Update(UpdatedEvents);
//            Core.Instance.Update(UpdatedDevices);

            return new Response
            {
                Location = Context.Request,
                StatusCode = System.Net.HttpStatusCode.Moved
            };
        }

    }
}
