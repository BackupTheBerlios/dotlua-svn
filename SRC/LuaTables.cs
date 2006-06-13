using System;
using System.Collections.Generic;
using System.Text;

namespace dotLua
{
    /// <summary>
    /// Contains a dictonary of all tables available.
    /// </summary>
    public sealed class LuaTables
    {
        private Lua state = null;

        private LuaTable registry = null;
        private LuaTable global = null;

        /// <summary>
        /// Internal enum for special tables
        /// </summary>
        private enum SpecialTables
        {
            Registry = -10000,      // Registry
            Global = -10001         // Global
        }

        /// <summary>
        /// Returns a table that can be used for internal storage
        /// </summary>
        public LuaTable Registry
        {
            get
            {
                return registry;
            }
        }

        /// <summary>
        /// Returns a table representing the global namespace
        /// </summary>
        public LuaTable Global
        {
            get
            {
                return global;
            }
        }
        
        /// <summary>
        /// Constructs a new table collection from the given state.
        /// </summary>
        /// <param name="state">State</param>
        public LuaTables(Lua state)
        {
            if (state == null)
            { // state must not be null
                throw new ArgumentNullException("state");
            }
            this.state = state;
            // Special ones
            registry = new LuaTable(state, (int)SpecialTables.Registry);
            global = new LuaTable(state, (int)SpecialTables.Global);
        }

        /// <summary>
        /// Pushes a new table on top of the stack
        /// </summary>
        /// <returns>Returns the index of the newly created table.</returns>
        public LuaTable Add()
        {
            int ret = 0;

            NativeLua.lua_newtable(state.Handle);
            ret = state.Stack.TopIndex;

            return new LuaTable(state, ret);
        }

        /// <summary>
        /// Inserts a new table to specified location on the stack.
        /// </summary>
        /// <param name="index">Location where the new table should be inserted.</param>
        public void Insert(int index)
        {
            Add(); // Add it
            // Then move it somewhere
            state.Stack.Insert(index);
        }

        /// <summary>
        /// Removes the specified table. 
        /// </summary>
        /// <param name="index">Table to be removed</param>
        public void Remove(int index)
        {
            if (state.Stack.TypeOf(index) != LuaType.Table)
            { // Not a valid table
                throw new LuaException("Specified index is not a table.");
            }
            // Remove this element
            state.Stack.Remove(index);
        }
    }
}
