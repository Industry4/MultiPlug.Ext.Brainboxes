using System;
using Brainboxes.IO;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.Brainboxes.Models.Components.Device;

namespace MultiPlug.Ext.Brainboxes.Components.Device
{
    public class BBOutput : BBOutputProperties
    {
        private IOLine m_Line = null;

        public EventExternal UIToggleEvent { get; set; } = new EventExternal { Id = "UIToggle" + System.Guid.NewGuid().ToString(), Guid = Guid.NewGuid().ToString(), Description = "UI Toggle" };

        public BBOutput()
        {
        }

        public BBOutput(IOLine theIOLine)
        {
            m_Line = theIOLine;
            Name = m_Line.IONumber.ToString();

            // TEMP
            UIToggleEvent.Description = "UI Toggle for IO " + Name;
            var Defaults = CreateDefaultSubscriptionProperties();
            var NewSubscription = new BBSubscription { Guid = System.Guid.NewGuid().ToString(), Id = UIToggleEvent.Id };
            NewSubscription.Update(Defaults);
            NewSubscription.EventConsumer = new BBEventConsumer(Defaults);
            Subscriptions.Add(NewSubscription);
        }

        public BBOutput(string theName)
        {
            Name = theName;

            // TEMP
            UIToggleEvent.Description = "UI Toggle for IO " + Name;
            var Defaults = CreateDefaultSubscriptionProperties();
            var NewSubscription = new BBSubscription { Guid = System.Guid.NewGuid().ToString(), Id = UIToggleEvent.Id };
            NewSubscription.Update(Defaults);
            NewSubscription.EventConsumer = new BBEventConsumer(Defaults);
            Subscriptions.Add(NewSubscription);
        }

        public IOLine IOLine
        {
            set
            {
                m_Line = value;
                Subscriptions.ForEach(s => {
                    if (s.EventConsumer == null)
                    {
                        if (s.IOLine == null)
                        {
                            s.Update(CreateDefaultSubscriptionProperties() );
                        }
                        else
                        {
                            s.IOLine = m_Line;
                        }

                        s.EventConsumer = new BBEventConsumer(s);
                    }
                    else
                    {
                        var sub = s.EventConsumer as BBEventConsumer;
                        sub.Line = m_Line;
                    }
                });
            }
        }

        public int Value
        {
            get
            {
                if(m_Line == null)
                {
                    return -1;
                }
                else
                {
                    return m_Line.Value;
                }
            }
            set
            {
                if (m_Line != null)
                {
                    m_Line.Value = value;
                }
            }
        }

        public void Add( string theEventId, BBSubscriptionProperties theProperties)
        {
            Add(theEventId, System.Guid.NewGuid().ToString(), theProperties);
        }
        public void Add(string theEventId, string theGuid, BBSubscriptionProperties theProperties)
        {
            theProperties.IOLine = m_Line; 

            var NewSubscription = new BBSubscription
            {
                EventConsumer = (m_Line != null) ? new BBEventConsumer(theProperties) : null,
                Guid = theGuid,
                Id = theEventId
            };

            NewSubscription.Update(theProperties);
            Subscriptions.Add(NewSubscription);
        }

        public bool Remove(BBSubscription theSubscription)
        {
            return Subscriptions.Remove(theSubscription);
        }


        // TODO This should not be here.
        private BBSubscriptionProperties CreateDefaultSubscriptionProperties()
        {
            return new BBSubscriptionProperties
            {
                IOLine = m_Line,
                HighKey = "value",
                HighValue = "1",
                LowKey = "value",
                LowValue = "0"
            };
        }

        internal void Update(BBOutputProperties theProperties)
        {
            if(theProperties.Name != Name )
            {
                return;
            }

            if( theProperties.Subscriptions != null)
            {
                foreach( var Subscription in theProperties.Subscriptions)
                {
                    if( ! Subscription.Id.StartsWith("UIToggle"))
                    {
                        Subscriptions.Add(Subscription);
                    }

                }
            }
        }
    }
}
