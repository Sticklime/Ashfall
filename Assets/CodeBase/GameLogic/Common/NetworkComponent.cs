using Unity.Entities;

namespace CodeBase.GameLogic.Common
{
    public struct NetworkComponent : IComponentData
    {
        public int NetworkId;
    }
}