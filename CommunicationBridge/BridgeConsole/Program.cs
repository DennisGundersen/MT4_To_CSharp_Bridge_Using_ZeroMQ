using BridgeLibrary;
using BridgeLibrary.Adapter;

namespace BridgeConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing...");
            using (IBridge server = new BridgeZeroMQ())
            {
                Dictionary<string, string> defaultConfig = new()
                {
                    {"address", "tcp://127.0.0.1:9001" }
                };

                // Wire up the CTRL+C handler
                Console.CancelKeyPress += (sender, e) =>
                {
                    Console.WriteLine("Cancel Key Pressed");
                    server.Stop();
                };
                server.Configure(defaultConfig);
                Console.WriteLine(String.Format("Starting server {0} listening on `{1}`", server.Name, defaultConfig["address"]));
                server.Start();
            }
        }
    }
}