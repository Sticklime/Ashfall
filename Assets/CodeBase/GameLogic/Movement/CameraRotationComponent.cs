using Unity.Entities;

namespace CodeBase.GameLogic.Movement
{
    public struct CameraRotationComponent : IComponentData
    {
        public float Sensitivity;
        public float VerticalAngle;
        public float HorizontalAngle;
    }
}