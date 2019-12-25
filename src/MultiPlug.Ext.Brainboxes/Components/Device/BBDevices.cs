using System;
using System.Collections.Generic;
using System.Linq;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.Brainboxes.Models.Settings.Home;

namespace MultiPlug.Ext.Brainboxes.Components.Device
{
    class BBDevices : EventableBase
    {
        private static object m_ArrayLock = new object();

        public Event Event { get; private set; }

        public BBDevice[] Devices { get; private set; } = new BBDevice[0];

        public BBDevices()
        {
            var EventGuid = Guid.NewGuid().ToString();
            Event = new Event { Guid = EventGuid, Id = EventGuid, Description = "Devices List", Object = this };
        }


        public override Payload CachedValue()
        {
            throw new NotImplementedException();
        }

        public void Add(BBDevice theDevice)
        {
            lock (m_ArrayLock)
            {
                var DeviceList = new List<BBDevice>(Devices);
                DeviceList.Add(theDevice);
                Devices = DeviceList.ToArray();

                theDevice.Status.StatusUpdate += OnDeviceStatusUpdate;
                theDevice.NameAndLocationUpdated += OnDeviceNameAndLocationUpdate;

                var FEDevices = Devices.Select(Device => new HomeDevice { Guid = Device.Guid, IPAddress = Device.IP, Name = Device.Name, Location = Device.Location, Status = Device.Status.ToString() }).ToArray();

                Update?.Invoke(new Payload(Event.Id, new Pair[] { new Pair("Devices", FEDevices) }));
            }
        }

        private void OnDeviceNameAndLocationUpdate(object sender, EventArgs e)
        {
            var FEDevices = Devices.Select(Device => new HomeDevice { Guid = Device.Guid, IPAddress = Device.IP, Name = Device.Name, Location = Device.Location, Status = Device.Status.ToString() }).ToArray();

            Update?.Invoke(new Payload(Event.Id, new Pair[] { new Pair("Devices", FEDevices) }));
        }

        private void OnDeviceStatusUpdate(object sender, StatusEnum e)
        {
            var FEDevices = Devices.Select(Device => new HomeDevice { Guid = Device.Guid, IPAddress = Device.IP, Name = Device.Name, Location = Device.Location, Status = Device.Status.ToString() }).ToArray();

            Update?.Invoke(new Payload(Event.Id, new Pair[] { new Pair("Devices", FEDevices) }));
        }

        public void Remove(BBDevice theDevice)
        {
            lock (m_ArrayLock)
            {
                theDevice.Status.StatusUpdate -= OnDeviceStatusUpdate;
                theDevice.NameAndLocationUpdated -= OnDeviceNameAndLocationUpdate;

                var DeviceList = new List<BBDevice>(Devices);
                DeviceList.Remove(theDevice);
                Devices = DeviceList.ToArray();

                var FEDevices = Devices.Select(Device => new HomeDevice { Guid = Device.Guid, IPAddress = Device.IP, Name = Device.Name, Location = Device.Location, Status = Device.Status.ToString() }).ToArray();

                Update?.Invoke(new Payload(Event.Id, new Pair[] { new Pair("Devices", FEDevices) }));
            }
        }
    }
}
