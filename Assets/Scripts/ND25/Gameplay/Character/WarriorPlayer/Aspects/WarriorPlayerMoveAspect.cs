﻿using ND25.Gameplay.Character.WarriorPlayer.Component;
using ND25.Input.InputECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
namespace ND25.Gameplay.Character.WarriorPlayer.Aspects
{
    public readonly partial struct WarriorPlayerMoveAspect : IAspect
    {
        public readonly RefRW<LocalTransform> transform;
        public readonly RefRO<MoveData> movementData;
        public readonly RefRO<PlayerInputData> inputData;

        public void Move(float deltaTime)
        {
            float3 move = new float3(x: inputData.ValueRO.moveInput.x, y: 0f, z: inputData.ValueRO.moveInput.y) * movementData.ValueRO.speed * deltaTime;
            transform.ValueRW.Position += move;
        }
    }
}
