using Scellecs.Morpeh;

namespace CodeBase.GameLogic.Input
{
    public struct InputNetworkComponent : IComponent
    {
        public int OwnerId;
        public Common.Input PlayerInput;
    }
}