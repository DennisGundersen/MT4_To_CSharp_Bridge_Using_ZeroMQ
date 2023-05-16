using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeLibrary
{
    public interface IBridge: IDisposable
    {
        public string Name { get; }
        public void Configure(Dictionary<string, string> config);
        public void Start();
        public void Stop();
    }
}
