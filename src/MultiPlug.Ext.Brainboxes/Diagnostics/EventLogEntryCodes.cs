
namespace MultiPlug.Ext.Brainboxes.Diagnostics
{
    internal enum EventLogEntryCodes
    {
        Reserved = 0,
        SourceDevice = 1,
        SourceDeviceConnection = 2,
        SourceDeviceFetch = 3,


        DeviceCreated = 21,
        MACMissMatch = 22,
        MACAddressSet = 23,
        Connecting = 24,
        Connected = 25,
        PropertyChangedTrue = 26,
        PropertyChangedFalse = 27,
        NoIPSet = 28,
        ConnectionException = 29,
        FetchSuccessful = 30,
        FetchException = 31,
    }
}
