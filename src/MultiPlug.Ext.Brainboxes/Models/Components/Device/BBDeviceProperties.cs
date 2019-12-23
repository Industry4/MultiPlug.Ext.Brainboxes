using Brainboxes.IO;
using MultiPlug.Base;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.Brainboxes.Components.Device;
using System;
using System.Runtime.Serialization;


namespace MultiPlug.Ext.Brainboxes.Models.Components.Device
{
    public class BBDeviceProperties : MultiPlugBase
    {
        public event EventHandler<string> IPAddressUpdated;
        public event EventHandler<string> NameUpdated;
        public event EventHandler NameAndLocationUpdated;

        public EDDevice EDDevice { get; set; } = null;


        private string m_Name = string.Empty;
        private string m_Ip = string.Empty;
        private const int c_MaxNameLength = 10;

        public Subscription ConnectButtonSubscription;
        public Subscription RestartButtonSubscription;


        [DataMember]
        public EventExternal ConnectButtonEvent { get; set; }

        [DataMember]
        public EventExternal RestartButtonEvent { get; set; }

        [DataMember]
        public string Name
        {
            get
            {
                return m_Name;
            }

            set
            {
                if (!string.IsNullOrEmpty(value) && m_Name != value)
                {
                    m_Name = (value.Length > c_MaxNameLength) ? value.Substring(0, c_MaxNameLength) : value;
                    NameUpdated?.Invoke(this, m_Name);
                }
            }
        }

        [DataMember]
        public string IP
        {
            get
            {
                return m_Ip;
            }

            set
            {
                if (m_Ip != value)
                {
                    m_Ip = value;
                    MACFetch = MACResult.Unknown;
                    IPAddressUpdated?.Invoke(this, value);
                }
            }
        }

        [DataMember]
        public string Guid { get; set; } = string.Empty;
        [DataMember]
        public string MACAddress { get; set; } = string.Empty;
        public string Location { get; protected set; } = string.Empty;
        public string Firmware { get; protected set; } = string.Empty;
        public string ProductModel { get; protected set; } = string.Empty;

        [DataMember]
        public bool AutoConnect { get; set; }

        public MACResult MACFetch { get; set; } = MACResult.Unknown;

        [DataMember]
        public BBOutput[] Outputs { get; set; } = new BBOutput[0];

        [DataMember]
        public BBDeviceEvent[] IOEvents { get; set; } = new BBDeviceEvent[0];


        protected void Set( string theName, string theLocation)
        {
            Name = theName;
            Location = theLocation;
            NameAndLocationUpdated?.Invoke(this, EventArgs.Empty);
        }

    }
}
