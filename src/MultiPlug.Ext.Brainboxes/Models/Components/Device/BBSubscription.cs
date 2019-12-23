
namespace MultiPlug.Ext.Brainboxes.Models.Components.Device
{
    public class BBSubscription : BBSubscriptionProperties
    {
        public void Update(BBSubscriptionProperties theProperties)
        {
            IOLine = theProperties.IOLine;
            HighKey = theProperties.HighKey;
            HighValue = theProperties.HighValue;
            LowKey = theProperties.LowKey;
            LowValue = theProperties.LowValue;
        }
    }
}
