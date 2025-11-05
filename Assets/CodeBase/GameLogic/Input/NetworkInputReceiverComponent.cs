using Unity.Entities;

namespace CodeBase.GameLogic.Input
{
    public struct NetworkInputReceiverComponent : IComponentData
    {
        public Input PlayerInput;
    }
}
