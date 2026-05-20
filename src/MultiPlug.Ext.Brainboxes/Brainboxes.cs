using System;
using System.Collections.Generic;
using MultiPlug.Base.Exchange;
using MultiPlug.Extension.Core;
using MultiPlug.Extension.Core.Http;
using MultiPlug.Ext.Brainboxes.Models;
using MultiPlug.Ext.Brainboxes.Properties;
using MultiPlug.Ext.Brainboxes.Models.Components.Device;
using MultiPlug.Ext.Brainboxes.Controllers.Settings;

namespace MultiPlug.Ext.Brainboxes
{
    public class Brainboxes : MultiPlugExtension
    {
        public Brainboxes()
        {
            Core.Instance.EventsUpdated += new EventHandler<Event[]>(OnEventsUpdated);
            Core.Instance.SubscriptionsUpdated += new EventHandler<Subscription[]>(OnSubscriptionsUpdated);

            MultiPlugServices.Logging.RegisterDefinitions(Diagnostics.EventLogDefinitions.DefinitionsId, Diagnostics.EventLogDefinitions.Definitions, true);
            Core.Instance.MultiPlugServices = MultiPlugServices;
        }

        private void OnSubscriptionsUpdated(object sender, Subscription[] e)
        {
            MultiPlugActions.Extension.Updates.Subscriptions();
        }

        private void OnEventsUpdated(object sender, Event[] e)
        {
            MultiPlugActions.Extension.Updates.Events();
        }

        public override Event[] Events
        {
            get
            {
                return Core.Instance.Events;
            }
        }

        public override Subscription[] Subscriptions
        {
            get
            {
                return Core.Instance.Subscriptions;
            }
        }

        public override RazorTemplate[] RazorTemplates
        {
            get
            {
                return new RazorTemplate[]
                {
                    new RazorTemplate(Templates.SettingsNavigation, Resources.SettingsNavigation),
                    new RazorTemplate(Templates.SettingsDevice, Resources.Device_html),
                    new RazorTemplate(Templates.SettingsDiscovery, Resources.Discovery_html),
                    new RazorTemplate(Templates.SettingsDeviceSubscription, Resources.Subscription_html),
                    new RazorTemplate(Templates.SettingsDevices, Resources.Settings_html),
                    new RazorTemplate(Templates.SettingsDeviceDefaults, Resources.Defaults_html),
                    new RazorTemplate(Templates.SettingsDeviceNotFound, Resources.DeviceNotFound_html),
                    new RazorTemplate(Templates.SettingsAbout, Resources.SettingsAbout)
                };
            }
        }

        private List<Models.Load.Root> InitProcess = new List<Models.Load.Root>();

        public override void Initialise()
        {
            var newsubs = new List<SubModel>();
            var newevents = new List<EventModel>();
            var newdevices = new List<BBDeviceProperties>();

            var ToProcess = InitProcess;
            InitProcess = new List<Models.Load.Root>();

            foreach ( var unprocessed in ToProcess)
            {
                if(unprocessed.Devices != null)
                {
                    newdevices.AddRange(unprocessed.Devices);
                }

                if(unprocessed.Discovery != null)
                {
                    Core.Instance.UpdateDiscoveryProperties(unprocessed.Discovery.Properties);
                }

                if (unprocessed.Discovery != null)
                {
                    Core.Instance.UpdateDiscoveryProperties(unprocessed.Discovery.Properties);
                }

                if (unprocessed.Defaults != null)
                {
                    Core.Instance.Update(unprocessed.Defaults);
                }
            }

            Core.Instance.Update(newdevices.ToArray());

            InitProcess.Clear();

            MultiPlugActions.Extension.Updates.Events();

            MultiPlugActions.Extension.Updates.Subscriptions();

            Core.Instance.InitialiseDevices();
        }

        public override object Save()
        {
            return Core.Instance;
        }

        public override void Start()
        {
            Core.Instance.Start();
        }

        public override void Shutdown()
        {
            Core.Instance.Shutdown();
        }

        public void Load(Models.Load.Root config)
        {
             InitProcess.Add(config);
        }
    }
}
