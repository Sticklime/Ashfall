using CodeBase.Infrastructure.ECS;
using Unity.Entities;
using UnityEngine;

namespace CodeBase.Infrastructure.FSM.State
{
    public class ClientLoopState : IState
    {
        private readonly SystemEngine _systemEngine;
        private World _clientWorld;

        public ClientLoopState(SystemEngine systemEngine)
        {
            _systemEngine = systemEngine;
        }

        public void Enter()
        {
            _clientWorld = _systemEngine.ActiveWorld;

            if (_clientWorld == null || !_clientWorld.IsCreated)
            {
                Debug.LogError("[DOTS NET] Client world is not available for loop state.");
                return;
            }

            Debug.Log($"[DOTS NET] Client loop started in world '{_clientWorld.Name}'.");
        }

        public void Exit()
        {
            if (_clientWorld != null && _clientWorld.IsCreated)
            {
                _systemEngine.UnregisterWorld(_clientWorld, disposeWorld: true);
                Debug.Log("[DOTS NET] Client loop stopped.");
            }

            _clientWorld = null;
        }
    }
}