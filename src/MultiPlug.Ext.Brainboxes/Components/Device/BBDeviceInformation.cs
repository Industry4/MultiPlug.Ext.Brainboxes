using System;
using System.Xml;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using MultiPlug.Ext.Brainboxes.Models.Components.Device;
using MultiPlug.Ext.Brainboxes.Diagnostics;

namespace MultiPlug.Ext.Brainboxes.Components.Device
{
    internal class BBDeviceInformation
    {
        private CancellationTokenSource m_TokenSource = new CancellationTokenSource();

        internal event Action<DeviceInformation> FetchCompleted;
        internal event Action<EventLogEntryCodes, string[]> Log;
        internal event Action FetchError;

        private Task m_Task;

        public BBDeviceInformation()
        {
        }

        public void BeginFetch(object sender, string theIPAddress)
        {
            Cancel();

            m_TokenSource = new CancellationTokenSource();

            if ((m_Task == null || m_Task.IsCompleted))
            {
                m_Task = Task.Run(() => FetchDeviceInformation(theIPAddress) );
            }
        }

        public void Cancel()
        {
            if (m_Task != null && !m_Task.IsCompleted)
            {
                m_TokenSource.Cancel();
                m_Task.Wait();
            }
        }

        private void FetchDeviceInformation(string theIPAddress)
        {
            var DevInfoUrl = "http://" + theIPAddress + "/devinfo.xml";
            string xmlStr = string.Empty;

            do
            {
                try
                {
                    using (var wc = new WebClient())
                    {
                        xmlStr = wc.DownloadString(DevInfoUrl);
                    }
                }
                catch (Exception ex)
                {
                    Log?.Invoke(EventLogEntryCodes.FetchException, new string[] { theIPAddress, ex.Message });
                    FetchError?.Invoke();
                    Task.Delay(3000).Wait();
                }

            } while (xmlStr == string.Empty && (!m_TokenSource.Token.IsCancellationRequested));

            if (m_TokenSource.Token.IsCancellationRequested)
            {
                return;
            }
            else
            {
                Log?.Invoke(EventLogEntryCodes.FetchSuccessful, new string[] { theIPAddress });

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlStr);

                XmlNode MACNode = xmlDoc.DocumentElement.SelectSingleNode("/lines/macaddr");
                XmlNode LocNode = xmlDoc.DocumentElement.SelectSingleNode("/lines/loc");
                XmlNode FwverNode = xmlDoc.DocumentElement.SelectSingleNode("/lines/fwver");
                XmlNode ModelNode = xmlDoc.DocumentElement.SelectSingleNode("/lines/model");
                XmlNode DevNameNode = xmlDoc.DocumentElement.SelectSingleNode("/lines/devname");

                 FetchCompleted?.Invoke(new DeviceInformation
                {
                    MACAddress = MACNode.InnerText,
                    Name = DevNameNode.InnerText,
                    Location = LocNode.InnerText,
                    Firmware = FwverNode.InnerText,
                    ProductModel = ModelNode.InnerText
                });
            }
        }
    }
}
