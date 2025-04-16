using Unity.Entities;
using Unity.Mathematics;
namespace ND25.Input.InputECS
{
    public struct PlayerInputData : IComponentData
    {
        public float2 moveInput;
    }
}
