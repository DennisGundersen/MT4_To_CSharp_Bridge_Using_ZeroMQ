using NetMQ;
using NetMQ.Sockets;

namespace BridgeLibrary.Adapter
{
    public class BridgeZeroMQ : Base, IBridge
    {
        private readonly CancellationTokenSource CancellationToken = new();
        const string DEF_ADDRESS = "tcp://127.0.0.1:9001";
        const string CONNECT_MESSAGE = "HELO";
        const string CONNECT_MESSAGE_ACK = "EHLO";

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
                runtime.Run(CancellationToken.Token, ServerAsync(CancellationToken.Token));
            }
        }

        public void Stop()
        {
            Console.WriteLine("Stopping using cancellation token");
            CancellationToken.Cancel();
        }

        public void Dispose()
        {
            CancellationToken.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion

        #region implementation
        private async Task ServerAsync(CancellationToken token)
        {
            using (var server = new ResponseSocket(ServerAddress))
            {
                Console.WriteLine("Waiting for messages...");
                while (!token.IsCancellationRequested)
                {
                    var (message, more) = await server.ReceiveFrameStringAsync(token);
                    Console.WriteLine(String.Format("Received message: `{0}` (more: {1})", message, more));
                    string res = (message == CONNECT_MESSAGE) ? CONNECT_MESSAGE_ACK : "OK " + message;
                    Console.WriteLine("Sending response `{0}`", res);
                    server.SendFrame(res);
                }

            }
        }

        protected override void ReceiveCommand(out string command, out string data)
        {
            throw new NotImplementedException();
        }

        protected override void Respond(string response)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}