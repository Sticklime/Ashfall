using System;
using CodeBase.GameLogic.CustomPhysics;
using CodeBase.GameLogic.Input;
using CodeBase.GameLogic.Movement;
using CodeBase.GameLogic.PickUp;
using Unity.Entities;

namespace CodeBase.Infrastructure.ECS
{
    public class SystemEngine : IDisposable
    {
        private readonly World _world;
        private SimulationSystemGroup _simulationGroup;
        private FixedStepSimulationSystemGroup _fixedStepGroup;
        private LateSimulationSystemGroup _lateSimulationGroup;
        private PresentationSystemGroup _presentationGroup;

        public SystemEngine(World world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
        }

        public World World => _world;

        public void Initialize()
        {
            _simulationGroup = _world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            _fixedStepGroup = _world.GetOrCreateSystemManaged<FixedStepSimulationSystemGroup>();
            _lateSimulationGroup = _world.GetOrCreateSystemManaged<LateSimulationSystemGroup>();
            _presentationGroup = _world.GetOrCreateSystemManaged<PresentationSystemGroup>();

            AddSystemToSimulation<NetworkInputApplySystem>();
            AddSystemToSimulation<CameraRotationSystem>();
            AddSystemToSimulation<CharacterMovementSystem>();
            AddSystemToSimulation<SprintSystem>();
            AddSystemToSimulation<JumpSystem>();
            AddSystemToSimulation<GroundCheckSystem>();
            AddSystemToSimulation<GravitySystem>();
            AddSystemToSimulation<InteractSystem>();

            _simulationGroup.SortSystemUpdateList();

            World.DefaultGameObjectInjectionWorld = _world;
        }

        private void AddSystemToSimulation<T>() where T : ComponentSystemBase
        {
            var system = _world.GetOrCreateSystemManaged<T>();
            _simulationGroup.AddSystemToUpdateList(system);
        }

        public void Tick()
        {
            if (_simulationGroup == null)
                return;

            using (new WorldScope(_world))
            {
                _simulationGroup.Update();
            }
        }

        public void FixedTick()
        {
            if (_fixedStepGroup == null)
                return;

            using (new WorldScope(_world))
            {
                _fixedStepGroup.Update();
            }
        }

        public void LateTick()
        {
            if (_lateSimulationGroup == null)
                return;

            using (new WorldScope(_world))
            {
                _lateSimulationGroup.Update();
            }
        }

        public void PostTick()
        {
            if (_presentationGroup == null)
                return;

            using (new WorldScope(_world))
            {
                _presentationGroup.Update();
            }
        }

        public void Dispose()
        {
            _world?.Dispose();
        }

        private readonly struct WorldScope : IDisposable
        {
            private readonly World _previousWorld;

            public WorldScope(World target)
            {
                _previousWorld = World.DefaultGameObjectInjectionWorld;
                World.DefaultGameObjectInjectionWorld = target;
            }

            public void Dispose()
            {
                World.DefaultGameObjectInjectionWorld = _previousWorld;
            }
        }
    }
}