using System;
using Brainboxes.IO;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.Brainboxes.Models.Components.Device;

namespace MultiPlug.Ext.Brainboxes.Components.Device
{
    public class BBEventConsumer : EventConsumer
    {
        private BBSubscriptionProperties m_Properties;

        public BBEventConsumer(BBSubscriptionProperties theProperties)
        {
            m_Properties = theProperties;
        }

        public IOLine Line
        {
            set
            {
                m_Properties.IOLine = value;
            }
        }

        public override void OnEvent(Payload e)
        {
            bool Found = false;

            foreach( var p in e.Pairs)
            {
                if(string.Equals(p.Type, m_Properties.HighKey, StringComparison.OrdinalIgnoreCase) && string.Equals(p.Value, m_Properties.HighValue, StringComparison.OrdinalIgnoreCase))
                {
                    Found = true;
                    m_Properties.IOLine.Value = 1;
                    break;
                }
                if (string.Equals(p.Type, m_Properties.LowKey, StringComparison.OrdinalIgnoreCase) && string.Equals(p.Value, m_Properties.LowValue, StringComparison.OrdinalIgnoreCase))
                {
                    Found = true;
                    m_Properties.IOLine.Value = 0;
                    break;
                }
            }

            if( ! Found ) // Toggle
            {
                m_Properties.IOLine.Value = (m_Properties.IOLine.Value == 0) ? 1 : 0;
            }
        }
    }
}
