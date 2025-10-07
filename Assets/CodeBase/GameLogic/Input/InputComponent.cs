using Scellecs.Morpeh;
using UnityEngine;

namespace CodeBase.GameLogic.Common
{
    public struct InputComponent : IComponent
    {
        public int OwnerId;
        public Input PlayerInput;
    }

    public struct Input
    {
        public Vector2 Move;
        public Vector2 Look;
        public bool JumpTriggered;
        public bool SprintProgress;
    }
}