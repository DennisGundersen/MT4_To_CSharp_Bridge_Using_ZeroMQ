using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BridgeLibrary.Adapter
{
    public abstract class Base : IBridge
    {
        public delegate string Cmd(string[] input);
        protected Dictionary<string, Cmd> BaseCommands;
        protected Dictionary<string, Cmd> CustomCommands;
        private string version = "";

        public string Name { get; private set; } = "?";

        protected Base(string name)
        {
            version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "?";
            BaseCommands = new Dictionary<string, Cmd>()
            {
                { "HELO", Welcome },
                { "Version", Version },
                { "Tic", Tac }
            };
            CustomCommands = new Dictionary<string, Cmd>();
            Name = name;
        }

        public void AddCommands(Dictionary<string, Cmd> newCommands)
        {
            foreach (var command in newCommands)
            {
                CustomCommands[command.Key] = command.Value;
                //UserCommands.TryAdd(command.Key, command.Value);
            }
        }

        public string Execute(Command command)
        {
            Cmd? cmd = null;
            if (BaseCommands.TryGetValue(command.Name, out cmd))
            {
                Console.WriteLine("Executing Standard Command: {0}({1})", command.Name, command.Data);
                return cmd.Invoke(command.Data);
            }
            if (CustomCommands.TryGetValue(command.Name, out cmd))
            {
                Console.WriteLine("Executing Custom Command: {0}({1})", command.Name, command.Data);
                return cmd.Invoke(command.Data);
            }
            Console.WriteLine("Received unknown command {0}({1})", command.Name, String.Join(" | ", command.Data));
            return "ERR: Unknown command";
        }

        public string Welcome(string[] input) => "EHLO";

        public string Version(string[] input) => version;

        public string Tac(string[] input) => "Tac " + input;

        protected abstract Task<Command> ReceiveCommand(CancellationToken token);
        protected abstract void Respond(string response);

        public abstract void Configure(Dictionary<string, string> config);

        public abstract void Start();

        public abstract void Stop();

        public abstract void Dispose();

    }
}
