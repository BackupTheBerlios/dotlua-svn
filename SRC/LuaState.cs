using System;
using System.Collections.Generic;
using System.Text;

namespace dotLua
{
    /// <summary>
    /// Represents a Lua namespace state.
    /// </summary>
    public class LuaState : IDisposable
    {
        /// <summary>
        /// The internal state handle.
        /// </summary>
        protected IntPtr lua = IntPtr.Zero;
        /// <summary>
        /// The tables associated with the state.
        /// </summary>
        protected LuaTables tables = null;
        /// <summary>
        /// The stack associated with the state.
        /// </summary>
        protected LuaStack stack = null;
        /// <summary>
        /// The garbage collector associated with the state.
        /// </summary>
        protected LuaGC gc = null;

        /// <summary>
        /// Creates a new empty LUA state object.
        /// </summary>
        public LuaState()
        {
            lua = NativeLua.lua_open();
            if (lua == IntPtr.Zero)
            { // Error initializing subsystem
                throw new LuaException("Error initializing LUA subsystem");
            }
            stack = new LuaStack(this);
            tables = new LuaTables(this);
            gc = new LuaGC(this);
        }

        /// <summary>
        /// Creates a new LUA state from the given state. The class will not try load the
        /// default libraries for this instance.
        /// </summary>
        /// <param name="state">An existing LUA state.</param>
        public LuaState(IntPtr state)
        {
            if (state == IntPtr.Zero)
            { // Invalid parameter
                throw new ArgumentException("State must not be zero.", "state");
            }
            lua = state;
            stack = new LuaStack(this);
            tables = new LuaTables(this);
            gc = new LuaGC(this);
        }

        /// <summary>
        /// Closes the LUA state to clean up all unmanaged handles.
        /// </summary>
        public void Dispose()
        {
            if (lua != IntPtr.Zero)
            { // Close and dispose our object
                NativeLua.lua_close(lua);
                lua = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Allows operations on the garbage collector of LUA
        /// </summary>
        public LuaGC GC
        {
            get
            {
                return gc;
            }
        }

        /// <summary>
        /// A global collection on the tables within this state.
        /// </summary>
        public LuaTables Tables
        {
            get
            {
                return tables;
            }
        }


        /// <summary>
        /// Returns the stack frame for this LUA state.
        /// </summary>
        public LuaStack Stack
        {
            get
            {
                return stack;
            }
        }


        /// <summary>
        /// Returns the internal native handle of the state. This handle can be
        /// passed to any other native LUA C API.
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                return lua;
            }
        }
    }
}
