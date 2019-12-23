using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

using MultiPlug.Base.Exchange;
using MultiPlug.Base;
using MultiPlug.Ext.Brainboxes.Components.Device;
using MultiPlug.Ext.Brainboxes.Components.Discovery;
using MultiPlug.Ext.Brainboxes.Models.Components.Discovery;
using MultiPlug.Ext.Brainboxes.Components.Defaults;
using MultiPlug.Ext.Brainboxes.Models.Components.Device;
using MultiPlug.Ext.Brainboxes.Models.Components.Defaults;
using System.Threading.Tasks;
using System.Threading;

namespace MultiPlug.Ext.Brainboxes
{
    public class Core : MultiPlugBase
    {
        private static Core m_Instance = null;

        public static Core Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new Core();
                }
                return m_Instance;
            }
        }

        private Core()
        {
            Discovery.Discovered += UpdateByDiscovery; ;
            Discovery.EventsChanged += OnEventsUpdated;
            Discovery.SubscriptionsChanged += OnSubscriptionsUpdated;

            ConstructEvents();
            ConstructSubscriptions();
        }

        private void ConstructEvents()
        {
            var EventsList = new List<Event>();
            EventsList.Add(Logging.Instance.Event);
            EventsList.Add(m_Devices.Event);
            Array.ForEach(Devices, Device => EventsList.AddRange(Device.Events));
            EventsList.Add(Discovery.Properties.StartDeviceDiscovery);
            Events = EventsList.ToArray();
            EventsUpdated?.Invoke(this, Events);
        }

        private void ConstructSubscriptions()
        {
            var SubscriptionsList = new List<Subscription>();
            SubscriptionsList.Add(Discovery.Properties.DiscoverySubscription);
            Array.ForEach(Devices, Device => SubscriptionsList.AddRange(Device.Subscriptions));
            Subscriptions = SubscriptionsList.ToArray();
            SubscriptionsUpdated?.Invoke(this, Subscriptions);
        }

        public Event[] Events { get; private set; } = new Event[0];

        public Subscription[] Subscriptions { get; private set; } = new Subscription[0];

        private BBDevices m_Devices = new BBDevices();

        public string DevicesListEventId { get { return m_Devices.Event.Id; } }

        [DataMember]
        public BBDevice[] Devices { get { return m_Devices.Devices; } }
        public EventHandler<Event[]> EventsUpdated { get; internal set; }
        public EventHandler<Subscription[]> SubscriptionsUpdated { get; internal set; }

        [DataMember]
        public BBDiscovery Discovery { get; internal set; } = new BBDiscovery();

        [DataMember]
        public BBDefaults Defaults { get; internal set; } = new BBDefaults();

        private bool m_RaiseEvents = true;
        private bool m_SubscriptionsUpdatedEventRaised = false;
        private bool m_EventsUpdatedEventRaised = false;

        public void UpdateDiscoveryProperties(BBDiscoveryProperties theModel)
        {
            Discovery.UpdateProperties(theModel);
        }

        private void UpdateByDiscovery(object sender, BBDeviceFound e)
        {
            Update(new BBDeviceProperties[] { new BBDeviceProperties {  MACAddress = e.MAC, IP = e.IPAddress } });
        }

        private CancellationTokenSource TokenSource = new CancellationTokenSource();

        public void InitialiseDevices()
        {
            List<Task> TaskList = new List<Task>();

            Array.ForEach(Devices, d => TaskList.Add(d.Connect(TokenSource.Token)));

            Task.WhenAll(TaskList).Wait(TokenSource.Token);
        }

        public void Shutdown()
        {
            TokenSource.Cancel();

            Array.ForEach(Devices, d => d.DeleteAndTidyUp());
        }

        internal void Update(BBDeviceProperties[] theDeviceProperties)
        {
            m_SubscriptionsUpdatedEventRaised = false;
            m_EventsUpdatedEventRaised = false;

            m_RaiseEvents = false;

            foreach (var DeviceProperties in theDeviceProperties)
            {
                var DeviceSearch = Devices.FirstOrDefault(d => d.Guid == DeviceProperties.Guid);

                if( DeviceSearch != null)
                {
                    DeviceSearch.Update(DeviceProperties);
                }
                else
                {
                    DeviceSearch = Devices.FirstOrDefault(d => d.MACAddress == DeviceProperties.MACAddress);

                    if (DeviceSearch != null)
                    {
                        DeviceProperties.Guid = DeviceSearch.Guid;

                        DeviceSearch.Update(DeviceProperties);
                    }
                    else
                    {
                        if (Devices.FirstOrDefault(d => d.IP == DeviceProperties.IP || d.MACAddress == DeviceProperties.MACAddress) != null) // Duplicate Search
                        {
                            continue;
                        }

                        if (string.IsNullOrEmpty(DeviceProperties.Guid))
                        {
                            DeviceProperties.Guid = System.Guid.NewGuid().ToString();
                        }

                        var NewDevice = new BBDevice(DeviceProperties.Guid);

                        NewDevice.DeviceException += OnDeviceException;
                        NewDevice.EventsUpdated += OnEventsUpdated;
                        NewDevice.SubscriptionsUpdated += OnSubscriptionsUpdated;
                        NewDevice.Update(DeviceProperties);
                        m_Devices.Add(NewDevice);

                        m_SubscriptionsUpdatedEventRaised = true;
                        m_EventsUpdatedEventRaised = true;
                    }
                }
            }

            m_RaiseEvents = true;

            if(m_SubscriptionsUpdatedEventRaised)
            {
                OnSubscriptionsUpdated(this, EventArgs.Empty);
            }

            if(m_EventsUpdatedEventRaised)
            {
                OnEventsUpdated(this, EventArgs.Empty);
            }
        }

        internal void Update(BBDefaultProperties theProperties)
        {
            Defaults.Update(theProperties);
        }

        private void OnSubscriptionsUpdated(object sender, EventArgs e)
        {
            if(m_RaiseEvents)
            {
                ConstructSubscriptions();
            }
            else
            {
                m_SubscriptionsUpdatedEventRaised = true;
            }
        }

        private void OnEventsUpdated(object sender, EventArgs e)
        {
            if(m_RaiseEvents)
            {
                ConstructEvents();
            }
            else
            {
                m_EventsUpdatedEventRaised = true;
            }
        }

        internal void Update(Models.Settings.Device.Subscription theUpdatedSubscriptions)
        {
            var DeviceSearch = Core.Instance.Devices.FirstOrDefault(d => d.Guid == theUpdatedSubscriptions.DeviceId);

            if( DeviceSearch != null)
            {
                DeviceSearch.UpdateSubscriptions(theUpdatedSubscriptions);

            }
        }

        internal bool Delete(string theDeviceId)
        {
            bool Result = true;

            var DeviceSearch = Devices.FirstOrDefault(d => d.Guid == theDeviceId);

            if( DeviceSearch == null )
            {
                Result = false;
            }
            else
            {
                DeviceSearch.DeleteAndTidyUp();
                DeviceSearch.DeviceException -= OnDeviceException;
                DeviceSearch.EventsUpdated -= OnEventsUpdated;
                DeviceSearch.SubscriptionsUpdated -= OnSubscriptionsUpdated;
                m_Devices.Remove(DeviceSearch);

                ConstructEvents();
                ConstructSubscriptions();
            }

            return Result;
        }

        internal void Delete(Models.Settings.Device.Subscription theUpdatedSubscriptions)
        {
            bool SubChangesMade = false;

            var DeviceSearch = Core.Instance.Devices.FirstOrDefault(d => d.Guid == theUpdatedSubscriptions.DeviceId);

            if (DeviceSearch != null)
            {
                BBOutput OutputSearch = DeviceSearch.Outputs.FirstOrDefault(o => o.Name == theUpdatedSubscriptions.OutputId);

                if (OutputSearch != null )
                {
                    BBSubscription Subscription = OutputSearch.Subscriptions.FirstOrDefault(s => s.Guid == theUpdatedSubscriptions.SubscriptionGuid);

                    if( OutputSearch.Remove(Subscription) )
                    {
                        SubChangesMade = true;
                    }
                }
            }

            if (SubChangesMade)
            {
                ConstructSubscriptions();
            }
        }

        void OnDeviceException(object sender, string e)
        {
            Logging.Instance.WriteLine(e);
        }
    }
}
