using System;
using System.Collections.Generic;
using System.Text;

namespace dotLua
{
    /// <summary>
    /// Defines some default std library methods.
    /// </summary>
    [LuaLibrary("1.0.0")]
    public static class std
    {
        /// <summary>
        /// Checks if a specified library with the given version is available and failes
        /// if the version is not installed or not present with the given version.
        /// **NOTE**: This method is a quick hack, it is neither efficient nor error perfect.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static int require(IntPtr state)
        {
            LuaFunctionArgs arg = new LuaFunctionArgs(state);
            LuaTable lib = null;
            string library = null;
            string version = null;
            string libversion = null;
            bool result = false;
            int index = 0;
            string error = null;

            if ((arg.ArgumentCount == 0 ||
                arg.ArgumentCount > 2) ||
                (arg.GetArgumentType(0) == LuaType.String &&
                 arg.GetArgumentType(1) == LuaType.String ) )
            { // Invalid arguments
                throw new LuaException("Invalid arguments for require()");
            }
            library = (string)arg[0];
            if (arg.ArgumentCount == 2)
            { // We have also a version to chekc
                version = (string)arg[1];
            }
            try
            {
                index = (int)arg.State.Tables.Global.GetValue(library);
                lib = new LuaTable(arg.State, index);
            }
            catch (Exception)
            {
                error = string.Format("script requires the module {0}.", library);
                throw new LuaException(error);
            }
            if (lib != null)
            { // Library not found
                if (version == null)
                { // Library found, we do not care about a version
                    result = true;
                }
                else
                { // Retrieve the version information
                    libversion = (string)lib.GetValue("__version");
                    result = (libversion == version);
                }
            }
            if (!result)
            {
                if (version == null)
                { // We only require a library
                    error = string.Format("script requires the module {0}.", library);
                    throw new LuaException(error);
                }
                else
                { // And a version
                    error = string.Format("script requires the module {0} with version {1} but version {1} is installed.", library, libversion, version);
                    throw new LuaException(error);
                }
            }
            return arg.EndMethod();
        }
    }
}
