using Unity.Entities;

namespace CodeBase.GameLogic.Movement
{
    public struct JumpComponent : IComponentData
    {
        public float JumpForce;
    }
}