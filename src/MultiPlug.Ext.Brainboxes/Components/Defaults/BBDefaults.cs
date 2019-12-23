using MultiPlug.Ext.Brainboxes.Models.Components.Defaults;

namespace MultiPlug.Ext.Brainboxes.Components.Defaults
{
    public class BBDefaults : BBDefaultProperties
    {
        internal void Update(BBDefaultProperties theProperties)
        {
            if (theProperties.Key != null)
            {
                Key = theProperties.Key;
            }

            if( theProperties.FallingEdge != null)
            {
                FallingEdge = theProperties.FallingEdge;
            }

            if (theProperties.RisingEdge != null)
            {
                RisingEdge = theProperties.RisingEdge;
            }
        }
    }
}
