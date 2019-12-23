using MultiPlug.Base.Exchange;
using MultiPlug.Ext.Brainboxes.Models.Components.Device;

namespace MultiPlug.Ext.Brainboxes.Components.Device
{
    public class BBDeviceRestart : EventConsumer
    {
        readonly BBDeviceProperties m_Properties;

        public BBDeviceRestart(BBDeviceProperties theProperties)
        {
            m_Properties = theProperties;
        }

        public override void OnEvent(Payload thePayload)
        {
            if( m_Properties.EDDevice != null && m_Properties.EDDevice.IsConnected)
            {
                m_Properties.EDDevice.Protocol.Restart();
            }
        }
    }
}
