using System;
using System.Linq;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.Brainboxes.Models.Components.Device;
using MultiPlug.Ext.Brainboxes.Models.Settings.Device;

namespace MultiPlug.Ext.Brainboxes.Components.Device
{
    public class BBDeviceConnectStatus : EventableBase
    {
        StatusEnum m_Status = StatusEnum.Disconnected;

        public override event MPEventHandler Update;

        public Event Event { get; private set; }

        public event EventHandler<StatusEnum> StatusUpdate;

        private BBDeviceProperties m_Properties;

        public BBDeviceConnectStatus(BBDeviceProperties theProperties)
        {
            m_Properties = theProperties;
            var EventGuid = Guid.NewGuid().ToString();
            Event = new Event { Guid = EventGuid, Id = EventGuid, Description = "Device Status", Object = this, Keys = new string[]{ "Status" } };
        }

        public override Payload CachedValue()
        {
            return new Payload(Event.Id, new Pair[] { new Pair(Event.Keys[0], construct()) });
        }

        public StatusEnum Status
        {
            get { return m_Status; }
            set
            {
                if ( value != m_Status)
                {
                    m_Status = value;
                    Update?.Invoke(new Payload(Event.Id, new Pair[] { new Pair(Event.Keys[0], construct()) }));
                    StatusUpdate?.Invoke(this, m_Status);
                }
            }
        }

        public override string ToString()
        {
            switch (Status)
            {
                case StatusEnum.Connected:
                    return "Connected";
                case StatusEnum.Connecting:
                    return "Connecting";
                case StatusEnum.Errored:
                    return "Errored";
                case StatusEnum.Disconnected:
                    return "Disconnected";
            }

            return "";
        }

        private Models.Settings.Device.Device construct()
        {
            string ConnectButtonText = "Disconnect";

            switch (Status)
            {
                case StatusEnum.Disconnected:
                    ConnectButtonText = "Connect";
                    break;
                case StatusEnum.Errored:
                    ConnectButtonText = "Connect";
                    break;
            }

            return new Models.Settings.Device.Device
            {
                Guid = m_Properties.Guid,
                Name = m_Properties.Name,
                IPAddress = m_Properties.IP,
                MACAddress = m_Properties.MACAddress,
                Location = m_Properties.Location,
                Firmware = m_Properties.Firmware,
                ProductModel = m_Properties.ProductModel,
                Status = ToString(),
                // Log = DeviceSearch.Logging.LogRead(),
                // LogEventId = DeviceSearch.Logging.Event.Id,
                ConnectButtonText = ConnectButtonText,
                // ConnectButtonEventId = DeviceSearch.ConnectButtonEvent.Id,
                // RestartButtonEventId = DeviceSearch.RestartButtonEvent.Id,
                // DeviceStatusEventId = DeviceSearch.Connection.Status.Event.Id,
                Events = m_Properties.IOEvents.Select(e => new ESViewModel { IOid = e.IONumber.ToString(), EventId = e.Id, Guid = e.Guid, Description = e.Description, RisingEdge = e.RisingEdgeValue, FallingEdge = e.FallingEdgeValue }).ToArray(),
                Outputs = m_Properties.Outputs.Select(o => new DeviceOutputViewModel { Name = o.Name, Subscriptions = o.Subscriptions.Select(s => new ESViewModel { EventId = s.Id, Guid = s.Guid }).ToArray() }).ToArray()
            };


        }
    }
}
