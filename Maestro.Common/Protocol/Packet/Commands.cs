namespace Maestro.Common.Protocol.Packet
{
    public enum Commands : byte
    {
        ProtocolVersion = 0,
        Descriptor = 1,
        PackageData = 2,
        GetValues = 3,
        SetValues = 4,
        Reset = 5,
        CopySetup = 6,
        ActiveView = 7,
        ShowMessage = 8,
        MessageResult = 9,
        PluginCommand = 10,
    }
}