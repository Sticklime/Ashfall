using System;
using CodeBase.Infrastructure.ECS;
using CodeBase.Infrastructure.FSM;
using CodeBase.Infrastructure.FSM.State;
using VContainer.Unity;

namespace CodeBase.Infrastructure
{
    public class EntryPoint : IStartable
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
            _stateMachine.RegisterState<ServerLoopState>();
            _stateMachine.RegisterState<ClientLoopState>();

            _stateMachine.Enter<BootState>();
        }

        public void Dispose() =>
            _systemEngine.Dispose();
    }
}