using System;
using System.Runtime.Serialization;
using MultiPlug.Base.Exchange;
using Brainboxes.IO;

namespace MultiPlug.Ext.Brainboxes.Models
{
    public class BBDeviceEvent : Event
    {
        [DataMember]
        public int IONumber { get; set; }
        [DataMember]
        public string RisingEdgeValue { get; set; }
        [DataMember]
        public string FallingEdgeValue { get; set; }

        public Payload m_Cached { get; set; } = new Payload(string.Empty, new PayloadSubject[0]);

        public BBDeviceEvent()
        {
            this.CachedPayload = new Func<Payload>(GetCachedPayload);
            m_Cached = new Payload(Id, new PayloadSubject[0]);
        }

        internal void SetInit(int theState)
        {
            if(theState == 1)
            {
                m_Cached = new Payload(Id, new PayloadSubject[] { new PayloadSubject("value", RisingEdgeValue) });
            }
            else
            {
                m_Cached = new Payload(Id, new PayloadSubject[] { new PayloadSubject("value", FallingEdgeValue) });
            }
        }

        internal void FireOnStart()
        {
            Invoke(m_Cached);
        }

        public void OnIOLineRisingEdgeChangedEvent(IOLine line, EDDevice device, IOChangeTypes changeType)
        {
            m_Cached = new Payload(Id, new PayloadSubject[] { new PayloadSubject("value", RisingEdgeValue) });
            Invoke(m_Cached);
        }

        public void OnIOLineFallingEdgeChangedEvent(IOLine line, EDDevice device, IOChangeTypes changeType)
        {
            m_Cached = new Payload(Id, new PayloadSubject[] { new PayloadSubject("value", FallingEdgeValue) });
            Invoke(m_Cached);
        }

        private Payload GetCachedPayload()
        {
            return m_Cached;
        }
    }
}
