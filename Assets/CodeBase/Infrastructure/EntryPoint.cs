using CodeBase.Infrastructure.FSM.State;
using CodeBase.Infrastructure.FSM;
using VContainer.Unity;
using System;

public class EntryPoint : IStartable, ITickable, IFixedTickable, ILateTickable, IPostTickable,
    IDisposable
{
    private readonly SystemEngine _systemEngine;
    private readonly IStateMachine _stateMachine;

    public EntryPoint(SystemEngine systemEngine, IStateMachine stateMachine)
    {
        _systemEngine = systemEngine;
        _stateMachine = stateMachine;
    }

    public void Start()
    {
        _stateMachine.RegisterState<BootState>();
        _stateMachine.RegisterState<ConnectToServerState>();
        _stateMachine.RegisterState<StartServerState>();
        _stateMachine.RegisterState<LoadLevelState>();
        _stateMachine.RegisterState<ServerLoopState>();
        _stateMachine.RegisterState<ClientLoopState>();

        _stateMachine.Enter<BootState>();
    }

    public void Tick() =>
        _systemEngine.Tick();

    public void FixedTick() =>
        _systemEngine.FixedTick();

    public void LateTick() =>
        _systemEngine.LateTick();

    public void PostTick() =>
        _systemEngine.PostTick();

    public void Dispose() =>
        _systemEngine.Dispose();
}