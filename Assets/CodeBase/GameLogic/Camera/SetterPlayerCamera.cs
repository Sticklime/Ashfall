using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic.Camera
{
    public class SetterPlayerCamera : NetworkBehaviour
    {
        [SerializeField] private UnityEngine.Camera _playerCamera;

        public override void Spawned() =>
            UpdateCameraState();

        private void UpdateCameraState() =>
            _playerCamera.enabled = Object.HasInputAuthority;
    }
}