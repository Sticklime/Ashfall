using Unity.Entities;

namespace CodeBase.GameLogic.Common
{
    public struct NetworkComponent : IComponentData
    {
        public PlayerRef Owner;
    }
}