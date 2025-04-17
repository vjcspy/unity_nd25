using Unity.Entities;
using UnityEngine;
namespace ND25.Core.ECS.Animation
{
    public class AnimationProxy : MonoBehaviour
    {
        private Animator _animator;
        private EntityManager _entityManager;

        private Vector3 _lastPosition;
        private Quaternion _lastRotation;
        private Vector3 _lastScale;
        private int _lastState = -1;
        public Entity entity;

        private void Awake()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _animator = GetComponent<Animator>();
        }

        private void LateUpdate()
        {
            AnimationSyncData animData = _entityManager.GetComponentData<AnimationSyncData>(entity: entity);

            Vector3 newPosition = animData.position;
            Quaternion newRotation = animData.rotation;
            Vector3 newScale = new Vector3(x: animData.scale, y: animData.scale, z: animData.scale);
            int newState = 2; // Có thể đổi thành animData.animationState nếu bạn thêm

            // Chỉ set nếu khác để tránh overhead
            if (newPosition != _lastPosition)
            {
                transform.position = newPosition;
                _lastPosition = newPosition;
            }

            if (newRotation != _lastRotation)
            {
                transform.rotation = newRotation;
                _lastRotation = newRotation;
            }

            if (newScale != _lastScale)
            {
                transform.localScale = newScale;
                _lastScale = newScale;
            }

            if (_lastState != newState)
            {
                _animator.SetInteger(name: "state", value: newState);
                _lastState = newState;
            }
        }
    }
}
