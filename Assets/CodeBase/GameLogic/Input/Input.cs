using UnityEngine;

namespace CodeBase.GameLogic.Input
{
    public struct Input : INetworkInput
    {
        public Vector2 Move;
        public Vector3 Look;
        public bool JumpTriggered;
        public bool SprintProgress;
    }
}