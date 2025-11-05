using System;
using System.Collections.Generic;
using CodeBase.GameLogic.CustomPhysics;
using CodeBase.GameLogic.Input;
using CodeBase.GameLogic.Movement;
using CodeBase.GameLogic.PickUp;
using Unity.Entities;

namespace CodeBase.Infrastructure.ECS
{
    public sealed class SystemEngine : IDisposable
    {
        private readonly World _defaultWorld;
        private readonly Dictionary<World, WorldContext> _worldContexts = new();
        private bool _initialized;
        private World _primaryWorld;

        public SystemEngine(World world)
        {
            _defaultWorld = world ?? throw new ArgumentNullException(nameof(world));
        }

        public World World => _defaultWorld;

        public void Initialize()
        {
            if (_initialized)
                return;

            RegisterWorld(_defaultWorld, ownsWorld: false, configureGameplay: true);

            World.DefaultGameObjectInjectionWorld = _defaultWorld;
            _primaryWorld ??= _defaultWorld;
            _initialized = true;
        }

        public void RegisterWorld(World world, bool ownsWorld = true, bool configureGameplay = true)
        {
            if (world == null)
                throw new ArgumentNullException(nameof(world));

            if (!world.IsCreated)
                throw new InvalidOperationException($"World '{world.Name}' has not been created.");

            if (_worldContexts.ContainsKey(world))
                return;

            var context = new WorldContext(world, ownsWorld);
            _worldContexts.Add(world, context);

            if (configureGameplay)
            {
                context.EnsureManagedSystem<NetworkInputApplySystem>();
                context.EnsureManagedSystem<CameraRotationSystem>();
                context.EnsureManagedSystem<CharacterMovementSystem>();
                context.EnsureManagedSystem<SprintSystem>();
                context.EnsureManagedSystem<JumpSystem>();
                context.EnsureManagedSystem<GroundCheckSystem>();
                context.EnsureManagedSystem<GravitySystem>();
                context.EnsureManagedSystem<InteractSystem>();
                context.FlushSystemOrdering();
            }

            _primaryWorld ??= world;
        }

        public bool UnregisterWorld(World world, bool disposeWorld = false)
        {
            if (world == null)
                return false;

            if (!_worldContexts.TryGetValue(world, out var context))
                return false;

            _worldContexts.Remove(world);

            if ((disposeWorld || context.OwnsWorld) && world.IsCreated)
            {
                world.Dispose();
            }

            if (_primaryWorld == world)
            {
                _primaryWorld = _defaultWorld.IsCreated ? _defaultWorld : null;
            }

            return true;
        }

        public void SetActiveWorld(World world)
        {
            if (world == null)
                throw new ArgumentNullException(nameof(world));

            if (!_worldContexts.ContainsKey(world))
                throw new InvalidOperationException($"World '{world.Name}' is not registered in the SystemEngine.");

            _primaryWorld = world;
        }

        public World ActiveWorld => _primaryWorld ?? (_defaultWorld.IsCreated ? _defaultWorld : null);

        public void Tick()
        {
            foreach (var context in _worldContexts.Values)
            {
                context.UpdateInitialization();
                context.UpdateSimulation();
            }
        }

        public void FixedTick()
        {
            foreach (var context in _worldContexts.Values)
            {
                context.UpdateFixedStep();
            }
        }

        public void LateTick()
        {
            foreach (var context in _worldContexts.Values)
            {
                context.UpdateLateSimulation();
            }
        }

        public void PostTick()
        {
            foreach (var context in _worldContexts.Values)
            {
                context.UpdatePresentation();
            }
        }

        public void Dispose()
        {
            foreach (var context in _worldContexts.Values)
            {
                if (context.OwnsWorld && context.World.IsCreated)
                {
                    context.World.Dispose();
                }
            }

            _worldContexts.Clear();
        }

        private sealed class WorldContext
        {
            private readonly World _world;
            private readonly InitializationSystemGroup _initializationGroup;
            private readonly SimulationSystemGroup _simulationGroup;
            private readonly FixedStepSimulationSystemGroup _fixedStepGroup;
            private readonly LateSimulationSystemGroup _lateSimulationGroup;
            private readonly PresentationSystemGroup _presentationGroup;
            private bool _simulationOrderDirty;

            public WorldContext(World world, bool ownsWorld)
            {
                _world = world;
                OwnsWorld = ownsWorld;

                _initializationGroup = world.GetOrCreateSystemManaged<InitializationSystemGroup>();
                _simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
                _fixedStepGroup = world.GetOrCreateSystemManaged<FixedStepSimulationSystemGroup>();
                _lateSimulationGroup = world.GetOrCreateSystemManaged<LateSimulationSystemGroup>();
                _presentationGroup = world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            }

            public World World => _world;
            public bool OwnsWorld { get; }

            public void EnsureManagedSystem<TSystem>() where TSystem : ComponentSystemBase
            {
                if (_simulationGroup == null || !_world.IsCreated)
                    return;

                if (_world.GetExistingSystemManaged<TSystem>() != null)
                    return;

                var system = _world.GetOrCreateSystemManaged<TSystem>();
                _simulationGroup.AddSystemToUpdateList(system);
                _simulationOrderDirty = true;
            }

            public void FlushSystemOrdering()
            {
                if (_simulationOrderDirty && _simulationGroup != null)
                {
                    _simulationGroup.SortSystemUpdateList();
                    _simulationOrderDirty = false;
                }
            }

            public void UpdateInitialization()
            {
                if (!_world.IsCreated || _initializationGroup == null)
                    return;

                using (new WorldScope(_world))
                {
                    _initializationGroup.Update();
                }
            }

            public void UpdateSimulation()
            {
                if (!_world.IsCreated || _simulationGroup == null)
                    return;

                using (new WorldScope(_world))
                {
                    _simulationGroup.Update();
                }
            }

            public void UpdateFixedStep()
            {
                if (!_world.IsCreated || _fixedStepGroup == null)
                    return;

                using (new WorldScope(_world))
                {
                    _fixedStepGroup.Update();
                }
            }

            public void UpdateLateSimulation()
            {
                if (!_world.IsCreated || _lateSimulationGroup == null)
                    return;

                using (new WorldScope(_world))
                {
                    _lateSimulationGroup.Update();
                }
            }

            public void UpdatePresentation()
            {
                if (!_world.IsCreated || _presentationGroup == null)
                    return;

                using (new WorldScope(_world))
                {
                    _presentationGroup.Update();
                }
            }
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