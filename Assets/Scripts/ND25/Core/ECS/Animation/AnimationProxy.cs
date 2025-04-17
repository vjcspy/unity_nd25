using Unity.Entities;
using UnityEngine;
namespace ND25.Core.ECS.Animation
{
    public class AnimationProxy : MonoBehaviour
    {
        private Animator _animator;
        private EntityManager _entityManager;
        public Entity entity;

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _animator = GetComponent<Animator>();
        }

        private void LateUpdate()
        {
            AnimationSyncData animData = _entityManager.GetComponentData<AnimationSyncData>(entity: entity);

            transform.position = animData.position;
            transform.rotation = animData.rotation;
            transform.localScale = new Vector3(x: animData.scale, y: animData.scale, z: animData.scale);

            _animator.SetInteger(name: "state", value: 2);
        }
    }
}
