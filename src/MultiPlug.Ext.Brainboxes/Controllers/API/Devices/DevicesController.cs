using System.Linq;
using System.Text;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Exchange;
using MultiPlug.Base.Http;

namespace MultiPlug.Ext.Brainboxes.Controllers.API.Devices
{
    [Route("")]
    class DevicesController : APIEndpoint
    {
        public Response Get()
        {
            var DeviceSearch = Core.Instance.Devices.Select(Device => new
            {
                Id = Device.Guid,
                Name = Device.Name,
                IPAddress = Device.IP,
                MACAddress = Device.MACAddress,
                Location = Device.Location,
                Firmware = Device.Firmware,
                Model = Device.ProductModel,
                Status = Device.Status.ToString(),
                Inputs = Device.IOEvents.Count(),
                Outputs = Device.Outputs.Count()
            }).ToArray();

            Pair Json = new Pair(string.Empty, DeviceSearch);

            return new Response { MediaType = "application/json", RawBytes = Encoding.ASCII.GetBytes(Json.Value) };
        }
    }
}
