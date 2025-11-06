using Unity.Entities;

namespace CodeBase.GameLogic.Input
{
    public struct InputComponent : IComponentData
    {
        public PlayerCommand PlayerInput;
    }
}