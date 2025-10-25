using Fusion;
using Scellecs.Morpeh;

namespace CodeBase.GameLogic.Movement
{
    public struct CameraComponent : IComponent
    {
        public UnityEngine.Camera Camera;
        public PlayerRef Owned;
    }
}