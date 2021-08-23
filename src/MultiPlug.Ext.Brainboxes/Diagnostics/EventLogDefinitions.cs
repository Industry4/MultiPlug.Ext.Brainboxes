using MultiPlug.Base.Diagnostics;

namespace MultiPlug.Ext.Brainboxes.Diagnostics
{
    internal class EventLogDefinitions
    {
        internal const string DefinitionsId = "MultiPlug.Ext.Brainboxes.EN";

        internal static EventLogDefinition[] Definitions { get; set; } = new EventLogDefinition[]
        {
            new EventLogDefinition { Code = (int) EventLogEntryCodes.DeviceCreated, Source = (int) EventLogEntryCodes.SourceDevice,  StringFormat = "DeviceCreated", Type = EventLogEntryType.Information  },
            new EventLogDefinition { Code = (int) EventLogEntryCodes.MACMissMatch, Source = (int) EventLogEntryCodes.SourceDevice,  StringFormat = "Device of IP [{0}] has a different MAC Address than expected. Expected [{1}] Actual [{2}]", Type = EventLogEntryType.Warning  },
            new EventLogDefinition { Code = (int) EventLogEntryCodes.MACAddressSet, Source = (int) EventLogEntryCodes.SourceDevice,  StringFormat = "Device of IP [{0}] MAC Address set to {1}", Type = EventLogEntryType.Information },
            new EventLogDefinition { Code = (int) EventLogEntryCodes.Connecting, Source = (int) EventLogEntryCodes.SourceDeviceConnection,  StringFormat = "Connecting Device with IP [{0}]", Type = EventLogEntryType.Information },
            new EventLogDefinition { Code = (int) EventLogEntryCodes.Connected, Source = (int) EventLogEntryCodes.SourceDeviceConnection,  StringFormat = "Connected to Device: [{0}] at IP [{1}]", Type = EventLogEntryType.Information },
            new EventLogDefinition { Code = (int) EventLogEntryCodes.PropertyChangedTrue, Source = (int) EventLogEntryCodes.SourceDeviceConnection,  StringFormat = "Device Status Changed: Property [{0}] Value [true]", Type = EventLogEntryType.Information  },
            new EventLogDefinition { Code = (int) EventLogEntryCodes.PropertyChangedFalse, Source = (int) EventLogEntryCodes.SourceDeviceConnection,  StringFormat = "Device Status Changed: Property [{0}] Value [false]", Type = EventLogEntryType.Information  },
            new EventLogDefinition { Code = (int) EventLogEntryCodes.NoIPSet, Source = (int) EventLogEntryCodes.SourceDeviceConnection,  StringFormat = "Tried to connect but no IP set.", Type = EventLogEntryType.Warning },
            new EventLogDefinition { Code = (int) EventLogEntryCodes.ConnectionException, Source = (int) EventLogEntryCodes.SourceDeviceConnection,  StringFormat = "Exception from Device with IP {0} Exception: {1}", Type = EventLogEntryType.Error },
            new EventLogDefinition { Code = (int) EventLogEntryCodes.FetchSuccessful, Source = (int) EventLogEntryCodes.SourceDeviceFetch,  StringFormat = "Fetch Successful from Device with IP [{0}] fetching Device Information over http.", Type = EventLogEntryType.Information },
            new EventLogDefinition { Code = (int) EventLogEntryCodes.FetchException, Source = (int) EventLogEntryCodes.SourceDeviceFetch,  StringFormat = "Exception from Device with IP [{0}] fetching Device Information over http. {1}", Type = EventLogEntryType.Error },
        };
    }
}