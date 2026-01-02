using CodeBase.Infrastructure.ECS;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace CodeBase.Infrastructure.FSM.State
{
    public class StartServerState : IState
    {
        private const ushort DefaultPort = 7777;

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
            _serverWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
            World.DefaultGameObjectInjectionWorld = _serverWorld;
            _systemEngine.InitializeWorld(_serverWorld);

            var entityManager = _serverWorld.EntityManager;

            var listenEndpoint = NetworkEndpoint.AnyIpv4;
            listenEndpoint.Port = DefaultPort;

            _serverEntity = entityManager.CreateEntity(typeof(NetworkStreamRequestListen));
            entityManager.SetComponentData(_serverEntity, new NetworkStreamRequestListen
            {
                Endpoint = listenEndpoint
            });

            Debug.Log($"[DOTS NET] Server started on port {listenEndpoint.Port}");

            _stateMachine.Enter<ServerLoopState>();
        }

        public void Exit()
        {
        
        }
    }
}
