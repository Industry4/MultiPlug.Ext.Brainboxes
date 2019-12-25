using System;
using System.IO;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.Brainboxes.Models.Components.Device;

namespace MultiPlug.Ext.Brainboxes.Components.Device
{
    public class BBDeviceLogging : EventableBase
    {
        readonly BBDeviceProperties m_Properties;
        private string m_LogPath = string.Empty;
        private bool m_DeletingLogs = false;

        public Event Event { get; private set; }

        public BBDeviceLogging(BBDeviceProperties theProperties)
        {
            m_Properties = theProperties;

            var EventGuid = Guid.NewGuid().ToString();
            Event = new Event { Guid = EventGuid, Id = EventGuid, Description = "Device Logging", Object = this, Keys = new string[] { "Log" } };
        }

        private void ConstructLogPath()
        {

            m_LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logging");

            try
            {
                Directory.CreateDirectory(m_LogPath);
            }
            catch { }

            m_LogPath = Path.Combine(m_LogPath, m_Properties.Guid);

            try
            {
                Directory.CreateDirectory(m_LogPath);
            }
            catch{ }

            m_LogPath = Path.Combine(m_LogPath, m_Properties.Guid + ".txt" );

            if( File.Exists(m_LogPath))
            {
                try
                {
                    File.Delete(m_LogPath);
                }
                catch { }
            }
        }

        public void Delete()
        {
            m_DeletingLogs = true;

            if (File.Exists(m_LogPath))
            {
                try
                {
                    File.Delete(m_LogPath);
                }
                catch { }
            }

            var DirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logging", m_Properties.Guid);

            if( Directory.Exists(DirPath))
            {
                try
                {
                    Directory.Delete(DirPath);
                }
                catch { }
            }
        }

        public void Log(string theLog)
        {
            if(m_DeletingLogs)
            {
                return;
            }

            if(m_LogPath == string.Empty)
            {
                ConstructLogPath();
            }

            string LogWithDate = $"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()} : {theLog}";

            using (StreamWriter w = File.AppendText(m_LogPath))
            {
                w.WriteLine(LogWithDate);
                w.WriteLine("-------------------------------");
            }

            Update?.Invoke(new Payload(Event.Id, new Pair[] { new Pair(Event.Keys[0], LogWithDate) }));
        }

        public string LogRead()
        {
            if (m_LogPath == string.Empty)
            {
                return string.Empty;
            }

            return File.ReadAllText(m_LogPath);
        }

        public override Payload CachedValue()
        {
            throw new NotImplementedException();
        }
    }
}
