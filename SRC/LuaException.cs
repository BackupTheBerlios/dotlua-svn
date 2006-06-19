using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace dotLua
{
    /// <summary>
    /// Thrown if an error occures in the LUA subsystem. Any other
    /// exception thrown by dotLUA derives from this class.
    /// </summary>
    public class LuaException : ApplicationException
    {
        /// <summary>
        /// Constructs an empty LuaException object.
        /// </summary>
        public LuaException() 
            : base()
        {
        }

        /// <summary>
        /// Constructs a LuaException object with the given error message.
        /// </summary>
        /// <param name="message">A descriptive error message.</param>
        public LuaException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructs a LuaException with the given error message and a reference 
        /// to an exception which caused this exception.
        /// </summary>
        /// <param name="message">A descriptive error message.</param>
        /// <param name="innerException">An exception which caused this exception.</param>
        public LuaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
