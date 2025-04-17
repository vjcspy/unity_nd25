using ND25.Gameplay.Character.Common.Component;
using ND25.Gameplay.Character.WarriorPlayer.Component;
using ND25.Input.InputECS;
using Unity.Entities;
using UnityEngine;
namespace ND25.Gameplay.Character.WarriorPlayer
{
    public class WarriorPlayerAuthoring : MonoBehaviour
    {
        [Header(header: "Warrior Player Visual Prefab")]
        [SerializeField] public GameObject playerVisualPrefab;

        [Header(header: "Movement")]
        [SerializeField] public float moveSpeed = 5f;
    }

    public class WarriorPlayerAuthoringBaker : Baker<WarriorPlayerAuthoring>
    {
        public override void Bake(WarriorPlayerAuthoring authoring)
        {
            Entity entity = GetEntity(flags: TransformUsageFlags.Dynamic);
            AddComponent(entity: entity, component: new PlayerInputData());
            AddComponent(entity: entity, component: new MoveData
            {
                speed = authoring.moveSpeed
            });
            AddComponentObject(entity: entity, component: new WarriorPlayerVisualizationRefData
            {
                gameObject = authoring.playerVisualPrefab
            });
            AddComponent<WarriorPlayerTag>(entity: entity);
        }
    }
}
