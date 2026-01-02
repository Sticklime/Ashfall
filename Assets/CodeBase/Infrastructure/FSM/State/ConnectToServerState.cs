using CodeBase.Infrastructure.ECS;
using Cysharp.Threading.Tasks;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace CodeBase.Infrastructure.FSM.State
{
    public class ConnectToServerState : IState
    {
        private const ushort DefaultPort = 7777;
        private const string DefaultServerAddress = "127.0.0.1";

        private readonly IStateMachine _stateMachine;
        private readonly SystemEngine _systemEngine;
        private World _clientWorld;
        private Entity _clientEntity;

        public ConnectToServerState(IStateMachine stateMachine, SystemEngine systemEngine)
        {
            _stateMachine = stateMachine;
            _systemEngine = systemEngine;
        }

        public void Enter()
        {
            ConnectAsync().Forget();
        }

        private async UniTaskVoid ConnectAsync()
        {
            _clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");
            World.DefaultGameObjectInjectionWorld = _clientWorld;
            _systemEngine.InitializeWorld(_clientWorld);

            var entityManager = _clientWorld.EntityManager;
            var connectEndpoint = NetworkEndpoint.Parse(DefaultServerAddress, DefaultPort);

            _clientEntity = entityManager.CreateEntity(typeof(NetworkStreamRequestConnect));
            entityManager.SetComponentData(_clientEntity, new NetworkStreamRequestConnect
            {
                Endpoint = connectEndpoint
            });

            Debug.Log($"[DOTS NET] Client connecting to {DefaultServerAddress}:{DefaultPort}");
            
            await UniTask.WaitUntil(() =>
                _clientWorld.IsCreated &&
                _clientWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamConnection)).CalculateEntityCount() > 0);

            Debug.Log("[DOTS NET] Client connected");

            _stateMachine.Enter<ClientLoopState>();
        }

        public void Exit()
        {
        }
    }
}
