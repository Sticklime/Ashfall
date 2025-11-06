using System;
using CodeBase.GameLogic.CustomPhysics;
using CodeBase.GameLogic.Movement;
using CodeBase.GameLogic.PickUp;
using Unity.Entities;
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
            World.DefaultGameObjectInjectionWorld = _world;
            var simulationGroup = _world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            
            AddSystem<CameraRotationSystem>(simulationGroup);
            AddSystem<CharacterMovementSystem>(simulationGroup);
            AddSystem<SprintSystem>(simulationGroup);
            AddSystem<JumpSystem>(simulationGroup);
            AddSystem<GroundCheckSystem>(simulationGroup);
            AddSystem<GravitySystem>(simulationGroup);
            AddSystem<InteractSystem>(simulationGroup);
            AddSystem<PlayerSpawnSystem>(simulationGroup);

            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(_world);
        }

        private void AddSystem<T>(ComponentSystemGroup group) where T : ComponentSystemBase
        {
            T system = _resolver.Resolve<T>();
            _world.AddSystemManaged(system);
            group.AddSystemToUpdateList(system);
        }

        public void Dispose()
        {
            if (_world != null && _world.IsCreated)
                _world.Dispose();
        }
    }
}