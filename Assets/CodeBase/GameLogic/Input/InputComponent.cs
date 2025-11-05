using System.ComponentModel;

namespace CodeBase.GameLogic.Input
{
    public struct InputComponent : IComponent
    {
        public NetworkInputReceiver NetworkInputReceiver;
        public Input PlayerInput;
    }
}