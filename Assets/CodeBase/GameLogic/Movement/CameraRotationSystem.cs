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

            foreach (var (rotation, input, cameraComponent, transform) in
                     SystemAPI.Query<RefRW<CameraRotationComponent>, RefRO<InputComponent>, ManagedAPI<CameraComponent>, RefRW<LocalTransform>>()
                         .WithAll<PlayerTag>())
            {
                Vector3 lookInput = input.ValueRO.PlayerInput.Look;
                float horizontalRotation = lookInput.x * rotation.ValueRO.Sensitivity * deltaTime;
                float verticalRotation = -lookInput.y * rotation.ValueRO.Sensitivity * deltaTime;

                rotation.ValueRW.VerticalAngle = math.clamp(rotation.ValueRO.VerticalAngle + verticalRotation, -90f, 90f);
                rotation.ValueRW.HorizontalAngle += horizontalRotation;

                Camera camera = cameraComponent.Value.Camera;
                if (camera != null)
                {
                    camera.transform.localRotation = Quaternion.Euler(rotation.ValueRO.VerticalAngle, 0f, 0f);
                }

                float3 mountEuler = new float3(0f, math.radians(rotation.ValueRO.HorizontalAngle), 0f);
                transform.ValueRW.Rotation = quaternion.EulerXYZ(mountEuler);
            }
        }
    }
}