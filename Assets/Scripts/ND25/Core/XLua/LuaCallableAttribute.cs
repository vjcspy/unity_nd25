using System;
namespace ND25.Core.XLua
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class LuaCallableAttribute : Attribute
    {
        public LuaCallableAttribute()
        {
        }
    }
}