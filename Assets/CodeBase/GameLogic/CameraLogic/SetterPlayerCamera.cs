using UnityEngine;

namespace CodeBase.GameLogic.CameraLogic
{
    public class SetterPlayerCamera : MonoBehaviour
    {
        [SerializeField] private Camera _playerCamera;

        private void Start()
        {
            UpdateCameraState();
        }

        private void UpdateCameraState()
        {
        }
    }
}