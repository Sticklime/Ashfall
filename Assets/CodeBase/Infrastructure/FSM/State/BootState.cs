namespace CodeBase.Infrastructure.FSM.State
{
    public class BootState : IState
    {
        private readonly SystemEngine _systemEngine;
        private readonly IStateMachine _stateMachine;
        private readonly IInputService _inputService;

        public BootState(SystemEngine systemEngine, IStateMachine stateMachine, IInputService inputService)
        {
            _systemEngine = systemEngine;
            _stateMachine = stateMachine;
            _inputService = inputService;
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