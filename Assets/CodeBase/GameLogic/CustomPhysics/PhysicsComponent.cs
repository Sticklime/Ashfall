using Unity.Entities;
using Unity.Mathematics;

namespace CodeBase.GameLogic.CustomPhysics
{
    public struct PhysicsComponent : IComponentData
    {
        public float Weight;
        public float3 Velocity;
    }
}