using System;
using System.Collections.Generic;
using System.Text;

namespace dotLua
{
    /// <summary>
    /// Handles the garbage collector. 
    /// </summary>
    public sealed class LuaGC
    {
        private Lua state = null;

        /// <summary>
        /// Constructs a new LuaGC 
        /// </summary>
        /// <param name="state">A LUA state.</param>
        public LuaGC(Lua state)
        {
            if (state == null)
            { // State must not be null
                throw new ArgumentNullException("state must not be null.", "state");
            }
            this.state = state;
        }

        /// <summary>
        /// Returns the amount of currently used memory.
        /// </summary>
        public int TotalMemory
        {
            get
            {
                return NativeLua.lua_getgccount(state.Handle);
            }
        }

        /// <summary>
        /// Sets or retrieves the garbage collectors threshold in KBytes
        /// </summary>
        public int Threshold
        {
            get
            { // Return threshold
                return NativeLua.lua_getgcthreshold(state.Handle);
            }
            set
            { // Set new threshold
                NativeLua.lua_setgcthreshold(state.Handle, value);
            }
        }

        /// <summary>
        /// Immediatly runs the garbage collector.
        /// </summary>
        public void Collect()
        {
            int current = Threshold;
            // Set threshold to zero
            Threshold = 0;
            // Set it back
            Threshold = current;
        }
    }
}
