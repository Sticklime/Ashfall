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
            _systemEngine.Initialize();

            _serverWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
            World.DefaultGameObjectInjectionWorld = _serverWorld;

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
            if (_serverWorld != null && _serverWorld.IsCreated)
            {
                if (_serverEntity != Entity.Null && _serverWorld.EntityManager.Exists(_serverEntity))
                {
                    _serverWorld.EntityManager.DestroyEntity(_serverEntity);
                    _serverEntity = Entity.Null;
                }

                _serverWorld.Dispose();
                _serverWorld = null;
            }

            _systemEngine.Dispose();
            Debug.Log("[DOTS NET] Server stopped");
        }
    }
}
