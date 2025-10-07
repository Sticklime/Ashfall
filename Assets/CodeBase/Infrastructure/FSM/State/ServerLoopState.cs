namespace CodeBase.Infrastructure.FSM.State
{
    public class ServerLoopState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly IGameFactory _gameFactory;
        private readonly IInputService _inputService;

        public ServerLoopState(IStateMachine stateMachine, IGameFactory gameFactory, IInputService inputService)
        {
            _stateMachine = stateMachine;
            _gameFactory = gameFactory;
            _inputService = inputService;
        }

        public void Exit()
        {
        }

        public void Enter()
        {
        }
    }
}