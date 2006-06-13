using System;
using System.Collections.Generic;
using System.Text;
using dotLua;

namespace dotLuaTest
{
    [LuaLibrary("1.0.0")] // Mark as LUA library
    public class Screen
    {
        public static int WriteLine(IntPtr state)
        {
            LuaFunctionArgs arg = new LuaFunctionArgs(state);
            List<object> list = new List<object>();
            
            if (arg.ArgumentCount > 0)
            {
                string format = (string)arg[0];
                string written;

                for (int i = 1; i < arg.ArgumentCount; ++i)
                {
                    list.Add(arg[i]);
                }

                written = string.Format(format, list.ToArray());
                Console.WriteLine(written);

                arg.ReturnValues.Add(written.Length);
            }

            return arg.EndMethod();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Lua lua = null;
            /*
            lua.Register("writeline", new LuaCallbackFunction(WriteLine));
            lua.ProtectedCall();

            lua.Tables.Global.Push("hello", "world");
            Console.WriteLine(lua.Tables.Global.Pop("hello"));
            Console.WriteLine(lua.Tables.Global.Pop("hello"));*/
            try
            {
                lua = new Lua("main.lua");
                lua.LoadLibrary(typeof(Screen));
                lua.Execute(); 
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }           
           
            Console.ReadKey();
        }
    }
}


