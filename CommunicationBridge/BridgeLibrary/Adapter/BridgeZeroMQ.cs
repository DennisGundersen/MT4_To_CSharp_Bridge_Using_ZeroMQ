using NetMQ;
using NetMQ.Sockets;
using System.ServiceModel.Channels;
using System.Text;

namespace BridgeLibrary.Adapter
{
    public class BridgeZeroMQ : Base
    {
        private Encoding encoding = Encoding.UTF8;
        private readonly ResponseSocket Server = new();
        private readonly CancellationTokenSource CancellationToken = new();
        const string DEF_ADDRESS = "tcp://127.0.0.1:9001";

        public BridgeZeroMQ() : base("ZeroMQ Bridge")
        {

        }

        #region IBridge
        
        public string ServerAddress { get; private set; } = String.Empty;

        public override void Configure(Dictionary<string, string> config)
        {
            config.TryGetValue("address", out string? address);
            ServerAddress = String.IsNullOrEmpty(address) ? DEF_ADDRESS : address;

        }

        public override void Start()
        {
            using (var runtime = new NetMQRuntime())
            {
                Server.Bind(ServerAddress);
                runtime.Run(CancellationToken.Token, ServerAsync(CancellationToken.Token));
            }
        }

        public override void Stop()
        {
            Console.WriteLine("Stopping using cancellation token");
            CancellationToken.Cancel();
        }

        public override void Dispose()
        {
            CancellationToken.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion

        #region implementation
        private async Task ServerAsync(CancellationToken token)
        {
            Console.WriteLine("Waiting for messages...");
            while (!token.IsCancellationRequested)
            {
                var command = await ReceiveCommand(token);
                Console.WriteLine("Received command: `{0}`(params {1}:{2})", command.Name, command.Data.Length, String.Join(',', command.Data));
                string res = Execute(command);
                Respond(res);
            }

        }

        protected override Task<Command> ReceiveCommand(CancellationToken token)
        {
            return ReceiveCommandInternal(token);
        }

        private async Task<Command> ReceiveCommandInternal(CancellationToken token)
        {
            var (data, more) = await Server.ReceiveFrameStringAsync(encoding, token);
            string name = data;
            Console.WriteLine("Received message: `{0}` (more: {1})", data, more);
            List<string> args = new();
            while(more)
            {
                (data, more) = await Server.ReceiveFrameStringAsync(encoding, token);
                args.Add(data);
            }
            return new Command(name, args.ToArray());

        }

        protected override void Respond(string response)
        {
            Server.SendFrame(response);
        }


        #endregion
    }
}