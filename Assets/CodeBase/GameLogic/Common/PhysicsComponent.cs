using Scellecs.Morpeh;
using UnityEngine;

public struct PhysicsComponent : IComponent
{
    public float Gravity;
    public float CheckGroundDistance;
    public bool IsGrounded;
    public LayerMask LayerGround;
    public Vector3 Velocity;
}