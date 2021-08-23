using System;
using System.Text;

using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.Brainboxes.Properties;

namespace MultiPlug.Ext.Brainboxes.Controllers.Assets.Js
{
    [Route("js/*")]
    public class JavaScriptsController : AssetsEndpoint
    {
        public Response Get(string theName)
        {
            string Result = string.Empty;

            switch (theName)
            {
                case "devices.js":
                    Result = Resources.devices_js;
                    break;

                case "device.js":
                    Result = Resources.device_js;
                    break;

                default:
                    Console.WriteLine("ERROR Javascript missing:               " + theName);
                    break;
            }

            if (string.IsNullOrEmpty(Result))
            {
                return new Response { StatusCode = System.Net.HttpStatusCode.NotFound };
            }
            else
            {
                return new Response { MediaType = "text/javascript", RawBytes = Encoding.ASCII.GetBytes(Result) };
            }
        }
    }
}
