using Unity.Entities;

namespace CodeBase.GameLogic.Movement
{
    public struct MoveComponent : IComponentData
    {
        public float Speed;
        public float SpeedBase;
        public float SprintSpeed;
    }
}