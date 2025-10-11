using Scellecs.Morpeh;
using UnityEngine;

public struct PhysicsComponent : IComponent
{
    public float Weight;
    public float CheckGroundDistance;
    public bool IsGrounded;
    public LayerMask LayerGround;
    public Vector3 Velocity;
    public bool IsStillTouchingGround;
}