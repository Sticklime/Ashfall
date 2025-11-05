using Fusion;
using Unity.Entities;

namespace CodeBase.GameLogic.Movement
{
    public class CameraComponent : IComponentData
    {
        public UnityEngine.Camera Camera;
        public PlayerRef Owned;
    }
}