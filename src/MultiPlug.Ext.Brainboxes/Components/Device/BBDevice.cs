using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

using MultiPlug.Base.Exchange;
using MultiPlug.Ext.Brainboxes.Models;
using MultiPlug.Ext.Brainboxes.Models.Components.Device;
using MultiPlug.Base.Exchange.API;
using MultiPlug.Ext.Brainboxes.Diagnostics;

namespace MultiPlug.Ext.Brainboxes.Components.Device
{
    public class BBDevice : BBDeviceProperties
    {
        public event EventHandler EventsUpdated;
        public event EventHandler SubscriptionsUpdated;
        public event Action DeviceInformationFetchError;

        public BBDeviceConnect Connection { get; private set; }
        private BBDeviceRestart m_DeviceRestart;

        private Event[] m_Events = new Event[0];
        private Subscription[] m_Subscriptions = new Subscription[0];

        private BBDeviceInformation m_DeviceInformation = new BBDeviceInformation();

        private ILoggingService m_LoggingService;

        public string LogEventId { get { return m_LoggingService.EventId; } }

        public BBDevice(string theGuid, ILoggingService theLoggingService)
        {
            Guid = theGuid;

            m_LoggingService = theLoggingService;

            Connection = new BBDeviceConnect(this);
            Connection.Log += OnLogWriteEntry;
            Connection.Connected += OnDeviceConnected;
            m_DeviceRestart = new BBDeviceRestart(this);


            IPAddressUpdated += Connection.OnIPAddressUpdated;
            NameUpdated += Connection.OnNameUpdated;
            IPAddressUpdated += m_DeviceInformation.BeginFetch;
            m_DeviceInformation.FetchCompleted += OnDeviceInformationFetchCompleted;
            m_DeviceInformation.Log += OnLogWriteEntry;
            m_DeviceInformation.FetchError += () => DeviceInformationFetchError?.Invoke();

            string FixID = System.Guid.NewGuid().ToString();

            ConnectButtonEvent = new EventExternal { Id = FixID, Guid = System.Guid.NewGuid().ToString(), Description = "System Event: Connect to device" };
            ConnectButtonSubscription = new Subscription { Id = FixID, Guid = System.Guid.NewGuid().ToString(), EventConsumer = Connection };

            FixID = System.Guid.NewGuid().ToString();

            RestartButtonEvent = new EventExternal { Id = FixID, Guid = System.Guid.NewGuid().ToString(), Description = "System Event: Restart the device" };
            RestartButtonSubscription = new Subscription { Id = FixID, Guid = System.Guid.NewGuid().ToString(), EventConsumer = m_DeviceRestart };

            ConstructSubscriptions();
            ConstructEvents();
        }

        public void Update(BBDeviceProperties theProperties)
        {
            bool FlagSubscriptionUpdated = false;
            bool FlagEventUpdated = false;

            if ( theProperties == null )
            {
                return;
            }

            if( theProperties.Guid != null && theProperties.Guid != Guid )
            {
                return;
            }

            // Don't set this to false, as it may not be set in the first place
            if( theProperties.AutoConnect ) 
            {
                AutoConnect = true;
            }

            if( theProperties.ConnectButtonEvent != null )
            {
                if ( Event.Merge( ConnectButtonEvent, theProperties.ConnectButtonEvent ) )
                {
                    FlagEventUpdated = true;
                    ConnectButtonSubscription.Id = theProperties.ConnectButtonEvent.Id;
                    FlagSubscriptionUpdated = true;
                }
            }

            if( theProperties.RestartButtonEvent != null )
            {
                if ( Event.Merge( RestartButtonEvent, theProperties.RestartButtonEvent ) )
                {
                    FlagEventUpdated = true;
                    RestartButtonSubscription.Id = theProperties.RestartButtonEvent.Id;
                    FlagSubscriptionUpdated = true;
                }
            }

            if (theProperties.Name != null && theProperties.Name != Name)
            {
                Name = theProperties.Name;
            }

            if (theProperties.IP != null && theProperties.IP != IP )
            {
                IPAddress address;
                if( IPAddress.TryParse(theProperties.IP, out address) )
                {
                    IP = theProperties.IP;
                }
            }

            if ( ! string.IsNullOrEmpty(theProperties.MACAddress))
            {
                MACAddress = theProperties.MACAddress;
            }

            if ( theProperties.IOEvents != null)
            {
                List<BBDeviceEvent> IOEventsList = new List<BBDeviceEvent>(IOEvents);

                foreach( var NewIOEvent in theProperties.IOEvents)
                {
                    BBDeviceEvent Search = IOEventsList.FirstOrDefault(IOEvent => IOEvent.IONumber == NewIOEvent.IONumber);

                    if( Search != null)
                    {
                        if( Search.Id != NewIOEvent.Id)
                        {
                            Search.Id = NewIOEvent.Id;
                            FlagEventUpdated = true;
                        }

                        if (Search.Description != NewIOEvent.Description)
                        {
                            Search.Description = NewIOEvent.Description;
                            FlagEventUpdated = true;
                        }

                        if( ! string.IsNullOrEmpty( NewIOEvent.RisingEdgeValue ) )
                        {
                            Search.RisingEdgeValue = NewIOEvent.RisingEdgeValue;
                        }

                        if (!string.IsNullOrEmpty(NewIOEvent.FallingEdgeValue))
                        {
                            Search.FallingEdgeValue = NewIOEvent.FallingEdgeValue;
                        }
                    }
                    else
                    {
                     //   NewIOEvent.Object = new BBEventFire(NewIOEvent.Id, NewIOEvent.Keys[0], NewIOEvent.RisingEdgeValue, NewIOEvent.FallingEdgeValue);
                        IOEventsList.Add(NewIOEvent);
                    }

                }

                if(FlagEventUpdated)
                {
                    IOEvents = IOEventsList.ToArray();
                }
            }

            if ( theProperties.Outputs != null && theProperties.Outputs.Any())
            {
                Outputs = theProperties.Outputs.Select(o =>
                {
                    var Output = new BBOutput(o.Name);

                    if (o.Subscriptions != null)
                    {
                        Output.Update(new BBOutputProperties { Name = o.Name, Subscriptions = o.Subscriptions });
                    }

                    return Output;

                }).ToArray();

                FlagSubscriptionUpdated = true;
            }

            if (FlagSubscriptionUpdated)
            {
                ConstructSubscriptions();
            }
            if (FlagEventUpdated)
            {
                ConstructEvents();
            }
        }

        internal void Start()
        {
            Array.ForEach(IOEvents, IOEvent => IOEvent.FireOnStart());
        }

        private void OnDeviceConnected()
        {
            ConstructSubscriptions();
            ConstructEvents();
        }

        private void ConstructEvents()
        {
            var list = new List<Event>();
            list.Add(ConnectButtonEvent);
            list.Add(RestartButtonEvent);
            list.Add(Connection.Status.Event);
            list.AddRange(IOEvents);
            list.AddRange(Outputs.Select(o => o.UIToggleEvent));
            m_Events = list.ToArray();
            EventsUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void ConstructSubscriptions()
        {
            var list = new List<Subscription>();
            list.Add(ConnectButtonSubscription);
            list.Add(RestartButtonSubscription);
            list.AddRange(Outputs.SelectMany(o => o.Subscriptions));
            m_Subscriptions = list.ToArray();
            SubscriptionsUpdated?.Invoke(this, EventArgs.Empty);
        }

        public Event[] Events
        {
            get
            {
                return m_Events;
            }
        }

        public Subscription[] Subscriptions
        {
            get
            {
                return m_Subscriptions;
            }
        }

        private void OnLogWriteEntry(EventLogEntryCodes theLogCode, string[] theArg)
        {
            m_LoggingService.WriteEntry((uint)theLogCode, theArg);
        }

        public void DeleteAndTidyUp()
        {
            m_DeviceInformation.Cancel();
            Connection.Disconnect();
        }

        public Task Connect(CancellationToken theCancellationToken)
        {
            return Task.Run(() =>
            {
                while (!theCancellationToken.IsCancellationRequested)
                {
                    if( !AutoConnect )
                    {
                        break;
                    }
                    else if ( AutoConnect && MACFetch == MACResult.Matched )
                    {
                        Connection.Connect( true );
                        break;
                    }
                    else
                    {
                        Task.Delay(2000).Wait();
                    }
                }
            });
        }

        private void OnDeviceInformationFetchCompleted(DeviceInformation theDeviceInformation)
        {
            if (string.IsNullOrEmpty(MACAddress))
            {
                MACAddress = theDeviceInformation.MACAddress;
                if (string.IsNullOrEmpty(Name))
                {
                    Set(theDeviceInformation.Name, theDeviceInformation.Location);
                }
                else
                {
                    Set(Name, theDeviceInformation.Location);
                }
                Firmware = theDeviceInformation.Firmware;
                ProductModel = theDeviceInformation.ProductModel;
                MACFetch = MACResult.Matched;
                OnLogWriteEntry(EventLogEntryCodes.MACAddressSet, new string[] { IP, MACAddress });
            }
            else
            {
                if (theDeviceInformation.MACAddress.Replace(":", string.Empty).Equals(MACAddress.Replace(":", string.Empty), StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(Name))
                    {
                        Set(theDeviceInformation.Name, theDeviceInformation.Location);
                    }
                    else
                    {
                        Set(Name, theDeviceInformation.Location);
                    }
                    Firmware = theDeviceInformation.Firmware;
                    ProductModel = theDeviceInformation.ProductModel;

                    MACFetch = MACResult.Matched;
                }
                else
                {
                    MACFetch = MACResult.Unmatched;
                    OnLogWriteEntry(EventLogEntryCodes.MACMissMatch, new string[] { IP, MACAddress, theDeviceInformation.MACAddress });
                }
            }
        }

        public BBDeviceConnectStatus Status
        {
            get
            {
                return Connection.Status;
            }
        }

        internal void UpdateSubscriptions(Models.Settings.Device.Subscription theUpdatedSubscriptions)
        {
            bool SubChangesMade = false;

            BBSubscription Subscription = null;

            bool CreateNew = string.Equals(theUpdatedSubscriptions.SubscriptionGuid, "new", System.StringComparison.OrdinalIgnoreCase);

            BBOutput OutputSearch = null;

            OutputSearch = Outputs.FirstOrDefault(o => o.Name == theUpdatedSubscriptions.OutputId);

            if (OutputSearch != null && !CreateNew)
            {
                Subscription = OutputSearch.Subscriptions.FirstOrDefault(s => s.Guid == theUpdatedSubscriptions.SubscriptionGuid);
            }

            if (CreateNew && OutputSearch != null)
            {
                OutputSearch.Add(theUpdatedSubscriptions.SubscriptionId, new BBSubscriptionProperties
                {
                    HighSubject = theUpdatedSubscriptions.HighKey,
                    HighValue = theUpdatedSubscriptions.HighValue,
                    LowSubject = theUpdatedSubscriptions.LowKey,
                    LowValue = theUpdatedSubscriptions.LowValue
                });

                SubChangesMade = true;
            }
            else
            {
                if (Subscription != null)
                {
                    if (Subscription.Id != theUpdatedSubscriptions.SubscriptionId)
                    {
                        SubChangesMade = true;
                        Subscription.Id = theUpdatedSubscriptions.SubscriptionId;
                    }


                    Subscription.Update(new BBSubscriptionProperties
                    {
                        HighSubject = theUpdatedSubscriptions.HighKey,
                        HighValue = theUpdatedSubscriptions.HighValue,
                        LowSubject = theUpdatedSubscriptions.LowKey,
                        LowValue = theUpdatedSubscriptions.LowValue
                    });
                }
            }

            if (SubChangesMade)
            {
                ConstructSubscriptions();
            }
        }
    }
}
