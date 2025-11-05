using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace CodeBase.GameLogic.Movement
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct CameraRotationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (rotation, input, cameraTransform, mountTransform) in
                     SystemAPI.Query<RefRW<CameraRotationComponent>, RefRO<InputComponent>, RefRW<CameraTransform>, RefRW<MountTransform>>())
            {
                float2 lookInput = input.ValueRO.Look;
                float horizontalRotation = lookInput.x * rotation.ValueRO.Sensitivity * deltaTime;
                float verticalRotation = -lookInput.y * rotation.ValueRO.Sensitivity * deltaTime;

                rotation.ValueRW.VerticalAngle = math.clamp(rotation.ValueRO.VerticalAngle + verticalRotation, -90f, 90f);
                rotation.ValueRW.HorizontalAngle += horizontalRotation;

                float3 cameraEuler = new float3(rotation.ValueRO.VerticalAngle, 0f, 0f);
                float3 mountEuler = new float3(0f, rotation.ValueRO.HorizontalAngle, 0f);

                cameraTransform.ValueRW.Rotation = quaternion.EulerXYZ(math.radians(cameraEuler));
                mountTransform.ValueRW.Rotation = quaternion.EulerXYZ(math.radians(mountEuler));
            }
        }
    }
}