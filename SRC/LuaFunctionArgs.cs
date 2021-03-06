using System;
using System.Collections.Generic;
using System.Text;

namespace dotLua
{
    /// <summary>
    /// A class which sums up high level functionality for callback routines.
    /// </summary>
    public sealed class LuaFunctionArgs
    {
        private Array args = null;
        private List<object> ret = new List<object>();
        private LuaState state = null;
        private LuaStack stack = null;

        /// <summary>
        /// Constructs a new LuaFunctionArgs object from the given LUA state. You obtain
        /// this state via the parameter of your callback routine.
        /// </summary>
        /// <param name="state">State as given through the callback parameter.</param>
        public LuaFunctionArgs(IntPtr state)
        {
            if (IntPtr.Zero == state)
            { // Shall not be zero
                throw new ArgumentException("state must not be zero");
            }
            this.state = new Lua(state);
            stack = this.state.Stack;

            // Construct argument list
            //
            int argc = stack.TopIndex;
            List<object> obj = new List<object>();

            for (int i = 1; i < argc; ++i)
            {
                obj.Add(stack[i]);
                // stack.Pop();
            }
            args = obj.ToArray();
        }

        /// <summary>
        /// Finalizes a LUA callback routine by pushing the return values on the stack and
        /// returns the amount of return values.
        /// </summary>
        /// <returns>This return value shall also be returned by the callback.</returns>
        public int EndMethod()
        {
            foreach (object o in ret)
            { // Push value
                stack.Push(o);
            }
            return ret.Count;
        }

        /// <summary>
        /// Returns the Lua stack associated with the callback.
        /// </summary>
        public LuaStack Stack
        {
            get
            {
                return stack;
            }
        }

        /// <summary>
        /// Returns the LUA object associated with the callback.
        /// </summary>
        public LuaState State
        {
            get
            {
                return state;
            }
        }

        /// <summary>
        /// Returns a global array containing all arguments for the function.
        /// </summary>
        public Array Arguments
        {
            get
            {
                return args;
            }
        }

        /// <summary>
        /// Returns the number of arguments
        /// </summary>
        public int ArgumentCount
        {
            get
            {
                return args.Length;
            }
        }


        /// <summary>
        /// Retrieves the argument with the given index.
        /// </summary>
        /// <param name="index">Index of the parameter to retrieve.</param>
        /// <returns>The value of the parameter.</returns>
        public object this[int index]
        {
            get
            {
                return Arguments.GetValue(index);
            }
        }

        /// <summary>
        /// Returns the type of the argument specified via the index.
        /// </summary>
        /// <param name="index">Index of argument to check.</param>
        /// <returns>The type of the argument.</returns>
        public LuaType GetArgumentType(int index)
        {
            return stack.TypeOf(index);
        }

        /// <summary>
        /// Sets or retrieves a list of objects that can be filled with return values.
        /// </summary>
        public List<object> ReturnValues
        {
            get
            {
                return ret;
            }
            set
            {
                if (value == null)
                { // If null is passed then we should
                    // just create an empty one
                    ret = new List<object>();
                }
                else
                { // Else assign it
                    ret = value;
                }
            }
        }
    }
}
