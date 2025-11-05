using Unity.Entities;
using UnityEngine;

namespace CodeBase.GameLogic.CustomPhysics
{
    public struct GroundCheckComponent : IComponentData
    {
        public float CheckGroundDistance;
        public bool IsGrounded;
        public Vector3 GroundPoint;
        public LayerMask GroundMask;
    }
}