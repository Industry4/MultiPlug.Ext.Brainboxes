using System;
using System.Runtime.Serialization;

using MultiPlug.Base.Exchange;
using MultiPlug.Ext.Brainboxes.Models;
using System.Linq;
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

        //private FireHelper m_Fire = new FireHelper();

        public BBDiscovery()
        {
            Properties.DiscoverySubscription = new Subscription
            {
                EventConsumer = this,
                Guid = Guid.NewGuid().ToString(),
                Id = ""
            };

            //Properties.DiscoveryEvent = new Event
            //{
            //    Description = "UI Event: New Brainboxes Device Discovered.",
            //    Guid = Guid.NewGuid().ToString(),
            //    Id = Guid.NewGuid().ToString(),
            //    Object = m_Fire
            //};

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
            //Properties.DiscoveryEvent.Object = m_Fire;
        }

        public void UpdateProperties( BBDiscoveryProperties theModel )
        {
            bool SubscriptionsChangedFlag = false;
            bool EventsChangedFlag = false;

            //if( theModel.DiscoveryEvent != null)
            //{
            //    if( Properties.DiscoveryEvent.Id != theModel.DiscoveryEvent.Id)
            //    {
            //        EventsChangedFlag = true;
            //        Properties.DiscoveryEvent.Id = theModel.DiscoveryEvent.Id;
            //    }
            //    if (Properties.DiscoveryEvent.Description != theModel.DiscoveryEvent.Description)
            //    {
            //        EventsChangedFlag = true;
            //        Properties.DiscoveryEvent.Description = theModel.DiscoveryEvent.Description;
            //    }
            //}

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

        //private class FireHelper : EventableBase
        //{
        //    public override event MPEventHandler Update;

        //    public void Fire(string id, string guid, string ip)
        //    {
        //        Update?.Invoke(new Payload
        //        (
        //            id,
        //            new Pair[]
        //            {
        //                new Pair( "guid", ip ),
        //                new Pair( "ip", ip )
        //                //new Pair {Type="name", Value = name },
        //                //new Pair {Type="autoconnect", Value = autoconnect }
        //            }
        //        ));
        //    }
        //    public override Payload CachedValue()
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //internal void isNew(string IpAddress, string theGuid)
        //{
        //    m_Fire.Fire(Properties.DiscoveryEvent.Id, theGuid, IpAddress);
        //}
    }
}
