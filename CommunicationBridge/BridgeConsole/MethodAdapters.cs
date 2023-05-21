using BridgeLibrary.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeConsole
{
    internal class MethodAdapters
    {
        readonly Library2Expose.InstanceExport Library2Expose_InstanceExport = new();
        //readonly Library2Expose.StaticExport Library2Expose_StaticExport = new();
        public MethodAdapters() { }

        
        public string Library2Expose_InstanceExport_PublicMethod1(string[] args)
        {
            return Library2Expose_InstanceExport.PublicMethod1().ToString();
        }

        public string Library2Expose_InstanceExport_PublicMethod2(string[] args)
        {
            bool r;
            System.Int32 arg0;
            r = System.Int32.TryParse(args[0], out arg0);
            if (!r) return String.Format("ERR: argument 0 ({0}) couldn't be parsed as {1}", args[0], "System.Int32");
            return Library2Expose_InstanceExport.PublicMethod2(arg0).ToString();
        }

        public string Library2Expose_InstanceExport_StaticPublicMethod1(string[] args)
        {
            return Library2Expose.InstanceExport.StaticPublicMethod1().ToString();
        }
        public string Library2Expose_InstanceExport_StaticPublicMethod2(string[] args)
        {
            bool r;
            System.Int32 arg0;
            r = System.Int32.TryParse(args[0], out arg0);
            if (!r) return String.Format("ERR: argument 0 ({0}) couldn't be parsed as {1}", args[0], "System.Int32");
            return Library2Expose.InstanceExport.StaticPublicMethod2(arg0).ToString();
        }

    }
}

