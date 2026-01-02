using System;
using CodeBase.GameLogic.CustomPhysics;
using CodeBase.GameLogic.Movement;
using CodeBase.GameLogic.PickUp;
using Unity.Entities;
using Unity.NetCode;
using VContainer;

namespace CodeBase.Infrastructure.ECS
{
    public class SystemEngine : IDisposable
    {
        private readonly World _world;
        private readonly IObjectResolver _resolver;

        public SystemEngine(World world, IObjectResolver resolver)
        {
            _world = world;
            _resolver = resolver;
        }

        public void Initialize()
        {
            InitializeWorld(_world);
            World.DefaultGameObjectInjectionWorld = _world;
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(_world);
        }

        public void InitializeWorld(World world)
        {
            if (world == null || !world.IsCreated)
                return;

            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();

            AddSystem<CameraRotationSystem>(world, simulationGroup);
            AddSystem<CharacterMovementSystem>(world, simulationGroup);
            AddSystem<SprintSystem>(world, simulationGroup);
            AddSystem<JumpSystem>(world, simulationGroup);
            AddSystem<GroundCheckSystem>(world, simulationGroup);
            AddSystem<GravitySystem>(world, simulationGroup);
            AddSystem<InteractSystem>(world, simulationGroup);

            var ghostSimulationGroup = world.GetExistingSystemManaged<GhostSimulationSystemGroup>();

            AddSystem<PlayerSpawnSystem>(world, ghostSimulationGroup);
        }

        private void AddSystem<TSystem>(World world, ComponentSystemGroup group)
            where TSystem : ComponentSystemBase
        {
            if (world.GetExistingSystemManaged<TSystem>() != null)
                return;

            using var scope = _resolver.CreateScope(b => b.Register<TSystem>(Lifetime.Singleton));
            TSystem system = scope.Resolve<TSystem>();

            world.AddSystemManaged(system);
            group.AddSystemToUpdateList(system);
        }


        public void Dispose()
        {
            if (_world != null && _world.IsCreated)
                _world.Dispose();
        }
    }
}