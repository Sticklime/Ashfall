using CodeBase.Infrastructure.ECS;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace CodeBase.Infrastructure.FSM.State
{
    public class StartServerState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly SystemEngine _systemEngine;
        private World _serverWorld;
        private Entity _serverEntity;

        public StartServerState(IStateMachine stateMachine, SystemEngine systemEngine)
        {
            _stateMachine = stateMachine;
            _systemEngine = systemEngine;
        }

        public void Enter()
        {
            _systemEngine.Initialize();

            _serverWorld = new World("ServerWorld");
            World.DefaultGameObjectInjectionWorld = _serverWorld;

            ClientServerBootstrap.CreateServerWorld("ServerWorld");
            var networkDriver = new UnityTransport();
            networkDriver.SetConnectionData("0.0.0.0", 7777, "0.0.0.0");

            var networkEntity = _serverWorld.EntityManager.CreateEntity(typeof(NetworkStreamDriver));
            _serverWorld.EntityManager.SetComponentData(networkEntity, new NetworkStreamDriver { Value = networkDriver.Driver });

            _serverEntity = networkEntity;

            var simulation = _serverWorld.GetOrCreateSystemManaged<SimulationSystemGroup>();
            var networkReceive = _serverWorld.GetOrCreateSystemManaged<NetworkStreamReceiveSystemGroup>();
            var ghostSend = _serverWorld.GetOrCreateSystemManaged<GhostSendSystemGroup>();

            simulation.AddSystemToUpdateList(networkReceive);
            simulation.AddSystemToUpdateList(ghostSend);

            Debug.Log("[DOTS NET] Server started on port 7777");

            _stateMachine.Enter<ServerLoopState>();
        }

        public void Exit()
        {
            if (_serverWorld != null && _serverWorld.IsCreated)
            {
                _serverWorld.Dispose();
                _serverWorld = null;
            }

            _systemEngine.Dispose();
            Debug.Log("[DOTS NET] Server stopped");
        }
    }
}
