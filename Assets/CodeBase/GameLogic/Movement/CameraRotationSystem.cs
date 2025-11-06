using CodeBase.GameLogic.Common;
using CodeBase.GameLogic.Input;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace CodeBase.GameLogic.Movement
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class CameraRotationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            var entityManager = EntityManager;

            foreach (var (rotation, input, transform, entity) in
                     SystemAPI.Query<RefRW<CameraRotationComponent>, RefRO<InputComponent>, RefRW<LocalTransform>>()
                         .WithAll<PlayerTag>()
                         .WithEntityAccess())
            {
                if (!entityManager.HasComponent<CameraComponent>(entity))
                    continue;

                var cameraComponent = entityManager.GetComponentData<CameraComponent>(entity);
                Camera camera = cameraComponent.Camera;
                if (camera == null)
                    continue;

                Vector3 lookInput = input.ValueRO.PlayerInput.Look;
                float horizontalRotation = lookInput.x * rotation.ValueRO.Sensitivity * deltaTime;
                float verticalRotation = -lookInput.y * rotation.ValueRO.Sensitivity * deltaTime;

                rotation.ValueRW.VerticalAngle = math.clamp(rotation.ValueRO.VerticalAngle + verticalRotation, -90f, 90f);
                rotation.ValueRW.HorizontalAngle += horizontalRotation;

                camera.transform.localRotation = Quaternion.Euler(rotation.ValueRO.VerticalAngle, 0f, 0f);

                float3 mountEuler = new float3(0f, math.radians(rotation.ValueRO.HorizontalAngle), 0f);
                transform.ValueRW.Rotation = quaternion.EulerXYZ(mountEuler);
            }
        }
    }
}