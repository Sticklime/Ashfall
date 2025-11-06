using CodeBase.GameLogic.CustomPhysics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(BuildPhysicsWorld))]
public partial class GroundCheckSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<PhysicsWorldSingleton>();
    }

    protected override void OnUpdate()
    {
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
        var collisionWorld = physicsWorld.CollisionWorld;
        
        foreach (var (groundCheck, transform) in
                 SystemAPI.Query<RefRW<GroundCheckComponent>, RefRO<LocalTransform>>())
        {
            float3 origin = transform.ValueRO.Position + new float3(0, 0.1f, 0);
            float3 direction = math.down();
            float maxDistance = groundCheck.ValueRO.CheckGroundDistance + 0.1f;

            var rayInput = new RaycastInput
            {
                Start = origin,
                End = origin + direction * maxDistance,
                Filter = CollisionFilter.Default
            };

            if (collisionWorld.CastRay(rayInput, out var hit))
            {
                groundCheck.ValueRW.IsGrounded = true;
                groundCheck.ValueRW.GroundPoint = hit.Position;
            }
            else
            {
                groundCheck.ValueRW.IsGrounded = false;
                groundCheck.ValueRW.GroundPoint = float3.zero;
            }

#if UNITY_EDITOR
            Color color = groundCheck.ValueRO.IsGrounded ? Color.green : Color.red;
            Debug.DrawRay(origin, direction * groundCheck.ValueRO.CheckGroundDistance, color);
#endif
        }
    }
}
