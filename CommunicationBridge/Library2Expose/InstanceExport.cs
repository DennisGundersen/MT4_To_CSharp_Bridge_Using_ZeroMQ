using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library2Expose
{
    public class InstanceExport
    {
        public int PublicMethod1() => 0;
        public int PublicMethod2(int a) => a;
        public int PublicMethod3(int a, string b, bool c) => c ? a : int.Parse(b);
        public void PublicMethod4() { }

        protected int ProtectedMethod1() => 0;
        protected int ProtectedMethod2(int a) => a;
        protected int ProtectedMethod3(int a, string b, bool c) => c ? a : int.Parse(b);

        private int PrivateMethod1() => 0;
        private int PrivateMethod2(int a) => a;
        private int PrivateMethod3(int a, string b, bool c) => c ? a : int.Parse(b);

        public static int StaticPublicMethod1() => 0;
        public static int StaticPublicMethod2(int a) => a;
        public static int StaticPublicMethod3(int a, string b, bool c) => c ? a : int.Parse(b);

        private static int StaticPrivateMethod1() => 0;
        private static int StaticPrivateMethod2(int a) => a;
        private static int StaticPrivateMethod3(int a, string b, bool c) => c ? a : int.Parse(b);


    }
}
