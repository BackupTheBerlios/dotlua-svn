using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace dotLua
{
    /// <summary>
    /// This attribute marks a class to be loaded as a library for LUA.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class LuaLibrary : Attribute
    {
        private string version = null;
        
        /// <summary>
        /// Marks the given class or struct as a LUA library.
        /// </summary>
        /// <param name="version">The version of the library.</param>
        public LuaLibrary(string version)
        {
            this.version = version;
            if (!Regex.IsMatch(version, @"(\d+)\.(\d+)\.(\d+)"))
            {
                throw new LuaException("LUA Library version does not have a correct format.");
            }
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
    }
}
