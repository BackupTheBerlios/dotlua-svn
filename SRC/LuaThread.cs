using System;
using System.Collections.Generic;
using System.Text;

namespace dotLua
{
    /// <summary>
    /// This class describes a couroutine as used by the LUA library.
    /// </summary>
    public class LuaThread : LuaState
    {

        /// <summary>
        /// Creates a new LuaThread object with a new thread out of the context of the given Lua state.
        /// </summary>
        /// <param name="parent"></param>
        public LuaThread(Lua parent)
        {
            if (parent == null || parent.Handle == IntPtr.Zero)
            { // Error the parent is null
                throw new ArgumentNullException("parent");
            }
            this.lua = NativeLua.lua_newthread(parent.Handle);
        }

        /// <summary>
        /// Creates a new LuaThread object and assigns the given state of an already running
        /// thread to it.
        /// </summary>
        /// <param name="state">State of the running thread.</param>
        public LuaThread(IntPtr state) : base(state)
        {
        }

        /// <summary>
        /// Starts the execution of the given thread, and waits for the LuaThread to finish
        /// it's execution.
        /// </summary>
        /// <param name="arguments">Number of arguments assigned to the body.</param>
        public void Resume(int arguments)
        {
            int retval = 0;

            if (lua == IntPtr.Zero)
            { // Error
                throw new LuaException("The state is zero.");
            }
            retval = NativeLua.lua_resume(lua, arguments);
            if (retval != 0)
            { // The execution returned an error. We can now pop an error
              // message of the stack for further information.
                object msg = Stack.TopValue;

                if (msg is string)
                { // We gotta a well known error.
                    Stack.Pop();
                    // Throw error with the error message from the subsystem
                    throw new LuaException((string)msg);
                }
                else
                { // Throw an unknown error
                    throw new LuaException("The subsystem caused an unknown error.");
                }
            }
        }

        /// <summary>
        /// Calls the routine associated with the thread, and automatically passes the given
        /// arguments to the routine.
        /// </summary>
        /// <param name="arguments">Arguments to pass to the thread routine.</param>
        public void Resume(params object[] arguments)
        {
            if (lua == IntPtr.Zero)
            { // Error the state is zero.
                throw new LuaException("The state is zero.");
            }
            foreach ( object o in arguments )
            { // Push the arguments
                Stack.Push(o);
            }
            Resume(arguments.Length);
        }

        /// <summary>
        /// Stops the execution of the given thread and returns the amount of parameters
        /// specified back to the caller of "Resume()".
        /// </summary>
        /// <param name="results">Number of return values.</param>
        /// <returns>If in a C#-Callback use this as your return value.</returns>
        public int Yield(int results)
        {
            if (lua == IntPtr.Zero)
            { // Error
                throw new LuaException("The state is zero.");
            }
            return NativeLua.lua_yield(lua, results);
        }

        /// <summary>
        /// Stops the execution of the thread and returns the specified objects as results
        /// of the thread.
        /// </summary>
        /// <param name="results"></param>
        /// <returns>If in a C#-Callback use this as your return value.</returns>
        public int Yield(params object[] results)
        {
            if (lua == IntPtr.Zero)
            { // Error
                throw new LuaException("The state is zero.");
            }
            foreach (object o in results)
            { // Push the value
                Stack.Push(o);
            }
            return Yield(results.Length);
        }

        /// <summary>
        /// Pops the amount of values from the stack of the source, and pushes onto the destinations
        /// threads stack. This method can be used to transfer values between threads.
        /// </summary>
        /// <param name="source">Source thread.</param>
        /// <param name="destination">Destination thread.</param>
        /// <param name="count">Number of arguments to copy.</param>
        public static void Move(LuaThread source, LuaThread destination, int count)
        {
            if (source != null || destination != null)
            { // One of those is null.
                throw new ArgumentNullException("source or destination");
            }
            NativeLua.lua_xmove(source.Handle, destination.Handle, count);
        }
    }
}
