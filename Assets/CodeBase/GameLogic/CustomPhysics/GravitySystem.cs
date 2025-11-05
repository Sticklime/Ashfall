using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace CodeBase.GameLogic.CustomPhysics
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct GravitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (physics, groundCheck, transform) in
                     SystemAPI.Query<RefRW<PhysicsComponent>, RefRO<GroundCheckComponent>, RefRW<LocalTransform>>())
            {
                if (!groundCheck.ValueRO.IsGrounded)
                {
                    physics.ValueRW.Velocity += math.down() * physics.ValueRO.Weight * deltaTime;
                    transform.ValueRW.Position += physics.ValueRW.Velocity * deltaTime;
                }
                else
                {
                    physics.ValueRW.Velocity = float3.zero;
                    transform.ValueRW.Position = new float3(
                        transform.ValueRO.Position.x,
                        groundCheck.ValueRO.GroundPoint.y + groundCheck.ValueRO.CheckGroundDistance,
                        transform.ValueRO.Position.z
                    );
                }
            }
        }
    }
}