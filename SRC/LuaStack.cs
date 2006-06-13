using System;
using System.Collections.Generic;
using System.Text;

namespace dotLua
{
    /// <summary>
    /// Implements a LUA stack which can be manipulated
    /// </summary>
    public sealed class LuaStack
    {
        private IntPtr lua = IntPtr.Zero;

        /// <summary>
        /// Constructs a new LUA stack from the given state.
        /// </summary>
        /// <param name="state">A valid LUA state</param>
        public LuaStack(IntPtr state)
        {
            if (state == IntPtr.Zero)
            { // Invalid state passed
                throw new ArgumentException("state must not be Zero", "state");
            }
            lua = state;
        }

        /// <summary>
        /// Creates a new  LUA stack from the given state and automatically grows it.
        /// </summary>
        /// <param name="state">A valid LUA state</param>
        /// <param name="grow">Automatically grows the stack</param>
        public LuaStack(IntPtr state, int grow)
            : this(state)
        {
            Grow(grow);
        }

        /// <summary>
        /// Sets or retrieves the index of the top level item.
        /// </summary>
        public int TopIndex
        {
            get
            {
                return NativeLua.lua_gettop(lua);
            }
            set
            {
                NativeLua.lua_settop(lua, value);
            }
        }

        /// <summary>
        /// Retrieves the value which is on top of the stack without
        /// removing it.
        /// </summary>
        public object TopValue
        {
            get
            {
                return this[TopIndex];
            }
        }

        /// <summary>
        /// Retrieves the amount of items on the stack.
        /// </summary>
        public int Count
        {
            get
            { // Return this value
                return TopIndex;
            }
        }

        /// <summary>
        /// Grows the stack of "size" items.
        /// </summary>
        /// <param name="size">Elements to add</param>
        public void Grow(int size)
        {
            if (NativeLua.lua_checkstack(lua, size) == 0)
            { // Throw on allocation failure
                throw new LuaException("Not enough memory to grow stack");
            }
        }

        /// <summary>
        /// Removes a specified amount of elements from the top of the stack.
        /// </summary>
        public void Pop(int items)
        {
            if (TopIndex > 0)
            {
                TopIndex = -(items) - 1;
            }
        }

        /// <summary>
        /// Removes one element from the stack.
        /// </summary>
        public void Pop()
        {
            Pop(1);
        }

        /// <summary>
        /// Clears the entire stack.
        /// </summary>
        public void Clear()
        {
            TopIndex = 0;
        }

        /// <summary>
        /// Moves the top item into the given position.
        /// </summary>
        /// <param name="index">Position</param>
        public void Insert(int index)
        {
            NativeLua.lua_insert(lua, index);
        }

        /// <summary>
        /// Pushes onto the stack a copy of the element at the given index.
        /// </summary>
        /// <param name="index">The items index which should be copied.</param>
        public void PushValue(int index)
        {
            NativeLua.lua_pushvalue(lua, index);
        }

        /// <summary>
        /// Replaces the given item with the top element.
        /// </summary>
        /// <param name="index">Index to replace</param>
        public void Replace(int index)
        {
            NativeLua.lua_replace(lua, index);
        }

        /// <summary>
        /// Removes the given object and shifts the items on top down to fill the gap.
        /// </summary>
        /// <param name="index">Item to be removed</param>
        public void Remove(int index)
        {
            NativeLua.lua_remove(lua, index);
        }

        /// <summary>
        /// Retrieves the type of the given stack item.
        /// </summary>
        /// <param name="index">Index of the item</param>
        /// <returns>The type of the item</returns>
        public LuaType TypeOf(int index)
        {
            LuaType res = 0;

            res = (LuaType)NativeLua.lua_type(lua, index);
            if (res == LuaType.None)
            { // Nothing, invalid index
                throw new IndexOutOfRangeException("Invalid index for TypeOf query.");
            }
            // Return our result
            return res;
        }

        /// <summary>
        /// Compares two stack items if they are equal.
        /// </summary>
        /// <param name="index1">First index to compare</param>
        /// <param name="index2">Second index to compare</param>
        /// <param name="raw">If true only primitive comparision is being done without using metainformation</param>
        /// <returns>True if both are equal</returns>
        public bool Equal(int index1, int index2, bool raw)
        {
            int res = 0;

            if (!raw)
            { // normal compare
                res = NativeLua.lua_equal(lua, index1, index2);
            }
            else
            { // raw compare
                res = NativeLua.lua_rawequal(lua, index1, index2);
            }
            return (res != 0);
        }

        /// <summary>
        /// Compares two stack items if they are equal.
        /// </summary>
        /// <param name="index1">First index to compare</param>
        /// <param name="index2">Second index to compare</param>
        /// <returns>True if both are equal</returns>
        public bool Equal(int index1, int index2)
        {
            return Equal(index1, index2, false);
        }

        /// <summary>
        /// Retrieves a given item from the stack.
        /// </summary>
        /// <param name="index">Index of the item to be retrieved</param>
        /// <returns>Value of the item</returns>
        public object this[int index]
        {
            get
            {
                LuaType type = TypeOf(index);
                object value = null;

                if (type == LuaType.None)
                { // Invalid type
                    throw new IndexOutOfRangeException("Index does not exist or has type LUA_NONE");
                }
                switch (type)
                {
                    case LuaType.Boolean:
                        { // A boolean
                            value = (bool)(NativeLua.lua_toboolean(lua, index) != 0);
                        }
                        break;

                    case LuaType.Nil:
                        { // Nil this means, null
                            value = null;
                        }
                        break;

                    case LuaType.Number:
                        { // A ordinary number :)
                            value = (NativeLua.lua_tonumber(lua, index));
                        }
                        break;

                    case LuaType.String:
                        { // A string
                            value = (NativeLua.lua_tostring(lua, index));
                        }
                        break;

                    case LuaType.Function:
                        { // A function
                            value = (NativeLua.lua_tocfunction(lua, index));
                        }
                        break;

                    case LuaType.UserData:
                        { // User data
                            value = (NativeLua.lua_touserdata(lua, index));
                        }
                        break;

                    case LuaType.Thread:
                        {
                            value = new Lua(NativeLua.lua_tothread(lua, index));
                        }
                        break;

                    case LuaType.LightUserData:
                    case LuaType.Table:
                        { // I may event a table object some fine day
                            // But for now we even take the table into the pointers
                            value = (NativeLua.lua_topointer(lua, index));
                        }
                        break;

                    default:
                        { // Error... Unknown object
                            throw new LuaException("Unknown object");
                        }
                }
                return value;
            }
        }

        /// <summary>
        /// Pushes a nil value on top of the stack.
        /// </summary>
        /// <returns>Returns the index of the newly pushed item.</returns>
        public int Push()
        {
            NativeLua.lua_pushnil(lua);
            return TopIndex;
        }

        /// <summary>
        /// Pushes the given string on top of the stack.
        /// </summary>
        /// <param name="value">Value to push, if null nil is being pushed.</param>
        /// <returns>Returns the index of the newly pushed item.</returns>
        public int Push(object value)
        {
            if (value == null)
            { // push nil
                return Push();
            }
            if (value.GetType() == typeof(string))
            {
                NativeLua.lua_pushstring(lua, (string)value);
            }
            else if (value.GetType() == typeof(double) ||       // Normal doubles
                     value.GetType() == typeof(float) ||        // single precision
                     value.GetType() == typeof(int) ||          // Int32
                     value.GetType() == typeof(long) ||         // Int64
                     value.GetType() == typeof(short) ||        // Int16
                     value.GetType() == typeof(uint) ||         // UInt23 ;D... just kidding
                     value.GetType() == typeof(ushort) ||       // UInt16
                     value.GetType() == typeof(ulong) ||        // UInt64
                     value.GetType() == typeof(byte))          // __int8 :-)
            {
                NativeLua.lua_pushnumber(lua, Convert.ToDouble(value));
            }
            else if (value.GetType() == typeof(bool))
            {
                NativeLua.lua_pushboolean(lua, (int)value);
            }
            else if (value.GetType() == typeof(LuaCallbackFunction))
            {
                NativeLua.lua_pushcfunction(lua, (LuaCallbackFunction)value);
            }
            else if (value.GetType() == typeof(IntPtr))
            {
                NativeLua.lua_pushlightuserdata(lua, (IntPtr)value);
            }
            else
            { // Error
                throw new ArgumentException("value's type is not supported by the LUA subsystem", "value");
            }
            return TopIndex;
        }

        /// <summary>
        /// Concenates the count values at the top of the stack, pops them, and leaves the result at the top.
        /// </summary>
        /// <param name="count">Number of items to concat</param>
        public void Concat(int count)
        {
            NativeLua.lua_concat(lua, count);
        }

#if DEBUG
        /// <summary>
        /// Converts the entire stack to a human read able form. This is used to debug
        /// the stack.
        /// </summary>
        /// <returns>Returns the stack as string</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("[");
            for (int i = 1; i <= TopIndex; ++i)
            {
                if (TypeOf(i) == LuaType.String ||
                    TypeOf(i) == LuaType.Boolean ||
                    TypeOf(i) == LuaType.Number ||
                    TypeOf(i) == LuaType.Nil)
                {
                    builder.Append(String.Format(" {0}", (this[i] != null ? this[i] : "(nil)")));
                }
                else if (TypeOf(i) == LuaType.Table)
                {
                    builder.Append(" (Table)");
                }
                else if (TypeOf(i) == LuaType.Function)
                {
                    builder.Append(" (Function)");
                }
                else
                {
                    builder.Append(" (Unknown)");
                }
                if (i != TopIndex)
                { // Append a comma
                    builder.Append(",");
                }
            }
            builder.Append("]");

            return builder.ToString();
        }
#endif
    }
}
