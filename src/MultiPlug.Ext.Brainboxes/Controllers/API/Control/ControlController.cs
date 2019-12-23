using System.Linq;
using System.Text;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Exchange;
using MultiPlug.Base.Http;
using MultiPlug.Ext.Brainboxes.Models.API;
using System;

namespace MultiPlug.Ext.Brainboxes.Controllers.API.Control
{
    [Route("control/*")]
    class ControlController : Controller
    {
        public Response Get(GetDeviceIO theModel)
        {
            if( theModel == null || theModel.Id == null)
            {
                return new Response { StatusCode = System.Net.HttpStatusCode.NotFound };
            }

            var DeviceSearch = Core.Instance.Devices.FirstOrDefault(Device => Device.Guid == theModel.Id);

            if( DeviceSearch != null)
            {
                var IOSearch = DeviceSearch.Outputs.FirstOrDefault(Input => Input.Name == theModel.Io);

                if( IOSearch != null)
                {
                    Pair Json = new Pair(string.Empty, new DeviceIO { Id = theModel.Id, Io = theModel.Io.ToString(), State = IOSearch.Value.ToString() });

                    return new Response { MediaType = "application/json", RawBytes = Encoding.ASCII.GetBytes(Json.Value) };
                }
                else
                {
                    return new Response { StatusCode = System.Net.HttpStatusCode.NotFound };
                }
            }
            else
            {
                return new Response { StatusCode = System.Net.HttpStatusCode.NotFound };
            }
        }

        public Response Post(SetDeviceIO theModel)
        {
            if (theModel == null || theModel.Id == null)
            {
                return new Response { StatusCode = System.Net.HttpStatusCode.NotFound };
            }

            int NewState;

            if ( ! Int32.TryParse(theModel.State, out NewState) )
            {
                return new Response { StatusCode = System.Net.HttpStatusCode.InternalServerError };
            }

            var DeviceSearch = Core.Instance.Devices.FirstOrDefault(Device => Device.Guid == theModel.Id);

            if (DeviceSearch != null)
            {
                var IOSearch = DeviceSearch.Outputs.FirstOrDefault(Output => Output.Name == theModel.Io);

                if (IOSearch != null)
                {
                    if(NewState == 0 || NewState == 1)
                    {
                        IOSearch.Value = NewState;
                    }

                    Pair Json = new Pair(string.Empty, new DeviceIO { Id = theModel.Id, Io = theModel.Io.ToString(), State = IOSearch.Value.ToString() });

                    return new Response { MediaType = "application/json", RawBytes = Encoding.ASCII.GetBytes(Json.Value) };
                }
                else
                {
                    return new Response { StatusCode = System.Net.HttpStatusCode.NotFound };
                }
            }
            else
            {
                return new Response { StatusCode = System.Net.HttpStatusCode.NotFound };
            }
        }
    }
}
