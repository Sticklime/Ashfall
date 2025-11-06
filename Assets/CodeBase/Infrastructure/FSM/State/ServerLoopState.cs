using System.Linq;
using CodeBase.Infrastructure.ECS;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services.Input;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace CodeBase.Infrastructure.FSM.State
{
    public class ServerLoopState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly IGameFactory _gameFactory;
        private readonly IInputService _inputService;
        private readonly SystemEngine _systemEngine;

        public ServerLoopState(IStateMachine stateMachine, IGameFactory gameFactory, 
            IInputService inputService, SystemEngine systemEngine)
        {
            _stateMachine = stateMachine;
            _gameFactory = gameFactory;
            _inputService = inputService;
            _systemEngine = systemEngine;
        }

        public void Enter()
        {
            Debug.Log("[ServerLoopState] Server loop started");
        }

        public void Exit()
        {
            var serverWorld = World.All.FirstOrDefault(w => w.Name == "ServerWorld");

            if (serverWorld is { IsCreated: true })
            {
                var entityManager = serverWorld.EntityManager;

                var query = entityManager.CreateEntityQuery(typeof(NetworkStreamRequestListen));
                var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);

                foreach (var entity in entities)
                {
                    if (entityManager.Exists(entity))
                        entityManager.DestroyEntity(entity);
                }

                entities.Dispose();
                query.Dispose();

                serverWorld.Dispose();
                Debug.Log("[DOTS NET] Server world disposed");
            }

            _systemEngine?.Dispose();

            Debug.Log("[DOTS NET] Server stopped");
        }
    }
}