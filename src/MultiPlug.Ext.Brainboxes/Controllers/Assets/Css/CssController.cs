using System.Text;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.Brainboxes.Properties;

namespace MultiPlug.Ext.Brainboxes.Controllers.Assets.Css
{
    [Route("css/*")]
    class CssController : AssetsEndpoint
    {
        public Response Get(string theName)
        {
            string Result = string.Empty;

            switch (theName)
            {
                case "toggleswitch.css":
                    Result = Resources.ToggleSwitch_css;
                    break;
                case "IOStateColours.css":
                    Result = Resources.IOStateColours_css;
                    break;
            }

            if (string.IsNullOrEmpty(Result))
            {
                return new Response { StatusCode = System.Net.HttpStatusCode.NotFound };
            }
            else
            {
                return new Response { MediaType = "text/css", RawBytes = Encoding.ASCII.GetBytes(Result) };
            }
        }
    }
}
