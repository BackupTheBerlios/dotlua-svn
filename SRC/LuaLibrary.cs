using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace dotLua
{
    /// <summary>
    /// This attribute marks a class to be loaded as a library for LUA. A LUA library is a 
    /// class which contains static methods. Any static method which fits the LuaCallbackFunction delegate
    /// is being associated to a public table named after the class. The version parameter helps
    /// a script to differ between versions of one and the same library.
    /// </summary>
    /// <example>[LuaLibrary("1.0.0")]
    /// class Screen
    /// {
    ///     public static int WriteLine ( IntPtr state )
    ///     {
    ///         // ... code here
    ///     }
    /// }
    /// // Results in a public table "Screen" with a member variable containing a function
    /// // reference to "WriteLine". Thus it can be accessed as:
    /// -- Access inside LUA
    /// Screen.WriteLine(some, arguments);</example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class LuaLibrary : Attribute
    {
        private string version = null;
        private string name = null;
        
        /// <summary>
        /// Marks the given class or struct as a LUA library.
        /// </summary>
        /// <param name="version">The version of the library with the format: Major.Minor.Revision</param>
        public LuaLibrary(string version)
        {
            this.version = version;
            if (!Regex.IsMatch(version, @"(\d+)\.(\d+)\.(\d+)"))
            {
                throw new LuaException("LUA Library version does not have a correct format.");
            }
        }

        /// <summary>
        /// Marks the given class or struct as a LUA library, and associates the given name
        /// with the library. Instead of the class name, the given name will be used to name
        /// the necessary contructs inside LUA.
        /// </summary>
        /// <param name="version">The version of the library with the format: Major.Minor.Revision.</param>
        /// <param name="name">Name of the library inside LUA.</param>
        public LuaLibrary(string version, string name)
        {
            this.version = version;
            if (!Regex.IsMatch(version, @"(\d+)\.(\d+)\.(\d+)"))
            {
                throw new LuaException("LUA Library version does not have a correct format.");
            }
            this.name = name;
        }

        /// <summary>
        /// Retrieves the version of this library.
        /// </summary>
        public string Version
        {
            get
            {
                return version;
            }
        }

        /// <summary>
        /// Retrieves the name of the library (if set).
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }
    }
}
