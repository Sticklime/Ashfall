using CodeBase.Infrastructure.Factory;

namespace CodeBase.Infrastructure.FSM.State
{
    public class LoadLevelState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly IGameFactory _gameFactory;

        public LoadLevelState(IStateMachine stateMachine, IGameFactory gameFactory)
        {
            _stateMachine = stateMachine;
            _gameFactory = gameFactory;
        }

        public void Enter()
        {
            _stateMachine.Enter<ServerLoopState>();
        }

        public void Exit()
        {
        }
    }
}