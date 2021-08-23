using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brainboxes.IO;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.Brainboxes.Models;
using MultiPlug.Ext.Brainboxes.Models.Components.Device;
using MultiPlug.Ext.Brainboxes.Diagnostics;

namespace MultiPlug.Ext.Brainboxes.Components.Device
{
    public class BBDeviceConnect : EventConsumer
    {
        internal event Action<EventLogEntryCodes, string[]> Log;
        internal event Action Connected;

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
                        Task.Delay(2000).ContinueWith(t => ConnectTask());
                    }
                });
            }
        }

        private void TryToConnect()
        {
            if (string.IsNullOrEmpty(m_Properties.IP))
            {
                Log?.Invoke(EventLogEntryCodes.NoIPSet, null);
                return;
            }

            Log?.Invoke( EventLogEntryCodes.Connecting, new string[] { m_Properties.IP } );
            Status.Status = StatusEnum.Connecting;

            try
            {
                if(m_Properties.EDDevice == null)
                {
                    m_Properties.EDDevice = EDDevice.Create(m_Properties.IP);
                    m_Properties.EDDevice.DeviceStatusChangedEvent += DeviceStatusChanged;
                }
                Log?.Invoke(EventLogEntryCodes.Connected, new string[] { m_Properties.EDDevice.Describe(), m_Properties.IP });
            }
            catch(System.Net.WebException ex)
            {
                m_Properties.EDDevice = null;

                Status.Status = StatusEnum.Errored;

                Log?.Invoke(EventLogEntryCodes.ConnectionException, new string[] { m_Properties.IP, ex.Message } );
                throw (ex);
            }
            catch (Exception ex)
            {
                m_Properties.EDDevice = null;

                Status.Status = StatusEnum.Errored;

                Log?.Invoke(EventLogEntryCodes.ConnectionException, new string[] { m_Properties.IP, ex.Message });
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
              //  BBEventFire EventFire = null;

                if (FoundEvent == null)
                {
                    string NewGuid = System.Guid.NewGuid().ToString();

                    FoundEvent = new BBDeviceEvent
                    {
                        Guid = NewGuid,
                        Id = NewGuid,
                        IONumber = InputIO.IONumber,
                        Description = "Device [" + m_Properties.MACAddress + "] I/O [" + InputIO.IONumber.ToString() + "]",
                        Subjects = new string[] { Core.Instance.Defaults.Key },
                        RisingEdgeValue = Core.Instance.Defaults.RisingEdge,
                        FallingEdgeValue = Core.Instance.Defaults.FallingEdge
                    };

                   // EventFire = new BBEventFire(NewEvent.Guid, NewEvent.Keys[0], NewEvent.RisingEdgeValue, NewEvent.FallingEdgeValue);
                //    NewEvent.Object = EventFire;
                    NewEvents.Add(FoundEvent);
                }
                else
                {
                 //   EventFire = FoundEvent.Object as BBEventFire;
                }

                InputIO.IOLineRisingEdge += FoundEvent.OnIOLineRisingEdgeChangedEvent;
                InputIO.IOLineFallingEdge += FoundEvent.OnIOLineFallingEdgeChangedEvent;

                FoundEvent.SetInit(InputIO.Value);
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
            Connected?.Invoke();
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
            if(newValue)
            {
                Log?.Invoke(EventLogEntryCodes.PropertyChangedTrue, new string[] { property });
            }
            else
            {
                Log?.Invoke(EventLogEntryCodes.PropertyChangedFalse, new string[] { property });
            }

            if( m_Properties.EDDevice == null && ( ! m_Properties.EDDevice.IsConnected ) )
            {
                Status.Status = StatusEnum.Disconnected;
            }
        }
    }
}
