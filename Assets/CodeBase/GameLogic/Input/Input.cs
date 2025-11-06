using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace CodeBase.GameLogic.Input
{
    public struct PlayerCommand : ICommandData
    {
        public NetworkTick Tick { get; set; }
        public Vector2 Move;
        public Vector3 Look;
        public bool JumpTriggered;
        public bool SprintProgress;
    }
}