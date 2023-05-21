using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeLibrary.Adapter
{
    public readonly record struct Command(string Name, string[] Data);
}
