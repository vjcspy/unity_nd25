using System;

namespace Core.XLua
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class LuaCallableAttribute : Attribute
    {
        public LuaCallableAttribute()
        {
        }
    }
}