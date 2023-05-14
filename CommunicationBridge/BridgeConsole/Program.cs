using BridgeLibrary;
using BridgeLibrary.Adapter;

namespace BridgeConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing...");
            IBridge server = new BridgeZeroMQ();
            Dictionary<string, string> defaultConfig = new()
            {
                {"address", "tcp://localhost:9001" }
            };
            server.Configure(defaultConfig);
            Console.WriteLine(String.Format("Starting server listening on `{0}`", defaultConfig["address"]));
            server.Start();
        }
    }
}