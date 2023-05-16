using System.Reflection;

namespace BridgeGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }
            foreach(var arg in args) 
            { 
                Assembly assembly = Assembly.LoadFrom(arg);
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                    if (methods.Length > 0)
                    {
                        Console.WriteLine(type.FullName);

                        foreach (MethodInfo method in methods)
                        {
                            Console.WriteLine(String.Format("\t{0}({1}) : {2}", method.Name, String.Join(", ", method.GetParameters().Select(i => i.ParameterType.ToString()).ToArray()), method.ReturnType.ToString()));
                        }
                    }
                }
            }
        }
    }
}