using System;
using System.Linq;
using System.Runtime.Serialization;

using MultiPlug.Base.Exchange;
using MultiPlug.Ext.Brainboxes.Models.Components.Discovery;

namespace MultiPlug.Ext.Brainboxes.Components.Discovery
{
    public class BBDiscovery : EventConsumer
    {
        public event EventHandler<BBDeviceFound> Discovered;

        public event EventHandler SubscriptionsChanged;
        public event EventHandler EventsChanged;

        [DataMember]
        public BBDiscoveryProperties Properties { get; internal set; } = new BBDiscoveryProperties();

        public BBDiscovery()
        {
            Properties.DiscoverySubscription = new Subscription
            {
                EventConsumer = this,
                Guid = Guid.NewGuid().ToString(),
                Id = ""
            };

            Properties.StartDeviceDiscovery = new EventExternal
            {
                Id = Guid.NewGuid().ToString(),
                Guid = Guid.NewGuid().ToString(),
                Description = "Starts the Device Discovery routine by a user action."
            };
        }

        public BBDiscovery( BBDiscoveryProperties theModel)
        {
            Properties = theModel;
            Properties.DiscoverySubscription.EventConsumer = this;
        }

        public void UpdateProperties( BBDiscoveryProperties theModel )
        {
            bool SubscriptionsChangedFlag = false;
            bool EventsChangedFlag = false;

            if( theModel.DiscoverySubscription != null)
            {
                if (Properties.DiscoverySubscription.Id != theModel.DiscoverySubscription.Id)
                {
                    SubscriptionsChangedFlag = true;
                    Properties.DiscoverySubscription.Id = theModel.DiscoverySubscription.Id;
                }
            }

            if( theModel.StartDeviceDiscovery != null)
            {
                if (Properties.StartDeviceDiscovery.Id != theModel.StartDeviceDiscovery.Id)
                {
                    EventsChangedFlag = true;
                    Properties.StartDeviceDiscovery.Id = theModel.StartDeviceDiscovery.Id;
                }
                if (Properties.StartDeviceDiscovery.Description != theModel.StartDeviceDiscovery.Description)
                {
                    EventsChangedFlag = true;
                    Properties.StartDeviceDiscovery.Description = theModel.StartDeviceDiscovery.Description;
                }
            }

            if( EventsChangedFlag && EventsChanged != null)
            {
                EventsChanged(null, null);
            }

            if (SubscriptionsChangedFlag && SubscriptionsChanged != null)
            {
                SubscriptionsChanged(null, null);
            }
        }

        private static object m_Lock = new object();

        public override void OnEvent(Payload e)
        {
            lock(m_Lock)
            {
                var Location = e.Pairs.FirstOrDefault(P => string.Equals(P.Type, "location", StringComparison.OrdinalIgnoreCase) );
                var MACAddress = e.Pairs.FirstOrDefault(P => string.Equals(P.Type, "SerialNumber", StringComparison.OrdinalIgnoreCase));

                if ( Location != null && MACAddress != null)
                {
                    string IpAddress = Location.Value.Substring(7);

                    IpAddress = IpAddress.Substring(0, IpAddress.IndexOf(':'));
                    Discovered?.Invoke(null, new BBDeviceFound { MAC = MACAddress.Value, IPAddress = IpAddress });
                }
            }
        }

        internal void BeginSearch()
        {
            Properties.StartDeviceDiscovery.Fire(new Payload(Properties.StartDeviceDiscovery.Id));
        }
    }
}
