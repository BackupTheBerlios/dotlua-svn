using System;
using System.Collections.Generic;
using System.Text;

namespace dotLua
{
    /// <summary>
    /// Thrown if the LUA script has syntax errors.
    /// </summary>
    public sealed class LuaSyntaxException : LuaException
    {
        /// <summary>
        /// Constructs a LuaSyntaxException object with the given message
        /// which details the syntax error.
        /// </summary>
        /// <param name="message">Descriptive error message.</param>
        public LuaSyntaxException(string message)
            : base(message)
        {
        }
    }
}
