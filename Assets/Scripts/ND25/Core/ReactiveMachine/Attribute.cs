using System;

namespace ND25.Core.ReactiveMachine
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class ReactiveMachineEffectAttribute : Attribute
    {
        // public ReactiveMachineEffectAttribute(params string[] types)
        // {
        //     Types = types ?? Array.Empty<string>();
        // }
        //
        // // Khai báo Types với giá trị mặc định là mảng rỗng nếu không truyền tham số
        // public string[] Types { get; }
    }
}