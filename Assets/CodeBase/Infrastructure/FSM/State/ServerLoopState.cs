using CodeBase.Infrastructure.ECS;
using Unity.Entities;
using UnityEngine;

namespace CodeBase.Infrastructure.FSM.State
{
    public class ServerLoopState : IState
    {
        private readonly SystemEngine _systemEngine;
        private World _serverWorld;

        public ServerLoopState(SystemEngine systemEngine)
        {
            _systemEngine = systemEngine;
        }

        public void Enter()
        {
            _serverWorld = _systemEngine.ActiveWorld;

            if (_serverWorld == null || !_serverWorld.IsCreated)
            {
                Debug.LogError("[DOTS NET] Server world is not available for loop state.");
                return;
            }

            Debug.Log($"[DOTS NET] Server loop started in world '{_serverWorld.Name}'.");
        }

        public void Exit()
        {
            if (_serverWorld != null && _serverWorld.IsCreated)
            {
                _systemEngine.UnregisterWorld(_serverWorld, disposeWorld: true);
                Debug.Log("[DOTS NET] Server loop stopped.");
            }

            _serverWorld = null;
        }
    }
}