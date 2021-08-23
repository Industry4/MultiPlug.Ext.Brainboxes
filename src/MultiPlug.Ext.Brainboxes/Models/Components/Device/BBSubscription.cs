
namespace MultiPlug.Ext.Brainboxes.Models.Components.Device
{
    public class BBSubscription : BBSubscriptionProperties
    {
        public void Update(BBSubscriptionProperties theProperties)
        {
            IOLine = theProperties.IOLine;
            HighSubject = theProperties.HighSubject;
            HighValue = theProperties.HighValue;
            LowSubject = theProperties.LowSubject;
            LowValue = theProperties.LowValue;
        }
    }
}
