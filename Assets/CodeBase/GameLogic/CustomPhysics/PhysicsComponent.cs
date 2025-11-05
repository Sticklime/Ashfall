using Scellecs.Morpeh;
using Unity.Entities;
using UnityEngine;

namespace CodeBase.GameLogic.CustomPhysics
{
    public struct PhysicsComponent : IComponentData
    {
        public float Weight;
        public Vector3 Velocity;
    }
}