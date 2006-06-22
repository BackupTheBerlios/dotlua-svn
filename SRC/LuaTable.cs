using System;
using System.Collections.Generic;
using System.Text;

namespace dotLua
{
    /// <summary>
    /// Represents a LUA table and provides operations to modify those.
    /// </summary>
    public sealed class LuaTable
    {
        private int index = 0;
        private Lua state = null;

        /// <summary>
        /// Constructs a table that belongs to the given state identified by the given index on the stack
        /// of the given state.
        /// </summary>
        /// <param name="state">The associated LUA state</param>
        /// <param name="index">Index identifying the table.</param>
        public LuaTable(Lua state, int index)
        {
            if (state == null)
            { // Error state must not be null
                throw new ArgumentNullException("state");
            }
            this.state = state;
            this.index = index;
        }

        /// <summary>
        /// Returns the index of the table.
        /// </summary>
        public int Index
        {
            get
            {
                return index;
            }
        }

        /// <summary>
        /// Sets this table. Setting a table pops the last two items from
        /// the stack and treats the first as the key and the second as the value.
        /// </summary>
        public void SetTable()
        {
            NativeLua.lua_settable(state.Handle, index);
        }

        /// <summary>
        /// Adds the specified key/nil pair to the table.
        /// </summary>
        /// <param name="key"></param>
        public void SetValue(object key)
        {
            SetValue(key, null);
        }

        /// <summary>
        /// Adds the specified key/value pair to the table.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value if null nil is being pushed.</param>
        public void SetValue(object key, object value)
        {
            if (key == null)
            { // key must not be null
                throw new ArgumentNullException("key");
            }
            state.Stack.Push(key);
            if (value != null)
            { // Push the value
                state.Stack.Push(value);
            }
            else
            { // Push nil
                state.Stack.Push();
            }
            // And assign it to us
            SetTable();
        }

        /// <summary>
        /// Gets the table. Getting a table retrieves the key from the stack and pushes
        /// the value associated with the key back to the stack.
        /// </summary>
        public void GetTable()
        {
            NativeLua.lua_gettable(state.Handle, index);
        }

        /// <summary>
        /// Retrieves a value key/pair from the table and removes the value from the stack.
        /// </summary>
        /// <param name="key">Key to query.</param>
        /// <returns>The value associated with the key. Null if it's nil or does not exist.</returns>
        public object Pop(object key)
        {
            object o = GetValue(key);
            // Pop it
            state.Stack.Pop();
            return o;
        }

        /// <summary>
        /// Retrieves a value and leaves the value on the stack. If the value is a table,
        /// the index of table is being retrieved.
        /// </summary>
        /// <param name="key">Key to query.</param>
        /// <returns>The value associated with the key.</returns>
        public object GetValue(object key)
        {
            int top = 0;
            object value = null;

            // Push the key on the stack
            state.Stack.Push(key);
            GetTable();
            top = state.Stack.TopIndex;
            if (state.Stack.TypeOf(top) != LuaType.Table)
            {
                // Retrieve it
                value = state.Stack[top];
                // and finally return it
                return value;
            }
            else
            {
                return top;
            }
        }

        /// <summary>
        /// Converts the given table to a dictionary containing all key/values
        /// pairs inside a table.
        /// </summary>
        /// <returns>Dictonary object containing all key/value pairs. from the table.</returns>
        public Dictionary<object, object> ToDictionary()
        {
            Dictionary<object, object> obj = new Dictionary<object, object>();

            // Push nil to mark the start of a traversal
            state.Stack.Push();
            // Then call next
            while (NativeLua.lua_next(state.Handle, index) != 0)
            {
                // Key is at -2 and value at -1
                obj.Add(state.Stack[-2], state.Stack[-1]);
                // Pop the value and keep the key for the next iteration
                state.Stack.Pop();
            }
            Console.WriteLine(state.Stack.ToString());

            return obj;
        }
    }
}
