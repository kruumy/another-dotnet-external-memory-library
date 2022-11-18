using System.Diagnostics;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class MiscExtenstions
    {
        public static void print(this object input, bool endl = true)
        {
            if (input is string s) Console.Write(s);
            else if (input is byte[] ba) Console.Write(ba.GetHexString());
            else if (input is byte b) Console.Write(b.GetHexString());
            else
            {
                try
                {
                    System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true,
                        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.Strict

                    };
                    Console.Write(System.Text.Json.JsonSerializer.Serialize(input, input.GetType(), options));
                }
                catch
                {
                    Console.Write(input.ToString());
                }

            }

            if (endl) Console.WriteLine();
        }
        public static ProcessModule GetByName(this ProcessModuleCollection modules, string name)
        {
            foreach (ProcessModule item in modules)
                if (item.ModuleName == name)
                    return item;
            throw new Exception("Name Did Not Match Any ModuleNames");
        }
    }
}
