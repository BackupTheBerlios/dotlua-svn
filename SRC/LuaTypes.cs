using System;
using System.Collections.Generic;
using System.Text;

namespace dotLua
{
    /// <summary>
    /// A callback function from LUA to C#.
    /// </summary>
    /// <param name="state">Current state of the caller.</param>
    /// <returns>Should return the number of return values.</returns>
    public delegate int LuaCallbackFunction(IntPtr state);

    /// <summary>
    /// Available types in LUA.
    /// </summary>
    public enum LuaType
    {
        /// <summary>
        /// Nothing or invalid type.
        /// </summary>
        None = -1,
        /// <summary>
        /// A nil value. Like null in C#.
        /// </summary>
        Nil = 0,
        /// <summary>
        /// true/false boolean.
        /// </summary>
        Boolean = 1,
        /// <summary>
        /// Light user data, not supported yet.
        /// </summary>
        LightUserData = 2,
        /// <summary>
        /// A double precision floating point number.
        /// </summary>
        Number = 3,
        /// <summary>
        /// A null terminated character array, aka C string.
        /// </summary>
        String = 4,
        /// <summary>
        /// A table.
        /// </summary>
        Table = 5,
        /// <summary>
        /// A callback function.
        /// </summary>
        Function = 6,
        /// <summary>
        /// User data, not supported yet.
        /// </summary>
        UserData = 7,
        /// <summary>
        /// A coroutine, not supported yet.
        /// </summary>
        Thread = 8
    }

    /// <summary>
    /// Special tables like global or local environment.
    /// </summary>
    public enum SpecialTables
    {
        /// <summary>
        /// Global namespace table.
        /// </summary>
        Global = -10001,
        /// <summary>
        /// A registry which can be used to store internal values.
        /// </summary>
        Registry = -10000  
    }

    /// <summary>
    /// Lua errors as returned by call and pcall
    /// </summary>
    public enum LuaError
    {
        /// <summary>
        /// No error occured.
        /// </summary>
        NoError = 0,
        /// <summary>
        /// Runtime error.
        /// </summary>
        ErrorRuntime = 1,
        /// <summary>
        /// Error reading file.
        /// </summary>
        ErrorFile = 2,
        /// <summary>
        /// Syntax error.
        /// </summary>
        ErrorSyntax = 3,
        /// <summary>
        /// Memory error; e.g. not enough memory.
        /// </summary>
        ErrorMemory = 4,
        /// <summary>
        /// Unknown internal error.
        /// </summary>
        ErrorUnknown = 5
    }
}
