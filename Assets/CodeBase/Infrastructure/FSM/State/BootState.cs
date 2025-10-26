using CodeBase.Infrastructure.ECS;
using CodeBase.Infrastructure.Services.Input;
using Fusion;

namespace CodeBase.Infrastructure.FSM.State
{
    public class BootState : IState
    {
        private readonly SystemEngine _systemEngine;
        private readonly IStateMachine _stateMachine;
        private readonly IInputService _inputService;
        private readonly NetworkRunner _networkRunner;

        public BootState(SystemEngine systemEngine, IStateMachine stateMachine, IInputService inputService,
            NetworkRunner networkRunner)
        {
            _systemEngine = systemEngine;
            _stateMachine = stateMachine;
            _inputService = inputService;
            _networkRunner = networkRunner;
        }

        public void Enter()
        {
            _inputService.Initialize();
            _systemEngine.Initialize();

#if SERVER
            _stateMachine.Enter<StartServerState>();
#elif CLIENT
            _stateMachine.Enter<ConnectToServerState>();
#endif
        }

        public void Exit()
        {
        }
    }
}