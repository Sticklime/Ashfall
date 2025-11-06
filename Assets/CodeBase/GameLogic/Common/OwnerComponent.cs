using Unity.Entities;

namespace CodeBase.GameLogic.Common
{
    public struct OwnerComponent : IComponentData
    {
        public int NetworkId;
    }
}