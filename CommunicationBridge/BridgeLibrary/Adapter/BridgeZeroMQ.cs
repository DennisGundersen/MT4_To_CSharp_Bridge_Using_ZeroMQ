using NetMQ;
using NetMQ.Sockets;

namespace BridgeLibrary.Adapter
{
    public class BridgeZeroMQ : IBridge
    {
        const string DEF_ADDRESS = "tcp://localhost:9001";
        #region IBridge
        public string Name => "ZeroMQ Bridge";

        public string ServerAddress { get; private set; } = String.Empty;

        public void Configure(Dictionary<string, string> config)
        {
            config.TryGetValue("address", out string? address);
            ServerAddress = String.IsNullOrEmpty(address) ? DEF_ADDRESS : address;

        }

        public void Start()
        {
            using (var runtime = new NetMQRuntime())
            {
                runtime.Run(ServerAsync());
            }

            
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region implementation
        private async Task ServerAsync()
        {
            using (var server = new ResponseSocket(ServerAddress))
            {
                var (message, more) = await server.ReceiveFrameStringAsync();
                Console.WriteLine(String.Format("Received message: `{0}` (more: {1})", message, more));
            }
        }
        #endregion
    }
}