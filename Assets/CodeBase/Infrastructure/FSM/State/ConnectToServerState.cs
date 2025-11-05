using System;
using CodeBase.Infrastructure.ECS;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace CodeBase.Infrastructure.FSM.State
{
    public class ConnectToServerState : IState
    {
        private const ushort DefaultPort = 7777;

        private readonly IStateMachine _stateMachine;
        private readonly SystemEngine _systemEngine;
        private World _clientWorld;

        public ConnectToServerState(IStateMachine stateMachine, SystemEngine systemEngine)
        {
            _stateMachine = stateMachine;
            _systemEngine = systemEngine;
        }

        public void Enter()
        {
            ClientServerBootstrap.RequestedPlayType = ClientServerBootstrap.PlayType.Client;
            _clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");

            if (_clientWorld == null || !_clientWorld.IsCreated)
                throw new InvalidOperationException("Failed to create DOTS client world.");

            _systemEngine.RegisterWorld(_clientWorld);
            _systemEngine.SetActiveWorld(_clientWorld);

            var entityManager = _clientWorld.EntityManager;

            var connectEndpoint = NetworkEndpoint.LoopbackIpv4;
            connectEndpoint.Port = DefaultPort;

            var connectEntity = entityManager.CreateEntity(typeof(NetworkStreamRequestConnect));
            entityManager.SetComponentData(connectEntity, new NetworkStreamRequestConnect
            {
                Endpoint = connectEndpoint
            });

            Debug.Log($"[DOTS NET] Client connecting to {connectEndpoint.Address}:{connectEndpoint.Port}");

            _stateMachine.Enter<ClientLoopState>();
        }

        public void Exit()
        {
            // Client lifecycle is managed by ClientLoopState
        }
    }
}