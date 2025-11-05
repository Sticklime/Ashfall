using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace CodeBase.GameLogic.CustomPhysics
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(BuildPhysicsWorld))]
    public partial struct GroundCheckSystem : ISystem
    {
        private ComponentLookup<LocalToWorld> _transformLookup;
        private BuildPhysicsWorld _buildPhysicsWorld;
        private CollisionWorld _collisionWorld;

        public void OnCreate(ref SystemState state)
        {
            _transformLookup = state.GetComponentLookup<LocalToWorld>(true);
            _buildPhysicsWorld = state.World.GetExistingSystemManaged<BuildPhysicsWorld>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _transformLookup.Update(ref state);
            _collisionWorld = _buildPhysicsWorld.PhysicsWorld.CollisionWorld;
            float deltaTime = SystemAPI.Time.DeltaTime;

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

                if (_collisionWorld.CastRay(rayInput, out Unity.Physics.RaycastHit hit))
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
}
