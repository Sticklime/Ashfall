using Fusion;

namespace CodeBase.Infrastructure.FSM.State
{
    public class LoadLevelState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly IGameFactory _gameFactory;
        private readonly NetworkRunner _networkRunner;

        public LoadLevelState(IStateMachine stateMachine, IGameFactory gameFactory, NetworkRunner networkRunner)
        {
            _stateMachine = stateMachine;
            _gameFactory = gameFactory;
            _networkRunner = networkRunner;
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