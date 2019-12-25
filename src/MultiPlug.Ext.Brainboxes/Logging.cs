using System;
using System.Linq;
using System.Text;
using MultiPlug.Base.Exchange;

namespace MultiPlug.Ext.Brainboxes
{
    public class Logging : EventableBase
    {
        private StringBuilder m_TraceLog;
        private Event m_WriteEvent;

        private static Logging m_Instance = null;

        public static Logging Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new Logging();
                }
                return m_Instance;
            }
        }

        private Logging()
        {
            m_TraceLog = new StringBuilder();

            var EventGuid = Guid.NewGuid().ToString();
            m_WriteEvent = new Event { Guid = EventGuid, Id = EventGuid, Description = "Brainboxes Trace Log", Object = this };
        }

        public void WriteLine( string value )
        {
            m_TraceLog.AppendLine(value);

            Update?.Invoke( new Payload ( m_WriteEvent.Id, new Pair[] { new Pair( "WriteLine", value ) } ) );
        }

        public override string ToString()
        {
            return m_TraceLog.ToString();
        }

        public override Payload CachedValue()
        {
            throw new NotImplementedException();
        }

        public Event Event
        {
            get
            {
                return m_WriteEvent;
            }
        }
    }
}
