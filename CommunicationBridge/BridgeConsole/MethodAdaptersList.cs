namespace BridgeConsole
{
    internal partial class Program
    {
        private static Dictionary<string, BridgeLibrary.Adapter.Base.Cmd> GeneratedAvailableCommands()
        {
            return new() {
                { "Library2Expose_InstanceExport_PublicMethod1", commands.Library2Expose_InstanceExport_PublicMethod1 },
                { "Library2Expose_InstanceExport_PublicMethod2", commands.Library2Expose_InstanceExport_PublicMethod2 },
                { "Library2Expose_InstanceExport_StaticPublicMethod1", commands.Library2Expose_InstanceExport_StaticPublicMethod1 },
                { "Library2Expose_InstanceExport_StaticPublicMethod2", commands.Library2Expose_InstanceExport_StaticPublicMethod2 }
            };
        }
    }
}
