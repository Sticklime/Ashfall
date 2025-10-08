using CodeBase.GameLogic;
using CodeBase.GameLogic.Input;
using CodeBase.GameLogic.Movement;
using CodeBase.GameLogic.PickUp;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

public class SystemEngine
{
    private readonly World _world;
    private readonly IObjectResolver _resolver;

    private SystemsGroup _gameLogicGroup;

    public SystemEngine(World world, IObjectResolver resolver)
    {
        _world = world;
        _resolver = resolver;
    }

    public void Initialize()
    {
        InitializeSystemGroup();
        InitializeSystem();
    }

    private void InitializeSystemGroup()
    {
        _gameLogicGroup = _world.CreateSystemsGroup();

        _world.AddSystemsGroup(order: 0, _gameLogicGroup);
    }

    private void InitializeSystem()
    {
        _gameLogicGroup.AddSystem(AddSystem<CameraRotationSystem>());
        _gameLogicGroup.AddSystem(AddSystem<CharacterMovementSystem>());
        _gameLogicGroup.AddSystem(AddSystem<SprintSystem>());
        _gameLogicGroup.AddSystem(AddSystem<JumpSystem>());
        _gameLogicGroup.AddSystem(AddSystem<GravitySystem>());
        _gameLogicGroup.AddSystem(AddSystem<InteractSystem>());
        _gameLogicGroup.AddSystem(AddSystem<NetworkInputApplySystem>());

        _world.Commit();
    }

    private TSystem AddSystem<TSystem>() where TSystem : IInitializer =>
        _resolver.CreateScope(builder => builder.Register<TSystem>(Lifetime.Singleton)).Resolve<TSystem>();

    public void Tick() =>
        _world?.Update(Time.deltaTime);

    public void FixedTick() =>
        _world?.FixedUpdate(Time.fixedDeltaTime);

    public void LateTick() =>
        _world?.LateUpdate(Time.deltaTime);

    public void PostTick() =>
        _world?.CleanupUpdate(Time.deltaTime);

    public void Dispose()
    {
        _gameLogicGroup?.Dispose();
        _world?.Dispose();
    }
}