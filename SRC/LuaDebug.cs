using System;
using System.Collections.Generic;
using System.Text;

namespace dotLua
{
    /// <summary>
    /// Interface for providing debug information.
    /// </summary>
    public sealed class LuaDebug
    {
        private NativeLua.luaDebug info;
        private Lua state = null;
        int level = -1;

        /// <summary>
        /// Constructs an empty LuaDebug object.
        /// </summary>
        public LuaDebug()
        {
            info._curline = info._event = info._linedef = 0;
            info._name = info._namewhat = info._source = "";
            info._what = "";
            info._upvalues = 0;
        }

        /// <summary>
        /// Retrieve extended information
        /// </summary>
        private void RetrieveInfo(string what)
        {
            int ret = 0;

            ret = NativeLua.lua_getinfo(state.Handle, what, ref info);
            if (ret == 0)
            { // Error
                throw new LuaException("Error while retrieving further debug information");
            }
        }

        /// <summary>
        /// Constructs a new LuaDebug object containing information about the given level.
        /// Level 0 is the current called function, thereas "level+1" is the level which called "level".
        /// </summary>
        /// <param name="state">The LUA state.</param>
        /// <param name="level">The level to retrieve.</param>
        /// <returns>A filled LuaDebug object.</returns>
        public static LuaDebug FromLevel(Lua state, int level)
        {
            LuaDebug obj = new LuaDebug();
            int ret = 0;

            obj.state = state;
            obj.level = level;
            // Retrieve information
            ret = NativeLua.lua_getstack(state.Handle, level, ref obj.info);
            if (ret == 0)
            { // Error
                throw new LuaException("Called with a depth greater than the stack depth.");
            }
            else if (ret == 1)
            { // Everything is okay
                // Retrieve extended information
                obj.RetrieveInfo("nSlu");
                // And return the handle
                return obj;
            }
            else
            { // Unknown return value/error
                throw new LuaException("Unknown error");
            }
        }

        /// <summary>
        /// Construcs a new LuaDebug object containing information about a non-active
        /// function from the given table.
        /// <remarks>A debug object from a non-active function does neither have a caller nor a level.</remarks>
        /// </summary>
        /// <param name="state">The LUA state</param>
        /// <param name="name">Name of the function.</param>
        /// <param name="table">Table which holds the function.</param>
        /// <returns>A filled LuaDebug object.</returns>
        public static LuaDebug FromFunction(Lua state, string name, LuaTable table)
        {
            LuaDebug debug = new LuaDebug();

            // Assign the state
            debug.state = state;
            // Retrieve the value
            state.Stack.Push(name);
            table.GetTable();
            // Retrieve information:
            // The '>' tells the LUA subsystem to search for a
            // non-active function.
            debug.RetrieveInfo(">nSlu");

            return debug;
        }

        /// <summary>
        /// Retrieves the current line of execution.
        /// </summary>
        public int CurrentLine
        {
            get
            {
                return info._curline;
            }
        }

        /// <summary>
        /// Retrieves the line number where the definition of the function starts. 
        /// If no information is available it retrieves -1.
        /// </summary>
        public int Line
        {
            get
            {
                return info._linedef;
            }
        }

        /// <summary>
        /// Returns the name of the source. If the function was defined in a string
        /// the name of the string is being returned. If the function was defined in a file, 
        /// then source starts with a '@' followed by the file name. 
        /// </summary>
        public string Source
        {
            get
            {
                return info._source;
            }
        }

        /// <summary>
        /// A shorter "printable" version of Source.
        /// </summary>
        public string ShortSource
        {
            get
            {
                return info._shortsrc;
            }
        }

        /// <summary>
        /// Returns the string "Lua" if this is a Lua function, "C" if this is a 
        /// callback function, "main" if this is the main part of a chunk, and "tail" 
        /// if this was a function that did a tail call. In the latter case, Lua 
        /// has no other information about this function. 
        /// </summary>
        public string What
        {
            get
            {
                return info._what;
            }
        }

        /// <summary>
        /// Retrieves a reasonable name for the given function. Because functions in Lua are 
        /// first class values, they do not have a fixed name: Some functions may be the value
        /// of multiple global variables, while others may be stored only in a table field. The 
        /// lua_getinfo function checks how the function was called or whether it is the value of
        /// a global variable to find a suitable name. If it cannot find a name, then name is set to null. 
        /// </summary>
        public string Name
        {
            get
            {
                return info._name;
            }
        }

        /// <summary>
        /// Returns an explanation for the "Name" property. The value of namewhat can be "global",
        /// "local", "method", "field", or "" (the empty string), according to how the function was called.
        /// (Lua uses the empty string when no other option seems to apply.) 
        /// </summary>
        public string NameWhat
        {
            get
            {
                return info._namewhat;
            }
        }

        /// <summary>
        /// Retrieves the number of upvalues of the function. 
        /// </summary>
        public int UpValues
        {
            get
            {
                return info._upvalues;
            }
        }

        /// <summary>
        /// Safe check if the current level has a parent level (a caller).
        /// </summary>
        public bool HasCaller
        {
            get
            {
                if (level > -1)
                {
                    int ret = 0;
                    NativeLua.luaDebug debug = new NativeLua.luaDebug();
                    ret = NativeLua.lua_getstack(state.Handle, level + 1, ref debug);
                    if (ret == 1)
                    { // Yes it has one
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Returns the parent level (the caller) for the given debug level. If
        /// it has been constructed from a given level and the level has a parent it
        /// returns a LuaDebug object representing the caller of this level, otherwise
        /// a LuaException is being thrown.
        /// </summary>
        public LuaDebug Caller
        {
            get
            {
                if (level == -1)
                { // error
                    throw new LuaException("LuaDebug was constructed from a non active function.");
                }
                // parent one
                LuaDebug parent = LuaDebug.FromLevel(state, level + 1);

                return parent;
            }
        }

        /// <summary>
        /// Retrieves the level from which the debug object has been created.
        /// </summary>
        public int Level
        {
            get
            {
                return level;
            }
        }
    }
}
