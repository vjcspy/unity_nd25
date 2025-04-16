using ND25.Gameplay.Character.WarriorPlayer.Component;
using ND25.Input.InputECS;
using Unity.Entities;
using UnityEngine;
namespace ND25.Gameplay.Character.WarriorPlayer
{
    public class WarriorPlayerAuthoring : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;

        // Baker để chuyển GameObject thành Entity
        private class WarriorPlayerAuthoringBaker : Baker<WarriorPlayerAuthoring>
        {
            public override void Bake(WarriorPlayerAuthoring authoring)
            {
                Entity entity = GetEntity(flags: TransformUsageFlags.Dynamic);
                AddComponent(entity: entity, component: new PlayerInputData());
                AddComponent(entity: entity, component: new MoveData
                {
                    speed = authoring.moveSpeed
                });
                AddComponent<WarriorPlayerTag>(entity: entity);
            }
        }
    }
}
