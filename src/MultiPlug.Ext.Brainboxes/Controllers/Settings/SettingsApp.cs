using System.Collections.Generic;

using MultiPlug.Base.Http;
using MultiPlug.Extension.Core.Http;
using MultiPlug.Extension.Core.Attribute;
using MultiPlug.Ext.Brainboxes.Controllers.Settings.Home;
using MultiPlug.Ext.Brainboxes.Controllers.Settings.Device;
using MultiPlug.Ext.Brainboxes.Controllers.Settings.Subscription;
using MultiPlug.Ext.Brainboxes.Controllers.Settings.Defaults;

namespace MultiPlug.Ext.Brainboxes.Controllers.Settings
{
    [Name("Brainboxes")]
    [HttpEndpointType(HttpEndpointType.Settings)]
    [ViewAs(ViewAs.Partial)]
    class SettingsApp : HttpEndpoint
    {
        readonly Controller[] m_Controllers;

        public SettingsApp()
        {
            m_Controllers = new Controller[]
            {
                new HomeController(),
                new DeviceController(),
                new DeviceDeleteController(),
                new SubscriptionController(),
                new SubscriptionDeleteController(),
                new DiscoveryController(),
                new DefaultsController(),
                new OLDHomeController()
             };
        }

        public override IEnumerable<Controller> Controllers
        {
            get
            {
                return m_Controllers;
            }
        }
    }
}
