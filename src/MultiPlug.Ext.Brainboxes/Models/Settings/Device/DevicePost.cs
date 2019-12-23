
namespace MultiPlug.Ext.Brainboxes.Models.Settings.Device
{
    public class DevicePost
    {
        public string Guid { get; set; }
        public string IPAddress { get; set; }
        public string Name { get; set; }
        public string[] IONumber { get; set; }
        public string[] EventId { get; set; }
        public string[] EventDescription { get; set; }
        public string[] High { get; set; }
        public string[] Low { get; set; }
    }
}
