using Brainboxes.IO;
using MultiPlug.Base.Exchange;

namespace MultiPlug.Ext.Brainboxes.Components.Device
{
    public class BBEventFire : EventableBase
    {
        public override event MPEventHandler Update;
        private string m_Id = "";

        public string MyProperty { get; set; }

        private string m_KeyValue;

        public string RisingEdgeValue { get; private set; }
        public string FallingEdgeValue { get; private set; }

        public Payload m_Cached { get; set; } = new Payload(string.Empty, new Pair[0]);

        public BBEventFire( string theEventId, string theKeyValue, string theRisingEdgeValue, string theFallingEdgeValue)
        {
            m_KeyValue = theKeyValue;
            RisingEdgeValue = theRisingEdgeValue;
            FallingEdgeValue = theFallingEdgeValue;
            m_Id = theEventId;
        }

        public string Id
        {
            get
            {
                return m_Id;
            }
            set
            {
                m_Id = value;
            }
        }

        public void OnIOLineRisingEdgeChangedEvent(IOLine line, EDDevice device, IOChangeTypes changeType)
        {
            m_Cached = new Payload(m_Id, new Pair[] { new Pair("value", RisingEdgeValue) });
            Update?.Invoke( m_Cached );
        }

        public void OnIOLineFallingEdgeChangedEvent(IOLine line, EDDevice device, IOChangeTypes changeType)
        {
            m_Cached = new Payload(m_Id, new Pair[] { new Pair("value", FallingEdgeValue) });
            Update?.Invoke( m_Cached );
        }


        public override Payload CachedValue()
        {
            return m_Cached;
        }
    }
}
