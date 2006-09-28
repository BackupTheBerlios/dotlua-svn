using System;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace dotLua
{
    /// <summary>
    /// This class contains native Interop calls to our
    /// lua_stdcall DLL. This class is for internal use only.
    /// </summary>
    sealed class NativeLua
    {
        private const string dllname = "lua_stdcall.dll";

        private const int LUA_IDSIZE = 60; // internal value

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi, Pack=1)]
        public struct luaDebug
        {
            public int _event;
            [MarshalAs(UnmanagedType.LPStr)]
            public string _name;
            [MarshalAs(UnmanagedType.LPStr)]
            public string _namewhat;
            [MarshalAs(UnmanagedType.LPStr)]
            public string _what;
            [MarshalAs(UnmanagedType.LPStr)]
            public string _source;
            public int _curline;
            public int _upvalues;
            public int _linedef;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=LUA_IDSIZE)]
            public string _shortsrc;
            /*
             *  here begins internal stuff we don't care about
             */
        }

        [DllImport(dllname, CharSet=CharSet.Ansi)]
        public static extern int lua_getinfo (IntPtr state, string what, ref luaDebug debug);



        [DllImport(dllname, CharSet = CharSet.Ansi)]
        public static extern int lua_getstack(IntPtr state, int level, ref luaDebug debug); 
 
        [DllImport(dllname, CharSet = CharSet.Ansi)]
        public static extern IntPtr lua_open();

        [DllImport(dllname, CharSet = CharSet.Ansi)]
        public static extern int luaopen_base(IntPtr state);

        [DllImport(dllname, CharSet = CharSet.Ansi)]
	    public static extern int luaopen_math(IntPtr state);

        [DllImport(dllname, CharSet = CharSet.Ansi)]
	    public static extern int luaopen_string(IntPtr state);

        [DllImport(dllname, CharSet = CharSet.Ansi)]
	    public static extern int luaopen_debug(IntPtr state);

        [DllImport(dllname, CharSet = CharSet.Ansi)]
	    public static extern int luaopen_table(IntPtr state);

        [DllImport(dllname, CharSet = CharSet.Ansi)]
        public static extern int luaopen_io(IntPtr state);

        [DllImport(dllname, CharSet = CharSet.Ansi)]
        public static extern void lua_close(IntPtr state);

        [DllImport(dllname, CharSet = CharSet.Ansi)]
        public static extern int luaL_loadfile(IntPtr state, [MarshalAs(UnmanagedType.LPStr)]string file);

        [DllImport(dllname, CharSet = CharSet.Ansi)]
        public static extern int luaL_loadbuffer(IntPtr state, [MarshalAs(UnmanagedType.LPStr)]string buffer, int size, [MarshalAs(UnmanagedType.LPStr)]string name);

        [DllImport(dllname, CharSet = CharSet.Ansi)]
        public static extern LuaError lua_pcall(IntPtr state, int args, int ret, int error);

        [DllImport(dllname)]
        public static extern void lua_call(IntPtr state, int args, int ret);

        // Stack functions
        [DllImport(dllname, CharSet = CharSet.Ansi)]
        public static extern int lua_gettop(IntPtr state);

        [DllImport(dllname)]
        public static extern int lua_checkstack(IntPtr state, int extra);

        [DllImport(dllname)]
        public static extern void lua_settop(IntPtr state, int index);

        [DllImport(dllname)]
        public static extern void lua_pushvalue(IntPtr state, int index);

        [DllImport(dllname)]
        public static extern void lua_remove(IntPtr state, int index);

        [DllImport(dllname)]
        public static extern void lua_insert(IntPtr state, int index);

        [DllImport(dllname)]
        public static extern void lua_replace(IntPtr state, int index);

        [DllImport(dllname)]
        public static extern int lua_type(IntPtr state, int index);

        [DllImport(dllname)]
        public static extern int lua_equal(IntPtr state, int index1, int index2);

        [DllImport(dllname)]
        public static extern int lua_rawequal(IntPtr state, int index1, int index2);

        [DllImport(dllname)]
        public static extern int lua_lessthan(IntPtr state, int index1, int index2);

        [DllImport(dllname)]
        public static extern void lua_concat(IntPtr state, int n);


        // Retrieve methods
        [DllImport(dllname)]
        public static extern int lua_toboolean (IntPtr state, int index);

        [DllImport(dllname)]
        public static extern double lua_tonumber (IntPtr state, int index);

        [DllImport(dllname, CharSet=CharSet.Ansi)]
        public static extern string lua_tostring (IntPtr state, int index);

        [DllImport(dllname)]
        public static extern int lua_strlen (IntPtr state, int index);

        [DllImport(dllname)]
        public static extern IntPtr lua_tocfunction (IntPtr state, int index);

        [DllImport(dllname)]
        public static extern IntPtr lua_touserdata (IntPtr state, int index);

        [DllImport(dllname)]
        public static extern IntPtr lua_tothread (IntPtr state, int index);

        [DllImport(dllname)]
        public static extern IntPtr lua_topointer (IntPtr state, int index);

        [DllImport(dllname)]
        public static extern void lua_pushboolean       (IntPtr state, int boolean);
        [DllImport(dllname)]
        public static extern void lua_pushnumber        (IntPtr state, double number);
        [DllImport(dllname, CharSet=CharSet.Ansi)]
        public static extern void lua_pushlstring       (IntPtr state, [MarshalAs(UnmanagedType.LPStr)]string str, int length);
        [DllImport(dllname, CharSet = CharSet.Ansi)]
        public static extern void lua_pushstring        (IntPtr state, [MarshalAs(UnmanagedType.LPStr)]string str);
        [DllImport(dllname)]
        public static extern void lua_pushnil           (IntPtr state);
        [DllImport(dllname)]
        public static extern void lua_pushcclosure(IntPtr state, [MarshalAs(UnmanagedType.FunctionPtr)]LuaCallbackFunction function, int size);

        public static void lua_pushcfunction(IntPtr state, [MarshalAs(UnmanagedType.FunctionPtr)]LuaCallbackFunction function)
        {
            lua_pushcclosure(state, function, 0);
        }

        [DllImport(dllname)]
        public static extern LuaCallbackFunction lua_atpanic(IntPtr state, [MarshalAs(UnmanagedType.FunctionPtr)]LuaCallbackFunction panicf);


        [DllImport(dllname)]
        public static extern void lua_pushlightuserdata (IntPtr state, IntPtr data);
              
        [DllImport(dllname)]
        public static extern void lua_newtable(IntPtr state);

        [DllImport(dllname)]
        public static extern void lua_settable(IntPtr state, int table);

        [DllImport(dllname)]
        public static extern void lua_gettable(IntPtr state, int table);

        [DllImport(dllname)]
        public static extern int lua_getgccount(IntPtr state);
        [DllImport(dllname)]
        public static extern int lua_getgcthreshold(IntPtr state);
        [DllImport(dllname)]
        public static extern void lua_setgcthreshold(IntPtr state, int newthreshold);

        [DllImport(dllname)]
        public static extern int lua_next(IntPtr state, int index);

        [DllImport(dllname)]
        public static extern IntPtr lua_newthread(IntPtr state);

        [DllImport(dllname)]
        public static extern int lua_resume(IntPtr state, int args);

        [DllImport(dllname)]
        public static extern int lua_yield(IntPtr state, int retvals);

        [DllImport(dllname)]
        public static extern void lua_xmove(IntPtr source, IntPtr dest, int n);
    }
}
