using Scellecs.Morpeh;
using UnityEngine;

namespace CodeBase.GameLogic.CustomPhysics
{
    public struct PhysicsComponent : IComponent
    {
        public float Weight;
        public Vector3 Velocity;
    }
}