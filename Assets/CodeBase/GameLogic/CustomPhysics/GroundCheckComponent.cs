using Scellecs.Morpeh;
using UnityEngine;

namespace CodeBase.GameLogic.CustomPhysics
{
    public struct GroundCheckComponent : IComponent
    {
        public float CheckGroundDistance;
        public bool IsGrounded;
        public Vector3 GroundPoint;
        public LayerMask LayerGround;
    }
}