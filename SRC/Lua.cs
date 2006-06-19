using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace dotLua
{
    /// <summary>
    /// The core class of the library. It provides public methods to manipulate a given
    /// LUA state object.
    /// </summary>
    public sealed class Lua : IDisposable
    {
        private IntPtr lua = IntPtr.Zero;

        private LuaCallbackFunction panic = null;
        
        private LuaStack stack = null;
        private LuaTables tables = null;
        private LuaGC gc = null;

        private int handler = 0; // Index of the error handler

        /// <summary>
        /// Constructs a new LUA intepreter state and loads standard
        /// libraries.
        /// </summary>
        public Lua()
        {
            lua = NativeLua.lua_open();
            if (lua == IntPtr.Zero)
            { // Error initializing subsystem
                throw new LuaException("Error initializing LUA subsystem");
            }
            stack = new LuaStack(lua);
            tables = new LuaTables(this);
            gc = new LuaGC(this);
            // Load libraries
            LoadLibraries();
            // Load STD library
            LoadLibrary(typeof(System));
            // Register an error handler
            RegisterErrorHandler();
        }

        /// <summary>
        /// Creates a new LUA intepreter from the given state. The class will not try load the
        /// default libraries for this instance.
        /// </summary>
        /// <param name="state">An existing LUA state</param>
        public Lua(IntPtr state)
        {
            if (state == IntPtr.Zero)
            { // Invalid parameter
                throw new ArgumentException("State must not be zero.", "state");
            }
            lua = state;
            stack = new LuaStack(lua);
            tables = new LuaTables(this);
            gc = new LuaGC(this);
            // Register an error handler
            RegisterErrorHandler();
        }

        /// <summary>
        /// Constructs a new LUA state and loads the given script file.
        /// </summary>
        /// <param name="file">Full qualified path to a script file.</param>
        public Lua(string file)
            : this()
        {
            LoadFile(file);
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
        /// Pushes our own error handler on top of the stack
        /// </summary>
        private void RegisterErrorHandler()
        {
            panic = new LuaCallbackFunction(LuaPanic);
            // Push it and store a value
            handler = Stack.Push(panic);
            NativeLua.lua_atpanic(lua, panic);
        }

        /// <summary>
        /// Called when LUA panics
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private int LuaPanic(IntPtr state)
        {
            if (Stack.TypeOf(1) == LuaType.String)
            {
                string msg = (string)Stack[1];
                // Throw new error message
                throw new LuaSyntaxException(msg + "\r\nStack: " + stack.ToString());
            }
            return 0;
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
        /// Returns the given global variable.
        /// </summary>
        /// <param name="key">Name of the variable.</param>
        /// <returns>The value of the variable or null if nil or it does not exist.</returns>
        public object this[object key]
        {
            get
            {
                return Tables.Global.Pop(key);
            }
            set
            {
                if (value == null)
                { // Set nil
                    Tables.Global.SetValue(key);
                }
                else
                {
                    Tables.Global.SetValue(key, value);
                }
            }
        }

        /// <summary>
        /// This protected method loads the default libraries of LUA
        /// </summary>
        private void LoadLibraries()
        {
            NativeLua.luaopen_base(lua); // Normal functions
            NativeLua.luaopen_debug(lua); // Helps to debug scripts
            NativeLua.luaopen_io(lua); // Basic input/output
            NativeLua.luaopen_math(lua); // mathematical functions
            NativeLua.luaopen_string(lua); // String functions
            NativeLua.luaopen_table(lua); // Table functions
        }

        /// <summary>
        /// Checks a return value from a LUA function for an error
        /// </summary>
        /// <param name="ret"></param>
        private void CheckError ( LuaError ret ) 
        {
            if (ret != LuaError.NoError)
            { // Error
                switch (ret)
                {
                    case LuaError.ErrorRuntime:
                        {
                            throw new LuaException("Runtime error in LUA script.");
                        }

                    case LuaError.ErrorFile:
                        {
                            throw new LuaException("I/O error in LUA subsystem.");
                        }

                    case LuaError.ErrorSyntax:
                        { // Error in syntax
                            string syntax = null;
                            if (Stack.TypeOf(Stack.TopIndex) == LuaType.String)
                            {
                                // Retrieve the value from the string
                                syntax = (string)Stack.TopValue;
                                // Pop it
                                Stack.Pop();
                            }
                            throw new LuaSyntaxException(syntax);
                        }                            

                    case LuaError.ErrorMemory:
                        { // Error in memory
                            throw new LuaException("LUA caused an error in memory allocation.");
                        }                            

                    default:
                        { // Unknown
                            throw new LuaException("Unknown exception from LUA.");
                        }                            
                }                    
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

        /// <summary>
        /// Loads the script from a given string.
        /// </summary>
        /// <param name="buffer">String containing the LUA script to load.</param>
        /// <param name="name">Name of the LUA script junk.</param>
        public void Load(string buffer, string name)
        {
            LuaError ret = LuaError.NoError;

            ret = (LuaError)NativeLua.luaL_loadbuffer(lua, buffer, buffer.Length, name);
            CheckError(ret);
        }
           
        /// <summary>
        /// Loads the script from a given file.
        /// </summary>
        public void LoadFile(string file)
        {
            LuaError ret = LuaError.NoError;

            ret = (LuaError)NativeLua.luaL_loadfile(lua, file);
            CheckError(ret);
        }

        /// <summary>
        /// Starts the execution of the previosly loaded script chunks.
        /// </summary>
        public void Execute()
        {
            Execute(0, null);
        }


        /// <summary>
        /// Executes the script and expects a specified amount of return parameters back.
        /// </summary>
        /// <param name="returnvalues">Number of return values expected.</param>
        public void Execute(int returnvalues)
        {
            Execute(returnvalues, null);
        }

        /// <summary>
        /// Executes the script with the amount of arguements and expects
        /// a specified amount of return parameters back
        /// </summary>
        /// <param name="arguments">Arguments passed to the script.</param>
        /// <param name="returnvalues">Returnvalues which should be pushed on the callers stack.</param>
        public void Execute(int returnvalues, params object[] arguments)
        {
            int argc = 0;
            if (arguments != null)
            { // We do not want arguments
                argc = arguments.Length;
                foreach (object o in arguments)
                {
                    Stack.Push(o);
                }
            }
            // Call the method with the given arguments
            CheckError(NativeLua.lua_pcall(lua, argc, returnvalues, handler));
        }

        /// <summary>
        /// Calls a specified function which is declared inside LUA script.
        /// </summary>
        /// <param name="function">Name of the function.</param>
        public void Call(string function)
        {
            // Call submethod
            Call(function, 0, null);
        }

        /// <summary>
        /// Calls a specified function which is declared inside LUA script.
        /// </summary>
        /// <param name="function">Name of the function.</param>
        /// <param name="returnvalues">Number of return values expected.</param>
        public void Call(string function, int returnvalues)
        {
            Call(function, returnvalues, null);
        }

        /// <summary>
        /// Calls a specified function which is declared inside LUA script.
        /// </summary>
        /// <param name="function">Name of the function.</param>
        /// <param name="returnvalues">Number of return values expected.</param>
        /// <param name="arguments">Parameters that should be passed to the function.</param>
        public void Call(string function, int returnvalues, params object[] arguments)
        {
            object func = null;
            
            func = Tables.Global.GetValue(function);
            if (func == null)
            { // Method does not exist
               throw new LuaException(string.Format("The function '{0}' does not exist in global namespace.", function));
            }
            // Execute it
            Execute(returnvalues, arguments);
            
        }

        /// <summary>
        /// Registers a callback function. Every callback function is being
        /// registered in the global namespace.
        /// </summary>
        /// <param name="name">Name of the function, as it should be available inside the Script</param>
        /// <param name="function">Application defined callback function</param>
        /// <remarks>To register a callback function in the given namespace, create a new
        /// variable holding a table. Then insert the callback function into this table to
        /// retrieve a new namespace.</remarks>
        public void Register(string name, LuaCallbackFunction function)
        {
            if (name == null)
            { // name must not be null
                throw new ArgumentNullException("name");
            }
            if (function == null)
            { // function must not be null
                throw new ArgumentNullException("function");
            }
            // That easy :-)
            Tables.Global.SetValue(name, function);
        }

        /// <summary>
        /// Loads the given file as a library for LUA.
        /// </summary>
        /// <param name="file">Filename or path of the assembly to load.</param> 
        public void LoadLibrary(string file)
        {
            Assembly assembly = Assembly.LoadFrom(file);

            LoadLibrary(assembly);
        }

        /// <summary>
        /// Loads the given assembly as a library for LUA.
        /// </summary>
        /// <param name="assembly">Assembly which should be loaded.</param>
        public void LoadLibrary(Assembly assembly)
        {
            // Check every type in the assembly
            foreach (Type cl in assembly.GetTypes())
            { // Load class
                LoadLibrary(cl);
            }
        } // LoadFrom

        /// <summary>
        /// Loads the given type as a library for LUA.
        /// </summary>
        /// <param name="type">Type of the class to be loaded.</param>
        public void LoadLibrary(Type type)
        {
            LuaTable table = null;
            bool created = false;
            LuaLibrary[] attributes = (LuaLibrary[])type.GetCustomAttributes(typeof(LuaLibrary), false);

            // But only load a class which has our attribute
            // we ignore the other ones.
            if (type.IsClass && 
                attributes.Length > 0 )
            { // ... and check their methods
                foreach (MethodInfo info in type.GetMethods())
                { // Only methods that look like this:
                    // public static int Method ( IntPtr state )
                    if ( info.IsStatic )
                    { // Only static ones
                        LuaCallbackFunction del = null;
                        try
                        {
                            // Create a new LuaCallbackFunction object
                            del = (LuaCallbackFunction)Delegate.CreateDelegate(typeof(LuaCallbackFunction), info);
                        }
                        catch (Exception)
                        { // Throw an error
                            throw new LuaException("Library routine " + info.Name + " does not have the correct signature.");
                        }
                        if (!created)
                        { // The global table correspondending has not been created yet
                            // Push name of the class (for later use)
                            Stack.Push(type.Name);
                            // Create a new empty table
                            table = Tables.Add();
                            // Mark it as created
                            created = true;

                            // Save version.
                            table.SetValue("__version", attributes[0].Version); 
                        }
                        // Register delegate
                        table.SetValue(info.Name, del);
                    }
                } // for each
                // Hopefully we still have the name of the class and the table
                // on top of the stack. If not, well... we are screwed.
                if (created)
                {
                    string stack = Stack.ToString();
                    Tables.Global.SetTable();
                    stack = Stack.ToString();
                }
            } // if
            else
            { // Throw an error
                throw new ArgumentException("Type must be a class or struct and the LuaLibrary custom attribute must be applied to it.", "type");
            }
        }
    }
}
