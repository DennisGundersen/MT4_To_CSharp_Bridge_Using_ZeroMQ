using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BridgeLibrary.Adapter
{
    public abstract class Base
    {
        public delegate string Cmd(string input);
        protected Dictionary<string, Cmd> BaseCommands;
        protected Dictionary<string, Cmd> CustomCommands;
        private string version = "";
        protected Base()
        {
            version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "?";
            BaseCommands = new Dictionary<string, Cmd>()
            {
                { "HELO", Welcome },
                { "Version", Version },
                { "Tic", Tac }
            };
            CustomCommands = new Dictionary<string, Cmd>();
        }

        public void AddCommands(Dictionary<string, Cmd> newCommands)
        {
            foreach (var command in newCommands)
            {
                CustomCommands[command.Key] = command.Value;
                //UserCommands.TryAdd(command.Key, command.Value);
            }
        }

        public string Execute(string command, string args)
        {
            Cmd? cmd = null;
            if (BaseCommands.TryGetValue(command, out cmd))
            {
                Console.WriteLine("Executing Standard Command: {0}({1})", command, args);
                return cmd.Invoke(args);
            }
            if (CustomCommands.TryGetValue(command, out cmd)) 
            {
                Console.WriteLine("Executing Custom Command: {0}({1})", command, args);
                return cmd.Invoke(args); 
            }
            Console.WriteLine("Received unknown command {0}({1})", command, args);
            return "ERR: Unknown command";
        }

        public string Welcome(string input) => "EHLO";

        public string Version(string input) => version;

        public string Tac(string input) => "Tac " + input;

        protected abstract void ReceiveCommand(out string command, out string data);
        protected abstract void Respond(string response);
    }
}
