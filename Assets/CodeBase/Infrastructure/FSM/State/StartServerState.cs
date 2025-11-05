using System;
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

        public StartServerState(IStateMachine stateMachine, SystemEngine systemEngine)
        {
            _stateMachine = stateMachine;
            _systemEngine = systemEngine;
        }

        public void Enter()
        {
            if (_serverWorld != null && _serverWorld.IsCreated)
                return;

            ClientServerBootstrap.RequestedPlayType = ClientServerBootstrap.PlayType.Server;
            _serverWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");

            if (_serverWorld == null || !_serverWorld.IsCreated)
                throw new InvalidOperationException("Failed to create DOTS server world.");

            _systemEngine.RegisterWorld(_serverWorld);
            _systemEngine.SetActiveWorld(_serverWorld);

            var entityManager = _serverWorld.EntityManager;

            var listenEndpoint = NetworkEndpoint.AnyIpv4;
            listenEndpoint.Port = DefaultPort;

            var listenEntity = entityManager.CreateEntity(typeof(NetworkStreamRequestListen));
            entityManager.SetComponentData(listenEntity, new NetworkStreamRequestListen
            {
                Endpoint = listenEndpoint
            });

            Debug.Log($"[DOTS NET] Server listening on port {listenEndpoint.Port}");

            _stateMachine.Enter<ServerLoopState>();
        }

        public void Exit()
        {
            // Server lifecycle is managed by ServerLoopState
        }
    }
}
