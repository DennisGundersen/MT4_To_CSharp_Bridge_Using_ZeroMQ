using BridgeLibrary;
using BridgeLibrary.Adapter;
using static BridgeLibrary.Adapter.Base;

namespace BridgeConsole
{
    internal partial class Program
    {
        private static MethodAdapters commands = new();
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing...");
            using (Base server = new BridgeZeroMQ())
            {
                var address = args.Length == 0 || String.IsNullOrEmpty(args[0]) ? "tcp://127.0.0.1:9001" : args[0];
                Dictionary<string, string> defaultConfig = new()
                {
                    {"address", address }
                };

                // Wire up the CTRL+C handler
                Console.CancelKeyPress += (sender, e) =>
                {
                    Console.WriteLine("Cancel Key Pressed");
                    server.Stop();
                };
                server.Configure(defaultConfig);
                Console.WriteLine(String.Format("Starting server {0} listening on `{1}`", server.Name, defaultConfig["address"]));
                server.AddCommands(PrepareCommands());
                server.AddCommands(GeneratedAvailableCommands());
                server.Start();
            }
        }

        /// <summary>
        /// Hand coded list
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, Cmd> PrepareCommands()
        {
            return new() {
                { "A", commands.Library2Expose_InstanceExport_PublicMethod1 },
                { "B", commands.Library2Expose_InstanceExport_PublicMethod2 },
                { "SA", commands.Library2Expose_InstanceExport_StaticPublicMethod1 },
                { "SB", commands.Library2Expose_InstanceExport_StaticPublicMethod2 }
            };
        }
    }
}