namespace Library2Expose
{
    public static class StaticExport
    {
        public static int PublicMethod1() => 0;
        public static int PublicMethod2(int a) => a;
        public static int PublicMethod3(int a, string b, bool c) => c ? a : int.Parse(b);

        private static int PrivateMethod1() => 0;
        private static int PrivateMethod2(int a) => a;
        private static int PrivateMethod3(int a, string b, bool c) => c ? a : int.Parse(b);


    }
}