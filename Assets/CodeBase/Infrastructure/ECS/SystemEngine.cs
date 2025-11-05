using CodeBase.GameLogic.CustomPhysics;
using CodeBase.GameLogic.Input;
using CodeBase.GameLogic.Movement;
using CodeBase.GameLogic.PickUp;
using Unity.Entities;
using UnityEngine;
using VContainer;

namespace CodeBase.Infrastructure.ECS
{
    public class SystemEngine 
    {
        private World _world;
        private SimulationSystemGroup _simulationGroup;
        private readonly IObjectResolver _resolver;

        public SystemEngine(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public void Initialize()
        {
            _world = new World("GameWorld");
            _simulationGroup = _world.GetOrCreateSystemManaged<SimulationSystemGroup>();

            AddSystem<NetworkInputApplySystem>();
            AddSystem<CameraRotationSystem>();
            AddSystem<CharacterMovementSystem>();
            AddSystem<SprintSystem>();
            AddSystem<JumpSystem>();
            AddSystem<GroundCheckSystem>();
            AddSystem<GravitySystem>();
            AddSystem<InteractSystem>();

            World.DefaultGameObjectInjectionWorld = _world;
        }

        private void AddSystem<T>() where T : SystemBase, new()
        {
            var system = _world.CreateSystemManaged<T>();
            _simulationGroup.AddSystemToUpdateList(system);
        }

        public void Dispose()
        {
            _world?.Dispose();
            _world = null;
        }
    }
}