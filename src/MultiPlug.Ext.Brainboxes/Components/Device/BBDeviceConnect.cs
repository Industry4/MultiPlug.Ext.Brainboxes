using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brainboxes.IO;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.Brainboxes.Models;
using MultiPlug.Ext.Brainboxes.Models.Components.Device;

namespace MultiPlug.Ext.Brainboxes.Components.Device
{
    public class BBDeviceConnect : EventConsumer
    {
        public event EventHandler<string> Log;
        public event EventHandler Connected;

        private Task m_Task;

        private bool m_RetryOnFail;

        public BBDeviceConnectStatus Status { get; private set; }

        readonly BBDeviceProperties m_Properties;

        public BBDeviceConnect(BBDeviceProperties theProperties)
        {
            m_Properties = theProperties;
            Status = new BBDeviceConnectStatus(theProperties);
        }

        public override void OnEvent(Payload thePayload)
        {
            if ( m_Properties.EDDevice == null || ( ! m_Properties.EDDevice.IsConnected ) )
            {
                m_Properties.AutoConnect = true;
                Connect( false );
            }
            else
            {
                m_Properties.AutoConnect = false;
                Disconnect();
            }
        }

        public void Connect( bool Retry )
        {
            m_RetryOnFail = Retry;
            ConnectTask();
        }

        internal void OnNameUpdated(object sender, string theName)
        {
            if (m_Properties.EDDevice != null && m_Properties.EDDevice.IsConnected)
            {
                m_Properties.EDDevice.Protocol.DeviceName = theName;
            }
        }

        internal void OnIPAddressUpdated(object sender, string e)
        {
            Disconnect();
        }

        public void Disconnect()
        {
            if (m_Properties.EDDevice != null && m_Properties.EDDevice.IsConnected)
            {
                m_Properties.EDDevice.Disconnect();
                m_Properties.EDDevice = null;
                Status.Status = StatusEnum.Disconnected;
            }
        }

        private void ConnectTask()
        {
            if( ( m_Properties.MACFetch == MACResult.Matched ) && ( m_Task == null || m_Task.IsCompleted ) )
            {
                m_Task = Task.Run(() => TryToConnect()).ContinueWith(d =>
                {
                    if( d.IsFaulted && m_RetryOnFail )
                    {
                        ConnectTask();
                    }
                });
            }
        }

        private void TryToConnect()
        {
            if (string.IsNullOrEmpty(m_Properties.IP))
            {
                Log?.Invoke(this, "Tried to connect but no IP set.");
                return;
            }

            Log?.Invoke(this, "Connecting device with IP [" + m_Properties.IP + "]");
            Status.Status = StatusEnum.Connecting;

            try
            {
                if(m_Properties.EDDevice == null)
                {
                    m_Properties.EDDevice = EDDevice.Create(m_Properties.IP);
                    m_Properties.EDDevice.DeviceStatusChangedEvent += DeviceStatusChanged;
                }

                Log?.Invoke(this, "Connected to device: [" + m_Properties.EDDevice.Describe() + "] at IP [" + m_Properties.IP + "]");
            }
            catch(System.Net.WebException ex)
            {
                m_Properties.EDDevice = null;

                Status.Status = StatusEnum.Errored;

                Log?.Invoke(this, "Exception from device with IP [" + m_Properties.IP + "] From [EDDevice.Create] Exception: " + ex.Message);
                throw (ex);
            }
            catch (Exception ex)
            {
                m_Properties.EDDevice = null;

                Status.Status = StatusEnum.Errored;

                Log?.Invoke(this, "Exception from device with IP [" + m_Properties.IP + "] From [EDDevice.Create] Exception: " + ex.Message);
                throw (ex);
            }

            if (!string.IsNullOrEmpty(m_Properties.Name) && m_Properties.Name != m_Properties.EDDevice.Protocol.DeviceName)
            {
                m_Properties.EDDevice.Protocol.DeviceName = m_Properties.Name;
            }
            if (string.IsNullOrEmpty(m_Properties.Name) && m_Properties.EDDevice.Protocol.DeviceName != null)
            {
                m_Properties.Name = m_Properties.EDDevice.Protocol.DeviceName;
            }

            var NewEvents = new List<BBDeviceEvent>();
            NewEvents.AddRange(m_Properties.IOEvents);

            foreach (var InputIO in m_Properties.EDDevice.Inputs)
            {
                BBDeviceEvent FoundEvent = m_Properties.IOEvents.FirstOrDefault(e => e.IONumber == InputIO.IONumber);
                BBEventFire EventFire = null;

                if (FoundEvent == null)
                {
                    string NewGuid = System.Guid.NewGuid().ToString();

                    var NewEvent = new BBDeviceEvent
                    {
                        Guid = NewGuid,
                        Id = NewGuid,
                        IONumber = InputIO.IONumber,
                        Description = "Device [" + m_Properties.MACAddress + "] I/O [" + InputIO.IONumber.ToString() + "]",
                        Keys = new string[] { Core.Instance.Defaults.Key },
                        RisingEdgeValue = Core.Instance.Defaults.RisingEdge,
                        FallingEdgeValue = Core.Instance.Defaults.FallingEdge
                    };

                    EventFire = new BBEventFire(NewEvent.Guid, NewEvent.Keys[0], NewEvent.RisingEdgeValue, NewEvent.FallingEdgeValue);
                    NewEvent.Object = EventFire;
                    NewEvents.Add(NewEvent);
                }
                else
                {
                    EventFire = FoundEvent.Object as BBEventFire;
                }

                InputIO.IOLineRisingEdge += EventFire.OnIOLineRisingEdgeChangedEvent;
                InputIO.IOLineFallingEdge += EventFire.OnIOLineFallingEdgeChangedEvent;
            }

            m_Properties.IOEvents = NewEvents.ToArray();

            var NewOutputs = new List<BBOutput>();
            NewOutputs.AddRange(m_Properties.Outputs);

            foreach (var item in m_Properties.EDDevice.Outputs)
            {
                BBOutput AOutput = m_Properties.Outputs.FirstOrDefault(o => o.Name == item.IONumber.ToString());
                if (AOutput == null)
                {
                    NewOutputs.Add(new BBOutput(item));
                }
                else
                {
                    AOutput.IOLine = item;
                }
            }

            m_Properties.Outputs = NewOutputs.ToArray();

            Status.Status = StatusEnum.Connected;
            Connected?.Invoke(this, EventArgs.Empty);
        }

        public bool IsConnected
        {
            get
            {
                return (m_Properties.EDDevice != null && m_Properties.EDDevice.IsConnected) ? true : false;
            }
        }

        private void DeviceStatusChanged(IDevice<IConnection, IIOProtocol> device, string property, bool newValue)
        {
            string NewV = (newValue) ? "Yes" : "No";

            Log?.Invoke(this, "Device Status Changed: Property [" + property + "] Value [" + NewV + "]");

            if( m_Properties.EDDevice == null && ( ! m_Properties.EDDevice.IsConnected ) )
            {
                Status.Status = StatusEnum.Disconnected;
            }
        }
    }
}
