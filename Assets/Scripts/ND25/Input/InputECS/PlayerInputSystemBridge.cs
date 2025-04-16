// using Unity.Entities;
// using UnityEngine;
// namespace ND25.Input.InputECS
// {
//     public class PlayerInputSystemBridge : MonoBehaviour
//     {
//         private PCControls pcControls;
//         private EntityManager entityManager;
//         private Entity playerEntity;
//         private bool hasInitialized = false;
//
//         private void Awake()
//         {
//             pcControls = new PCControls();
//             entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
//
//             // Đăng ký callback khi world khởi tạo xong hoặc khi scene load xong
//             World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<Unity.Entities.SimulationSystemGroup>()
//                 .AddSystemToUpdateList(new InitPlayerEntitySystem(this));
//         }
//
//         private void OnEnable()
//         {
//             pcControls.GamePlay.Enable();
//         }
//
//         private void OnDisable()
//         {
//             pcControls.GamePlay.Disable();
//         }
//
//
//         void Awake()
//         {
//
//         }
//
//         void Update()
//         {
//             if (!hasInitialized || !moveAction.enabled) return;
//
//             Vector2 moveValue = moveAction.ReadValue<Vector2>();
//
//             var inputData = entityManager.GetComponentData<PlayerInputData>(playerEntity);
//             inputData.moveInput = moveValue;
//             entityManager.SetComponentData(playerEntity, inputData);
//         }
//
//         // Gọi từ hệ thống khởi tạo bên dưới sau khi Entity được tạo ra từ Baker
//         public void SetPlayerEntity(Entity entity)
//         {
//             playerEntity = entity;
//             hasInitialized = true;
//         }
//     }
// }
