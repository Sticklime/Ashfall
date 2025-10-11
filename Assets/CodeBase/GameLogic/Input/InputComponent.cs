using Fusion;
using Scellecs.Morpeh;
using UnityEngine;

namespace CodeBase.GameLogic.Input
{
    public struct InputComponent : IComponent
    {
        public NetworkInputReceiver NetworkInputReceiver;
        public Input PlayerInput;
    }

    public struct Input : INetworkInput
    {
        public Vector2 Move;
        public Vector3 Look;
        public bool JumpTriggered;
        public bool SprintProgress;
    }
}