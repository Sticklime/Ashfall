using System.Linq;
using CodeBase.Infrastructure.ECS;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services.Input;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using VContainer;

namespace CodeBase.Infrastructure.FSM.State
{
    public class ClientLoopState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly IInputService _inputService;
        private readonly IObjectResolver _objectResolver;
        private readonly IGameFactory _gameFactory;
        private readonly SystemEngine _systemEngine;

        public ClientLoopState(
            IStateMachine stateMachine,
            IInputService inputService,
            IObjectResolver objectResolver,
            IGameFactory gameFactory,
            SystemEngine systemEngine)
        {
            _stateMachine = stateMachine;
            _inputService = inputService;
            _objectResolver = objectResolver;
            _gameFactory = gameFactory;
            _systemEngine = systemEngine;
        }

        public void Enter()
        {
            Debug.Log("[ClientLoopState] Client loop started");
        }

        public void Exit()
        {
            var clientWorld = World.All.FirstOrDefault(w => w.Name == "ClientWorld");

            if (clientWorld != null && clientWorld.IsCreated)
            {
                var entityManager = clientWorld.EntityManager;

                var query = entityManager.CreateEntityQuery(typeof(NetworkStreamRequestConnect));
                var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);

                foreach (var entity in entities)
                {
                    if (entityManager.Exists(entity))
                        entityManager.DestroyEntity(entity);
                }

                entities.Dispose();
                query.Dispose();

                clientWorld.Dispose();
                Debug.Log("[DOTS NET] Client world disposed");
            }

            _systemEngine?.Dispose();

            Debug.Log("[DOTS NET] Client disconnected");
        }
    }
}
