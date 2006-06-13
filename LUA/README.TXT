
What is lua_stdcall?
------------------------------------------------------------------------------

lua_stdcall.dll is a dynamic link library compiled from LUA 5.0.2. I didn't 
modify anything EXCEPT all API routines and the callback definitions have 
changed from __cdecl to __stdcall. To make it compatible with C# delegate
system; which only support __stdcall delegates.

Feel free to redistribute this DLL under the terms of LUA's licence.

**NOTICE**: dotLua REQUIRES this DLL.
**NOTICE**: It has been linked with MSVCR80.DLL, so you also need this
            DLL.
