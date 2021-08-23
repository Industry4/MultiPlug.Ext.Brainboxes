using System.Drawing;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.Brainboxes.Properties;

namespace MultiPlug.Ext.Brainboxes.Controllers.Assets.Images
{
    [Route("images/*")]
    public class ImageController : AssetsEndpoint
    {
        public Response Get(string theName)
        {
            ImageConverter converter = new ImageConverter();
            return new Response { RawBytes = (byte[])converter.ConvertTo(Resources.brainboxes_logo, typeof(byte[])), MediaType = "image/png" };
        }
    }
}
